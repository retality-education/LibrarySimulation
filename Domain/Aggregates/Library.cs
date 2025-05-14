using LibrarySimulation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrarySimulation.Domain.Services;
using LibrarySimulation.Infrastructure.Helpers;
using LibrarySimulation.Core.Enums;
using LibrarySimulation.Domain.Services.Factories;
using System.Diagnostics;
using LibrarySimulation.Domain.Entities.Persons;
using LibrarySimulation.Core.Interfaces;
using LibrarySimulation.Core;

namespace LibrarySimulation.Domain.Aggregates
{
    // Класс, представляющий библиотеку
    internal class Library
    {
        // Список наблюдателей, которые будут получать уведомления о событиях в библиотеке
        private List<IObserver> _observers = new();

        // Список публикаций в библиотеке
        public List<LibraryPublication> Publications { get; } = new();

        // Список библиотекарей
        public List<Librarian> Librarians { get; } = new();

        // Текущая дата в библиотеке
        public DateTime today { get; set; } = new DateTime(2025, 1, 1);

        // Количество потерянных публикаций
        public int CountOfLostPublications { get; set; } = 0;

        // Количество доступных публикаций
        public int CountOfAvailablePublications { get; set; } = 0;

        // Проверяет, содержится ли публикация в библиотеке
        public bool isLibraryContainsPublication(Publication publication)
        {
            return Publications.Select(x => x.Publication).Contains(publication);
        }
        #region Добавление Публикаций, Копий, Работников
        // Метод для добавления новой публикации в библиотеку
        public void AddNewPublication(Publication publication, int count = 0)
        {
            var temp = new LibraryPublication(publication); // Создание новой библиотеки публикации
            temp.AddCopiesOfPublication(count); // Добавление копий публикации

            // Обновление количества доступных публикаций с блокировкой для потокобезопасности
            lock (SyncHelper.ChangeCountOfAvailablePublications)
            {
                CountOfAvailablePublications += count; // Увеличение количества доступных публикаций
                Notify(LibraryEvents.CountOfAvailablePublicationsChanged, CountOfAvailablePublications); // Оповещение об изменении количества доступных публикаций
            }
            // Добавление публикации в список
            Publications.Add(temp);
        }

        // Метод для добавления копий существующей публикации
        public void AddCopiesOfPublication(Publication publication, int count)
        {
            Publications.First(x => x.Publication == publication).AddCopiesOfPublication(count); // Находим публикацию и добавляем копии
        }

        // Метод для добавления нового библиотекаря
        public void AddLibrarian(string name)
        {
            var temp = LibraryFactory.CreateLibrarian(name, this); // Создание нового библиотекаря
            Librarians.Add(temp); // Добавление библиотекаря в список
            Notify(LibraryEvents.CreateWorker, WorkerID: temp.Id); // Оповещение о создании работника
        }
        #endregion

        #region Взаимодействия читателя с библиотекой
        // Метод для получения наименее загруженного библиотекаря
        private Librarian GetLeastBusyLibrarian()
        {
            return Librarians.OrderBy(l => l.ReaderQueue.Count).First(); // Сортировка библиотекарей по количеству читателей в очереди
        }

        // Метод, вызываемый, когда читатель приходит в библиотеку
        public void ReaderComeToLibrary(Reader reader)
        {
            // Уведомление о том, пришел ли читатель с книгой или без
            if (reader.Requests.Peek().RequestType is RequestType.Return)
                Notify(LibraryEvents.ReaderComeToLibraryWithBook, reader.Id);
            else
                Notify(LibraryEvents.ReaderComeToLibraryWithoutBook, reader.Id);

            // Имитация времени, необходимого для прихода в библиотеку
            Thread.Sleep(TimingConsts.TimeToGoToLibrary);

            // Получение наименее загруженного библиотекаря
            Librarian worker = GetLeastBusyLibrarian();
            Notify(LibraryEvents.ReaderJoinedQueue, reader.Id, WorkerID: worker.Id); // Уведомление о том, что читатель присоединился к очереди
            Thread.Sleep(TimingConsts.TimeToTakePlaceInQueue + 300); // Имитация времени ожидания в очереди

            // Добавление читателя в очередь к библиотекарю
            worker.ReaderQueue.Enqueue(reader);
        }
        #endregion

        #region Взаимодействие рабочего с библиотекой
        // Метод, вызываемый, когда библиотекарь выдает книгу читателю
        public void WorkerTookBookInLibrary(Publication publication, int readerId, int workerId, DateTime today)
        {
            lock (SyncHelper.ChangeInLibrary) // Блокировка для потокобезопасного изменения данных библиотеки
            {
                var temp = Publications.First(x => x.Publication == publication);
                temp.owners[readerId] = today; // Запись даты, когда читатель взял книгу
                temp.AvailableCopies--; // Уменьшение доступного количества копий публикации
            }
            lock (SyncHelper.ChangeCountOfAvailablePublications) // Блокировка для изменения общего счёта доступных публикаций
            {
                CountOfAvailablePublications--; // Уменьшение общего количества доступных публикаций
                Notify(LibraryEvents.CountOfAvailablePublicationsChanged, CountOfAvailablePublications); // Оповещение об изменении количества
            }
            Notify(LibraryEvents.WorkerTookBookInLibrary, WorkerID: workerId); // Уведомление наблюдателей о выдаче книги
        }

        // Метод, вызываемый, когда библиотекарь принимает книгу обратно от читателя
        internal void WorkerReturnBookToLibrary(Publication publication, int readerId, int workerId)
        {
            lock (SyncHelper.ChangeInLibrary) // Потокобезопасное изменение данных библиотеки
            {
                var temp = Publications.First(x => x.Publication == publication);
                lock (SyncHelper.ChangeCountOfLostPublications)
                {
                    // Проверка, превышен ли срок заемного периода, что может считать книгу утерянной
                    if (temp.isBookOverBorrowedByPerson(today, readerId))
                    {
                        CountOfLostPublications--; // Уменьшаем количество потерянных публикаций
                        Notify(LibraryEvents.CountOfLostPublicationsChanged, CountOfLostPublications); // Оповещение о снижении счетчика утерянных книг
                    }
                }
                lock (SyncHelper.ChangeCountOfAvailablePublications)
                {
                    CountOfAvailablePublications++; // Увеличиваем количество доступных публикаций
                    Notify(LibraryEvents.CountOfAvailablePublicationsChanged, CountOfAvailablePublications); // Оповещение об изменении количества доступных книг
                }
                temp.owners.Remove(readerId); // Удаляем запись о владельце книги
                temp.AvailableCopies++; // Увеличиваем число доступных копий публикации
            }
            Notify(LibraryEvents.WorkerReturnedBookToLibrary, WorkerID: workerId); // Уведомление о возврате книги
        }
        #endregion

        #region Оповещения о событиях
        // Позволяет подписать наблюдателя на события библиотеки
        public void Subscribe(IObserver observer)
        {
            lock (SyncHelper.ObserveLock) // Синхронизация списка наблюдателей
            {
                _observers.Add(observer);
            }
        }

        // Позволяет отписать наблюдателя от событий
        public void Unsubcribe(IObserver observer)
        {
            lock (SyncHelper.ObserveLock) // Синхронизация списка наблюдателей
            {
                _observers.Remove(observer);
            }
        }

        // Уведомляет всех подписанных наблюдателей о наступлении события библиотеки
        public void Notify(LibraryEvents libraryEvent, int ReaderID = 0, int WorkerID = 0)
        {
            lock (SyncHelper.ObserveLock) // Синхронизация доступа к списку наблюдателей
            {
                foreach (var observer in _observers)
                {
                    observer.OnLibraryEvent(libraryEvent, ReaderID, WorkerID); // Вызов метода обработки события у каждого наблюдателя
                }
            }
        }
        #endregion
    }
}

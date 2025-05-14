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
using LibrarySimulation.Domain.Entities.Publications;

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

        private Random _random = new Random();
        private int _readersCount;
        private List<Reader> _allReaders = new();

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

        #region init library
        //заполнение библиотеки публикациями
        private void FillLibraryWithPublications()
        {
            var libraryPublications = new List<Publication>
            {
                new Book { Title = "Война и мир", Author = "Лев Толстой", Year = 1869, Theme = Theme.Literature },
                new Book { Title = "Преступление и наказание", Author = "Фёдор Достоевский", Year = 1866, Theme = Theme.Literature },
                new Journal { Title = "National Geographic", Author = "Various", Year = 2023, Theme = Theme.Science },
                new Textbook { Title = "Основы программирования", Author = "Д. Кнут", Year = 2020, Theme = Theme.Technology },
                new Thesis { Title = "Квантовая механика", Author = "А. Эйнштейн", Year = 1924, Theme = Theme.Science },
                new Book { Title = "1984", Author = "Джордж Оруэлл", Year = 1949, Theme = Theme.Literature },
                new Journal { Title = "Nature", Author = "Various", Year = 2023, Theme = Theme.Science },
                new Textbook { Title = "Анатомия человека", Author = "И. Павлов", Year = 2018, Theme = Theme.Medicine },
                new Thesis { Title = "История Древнего Рима", Author = "М. Ростовцев", Year = 1918, Theme = Theme.History },
                new Book { Title = "Мастер и Маргарита", Author = "Михаил Булгаков", Year = 1967, Theme = Theme.Literature },
                new Journal { Title = "Forbes", Author = "Various", Year = 2023, Theme = Theme.Technology },
                new Textbook { Title = "Основы химии", Author = "Д. Менделеев", Year = 1869, Theme = Theme.Science },
                new Thesis { Title = "Теория относительности", Author = "А. Эйнштейн", Year = 1905, Theme = Theme.Science },
                new Book { Title = "Гарри Поттер и философский камень", Author = "Дж. К. Роулинг", Year = 1997, Theme = Theme.Literature },
                new Journal { Title = "Science", Author = "Various", Year = 2023, Theme = Theme.Science },
                new Textbook { Title = "История Средних веков", Author = "Л. Гумилёв", Year = 1980, Theme = Theme.History },
                new Thesis { Title = "Искусственный интеллект", Author = "А. Тьюринг", Year = 1950, Theme = Theme.Technology },
                new Book { Title = "Тихий Дон", Author = "Михаил Шолохов", Year = 1940, Theme = Theme.Literature },
                new Journal { Title = "The Lancet", Author = "Various", Year = 2023, Theme = Theme.Medicine },
                new Textbook { Title = "Физика для вузов", Author = "Р. Фейнман", Year = 1963, Theme = Theme.Science },
                new Thesis { Title = "Кибернетика", Author = "Н. Винер", Year = 1948, Theme = Theme.Technology },
                new Book { Title = "Анна Каренина", Author = "Лев Толстой", Year = 1877, Theme = Theme.Literature },
                new Journal { Title = "Time", Author = "Various", Year = 2023, Theme = Theme.History },
                new Textbook { Title = "Биология клетки", Author = "Б. Албертс", Year = 2002, Theme = Theme.Medicine },
                new Thesis { Title = "Квантовая электродинамика", Author = "Р. Фейнман", Year = 1949, Theme = Theme.Science }
            };

            foreach (var publication in libraryPublications)
                AddNewPublication(publication, _random.Next(1, 5));

        }
        private void InitializeLibrarians()
        {
            AddLibrarian("Анна Ивановна");
            AddLibrarian("Петр Сергеевич");
        }

        // Определение количества читателей в зависимости от сезона
        private int GetReadersCountForSeason()
        {
            int month = today.Month;

            // Осень (сентябрь-ноябрь) - больше читателей
            if (month >= 9 && month <= 11)
            {
                return _random.Next(5, 10);
            }
            // Лето (июнь-август) - меньше читателей
            else if (month >= 6 && month <= 8)
            {
                return _random.Next(1, 4);
            }
            // Весна (март-май) - среднее количество
            else if (month >= 3 && month <= 5)
            {
                return _random.Next(3, 7);
            }
            // Зима (декабрь-февраль) - среднее количество
            else
            {
                return _random.Next(2, 6);
            }
        }
        // Определение типа запроса в зависимости от сезона
        private RequestType GetRequestTypeBasedOnSeason(int readerId)
        {
            int month = today.Month;
            bool hasBorrowedBooks = Publications
                            .Where(x => x.owners.ContainsKey(readerId))
                            .ToList()
                            .Count > 0;

            if (!hasBorrowedBooks) return RequestType.Take;

            // Осень - чаще берут
            if (month >= 9 && month <= 11)
                return _random.Next(10) < 7 ? RequestType.Take : RequestType.Return;
            // Лето - чаще возвращают
            else if (month >= 6 && month <= 8)
                return _random.Next(10) < 3 ? RequestType.Take : RequestType.Return;
            // Другие сезоны - 50/50
            else
                return _random.Next(2) == 0 ? RequestType.Take : RequestType.Return;
        }


        private Reader GetOrCreateReader()
        {
            //ищем неактивных читателей
            var allNonActiveReaders = _allReaders.Where(x => !x.isReaderActive).ToList();

            if (allNonActiveReaders.Count > 0 && _random.Next(10) < 3)
            {
                return allNonActiveReaders[_random.Next(allNonActiveReaders.Count)];
            }

            //если таких нет создаем новых 
            _readersCount++;
            var newReader = new Reader($"Читатель_{_readersCount}");
            _allReaders.Add(newReader);
            return newReader;
        }

        //создание нового читателя
        private Reader GenerateNewReader()
        {
            var reader = GetOrCreateReader();
            reader.Requests = new PriorityQueue<Request, int>();


            // Получаем список взятых книг, если нет - создаем
            var borrowedBooks = Publications
                .Where(x => x.owners.ContainsKey(reader.Id))
                .Select(x => x.Publication)
                .ToList();
            //генерируем случайное количество запросов
            int requestCount = _random.Next(1, 4);

            //по каждому запросу
            for (int i = 0; i < requestCount; i++)
            {
                //получаем тип запроса на основе сезонности
                RequestType requestType = GetRequestTypeBasedOnSeason(reader.Id);
                Publication selectedPub;

                //выбираем случайную книгу либо на возврат либо на взятие
                if (requestType == RequestType.Return && borrowedBooks.Count > 0)
                    selectedPub = borrowedBooks[_random.Next(borrowedBooks.Count)];
                else
                    selectedPub = Publications[_random.Next(Publications.Count)].Publication;

                //добавление нового запроса читателя в его очередь
                reader.Requests.Enqueue(new Request(requestType, selectedPub), (int)requestType);
            }

            return reader;
        }
        #endregion

        #region library start logic
        //запуск
        public void Start()
        {
            new Thread(StartSimulation).Start();
        }

        //основной метод симуляции
        private void StartSimulation()
        {
            //заполняем библиотеку публикациями
            FillLibraryWithPublications();
            //инициализируем библиотекарей
            InitializeLibrarians();
            //запуск фоновой задачи пополнения библиотеки книгами
            Task.Run(() => refillLibrary());

            //запускаем основной бесконечный цикл
            while (true)
            {
                // изменяем дату на 15 дней и уведомляем об изменениях
                today = today.AddDays(15);
                Notify(LibraryEvents.DateChanged, today.Year * 10000 + today.Month * 100 + today.Day);

                lock (SyncHelper.ChangeCountOfLostPublications)
                {
                    //сохраняем количество потерянных публикаций
                    var lastCount = CountOfLostPublications;
                    //пересчитываем потерянные
                    CountOfLostPublications = Publications
                                                            .Select(x => x.CountOfMissingBooks(today))
                                                            .Where(x => x > 0)
                                                            .Sum();
                    //если значение изменилось уведомляем об этом
                    if (lastCount != CountOfLostPublications)
                        Notify(LibraryEvents.CountOfLostPublicationsChanged, CountOfLostPublications);
                }
                //количество читателей для генерации основываясь на сезоне
                int readersToGenerate = GetReadersCountForSeason();

                //создаем нужное количество читателей
                for (int i = 0; i < readersToGenerate; i++)
                {
                    var reader = GenerateNewReader();
                    if (reader.Requests.Count > 0)
                    {
                        ReaderComeToLibrary(reader);
                    }
                }

                // Имитация работы библиотеки (читатели приходят и уходят)
                Thread.Sleep(TimingConsts.TimeBetweenDays); // Пауза между днями
            }
        }

        //пополнение библиотеки
        private void refillLibrary()
        {
            while (true)
            {
                //пауза на 25 сек
                Thread.Sleep(25000);

                //формируем список публикаций, у которых не хватает экземпляров на данный момент
                var temp = Publications
                            .Select(x => (x, x.CountOfMissingBooks(today)))
                            .Where(x => x.Item2 > 0)
                            .ToList();

                if (temp.Count > 0)
                {
                    Notify(LibraryEvents.LibraryRefilled);
                    //для каждой публикации
                    foreach (var x in temp)
                    {
                        lock (SyncHelper.ChangeCountOfAvailablePublications)
                        {
                            //добавляем недостающие копии
                            x.Item1.AddCopiesOfPublication(x.Item2);
                            CountOfAvailablePublications += x.Item2;
                            Notify(LibraryEvents.CountOfAvailablePublicationsChanged, CountOfAvailablePublications);
                        }
                    }
                }
            }
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

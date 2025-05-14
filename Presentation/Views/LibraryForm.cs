using LibrarySimulation.Core.Enums;
using LibrarySimulation.Domain.Aggregates;
using LibrarySimulation.Domain.Entities;
using LibrarySimulation.Domain.Services.Factories;
using LibrarySimulation.Domain.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using LibrarySimulation.Presentation.Controllers;
using LibrarySimulation.Domain.Entities.Persons;
using System.Reflection.PortableExecutable;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Collections.Concurrent;
using LibrarySimulation.Core;

namespace LibrarySimulation.Presentation.Views
{
    // Класс, представляющий форму библиотеки, реализующий интерфейс ILibraryView
    internal partial class LibraryForm : Form, ILibraryView
    {
        public LibraryController Controller;

        // Потокобезопасные коллекции для управления движением читателей и библиотекарей
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _readerMovements = new();
        private readonly ConcurrentDictionary<int, CancellationTokenSource> _workerMovements = new();
        private readonly ConcurrentDictionary<int, int> _heightOfLibrarians = new();
        private readonly ConcurrentDictionary<int, int> _idFreePositionForReader = new(); //id_worker -> id_position(_queueXPositions)
        private readonly ConcurrentDictionary<int, PictureBox> _readers = new();
        private readonly ConcurrentDictionary<int, PictureBox> _librarians = new();
        private readonly ConcurrentDictionary<int, PictureBox> _librarianAnswers = new();
        private readonly ConcurrentDictionary<int, PictureBox> _readerAnswers = new();
        private readonly ConcurrentDictionary<int, DateTime> _lastLibrarianAnswerChange = new();
        private readonly ConcurrentDictionary<int, DateTime> _lastReaderAnswerChange = new();
        private readonly System.Windows.Forms.Timer _dialogueResetTimer = new();


        // Координаты и блокировки
        private readonly List<int> _queueXPositions = new() { 620, 780, 940, 1100 }; // Позиции для очереди читателей
        private readonly object _syncNextPosition = new(); // Блокировка для синхронизации доступа к позициям
        private readonly object _createWorkerLock = new(); // Блокировка для создания библиотекарей
        private readonly Point _libraryLocation = new(107, 217); // Позиция библиотеки на форме


        // Компоненты формы
        private readonly List<PictureBox> _answerLibrarianPictures; // Картинки ответов библиотекарей
        private readonly List<PictureBox> _answerReaderPictures; // Картинки ответов читателей
        private readonly List<PictureBox> _workersPictures; // Картинки библиотекарей

        private int _heightForNextLibrarian = 120; // Высота для следующего библиотекаря
        private int _xForLibrarians = 400; // X-координата для библиотекарей
        private int _xForNextReader = 620; // X-координата для следующего читателя
        private int _idLibrarian = 0; // Идентификатор библиотекаря

        // Конструктор формы
        public LibraryForm()
        {
            InitializeComponent(); // Инициализация компонентов формы
            _answerLibrarianPictures = new() { LibrarianAnswer1, LibrarianAnswer2 }; // Инициализация картинок ответов библиотекарей
            _answerReaderPictures = new() { ReaderAnswer1, ReaderAnswer2 }; // Инициализация картинок ответов читателей
            _workersPictures = new() { Librarian1, Librarian2 }; // Инициализация картинок библиотекарей
            _dialogueResetTimer.Interval = 200; // Установка интервала таймера для сброса старых диалогов
            _dialogueResetTimer.Tick += ResetOldDialogues; // Подписка на событие таймера
            _dialogueResetTimer.Start(); // Запуск таймера
        }


        #region Movement Logic
        // Асинхронный метод для перемещения PictureBox по оси Y
        private async Task MoveToY(PictureBox pictureBox, int targetY, CancellationToken token, int durationMs = 500)
        {
            try
            {
                if (token.IsCancellationRequested)
                {
                    // Если отменено, мгновенно перемещаем в конечную позицию по Y
                    this.InvokeIfRequired(() => pictureBox.Top = targetY);
                    return;
                }

                int startY = pictureBox.Top; // Начальная позиция по Y
                float distance = targetY - startY; // Расстояние перемещения
                int steps = Math.Max(1, durationMs / 16); // Количество шагов (~60 FPS)
                float stepY = distance / steps; // Размер шага перемещения
                for (int i = 1; i <= steps; i++)
                {
                    if (token.IsCancellationRequested)
                    {
                        // При отмене во время перемещения мгновенно позиционируем объект
                        this.InvokeIfRequired(() => pictureBox.Top = targetY);
                        return;
                    }
                    int newY = startY + (int)(stepY * i);
                    this.InvokeIfRequired(() => pictureBox.Top = newY);

                    try
                    {
                        await Task.Delay(16, token); // Задержка между шагами анимации с поддержкой отмены
                    }
                    catch (OperationCanceledException)
                    {
                        // При отмене - также сразу позиционируем объект
                        this.InvokeIfRequired(() => pictureBox.Top = targetY);
                        return;
                    }
                    catch { /* Игнорируем другие ошибки */ }
                }

                // Финализация позиции
                this.InvokeIfRequired(() => pictureBox.Top = targetY);
            }
            catch
            {
                // Гарантируем установку конечной позиции даже при ошибках
                this.InvokeIfRequired(() => pictureBox.Top = targetY);
            }
        }

        // Асинхронный метод для перемещения PictureBox по оси X с анимацией
        private async Task MoveToX(PictureBox pictureBox, int targetX, CancellationToken token, int durationMs = 500)
        {
            try
            {
                if (token.IsCancellationRequested)
                    return; // Если операция отменена, прерываем
                int startX = pictureBox.Left; // Начальная позиция по X
                float distance = targetX - startX;
                int steps = Math.Max(1, durationMs / 16);  // Количество шагов анимации
                float stepX = distance / steps;


                for (int i = 1; i <= steps; i++)
                {
                    if (token.IsCancellationRequested)
                        break;
                    int newX = startX + (int)(stepX * i);

                    this.InvokeIfRequired(() => pictureBox.Left = newX);
                    try
                    {
                        await Task.Delay(16, token);
                    }
                    catch (Exception ex) { }
                }

                // Финализация позиции
                this.InvokeIfRequired(() => pictureBox.Left = targetX);
            }
            catch (Exception) {}
        }

        // Отмена движения читателя
        private void CancelReaderMovement(int readerId)
        {
            if (_readerMovements.TryRemove(readerId, out var cts))
            {
                try
                {
                    cts.Cancel(); // Отменяем токен отмены, что прервет текущую анимацию движения
                }
                catch (ObjectDisposedException) { /* Игнорируем если токен уже освобожден */ }
            }
        }
        #endregion
        private void ResetOldDialogues(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var resetTime = TimeSpan.FromSeconds(1); // 1 секунда

            // Сбрасываем старые ответы библиотекаря
            foreach (var kvp in _lastLibrarianAnswerChange.ToArray())
            {
                if (now - kvp.Value > resetTime)
                {
                    if (_librarianAnswers.TryGetValue(kvp.Key, out var pictureBox))
                    {
                        this.InvokeIfRequired(() => pictureBox.Image = null);
                        _lastLibrarianAnswerChange.TryRemove(kvp.Key, out _);
                    }
                }
            }

            // Сбрасываем старые ответы читателя
            foreach (var kvp in _lastReaderAnswerChange.ToArray())
            {
                if (now - kvp.Value > resetTime)
                {
                    if (_readerAnswers.TryGetValue(kvp.Key, out var pictureBox))
                    {
                        this.InvokeIfRequired(() => pictureBox.Image = null);
                        _lastReaderAnswerChange.TryRemove(kvp.Key, out _);
                    }
                }
            }
        }

        #region ILibraryView Implementation
        // Реализация интерфейса ILibraryView, который определяет методы для обновления представления библиотеки
        public void OnCreateWorker(int workerId)
        {
            lock (_createWorkerLock) // Блокировка для потокобезопасного доступа к созданию библиотекарей
            {
                // Установка высоты для нового библиотекаря
                _heightOfLibrarians[workerId] = _heightForNextLibrarian;
                // Установка картинок ответов библиотекаря и читателя
                _librarianAnswers[workerId] = _answerLibrarianPictures[_idLibrarian];
                _readerAnswers[workerId] = _answerReaderPictures[_idLibrarian];
                // Установка картинки библиотекаря
                _librarians[workerId] = _workersPictures[_idLibrarian];
                _idFreePositionForReader[workerId] = 0; // Инициализация свободной позиции для читателя
                _idLibrarian++; // Увеличение счетчика библиотекарей
                _heightForNextLibrarian += 295; // Увеличение высоты для следующего библиотекаря
            }
        }

        // Метод для создания PictureBox для читателя
        private PictureBox CreateReaderPicture()
        {
            return new PictureBox
            {
                Size = new Size(80, 220), // Размер PictureBox
                SizeMode = PictureBoxSizeMode.StretchImage, // Режим растяжения изображения
                Location = new Point(1200, 270) // Начальная позиция за пределами видимости
            };
        }

        // Метод, вызываемый, когда читатель приходит в библиотеку с книгой
        public async void OnReaderComeToLibraryWithBook(int readerId)
        {
            var reader = CreateReaderPicture(); // Создание PictureBox для читателя
            reader.Image = Properties.Resources.ReaderWithBook; // Установка изображения читателя с книгой
            _readers[readerId] = reader; // Сохранение PictureBox в словаре читателей
            this.InvokeIfRequired(() => Controls.Add(reader)); // Добавление PictureBox на форму
            var cts = new CancellationTokenSource(); // Создание токена отмены для анимации
            _readerMovements[readerId] = cts; // Сохранение токена в словаре движений читателей
            await MoveToX(reader, 1150, cts.Token, TimingConsts.TimeToComeInLibrary); // Анимация движения в библиотеку
        }

        // Метод, вызываемый, когда читатель приходит в библиотеку без книги
        public async void OnReaderComeToLibraryWithoutBook(int readerId)
        {
            var reader = CreateReaderPicture(); // Создание PictureBox для читателя
            reader.Image = Properties.Resources.Reader; // Установка изображения читателя без книги
            _readers[readerId] = reader; // Сохранение PictureBox в словаре читателей
            this.InvokeIfRequired(() => Controls.Add(reader)); // Добавление PictureBox на форму
            var cts = new CancellationTokenSource(); // Создание токена отмены для анимации
            _readerMovements[readerId] = cts; // Сохранение токена в словаре движений читателей
            await MoveToX(reader, 1150, cts.Token, TimingConsts.TimeToComeInLibrary); // Анимация движения в библиотеку
        }

        // Метод, вызываемый, когда читатель присоединяется к очереди библиотекаря
        public async void OnReaderJoinedQueue(int readerId, int workerId)
        {
            if (!_readers.TryGetValue(readerId, out var reader)) return; // Проверка, существует ли читатель
            CancelReaderMovement(readerId); // Отмена предыдущих движений читателя
            int targetY = _heightOfLibrarians[workerId]; // Получение высоты библиотекаря
            int targetX;
            lock (_syncNextPosition) // Блокировка для синхронизации доступа к позициям
            {
                targetX = _queueXPositions[_idFreePositionForReader[workerId]]; // Получение свободной позиции в очереди
                _idFreePositionForReader[workerId]++; // Увеличение счетчика свободных позиций
            }

            var cts = new CancellationTokenSource(); // Создание токена отмены для анимации
            _readerMovements[readerId] = cts; // Сохранение токена в словаре движений читателей

            await MoveToY(reader, targetY, cts.Token, TimingConsts.TimeToTakePlaceInQueue - 700);
            await MoveToX(reader, targetX, cts.Token, TimingConsts.TimeToTakePlaceInQueue - 500);
        }

        #region just swap images
        //устанавливаем изображение для библиотекаря, когда читатель начинает диалог
        public void OnReaderStartedDialogueWithWorker(int readerId, int workerId)
        {
            _librarianAnswers[workerId].Image = Properties.Resources.WhatYouWant;
            _lastLibrarianAnswerChange[workerId] = DateTime.Now;
        }
        //когда читатель запрашивает книгу
        public void OnReaderAskedForBook(int readerId, int workerId)
        {
            _readerAnswers[workerId].Image = Properties.Resources.WannaTakeBook;
            _lastReaderAnswerChange[workerId] = DateTime.Now;
        }
        //запрашивает возврат книги
        public void OnReaderAskedForReturnBook(int readerId, int workerId)
        {
            _readerAnswers[workerId].Image = Properties.Resources.WannaReturnBook;
            _lastReaderAnswerChange[workerId] = DateTime.Now;
        }
        //библиотекарь отклоняет запрос
        public void OnWorkerDeclineRequest(int readerId, int workerId)
        {
            _librarianAnswers[workerId].Image = Properties.Resources.RequestDeclined;
            _lastLibrarianAnswerChange[workerId] = DateTime.Now;
        }
        //принимает запрос
        public void OnWorkerAcceptRequest(int readerId, int workerId)
        {
            _librarianAnswers[workerId].Image = Properties.Resources.YesOk;
            _lastLibrarianAnswerChange[workerId] = DateTime.Now;
        }
        //библиотекарь возвращает книгу
        public void OnWorkerReturnedBookToLibrary(int workerId)
        {
            _librarians[workerId].Image = Properties.Resources.Employee;
        }
        //находит книгу
        public void OnWorkerFoundBook(int workerId)
        {
            _librarianAnswers[workerId].Image = Properties.Resources.BookExist;
            _lastLibrarianAnswerChange[workerId] = DateTime.Now;
        }
        //берет книгу 
        public void OnWorkerTookBookInLibrary(int workerId)
        {
            _librarians[workerId].Image = Properties.Resources.EmployeeWithBook;
        }
        //не нашел книгу
        public void OnWorkerNotFoundBook(int workerId)
        {
            _librarianAnswers[workerId].Image = Properties.Resources.BookNotExist;
            _lastLibrarianAnswerChange[workerId] = DateTime.Now;
        }
        //читатель берет книгу
        public void OnReaderTookBook(int readerId, int workerId)
        {
            _readers[readerId].Image = Properties.Resources.ReaderWithBook;// Добавляем книгу в руки читателя
            _librarians[workerId].Image = Properties.Resources.Employee;// Убираем книгу из рук рабочего
        }
        //читатель отдает книгу
        public void OnReaderGaveBook(int readerId, int workerId)
        {
            _readers[readerId].Image = Properties.Resources.Reader;
            _librarians[workerId].Image = Properties.Resources.EmployeeWithBook;
        }
        //читатель счастлив
        public void OnReaderBecameHappy(int readerId, int workerId)
        {
            _readerAnswers[workerId].Image = Properties.Resources.ReaderHappy;
            _lastReaderAnswerChange[workerId] = DateTime.Now;
        }
        //читатель зол
        public void OnReaderBecameAngry(int readerId, int workerId)
        {
            _readerAnswers[workerId].Image = Properties.Resources.ReaderAngry;
            _lastReaderAnswerChange[workerId] = DateTime.Now;
        }
        #endregion
        //перемещение библиотекаря чтобы вернуть книгу
        public async void OnWorkerGoingToReturnBook(int workerId)
        {
            if (!_librarians.TryGetValue(workerId, out var worker)) return;

            var cts = new CancellationTokenSource();
            _workerMovements[workerId] = cts;

            await MoveToX(worker, _libraryLocation.X, cts.Token, TimingConsts.TimeToGoToLibrary - 250);
            await MoveToY(worker, _libraryLocation.Y, cts.Token, TimingConsts.TimeToGoToLibrary - 250);
        }
        //чтобы взять книгу
        public async void OnWorkerGoingToTakeBook(int workerId)
        {
            if (!_librarians.TryGetValue(workerId, out var worker)) return;

            var cts = new CancellationTokenSource();
            _workerMovements[workerId] = cts;

            await MoveToX(worker, _libraryLocation.X, cts.Token, TimingConsts.TimeToGoToLibrary - 250);
            await MoveToY(worker, _libraryLocation.Y, cts.Token, TimingConsts.TimeToGoToLibrary - 250);
        }

        public async void OnWorkerReturningToAcceptRequests(int workerId)
        {
            if (!_librarians.TryGetValue(workerId, out var worker)) return;

            var cts = new CancellationTokenSource();
            _workerMovements[workerId] = cts;

            await MoveToY(worker, _heightOfLibrarians[workerId], cts.Token, TimingConsts.TimeToReturnToStoika - 250);
            await MoveToX(worker, _xForLibrarians, cts.Token, TimingConsts.TimeToReturnToStoika - 250);
            
        }
        public async void OnReaderEndedDialogueWithWorker(int readerId, int workerId)
        {
            if (!_readers.TryGetValue(readerId, out var reader))
                return;
            _librarianAnswers[workerId].Image = Properties.Resources.Goodbye;
            _lastLibrarianAnswerChange[workerId] = DateTime.Now;

            CancelReaderMovement(readerId);

            bool exitUp = reader.Top < this.Height / 2;
            int exitY = exitUp ? -reader.Height : this.Height + reader.Height;

            var cts = new CancellationTokenSource();
            _readerMovements[readerId] = cts;
            try
            {
                await MoveToY(reader, exitY, cts.Token, TimingConsts.TimeToLeaveFromLibrary);
            }
            finally
            {
                // Освобождаем ресурсы только после завершения движения
                if (_readerMovements.TryRemove(readerId, out var oldCts))
                {
                    oldCts.Dispose();
                }
                RemoveReader(readerId, workerId);
            }
        }

        public async void OnReaderLeavingFromLibrary(int readerId, int workerId)
        {

            await UpdateQueueAfterReaderLeft(readerId, workerId);
        }
        private async Task UpdateQueueAfterReaderLeft(int ReaderId, int WorkerId)
        {
            // Группируем читателей по очередям (по Y-координате)
            var queues = _readers
               .Where(x => Math.Abs(x.Value.Top -_heightOfLibrarians[WorkerId]) < 150)
              .OrderBy(g => g.Value.Left)
              .ToList();  // Сортируем очереди сверху вниз


                // Обновляем позиции для каждого читателя в очереди
                for (int i = 0; i < queues.Count; i++)
                {
                    var readerId = queues[i].Key;
                    var reader = queues[i].Value;

                    // Новая позиция должна быть на 150px левее для каждого следующего
                    int newX = _queueXPositions.First() + (i * 160);

                    // Если позиция не изменилась - пропускаем
                    if (reader.Left == newX) continue;

                    CancelReaderMovement(readerId);

                    var cts = new CancellationTokenSource();
                    _readerMovements[readerId] = cts;

                    // Плавное перемещение к новой позиции
                    await MoveToX(reader, newX, cts.Token, 250);
                }
        }
        //удаление читателя из библиотеки
        private void RemoveReader(int readerId, int workerId)
        {
            CancelReaderMovement(readerId);

            lock (_syncNextPosition)
            {
                _idFreePositionForReader[workerId]--;
            }

            if (_readers.TryRemove(readerId, out var reader))
            {
                this.InvokeIfRequired(() =>
                {
                    Controls.Remove(reader);
                    reader.Dispose();
                });
            }
        }
        //обновление состояния библиотеки после пополнения ее книгами
        public async void OnLibraryRefilled()
        {
            await MoveToY(gruzovik, 430, new CancellationToken());

            await Task.Delay(300);

            await MoveToY(gruzovik, 650, new CancellationToken());
        }
        //вывод информации о книгах
        public void OnCountOfLostPublicationsChanged(int count)
        {
            CountOfLostPublications.Text = $"Кол-во потерянных публикаций:{count}";
        }
        public void OnCountOfAvailablePublicationsChanged(int count)
        {
            CountOfAvailablePublications.Text = $"Кол-во доступных публикаций:{count}";
        }
        //смена даты
        public void OnDateChanged(int date)
        {
            DateTime restoredDate = new DateTime(
                date / 10000,        // Год (2025)
                (date / 100) % 100,  // Месяц (1)
                date % 100           // День (1)
            );
            CurrentDate.Text = $"Текущая дата: {restoredDate.ToString("dd.MM.yyyy")}" ;
        }

        #region Helper Methods
        //для изменения формы из другого потока
        private void InvokeIfRequired(Action action)
        {
            if (this.InvokeRequired)
                this.Invoke(action);
            else
                action();
        }
        #endregion

        #endregion
        //обработка событий
        public void OnLibraryEvent(LibraryEvents eventType, int ReaderID = -1, int WorkerID = -1)
        {
            Action action = eventType switch
            {
                LibraryEvents.DateChanged => () => OnDateChanged(ReaderID),
                LibraryEvents.LibraryRefilled => () => OnLibraryRefilled(),
                LibraryEvents.CountOfLostPublicationsChanged => () => OnCountOfLostPublicationsChanged(ReaderID),
                LibraryEvents.CountOfAvailablePublicationsChanged => () => OnCountOfAvailablePublicationsChanged(ReaderID),
                LibraryEvents.CreateWorker => () => OnCreateWorker(WorkerID),
                LibraryEvents.ReaderComeToLibraryWithBook => () => OnReaderComeToLibraryWithBook(ReaderID),
                LibraryEvents.ReaderComeToLibraryWithoutBook => () => OnReaderComeToLibraryWithoutBook(ReaderID),
                LibraryEvents.ReaderJoinedQueue => () => OnReaderJoinedQueue(ReaderID, WorkerID),
                LibraryEvents.ReaderStartedDialogueWithWorker => () => OnReaderStartedDialogueWithWorker(ReaderID, WorkerID),
                LibraryEvents.ReaderAskedForBook => () => OnReaderAskedForBook(ReaderID, WorkerID),
                LibraryEvents.ReaderAskerForReturnBook => () => OnReaderAskedForReturnBook(ReaderID, WorkerID),
                LibraryEvents.WorkerDeclineRequest => () => OnWorkerDeclineRequest(ReaderID, WorkerID),
                LibraryEvents.WorkerAcceptRequest => () => OnWorkerAcceptRequest(ReaderID, WorkerID),
                LibraryEvents.WorkerGoingToReturnBook => () => OnWorkerGoingToReturnBook(WorkerID),
                LibraryEvents.WorkerReturnedBookToLibrary => () => OnWorkerReturnedBookToLibrary(WorkerID),
                LibraryEvents.WorkerReturningToAcceptRequests => () => OnWorkerReturningToAcceptRequests(WorkerID),
                LibraryEvents.WorkerFoundBook => () => OnWorkerFoundBook(WorkerID),
                LibraryEvents.WorkerGoingToTakeBook => () => OnWorkerGoingToTakeBook(WorkerID),
                LibraryEvents.WorkerTookBookInLibrary => () => OnWorkerTookBookInLibrary(WorkerID),
                LibraryEvents.WorkerNotFoundBook => () => OnWorkerNotFoundBook(WorkerID),
                LibraryEvents.ReaderTookBook => () => OnReaderTookBook(ReaderID, WorkerID),
                LibraryEvents.ReaderGaveBook => () => OnReaderGaveBook(ReaderID, WorkerID),
                LibraryEvents.ReaderBecameHappy => () => OnReaderBecameHappy(ReaderID, WorkerID),
                LibraryEvents.ReaderBecameAngry => () => OnReaderBecameAngry(ReaderID, WorkerID),
                LibraryEvents.ReaderEndedDialogueWithWorker => () => OnReaderEndedDialogueWithWorker(ReaderID, WorkerID),
                LibraryEvents.ReaderLeavingFromLibrary => () => OnReaderLeavingFromLibrary(ReaderID, WorkerID),
                _ => () => { }
            };
            this.InvokeIfRequired(() => action());
 
        }
    }
}

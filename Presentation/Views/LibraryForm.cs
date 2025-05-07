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
    internal partial class LibraryForm : Form, ILibraryView
    {
        public LibraryController Controller;

        // Потокобезопасные коллекции
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
        private readonly List<int> _queueXPositions = new() { 620, 780, 940, 1100 };
        private readonly object _syncNextPosition = new();
        private readonly object _createWorkerLock = new();
        private readonly Point _libraryLocation = new(107, 217);

        // Компоненты формы
        private readonly List<PictureBox> _answerLibrarianPictures;
        private readonly List<PictureBox> _answerReaderPictures;
        private readonly List<PictureBox> _workersPictures;
        private int _heightForNextLibrarian = 120;
        private int _xForLibrarians = 400;
        private int _xForNextReader = 620;
        private int _idLibrarian = 0;

        public LibraryForm()
        {
            InitializeComponent();
            _answerLibrarianPictures = new() { LibrarianAnswer1, LibrarianAnswer2 };
            _answerReaderPictures = new() { ReaderAnswer1, ReaderAnswer2 };
            _workersPictures = new() { Librarian1, Librarian2 };

            _dialogueResetTimer.Interval = 200; // Проверяем каждые 200 мс
            _dialogueResetTimer.Tick += ResetOldDialogues;
            _dialogueResetTimer.Start();
        }

        #region Movement Logic
        private async Task MoveToY(PictureBox pictureBox, int targetY, CancellationToken token, int durationMs = 500)
        {
            try
            {
                if (token.IsCancellationRequested)
                    return;

                int startY = pictureBox.Top;
                float distance = targetY - startY;
                int steps = Math.Max(1, durationMs / 16); // ~60 FPS
                float stepY = distance / steps;

                for (int i = 1; i <= steps; i++)
                {
                    if (token.IsCancellationRequested)
                        break;
                    int newY = startY + (int)(stepY * i);

                    this.InvokeIfRequired(() => pictureBox.Top = newY);
                    try
                    {
                        await Task.Delay(16, token); // Фиксированный интервал для плавности
                    }
                    catch (Exception e) { }
                }


                // Финализация позиции
                this.InvokeIfRequired(() => pictureBox.Top = targetY);
            }
            catch (Exception) { }
        }

        private async Task MoveToX(PictureBox pictureBox, int targetX, CancellationToken token, int durationMs = 500)
        {
            try
            {
                if (token.IsCancellationRequested)
                    return; // Если токен уже отменён, выходим

                int startX = pictureBox.Left;
                float distance = targetX - startX;
                int steps = Math.Max(1, durationMs / 16); // ~60 FPS
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

        private void CancelReaderMovement(int readerId)
        {
            if (_readerMovements.TryRemove(readerId, out var cts))
            {
                try
                {
                    cts.Cancel();
                }
                catch (ObjectDisposedException) { }
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
        public void OnCreateWorker(int workerId)
        {
            lock (_createWorkerLock)
            {
                _heightOfLibrarians[workerId] = _heightForNextLibrarian;
                _librarianAnswers[workerId] = _answerLibrarianPictures[_idLibrarian];
                _readerAnswers[workerId] = _answerReaderPictures[_idLibrarian];
                _librarians[workerId] = _workersPictures[_idLibrarian];
                _idFreePositionForReader[workerId] = 0;
                _idLibrarian++;

                _heightForNextLibrarian += 295;
            }
        }

        private PictureBox CreateReaderPicture()
        {
            return new PictureBox
            {
                Size = new Size(80, 220),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(1200, 270)
            };
        }

        public async void OnReaderComeToLibraryWithBook(int readerId)
        {
            var reader = CreateReaderPicture();
            reader.Image = Properties.Resources.ReaderWithBook;
            _readers[readerId] = reader;
            this.InvokeIfRequired(() => Controls.Add(reader));

            var cts = new CancellationTokenSource();
            _readerMovements[readerId] = cts;
            await MoveToX(reader, 1150, cts.Token, TimingConsts.TimeToComeInLibrary);
        }

        public async void OnReaderComeToLibraryWithoutBook(int readerId)
        {
            var reader = CreateReaderPicture();
            reader.Image = Properties.Resources.Reader;
            _readers[readerId] = reader;
            this.InvokeIfRequired(() => Controls.Add(reader));

            var cts = new CancellationTokenSource();
            _readerMovements[readerId] = cts;
            await MoveToX(reader, 1150, cts.Token, TimingConsts.TimeToComeInLibrary);
        }

        public async void OnReaderJoinedQueue(int readerId, int workerId)
        {
            if (!_readers.TryGetValue(readerId, out var reader)) return;

            CancelReaderMovement(readerId);

            int targetY = _heightOfLibrarians[workerId];
            int targetX;

            lock (_syncNextPosition)
            {
                targetX = _queueXPositions[_idFreePositionForReader[workerId]];
                _idFreePositionForReader[workerId]++;
            }

            var cts = new CancellationTokenSource();
            _readerMovements[readerId] = cts;

            await MoveToY(reader, targetY, cts.Token, TimingConsts.TimeToTakePlaceInQueue - 700);
            await MoveToX(reader, targetX, cts.Token, TimingConsts.TimeToTakePlaceInQueue - 500);
        }

        #region just swap images
        public void OnReaderStartedDialogueWithWorker(int readerId, int workerId)
        {
            _librarianAnswers[workerId].Image = Properties.Resources.WhatYouWant;
            _lastLibrarianAnswerChange[workerId] = DateTime.Now;
        }

        public void OnReaderAskedForBook(int readerId, int workerId)
        {
            _readerAnswers[workerId].Image = Properties.Resources.WannaTakeBook;
            _lastReaderAnswerChange[workerId] = DateTime.Now;
        }

        public void OnReaderAskedForReturnBook(int readerId, int workerId)
        {
            _readerAnswers[workerId].Image = Properties.Resources.WannaReturnBook;
            _lastReaderAnswerChange[workerId] = DateTime.Now;
        }

        public void OnWorkerDeclineRequest(int readerId, int workerId)
        {
            _librarianAnswers[workerId].Image = Properties.Resources.RequestDeclined;
            _lastLibrarianAnswerChange[workerId] = DateTime.Now;
        }

        public void OnWorkerAcceptRequest(int readerId, int workerId)
        {
            _librarianAnswers[workerId].Image = Properties.Resources.YesOk;
            _lastLibrarianAnswerChange[workerId] = DateTime.Now;
        }
        public void OnWorkerReturnedBookToLibrary(int workerId)
        {
            _librarians[workerId].Image = Properties.Resources.Employee;
        }
        public void OnWorkerFoundBook(int workerId)
        {
            _librarianAnswers[workerId].Image = Properties.Resources.BookExist;
            _lastLibrarianAnswerChange[workerId] = DateTime.Now;
        }
        public void OnWorkerTookBookInLibrary(int workerId)
        {
            _librarians[workerId].Image = Properties.Resources.EmployeeWithBook;
        }

        public void OnWorkerNotFoundBook(int workerId)
        {
            _librarianAnswers[workerId].Image = Properties.Resources.BookNotExist;
            _lastLibrarianAnswerChange[workerId] = DateTime.Now;
        }

        public void OnReaderTookBook(int readerId, int workerId)
        {
            _readers[readerId].Image = Properties.Resources.ReaderWithBook;// Добавляем книгу в руки читателя
            _librarians[workerId].Image = Properties.Resources.Employee;// Убираем книгу из рук рабочего
        }
        public void OnReaderGaveBook(int readerId, int workerId)
        {
            _readers[readerId].Image = Properties.Resources.Reader;
            _librarians[workerId].Image = Properties.Resources.EmployeeWithBook;
        }

        public void OnReaderBecameHappy(int readerId, int workerId)
        {
            _readerAnswers[workerId].Image = Properties.Resources.ReaderHappy;
            _lastReaderAnswerChange[workerId] = DateTime.Now;
        }

        public void OnReaderBecameAngry(int readerId, int workerId)
        {
            _readerAnswers[workerId].Image = Properties.Resources.ReaderAngry;
            _lastReaderAnswerChange[workerId] = DateTime.Now;
        }
        #endregion
        public async void OnWorkerGoingToReturnBook(int workerId)
        {
            if (!_librarians.TryGetValue(workerId, out var worker)) return;

            var cts = new CancellationTokenSource();
            _workerMovements[workerId] = cts;

            await MoveToX(worker, _libraryLocation.X, cts.Token, TimingConsts.TimeToGoToLibrary - 250);
            await MoveToY(worker, _libraryLocation.Y, cts.Token, TimingConsts.TimeToGoToLibrary - 250);
        }
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

        #region Helper Methods
        private void InvokeIfRequired(Action action)
        {
            if (this.InvokeRequired)
                this.Invoke(action);
            else
                action();
        }
        #endregion

        #endregion
        public void OnLibraryEvent(LibraryEvents eventType, int ReaderID = -1, int WorkerID = -1)
        {
            Action action = eventType switch
            {
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

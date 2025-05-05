using LibrarySimulation.Core.Enums;
using LibrarySimulation.Domain.Aggregates;
using LibrarySimulation.Domain.Entities;
using LibrarySimulation.Domain.Services.Factories;
using LibrarySimulation.Domain.Services;
using LibrarySimulation.Presentation.Views.Components;
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

namespace LibrarySimulation.Presentation.Views
{
    internal partial class LibraryForm : Form, ILibraryView
    {
        public LibraryController Controller;

        private Dictionary<int, PictureBox> Librarians = new();
        private Dictionary<int, PictureBox> Readers = new();
        public LibraryForm()
        {
            InitializeComponent();
            
        }

        #region ILibraryView
        public void OnReaderComeToLibrary(int readerId)
        {
            var reader = CreateReaderPicture();
            Readers[readerId] = reader;

        }
        private Image ByteArrayToPngImage(byte[] byteArray)
        {
            using (var ms = new MemoryStream(byteArray))
            {
                return new Bitmap(ms); 
            }
        }
        private PictureBox CreateReaderPicture()
        {
            var newReader = new PictureBox();
            newReader.Image = ByteArrayToPngImage(Properties.Resources.Reader);
            newReader.Size = new Size(80, 220);
            newReader.SizeMode = PictureBoxSizeMode.StretchImage;
            newReader.Location = new Point(locationX, 220);
        }
        // Dictionary< int, List<PictureBox> > LibrarianQueue
        public void OnReaderJoinedQueue(int readerId, int workerId)
        {
            // MoveToQueueOfWorker
            // where Worker = workerId
        }

        public void OnReaderStartedDialogueWithWorker(int readerId, int workerId)
        {
            // CreateMessageFromWorker
            // with text "Что вы хотите?"
            // where Worker = workerId
        }

        public void OnReaderAskedForBook(int readerId)
        {
            // CreateMessageFromReader
            // with text "Хочу взять книгу"
            // where Reader = readerId
        }

        public void OnReaderAskedForReturnBook(int readerId)
        {
            // CreateMessageFromReader
            // with text "Хочу вернуть книгу"
            // where Reader = readerId
        }

        public void OnWorkerDeclineRequest(int readerId, int workerId)
        {
            // CreateMessageFromWorker
            // with text "Вам отказано!"
            // where Worker = workerId
        }

        public void OnWorkerAcceptRequest(int readerId, int workerId)
        {
            // CreateMessageFromWorker
            // with text "Да, хорошо!"
            // where Worker = workerId
        }

        public void OnWorkerGoingToReturnBook(int workerId)
        {
            // WorkerMoveToLibrary
            // where Worker = workerId
        }

        public void OnWorkerReturnedBookToLibrary(int workerId)
        {
            // Убираем книгу из рук рабочего
            // where Worker = workerId
        }

        public void OnWorkerReturningToAcceptRequests(int workerId)
        {
            //WorkerMoveToHisStoika
            // where Worker = workerId
        }

        public void OnWorkerFoundBook(int workerId)
        {
            //CreateMessageFromWorker
            // with text = "Книга в наличии"
        }

        public void OnWorkerGoingToTakeBook(int workerId)
        {
            // WorkerMoveToLibrary
            // where Worker = workerId
        }

        public void OnWorkerTookBookInLibrary(int workerId)
        {
            // Добавляем книгу в руки рабочего
            // where Worker = workerId
        }

        public void OnWorkerNotFoundBook(int workerId)
        {
            //CreateMessageFromWorker
            // with text = "Книги нет в наличии"
        }

        public void OnReaderTookBook(int readerId, int workerId)
        {
            // Добавляем книгу в руки читателя
            // Убираем книгу из рук рабочего
        }

        public void OnReaderGaveBook(int readerId, int workerId)
        {
            // Добавляем книгу в руки рабочего
            // Убираем книгу из рук читателя
        }

        public void OnReaderBecameHappy(int readerId)
        {
            //CreateMessageFromReader
            // with text = "Я ДОБРИ <3"
        }

        public void OnReaderBecameAngry(int readerId)
        {
            //CreateMessageFromReader
            // with text = "Я ЗЛОЙ"
        }

        public void OnReaderEndedDialogueWithWorker(int readerId, int workerId)
        {
            //CreateMessageFromWorker
            // with text = "До свидания"
            // MoveOtherReadersInQueue
        }

        public void OnReaderLeavingFromLibrary(int readerId)
        {
            // ReaderMoveToVoid(up or down)
        }
        #endregion
        public void OnLibraryEvent(LibraryEvents eventType, int ReaderID = -1, int WorkerID = -1)
        {
            Action action = eventType switch
            {
                LibraryEvents.ReaderComeToLibrary => () => OnReaderComeToLibrary(ReaderID),
                LibraryEvents.ReaderJoinedQueue => () => OnReaderJoinedQueue(ReaderID, WorkerID),
                LibraryEvents.ReaderStartedDialogueWithWorker => () => OnReaderStartedDialogueWithWorker(ReaderID, WorkerID),
                LibraryEvents.ReaderAskedForBook => () => OnReaderAskedForBook(ReaderID),
                LibraryEvents.ReaderAskerForReturnBook => () => OnReaderAskedForReturnBook(ReaderID),
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
                LibraryEvents.ReaderBecameHappy => () => OnReaderBecameHappy(ReaderID),
                LibraryEvents.ReaderBecameAngry => () => OnReaderBecameAngry(ReaderID),
                LibraryEvents.ReaderEndedDialogueWithWorker => () => OnReaderEndedDialogueWithWorker(ReaderID, WorkerID),
                LibraryEvents.ReaderLeavingFromLibrary => () => OnReaderLeavingFromLibrary(ReaderID),
                _ => () => { }
            };
            action();
        }
    }
}

using LibrarySimulation.Core.Enums;
using LibrarySimulation.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Domain.Entities.Persons
{
    internal class Librarian : Person
    {
        private Library _library { get; set; }

        private Thread _thread;
        public Queue<Reader> ReaderQueue { get; } = new Queue<Reader>();

        
        public Librarian(string Name, Library library) : base(Name)
        {
            _library = library;
            _thread = new Thread(ProcessRequests);
            _thread.Start();
        }

        public void ProcessRequests()
        {
            while (true)
            {
                if (ReaderQueue.Count > 0)
                {
                    var reader = ReaderQueue.Dequeue();

                    //Оповещает о том, что рабочий начал диалог с читателем
                    _library.Notify(LibraryEvents.ReaderStartedDialogueWithWorker, WorkerID: Id);
                    
                    ProcessReaderRequests(reader, _library.today);

                }
                Thread.Sleep(100);//задержка между проверками
            }
        }
        private void ProcessReaderRequests(Reader reader, DateTime today)
        {
            int count = reader.Requests.Count;

            while(count > 0)
            {
                Thread.Sleep(100);

                Request reader_request = reader.Requests.Dequeue();
                RequestStatus requestStatus;

                if (reader_request.RequestType == RequestType.Return)
                {
                    _library.Notify(LibraryEvents.ReaderAskerForReturnBook, reader.Id);
                    requestStatus = ProcessReturnRequest(reader.Id, reader_request);
                }
                else
                { 
                    _library.Notify(LibraryEvents.ReaderAskedForBook, reader.Id);
                    requestStatus = ProcessTakeRequest(reader.Id, reader_request, today);
                }

                if (requestStatus == RequestStatus.Rejected)
                    _library.Notify(LibraryEvents.ReaderBecameAngry, reader.Id);
                else
                    _library.Notify(LibraryEvents.ReaderBecameHappy, reader.Id);

                count--;

                Thread.Sleep(100);
            }

            _library.Notify(LibraryEvents.ReaderEndedDialogueWithWorker, reader.Id, Id);
            _library.Notify(LibraryEvents.ReaderLeavingFromLibrary, reader.Id);
        }


        #region Request Took Book
        private bool checkBookAvailability(Publication publication)
        {
            if (_library.isLibraryContainsPublication(publication))
            {
                _library.Notify(LibraryEvents.WorkerFoundBook, WorkerID: Id);
                return true;
            }
            else
            {
                _library.Notify(LibraryEvents.WorkerNotFoundBook, WorkerID: Id);
                return false;
            }
        }
        private bool isReaderHasOverBorrowedBooks(int readerId, DateTime today)
        {
            return 
                _library.Publications
                .Where(x => x.owners.ContainsKey(readerId))
                .Any(x => x.isBookOverBorrowedByPerson(today, readerId));
        }
        
        private RequestStatus ProcessTakeRequest(int readerId, Request request, DateTime today)
        {

            RequestStatus requestStatus = RequestStatus.Rejected;

            //Если есть просроченные книги, то отказываем в прокате книги
            if (isReaderHasOverBorrowedBooks(readerId, today))
            {
                requestStatus = RequestStatus.Rejected;

                //Оповещаем, что рабочий отказала в выдаче книги
                _library.Notify(LibraryEvents.WorkerDeclineRequest, readerId, Id);

                return requestStatus;
            }

            _library.Notify(LibraryEvents.WorkerCheckBookAvailability, WorkerID: Id);
            Thread.Sleep(500);
            
            //Находит книгу или не находит книгу в наличии
            if (checkBookAvailability(request.Publication))
            {
                requestStatus = RequestStatus.Approved;

                //Оповещаем, что рабочий пошёл за книгой
                _library.Notify(LibraryEvents.WorkerGoingToTakeBook, WorkerID: Id);

                Thread.Sleep(1000); // Библиотекарь идёт за книгой

                //Рабочий берёт книгу с полки
                _library.WorkerTookBookInLibrary(request.Publication, readerId, Id, today);

                //Оповещаем, что рабочий идёт обратно к читателю
                _library.Notify(LibraryEvents.WorkerReturningToAcceptRequests, WorkerID: Id);

                Thread.Sleep(1000);  // Библиотекарь возвращается с книгой

                //Читатель берёт книгу
                _library.Notify(LibraryEvents.ReaderTookBook, ReaderID: readerId, WorkerID: Id);


                return requestStatus;
            }

            return requestStatus;
        }
        #endregion

        #region Request Return Book

        private bool checkExistionOfBook(Publication publication)
        {
            if (_library.isLibraryContainsPublication(publication))
            {
                _library.Notify(LibraryEvents.WorkerFoundBook, WorkerID: Id);
                return true;
            }
            else
            {
                _library.Notify(LibraryEvents.WorkerNotFoundBook, WorkerID: Id);
                return false;
            }
        }
        private RequestStatus ProcessReturnRequest(int readerId, Request request)
        {
            RequestStatus requestStatus = RequestStatus.Rejected;
     

            if (checkExistionOfBook(request.Publication))
            {
                requestStatus = RequestStatus.Approved;

                //Оповещаем, что читатель отдал книгу
                _library.Notify(LibraryEvents.ReaderGaveBook, ReaderID: readerId, WorkerID: Id);

                //Оповещаем, что рабочий идёт возвращать книгу
                _library.Notify(LibraryEvents.WorkerGoingToReturnBook, WorkerID: Id);

                // Библиотекарь идёт возвращать книгу
                Thread.Sleep(1000);

                //Библиотекарь возвращает книгу в библиотеку
                _library.WorkerReturnBookToLibrary(request.Publication, readerId, Id);

                //Оповещаем, что библиотекарь возвращается обрабатывать запросы
                _library.Notify(LibraryEvents.WorkerReturningToAcceptRequests, WorkerID: Id);

                //Библиотекарь идёт к читателю обратно
                Thread.Sleep(1000);

                //Читатель забрал книгу
                _library.Notify(LibraryEvents.ReaderTookBook, ReaderID: readerId, WorkerID: Id);

            }

            return requestStatus;
        }
        #endregion
    }

}

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
    internal class Library
    {
        private List<IObserver> _observers = new ();
        public List<LibraryPublication> Publications { get; } = new ();
        public List<Librarian> Librarians { get; } = new ();
        public DateTime today { get; set; } = new DateTime(2025, 1, 1);

        public bool isLibraryContainsPublication(Publication publication)
        {
            return Publications.Select(x => x.Publication).Contains(publication);
        }

        #region Добавление Публикаций, Копий, Работников
        public void AddNewPublication(Publication publication, int count = 0)
        {
            var temp = new LibraryPublication(publication);
            temp.AddCopiesOfPublication(count);
            Publications.Add(temp);
        }
        public void AddCopiesOfPublication(Publication publication, int count)
        {
            Publications.First(x => x.Publication == publication).AddCopiesOfPublication(count);
        }
       
        public void AddLibrarian(string name)
        {
            var temp = LibraryFactory.CreateLibrarian(name, this);
            Librarians.Add(temp);
            Notify(LibraryEvents.CreateWorker, WorkerID: temp.Id);
        }

        #endregion

        #region Взаимодействия читателя с библиотекой
        private Librarian GetLeastBusyLibrarian()
        {
            return Librarians.OrderBy(l => l.ReaderQueue.Count).First();
        }
        public void ReaderComeToLibrary(Reader reader)
        {
            if (reader.Requests.Peek().RequestType is RequestType.Return)
                Notify(LibraryEvents.ReaderComeToLibraryWithBook, reader.Id);
            else
                Notify(LibraryEvents.ReaderComeToLibraryWithoutBook, reader.Id);
            
            Thread.Sleep(TimingConsts.TimeToGoToLibrary);

            Librarian worker = GetLeastBusyLibrarian();
            Notify(LibraryEvents.ReaderJoinedQueue, reader.Id, WorkerID: worker.Id);
            Thread.Sleep(TimingConsts.TimeToTakePlaceInQueue);
            
            worker.ReaderQueue.Enqueue(reader);
        }
        #endregion

        #region Взаимодействие рабочего с библиотекой
        public void WorkerTookBookInLibrary(Publication publication, int readerId, int workerId, DateTime today)
        {
            var temp = Publications.First(x => x.Publication == publication);
            temp.owners[readerId] = today;
            temp.AvailableCopies--;

            Notify(LibraryEvents.WorkerTookBookInLibrary, WorkerID: workerId); 
        }

        internal void WorkerReturnBookToLibrary(Publication publication, int readerId, int workerId)
        {
            var temp = Publications.First(x => x.Publication == publication);
            temp.owners.Remove(readerId);
            temp.AvailableCopies++;

            Notify(LibraryEvents.WorkerReturnedBookToLibrary, WorkerID: workerId);
        }
        #endregion

        #region Оповещения о событиях
        public void Subscribe(IObserver observer)
        {
            lock (SyncHelper.ObserveLock)
            {
                _observers.Add(observer);
            }
        }
        public void Unsubcribe(IObserver observer)
        {
            lock (SyncHelper.ObserveLock)
            {
                _observers.Remove(observer);
            }
        }
        public void Notify(LibraryEvents libraryEvent, int ReaderID = 0, int WorkerID = 0)
        {
            lock (SyncHelper.ObserveLock)
            {
                foreach (var observer in _observers)
                {
                    observer.OnLibraryEvent(libraryEvent, ReaderID, WorkerID);
                }
            }
        }
        #endregion
    }
}

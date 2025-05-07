using LibrarySimulation.Core.Enums;
using LibrarySimulation.Core.Interfaces;
using LibrarySimulation.Presentation.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Presentation.Views
{
    internal interface ILibraryView : IObserver
    {
        void OnDateChanged(int date);
        void OnLibraryRefilled();

        void OnCountOfLostPublicationsChanged(int count);

        void OnCountOfAvailablePublicationsChanged(int count);

        void OnCreateWorker(int WorkerId);

        // Читатель пришел в библиотеку с книгой
        void OnReaderComeToLibraryWithBook(int readerId);

        // Читатель пришёл в библиотеку без книги
        void OnReaderComeToLibraryWithoutBook(int readerId);

        // Читатель встал в очередь к библиотекарю
        void OnReaderJoinedQueue(int readerId, int workerId);

        // Начался диалог между читателем и библиотекарем
        void OnReaderStartedDialogueWithWorker(int readerId, int workerId);

        // Читатель запросил книгу
        void OnReaderAskedForBook(int readerId, int workerId);

        // Читатель хочет вернуть книгу
        void OnReaderAskedForReturnBook(int readerId, int workerId);

        // Библиотекарь отклонил запрос (просроченные книги)
        void OnWorkerDeclineRequest(int readerId, int workerId);

        // Библиотекарь принял запрос
        void OnWorkerAcceptRequest(int readerId, int workerId);

        // Библиотекарь пошел возвращать книгу
        void OnWorkerGoingToReturnBook(int workerId);

        // Библиотекарь вернул книгу в библиотеку
        void OnWorkerReturnedBookToLibrary(int workerId);

        // Библиотекарь возвращается принимать запросы
        void OnWorkerReturningToAcceptRequests(int workerId);

        // Библиотекарь нашел книгу
        void OnWorkerFoundBook(int workerId);

        // Библиотекарь пошел взять книгу
        void OnWorkerGoingToTakeBook(int workerId);

        // Библиотекарь взял книгу в библиотеке
        void OnWorkerTookBookInLibrary(int workerId);

        // Библиотекарь не нашел книгу
        void OnWorkerNotFoundBook(int workerId);

        // Читатель взял книгу
        void OnReaderTookBook(int readerId, int workerId);

        // Читатель отдал книгу
        void OnReaderGaveBook(int readerId, int workerId);

        // Читатель стал довольным
        void OnReaderBecameHappy(int readerId, int workerId);

        // Читатель рассердился
        void OnReaderBecameAngry(int readerId, int workerId);

        // Диалог с библиотекарем завершен
        void OnReaderEndedDialogueWithWorker(int readerId, int workerId);

        // Читатель уходит из библиотеки
        void OnReaderLeavingFromLibrary(int readerId, int workerId);
    }
}

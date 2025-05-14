using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Core.Enums
{
    //набор событий, которые могут происходить в библиотеке
    internal enum LibraryEvents
    {
        //смена даты
        DateChanged,

        //библиотека пополняется новыми книгами
        LibraryRefilled,
        
        CountOfLostPublicationsChanged,
        CountOfAvailablePublicationsChanged,

        //создание библиотекаря
        CreateWorker,

        //читатель приходит с книгой/без книги
        ReaderComeToLibraryWithBook,
        ReaderComeToLibraryWithoutBook,

        //читатель встал в очередь
        ReaderJoinedQueue,//(id worker)

        //начал диалог с работником
        ReaderStartedDialogueWithWorker,

        //просит взять/сдать книгу
        ReaderAskedForBook,
        ReaderAskerForReturnBook,

        //статус ответа(есть или нет просроченных книг)
        WorkerDeclineRequest, //(есть просроченные книги)
        WorkerAcceptRequest,

        //проверка доступности книг
        WorkerCheckBookAvailability,

        //библиотекарь идет возвращать/возвращает/готов обрабатывать запросы
        WorkerGoingToReturnBook,
        WorkerReturnedBookToLibrary,
        WorkerReturningToAcceptRequests,

        //библиотекарь находит книгу/идет взять/ взял книгу
        WorkerFoundBook,
        WorkerGoingToTakeBook,
        WorkerTookBookInLibrary,
        //WorkerReturningToAcceptRequests (просто повторение)

        //библиотекарь не нашел книгу
        WorkerNotFoundBook,

        //читать взял/сдал книгу
        ReaderTookBook,
        ReaderGaveBook,

        //читатель стал счастливым/злым
        ReaderBecameHappy,
        ReaderBecameAngry,

        //диалог закончен
        ReaderEndedDialogueWithWorker,

        //читатель вышел из библиотеки
        ReaderLeavingFromLibrary
    }
}

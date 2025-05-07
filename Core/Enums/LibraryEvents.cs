using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Core.Enums
{
    internal enum LibraryEvents
    {
        LibraryRefilled,

        CreateWorker, 

        ReaderComeToLibraryWithBook,
        ReaderComeToLibraryWithoutBook,

        ReaderJoinedQueue,//(id worker)

        ReaderStartedDialogueWithWorker,

        ReaderAskedForBook,
        ReaderAskerForReturnBook,

        WorkerDeclineRequest, //(есть просроченные книги)
        WorkerAcceptRequest,

        WorkerCheckBookAvailability,

        WorkerGoingToReturnBook,
        WorkerReturnedBookToLibrary,
        WorkerReturningToAcceptRequests,

        WorkerFoundBook,
        WorkerGoingToTakeBook,
        WorkerTookBookInLibrary,
        //WorkerReturningToAcceptRequests (просто повторение)

        WorkerNotFoundBook,

        ReaderTookBook,
        ReaderGaveBook,
        ReaderBecameHappy,

        ReaderBecameAngry,

        ReaderEndedDialogueWithWorker,

        ReaderLeavingFromLibrary
    }
}

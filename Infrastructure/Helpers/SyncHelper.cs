using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Infrastructure.Helpers
{
    internal static class SyncHelper
    {
        public static readonly object PublicationLock = new object();
        public static readonly object PersonLock = new object();
        public static readonly object ObserveLock = new object();
        public static readonly object ChangeInLibrary = new object();
    }
}

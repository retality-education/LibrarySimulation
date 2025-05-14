using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Infrastructure.Helpers
{
    //класс для обеспечения потокобезопасности
    internal static class SyncHelper
    {
        public static readonly object PublicationLock = new object();
        public static readonly object PersonLock = new object();
        public static readonly object ObserveLock = new object();
        public static readonly object ChangeInLibrary = new object();
        public static readonly object ChangeCountOfLostPublications = new object();
        public static readonly object ChangeCountOfAvailablePublications = new object();
        
    }
}

using LibrarySimulation.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibrarySimulation.Core.Interfaces
{
    internal interface IObserver 
    {
        void OnLibraryEvent(LibraryEvents eventType, int ReaderID = -1, int WorkerID = -1);
    }
}

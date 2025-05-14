using LibrarySimulation.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LibrarySimulation.Core.Interfaces
{
    //интерфейс для реализации паттерна Наблюдатель
    //используется для уведомления объектов о событиях
    //тип события, Id читателя и id работника
    internal interface IObserver 
    {
        void OnLibraryEvent(LibraryEvents eventType, int ReaderID = -1, int WorkerID = -1);
    }
}

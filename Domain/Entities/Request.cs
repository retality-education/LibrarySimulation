using LibrarySimulation.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Domain.Entities
{
    //описание запроса читателя
    internal class Request
    {
        public RequestType RequestType { get; set; }//тип запроса
        public Publication Publication { get; set; }//публикация
        public Request(RequestType requestType, Publication publication)//конструктор
        {
            RequestType = requestType;
            Publication = publication;
        }
    }
}

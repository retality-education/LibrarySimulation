using LibrarySimulation.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Domain.Entities
{
    internal class Request
    {
        public RequestType RequestType { get; set; }
        public Publication Publication { get; set; }
        public Request(RequestType requestType, Publication publication)
        {
            RequestType = requestType;
            Publication = publication;
        }
    }
}

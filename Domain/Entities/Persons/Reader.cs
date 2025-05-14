using LibrarySimulation.Domain.Aggregates;
using LibrarySimulation.Domain.Entities.Publications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Domain.Entities.Persons
{
    //класс наследник
    internal class Reader : Person
    {
        public PriorityQueue<Request, int> Requests { get; set; }//приоритетная очередь запросов читателя(элемент-приоритет)
        public bool isReaderActive { get; set; } = true;
        public Reader(string name) : base(name)
        {
           
        }
    }
}

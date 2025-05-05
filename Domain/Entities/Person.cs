using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Domain.Entities
{
    internal abstract class Person
    {
        private static int nextId = 0;
        public int Id { get; private set; }
        public string Name { get; set; }
        public Person(string Name) {
            this.Name = Name;
            Id = nextId++;
        }
    }
}

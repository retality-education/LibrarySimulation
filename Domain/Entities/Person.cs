using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Domain.Entities
{
    //класс описывающий человека
    //класс абстрактный, т.к от него будут наследоваться классы рабочего библиотеки и читателя
    internal abstract class Person
    {
        private static int nextId = 0;//статическое поле для генерации id
        public int Id { get; private set; }//уникальный id
        public string Name { get; set; }//имя человека
        public Person(string Name) //конструктор
        {
            this.Name = Name;
            Id = nextId++;
        }
    }
}

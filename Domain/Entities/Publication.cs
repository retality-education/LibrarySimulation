using LibrarySimulation.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Domain.Entities
{
    //класс для описания публикации в библиотеке
    internal class Publication
    {
        public string Title { get; set; }//название публикации
        public string Author { get; set; }//автор
        public int Year { get; set; }//год публикации
        public Theme Theme { get; set; }//тема публикации
        public PublicationType Type { get; set; }//тип публикации
        public override string ToString() => $"{Title} ({Author}, {Year})";
    }
}

using LibrarySimulation.Core.Enums;
using LibrarySimulation.Domain.Aggregates;
using LibrarySimulation.Domain.Entities.Publications;
using LibrarySimulation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrarySimulation.Domain.Entities.Persons;

namespace LibrarySimulation.Domain.Services.Factories
{
    //класс для создания различных объектов
    internal static class LibraryFactory
    {
        //создаем объект публикации по заданным параметрам
        public static Publication CreatePublication(PublicationType type, string title, string author, Theme theme, int year)
        {
            Publication publication;
            switch (type)
            {
                case PublicationType.Book:
                    publication = new Book();
                    break;
                case PublicationType.Journal:
                    publication = new Journal();
                    break;
                case PublicationType.Thesis:
                    publication = new Thesis();
                    break;
                case PublicationType.Textbook:
                    publication = new Textbook();
                    break;
                default:
                    throw new ArgumentException("Unknown publication type");
            }

            publication.Title = title;
            publication.Author = author;
            publication.Year = year;
            publication.Theme = theme;

            return publication;
        }
        //создаем публикацию в библиотеке
        public static LibraryPublication CreateLibraryPublication(Publication publication)
        {
            return new LibraryPublication(publication);
        }
        //создаем библиотекаря
        public static Librarian CreateLibrarian(string name, Library library)
        {
            return new Librarian(name, library);
        }
        //создаем читателя
        public static Reader CreateReader(string name)
        {
            return new Reader(name);
        }
    }
}

using LibrarySimulation.Core.Enums;
using LibrarySimulation.Domain.Entities;
using LibrarySimulation.Domain.Entities.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Domain.Aggregates
{
    //публикации в библиотеке
    //обертка над публикацией
    internal class LibraryPublication
    {

        public const int MaxDayOfBorrowBook = 10;//макс кол-во дней пользования книгой
        public int TotalCopies { get; set; }//кол-во копий в библиотеке
        public int AvailableCopies { get; set; }//кол-во доступных копий
        public Publication Publication { get; private set; }//сам объект публикации
        // readerId -> Дата_взятия
        public Dictionary<int, DateTime> owners { get; } = new() { };//словарь хранит id читателя и дату взятия им книги

        //конструктор
        public LibraryPublication(Publication publication) 
        { 
            Publication = publication;
        }
        //проверка на то, что книга взята на больший срок(просрочена)
        private bool isBookOverBorrowed(DateTime today, DateTime borrowDate)
        {
            return (today - borrowDate).TotalDays > MaxDayOfBorrowBook;
        }
        //проверка есть ли у конкретного читателя просроченные книги
        public bool isBookOverBorrowedByPerson(DateTime today, int readerId)
        {
            bool res = false;
            if (owners.ContainsKey(readerId))
                res = isBookOverBorrowed(today, owners[readerId]);
            return res;
        }
        //проверка общего кол-ва просроченных книг у всех читателей
        public int CountOfMissingBooks(DateTime today)
        {
            int res = 0;

            foreach (var owner in owners)
                if (isBookOverBorrowed(today, owner.Value))
                    res++;
            return res;
        }
        //добавление копий конкретной публикации
        public void AddCopiesOfPublication(int count)
        {
            TotalCopies += count;
            AvailableCopies += count;
        }
     
    }
}

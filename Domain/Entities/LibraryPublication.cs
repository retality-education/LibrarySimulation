using LibrarySimulation.Core.Enums;
using LibrarySimulation.Domain.Entities.Persons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Domain.Entities
{
    internal class LibraryPublication
    {
        
        public const int MaxDayOfBorrowBook = 10;
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public Publication Publication { get; private set; }
        // readerId -> Дата_взятия
        public Dictionary<int, DateTime> owners { get; } = new() { };

        public LibraryPublication(Publication publication) 
        { 
            Publication = publication;
        }

        private bool isBookOverBorrowed(DateTime today, DateTime borrowDate)
        {
            return (today - borrowDate).TotalDays > MaxDayOfBorrowBook;
        }
        public bool isBookOverBorrowedByPerson(DateTime today, int readerId)
        {
            bool res = false;
            if (owners.ContainsKey(readerId))
                res = isBookOverBorrowed(today, owners[readerId]);
            return res;
        } 
        public int CountOfMissingBooks(DateTime today)
        {
            int res = 0;

            foreach (var owner in owners)
                if (isBookOverBorrowed(today, owner.Value))
                    res++;
            return res;
        }
        public void AddCopiesOfPublication(int count)
        {
            TotalCopies += count;
            AvailableCopies += count;
        }
     
    }
}

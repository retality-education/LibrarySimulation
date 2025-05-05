using LibrarySimulation.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Domain.Entities
{
    internal class Publication
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public Theme Theme { get; set; }
        public PublicationType Type { get; set; }
        public override string ToString() => $"{Title} ({Author}, {Year})";
    }
}

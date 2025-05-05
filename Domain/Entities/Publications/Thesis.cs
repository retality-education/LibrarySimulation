using LibrarySimulation.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Domain.Entities.Publications
{
    internal class Thesis : Publication { 
        public Thesis() { 
            Type = PublicationType.Thesis; 
        } 
    }
}

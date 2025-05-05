using LibrarySimulation.Core.Enums;
using LibrarySimulation.Domain.Aggregates;
using LibrarySimulation.Domain.Entities;
using LibrarySimulation.Domain.Services.Factories;
using LibrarySimulation.Domain.Services;
using LibrarySimulation.Presentation.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrarySimulation.Domain.Entities.Persons;

namespace LibrarySimulation.Presentation.Controllers
{
    internal class LibraryController
    {
        private LibrarySimulator _simulator;
        private LibraryForm _view;

        public LibraryController(LibrarySimulator librarySimulator, LibraryForm view)
        {
            _view = view;

            _view.Controller = this;

            _simulator = librarySimulator;

            _simulator._library.Subscribe(view);

            _simulator.Start();
        }
    }
}

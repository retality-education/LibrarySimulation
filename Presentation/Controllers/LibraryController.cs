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
    //отвечает за взаимодействие между симулятором и пользовательским интерфейсом
    internal class LibraryController
    {
        private Library _library;//ссылка на объект симулятора
        private LibraryForm _view;//ссылка на объект интерфейса

        public LibraryController(Library library, LibraryForm view)
        {
            _view = view;

            _library = library;

            _view.Controller = this;//устанавливает ссылку на текущий контроллер в представлении

            _library.Subscribe(view);//подписывает представление на события библиотеки

            _library.Start();//запуск процесса работы библиотеки
        }

    }
}

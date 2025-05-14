using LibrarySimulation.Domain.Aggregates;
using LibrarySimulation.Domain.Services;
using LibrarySimulation.Presentation.Controllers;
using LibrarySimulation.Presentation.Views;

namespace LibrarySimulation
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var view = new LibraryForm();
            var library = new Library();

            var controller = new LibraryController(library, view);

            Application.Run(view);
        }
    }
}
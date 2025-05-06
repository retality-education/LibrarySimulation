using LibrarySimulation.Core.Enums;
using LibrarySimulation.Domain.Aggregates;
using LibrarySimulation.Domain.Entities;
using LibrarySimulation.Domain.Entities.Persons;
using LibrarySimulation.Domain.Entities.Publications;
using LibrarySimulation.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Threading.Timer;

namespace LibrarySimulation.Domain.Services
{
    internal class LibrarySimulator
    {
        public Library _library { get; private set; }
        private Random _random = new Random();
        private int _readersCount;
        private List<Reader> _allReaders = new ();
        private Dictionary<int, List<Publication>> _readerBorrowedBooks = new ();

        public LibrarySimulator(Library library)
        {
            _library = library;
            FillLibraryWithPublications();
        }
        private void FillLibraryWithPublications()
        {
            var libraryPublications = new List<Publication>
            {
                new Book { Title = "Война и мир", Author = "Лев Толстой", Year = 1869, Theme = Theme.Literature },
                new Book { Title = "Преступление и наказание", Author = "Фёдор Достоевский", Year = 1866, Theme = Theme.Literature },
                new Journal { Title = "National Geographic", Author = "Various", Year = 2023, Theme = Theme.Science },
                new Textbook { Title = "Основы программирования", Author = "Д. Кнут", Year = 2020, Theme = Theme.Technology },
                new Thesis { Title = "Квантовая механика", Author = "А. Эйнштейн", Year = 1924, Theme = Theme.Science },
                new Book { Title = "1984", Author = "Джордж Оруэлл", Year = 1949, Theme = Theme.Literature },
                new Journal { Title = "Nature", Author = "Various", Year = 2023, Theme = Theme.Science },
                new Textbook { Title = "Анатомия человека", Author = "И. Павлов", Year = 2018, Theme = Theme.Medicine },
                new Thesis { Title = "История Древнего Рима", Author = "М. Ростовцев", Year = 1918, Theme = Theme.History },
                new Book { Title = "Мастер и Маргарита", Author = "Михаил Булгаков", Year = 1967, Theme = Theme.Literature },
                new Journal { Title = "Forbes", Author = "Various", Year = 2023, Theme = Theme.Technology },
                new Textbook { Title = "Основы химии", Author = "Д. Менделеев", Year = 1869, Theme = Theme.Science },
                new Thesis { Title = "Теория относительности", Author = "А. Эйнштейн", Year = 1905, Theme = Theme.Science },
                new Book { Title = "Гарри Поттер и философский камень", Author = "Дж. К. Роулинг", Year = 1997, Theme = Theme.Literature },
                new Journal { Title = "Science", Author = "Various", Year = 2023, Theme = Theme.Science },
                new Textbook { Title = "История Средних веков", Author = "Л. Гумилёв", Year = 1980, Theme = Theme.History },
                new Thesis { Title = "Искусственный интеллект", Author = "А. Тьюринг", Year = 1950, Theme = Theme.Technology },
                new Book { Title = "Тихий Дон", Author = "Михаил Шолохов", Year = 1940, Theme = Theme.Literature },
                new Journal { Title = "The Lancet", Author = "Various", Year = 2023, Theme = Theme.Medicine },
                new Textbook { Title = "Физика для вузов", Author = "Р. Фейнман", Year = 1963, Theme = Theme.Science },
                new Thesis { Title = "Кибернетика", Author = "Н. Винер", Year = 1948, Theme = Theme.Technology },
                new Book { Title = "Анна Каренина", Author = "Лев Толстой", Year = 1877, Theme = Theme.Literature },
                new Journal { Title = "Time", Author = "Various", Year = 2023, Theme = Theme.History },
                new Textbook { Title = "Биология клетки", Author = "Б. Албертс", Year = 2002, Theme = Theme.Medicine },
                new Thesis { Title = "Квантовая электродинамика", Author = "Р. Фейнман", Year = 1949, Theme = Theme.Science }
            };

            foreach (var publication in libraryPublications)
                _library.AddNewPublication(publication, _random.Next(1, 5));
        }
        private void InitializeLibrarians()
        {
            _library.AddLibrarian("Анна Ивановна");
            _library.AddLibrarian("Петр Сергеевич");
        }

        // Определение количества читателей в зависимости от сезона
        private int GetReadersCountForSeason()
        {
            int month = _library.today.Month;

            // Осень (сентябрь-ноябрь) - больше читателей
            if (month >= 9 && month <= 11)
            {
                return _random.Next(5, 10);
            }
            // Лето (июнь-август) - меньше читателей
            else if (month >= 6 && month <= 8)
            {
                return _random.Next(1, 4);
            }
            // Весна (март-май) - среднее количество
            else if (month >= 3 && month <= 5)
            {
                return _random.Next(3, 7);
            }
            // Зима (декабрь-февраль) - среднее количество
            else
            {
                return _random.Next(2, 6);
            }
        }
        // Определение типа запроса в зависимости от сезона
        private RequestType GetRequestTypeBasedOnSeason(int readerId)
        {
            int month = _library.today.Month;
            bool hasBorrowedBooks = _readerBorrowedBooks.ContainsKey(readerId) &&
                                  _readerBorrowedBooks[readerId].Count > 0;

            if (!hasBorrowedBooks) return RequestType.Take;

            // Осень - чаще берут
            if (month >= 9 && month <= 11)
                return _random.Next(10) < 7 ? RequestType.Take : RequestType.Return;
            // Лето - чаще возвращают
            else if (month >= 6 && month <= 8)
                return _random.Next(10) < 3 ? RequestType.Take : RequestType.Return;
            // Другие сезоны - 50/50
            else
                return _random.Next(2) == 0 ? RequestType.Take : RequestType.Return;
        }

        private Reader GetOrCreateReader()
        {
            //
            if (_allReaders.Count > 0 && _random.Next(10) < 3)
            {
                return _allReaders[_random.Next(_allReaders.Count)];
            }

            // 
            _readersCount++;
            var newReader = new Reader($"Читатель_{_readersCount}");
            _allReaders.Add(newReader);
            _readerBorrowedBooks[newReader.Id] = new List<Publication>();
            return newReader;
        }

        private Reader GenerateNewReader()
        {
            var reader = GetOrCreateReader();
            reader.Requests = new PriorityQueue<Request, int>();

            // Получаем список взятых книг
            var borrowedBooks = _readerBorrowedBooks.ContainsKey(reader.Id) ?
                _readerBorrowedBooks[reader.Id] : new List<Publication>();

            int requestCount = _random.Next(1, 4);

            for (int i = 0; i < requestCount; i++)
            {
                RequestType requestType = GetRequestTypeBasedOnSeason(reader.Id);
                Publication selectedPub;

                if (requestType == RequestType.Return && borrowedBooks.Count > 0)
                    selectedPub = borrowedBooks[_random.Next(borrowedBooks.Count)];
                else
                    selectedPub = _library.Publications[_random.Next(_library.Publications.Count)].Publication;

                reader.Requests.Enqueue(new Request(requestType, selectedPub), (int)requestType);
            }

            return reader;
        }

        public void Start()
        {
            new Thread(StartSimulation).Start();
        }
        private void StartSimulation()
        {
            InitializeLibrarians();
            while (true)
            {
                // Увеличиваем день
                _library.today = _library.today.AddDays(15);

                int readersToGenerate = GetReadersCountForSeason();

                for (int i = 0; i < readersToGenerate; i++)
                {
                    var reader = GenerateNewReader();
                    if (reader.Requests.Count > 0)
                    {
                        _library.ReaderComeToLibrary(reader);
                    }
                }

                // Имитация работы библиотеки (читатели приходят и уходят)
                Thread.Sleep(5000); // Пауза между днями
            }
        }
    }
}

using LibrarySimulation.Core.Enums;
using LibrarySimulation.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarySimulation.Presentation.Views.Components
{
    internal class PersonVisual
    {
        public Person Person { get; }
        public Point Position { get; set; }
        public bool IsLibrarian { get; set; }
        public LibraryRequest CurrentRequest { get; set; }
        public DateTime RequestTime { get; set; }
        public bool ShowRequest { get; set; }
        public bool HasBook { get; set; }

        // Визуальные элементы
        public PictureBox MainBox { get; }
        public PictureBox CloudBox { get; }
        public PictureBox ResultBox { get; }
        public PictureBox BookBox { get; }

        public PersonVisual(Person person, Color color)
        {
            Person = person;

            // Основной бокс (персонаж)
            MainBox = new PictureBox
            {
                Size = new Size(50, 80),
                BackColor = color,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Облачко запроса
            CloudBox = new PictureBox
            {
                Size = new Size(40, 30),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            // Результат обработки
            ResultBox = new PictureBox
            {
                Size = new Size(20, 20),
                Visible = false
            };

            // Книга в руках
            BookBox = new PictureBox
            {
                Size = new Size(30, 20),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false
            };

            UpdatePositions();
        }

        public void UpdatePositions()
        {
            MainBox.Location = Position;
            CloudBox.Location = new Point(Position.X - 30, Position.Y - 40);
            ResultBox.Location = new Point(Position.X - 15, Position.Y - 20);
            BookBox.Location = new Point(Position.X - 20, Position.Y + 20);

            // Обновляем видимость элементов
            CloudBox.Visible = ShowRequest && (DateTime.Now - RequestTime).TotalSeconds < 3;
            BookBox.Visible = HasBook;
            ResultBox.Visible = CurrentRequest?.ProcessTime != null &&
                               (DateTime.Now - CurrentRequest.ProcessTime.Value).TotalSeconds < 2;

            if (ResultBox.Visible)
            {
                ResultBox.BackColor = CurrentRequest?.Status == RequestStatus.Approved
                    ? Color.LightGreen
                    : Color.Pink;
            }

            if (CloudBox.Visible)
            {
                if (CurrentRequest?.Type == RequestType.Take)
                {
                    CloudBox.BackColor = Color.White;
                }
                else
                {
                    CloudBox.BackColor = Color.LightGray;
                }
            }
        }
    }

}

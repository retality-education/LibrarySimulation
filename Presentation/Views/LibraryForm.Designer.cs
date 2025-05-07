namespace LibrarySimulation.Presentation.Views
{
    partial class LibraryForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Librarian2 = new PictureBox();
            polka1 = new PictureBox();
            polka2 = new PictureBox();
            BookShell = new PictureBox();
            Librarian1 = new PictureBox();
            LibrarianAnswer1 = new PictureBox();
            ReaderAnswer1 = new PictureBox();
            LibrarianAnswer2 = new PictureBox();
            ReaderAnswer2 = new PictureBox();
            gruzovik = new PictureBox();
            CountOfLostPublications = new Label();
            CountOfAvailablePublications = new Label();
            CurrentDate = new Label();
            ((System.ComponentModel.ISupportInitialize)Librarian2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)polka1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)polka2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BookShell).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Librarian1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)LibrarianAnswer1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ReaderAnswer1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)LibrarianAnswer2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)ReaderAnswer2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gruzovik).BeginInit();
            SuspendLayout();
            // 
            // Librarian2
            // 
            Librarian2.Image = Properties.Resources.Employee;
            Librarian2.Location = new Point(400, 415);
            Librarian2.Name = "Librarian2";
            Librarian2.Size = new Size(120, 140);
            Librarian2.SizeMode = PictureBoxSizeMode.StretchImage;
            Librarian2.TabIndex = 0;
            Librarian2.TabStop = false;
            // 
            // polka1
            // 
            polka1.Image = Properties.Resources.Stoika;
            polka1.Location = new Point(502, 182);
            polka1.Name = "polka1";
            polka1.Size = new Size(120, 78);
            polka1.SizeMode = PictureBoxSizeMode.StretchImage;
            polka1.TabIndex = 2;
            polka1.TabStop = false;
            // 
            // polka2
            // 
            polka2.Image = Properties.Resources.Stoika;
            polka2.Location = new Point(502, 477);
            polka2.Name = "polka2";
            polka2.Size = new Size(120, 78);
            polka2.SizeMode = PictureBoxSizeMode.StretchImage;
            polka2.TabIndex = 3;
            polka2.TabStop = false;
            // 
            // BookShell
            // 
            BookShell.Image = Properties.Resources.Library1;
            BookShell.Location = new Point(-68, 166);
            BookShell.Name = "BookShell";
            BookShell.Size = new Size(169, 243);
            BookShell.SizeMode = PictureBoxSizeMode.StretchImage;
            BookShell.TabIndex = 4;
            BookShell.TabStop = false;
            // 
            // Librarian1
            // 
            Librarian1.Image = Properties.Resources.Employee;
            Librarian1.Location = new Point(400, 120);
            Librarian1.Name = "Librarian1";
            Librarian1.Size = new Size(120, 140);
            Librarian1.SizeMode = PictureBoxSizeMode.StretchImage;
            Librarian1.TabIndex = 6;
            Librarian1.TabStop = false;
            // 
            // LibrarianAnswer1
            // 
            LibrarianAnswer1.Location = new Point(400, 52);
            LibrarianAnswer1.Name = "LibrarianAnswer1";
            LibrarianAnswer1.Size = new Size(125, 62);
            LibrarianAnswer1.SizeMode = PictureBoxSizeMode.StretchImage;
            LibrarianAnswer1.TabIndex = 7;
            LibrarianAnswer1.TabStop = false;
            // 
            // ReaderAnswer1
            // 
            ReaderAnswer1.Location = new Point(620, 52);
            ReaderAnswer1.Name = "ReaderAnswer1";
            ReaderAnswer1.Size = new Size(125, 62);
            ReaderAnswer1.SizeMode = PictureBoxSizeMode.StretchImage;
            ReaderAnswer1.TabIndex = 8;
            ReaderAnswer1.TabStop = false;
            // 
            // LibrarianAnswer2
            // 
            LibrarianAnswer2.Location = new Point(400, 347);
            LibrarianAnswer2.Name = "LibrarianAnswer2";
            LibrarianAnswer2.Size = new Size(125, 62);
            LibrarianAnswer2.SizeMode = PictureBoxSizeMode.StretchImage;
            LibrarianAnswer2.TabIndex = 9;
            LibrarianAnswer2.TabStop = false;
            // 
            // ReaderAnswer2
            // 
            ReaderAnswer2.Location = new Point(620, 347);
            ReaderAnswer2.Name = "ReaderAnswer2";
            ReaderAnswer2.Size = new Size(125, 62);
            ReaderAnswer2.SizeMode = PictureBoxSizeMode.StretchImage;
            ReaderAnswer2.TabIndex = 10;
            ReaderAnswer2.TabStop = false;
            // 
            // gruzovik
            // 
            gruzovik.Image = Properties.Resources.gruzovik;
            gruzovik.Location = new Point(2, 654);
            gruzovik.Name = "gruzovik";
            gruzovik.Size = new Size(233, 186);
            gruzovik.SizeMode = PictureBoxSizeMode.StretchImage;
            gruzovik.TabIndex = 11;
            gruzovik.TabStop = false;
            // 
            // CountOfLostPublications
            // 
            CountOfLostPublications.AutoSize = true;
            CountOfLostPublications.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 204);
            CountOfLostPublications.Location = new Point(12, 34);
            CountOfLostPublications.Name = "CountOfLostPublications";
            CountOfLostPublications.Size = new Size(320, 25);
            CountOfLostPublications.TabIndex = 12;
            CountOfLostPublications.Text = "Кол-во потерянных публикаций: 0";
            // 
            // CountOfAvailablePublications
            // 
            CountOfAvailablePublications.AutoSize = true;
            CountOfAvailablePublications.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 204);
            CountOfAvailablePublications.Location = new Point(12, 9);
            CountOfAvailablePublications.Name = "CountOfAvailablePublications";
            CountOfAvailablePublications.Size = new Size(308, 25);
            CountOfAvailablePublications.TabIndex = 13;
            CountOfAvailablePublications.Text = "Кол-во доступных публикаций: 0";
            // 
            // CurrentDate
            // 
            CurrentDate.AutoSize = true;
            CurrentDate.Font = new Font("Segoe UI", 10.8F, FontStyle.Bold, GraphicsUnit.Point, 204);
            CurrentDate.Location = new Point(876, 9);
            CurrentDate.Name = "CurrentDate";
            CurrentDate.Size = new Size(137, 25);
            CurrentDate.TabIndex = 14;
            CurrentDate.Text = "Текущая дата:";
            // 
            // LibraryForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Info;
            ClientSize = new Size(1162, 671);
            Controls.Add(CurrentDate);
            Controls.Add(CountOfAvailablePublications);
            Controls.Add(CountOfLostPublications);
            Controls.Add(gruzovik);
            Controls.Add(ReaderAnswer2);
            Controls.Add(LibrarianAnswer2);
            Controls.Add(ReaderAnswer1);
            Controls.Add(LibrarianAnswer1);
            Controls.Add(Librarian1);
            Controls.Add(Librarian2);
            Controls.Add(BookShell);
            Controls.Add(polka2);
            Controls.Add(polka1);
            Name = "LibraryForm";
            Text = "LibraryForm";
            ((System.ComponentModel.ISupportInitialize)Librarian2).EndInit();
            ((System.ComponentModel.ISupportInitialize)polka1).EndInit();
            ((System.ComponentModel.ISupportInitialize)polka2).EndInit();
            ((System.ComponentModel.ISupportInitialize)BookShell).EndInit();
            ((System.ComponentModel.ISupportInitialize)Librarian1).EndInit();
            ((System.ComponentModel.ISupportInitialize)LibrarianAnswer1).EndInit();
            ((System.ComponentModel.ISupportInitialize)ReaderAnswer1).EndInit();
            ((System.ComponentModel.ISupportInitialize)LibrarianAnswer2).EndInit();
            ((System.ComponentModel.ISupportInitialize)ReaderAnswer2).EndInit();
            ((System.ComponentModel.ISupportInitialize)gruzovik).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox Librarian2;
        private PictureBox polka1;
        private PictureBox polka2;
        private PictureBox BookShell;
        private PictureBox Librarian1;
        private PictureBox LibrarianAnswer1;
        private PictureBox ReaderAnswer1;
        private PictureBox LibrarianAnswer2;
        private PictureBox ReaderAnswer2;
        private PictureBox gruzovik;
        private Label CountOfLostPublications;
        private Label CountOfAvailablePublications;
        private Label CurrentDate;
    }
}
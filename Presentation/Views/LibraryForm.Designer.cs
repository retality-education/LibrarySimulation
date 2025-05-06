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
            Librarian1 = new PictureBox();
            polka1 = new PictureBox();
            polka2 = new PictureBox();
            BookShell = new PictureBox();
            pictureBox1 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)Librarian1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)polka1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)polka2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)BookShell).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // Librarian1
            // 
            Librarian1.Image = Properties.Resources.Employee;
            Librarian1.Location = new Point(388, 375);
            Librarian1.Name = "Librarian1";
            Librarian1.Size = new Size(114, 137);
            Librarian1.SizeMode = PictureBoxSizeMode.StretchImage;
            Librarian1.TabIndex = 0;
            Librarian1.TabStop = false;
            // 
            // polka1
            // 
            polka1.Image = Properties.Resources.Stoika;
            polka1.Location = new Point(478, 173);
            polka1.Name = "polka1";
            polka1.Size = new Size(120, 78);
            polka1.SizeMode = PictureBoxSizeMode.StretchImage;
            polka1.TabIndex = 2;
            polka1.TabStop = false;
            // 
            // polka2
            // 
            polka2.Image = Properties.Resources.Stoika;
            polka2.Location = new Point(478, 434);
            polka2.Name = "polka2";
            polka2.Size = new Size(120, 78);
            polka2.SizeMode = PictureBoxSizeMode.StretchImage;
            polka2.TabIndex = 3;
            polka2.TabStop = false;
            // 
            // BookShell
            // 
            BookShell.Image = Properties.Resources.Library;
            BookShell.Location = new Point(-179, 120);
            BookShell.Name = "BookShell";
            BookShell.Size = new Size(417, 243);
            BookShell.SizeMode = PictureBoxSizeMode.StretchImage;
            BookShell.TabIndex = 4;
            BookShell.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.Employee;
            pictureBox1.Location = new Point(388, 114);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(114, 137);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 6;
            pictureBox1.TabStop = false;
            // 
            // LibraryForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Info;
            ClientSize = new Size(1179, 524);
            Controls.Add(pictureBox1);
            Controls.Add(Librarian1);
            Controls.Add(BookShell);
            Controls.Add(polka2);
            Controls.Add(polka1);
            Name = "LibraryForm";
            Text = "LibraryForm";
            ((System.ComponentModel.ISupportInitialize)Librarian1).EndInit();
            ((System.ComponentModel.ISupportInitialize)polka1).EndInit();
            ((System.ComponentModel.ISupportInitialize)polka2).EndInit();
            ((System.ComponentModel.ISupportInitialize)BookShell).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private PictureBox Librarian1;
        private PictureBox polka1;
        private PictureBox polka2;
        private PictureBox BookShell;
        private PictureBox pictureBox1;
    }
}
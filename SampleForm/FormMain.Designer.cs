
namespace SampleForm
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainWindow1 = new SampleForm.MainWindow();
            this.SuspendLayout();
            // 
            // mainWindow1
            // 
            this.mainWindow1.BackColor = System.Drawing.Color.Black;
            this.mainWindow1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainWindow1.Interval = 10;
            this.mainWindow1.Location = new System.Drawing.Point(0, 0);
            this.mainWindow1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mainWindow1.Name = "mainWindow1";
            this.mainWindow1.Size = new System.Drawing.Size(800, 480);
            this.mainWindow1.TabIndex = 0;
            this.mainWindow1.VSync = true;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 480);
            this.Controls.Add(this.mainWindow1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormMain";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        public MainWindow mainWindow1;
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SampleForm
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        
        public static FormMain MainForm { get; private set; }
        public static MainWindow MainWindow => MainForm.mainWindow1;

        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm = new FormMain();
            Application.Run(MainForm);
        }
    }
}

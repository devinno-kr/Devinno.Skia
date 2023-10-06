using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Windowing.Common;

namespace SampleRPi
{
    public class Program
    {
        public static MainWindow MainWindow { get; private set; }

        static void Main(string[] args)
        {
            using (var view = new MainWindow { })
            {
                MainWindow = view;
                view.VSync = VSyncMode.On;
                view.CenterWindow();
                view.Run();
                MainWindow = null;
            }
        }
    }
}

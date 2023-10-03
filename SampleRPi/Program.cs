using OpenTK.Windowing.Common;
using System.Linq;

namespace SampleRPi
{
    public class Program
    {
        public static MainWindow MainWindow { get; private set; }

        static void Main(string[] args)
        {
            bool[] bs = new bool[] { true, false, true, false, false };
            var ls = bs.OrderBy(x => x).ToList();

            using (var view = new MainWindow())
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

using OpenTK.Windowing.Common;

namespace SampleRPi
{
    public class Program
    {
        public static MainWindow MainWindow { get; private set; }

        static void Main(string[] args)
        {
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

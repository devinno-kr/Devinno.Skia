using Devinno.Skia.OpenTK;

namespace SampleRPi
{
    public partial class MainWindow : DvViewWindow
    {
        public MainWindow() : base(W, H)
        {
            InitializeComponent();

            Design.SetPage(PageControl);
        }
    }
}

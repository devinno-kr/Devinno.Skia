using Devinno.Skia.Design;
using Devinno.Skia.Utils;

namespace SampleRPi.Pages
{
    public partial class PageControl : DvPage
    {
        public PageControl()
        {
            InitializeComponent();

            chk1.CheckedChanged += (o, s) => lmp.OnOff = chk1.Checked;
            tbtn1.CheckedChanged += (o, s) => lbtn.OnOff = tbtn1.Checked;

            ani.OffImage = Util.FromBitmap("./Images/anioff.png");
            ani.LoadGIF("./Images/ani.gif");
            ani.MouseClick += (o, s) => ani.OnOff = !ani.OnOff;
        }
    }
}

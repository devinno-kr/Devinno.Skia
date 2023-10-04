using System;
using Devinno.Skia.Utils;
using Devinno.Skia.Controls;
using Devinno.Skia.Containers;
using Devinno.Skia.Design;
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

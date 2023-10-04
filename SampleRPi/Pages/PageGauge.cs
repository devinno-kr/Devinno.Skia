using System;
using Devinno.Skia.Design;

namespace SampleRPi.Pages
{
    public partial class PageGauge : DvPage
    {
        public PageGauge()
        {
            InitializeComponent();

            prgsH.BarSize = prgsV.BarSize = 30;
            sldH.Tick = sldV.Tick = 10;

            knob.ValueChanged += (o, s) =>
            {
                prgsH.Value = prgsV.Value = gauge.Value = meter.Value = knob.Value;
            };
        }
    }
}

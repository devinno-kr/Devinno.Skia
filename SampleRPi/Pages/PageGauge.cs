using System;
using Devinno.Skia.Design;

namespace SampleRPi.Pages
{
    public partial class PageGauge : DvPage
    {
        public PageGauge()
        {
            InitializeComponent();
 
            knob.ValueChanged += (o, s) =>
            {
                prgsH.Value = prgsV.Value = gauge.Value = meter.Value = knob.Value;
            };
        }
    }
}

using System;
using Devinno.Skia.Design;
using Devinno.Skia.Tools;

namespace SampleRPi.Pages
{
    public partial class PageGauge : DvPage
    {
        DateTime prev = DateTime.Now;

        public PageGauge()
        {
            InitializeComponent();
        }

        protected override void OnUpdate()
        {
            meter.Value = gauge.Value = knob.Value;
            prgsH.Value = prgsV.Value = Math.Abs(MathTool.Map((DateTime.Now - prev).TotalMilliseconds % 2000, 0, 2000, 0, 200) - 100);
            
            base.OnUpdate();
        }
    }
}

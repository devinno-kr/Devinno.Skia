using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devinno.Skia.Design;
using Devinno.Skia.Tools;

namespace SampleRPi.Pages
{
    public partial class PageGauge : DvPage
    {
        #region Member Variable
        DateTime prev = DateTime.Now;
        #endregion

        #region Constructor
        public PageGauge()
        {
            InitializeComponent();
            knob.FillColor = SkiaSharp.SKColors.Orange;
            knob.ForeColor = SkiaSharp.SKColors.Aqua;
            knob.OnLampColor = SkiaSharp.SKColors.Orange;
        }
        #endregion

        #region Override
        #region OnUpdate
        protected override void OnUpdate()
        {
            meter.Value = gauge.Value = knob.Value;
            prgsH.Value = prgsV.Value = Math.Abs(MathTool.Map((DateTime.Now - prev).TotalMilliseconds % 2000, 0, 2000, 0, 200) - 100);
            
            base.OnUpdate();
        }
        #endregion
        #endregion
    }
}

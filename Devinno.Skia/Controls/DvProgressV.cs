using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
using Devinno.Skia.Theme;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvProgressV : DvControl
    {
        #region Properties
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? BarColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;

        public double Minimum { get; set; } = 0D;
        public double Maximum { get; set; } = 100D;
        public double Value { get; set; } = 0D;

        public string FormatString { get; set; } = "0";
        public bool Reverse { get; set; } = false;
        public bool DrawText { get; set; } = true;

        #region Text
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 10;
        #endregion
        #endregion

        #region Member Variable

        #endregion

        #region Constructor
        public DvProgressV()
        {
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            try
            {
                bounds((rtContent, rtEmpty, rtFill) =>
                {
                    var ds = Design;
                    var thm = Design?.Theme;
                    if (ds != null && thm != null)
                    {
                        var BoxColor = this.BoxColor ?? thm.ConcaveBoxColor;
                        var BarColor = this.BarColor ?? thm.PointColor;
                        var ForeColor = this.ForeColor ?? thm.ForeColor;
                        var BorderColor = thm.GetBorderColor(BarColor, BoxColor);
                        var BoxBorderColor = thm.GetBorderColor(BoxColor, ParentContainer.GetBackColor());

                        using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                        {
                            thm.DrawBox(Canvas, rtContent, BoxColor, BoxBorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel);

                            if (rtFill.Height > 0)
                                thm.DrawBox(Canvas, rtFill, BarColor, BorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.OutShadow | BoxStyle.InBevel | BoxStyle.GradientH);

                            if (DrawText)
                            {
                                var sz = new SKRect();
                                var s = string.IsNullOrWhiteSpace(FormatString) ? Value.ToString() : Value.ToString(FormatString);
                                p.MeasureText(s, ref sz);
                                var h = Convert.ToInt32(sz.Height + 5);
                                var rt = Util.FromRect(rtFill.Left - 1, Reverse ? rtFill.Bottom - h : rtFill.Top, rtFill.Width, h);

                                if (rtFill.Width > 0)
                                {
                                    var sp = Canvas.Save();
                                    Canvas.ClipRect(rtFill);

                                    Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, ForeColor, rt, Reverse ? DvContentAlignment.TopCenter : DvContentAlignment.BottomCenter);

                                    Canvas.RestoreToCount(sp);
                                }
                            }
                        }
                    }
                });
            }
            catch { }

            base.OnDraw(Canvas);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);

            var ng = Convert.ToInt32(rtContent.Width * 0.1F);

            var rtEmpty = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, rtContent.Height); 
            var hF = Convert.ToInt32(MathTool.Map(Value, Minimum, Maximum, 0, rtEmpty.Height));
            var rtFill = Util.FromRect(rtEmpty.Left, Reverse ? rtEmpty.Top : rtEmpty.Bottom - hF, rtEmpty.Width, hF);
            rtFill.Inflate(-ng, -ng);

            act(rtContent, rtEmpty, rtFill);
        }
        #endregion
        #endregion
    }
}

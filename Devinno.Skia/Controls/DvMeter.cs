using Devinno.Skia.Design;
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
    public class DvMeter : Design.DvControl
    {
        #region Properties
        #region Color
        public SKColor? NeedleColor { get; set; } = null;
        public SKColor? NeedlePointColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        #endregion
        #region Value
        public double Minimum { get; set; } = 0D;
        public double Maximum { get; set; } = 100D;
        public double Value { get; set; } = 0D;
        public double GraduationLarge { get; set; } = 10;
        public double GraduationSmall { get; set; } = 2;

        public string FormatString { get; set; } = "0";
        #endregion
        #region Angle
        public float StartAngle { get; set; } = 135F;
        public float SweepAngle { get; set; } = 270;
        #endregion
        #region Text
        public bool DrawText { get; set; } = true;
        public float TextOffsetX { get; set; } = 0;
        public float TextOffsetY { get; set; } = 0;

        public string ValueFontName { get; set; } = "NanumGothic";
        public float ValueFontSize { get; set; } = 18;
        public DvFontStyle ValueFontStyle { get; set; } = DvFontStyle.Normal;

        public string RemarkFontName { get; set; } = "NanumGothic";
        public float RemarkFontSize { get; set; } = 10;
        public DvFontStyle RemarkFontStyle { get; set; } = DvFontStyle.Normal;

        public string Title { get; set; } = "";
        public string TitleFontName { get; set; } = "NanumGothic";
        public float TitleFontSize { get; set; } = 10;
        public DvFontStyle TitleFontStyle { get; set; } = DvFontStyle.Normal;
        #endregion
        #region Bars
        public List<MeterBar> Bars { get; private set; } = new List<MeterBar>();
        #endregion
        #region Gap
        public float Gap { get; set; } = 5;
        #endregion
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtGauge, rtValue, rtTitle) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var NeedleColor = this.NeedleColor ?? thm.NeedelColor;
                    var NeedlePointColor = this.NeedlePointColor ?? SKColors.Red;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();

                    var text = string.IsNullOrWhiteSpace(FormatString) ? Value.ToString() : Value.ToString(FormatString);

                    thm.DrawMeter(Canvas,
                        rtContent, rtGauge, rtValue, rtTitle,
                        NeedleColor, NeedlePointColor, ForeColor, BackColor,
                        Minimum, Maximum, Value, GraduationLarge, GraduationSmall,
                        DrawText,
                        text, ValueFontName, ValueFontSize, ValueFontStyle,
                        Title, TitleFontName, TitleFontSize, TitleFontStyle,
                        RemarkFontName, RemarkFontSize, RemarkFontStyle,
                        StartAngle, SweepAngle, Bars);
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect> act)
        {
            var wh = Math.Min(Width, Height);
            var rtContent = Util.FromRect(0, 0, Width, Height);
            var rtGauge = Util.MakeRectangle(rtContent, new SKSize(wh, wh));

            var cp = MathTool.CenterPoint(rtGauge);
            var vh = ValueFontSize;
            var th = TitleFontSize;

            var pt = MathTool.GetPointWithAngle(cp, StartAngle - ((360F - SweepAngle) / 2F), (wh / 2F) - ((vh + Gap + th) / 2F));

            var rt = Util.MakeRectangle(pt, 150, vh + Gap + th);
            rt.Offset(TextOffsetX, TextOffsetY);

            var rtValue = Util.MakeRectangleAlign(rt, new SKSize(rt.Width, vh), DvContentAlignment.TopCenter);
            var rtTitle = Util.MakeRectangleAlign(rt, new SKSize(rt.Width, th), DvContentAlignment.BottomCenter);

            act(rtContent, rtGauge, rtValue, rtTitle);
        }
        #endregion
        #endregion
    }
}

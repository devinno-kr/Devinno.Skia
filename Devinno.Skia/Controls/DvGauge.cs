using Devinno.Skia.Design;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvGauge : Design.DvControl
    {
        #region Properties
        #region Color
        public SKColor? FillColor { get; set; } = null;
        public SKColor? EmptyColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        #endregion
        #region Value
        public double Minimum { get; set; } = 0D;
        public double Maximum { get; set; } = 100D;
        public double Value { get; set; } = 0D;
        public string FormatString { get; set; } = "0";
        #endregion
        #region Angle
        public float StartAngle { get; set; } = 135F;
        public float SweepAngle { get; set; } = 270;
        #endregion
        #region Text
        public string ValueFontName { get; set; } = "NanumGothic";
        public float ValueFontSize { get; set; } = 18;
        public DvFontStyle ValueFontStyle { get; set; } = DvFontStyle.Normal;

        public string Title { get; set; } = "";
        public string TitleFontName { get; set; } = "NanumGothic";
        public float TitleFontSize { get; set; } = 10;
        public DvFontStyle TitleFontStyle { get; set; } = DvFontStyle.Normal;
        #endregion
        #region BarSize
        public float BarSize { get; set; } = 20;
        #endregion
        #region BarPadding
        public float BarPadding { get; set; } = 3;
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
                    var FillColor = this.FillColor ?? thm.PointColor;
                    var EmptyColor = this.EmptyColor ?? thm.ControlBackColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var text = string.IsNullOrWhiteSpace(FormatString) ? Value.ToString() : Value.ToString(FormatString);
                    
                    thm.DrawGauge(Canvas,
                        rtContent, rtGauge, rtValue, rtTitle,
                        FillColor, EmptyColor, ForeColor, BackColor,
                        Minimum, Maximum, Value, 
                        text, ValueFontName, ValueFontSize, ValueFontStyle,
                        Title, TitleFontName, TitleFontSize, TitleFontStyle,
                        StartAngle, SweepAngle, BarSize, BarPadding
                        );
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

            var vh = ValueFontSize;
            var th = TitleFontSize;
            var rt = Util.MakeRectangleAlign(rtContent, new SKSize(rtContent.Width, vh + Gap + th), DvContentAlignment.MiddleCenter);

            var rtValue = Util.FromRect(0, rt.Top, rt.Width, vh);
            var rtTitle = Util.FromRect(0, rt.Top + vh + Gap, rt.Width, th);

            act(rtContent, rtGauge, rtValue, rtTitle);
        }
        #endregion
        #endregion
    }
}

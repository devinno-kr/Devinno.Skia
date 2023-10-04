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
    public class DvProgress : Design.DvControl
    {
        #region Properties
        #region Color
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? BarColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        #endregion
        #region Value
        public double Minimum { get; set; } = 0D;
        public double Maximum { get; set; } = 100D;
        public double Value { get; set; } = 0D;
        #endregion
        #region Direction
        public DvDirectionHV Direction { get; set; } = DvDirectionHV.Horizon;
        #endregion
        #region Text
        public bool DrawText { get; set; } = true;
        public bool Reverse { get; set; } = false;
        public string FormatString { get; set; } = "0";
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 10;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        #endregion
        #region BarPadding
        public float BarPadding { get; set; } = 5;
        #endregion
        #region BarSize
        public int? BarSize { get; set; } = null;
        #endregion
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            try
            {
                Areas((rtContent, rtEmpty, rtFill) =>
                {
                    var ds = Design;
                    var thm = Design?.Theme;
                    if (ds != null && thm != null)
                    {
                        var BoxColor = this.BoxColor ?? thm.ControlBackColor;
                        var BarColor = this.BarColor ?? thm.PointColor;
                        var ForeColor = this.ForeColor ?? thm.ForeColor;
                        var BackColor = ParentContainer.GetBackColor();

                        var text = string.IsNullOrWhiteSpace(FormatString) ? Value.ToString() : Value.ToString(FormatString);
                        thm.DrawProgress(Canvas,
                            rtContent, rtEmpty, rtFill,
                            BoxColor, BarColor, ForeColor, BackColor,
                            Direction, DrawText, Reverse,
                            text, FontName, FontSize, FontStyle);
                    }
                });
            }
            catch { }

            base.OnDraw(Canvas);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width, this.Height);
            var rtEmpty = Util.FromRect(rtContent);
            if (BarSize.HasValue)
                rtEmpty = MathTool.MakeRectangle(rtContent, Direction == DvDirectionHV.Vertical ?
                    new SKSize(BarSize.Value, rtContent.Height) : new SKSize(rtContent.Width, BarSize.Value)); 
         
            if (Direction == DvDirectionHV.Horizon)
            {
                var rtv = Util.FromRect(rtEmpty);
                rtv.Inflate(-BarPadding, -BarPadding);

                var wF = Convert.ToInt32(MathTool.Map(Value, Minimum, Maximum, 0, rtv.Width));
                var rtFill = Util.FromRect(Reverse ? rtv.Right - wF : rtv.Left, rtv.Top, wF, rtv.Height);

                act(rtContent, rtEmpty, rtFill);
            }
            else if(Direction == DvDirectionHV.Vertical)
            {
                var rtv = Util.FromRect(rtEmpty);
                rtv.Inflate(-BarPadding, -BarPadding);

                var hF = Convert.ToInt32(MathTool.Map(Value, Minimum, Maximum, 0, rtv.Height));
                var rtFill = Util.FromRect(rtv.Left, Reverse ? rtv.Top : rtv.Bottom - hF, rtv.Width, hF);

                act(rtContent, rtEmpty, rtFill);
            }

        }
        #endregion
        #endregion
    }
}

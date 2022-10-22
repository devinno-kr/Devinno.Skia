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
    public class DvSliderV : DvControl
    {
        #region Properties
        public SKColor? CursorColor { get; set; } = null;
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? BarColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;

        public double Minimum { get; set; } = 0D;
        public double Maximum { get; set; } = 100D;
        #region public double Value { get; set; }
        private double nVal = 0D;
        public double Value
        {
            get => nVal;
            set
            {
                if(nVal != value)
                {
                    nVal = value;
                    ValueChanged?.Invoke(this, null);
                }
            }
        }
        #endregion
        public double Tick { get; set; } = 0;

        public string FormatString { get; set; } = "0";
        public bool Reverse { get; set; } = false;
        public bool DrawText { get; set; } = true;

        #region Text
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 10;
        #endregion
        #endregion

        #region Event
        public event EventHandler ValueChanged;
        public event EventHandler CursorDown;
        public event EventHandler CursorUp;
        #endregion

        #region Member Variable
        bool bDown = false;
        int dx = 0;
        int dy = 0;
        #endregion

        #region Constructor
        public DvSliderV()
        {
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtBack, rtBar, rtCursor, rtA1, rtA2, rtA3) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var BoxColor = this.BoxColor ?? thm.ConcaveBoxColor;
                    var BarColor = this.BarColor ?? thm.PointColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var CursorColor = this.CursorColor ?? thm.ButtonColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var BorderColor = thm.GetBorderColor(BoxColor, BarColor);
                    var BoxBorderColor = thm.GetBorderColor(BoxColor, BackColor);
                    var CursorBorderColor = thm.GetBorderColor(CursorColor, BoxColor);

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        #region Tick            
                        if (Tick != 0)
                        {
                            for (double i = Minimum + Tick; i < Maximum; i += Tick)
                            {
                                float y = 0;
                                if (!Reverse)
                                    y = Convert.ToInt32(MathTool.Map(i, Minimum, Maximum, rtBack.Bottom, rtBack.Top));
                                else
                                    y = Convert.ToInt32(MathTool.Map(i, Minimum, Maximum, rtBack.Top, rtBack.Bottom));

                                y += 0.5F;

                                var nv = 7;
                                p.IsStroke = true;
                                p.StrokeWidth = 1;
                                p.Color = Util.FromArgb(15, SKColors.White);
                                Canvas.DrawLine(rtBar.Left - nv, y + 1, rtBar.Right + nv, y + 1, p);

                                p.Color = BoxBorderColor;
                                Canvas.DrawLine(rtBar.Left - nv, y, rtBar.Right + nv, y, p);
                            }
                        }
                        #endregion

                        thm.DrawBox(Canvas, rtBack, BoxColor, BoxBorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel);
                        thm.DrawBox(Canvas, rtBar, BarColor, BorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.GradientH | BoxStyle.InBevel | BoxStyle.OutShadow);
                       
                        #region Cursor
                        var cc = bDown ? CursorColor.BrightnessTransmit(thm.DownBrightness * -1) : CursorColor;

                        thm.DrawBox(Canvas,rtCursor, cc, CursorBorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow);

                        if (DrawText)
                        {
                            var s = string.IsNullOrWhiteSpace(FormatString) ? Value.ToString() : Value.ToString(FormatString);

                            var sp = Canvas.Save();
                            Canvas.ClipRect(rtCursor);
                            Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtCursor, DvContentAlignment.MiddleCenter, true);
                            Canvas.RestoreToCount(sp);
                        }
                        else
                        {
                            var cD = cc.BrightnessTransmit(-0.6F);
                            var cL = cc.BrightnessTransmit(0.3F);

                            p.IsStroke = false;
                            p.Color = cc.BrightnessTransmit(-0.1F);
                            Canvas.DrawRect(rtA1, p);
                            Canvas.DrawRect(rtA2, p);
                            Canvas.DrawRect(rtA3, p);

                            p.IsStroke = true;
                            p.StrokeWidth = 1;

                            rtA1.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                            rtA2.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                            rtA3.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);

                            p.Color = cD;
                            Canvas.DrawLine(rtA1.Left, rtA1.Top, rtA1.Left, rtA1.Bottom, p); Canvas.DrawLine(rtA1.Left, rtA1.Top, rtA1.Right, rtA1.Top, p);
                            Canvas.DrawLine(rtA2.Left, rtA2.Top, rtA2.Left, rtA2.Bottom, p); Canvas.DrawLine(rtA2.Left, rtA2.Top, rtA2.Right, rtA2.Top, p);
                            Canvas.DrawLine(rtA3.Left, rtA3.Top, rtA3.Left, rtA3.Bottom, p); Canvas.DrawLine(rtA3.Left, rtA3.Top, rtA3.Right, rtA3.Top, p);

                            p.Color = cL;
                            Canvas.DrawLine(rtA1.Right, rtA1.Top, rtA1.Right, rtA1.Bottom, p); Canvas.DrawLine(rtA1.Left, rtA1.Bottom, rtA1.Right, rtA1.Bottom, p);
                            Canvas.DrawLine(rtA2.Right, rtA2.Top, rtA2.Right, rtA2.Bottom, p); Canvas.DrawLine(rtA2.Left, rtA2.Bottom, rtA2.Right, rtA2.Bottom, p);
                            Canvas.DrawLine(rtA3.Right, rtA3.Top, rtA3.Right, rtA3.Bottom, p); Canvas.DrawLine(rtA3.Left, rtA3.Bottom, rtA3.Right, rtA3.Bottom, p);
                        }
                        #endregion
                    }
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            dx = x;
            dy = y;

            bounds((rtContent, rtBack, rtBar, rtCursor, rtA1, rtA2, rtA3) =>
            {
                if(CollisionTool.Check(rtCursor, x,y))
                {
                    bDown = true;
                    CursorDown?.Invoke(this, null);
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(int x, int y)
        {
            bounds((rtContent, rtBack, rtBar, rtCursor, rtA1, rtA2, rtA3) =>
            {
                if (bDown)
                {
                    if (!Reverse)
                        Value = MathTool.Map(MathTool.Constrain(y, rtBack.Top, rtBack.Bottom), rtBack.Top, rtBack.Bottom, Maximum, Minimum);
                    else
                        Value = MathTool.Map(MathTool.Constrain(y, rtBack.Top, rtBack.Bottom), rtBack.Top, rtBack.Bottom, Minimum, Maximum);

                    if (Tick != 0) Value = Math.Round(Value / Tick) * Tick;
                }
            });
            base.OnMouseMove(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            bounds((rtContent, rtBack, rtBar, rtCursor, rtA1, rtA2, rtA3) =>
            {
                if (bDown)
                {
                    bDown = false;
                    if (!Reverse)
                        Value = MathTool.Map(MathTool.Constrain(y, rtBack.Top, rtBack.Bottom), rtBack.Top, rtBack.Bottom, Maximum, Minimum);
                    else
                        Value = MathTool.Map(MathTool.Constrain(y, rtBack.Top, rtBack.Bottom), rtBack.Top, rtBack.Bottom, Minimum, Maximum);

                    if (Tick != 0) Value = Math.Round(Value / Tick) * Tick;

                    CursorUp?.Invoke(this, null);
                }
                else if (Math.Abs(MathTool.GetDistance(new SKPoint(x,y), new SKPoint(dx, dy))) < 10)
                {
                    if (!Reverse)
                        Value = MathTool.Map(MathTool.Constrain(y, rtBack.Top, rtBack.Bottom), rtBack.Top, rtBack.Bottom, Maximum, Minimum);
                    else
                        Value = MathTool.Map(MathTool.Constrain(y, rtBack.Top, rtBack.Bottom), rtBack.Top, rtBack.Bottom, Minimum, Maximum);

                    if (Tick != 0) Value = Math.Round(Value / Tick) * Tick;
                }
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
            var rtBack = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, rtContent.Height); rtBack.Inflate(-(rtContent.Width / 4), -(rtContent.Width / 2));

            var ng = Convert.ToInt32(rtBack.Width * 0.1F);
            var rtEmpty = Util.FromRect(rtBack.Left, rtBack.Top, rtBack.Width, rtBack.Height); rtEmpty.Inflate(-ng, -ng);
            var rtBar = new SKRect();

            var cp = MathTool.CenterPoint(rtEmpty);
            var ny = 0;
            if (!Reverse) ny = Convert.ToInt32(MathTool.Map(Value, Minimum, Maximum, rtEmpty.Bottom, rtEmpty.Top));
            else ny = Convert.ToInt32(MathTool.Map(Value, Minimum, Maximum, rtEmpty.Top, rtEmpty.Bottom));
            cp.Y = ny;

            if (!Reverse)
            {
                rtBar = Util.FromRect(rtEmpty.Left, ny, rtEmpty.Width, rtEmpty.Bottom - ny);
            }
            else
            {
                rtBar = Util.FromRect(rtEmpty.Left, rtEmpty.Top, rtEmpty.Width, ny - rtEmpty.Top);
            }
            
            var rtCursor = MathTool.MakeRectangle(cp, rtContent.Width);
            
            var rg = Convert.ToInt32(rtCursor.Height / 5D / 3D);
            var rtCursorAch = Util.MakeRectangleAlign(rtCursor, new SKSize(rg * 3 + 4, rg * 3 + 4), DvContentAlignment.MiddleCenter);
            var rtCursorAch1 = Util.FromRect(rtCursorAch.Left, rtCursorAch.Top, rg, rtCursorAch.Height);
            var rtCursorAch2 = Util.FromRect(rtCursorAch.Left + rg + 2, rtCursorAch.Top, rg, rtCursorAch.Height);
            var rtCursorAch3 = Util.FromRect(rtCursorAch.Left + rg + 2 + rg + 2, rtCursorAch.Top, rg, rtCursorAch.Height);

            rtBack = Util.INT(rtBack);
            rtBar = Util.INT(rtBar);
            rtCursor = Util.INT(rtCursor);
            rtCursorAch1 = Util.INT(rtCursorAch1);
            rtCursorAch2 = Util.INT(rtCursorAch2);
            rtCursorAch3 = Util.INT(rtCursorAch3);

            act(rtContent, rtBack, rtBar, rtCursor, rtCursorAch1, rtCursorAch2, rtCursorAch3);
        }
        #endregion
        #endregion
    }
}

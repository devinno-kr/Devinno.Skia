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
    public class DvSlider : Design.DvControl
    {
        #region Properties
        #region Color
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? BarColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? GraduationColor { get; set; } = null;
        public SKColor? CursorColor { get; set; } = null;
        #endregion
        #region Value
        public double Minimum { get; set; } = 0D;
        public double Maximum { get; set; } = 100D;
        #region public double Value { get; set; }
        private double nVal = 0D;
        public double Value
        {
            get => nVal;
            set
            {
                if (nVal != value)
                {
                    nVal = value;
                    ValueChanged?.Invoke(this, null);
                }
            }
        }
        #endregion
        public double? Tick { get; set; } = null;
        #endregion
        #region Direction
        public DvDirectionHV Direction { get; set; } = DvDirectionHV.Horizon;
        #endregion
        #region Reverse
        public bool Reverse { get; set; } = false;
        #endregion
        #region Text
        public string FormatString { get; set; } = "0";
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 10;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        #endregion
        #region CursorSize
        public float CursorSize { get; set; } = 30;
        #endregion
        #region BarSize
        public float BarSize { get; set; } = 5;
        #endregion
        #endregion

        #region Event
        public event EventHandler ValueChanged;
        public event EventHandler CursorDown;
        public event EventHandler CursorUp;
        #endregion

        #region Member Variable
        bool bCurDown = false;
        float mx = 0F;
        float my = 0F;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtEmpty, rtFill, rtCur) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var BoxColor = this.BoxColor ?? thm.ControlBackColor;
                    var BarColor = this.BarColor ?? thm.PointColor;
                    var CursorColor = this.CursorColor ?? thm.ButtonColor;
                    var GraduationColor = this.GraduationColor ?? thm.ForeColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();

                    var text = string.IsNullOrWhiteSpace(FormatString) ? Value.ToString() : Value.ToString(FormatString);
                    thm.DrawSlider(Canvas,
                        rtContent, rtEmpty, rtFill, rtCur,
                        BoxColor, BarColor, CursorColor, GraduationColor, ForeColor, BackColor,
                        Direction, Reverse,
                        Minimum, Maximum, Tick, BarSize, CursorSize,
                        text, FontName, FontSize, FontStyle,
                        bCurDown);
                }
            });

            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            mx = x;
            my = y;

            Areas((rtContent, rtEmpty, rtFill, rtCur) =>
            {
                if (CollisionTool.Check(rtCur, x, y))
                {
                    bCurDown = true;
                    CursorDown?.Invoke(this, null);
                    Design?.Input(this);
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtEmpty, rtFill, rtCur) =>
            {
                var vs = rtEmpty.Left;
                var ve = rtEmpty.Right;
                var v = x;
                if (Direction== DvDirectionHV.Horizon)
                {
                    vs = rtEmpty.Left;
                    ve = rtEmpty.Right;
                    v = x;
                }
                else if (Direction == DvDirectionHV.Vertical)
                {
                    vs = rtEmpty.Top;
                    ve = rtEmpty.Bottom;
                    v = y;
                }

                if (bCurDown)
                {
                    bCurDown = false;
                    if (Direction == DvDirectionHV.Horizon)
                    {
                        if (!Reverse)
                            Value = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Minimum, Maximum);
                        else
                            Value = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Maximum, Minimum);
                    }
                    else
                    {
                        if (Reverse)
                            Value = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Minimum, Maximum);
                        else
                            Value = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Maximum, Minimum);
                    }
                    if (Tick.HasValue) Value = Convert.ToSingle(Math.Round(Value / Tick.Value) * Tick);

                    CursorUp?.Invoke(this, null);
                }
                else if (Math.Abs(MathTool.GetDistance(new SKPoint(x, y), new SKPoint(mx, my))) < 10)
                {
                    if (Direction == DvDirectionHV.Horizon)
                    {
                        if (!Reverse)
                            Value = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Minimum, Maximum);
                        else
                            Value = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Maximum, Minimum);
                    }
                    else
                    {
                        if (Reverse)
                            Value = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Minimum, Maximum);
                        else
                            Value = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Maximum, Minimum);
                    }

                    if (Tick.HasValue) Value = Convert.ToSingle(Math.Round(Value / Tick.Value) * Tick);
                }
            });

            base.OnMouseUp(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(float x, float y)
        {
            Areas((rtContent, rtEmpty, rtFill, rtCur) =>
            {
                var vs = rtEmpty.Left;
                var ve = rtEmpty.Right;
                var v = x;

                if (Direction == DvDirectionHV.Horizon)
                {
                    vs = rtEmpty.Left;
                    ve = rtEmpty.Right;
                    v = x;
                }
                else if (Direction == DvDirectionHV.Vertical)
                {
                    vs = rtEmpty.Top;
                    ve = rtEmpty.Bottom;
                    v = y;
                }

                if (bCurDown)
                {
                    if (Direction == DvDirectionHV.Horizon)
                    {
                        if (!Reverse)
                            Value = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Minimum, Maximum);
                        else
                            Value = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Maximum, Minimum);
                    }
                    else
                    {
                        if (Reverse)
                            Value = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Minimum, Maximum);
                        else
                            Value = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Maximum, Minimum);
                    }
                    if (Tick.HasValue) Value = Convert.ToSingle(Math.Round(Value / Tick.Value) * Tick);
                }
            });
            base.OnMouseMove(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width, this.Height);

            if (Direction == DvDirectionHV.Horizon)
            {
                var rtEmpty = Util.MakeRectangleAlign(rtContent, new SKSize(rtContent.Width - CursorSize, BarSize), DvContentAlignment.BottomCenter);
                var wF = Convert.ToInt32(MathTool.Map(Value, Minimum, Maximum, 0, rtEmpty.Width));
                var rtFill = Util.FromRect(Reverse ? rtEmpty.Right - wF : rtEmpty.Left, rtEmpty.Top, wF, rtEmpty.Height);

                var l = !Reverse ? rtEmpty.Left : rtEmpty.Right;
                var r = !Reverse ? rtEmpty.Right : rtEmpty.Left;

                var cX = Convert.ToSingle(MathTool.Map(Value, Minimum, Maximum, l, r));
                var cY = (rtContent.Height - BarSize - 1F) / 2F;
                var rtCur = MathTool.MakeRectangle(new SKPoint(cX, cY), new SKSize(CursorSize, rtContent.Height - BarSize - 1));

                act(rtContent, rtEmpty, rtFill, rtCur);
            }
            else if (Direction == DvDirectionHV.Vertical)
            {
                var rtEmpty = Util.MakeRectangleAlign(rtContent, new SKSize(BarSize, rtContent.Height - CursorSize), DvContentAlignment.MiddleRight);
                var hF = Convert.ToInt32(MathTool.Map(Value, Minimum, Maximum, 0, rtEmpty.Height));
                var rtFill = Util.FromRect(rtEmpty.Left, Reverse ? rtEmpty.Top : rtEmpty.Bottom - hF, rtEmpty.Width, hF);

                var t = !Reverse ? rtEmpty.Bottom : rtEmpty.Top;
                var b = !Reverse ? rtEmpty.Top : rtEmpty.Bottom;

                var cX = (rtContent.Width - BarSize - 1F) / 2F;
                var cY = Convert.ToSingle(MathTool.Map(Value, Minimum, Maximum, t, b));
                var rtCur = MathTool.MakeRectangle(new SKPoint(cX, cY), new SKSize(rtContent.Width - BarSize - 1, CursorSize));

                act(rtContent, rtEmpty, rtFill, rtCur);
            }

        }
        #endregion
        #endregion
    }
}

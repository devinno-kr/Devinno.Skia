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
    public class DvRangeSlider : Design.DvControl
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
        #region public double RangeStart { get; set; }
        private double nRangeStart = 0D;
        public double RangeStart
        {
            get => nRangeStart;
            set
            {
                var val = MathTool.Constrain(value, Minimum, RangeEnd);
                if (nRangeStart != val)
                {
                    nRangeStart = val;
                    RangeStartChanged?.Invoke(this, null);
                }
            }
        }
        #endregion
        #region public double RangeEnd { get; set; }
        private double nRangeEnd = 0D;
        public double RangeEnd
        {
            get => nRangeEnd;
            set
            {
                var val = MathTool.Constrain(value, RangeStart, Maximum);
                if (nRangeEnd != val)
                {
                    nRangeEnd = val;
                    RangeEndChanged?.Invoke(this, null);
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
        public event EventHandler RangeStartChanged;
        public event EventHandler RangeStartCursorDown;
        public event EventHandler RangeStartCursorUp;

        public event EventHandler RangeEndChanged;
        public event EventHandler RangeEndCursorDown;
        public event EventHandler RangeEndCursorUp;
        #endregion

        #region Member Variable
        bool bRangeStartCurDown = false;
        bool bRangeEndCurDown = false;

        float mx = 0F;
        float my = 0F;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtEmpty, rtFill, rtCurStart, rtCurEnd) =>
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

                    var textS = string.IsNullOrWhiteSpace(FormatString) ? RangeStart.ToString() : RangeStart.ToString(FormatString);
                    var textE = string.IsNullOrWhiteSpace(FormatString) ? RangeEnd.ToString() : RangeEnd.ToString(FormatString);

                    thm.DrawRangeSlider(Canvas,
                        rtContent, rtEmpty, rtFill, rtCurStart, rtCurEnd,
                        BoxColor, BarColor, CursorColor, GraduationColor, ForeColor, BackColor,
                        Direction, Reverse,
                        Minimum, Maximum, Tick, BarSize, CursorSize,
                        textS, textE, FontName, FontSize, FontStyle,
                        bRangeStartCurDown, bRangeEndCurDown);
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

            Areas((rtContent, rtEmpty, rtFill, rtCurStart, rtCurEnd) =>
            {
                if (CollisionTool.Check(rtCurStart, x, y))
                {
                    bRangeStartCurDown = true;
                    RangeStartCursorDown?.Invoke(this, null);
                    Design?.Input(this);
                }
                else if (CollisionTool.Check(rtCurEnd, x, y))
                {
                    bRangeEndCurDown = true;
                    RangeEndCursorDown?.Invoke(this, null);
                    Design?.Input(this);
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtEmpty, rtFill, rtCurStart, rtCurEnd) =>
            {
                #region var
                var vs = rtEmpty.Left;
                var ve = rtEmpty.Right;
                var vrs = x + (CursorSize / 2F);
                var vre = x - (CursorSize / 2F);
                var v = x;

                if (Direction == DvDirectionHV.Horizon)
                {
                    vs = rtEmpty.Left;
                    ve = rtEmpty.Right;

                    if (Reverse)
                    {
                        vrs = x - (CursorSize / 2F);
                        vre = x + (CursorSize / 2F);
                    }
                    else
                    {
                        vrs = x + (CursorSize / 2F);
                        vre = x - (CursorSize / 2F);
                    }
                }
                else if (Direction == DvDirectionHV.Vertical)
                {
                    vs = rtEmpty.Top;
                    ve = rtEmpty.Bottom;

                    if (Reverse)
                    {
                        vrs = y + (CursorSize / 2F);
                        vre = y - (CursorSize / 2F);
                    }
                    else
                    {
                        vrs = y - (CursorSize / 2F);
                        vre = y + (CursorSize / 2F);
                    }
                }
                #endregion

                if (bRangeStartCurDown)
                {
                    v = vrs;

                    bRangeStartCurDown = false;

                    if (Reverse)
                    {
                        if (Direction == DvDirectionHV.Horizon)
                            RangeStart = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Maximum, Minimum);
                        else
                            RangeStart = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Minimum, Maximum);

                    }
                    else
                    {
                        if (Direction == DvDirectionHV.Horizon)
                            RangeStart = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Minimum, Maximum);
                        else
                            RangeStart = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Maximum, Minimum);
                    }

                    if (Tick.HasValue) RangeStart = Convert.ToSingle(Math.Round(RangeStart / Tick.Value) * Tick);

                    RangeStartCursorUp?.Invoke(this, null);
                }
                
                if (bRangeEndCurDown)
                {
                    v = vre;

                    bRangeEndCurDown = false;

                    if (Reverse)
                    {
                        if (Direction == DvDirectionHV.Horizon)
                            RangeEnd = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Maximum, Minimum);
                        else
                            RangeEnd = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Minimum, Maximum);
                    }
                    else
                    {
                        if (Direction == DvDirectionHV.Horizon)
                            RangeEnd = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Minimum, Maximum);
                        else
                            RangeEnd = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Maximum, Minimum);
                    }

                    if (Tick.HasValue) RangeEnd = Convert.ToSingle(Math.Round(RangeEnd / Tick.Value) * Tick);

                    RangeEndCursorUp?.Invoke(this, null);
                }
               
            });

            base.OnMouseUp(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(float x, float y)
        {
            Areas((rtContent, rtEmpty, rtFill, rtCurStart, rtCurEnd) =>
            {
                #region var
                var vs = rtEmpty.Left;
                var ve = rtEmpty.Right;
                var vrs = x + (CursorSize / 2F);
                var vre = x - (CursorSize / 2F);
                var v = x;

                if (Direction == DvDirectionHV.Horizon)
                {
                    vs = rtEmpty.Left;
                    ve = rtEmpty.Right;

                    if (Reverse)
                    {
                        vrs = x - (CursorSize / 2F);
                        vre = x + (CursorSize / 2F);
                    }
                    else
                    {
                        vrs = x + (CursorSize / 2F);
                        vre = x - (CursorSize / 2F);
                    }
                }
                else if (Direction == DvDirectionHV.Vertical)
                {
                    vs = rtEmpty.Top;
                    ve = rtEmpty.Bottom;
                    
                    if (Reverse)
                    {
                        vrs = y + (CursorSize / 2F);
                        vre = y - (CursorSize / 2F);
                    }
                    else
                    {
                        vrs = y - (CursorSize / 2F);
                        vre = y + (CursorSize / 2F);
                    }
                }
                #endregion

                if (bRangeStartCurDown)
                {
                    v = vrs;

                    if (Reverse)
                    {
                        if (Direction == DvDirectionHV.Horizon)
                            RangeStart = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Maximum, Minimum);
                        else
                            RangeStart = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Minimum, Maximum);

                    }
                    else
                    { 
                        if (Direction == DvDirectionHV.Horizon)
                            RangeStart = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Minimum, Maximum);
                        else
                            RangeStart = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Maximum, Minimum);
                    }

                    if (Tick.HasValue) RangeStart = Convert.ToSingle(Math.Round(RangeStart / Tick.Value) * Tick);
                }

                if (bRangeEndCurDown)
                {
                    v = vre;

                    if(Reverse)
                    {
                        if (Direction == DvDirectionHV.Horizon)
                            RangeEnd = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Maximum, Minimum);
                        else
                            RangeEnd = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Minimum, Maximum);
                    }
                    else
                    {
                        if (Direction == DvDirectionHV.Horizon)
                            RangeEnd = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Minimum, Maximum);
                        else
                            RangeEnd = MathTool.Map(MathTool.Constrain(v, vs, ve), vs, ve, Maximum, Minimum);
                    }

                    if (Tick.HasValue) RangeEnd = Convert.ToSingle(Math.Round(RangeEnd / Tick.Value) * Tick);
                }

            });
            base.OnMouseMove(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width, this.Height);

            if (Direction == DvDirectionHV.Horizon)
            {
                var rtc = Util.INT(MathTool.MakeRectangle(rtContent, new SKSize(rtContent.Width, CursorSize + 7 + BarSize)));
                var rtEmpty = Util.MakeRectangleAlign(rtc, new SKSize(rtc.Width - (CursorSize * 2), BarSize), DvContentAlignment.BottomCenter);
                var rtFill = Util.FromRect(rtEmpty);
                var rtCurStart = Util.FromRect(0, 0, CursorSize, rtc.Height - BarSize - 1);
                var rtCurEnd = Util.FromRect(0, 0, CursorSize, rtc.Height - BarSize - 1);

                if (Reverse)
                {
                    rtFill.Right = Convert.ToInt32(MathTool.Map(RangeStart, Minimum, Maximum, rtEmpty.Right, rtEmpty.Left));
                    rtFill.Left = Convert.ToInt32(MathTool.Map(RangeEnd, Minimum, Maximum, rtEmpty.Right, rtEmpty.Left));

                    rtCurEnd.Right = rtFill.Left;
                    rtCurEnd.Left = rtFill.Left - CursorSize;

                    rtCurStart.Left = rtFill.Right;
                    rtCurStart.Right = rtFill.Right + CursorSize;
                }
                else
                {
                    rtFill.Left = Convert.ToInt32(MathTool.Map(RangeStart, Minimum, Maximum, rtEmpty.Left, rtEmpty.Right));
                    rtFill.Right = Convert.ToInt32(MathTool.Map(RangeEnd, Minimum, Maximum, rtEmpty.Left, rtEmpty.Right));

                    rtCurStart.Right = rtFill.Left;
                    rtCurStart.Left = rtFill.Left - CursorSize;

                    rtCurEnd.Left = rtFill.Right;
                    rtCurEnd.Right = rtFill.Right + CursorSize;
                }

                act(rtContent, rtEmpty, rtFill, rtCurStart, rtCurEnd);
            }
            else if (Direction == DvDirectionHV.Vertical)
            {
                var rtc = Util.INT(MathTool.MakeRectangle(rtContent, new SKSize(CursorSize + 7 + BarSize, rtContent.Height)));
                var rtEmpty = Util.MakeRectangleAlign(rtc, new SKSize(BarSize, rtc.Height - (CursorSize * 2)), DvContentAlignment.MiddleRight);
                var rtFill = Util.FromRect(rtEmpty);
                var rtCurStart = Util.FromRect(0, 0, rtc.Width - BarSize - 1, CursorSize);
                var rtCurEnd = Util.FromRect(0, 0, rtc.Width - BarSize - 1, CursorSize);
              
                if (Reverse)
                {
                    rtFill.Bottom = Convert.ToInt32(MathTool.Map(RangeEnd, Minimum, Maximum, rtEmpty.Top, rtEmpty.Bottom));
                    rtFill.Top = Convert.ToInt32(MathTool.Map(RangeStart, Minimum, Maximum, rtEmpty.Top, rtEmpty.Bottom));

                    rtCurEnd.Top = rtFill.Bottom;
                    rtCurEnd.Bottom = rtFill.Bottom + CursorSize;

                    rtCurStart.Top = rtFill.Top - CursorSize;
                    rtCurStart.Bottom = rtFill.Top;
                }
                else
                {
                    rtFill.Top = Convert.ToInt32(MathTool.Map(RangeEnd, Minimum, Maximum, rtEmpty.Bottom, rtEmpty.Top));
                    rtFill.Bottom = Convert.ToInt32(MathTool.Map(RangeStart, Minimum, Maximum, rtEmpty.Bottom, rtEmpty.Top));

                    rtCurStart.Top = rtFill.Bottom;
                    rtCurStart.Bottom = rtFill.Bottom + CursorSize;

                    rtCurEnd.Top = rtFill.Top - CursorSize;
                    rtCurEnd.Bottom = rtFill.Top;
                }

                act(rtContent, rtEmpty, rtFill, rtCurStart, rtCurEnd);
            }

        }
        #endregion
        #endregion
    }
}

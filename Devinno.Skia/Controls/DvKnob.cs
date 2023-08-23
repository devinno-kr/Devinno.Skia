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
    public class DvKnob : Design.DvControl
    {
        #region Properties
        #region Color
        public SKColor? KnobColor { get; set; } = null;
        public SKColor? KnobBackColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? FillColor { get; set; } = null;
        public SKColor? OnLampColor { get; set; } = null;
        public SKColor? OffLampColor { get; set; } = null;
        #endregion
        #region Value
        public double Minimum { get; set; } = 0D;
        public double Maximum { get; set; } = 100D;
        public double? Tick { get; set; } = null;
        #region Value
        private double nValue = 0D;
        public double Value
        {
            get => nValue;
            set
            {
                var v = MathTool.Constrain(value, Minimum, Maximum);
                if (nValue != v)
                {
                    nValue = v;
                    ValueChanged?.Invoke(this, null);
                }
            }
        }
        #endregion
        #endregion
        #region Angle
        public float StartAngle { get; set; } = 135F;
        public float SweepAngle { get; set; } = 270;
        #endregion
        #region Gap
        public float Gap { get; set; } = 5;
        #endregion
        #region KnobPadding
        public float KnobPadding { get; set; } = 7;
        #endregion
        #region CusorDownState
        public bool CursorDownState { get; private set; }
        #endregion
        #endregion

        #region Member Variable
        double DownValue;
        double calcAngle;
        double downAngle;
        SKPoint prev;
        #endregion

        #region Event
        public event EventHandler ValueChanged;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtKnobBack, rtKnob, rtCursor) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var KnobColor = this.KnobColor ?? thm.ButtonColor;
                    var KnobBackColor = this.KnobBackColor ?? thm.ControlBackColor;
                    var FillColor = this.FillColor ?? thm.PointColor;
                    var OnLampColor = this.OnLampColor ?? SKColors.Red;
                    var OffLampColor = this.OffLampColor ?? thm.ButtonColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
          
                    thm.DrawKnob(Canvas,
                        rtContent, rtKnobBack, rtKnob, rtCursor,
                        KnobColor, KnobBackColor, FillColor, OnLampColor, OffLampColor, ForeColor, BackColor,
                        Minimum, Maximum, Value, Tick,
                        StartAngle, SweepAngle, CursorDownState, KnobPadding
                        );
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtKnobBack, rtKnob, rtCursor) =>
            {
                var ptLoc = new SKPoint(x, y);
                var rtc = Util.FromRect(rtCursor);//rtc.Inflate(20, 20);
                if (CollisionTool.CheckCircle(rtKnob, ptLoc))// && CollisionTool.Check(rtc, ptLoc))
                {
                    CursorDownState = true;
                    calcAngle = 0;
                    downAngle = MathTool.Map(Value, Minimum, Maximum, StartAngle, StartAngle + SweepAngle);
                    prev = ptLoc;
                    DownValue = Value;

                    Design?.Input(this);
                }

            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(float x, float y)
        {
            Areas((rtContent, rtKnobBack, rtKnob, rtCursor) =>
            {
                var cp = new SKPoint(rtContent.MidX, rtContent.MidY);
                var ptLoc = new SKPoint(x, y);

                if (CursorDownState)
                {
                    #region Value
                    var pv = MathTool.GetAngle(cp, prev);
                    var nv = MathTool.GetAngle(cp, ptLoc);

                    var v = nv - pv;
                    if (v < -300) v = 360 + v;
                    else if (v > 300) v = v - 360;
                    calcAngle += v;

                    var va = downAngle + calcAngle;
                    if (va > StartAngle + SweepAngle + 360) calcAngle -= 360;
                    else if (va < StartAngle - 360) calcAngle += 360;

                    var cv = MathTool.Map(calcAngle, 0D, SweepAngle, Minimum, Maximum);
                    Value = MathTool.Constrain(DownValue + cv, Minimum, Maximum);
                    #endregion

                    if (Tick.HasValue) Value = Convert.ToSingle(Math.Round(Value / Tick.Value) * Tick);

                    prev = ptLoc;
                }
            });
            base.OnMouseMove(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtKnobBack, rtKnob, rtCursor) =>
            {
                var cp = new SKPoint(rtContent.MidX, rtContent.MidY);
                var ptLoc = new SKPoint(x, y);

                if (SweepAngle > 360)
                {
                    var vang = Convert.ToSingle(MathTool.Map(MathTool.Constrain(Value, Minimum, Maximum), Minimum, Maximum, 0, SweepAngle));
                    var maxpage = Math.Floor(SweepAngle / 360D);
                    var nowpage = Math.Floor((vang + calcAngle) / 360D);
                }

                if (CursorDownState)
                {
                    #region Value
                    var pv = MathTool.GetAngle(cp, prev);
                    var nv = MathTool.GetAngle(cp, ptLoc);

                    var v = nv - pv;
                    if (v < -300) v = 360 + v;
                    else if (v > 300) v = v - 360;
                    calcAngle += v;

                    var cv = MathTool.Map(calcAngle, 0D, SweepAngle, Minimum, Maximum);
                    Value = MathTool.Constrain(DownValue + cv, Minimum, Maximum);
                    #endregion

                    if (Tick.HasValue) Value = Convert.ToSingle(Math.Round(Value / Tick.Value) * Tick);

                    CursorDownState = false;
                }
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect> act)
        {
            var wh = Math.Min(Width, Height);
            var rtContent = Util.FromRect(0, 0, Width, Height);
            var rtKnobBack = Util.MakeRectangle(rtContent, new SKSize(wh, wh));
            var rtKnob = Util.FromRect(rtKnobBack); rtKnob.Inflate(-KnobPadding, -KnobPadding);

            var cp = new SKPoint(rtContent.MidX, rtContent.MidY);
            var vang = Convert.ToSingle(MathTool.Map(MathTool.Constrain(Value, Minimum, Maximum), Minimum, Maximum, 0, SweepAngle)) + StartAngle;
            var vwh = rtKnob.Width / 2;
            var pt = MathTool.GetPointWithAngle(cp, vang, vwh - 20);
            var rtCursor = MathTool.MakeRectangle(pt, 24);

            act(rtContent, rtKnobBack, rtKnob, rtCursor);
        }
        #endregion
        #endregion
    }
}

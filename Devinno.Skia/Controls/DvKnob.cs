using Devinno.Skia.Design;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvKnob : DvControl
    {
        #region Properties
        public SKColor? KnobColor { get; set; } = null;
        public SKColor? FillColor { get; set; } = null;
        public SKColor? EmptyColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? CursorColor { get; set; } = null;
        public SKColor CursorDownColor { get; set; } = SKColors.Red;

        public double Minimum { get; set; } = 0D;
        public double Maximum { get; set; } = 100D;
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

        public int StartAngle { get; set; } = 135;
        public int SweepAngle { get; set; } = 270;

        public bool ValueDraw { get; set; } = true;
        public bool Gradient { get; set; } = false;

        public string FormatString { get; set; } = "0";
        public string Unit { get; set; } = null;
        public float UnitDistance { get; set; } = 0.7F;
        public float TextDistance { get; set; } = 0.5F;

        public bool CursorDownState { get; private set; }

        #region Text
        public string RemarkFontName { get; set; } = "NanumGothic";
        public int RemarkFontSize { get; set; } = 10;

        public string ValueFontName { get; set; } = "NanumGothic";
        public int ValueFontSize { get; set; } = 18;
        #endregion
        #endregion

        #region Event
        public event EventHandler ValueChanged;
        #endregion

        #region Member Variable
        SKBitmap bmMask;
        int maskW, maskH;
        
        double DownValue;
        double calcAngle;
        double downAngle;
        SKPoint prev;
        #endregion

        #region Constructor
        public DvKnob()
        {
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            try
            {
                bounds((rtContent, rtCircleOut, rtCircleIn, rtGauge, rtKnob, rtText, rtUnit) =>
                {
                    if (maskW != Width || maskH != Height || bmMask == null)
                    {
                        if (bmMask != null) bmMask.Dispose();
                        bmMask = ResourceTool.volumemask.Resize(new SKImageInfo(Convert.ToInt32(rtKnob.Width), Convert.ToInt32(rtKnob.Height)), SKFilterQuality.High);
                        maskW = Width;
                        maskH = Height;
                    }

                    var ds = Design;
                    var thm = Design?.Theme;
                    if (ds != null && thm != null)
                    {
                        var KnobColor = this.KnobColor ?? thm.KnobColor;
                        var FillColor = this.FillColor ?? thm.PointColor;
                        var EmptyColor = this.EmptyColor ?? thm.ConcaveBoxColor;
                        var ForeColor = this.ForeColor ?? thm.ForeColor;
                        var CursorColor = this.EmptyColor ?? thm.KnobCursorColor;
                        var CursorDownColor = this.CursorDownColor;
                        var BorderColor = thm.GetBorderColor(EmptyColor, ParentContainer.GetBackColor());
                        var Corner = thm.Corner;
                        
                        using (var p = new SKPaint() {  IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                        {
                            var cp = new SKPoint(rtContent.MidX, rtContent.MidY);
                            var sp1 = MathTool.GetPointWithAngle(cp, StartAngle, rtCircleOut.Width / 2F);
                            var sp2 = MathTool.GetPointWithAngle(cp, StartAngle + SweepAngle, rtCircleIn.Width / 2F);

                            #region Bar
                            {
                                var mang = (float)MathTool.Map(Maximum, Minimum, Maximum, 0, Math.Min(SweepAngle, 360));
                                var vang = (float)MathTool.Map(Value, Minimum, Maximum, 0, Math.Min(SweepAngle, 360));
                                var ng = Convert.ToInt32(rtCircleOut.Width * 0.05F);

                                p.StrokeWidth = ng;
                                p.IsStroke = true;
                                p.Color = EmptyColor;
                                Canvas.DrawArc(rtGauge, StartAngle, mang, false, p);
                                p.Color = FillColor;
                                Canvas.DrawArc(rtGauge, StartAngle, vang, false, p);
                            }
                            #endregion
                            #region Knob
                            {
                                p.IsStroke = false;
                                p.Color = KnobColor;

                                Canvas.DrawCircle(cp, rtKnob.Width / 2F, p);
                                if (bmMask != null) Canvas.DrawBitmap(bmMask, rtKnob);

                                p.IsStroke = true;
                                p.StrokeWidth = 1;
                                p.Color = BorderColor;
                                rtKnob.Inflate(p.StrokeWidth / 2F, p.StrokeWidth / 2F);
                                Canvas.DrawCircle(cp, rtKnob.Width / 2F, p);
                            }
                            #endregion
                            #region Text
                            if (ValueDraw)
                            {
                                var s = FormatString != null ? Value.ToString(FormatString) : Value.ToString();
                                Util.DrawText(Canvas, s, ValueFontName, ValueFontSize, DvFontStyle.Normal, ForeColor, rtText, DvContentAlignment.MiddleCenter, true);

                                if (!string.IsNullOrWhiteSpace(Unit))
                                {
                                    var s2 = Unit;
                                    Util.DrawText(Canvas, s2, RemarkFontName, RemarkFontSize, DvFontStyle.Normal, ForeColor, rtUnit, DvContentAlignment.MiddleCenter, true);
                                }
                            }
                            #endregion
                            #region Cursor
                            using (var fOut = SKImageFilter.CreateDropShadow(1, 1, 1, 1, Util.FromArgb(thm.OutShadowAlpha, SKColors.Black)))
                            {
                                var vang = Convert.ToSingle(MathTool.Map(MathTool.Constrain(Value, Minimum, Maximum), Minimum, Maximum, 0, SweepAngle)) + StartAngle;
                                var wh = rtKnob.Width / 2;
                                var pt1 = MathTool.GetPointWithAngle(cp, vang, wh - (wh / 6));
                                var pt2 = MathTool.GetPointWithAngle(cp, vang, wh - Convert.ToInt32(wh / 2.5));

                                p.ImageFilter = fOut;
                                using (var pth = new SKPath())
                                {
                                    var sz = Math.Max(1, rtKnob.Width / 32);
                                    var rt1 = MathTool.MakeRectangle(pt1, sz);
                                    var rt2 = MathTool.MakeRectangle(pt2, sz);
                                    var c = CursorDownState ? CursorDownColor : CursorColor;
                                    pth.AddArc(rt1, vang - 90, 180);
                                    pth.ArcTo(rt2, vang + 90, 180, false);
                                    pth.Close();

                                    p.IsStroke = false;
                                    p.Color = c;
                                    Canvas.DrawPath(pth, p);

                                    p.IsStroke = true;
                                    p.Color = thm.GetBorderColor(KnobColor, BorderColor);
                                    Canvas.DrawPath(pth, p);
                                }
                                p.ImageFilter = null;
                            }
                            #endregion
                        }
                    }
                });
            }
            catch { }

            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            bounds((rtContent, rtCircleOut, rtCircleIn, rtGauge, rtKnob, rtText, rtUnit) =>
            {
                var cp = new SKPoint(rtContent.MidX, rtContent.MidY);
                var vang = Convert.ToSingle(MathTool.Map(MathTool.Constrain(Value, Minimum, Maximum), Minimum, Maximum, 0, SweepAngle)) + StartAngle;
             
                var wh = rtKnob.Width / 2;
                var pt1 = MathTool.GetPointWithAngle(cp, vang, wh - (wh / 6));
                var pt2 = MathTool.GetPointWithAngle(cp, vang, wh - Convert.ToInt32(wh / 2.5));
                var ptc = MathTool.CenterPoint(pt1, pt2);
                var rtCur = MathTool.MakeRectangle(ptc, Convert.ToSingle(MathTool.GetDistance(pt1, pt2) * 2));
                var v = Convert.ToSingle(Math.Abs(wh / 6 - wh / 2.5));
                var ptLoc = new SKPoint(x, y);
                if (CollisionTool.CheckCircle(rtKnob, ptLoc) && CollisionTool.Check(rtCur, ptLoc))
                {
                    CursorDownState = true;
                    calcAngle = 0;
                    downAngle = MathTool.Map(Value, Minimum, Maximum, StartAngle, StartAngle + SweepAngle);
                    prev = ptLoc;
                    DownValue = Value;
                }

            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(int x, int y)
        {
            bounds((rtContent, rtCircleOut, rtCircleIn, rtGauge, rtKnob, rtText, rtUnit) =>
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
                    prev = ptLoc;
                }
            });
            base.OnMouseMove(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            bounds((rtContent, rtCircleOut, rtCircleIn, rtGauge, rtKnob, rtText, rtUnit) =>
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
                    CursorDownState = false;
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

            var whOut = Convert.ToInt32(Math.Min(rtContent.Width, rtContent.Height));
            var whIn = Convert.ToInt32(whOut * 0.7);
            var whGauge = Convert.ToInt32(whOut * 0.95);
            var whKnob = Convert.ToInt32(whOut * 0.9);
            var whUnit = Convert.ToInt32(whOut * UnitDistance);
            var whText = Convert.ToInt32(whOut * TextDistance);

            var rtCircleOut = Util.MakeRectangle(rtContent, new SKSize(whOut, whOut));
            var rtCircleIn = Util.MakeRectangle(rtContent, new SKSize(whIn, whIn));
            var rtGauge = Util.MakeRectangle(rtContent, new SKSize(whGauge, whGauge));
            var rtKnob = Util.MakeRectangle(rtContent, new SKSize(whKnob, whKnob));

            var cp = new SKPoint(rtContent.MidX, rtContent.MidY);
            var sp1 = MathTool.GetPointWithAngle(cp, 90, whUnit / 2F);
            var rtUnit = MathTool.MakeRectangle(sp1, 200, 100);

            var sp2 = MathTool.GetPointWithAngle(cp, 90, whText / 2F);
            var rtText = MathTool.MakeRectangle(sp2, 200, 100);

            act(rtContent, rtCircleOut, rtCircleIn, rtGauge, rtKnob, rtText, rtUnit);
        }
        #endregion
        #endregion
    }
}

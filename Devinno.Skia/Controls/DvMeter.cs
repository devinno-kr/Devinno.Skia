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
    public class DvMeter : DvControl
    {
        #region Properties
        public SKColor? NeedleColor { get; set; } = null;
        public SKColor? NeedlePointColor { get; set; } = null;
        public SKColor? FillColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;

        public double Minimum { get; set; } = 0D;
        public double Maximum { get; set; } = 100D;
        public double Value { get; set; } = 0D;
        public double GraduationLarge { get; set; } = 10;
        public double GraduationSmall { get; set; } = 2;

        public int StartAngle { get; set; } = 135;
        public int SweepAngle { get; set; } = 270;

        public bool ValueDraw { get; set; } = true;

        public string FormatString { get; set; } = "0";
        public string Unit { get; set; } = null;
        public float UnitDistance { get; set; } = 0.7F;
        public float TextDistance { get; set; } = 0.5F;

        public List<MeterBar> Bars { get; private set; } = new List<MeterBar>(); 

        #region Text
        public string RemarkFontName { get; set; } = "NanumGothic";
        public int RemarkFontSize { get; set; } = 10;

        public string ValueFontName { get; set; } = "NanumGothic";
        public int ValueFontSize { get; set; } = 18;
        #endregion
        #endregion

        #region Constructor
        public DvMeter()
        {
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            try
            {
                bounds((rtContent, rtCircleOut, rtCircleIn, rtCircleRmkS, rtCircleRmkL, rtCircleRmkT, rtCircleND, rtText, rtUnit, rtBar) =>
                {
                    var ds = Design;
                    var thm = Design?.Theme;
                    if (ds != null && thm != null)
                    {
                        var cBack = ParentContainer.GetBackColor();
                        var NeedleColor = this.NeedleColor ?? thm.NeedleColor;
                        var NeedlePointColor = this.NeedlePointColor ?? thm.NeedlePointColor;
                        var FillColor = this.FillColor ?? thm.PointColor;
                        var ForeColor = this.ForeColor ?? thm.ForeColor;
                        var BorderColor = thm.GetBorderColor(cBack, NeedleColor);
                        var Corner = thm.Corner;
                        
                        using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                        {
                            var cp = new SKPoint(rtContent.MidX, rtContent.MidY);

                            #region Remark
                            p.IsStroke = true;
                            p.Color = ForeColor;
                            p.StrokeWidth = 2;

                            Canvas.DrawArc(rtCircleIn, StartAngle, SweepAngle, false, p);

                            for (double i = Minimum; i <= Maximum; i += GraduationLarge)
                            {
                                var gsang = Convert.ToSingle(MathTool.Map(MathTool.Constrain(i, Minimum, Maximum), Minimum, Maximum, 0D, SweepAngle)) + StartAngle;

                                var p1 = MathTool.GetPointWithAngle(cp, gsang, rtCircleIn.Width / 2F);
                                var p2 = MathTool.GetPointWithAngle(cp, gsang, rtCircleRmkL.Width / 2F);
                                var pT = MathTool.GetPointWithAngle(cp, gsang, rtCircleRmkT.Width / 2F);
                                Canvas.DrawLine(new SKPoint(p1.X, p1.Y), new SKPoint(p2.X, p2.Y), p);

                                Util.DrawText(Canvas, i.ToString(), RemarkFontName, RemarkFontSize, DvFontStyle.Normal, ForeColor, new SKRect(pT.X - 30, pT.Y - 30, pT.X + 30, pT.Y + 30));
                            }

                            p.StrokeWidth = 1;
                            for (double i = Minimum; i <= Maximum; i += GraduationSmall)
                            {
                                var gsang = Convert.ToSingle(MathTool.Map(MathTool.Constrain(i, Minimum, Maximum), Minimum, Maximum, 0D, SweepAngle)) + StartAngle;

                                var p1 = MathTool.GetPointWithAngle(cp, gsang, rtCircleIn.Width / 2F);
                                var p2 = MathTool.GetPointWithAngle(cp, gsang, rtCircleRmkS.Width / 2F);

                                Canvas.DrawLine(new SKPoint(p1.X, p1.Y), new SKPoint(p2.X, p2.Y), p);
                            }
                            #endregion
                            #region Bar
                            if(Bars != null && Bars.Count>0)
                            {
                                foreach (var bar in Bars)
                                {
                                    var vangMin = Convert.ToSingle(MathTool.Map(MathTool.Constrain(bar.Minimum, Minimum, Maximum), Minimum, Maximum, 0, SweepAngle));
                                    var vangMax = Convert.ToSingle(MathTool.Map(MathTool.Constrain(bar.Maximum, Minimum, Maximum), Minimum, Maximum, 0, SweepAngle));

                                    p.IsStroke = true;
                                    p.Color = bar.Color;
                                    p.StrokeWidth = 7;

                                    Canvas.DrawArc(rtBar, vangMin + StartAngle, vangMax - vangMin, false, p);
                                }
                            }
                            #endregion
                            #region Needle
                            using (var path = new SKPath())
                            {
                                using (var filter = SKImageFilter.CreateDropShadow(1, 1, 1, 1, Util.FromArgb(thm.OutShadowAlpha, SKColors.Black)))
                                {
                                    p.ImageFilter = filter;

                                    var vang = Convert.ToSingle(MathTool.Map(MathTool.Constrain(Value, Minimum, Maximum), Minimum, Maximum, 0, SweepAngle));
                                    var pt = MathTool.GetPointWithAngle(cp, vang + StartAngle, rtCircleND.Width / 2F);

                                    var rtS = MathTool.MakeRectangle(pt, 3);
                                    var rtL = MathTool.MakeRectangle(cp, rtCircleOut.Width / 12);

                                    path.AddArc(rtL, vang + StartAngle + 90, 180);
                                    path.ArcTo(rtS, vang + StartAngle + 90 + 180, 180, false);
                                    path.Close();

                                    p.IsStroke = false;
                                    using (var lg = SKShader.CreateRadialGradient(cp, rtCircleOut.Width / 2F, new SKColor[] { NeedleColor, NeedleColor, NeedlePointColor, NeedlePointColor }, new float[] { 0, 0.6F, 0.6F, 1 }, SKShaderTileMode.Clamp))
                                    {
                                        p.Shader = lg;

                                        Canvas.DrawPath(path, p);
                                        p.ImageFilter = null;

                                        p.Shader = null;
                                    }

                                    p.Color = BorderColor;
                                    Canvas.DrawCircle(cp, 3, p);

                                    p.IsStroke = true;
                                    p.StrokeWidth = 1;
                                    p.Color = BorderColor;
                                    Canvas.DrawPath(path, p);
                                }
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
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);

            var whOut = Convert.ToInt32(Math.Min(rtContent.Width, rtContent.Height));
            var whIn = Convert.ToInt32(whOut * 0.8);
            var whRmkS = Convert.ToInt32(whOut * (0.8 + 0.035 * 1D));
            var whRmkL = Convert.ToInt32(whOut * (0.8 + 0.035 * 2D));
            var whRmkT = Convert.ToInt32(whOut * (0.8 + 0.035 * 4.5D));
            var whBar = Convert.ToInt32(whOut * 0.70);
            var whNeedle = Convert.ToInt32(whOut * 0.85);
            var whUnit = Convert.ToInt32(whOut * UnitDistance);
            var whText = Convert.ToInt32(whOut * TextDistance);

            var rtCircleOut = Util.MakeRectangle(rtContent, new SKSize(whOut, whOut));
            var rtCircleIn = Util.MakeRectangle(rtContent, new SKSize(whIn, whIn));
            var rtCircleRmkL = Util.MakeRectangle(rtContent, new SKSize(whRmkL, whRmkL));
            var rtCircleRmkS = Util.MakeRectangle(rtContent, new SKSize(whRmkS, whRmkS));
            var rtCircleRmkT = Util.MakeRectangle(rtContent, new SKSize(whRmkT, whRmkT));
            var rtCircleND = Util.MakeRectangle(rtContent, new SKSize(whRmkT, whRmkT));
            var rtBar = Util.MakeRectangle(rtContent, new SKSize(whBar, whBar));

            var cp = new SKPoint(rtContent.MidX, rtContent.MidY);
            var sp1 = MathTool.GetPointWithAngle(cp, 90, whUnit / 2F);
            var rtUnit = MathTool.MakeRectangle(sp1, 200, 100);

            var sp2 = MathTool.GetPointWithAngle(cp, 90, whText / 2F);
            var rtText = MathTool.MakeRectangle(sp2, 200, 100);

            act(rtContent, rtCircleOut, rtCircleIn, rtCircleRmkS, rtCircleRmkL, rtCircleRmkT, rtCircleND, rtText, rtUnit, rtBar);
        }
        #endregion
        #endregion
    }

    #region DvMeterBar
    public class MeterBar
    {
        public double Minimum { get; private set; }
        public double Maximum { get; private set; }
        public SKColor Color { get; private set; }

        public MeterBar(double Minimum, double Maximum, SKColor Color)
        {
            this.Minimum = Minimum;
            this.Maximum = Maximum;
            this.Color = Color;
        }
    }
    #endregion
}

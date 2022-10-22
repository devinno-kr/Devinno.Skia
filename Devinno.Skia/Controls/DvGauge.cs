using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
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
    public class DvGauge : DvControl
    {
        #region Properties
        public SKColor? FillColor { get; set; } = null;
        public SKColor? EmptyColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;

        public double Minimum { get; set; } = 0D;
        public double Maximum { get; set; } = 100D;
        public double Value { get; set; } = 0D;
        public double GraduationLarge { get; set; } = 10;
        public double GraduationSmall { get; set; } = 2;

        public int StartAngle { get; set; } = 135;
        public int SweepAngle { get; set; } = 270;

        public bool ValueDraw { get; set; } = true;
        public bool Gradient { get; set; } = false;

        public string FormatString { get; set; } = "0";
        public string Unit { get; set; } = null;
        public float UnitDistance { get; set; } = 0.7F;
        public float TextDistance { get; set; } = 0.5F;

        #region Text
        public string RemarkFontName { get; set; } = "NanumGothic";
        public int RemarkFontSize { get; set; } = 10;

        public string ValueFontName { get; set; } = "NanumGothic";
        public int ValueFontSize { get; set; } = 18;
        #endregion
        #endregion

        #region Member Variable
        SKBitmap bmMask;
        int maskW, maskH;
        #endregion

        #region Constructor
        public DvGauge()
        {
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            try
            {
                bounds((rtContent, rtCircleOut, rtCircleIn, rtCircleRmkS, rtCircleRmkL, rtCircleRmkT, rtText, rtUnit) =>
                {
                    if (maskW != Width || maskH != Height || bmMask == null)
                    {
                        if (bmMask != null) bmMask.Dispose();
                        bmMask = ResourceTool.circlegrad.Resize(new SKImageInfo(Convert.ToInt32(rtCircleOut.Width), Convert.ToInt32(rtCircleOut.Height)), SKFilterQuality.High);
                        maskW = Width;
                        maskH = Height;
                    }

                    var ds = Design;
                    var thm = Design?.Theme;
                    if (ds != null && thm != null)
                    {
                        var FillColor = this.FillColor ?? thm.PointColor;
                        var EmptyColor = this.EmptyColor ?? thm.ConcaveBoxColor;
                        var ForeColor = this.ForeColor ?? thm.ForeColor;
                        var EmptyBorderColor = thm.GetBorderColor(EmptyColor, ParentContainer.GetBackColor());
                        var FillBorderColor = thm.GetBorderColor(FillColor, ParentContainer.GetBackColor());
                        var Corner = thm.Corner;

                        using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                        {
                            var cp = new SKPoint(rtContent.MidX, rtContent.MidY);
                            var sp1 = MathTool.GetPointWithAngle(cp, StartAngle, rtCircleOut.Width / 2F);
                            var sp2 = MathTool.GetPointWithAngle(cp, StartAngle + SweepAngle, rtCircleIn.Width / 2F);

                            #region Remark
                            p.IsStroke = true;
                            p.Color = ForeColor;
                            p.StrokeWidth = 2;
                            if (GraduationLarge > 0)
                            {
                                for (double i = Minimum; i <= Maximum; i += GraduationLarge)
                                {
                                    var gsang = Convert.ToSingle(MathTool.Map(MathTool.Constrain(i, Minimum, Maximum), Minimum, Maximum, 0D, SweepAngle)) + StartAngle;

                                    var p1 = MathTool.GetPointWithAngle(cp, gsang, rtCircleIn.Width / 2F);
                                    var p2 = MathTool.GetPointWithAngle(cp, gsang, rtCircleRmkL.Width / 2F);
                                    var pT = MathTool.GetPointWithAngle(cp, gsang, rtCircleRmkT.Width / 2F);
                                    Canvas.DrawLine(new SKPoint(p1.X, p1.Y), new SKPoint(p2.X, p2.Y), p);

                                    Util.DrawText(Canvas, i.ToString(), RemarkFontName, RemarkFontSize, DvFontStyle.Normal, ForeColor, new SKRect(pT.X - 30, pT.Y - 30, pT.X + 30, pT.Y + 30));
                                }
                            }
                            p.StrokeWidth = 1;
                            if (GraduationSmall > 0)
                            {
                                for (double i = Minimum; i <= Maximum; i += GraduationSmall)
                                {
                                    var gsang = Convert.ToSingle(MathTool.Map(MathTool.Constrain(i, Minimum, Maximum), Minimum, Maximum, 0D, SweepAngle)) + StartAngle;

                                    var p1 = MathTool.GetPointWithAngle(cp, gsang, rtCircleIn.Width / 2F);
                                    var p2 = MathTool.GetPointWithAngle(cp, gsang, rtCircleRmkS.Width / 2F);

                                    Canvas.DrawLine(new SKPoint(p1.X, p1.Y), new SKPoint(p2.X, p2.Y), p);
                                }
                            }
                            #endregion
                            #region Empty
                            using (var path = new SKPath())
                            {
                                path.AddArc(rtCircleOut, StartAngle, SweepAngle);
                                path.ArcTo(rtCircleIn, StartAngle + SweepAngle, -SweepAngle, false);
                                path.Close();

                                using (var filterOut = SKImageFilter.CreateDropShadow(0, 2, 0, 0, Util.FromArgb(thm.OutBevelAlpha, SKColors.White)))
                                {
                                    p.ImageFilter = filterOut;

                                    p.IsStroke = false;
                                    p.Color = EmptyColor;
                                    Canvas.DrawPath(path, p);

                                    p.ImageFilter = null;
                                }

                                p.IsStroke = true;
                                p.Color = EmptyBorderColor;
                                Canvas.DrawPath(path, p);
                            }
                            #endregion
                            #region Fill
                            if (Value > Minimum)
                            {
                                using (var path = new SKPath())
                                {
                                    var Ang = Convert.ToSingle(MathTool.Map(Value, Minimum, Maximum, 0, SweepAngle));
                                    if (Ang > 0)
                                    {
                                        path.AddArc(rtCircleOut, StartAngle, Ang);
                                        path.ArcTo(rtCircleIn, StartAngle + Ang, -Ang, false);
                                        path.Close();

                                        #region Fill
                                        {
                                            p.IsStroke = false;
                                            p.Color = FillColor;
                                            Canvas.DrawPath(path, p);

                                            if (Gradient)
                                            {
                                                var sp = Canvas.Save();

                                                Canvas.ClipPath(path);
                                                if (bmMask != null) Canvas.DrawBitmap(bmMask, rtCircleOut);

                                                Canvas.RestoreToCount(sp);
                                            }
                                            else
                                            {
                                                #region Bevel
                                                var BevelLight = 0.6F;
                                                var BevelDark = -0.2F;

                                                var rt = Util.FromRect(rtCircleOut.Left, rtCircleOut.Top, rtCircleOut.Width, rtCircleOut.Height);
                                                rt.Inflate(-1, -1);

                                                var rt2 = Util.FromRect(rtCircleIn.Left, rtCircleIn.Top, rtCircleIn.Width, rtCircleIn.Height);
                                                rt2.Inflate(2, 2);

                                                var vsp = MathTool.GetPointWithAngle(cp, 225, rtCircleOut.Width / 2F);
                                                var vep = MathTool.GetPointWithAngle(cp, 45, rtCircleOut.Width / 2F);
                                                var csp = FillColor.BrightnessTransmit(BevelLight);
                                                var cep = FillColor.BrightnessTransmit(BevelDark);
                                                using (var lg = SKShader.CreateLinearGradient(new SKPoint(vsp.X, vsp.Y), new SKPoint(vep.X, vep.Y), new SKColor[] { csp, cep }, new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                                                {
                                                    p.Shader = lg;
                                                    p.IsStroke = true;
                                                    p.StrokeWidth = 2;
                                                    Canvas.DrawArc(rt, StartAngle, Ang, false, p);

                                                    p.Shader = null;
                                                }

                                                var vsp2 = MathTool.GetPointWithAngle(cp, 225, rtCircleIn.Width / 2F);
                                                var vep2 = MathTool.GetPointWithAngle(cp, 45, rtCircleIn.Width / 2F);
                                                var csp2 = FillColor.BrightnessTransmit(BevelLight);
                                                var cep2 = FillColor.BrightnessTransmit(BevelDark);
                                                using (var lg = SKShader.CreateLinearGradient(new SKPoint(vsp2.X, vsp2.Y), new SKPoint(vep2.X, vep2.Y), new SKColor[] { cep2, csp2 }, new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                                                {
                                                    p.Shader = lg;
                                                    p.IsStroke = true;
                                                    p.StrokeWidth = 2;
                                                    Canvas.DrawArc(rt2, StartAngle, Ang, false, p);

                                                    p.Shader = null;
                                                }
                                                #endregion
                                            }
                                        }
                                        #endregion

                                        #region Border
                                        p.IsStroke = true;
                                        p.StrokeWidth = 1;
                                        p.Color = FillBorderColor;
                                        Canvas.DrawPath(path, p);
                                        #endregion
                                    }
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
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);

            var whOut = Convert.ToInt32(Math.Min(rtContent.Width, rtContent.Height));
            var whIn = Convert.ToInt32(whOut * 0.7);
            var whRmkS = Convert.ToInt32(whOut * (0.7 - 0.035 * 1D));
            var whRmkL = Convert.ToInt32(whOut * (0.7 - 0.035 * 2D));
            var whRmkT = Convert.ToInt32(whOut * (0.7 - 0.035 * 4.5D));
            var whUnit = Convert.ToInt32(whOut * UnitDistance);
            var whText = Convert.ToInt32(whOut * TextDistance);

            var rtCircleOut = Util.MakeRectangle(rtContent, new SKSize(whOut, whOut));
            var rtCircleIn = Util.MakeRectangle(rtContent, new SKSize(whIn, whIn));
            var rtCircleRmkL = Util.MakeRectangle(rtContent, new SKSize(whRmkL, whRmkL));
            var rtCircleRmkS = Util.MakeRectangle(rtContent, new SKSize(whRmkS, whRmkS));
            var rtCircleRmkT = Util.MakeRectangle(rtContent, new SKSize(whRmkT, whRmkT));

            var cp = new SKPoint(rtContent.MidX, rtContent.MidY);
            var sp1 = MathTool.GetPointWithAngle(cp, 90, whUnit / 2F);
            var rtUnit = MathTool.MakeRectangle(sp1, 200, 100);

            var sp2 = MathTool.GetPointWithAngle(cp, 90, whText / 2F);
            var rtText = MathTool.MakeRectangle(sp2, 200, 100);

            rtCircleOut = Util.INT(rtCircleOut);
            rtCircleIn = Util.INT(rtCircleIn);
            rtCircleRmkS = Util.INT(rtCircleRmkS);
            rtCircleRmkL = Util.INT(rtCircleRmkL);
            rtCircleRmkT = Util.INT(rtCircleRmkT);
            rtText = Util.INT(rtText);
            rtUnit = Util.INT(rtUnit);

            act(rtContent, rtCircleOut, rtCircleIn, rtCircleRmkS, rtCircleRmkL, rtCircleRmkT, rtText, rtUnit);
        }
        #endregion
        #endregion
    }
}

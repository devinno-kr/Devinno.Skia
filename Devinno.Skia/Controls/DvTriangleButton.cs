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
    public class DvTriangleButton : DvControl
    {
        #region Properties
        public SKColor? ButtonColor { get; set; } = null;
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;

        public DvDirection Direction { get; set; } = DvDirection.Left;

        public int? Corner { get; set; } = null;

        public bool Gradient { get; set; } = true;
        #endregion

        #region Member Variable
        bool bDown = false;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtBox, rtButton, ng) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region Color
                    var BoxColor = this.BoxColor ?? thm.ConcaveBoxColor;
                    var ButtonColor = this.ButtonColor ?? thm.ButtonColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BorderColor = thm.GetBorderColor(BoxColor, ParentContainer.GetBackColor());
                    var ButtonBorderColor = thm.GetBorderColor(ButtonColor, BoxColor);
                    var Corner = this.Corner ?? thm.Corner;
                    #endregion

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, IsDither = DvDesign.DI, FilterQuality = DvDesign.FQ })
                    {
                        #region Box
                        {
                            var pts = GetPolygon(rtBox);
                            var pth = Util.RoundCorners(pts, Corner);

                            #region OutBevel
                            var fOut = SKImageFilter.CreateDropShadow(0, 2, 0, 0, Util.FromArgb(thm.OutBevelAlpha, SKColors.White));
                            p.ImageFilter = fOut;
                            #endregion
                            #region Fill
                            p.IsStroke = false;
                            p.Color = BoxColor;
                            Canvas.DrawPath(pth, p);
                            #endregion
                            #region Out Dispose
                            p.ImageFilter = null;
                            fOut.Dispose();
                            #endregion
                            #region InShadow
                            var sp = Canvas.Save();
                            Canvas.ClipPath(pth);
                            using (var mf = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1))
                            {
                                p.IsStroke = true;
                                p.StrokeWidth = 4;
                                p.Color = Util.FromArgb(thm.InShadowAlpha, SKColors.Black);
                                p.MaskFilter = mf;
                                Canvas.DrawPath(pth, p);
                                p.MaskFilter = null;
                            }
                            Canvas.RestoreToCount(sp);
                            #endregion
                            #region Border
                            p.IsStroke = true;
                            p.StrokeWidth = 1;
                            p.Color = BorderColor;
                            Canvas.DrawPath(pth, p);
                            #endregion

                            pth.Dispose();
                        }
                        #endregion

                        #region Button
                        {
                            var pts = GetPolygonBtn(rtButton, ng);
                            var pth = Util.RoundCorners(pts, Corner / 1.1F);
 
                            var cF = bDown ? ButtonColor.BrightnessTransmit(thm.DownBrightness) : ButtonColor;
                            var cB = bDown ? ButtonBorderColor.BrightnessTransmit(thm.DownBrightness) : ButtonBorderColor;
                            var cT = bDown ? ForeColor.BrightnessTransmit(thm.DownBrightness) : ForeColor;
 
                            #region Out
                            SKImageFilter fOut = null;
                            if (bDown)
                            {
                                #region OutBevel
                                fOut = SKImageFilter.CreateDropShadow(0, 2, 0, 0, Util.FromArgb(thm.OutBevelAlpha, SKColors.White));
                                //p.ImageFilter = fOut;
                                #endregion
                            }
                            else
                            {
                                #region OutShadow
                                fOut = SKImageFilter.CreateDropShadow(1, 1, 1, 1, Util.FromArgb(thm.OutShadowAlpha, SKColors.Black));
                                p.ImageFilter = fOut;
                                #endregion
                            }
                            #endregion
                            #region Fill
                            if (Gradient && !bDown)
                            {
                                var cS = ButtonColor.BrightnessTransmit(thm.GradientLight);
                                var cE = ButtonColor.BrightnessTransmit(thm.GradientDark);
                                var cS2 = Util.FromArgb(thm.GradientLightAlpha, SKColors.White);
                                var cE2 = Util.FromArgb(thm.GradientDarkAlpha, SKColors.Black);

                                using (var sh = SKShader.CreateLinearGradient(new SKPoint(rtContent.Left, rtContent.Top), new SKPoint(rtContent.Right, rtContent.Bottom), new SKColor[] { cS, cE }, SKShaderTileMode.Clamp))
                                {
                                    p.IsStroke = false;
                                    p.Color = SKColors.White;
                                    p.Shader = sh;

                                    Canvas.DrawPath(pth, p);

                                    p.Shader = null;
                                }

                                var tcp = Util.CenterPoint(pts.ToList());
                                var vcp = MathTool.GetPointWithAngle(tcp, 225, rtButton.Width / 4);

                                using (var sh = SKShader.CreateRadialGradient(vcp, rtContent.Width / 2F, new SKColor[] { cS2, cE2 }, SKShaderTileMode.Clamp))
                                {
                                    p.IsStroke = false;
                                    p.Color = SKColors.White;
                                    p.Shader = sh;

                                    Canvas.DrawPath(pth, p);

                                    p.Shader = null;
                                }
                            }
                            else
                            {
                                #region Fill
                                p.IsStroke = false;
                                p.Color = cF;
                                Canvas.DrawPath(pth, p);
                                #endregion
                            }

                            #endregion
                            #region Out Dispose
                            p.ImageFilter = null;
                            fOut.Dispose();
                            #endregion
                            #region In
                            if (bDown)
                            {
                                #region InShadow
                                {
                                    var sp = Canvas.Save();
                                    Canvas.ClipPath(pth);
                                    using (var mf = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1))
                                    {
                                        p.IsStroke = true;
                                        p.StrokeWidth = 4;
                                        p.Color = Util.FromArgb(thm.InShadowAlpha, SKColors.Black);
                                        p.MaskFilter = mf;
                                        Canvas.DrawPath(pth, p);
                                        p.MaskFilter = null;
                                    }
                                    Canvas.RestoreToCount(sp);
                                }
                                #endregion
                            }
                            else
                            {
                                #region InBevel
                                var sp = Canvas.Save();
                                Canvas.ClipPath(pth);
                                using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtButton.Left, rtButton.Top), new SKPoint(rtButton.Right, rtButton.Bottom),
                                                               new SKColor[] { thm.GetInBevelColor(cF), Util.FromArgb(0, cF) },
                                                               new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                                {
                                    p.Shader = lg;
                                    p.IsStroke = true;
                                    p.StrokeWidth = 4;
                                    Canvas.DrawPath(pth, p);
                                    p.Shader = null;
                                }
                                Canvas.RestoreToCount(sp);
                                #endregion
                            }
                            #endregion
                            #region Border
                            {
                                p.IsStroke = true;
                                p.StrokeWidth = 1;
                                p.Color = cB;
                                 
                                Canvas.DrawPath(pth, p);
                            }
                            #endregion
                            
                            pth.Dispose();
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
            bounds((rtContent, rtBox, rtButton, ng) =>
            {
                var pts = GetPolygon(rtContent);
                if (CollisionTool.CheckPolygon(pts, new SKPoint[] { new SKPoint(x, y), new SKPoint(x, y), new SKPoint(x, y) }))
                {
                    bDown = true;
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            bDown = false;
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, float> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
            var cp = MathTool.CenterPoint(rtContent);
            var rtBox = MathTool.MakeRectangle(cp, Math.Min(rtContent.Width, rtContent.Height));
            var rtButton = Util.FromRect(rtBox);
            var wh = Math.Min(rtBox.Width, rtBox.Height) * 0.1F;
            rtButton.Inflate(-wh, -wh);
            act(rtContent, rtBox, rtButton, wh);
        }
        #endregion
      
        #region GetPolygon
        SKPoint[] GetPolygon(SKRect rt)
        {
            var wh = Convert.ToSingle(Math.Min(rt.Width, rt.Height))/1.75F;
            var nwh = 0F;

            var ls = new List<SKPoint>();
            {
                var cp = MathTool.CenterPoint(rt);
                switch (Direction)
                {
                    case DvDirection.Up:
                        {
                            var sang = -90;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            var my = new SKPoint[] { p1, p2, p3 }.Max(x => x.Y);
                            nwh = (rt.Bottom - my) / 2F;

                            p1.Offset(0, nwh);
                            p2.Offset(0, nwh);
                            p3.Offset(0, nwh);

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;

                    case DvDirection.Down:
                        {
                            var sang = 90;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            var my = new SKPoint[] { p1, p2, p3 }.Min(x => x.Y);
                            nwh = (my - rt.Top) / 2F;

                            p1.Offset(0, -nwh);
                            p2.Offset(0, -nwh);
                            p3.Offset(0, -nwh);

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;

                    case DvDirection.Left:
                        {
                            var sang = 180;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            var mx = new SKPoint[] { p1, p2, p3 }.Max(x => x.X);
                            nwh = (rt.Right - mx) / 2F;

                            p1.Offset(nwh, 0);
                            p2.Offset(nwh, 0);
                            p3.Offset(nwh, 0);

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;

                    case DvDirection.Right:
                        {
                            var sang = 0;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            var mx = new SKPoint[] { p1, p2, p3 }.Min(x => x.X);
                            nwh = (mx - rt.Left) / 2F;

                            p1.Offset(-nwh, 0);
                            p2.Offset(-nwh, 0);
                            p3.Offset(-nwh, 0);

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;
                }
            }
            return ls.Count == 3 ? ls.ToArray() : null;
        }
        #endregion
        #region GetPolygonBtn
        SKPoint[] GetPolygonBtn(SKRect rt, float ng)
        {
            var wh = Convert.ToSingle(Math.Min(rt.Width, rt.Height)) / 1.75F;
            var nwh = 0F;
            
            var ls = new List<SKPoint>();
            {
                var cp = MathTool.CenterPoint(rt);
                switch (Direction)
                {
                    case DvDirection.Up:
                        {
                            var sang = -90;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            nwh = ng;
                            p1.Offset(0, nwh);
                            p2.Offset(0, nwh);
                            p3.Offset(0, nwh);

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;

                    case DvDirection.Down:
                        {
                            var sang = 90;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            nwh = ng;
                            p1.Offset(0, -nwh);
                            p2.Offset(0, -nwh);
                            p3.Offset(0, -nwh);

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;

                    case DvDirection.Left:
                        {
                            var sang = 180;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            nwh = ng;
                            p1.Offset(nwh, 0);
                            p2.Offset(nwh, 0);
                            p3.Offset(nwh, 0);

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;

                    case DvDirection.Right:
                        {
                            var sang = 0;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            nwh = ng;
                            p1.Offset(-nwh, 0);
                            p2.Offset(-nwh, 0);
                            p3.Offset(-nwh, 0);

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;
                }
            }
            return ls.Count == 3 ? ls.ToArray() : null;
        }
        #endregion
        #endregion

    }
 
}

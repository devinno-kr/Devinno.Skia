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
    public class DvCircleButton : DvControl
    {
        #region Properties
        public SKColor? ButtonColor { get; set; } = null;
        public SKColor? ButtonBackColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;

        public bool Gradient { get; set; } = true;

        #region Text
        public string Text { get; set; } = "Text";
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #region Icon
        public string IconString { get; set; } = null;
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        #endregion
        #endregion

        #region Member Variable
        bool bDown = false;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtCircleBack, rtCircle, rtText) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region Color
                    var ButtonColor = this.ButtonColor ?? thm.ButtonColor;
                    var ButtonBackColor = this.ButtonBackColor ?? thm.ConcaveBoxColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var Corner = thm.Corner;
                    var BorderColor = thm.GetBorderColor(ButtonColor, ButtonBackColor);
                    #endregion

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        var rt = rtCircle;

                        var cF = bDown ? ButtonColor.BrightnessTransmit(thm.DownBrightness) : ButtonColor;
                        var cB = bDown ? BorderColor.BrightnessTransmit(thm.DownBrightness) : BorderColor;
                        var cT = bDown ? ForeColor.BrightnessTransmit(thm.DownBrightness) : ForeColor;

                        #region Back
                        thm.DrawBox(Canvas, rtCircleBack, ButtonBackColor, BorderColor, RoundType.Ellipse, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel);
                        #endregion
                        #region Cursor
                        if (!bDown)
                        {
                            thm.DrawBox(Canvas, rt, cF, cB, RoundType.Ellipse, (Gradient ? BoxStyle.GradientV : BoxStyle.Fill) | BoxStyle.InBevel | BoxStyle.OutShadow);

                            var cx = Convert.ToSingle(MathTool.Map(0.25, 0, 1, rtCircle.Left, rtCircle.Right));
                            var cy = Convert.ToSingle(MathTool.Map(0.25, 0, 1, rtCircle.Top, rtCircle.Bottom));
                            var cS = Util.FromArgb(thm.GradientLightAlpha, SKColors.White);
                            var cE = Util.FromArgb(thm.GradientDarkAlpha, SKColors.Black);

                            using (var sh = SKShader.CreateRadialGradient(new SKPoint(cx, cy), rt.Width / 2F, new SKColor[] { cS, cE }, SKShaderTileMode.Clamp))
                            {
                                p.Shader = sh;

                                Canvas.DrawCircle(rt.MidX, rt.MidY, rt.Width / 2F, p);

                                p.Shader = null;
                            }

                            using (var lg = SKShader.CreateLinearGradient(new SKPoint(rt.Left, rt.Top), new SKPoint(rt.Right, rt.Bottom),
                                                               new SKColor[] { Util.FromArgb(30, SKColors.White), Util.FromArgb(30, SKColors.Black) },
                                                               new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                            {
                                p.Shader = lg;
                                p.IsStroke = true;
                                p.StrokeWidth = 2;
                                rt.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                                Canvas.DrawCircle(rt.MidX, rt.MidY, rt.Width / 2F-1, p);
                                p.Shader = null;
                            }

                            thm.DrawBox(Canvas, rtCircle, cF, cB, RoundType.Ellipse, BoxStyle.Border);
                        }
                        else thm.DrawBox(Canvas, rt, cF, cB, RoundType.Ellipse, BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel | BoxStyle.Border);
                        #endregion
                        #region Text
                        if (bDown) rtText.Offset(0, 1);
                        Util.DrawTextIcon(Canvas, Text, FontName, FontSize, DvFontStyle.Normal, IconGap, IconString, IconSize, cT, rtText, IconAlignment, DvContentAlignment.MiddleCenter, true, thm.OutShadowAlpha);
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
            bounds((rtContent, rtCircleBack, rtCircle, rtText) =>
            {
                if (CollisionTool.Check(rtCircle, x, y)) bDown = true;
            });
            base.OnMouseDown(x, y);
        }
        #endregion

        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            if (bDown)
            {
                bDown = false;
            }
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);
            var rtText = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, rtContent.Height - 1);
            var wh = Convert.ToInt32(Math.Min(rtContent.Width, rtContent.Height));
            var ng = Convert.ToInt32(rtContent.Height * 0.1F);
            //var ng = DvTheme.GP2;
            var rtCircleBack = Util.MakeRectangle(rtContent, new SKSize(wh, wh));
            var rtCircle  = Util.FromRect(rtCircleBack); rtCircle.Inflate(-ng, -ng);
            act(rtContent, rtCircleBack, Util.INT(rtCircle), rtText);
        }
        #endregion
        #endregion
    }
}

using Devinno.Collections;
using Devinno.Skia.Collections;
using Devinno.Skia.Controls;
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

namespace Devinno.Skia.Theme
{
    public class WhiteTheme : DvTheme
    {
        #region Const
        const BoxStyle BS_BTN_DOWN = BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.Border | BoxStyle.OutBevel;
        const BoxStyle BS_BTN_UP_FILL = BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.Border | BoxStyle.OutShadow;
        const BoxStyle BS_BTN_UP_GRAD = BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.Border | BoxStyle.OutShadow;
        const BoxStyle BS_BTN_UP_GRAD_H = BoxStyle.GradientH | BoxStyle.InBevel | BoxStyle.Border | BoxStyle.OutShadow;

        const BoxStyle BS_LBL = BoxStyle.Fill | BoxStyle.Border;
        const BoxStyle BS_LBL_VALUE = BoxStyle.Fill;
        const BoxStyle BS_LBL_TITLE = BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.Border;

        const BoxStyle BS_BOX = BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.Border;

        const BoxStyle BS_BOX_FILL = BoxStyle.Fill;
        const BoxStyle BS_BOX_BORDER = BoxStyle.Border | BoxStyle.InBevel;

        const BoxStyle BS_C_BACK = BoxStyle.Fill | BoxStyle.Border | BoxStyle.OutBevel;
        const BoxStyle BS_TOOL_CATEGORY = BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.Border;
        #endregion

        #region Properties
        public override float Corner => 6;
        public override SKColor ForeColor => SKColors.Black;
        public override SKColor BackColor => Util.FromArgb(180, 180, 180);
        public override SKColor ButtonColor => Util.FromArgb(220, 220, 220);
        public override SKColor LabelColor => Util.FromArgb(220, 220, 220);
        public override SKColor InputColor => Util.FromArgb(250, 250, 250);
        public override SKColor TitleColor => Util.FromArgb(210, 210, 210);
        public override SKColor WindowTitleColor => Util.FromArgb(220, 220, 220);
        public override SKColor PointColor => SKColors.DeepSkyBlue;
        public override SKColor ControlBackColor => Util.FromArgb(150, 150, 150);
        public override SKColor NeedelColor => Util.FromArgb(255, 255, 255);
        public override SKColor PanelColor => Util.FromArgb(190, 190, 190);
        public override SKColor GroupBoxColor => Util.FromArgb(90, 90, 90);
        public override SKColor ScrollBarColor => Util.FromArgb(150, 150, 150);
        public override SKColor ScrollCursorOffColor => Util.FromArgb(220, 220, 220);
        public override SKColor ScrollCursorOnColor => SKColors.DeepSkyBlue;
        public override SKColor GridColor => Util.FromArgb(60, 60, 60);
        public override SKColor ListBackColor => Util.FromArgb(220, 220, 220);
        public override SKColor ColumnColor => Util.FromArgb(190, 200, 210);
        public override SKColor SummaryColor => Util.FromArgb(70, 70, 70);

        public override byte OutShadowAlpha => 20;
        public override byte OutBevelAlpha => 60;
        public override byte InShadowAlpha => 60;
        public override float GradientLight => 0.1F;
        public override float GradientDark => -0.1F;
        public override float DownBrightness => -0.1F;
        public override float BorderBrightness => -0.15F;
        #endregion

        #region Member Variable
        private static Animation dani = new Animation();
        #endregion

        #region Method : Control Draw
        #region DrawText
        public override void DrawText(SKCanvas canvas, SKRect rect, string fontName, float fontSize, DvFontStyle fontStyle, string text, SKColor foreColor, DvContentAlignment contentAlignment = DvContentAlignment.MiddleCenter, bool down = false)
        {
            var cText = down ? foreColor.BrightnessTransmit(this.DownBrightness) : foreColor;

            if (down) rect.Offset(0, 1);
            Util.DrawText(canvas, text, fontName, fontSize, fontStyle, cText, rect, contentAlignment);
        }

        public override void DrawText(SKCanvas canvas, SKRect rect, string iconString, float iconSize, SKColor foreColor, DvContentAlignment contentAlignment = DvContentAlignment.MiddleCenter, bool down = false)
        {
            var cText = down ? foreColor.BrightnessTransmit(this.DownBrightness) : foreColor;

            if (down) rect.Offset(0, 1);
            Util.DrawIcon(canvas, iconString, iconSize, cText, rect, contentAlignment);
        }

        public override void DrawText(SKCanvas canvas, SKRect rect, DvTextIcon texticon, SKColor foreColor, DvContentAlignment contentAlignment = DvContentAlignment.MiddleCenter, bool down = false)
        {
            var cText = down ? foreColor.BrightnessTransmit(this.DownBrightness) : foreColor;

            if (down) rect.Offset(0, 1);
            if (texticon != null) Util.DrawTextIcon(canvas, texticon, cText, rect, contentAlignment);
        }
        #endregion
        #region DrawButton
        public override void DrawButton(SKCanvas canvas,
            SKRect rect,
            SKColor buttonColor, SKColor borderColor, SKColor foreColor, SKColor backColor,
            DvTextIcon texticon,
            DvRoundType round, bool gradient, bool backgroundDraw,
            DvContentAlignment align,
            bool down)
        {
            var cBox = down ? buttonColor.BrightnessTransmit(this.DownBrightness) : buttonColor;
            var cBorder = down ? borderColor.BrightnessTransmit(this.DownBrightness) : borderColor;

            var style = BoxStyle.None;
            if (down) style = BS_BTN_DOWN;
            else
            {
                style = gradient ? BS_BTN_UP_GRAD : BS_BTN_UP_FILL;

            }
            if (backgroundDraw) DrawBox(canvas, rect, cBox, cBorder, round, style, Corner);

            if (texticon != null) DrawText(canvas, rect, texticon, foreColor, align, down);
        }
        #endregion
        #region DrawLabel
        public override void DrawLabel(SKCanvas canvas,
            SKRect rect,
            SKColor labelColor, SKColor borderColor, SKColor foreColor, SKColor backColor,
            DvTextIcon texticon,
            DvRoundType round, bool backgroundDraw,
            DvContentAlignment align, bool drawBorder = true)
        {
            var style = drawBorder ? BS_LBL : BS_LBL_VALUE;
            if (backgroundDraw) DrawBox(canvas, rect, labelColor, borderColor, round, style, Corner);

            if (texticon != null) DrawText(canvas, rect, texticon, foreColor, align);
        }
        #endregion
        #region DrawTitle
        public override void DrawTitle(SKCanvas canvas,
            SKRect rect,
            SKColor labelColor, SKColor borderColor, SKColor foreColor, SKColor backColor,
            DvTextIcon texticon,
            DvRoundType round, bool backgroundDraw)
        {
            if (backgroundDraw) DrawBox(canvas, rect, labelColor, borderColor, round, BS_LBL_TITLE, Corner);
            if (texticon != null) DrawText(canvas, rect, texticon, foreColor);
        }
        #endregion
        #region DrawValueOnOff
        public override void DrawValueOnOff(SKCanvas canvas,
            SKRect rect,
            SKRect rtOn, SKRect rtOff,
            SKColor foreColor,
            string onText, string offText, string fontName, float fontSize, DvFontStyle fontStyle,
            bool onOff,
            bool useAnimation, Animation ani)
        {
            var Value = onOff;
            var cL = foreColor;
            var cD = cL.WithAlpha(75);
            var cOn = Value ? cL : cD;
            var cOff = Value ? cD : cL;

            var MIS = 12F;
            var MIG = 5F;

            var isOn = Value ? MIS : 0F;
            var isOff = Value ? 0F : MIS;
            var igOn = Value ? MIG : 0;
            var igOff = Value ? 0F : MIG;

            if (useAnimation && ani.IsPlaying)
            {
                if (Value)
                {
                    cOn = ani.Value(AnimationAccel.DCL, cD, cL);
                    cOff = ani.Value(AnimationAccel.DCL, cL, cD);
                    isOn = ani.Value(AnimationAccel.DCL, 0F, MIS);
                    isOff = ani.Value(AnimationAccel.DCL, MIS, 0F);
                    igOn = ani.Value(AnimationAccel.DCL, 0F, MIG);
                    igOff = ani.Value(AnimationAccel.DCL, MIG, 0F);
                }
                else
                {
                    cOn = ani.Value(AnimationAccel.DCL, cL, cD);
                    cOff = ani.Value(AnimationAccel.DCL, cD, cL);
                    isOff = ani.Value(AnimationAccel.DCL, 0F, MIS);
                    isOn = ani.Value(AnimationAccel.DCL, MIS, 0F);
                    igOff = ani.Value(AnimationAccel.DCL, 0F, MIG);
                    igOn = ani.Value(AnimationAccel.DCL, MIG, 0F);
                }
            }

            var tON = new DvTextIcon { FontName = fontName, FontSize = fontSize, FontStyle = fontStyle, IconString = "fa-check", IconSize = isOn, Text = onText, IconGap = igOn };
            var tOFF = new DvTextIcon { FontName = fontName, FontSize = fontSize, FontStyle = fontStyle, IconString = "fa-check", IconSize = isOff, Text = offText, IconGap = igOff };

            Util.DrawTextIcon(canvas, tON, cOn, rtOn);
            Util.DrawTextIcon(canvas, tOFF, cOff, rtOff);

            using (var p = new SKPaint { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                p.Color = Util.FromArgb(90, SKColors.Black);
                p.StrokeWidth = 1;

                var x1 = rect.MidX;
                var x2 = rect.MidX + 1;
                var y1 = Convert.ToSingle(MathTool.Map(0.25, 0, 1, rect.Top, rect.Bottom));
                var y2 = Convert.ToSingle(MathTool.Map(0.75, 0, 1, rect.Top, rect.Bottom));

                p.Color = Util.FromArgb(30, SKColors.White); canvas.DrawLine(x2, y1, x2, y2, p);
                p.Color = Util.FromArgb(90, SKColors.Black); canvas.DrawLine(x1, y1, x1, y2, p);
            }
        }
        #endregion
        #region DrawTriangleButton
        public override void DrawTriangleButton(SKCanvas canvas,
            SKRect rect, SKRect rtBox,
            SKColor buttonColor, SKColor buttonBackColor,
            DvDirection dir, bool gradient, float buttonPadding, float corner,
            bool down)
        {

            var borderColor = GetBorderColor(buttonColor, buttonBackColor);
            var cBox = down ? buttonColor.BrightnessTransmit(this.DownBrightness) : buttonColor;
            var cBorder = down ? borderColor.BrightnessTransmit(this.DownBrightness) : borderColor;
            var cOut = buttonBackColor;

            using (var p = new SKPaint { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Box
                var ptsBox = Util.GetPolygon(dir, rtBox);
                using (var pth = Util.RoundCorners(ptsBox, corner))
                {
                    using (var fOut = SKImageFilter.CreateDropShadow(0, 1.5F, 0, 0, Util.FromArgb(OutBevelAlpha, SKColors.White)))
                    {
                        p.ImageFilter = fOut;

                        p.IsStroke = false;
                        p.Color = cOut;
                        canvas.DrawPath(pth, p);

                        p.ImageFilter = null;

                        p.IsStroke = true;
                        p.Color = GetBorderColor(cOut, cOut);
                        canvas.DrawPath(pth, p);

                    }
                }
                #endregion
                #region Button
                var ptsBtn = Util.GetPolygonBtn(dir, MathTool.CenterPoint(ptsBox.ToList()), rect, buttonPadding);
                using (var pth = Util.RoundCorners(ptsBtn, corner / 1.2F))
                {
                    #region Fill
                    if (!down && gradient)
                    {
                        var cS = ButtonColor.BrightnessTransmit(GradientLight);
                        var cE = ButtonColor.BrightnessTransmit(GradientDark);
                        var cS2 = Util.FromArgb(30, SKColors.White);
                        var cE2 = Util.FromArgb(30, SKColors.Black);

                        using (var sh = SKShader.CreateLinearGradient(new SKPoint(rect.Left, rect.Top), new SKPoint(rect.Right, rect.Bottom), new SKColor[] { cS, cE }, SKShaderTileMode.Clamp))
                        {
                            p.IsStroke = false;
                            p.Color = SKColors.White;
                            p.Shader = sh;

                            canvas.DrawPath(pth, p);

                            p.Shader = null;
                        }

                        var tcp = Util.CenterPoint(ptsBtn.ToList());
                        var vcp = MathTool.GetPointWithAngle(tcp, 225, rect.Width / 4);

                        using (var sh = SKShader.CreateRadialGradient(vcp, rect.Width / 2F, new SKColor[] { cS2, cE2 }, SKShaderTileMode.Clamp))
                        {
                            p.IsStroke = false;
                            p.Color = SKColors.White;
                            p.Shader = sh;

                            canvas.DrawPath(pth, p);

                            p.Shader = null;
                        }
                    }
                    else
                    {
                        p.IsStroke = false;
                        p.Color = cBox;
                        canvas.DrawPath(pth, p);
                    }
                    #endregion

                    #region Effect
                    if (!down)
                    {
                        #region Bevel
                        {
                            var sp = canvas.Save();

                            canvas.ClipPath(pth);
                            canvas.Translate(1, 1);
                            p.IsStroke = true;
                            p.Color = Util.FromArgb(GetBevelAlpha(cBox), SKColors.White);
                            canvas.DrawPath(pth, p);

                            canvas.RestoreToCount(sp);
                        }
                        #endregion
                    }
                    else
                    {
                        #region InShadow
                        var sp = canvas.Save();

                        canvas.ClipPath(pth);
                        using (var mf = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1))
                        {
                            p.IsStroke = true;
                            p.StrokeWidth = 1;
                            p.Color = Util.FromArgb(InShadowAlpha, SKColors.Black);
                            p.MaskFilter = mf;

                            canvas.DrawPath(pth, p);

                            p.MaskFilter = null;
                        }

                        canvas.RestoreToCount(sp);
                        #endregion
                    }
                    #endregion

                    #region Border
                    p.IsStroke = true;
                    p.Color = cBorder;
                    canvas.DrawPath(pth, p);
                    #endregion
                }
                #endregion
            }

        }
        #endregion
        #region DrawCircleButton
        public override void DrawCircleButton(SKCanvas canvas,
            SKRect rect, SKRect rtCircleBack, SKRect rtCircle, SKRect rtText,
            SKColor buttonColor, SKColor buttonBackColor,
            bool gradient, bool down)
        {
            var borderColor = GetBorderColor(buttonColor, buttonBackColor);
            var cBox = down ? buttonColor.BrightnessTransmit(this.DownBrightness) : buttonColor;
            var cBorder = down ? borderColor.BrightnessTransmit(this.DownBrightness) : borderColor;
            var cOut = buttonBackColor;
            var cp = new SKPoint(rtCircle.MidX, rtCircle.MidY);

            using (var p = new SKPaint { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Box
                DrawBox(canvas, rtCircleBack, cOut, cBorder, DvRoundType.Ellipse, BS_C_BACK, 0);
                #endregion
                #region Button
                {
                    #region Fill
                    if (!down && gradient)
                    {
                        var cS = ButtonColor.BrightnessTransmit(GradientLight);
                        var cE = ButtonColor.BrightnessTransmit(GradientDark);
                        var cS2 = Util.FromArgb(30, SKColors.White);
                        var cE2 = Util.FromArgb(30, SKColors.Black);

                        var tcp = new SKPoint(rtCircle.MidX, rtCircle.MidY);
                        var vcp = MathTool.GetPointWithAngle(tcp, 225, rtCircle.Width / 3F);

                        using (var sh = SKShader.CreateLinearGradient(new SKPoint(rect.Left, rect.Top), new SKPoint(rect.Right, rect.Bottom), new SKColor[] { cS, cE }, SKShaderTileMode.Clamp))
                        {
                            p.IsStroke = false;
                            p.Color = SKColors.White;
                            p.Shader = sh;

                            canvas.DrawCircle(tcp, rtCircle.Width / 2F, p);

                            p.Shader = null;
                        }

                        using (var sh = SKShader.CreateRadialGradient(vcp, rect.Width / 2F, new SKColor[] { cS2, cE2 }, SKShaderTileMode.Clamp))
                        {
                            p.IsStroke = false;
                            p.Color = SKColors.White;
                            p.Shader = sh;

                            canvas.DrawCircle(tcp, rtCircle.Width / 2F, p);

                            p.Shader = null;
                        }
                    }
                    else
                    {
                        var tcp = new SKPoint(rtCircle.MidX, rtCircle.MidY);

                        p.IsStroke = false;
                        p.Color = cBox;
                        canvas.DrawCircle(tcp, rtCircle.Width / 2F, p);
                    }
                    #endregion

                    #region Effect
                    using (var pth = PathTool.Circle(cp.X, cp.Y, rtCircle.Width / 2F))
                    {
                        if (!down)
                        {
                            #region Bevel
                            {
                                var sp = canvas.Save();

                                canvas.ClipPath(pth);
                                canvas.Translate(1, 1);
                                p.IsStroke = true;
                                p.StrokeWidth = 2;
                                p.Color = Util.FromArgb(GetBevelAlpha(cBox), SKColors.White);
                                canvas.DrawPath(pth, p);

                                canvas.RestoreToCount(sp);
                            }
                            #endregion
                        }
                        else
                        {
                            #region InShadow
                            var sp = canvas.Save();

                            canvas.ClipPath(pth);
                            using (var mf = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1))
                            {
                                p.IsStroke = true;
                                p.StrokeWidth = 1;
                                p.Color = Util.FromArgb(InShadowAlpha, SKColors.Black);
                                p.MaskFilter = mf;

                                canvas.DrawPath(pth, p);

                                p.MaskFilter = null;
                            }

                            canvas.RestoreToCount(sp);
                            #endregion
                        }
                    }
                    #endregion

                    #region Border
                    p.IsStroke = true;
                    p.StrokeWidth = 1;
                    p.Color = cBorder;
                    canvas.DrawCircle(cp, rtCircle.Width / 2F, p);
                    #endregion
                }
                #endregion
            }
        }
        #endregion
        #region DrawCheckBox
        public override void DrawCheckBox(SKCanvas canvas,
            SKRect rect, float BoxSize,
            SKColor boxColor, SKColor borderColor, SKColor checkColor, SKColor foreColor,
            string text, string fontName, float fontSize, DvFontStyle fontStyle, DvContentAlignment align,
            bool check)
        {
            var rtIco = Util.FromRect(0, 0, BoxSize, BoxSize);
            Util.TextIconBounds(text, fontName, fontSize, fontStyle, 8, rect, rtIco, DvTextIconAlignment.LeftRight, align,
                (rtBox, rtText) =>
                {
                    DrawBox(canvas, rtBox, boxColor, borderColor, DvRoundType.Rect, BS_C_BACK, 0);
                    DrawText(canvas, rtText, fontName, fontSize, fontStyle, text, foreColor, align);

                    #region Check
                    if (check)
                    {
                        using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                        {
                            p.StrokeCap = SKStrokeCap.Round;
                            p.StrokeJoin = SKStrokeJoin.Round;
                            p.StrokeWidth = 4F;
                            p.Color = checkColor;
                            p.IsStroke = true;

                            using (var pth = PathTool.Check(rtBox))
                            {
                                canvas.DrawPath(pth, p);
                            }
                        }
                    }
                    #endregion

                });
        }
        #endregion
        #region DrawRadioBox
        public override void DrawRadioBox(SKCanvas canvas,
            SKRect rect, float BoxSize,
            SKColor boxColor, SKColor borderColor, SKColor checkColor, SKColor foreColor,
            string text, string fontName, float fontSize, DvFontStyle fontStyle, DvContentAlignment align,
            bool check)
        {
            var rtIco = Util.FromRect(0, 0, BoxSize, BoxSize);
            Util.TextIconBounds(text, fontName, fontSize, fontStyle, 8, rect, rtIco, DvTextIconAlignment.LeftRight, align,
                (rtBox, rtText) =>
                {
                    var INF = rtBox.Width / 4F;
                    var rtCheck = Util.FromRect(rtBox.Left, rtBox.Top, rtBox.Width, rtBox.Height);
                    rtCheck.Inflate(-INF, -INF);

                    DrawBox(canvas, rtBox, boxColor, borderColor, DvRoundType.Ellipse, BS_C_BACK, 0);
                    DrawText(canvas, rtText, fontName, fontSize, fontStyle, text, foreColor, align);

                    #region Check
                    if (check)
                    {
                        DrawBox(canvas, rtCheck, checkColor, borderColor, DvRoundType.Ellipse, BoxStyle.Fill | BoxStyle.OutShadow, 0);
                    }
                    #endregion

                });
        }
        #endregion
        #region DrawLamp
        public override void DrawLamp(SKCanvas canvas,
            SKRect rect,
            SKColor onLampColor, SKColor offLampColor, SKColor foreColor, SKColor backColor, SKColor borderColor,
            string text, string fontName, float fontSize, DvFontStyle fontStyle,
            int lampSize, DvTextIconAlignment lampAlignment, DvContentAlignment contentAlignment, bool onOff,
            bool useAnimation, Animation ani)
        {
            var rtIco = Util.FromRect(0, 0, lampSize, lampSize);
            Util.TextIconBounds(text, fontName, fontSize, fontStyle, 8, rect, rtIco, lampAlignment, contentAlignment,
                (rtLamp, rtText) =>
                {
                    rtLamp.Offset(0, 1);

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        SKColor cBS, cBE, cS, cE, cM;
                        GetLampColors(backColor, onLampColor, offLampColor, onOff, useAnimation, ani, out cBE, out cBS, out cS, out cE, out cM);

                        #region Back
                        p.IsStroke = false;
                        using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtLamp.Left, rtLamp.Top),
                                                                      new SKPoint(rtLamp.Left, rtLamp.Bottom),
                                                                      new SKColor[] { cBE, cBS },
                                                                      SKShaderTileMode.Clamp))
                        {
                            p.Shader = lg;
                            p.Color = SKColors.White;
                            canvas.DrawCircle(MathTool.CenterPoint(rtLamp), rtLamp.Width / 2F, p);
                            p.Shader = null;
                        }
                        #endregion
                        #region Lamp
                        var ng = rtLamp.Height / 8;
                        rtLamp.Inflate(-ng, -ng);

                        var cx = Convert.ToSingle(MathTool.Map(0.25, 0, 1, rtLamp.Left, rtLamp.Right));
                        var cy = Convert.ToSingle(MathTool.Map(0.25, 0, 1, rtLamp.Top, rtLamp.Bottom));

                        using (var sh = SKShader.CreateRadialGradient(new SKPoint(cx, cy), rtLamp.Width / 2F, new SKColor[] { cS, cE }, SKShaderTileMode.Clamp))
                        {
                            p.Shader = sh;

                            canvas.DrawCircle(rtLamp.MidX, rtLamp.MidY, rtLamp.Width / 2F, p);

                            p.Shader = null;
                        }

                        p.IsStroke = true;
                        p.StrokeWidth = 1;
                        p.Color = cM.BrightnessTransmit(-0.6F);
                        canvas.DrawCircle(rtLamp.MidX, rtLamp.MidY, rtLamp.Width / 2F, p);
                        #endregion
                    }

                    DrawText(canvas, rtText, fontName, fontSize, fontStyle, text, foreColor, contentAlignment);
                });
        }
        #endregion
        #region DrawOnOff
        public override void DrawOnOff(SKCanvas canvas,
            SKRect rect, SKRect rtArea, SKRect rtCursor, SKRect rtOn, SKRect rtOff, float cursorWidth, float onoffWidth,
            SKColor CursorColor, SKColor OnBoxColor, SKColor OffBoxColor, SKColor OnTextColor, SKColor OffTextColor, SKColor BackColor,
            string onText, string offText, string fontName, float fontSize, DvFontStyle fontStyle,
            bool OnOff, bool bCurDown)
        {
            #region color
            var cCursor = bCurDown ? CursorColor.BrightnessTransmit(DownBrightness) : CursorColor;
            #region var cBox;
            var cBox = OnOff ? OnBoxColor : OffBoxColor;
            {
                var v = Math.Abs(rtCursor.Left - rtArea.Left) / onoffWidth;
                v = MathTool.Constrain(v, 0, 1);

                var a = Convert.ToByte(MathTool.Map(v, 0F, 1F, OffBoxColor.Alpha, OnBoxColor.Alpha));
                var r = Convert.ToByte(MathTool.Map(v, 0F, 1F, OffBoxColor.Red, OnBoxColor.Red));
                var g = Convert.ToByte(MathTool.Map(v, 0F, 1F, OffBoxColor.Green, OnBoxColor.Green));
                var b = Convert.ToByte(MathTool.Map(v, 0F, 1F, OffBoxColor.Blue, OnBoxColor.Blue));

                cBox = Util.FromArgb(a, r, g, b);
            }
            #endregion
            var cCursorBorder = cBox.BrightnessTransmit(BorderBrightness);
            var cBoxBorder = GetBorderColor(cBox, BackColor);
            var cOnText = OnTextColor;
            var cOffText = OffTextColor;
            #endregion

            DrawBox(canvas, rect, cBox, cBoxBorder, DvRoundType.All, BS_C_BACK, rect.Height / 2F);
            DrawBox(canvas, rtCursor, cCursor, cCursorBorder, DvRoundType.All, BS_BTN_UP_GRAD, rect.Height / 2);

            {
                var sp = canvas.Save();
                canvas.ClipRoundRect(new SKRoundRect(rtArea));
                DrawText(canvas, rtOn, fontName, fontSize, fontStyle, onText, cOnText, DvContentAlignment.MiddleCenter);
                DrawText(canvas, rtOff, fontName, fontSize, fontStyle, offText, cOffText, DvContentAlignment.MiddleCenter);

                var rti = Util.FromRect(rtCursor);
                var cIco = cOffText;
                if (bCurDown) cIco = cOnText;
                Util.DrawIcon(canvas, "power_settings_new", 14, cIco, rti);

                canvas.RestoreToCount(sp);
            }
        }
        #endregion
        #region DrawSwitch
        public override void DrawSwitch(SKCanvas canvas,
            SKRect rtContent, SKRect rtSwitch, SKRect rtOn, SKRect rtOff, SKRect rtOnText, SKRect rtOffText,
            SKColor switchColor, SKColor onBoxColor, SKColor offBoxColor, SKColor foreColor,
            DvTextIcon OnTextIcon, DvTextIcon OffTextIcon,
            float Corner, bool OnOff, bool useAnimation, Animation ani)
        {
            var SO = 0.075F;
            var EO = 1F - SO;

            #region Color
            #region var BoxColor;
            var BoxColor = OnOff ? onBoxColor : offBoxColor;
            if (useAnimation && ani.IsPlaying)
            {
                if (ani.Variable == "ON") BoxColor = ani.Value(AnimationAccel.Linear, offBoxColor, onBoxColor);
                else if (ani.Variable == "OFF") BoxColor = ani.Value(AnimationAccel.Linear, onBoxColor, offBoxColor);
            }
            #endregion
            var BoxBorderColor = GetBorderColor(BoxColor, BackColor);
            #endregion

            var rtvBorder = new SKRoundRect(rtSwitch, Corner);
            DrawBox(canvas, rtContent, BoxColor, BoxBorderColor, DvRoundType.All, BS_C_BACK, Corner);

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Fill
                #region Color
                SKColor c1, c2, c3, c4;
                GetSwitchColors(switchColor, out c1, out c2, out c3, out c4);

                var ca1 = !OnOff ? c2 : c4;
                var ca2 = !OnOff ? c3 : c1;
                var ca3 = !OnOff ? c1 : c3;
                var ca4 = !OnOff ? c4 : c2;

                var va1 = !OnOff ? 0 : 0;
                var va2 = !OnOff ? 0.5F : SO;
                var va3 = !OnOff ? EO : 0.5F;
                var va4 = !OnOff ? 1 : 1;

                if (useAnimation && ani.IsPlaying)
                {
                    if (ani.Variable == "ON")
                    {
                        ca1 = ani.Value(AnimationAccel.DCL, c2, c4);
                        ca2 = ani.Value(AnimationAccel.DCL, c3, c1);
                        ca3 = ani.Value(AnimationAccel.DCL, c1, c3);
                        ca4 = ani.Value(AnimationAccel.DCL, c4, c2);

                        va2 = ani.Value(AnimationAccel.DCL, 0.5F, SO);
                        va3 = ani.Value(AnimationAccel.DCL, EO, 0.5F);
                    }
                    else if (ani.Variable == "OFF")
                    {
                        ca1 = ani.Value(AnimationAccel.DCL, c4, c2);
                        ca2 = ani.Value(AnimationAccel.DCL, c1, c3);
                        ca3 = ani.Value(AnimationAccel.DCL, c3, c1);
                        ca4 = ani.Value(AnimationAccel.DCL, c2, c4);

                        va2 = ani.Value(AnimationAccel.DCL, SO, 0.5F);
                        va3 = ani.Value(AnimationAccel.DCL, 0.5F, EO);
                    }
                }
                #endregion

                using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtSwitch.Left, rtSwitch.Top),
                                                            new SKPoint(rtSwitch.Right, rtSwitch.Top),
                                                            new SKColor[] { ca1, ca2, ca3, ca4 },
                                                            new float[] { va1, va2, va3, va4 },
                                                            SKShaderTileMode.Clamp))
                {
                    p.Shader = lg;
                    canvas.DrawRoundRect(new SKRoundRect(rtSwitch, Corner), p);
                    p.Shader = null;
                }
                #endregion
                #region Bevel
                {
                    var VLC = SO;
                    var VRC = EO;

                    var vLx = MathTool.Map(VLC, 0D, 1D, rtSwitch.Left, rtSwitch.Right);
                    var vRx = MathTool.Map(VRC, 0D, 1D, rtSwitch.Left, rtSwitch.Right);
                    var cpx = MathTool.CenterPoint(rtSwitch).X;
                    var rtL = Util.FromRect(rtOff.Left, rtOff.Top, rtOff.Width, rtOff.Height); rtL.Inflate(-1, -1);
                    var rtR = Util.FromRect(rtOn.Left, rtOn.Top, rtOn.Width, rtOn.Height); rtR.Inflate(-1, -1);
                    var rtLV = Util.FromRect(Convert.ToInt32(vLx), rtOn.Top, Convert.ToInt32(cpx - vLx), rtOn.Height);
                    var rtRV = Util.FromRect(Convert.ToInt32(cpx), rtOff.Top, Convert.ToInt32(vRx - cpx), rtOff.Height);

                    p.IsStroke = true;
                    p.StrokeWidth = 2;
                    var bc = SKColors.White.WithAlpha(GetBevelAlpha(c1));
                    if (OnOff)
                    {
                        #region Right
                        using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtR.Right + 1, rtR.Top),
                                                           new SKPoint(rtR.Left, rtR.Bottom),
                                                            new SKColor[] { bc, SKColors.Transparent },
                                                           SKShaderTileMode.Clamp))
                        {
                            var rtm = (useAnimation && ani.IsPlaying ? ani.Value(AnimationAccel.DCL, rtRV, rtR) : rtR);
                            var rt = new SKRoundRect(rtm, Corner);
                            rt.SetNinePatch(rtm, 0, Corner, Corner, Corner);

                            var sp = canvas.Save();

                            p.Shader = lg;
                            canvas.DrawRoundRect(rt, p);
                            p.Shader = null;

                            canvas.RestoreToCount(sp);
                        }
                        #endregion
                        #region Left
                        using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtLV.Left, rtLV.Top),
                                                           new SKPoint(rtLV.Right, rtLV.Bottom),
                                                            new SKColor[] { bc, SKColors.Transparent },
                                                           SKShaderTileMode.Clamp))
                        {
                            var rtm = (useAnimation && ani.IsPlaying ? ani.Value(AnimationAccel.DCL, rtL, rtLV) : rtLV);
                            var rt = new SKRoundRect(rtm, Corner);
                            rt.SetNinePatch(rtm, Corner, Corner, 0, Corner);
                            rt.Inflate(-1, -1);

                            var sp = canvas.Save();

                            p.Shader = lg;
                            canvas.DrawRoundRect(rt, p);
                            p.Shader = null;

                            canvas.RestoreToCount(sp);
                        }
                        #endregion
                    }
                    else
                    {
                        #region Left
                        using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtL.Left, rtL.Top),
                                                           new SKPoint(rtL.Right, rtL.Bottom),
                                                            new SKColor[] { bc, SKColors.Transparent },
                                                           SKShaderTileMode.Clamp))
                        {
                            var rtm = (useAnimation && ani.IsPlaying ? ani.Value(AnimationAccel.DCL, rtLV, rtL) : rtL);
                            var rt = new SKRoundRect(rtm, Corner);
                            rt.SetNinePatch(rtm, Corner, Corner, 0, Corner);

                            var sp = canvas.Save();

                            p.Shader = lg;
                            canvas.DrawRoundRect(rt, p);
                            p.Shader = null;

                            canvas.RestoreToCount(sp);
                        }
                        #endregion
                        #region Right
                        using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtRV.Right, rtRV.Top),
                                                           new SKPoint(rtRV.Left, rtRV.Bottom),
                                                            new SKColor[] { bc, SKColors.Transparent },
                                                           SKShaderTileMode.Clamp))
                        {
                            var rtm = (useAnimation && ani.IsPlaying ? ani.Value(AnimationAccel.DCL, rtR, rtRV) : rtRV);
                            var rt = new SKRoundRect(rtm, Corner);
                            rt.SetNinePatch(rtm, 0, Corner, Corner, Corner);
                            rt.Inflate(-1, -1);

                            var sp = canvas.Save();

                            p.Shader = lg;
                            canvas.DrawRoundRect(rt, p);
                            p.Shader = null;

                            canvas.RestoreToCount(sp);
                        }
                        #endregion
                    }

                    p.IsStroke = true;
                    p.StrokeWidth = 1;
                    p.Color = Util.FromArgb(90, SKColors.Black);
                    canvas.DrawLine(rtSwitch.MidX, rtSwitch.Top, rtSwitch.MidX, rtSwitch.Bottom, p);
                }
                #endregion
                #region Border
                p.IsStroke = true;
                p.StrokeWidth = 1;
                p.Color = BoxBorderColor;
                rtvBorder.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                canvas.DrawRoundRect(rtvBorder, p);
                #endregion
                #region Text
                var cTL = foreColor;
                var cTD = foreColor.BrightnessTransmit(-0.4F);
                var cOn = OnOff ? cTL : cTD;
                var cOff = OnOff ? cTD : cTL;

                var TON = new DvTextIcon
                {
                    FontName = OnTextIcon.FontName,
                    FontSize = OnTextIcon.FontSize,
                    FontStyle = OnTextIcon.FontStyle,
                    Text = OnTextIcon.Text,
                    IconString = OnTextIcon.IconString,
                    IconSize = OnTextIcon.IconSize,
                    IconGap = OnTextIcon.IconGap,
                    IconAlignment = OnTextIcon.IconAlignment,
                };

                var TOF = new DvTextIcon
                {
                    FontName = OffTextIcon.FontName,
                    FontSize = OffTextIcon.FontSize,
                    FontStyle = OffTextIcon.FontStyle,
                    Text = OffTextIcon.Text,
                    IconString = OffTextIcon.IconString,
                    IconSize = OffTextIcon.IconSize,
                    IconGap = OffTextIcon.IconGap,
                    IconAlignment = OffTextIcon.IconAlignment,
                };

                if (useAnimation && ani.IsPlaying)
                {
                    if (ani.Variable == "ON")
                    {
                        TON.IconSize = ani.Value(AnimationAccel.Linear, 0, OnTextIcon.IconSize);
                        TOF.IconSize = ani.Value(AnimationAccel.Linear, OffTextIcon.IconSize, 0);
                        TON.IconGap = ani.Value(AnimationAccel.Linear, 0, OnTextIcon.IconGap);
                        TOF.IconGap = ani.Value(AnimationAccel.Linear, OffTextIcon.IconGap, 0);
                        cOn = ani.Value(AnimationAccel.Linear, cTD, cTL);
                        cOff = ani.Value(AnimationAccel.Linear, cTL, cTD);
                    }
                    else if (ani.Variable == "OFF")
                    {
                        TON.IconSize = ani.Value(AnimationAccel.Linear, OnTextIcon.IconSize, 0);
                        TOF.IconSize = ani.Value(AnimationAccel.Linear, 0, OffTextIcon.IconSize);
                        TON.IconGap = ani.Value(AnimationAccel.Linear, OnTextIcon.IconGap, 0);
                        TOF.IconGap = ani.Value(AnimationAccel.Linear, 0, OffTextIcon.IconGap);
                        cOn = ani.Value(AnimationAccel.Linear, cTL, cTD);
                        cOff = ani.Value(AnimationAccel.Linear, cTD, cTL);
                    }
                }
                else
                {
                    TON.IconSize = OnOff ? OnTextIcon.IconSize : 0;
                    TOF.IconSize = OnOff ? 0 : OffTextIcon.IconSize;
                    TON.IconGap = OnOff ? OnTextIcon.IconGap : 0;
                    TOF.IconGap = OnOff ? 0 : OffTextIcon.IconGap;
                    cOn = OnOff ? cTL : cTD;
                    cOff = OnOff ? cTD : cTL;
                }

                DrawText(canvas, rtOnText, TON, cOn, DvContentAlignment.MiddleCenter);
                DrawText(canvas, rtOffText, TOF, cOff, DvContentAlignment.MiddleCenter);

                #endregion
            }
        }
        #endregion
        #region DrawProgress
        public override void DrawProgress(SKCanvas canvas,
            SKRect rtContent, SKRect rtEmpty, SKRect rtFill,
            SKColor boxColor, SKColor barColor, SKColor foreColor, SKColor backColor,
            DvDirectionHV direction, bool drawText, bool reverse,
            string text, string fontName, float fontSize, DvFontStyle fontStyle)
        {

            var borderColor = GetBorderColor(barColor, boxColor);
            var boxBorderColor = GetBorderColor(boxColor, backColor);

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                DrawBox(canvas, rtEmpty, boxColor, boxBorderColor, DvRoundType.All, BS_C_BACK, Corner);

                if ((direction == DvDirectionHV.Vertical && rtFill.Height > 0) || (direction == DvDirectionHV.Horizon && rtFill.Width > 0))
                    DrawBox(canvas, rtFill, barColor, borderColor, DvRoundType.All, direction == DvDirectionHV.Horizon ? BS_BTN_UP_GRAD : BS_BTN_UP_GRAD_H, Corner);

                if (drawText)
                {
                    var sz = new SKRect();
                    p.MeasureText(text, ref sz);

                    if (direction == DvDirectionHV.Vertical)
                    {
                        var h = Convert.ToInt32(sz.Height + 5);
                        var rt = Util.FromRect(rtFill.Left - 1, reverse ? rtFill.Bottom - h : rtFill.Top, rtFill.Width, h);

                        if (rtFill.Width > 0)
                        {
                            var sp = canvas.Save();
                            canvas.ClipRect(rtFill);
                            var rtv = Util.FromRect(rtFill); rtv.Inflate(0, -10);
                            Util.DrawText(canvas, text, fontName, fontSize, fontStyle, foreColor, rtv, reverse ? DvContentAlignment.BottomCenter : DvContentAlignment.TopCenter);
                            canvas.RestoreToCount(sp);
                        }
                    }
                    else if (direction == DvDirectionHV.Horizon)
                    {
                        var w = Convert.ToInt32(sz.Width + 5);
                        var rt = Util.FromRect(reverse ? rtFill.Left + 5 : rtFill.Right - w, rtFill.Top, w, rtFill.Height - 1);

                        if (rtFill.Width > 0)
                        {
                            var sp = canvas.Save();
                            canvas.ClipRect(rtFill);
                            var rtv = Util.FromRect(rtFill); rtv.Inflate(-10, 0);
                            Util.DrawText(canvas, text, fontName, fontSize, fontStyle, foreColor, rtv, reverse ? DvContentAlignment.MiddleLeft : DvContentAlignment.MiddleRight);
                            canvas.RestoreToCount(sp);
                        }
                    }
                }
            }
        }
        #endregion
        #region DrawSlider
        public override void DrawSlider(SKCanvas canvas,
            SKRect rtContent, SKRect rtEmpty, SKRect rtFill, SKRect rtCur,
            SKColor boxColor, SKColor barColor, SKColor cursorColor, SKColor graduationColor, SKColor foreColor, SKColor backColor,
            DvDirectionHV direction, bool reverse,
            double min, double max, double? tick, float barSize, float cursorSize,
            string text, string fontName, float fontSize, DvFontStyle fontStyle,
            bool bCurDown)
        {
            var borderColor = GetBorderColor(barColor, boxColor);
            var boxBorderColor = GetBorderColor(boxColor, backColor);
            var cursorBorderColor = GetBorderColor(cursorColor, backColor);

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                if (direction == DvDirectionHV.Horizon)
                {
                    #region Horizon
                    #region Bar
                    DrawBox(canvas, rtEmpty, boxColor, boxBorderColor, DvRoundType.All, BS_C_BACK, Corner);

                    if (rtFill.Width > 0)
                        DrawBox(canvas, rtFill, barColor, borderColor, DvRoundType.All, BoxStyle.GradientV | BoxStyle.Border | BoxStyle.InBevelLT, Corner);
                    #endregion
                    #region Graduation
                    if (tick.HasValue)
                    {
                        for (double v = min; v <= max; v += tick.Value)
                        {
                            if (v != min && v != max)
                            {
                                var x = Convert.ToSingle(Math.Floor(MathTool.Map(v, min, max, rtEmpty.Left, rtEmpty.Right))) + 0.5F;
                                //var x = Convert.ToSingle(MathTool.Map(v, min, max, rtEmpty.Left, rtEmpty.Right));
                                var y = rtEmpty.MidY;
                                var gp = 7;
                                p.IsAntialias = false;
                                p.IsStroke = true;
                                p.StrokeWidth = 1;
                                p.PathEffect = SKPathEffect.CreateDash(new float[] { 1, 1, }, 1);
                                p.Color = graduationColor;
                                canvas.DrawLine(x, rtEmpty.Top + 5, x, rtEmpty.Bottom - 6, p);
                                p.PathEffect = null;
                                p.IsAntialias = DvDesign.AA;
                            }
                        }
                    }
                    #endregion
                    #region var
                    var rt = new SKRect();
                    #endregion

                    #region Path : Cursor
                    rtCur.Inflate(-0.5F, -0.5F);
                    using (var path = PathTool.SliderCursor(rtCur, direction, Corner, out rt))
                    {
                        var cCur = bCurDown ? cursorColor.BrightnessTransmit(DownBrightness) : cursorColor;
                        var cCurBor = bCurDown ? cursorBorderColor.BrightnessTransmit(DownBrightness) : cursorBorderColor;
                        DrawPath(canvas, path, cCur, cCurBor, BoxStyle.Fill | BoxStyle.Border | BoxStyle.InBevelLT);
                        DrawText(canvas, rt, fontName, fontSize, fontStyle, text, foreColor, DvContentAlignment.MiddleCenter);
                    }
                    #endregion
                    #endregion
                }
                else if (direction == DvDirectionHV.Vertical)
                {
                    #region Vertical
                    #region Bar
                    DrawBox(canvas, rtEmpty, boxColor, boxBorderColor, DvRoundType.All, BS_C_BACK, Corner);

                    if (rtFill.Height > 0)
                        DrawBox(canvas, rtFill, barColor, borderColor, DvRoundType.All, BoxStyle.GradientH | BoxStyle.Border | BoxStyle.InBevelLT, Corner);
                    #endregion
                    #region Graduation
                    if (tick.HasValue)
                    {
                        for (double v = min; v <= max; v += tick.Value)
                        {
                            if (v != min && v != max)
                            {
                                var x = rtEmpty.MidX;
                                var y = Convert.ToSingle(Math.Floor(MathTool.Map(v, min, max, rtEmpty.Top, rtEmpty.Bottom))) + 0.5F;
                                //var y = Convert.ToSingle(MathTool.Map(v, min, max, rtEmpty.Top, rtEmpty.Bottom));
                                var gp = 7;
                                p.IsAntialias = false;
                                p.IsStroke = true;
                                p.StrokeWidth = 1;
                                p.PathEffect = SKPathEffect.CreateDash(new float[] { 1, 1, }, 1);
                                p.Color = graduationColor;
                                canvas.DrawLine(rtEmpty.Left + 5, y, rtEmpty.Right - 6, y, p);
                                p.PathEffect = null;
                                p.IsAntialias = DvDesign.AA;
                            }
                        }
                    }
                    #endregion
                    #region var
                    var rt = new SKRect();
                    #endregion

                    #region Path : Cursor
                    rtCur.Inflate(-0.5F, -0.5F);
                    using (var path = PathTool.SliderCursor(rtCur, direction, Corner, out rt))
                    {
                        var cCur = bCurDown ? cursorColor.BrightnessTransmit(DownBrightness) : cursorColor;
                        var cCurBor = bCurDown ? cursorBorderColor.BrightnessTransmit(DownBrightness) : cursorBorderColor;
                        DrawPath(canvas, path, cCur, cCurBor, BoxStyle.Fill | BoxStyle.Border | BoxStyle.InBevelLT);
                        DrawText(canvas, rt, fontName, fontSize, fontStyle, text, foreColor, DvContentAlignment.MiddleCenter);
                    }
                    #endregion
                    #endregion
                }
            }
        }
        #endregion
        #region DrawRangeSlider
        public override void DrawRangeSlider(SKCanvas canvas,
           SKRect rtContent, SKRect rtEmpty, SKRect rtFill, SKRect rtCurStart, SKRect rtCurEnd,
           SKColor boxColor, SKColor barColor, SKColor cursorColor, SKColor graduationColor, SKColor foreColor, SKColor backColor,
           DvDirectionHV direction, bool reverse,
           double min, double max, double? tick, float barSize, float cursorSize,
           string textS, string textE, string fontName, float fontSize, DvFontStyle fontStyle,
           bool bRangeStartCurDown, bool bRangeEndCurDown)
        {
            var borderColor = GetBorderColor(barColor, boxColor);
            var boxBorderColor = GetBorderColor(boxColor, backColor);
            var cursorBorderColor = GetBorderColor(cursorColor, backColor);

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                if (direction == DvDirectionHV.Horizon)
                {
                    #region Horizon
                    #region Bar
                    DrawBox(canvas, rtEmpty, boxColor, boxBorderColor, DvRoundType.All, BS_C_BACK, Corner);

                    if (rtFill.Width > 0)
                        DrawBox(canvas, rtFill, barColor, borderColor, DvRoundType.All, BoxStyle.GradientV | BoxStyle.Border | BoxStyle.InBevelLT, Corner);
                    #endregion
                    #region Graduation
                    if (tick.HasValue)
                    {
                        for (double v = min; v <= max; v += tick.Value)
                        {
                            if (v != min && v != max)
                            {
                                var x = Convert.ToSingle(Math.Floor(MathTool.Map(v, min, max, rtEmpty.Left, rtEmpty.Right))) + 0.5F;
                                //var x = Convert.ToSingle(MathTool.Map(v, min, max, rtEmpty.Left, rtEmpty.Right));
                                var y = rtEmpty.MidY;
                                p.IsAntialias = false;
                                p.IsStroke = true;
                                p.StrokeWidth = 1;
                                p.PathEffect = SKPathEffect.CreateDash(new float[] { 1, 1, }, 1);
                                p.Color = graduationColor;
                                canvas.DrawLine(x, rtEmpty.Top + 5, x, rtEmpty.Bottom - 6, p);
                                p.PathEffect = null;
                                p.IsAntialias = DvDesign.AA;
                            }
                        }
                    }
                    #endregion
                    #region var
                    var ax = 10F;
                    var ay = 7F;
                    var r = Corner * 2F;
                    #endregion

                    #region Path : Start
                    rtCurStart.Inflate(-0.5F, -0.5F);
                    var rtS = new SKRect();
                    using (var path = PathTool.RangeSliderCursorStart(rtCurStart, direction, reverse, Corner, out rtS))
                    {
                        var cCur = bRangeStartCurDown ? cursorColor.BrightnessTransmit(DownBrightness) : cursorColor;
                        var cCurBor = bRangeStartCurDown ? cursorBorderColor.BrightnessTransmit(DownBrightness) : cursorBorderColor;
                        DrawPath(canvas, path, cCur, cCurBor, BoxStyle.Fill | BoxStyle.Border | BoxStyle.InBevelLT);
                        DrawText(canvas, rtS, fontName, fontSize, fontStyle, textS, foreColor, DvContentAlignment.MiddleCenter);
                    }
                    #endregion
                    #region Path : End
                    rtCurEnd.Inflate(-0.5F, -0.5F);
                    var rtE = new SKRect();
                    using (var path = PathTool.RangeSliderCursorEnd(rtCurEnd, direction, reverse, Corner, out rtE))
                    {
                        var cCur = bRangeEndCurDown ? cursorColor.BrightnessTransmit(DownBrightness) : cursorColor;
                        var cCurBor = bRangeEndCurDown ? cursorBorderColor.BrightnessTransmit(DownBrightness) : cursorBorderColor;
                        DrawPath(canvas, path, cCur, cCurBor, BoxStyle.Fill | BoxStyle.Border | BoxStyle.InBevelLT);
                        DrawText(canvas, rtE, fontName, fontSize, fontStyle, textE, foreColor, DvContentAlignment.MiddleCenter);
                    }
                    #endregion
                    #endregion
                }
                else if (direction == DvDirectionHV.Vertical)
                {
                    #region Vertical
                    #region Bar
                    DrawBox(canvas, rtEmpty, boxColor, boxBorderColor, DvRoundType.All, BS_C_BACK, Corner);

                    if (rtFill.Height > 0)
                        DrawBox(canvas, rtFill, barColor, borderColor, DvRoundType.All, BoxStyle.GradientH | BoxStyle.Border | BoxStyle.InBevelLT, Corner);
                    #endregion
                    #region Graduation
                    if (tick.HasValue)
                    {
                        for (double v = min; v <= max; v += tick.Value)
                        {
                            if (v != min && v != max)
                            {
                                var x = rtEmpty.MidX;
                                var y = Convert.ToSingle(Math.Floor(MathTool.Map(v, min, max, rtEmpty.Top, rtEmpty.Bottom))) + 0.5F;
                                //var y = Convert.ToSingle(MathTool.Map(v, min, max, rtEmpty.Top, rtEmpty.Bottom));
                                p.IsAntialias = false;
                                p.IsStroke = true;
                                p.StrokeWidth = 1;
                                p.PathEffect = SKPathEffect.CreateDash(new float[] { 1, 1, }, 1);
                                p.Color = graduationColor;
                                canvas.DrawLine(rtEmpty.Left + 5, y, rtEmpty.Right - 6, y, p);
                                p.PathEffect = null;
                                p.IsAntialias = DvDesign.AA;
                            }
                        }
                    }
                    #endregion
                    #region var
                    var ax = 7F;
                    var ay = 10F;
                    var r = Corner * 2F;

                    #endregion

                    #region Path : Start
                    rtCurStart.Inflate(-0.5F, -0.5F);
                    var rtS = new SKRect();
                    using (var path = PathTool.RangeSliderCursorStart(rtCurStart, direction, reverse, Corner, out rtS))
                    {
                        var cCur = bRangeStartCurDown ? cursorColor.BrightnessTransmit(DownBrightness) : cursorColor;
                        var cCurBor = bRangeStartCurDown ? cursorBorderColor.BrightnessTransmit(DownBrightness) : cursorBorderColor;
                        DrawPath(canvas, path, cCur, cCurBor, BoxStyle.Fill | BoxStyle.Border | BoxStyle.InBevelLT);
                        DrawText(canvas, rtS, fontName, fontSize, fontStyle, textS, foreColor, DvContentAlignment.MiddleCenter);
                    }
                    #endregion
                    #region Path : End
                    rtCurEnd.Inflate(-0.5F, -0.5F);
                    var rtE = new SKRect();
                    using (var path = PathTool.RangeSliderCursorEnd(rtCurEnd, direction, reverse, Corner, out rtE))
                    {
                        var cCur = bRangeEndCurDown ? cursorColor.BrightnessTransmit(DownBrightness) : cursorColor;
                        var cCurBor = bRangeEndCurDown ? cursorBorderColor.BrightnessTransmit(DownBrightness) : cursorBorderColor;
                        DrawPath(canvas, path, cCur, cCurBor, BoxStyle.Fill | BoxStyle.Border | BoxStyle.InBevelLT);
                        DrawText(canvas, rtE, fontName, fontSize, fontStyle, textE, foreColor, DvContentAlignment.MiddleCenter);
                    }
                    #endregion
                    #endregion
                }
            }
        }
        #endregion
        #region DrawStepGauge
        public override void DrawStepGauge(SKCanvas canvas,
           SKRect rtContent, SKRect? rtBtnL, SKRect[] rtBtns, SKRect? rtBtnR,
           SKColor onColor, SKColor offColor, SKColor buttonColor, SKColor foreColor, SKColor backColor,
           int stepCount, int step, bool useButton, bool drawButton, float StepPadding,
           DvIcon leftIcon, DvIcon rightIcon, bool bLeftDown, bool bRightDown)
        {
            #region color
            var borderColor = GetBorderColor(buttonColor, backColor);
            var offborderColor = GetBorderColor(offColor, backColor);
            var onborderColor = GetBorderColor(onColor, backColor);
            #endregion

            #region Left
            if (rtBtnL.HasValue && useButton)
            {
                var rt = rtBtnL.Value;

                if (drawButton)
                    DrawButton(canvas,
                        rt,
                        buttonColor, borderColor, foreColor, backColor,
                        null,
                        DvRoundType.All,
                        true, true,
                        DvContentAlignment.MiddleCenter, bLeftDown);

                var cfL = bLeftDown ? foreColor.BrightnessTransmit(DownBrightness) : foreColor;
                if (bLeftDown) rt.Offset(0, 1);
                Util.DrawIcon(canvas, leftIcon, cfL, rt);
            }
            #endregion
            #region Right
            if (rtBtnR.HasValue && useButton)
            {
                var rt = rtBtnR.Value;
                if (drawButton)
                    DrawButton(canvas,
                        rt,
                        buttonColor, borderColor, foreColor, backColor,
                        null,
                        DvRoundType.All,
                        true, true,
                        DvContentAlignment.MiddleCenter, bLeftDown);

                var cfR = bRightDown ? foreColor.BrightnessTransmit(DownBrightness) : foreColor;
                if (bRightDown) rt.Offset(0, 1);
                Util.DrawIcon(canvas, rightIcon, cfR, rt);
            }
            #endregion

            for (int i = 0; i < stepCount; i++)
            {
                var rt = rtBtns[i];
                DrawBox(canvas, rt, offColor, offborderColor, DvRoundType.All, BS_C_BACK, Corner);

                if (i == step)
                {
                    rt.Inflate(-StepPadding, -StepPadding);
                    DrawBox(canvas, rt, onColor, onborderColor, DvRoundType.All, BS_BTN_UP_GRAD, Corner);
                }
            }
        }
        #endregion
        #region DrawGauge
        public override void DrawGauge(SKCanvas canvas,
            SKRect rtContent, SKRect rtGauge, SKRect rtValue, SKRect rtTitle,
            SKColor fillColor, SKColor emptyColor, SKColor foreColor, SKColor backColor,
            double minimum, double maximum, double value,
            string text, string valueFontName, float valueFontSize, DvFontStyle valueFontStyle,
            string title, string titleFontName, float titleFontSize, DvFontStyle titleFontStyle,
            float startAngle, float sweepAngle, float barSize, float barPadding)
        {
            rtGauge.Inflate(-0.5F, -0.5F);

            var emptyBorderColor = GetBorderColor(emptyColor, backColor);
            var fillBorderColor = GetBorderColor(fillColor, emptyBorderColor);

            using (var p = new SKPaint { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Empty
                using (var path = PathTool.Gauge(rtGauge, startAngle, sweepAngle, barSize))
                {
                    DrawPath(canvas, path, emptyColor, emptyBorderColor, BS_C_BACK);
                }
                #endregion

                #region Fill
                var Ang = Convert.ToSingle(MathTool.Map(value, minimum, maximum, 0, sweepAngle));
                if (Ang > 0)
                {
                    var rtOut = Util.FromRect(rtGauge); rtOut.Inflate(-barPadding, -barPadding);

                    using (var path = PathTool.Gauge(rtOut, startAngle, Ang, barSize - (barPadding * 2)))
                    {
                        DrawPath(canvas, path, fillColor, fillBorderColor, BoxStyle.Fill | BoxStyle.Border | BoxStyle.InBevelLT, 2, 1, 0.5F);
                    }
                }
                #endregion

                #region Text
                DrawText(canvas, rtValue, valueFontName, valueFontSize, valueFontStyle, text, foreColor, DvContentAlignment.MiddleCenter);
                DrawText(canvas, rtTitle, titleFontName, titleFontSize, titleFontStyle, title, foreColor, DvContentAlignment.MiddleCenter);
                #endregion
            }
        }
        #endregion
        #region DrawMeter
        public override void DrawMeter(SKCanvas canvas,
            SKRect rtContent, SKRect rtGauge, SKRect rtValue, SKRect rtTitle,
            SKColor needleColor, SKColor needlePointColor, SKColor foreColor, SKColor backColor,
            double minimum, double maximum, double value, double graduationLarge, double graduationSmall,
            bool drawText,
            string text, string valueFontName, float valueFontSize, DvFontStyle valueFontStyle,
            string title, string titleFontName, float titleFontSize, DvFontStyle titleFontStyle,
            string remarkFontName, float remarkFontSize, DvFontStyle remarkFontStyle,
            float startAngle, float sweepAngle, List<MeterBar> bars)
        {
            rtGauge.Inflate(-0.5F, -0.5F);

            var tw = remarkFontSize + 20;
            var rtCircleIn = Util.FromRect(rtGauge); rtCircleIn.Inflate(-tw, -tw);
            var rtBar = Util.FromRect(rtCircleIn); rtBar.Inflate(-5, -5);
            var rwh = rtCircleIn.Width / 2F;
            var gT = 20;
            var gL = 10;
            var gS = 5;
            var distN = rwh + gL;

            var borderColor = GetBorderColor(backColor, needleColor);

            using (var p = new SKPaint { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                var cp = new SKPoint(rtContent.MidX, rtContent.MidY);

                #region Remark
                p.IsStroke = true;
                p.Color = foreColor;
                p.StrokeWidth = 2;

                canvas.DrawArc(rtCircleIn, startAngle, sweepAngle, false, p);
                for (double i = minimum; i <= maximum; i += graduationLarge)
                {
                    var gsang = Convert.ToSingle(MathTool.Map(MathTool.Constrain(i, minimum, maximum), minimum, maximum, 0D, sweepAngle)) + startAngle;
                    var pB = MathTool.GetPointWithAngle(cp, gsang, rwh);
                    var pL = MathTool.GetPointWithAngle(cp, gsang, rwh + gL);
                    var pT = MathTool.GetPointWithAngle(cp, gsang, rwh + gT);
                    canvas.DrawLine(pB, pL, p);

                    var sp = canvas.Save();
                    canvas.Translate(pT);
                    canvas.RotateDegrees(gsang + 90);

                    var rt = MathTool.MakeRectangle(new SKPoint(0, 0), 60);
                    Util.DrawText(canvas, i.ToString(), remarkFontName, remarkFontSize, remarkFontStyle, foreColor, rt);

                    canvas.RestoreToCount(sp);
                }

                p.StrokeWidth = 1;
                for (double i = minimum; i <= maximum; i += graduationSmall)
                {
                    var gsang = Convert.ToSingle(MathTool.Map(MathTool.Constrain(i, minimum, maximum), minimum, maximum, 0D, sweepAngle)) + startAngle;
                    var pB = MathTool.GetPointWithAngle(cp, gsang, rwh);
                    var pS = MathTool.GetPointWithAngle(cp, gsang, rwh + gS);
                    canvas.DrawLine(pB, pS, p);
                }
                #endregion
                #region Bar
                if (bars != null && bars.Count > 0)
                {
                    foreach (var bar in bars)
                    {
                        var vangMin = Convert.ToSingle(MathTool.Map(MathTool.Constrain(bar.Minimum, minimum, maximum), minimum, maximum, 0, sweepAngle));
                        var vangMax = Convert.ToSingle(MathTool.Map(MathTool.Constrain(bar.Maximum, minimum, maximum), minimum, maximum, 0, sweepAngle));

                        p.IsStroke = true;
                        p.Color = bar.Color;
                        p.StrokeWidth = 5;

                        canvas.DrawArc(rtBar, vangMin + startAngle, vangMax - vangMin, false, p);
                    }
                }
                #endregion
                #region Needle
                using (var path = PathTool.MeterNeedle(rtContent, rtGauge, rtCircleIn,
                    value, minimum, maximum, startAngle, sweepAngle,
                    remarkFontSize, gT, gL, gS))
                {
                    using (var filter = SKImageFilter.CreateDropShadow(1, 1, 1, 1, Util.FromArgb(OutShadowAlpha, SKColors.Black)))
                    {
                        p.ImageFilter = filter;

                        #region Fill
                        p.IsStroke = false;
                        using (var lg = SKShader.CreateRadialGradient(cp, distN, new SKColor[] { needleColor, needleColor, needlePointColor, needlePointColor }, new float[] { 0, 0.6F, 0.61F, 1 }, SKShaderTileMode.Clamp))
                        {
                            p.Shader = lg;

                            canvas.DrawPath(path, p);
                            p.ImageFilter = null;

                            p.Shader = null;
                        }
                        #endregion
                        #region Pin
                        p.Color = borderColor;
                        canvas.DrawCircle(cp, 3, p);
                        #endregion
                        #region Border
                        p.IsStroke = true;
                        p.StrokeWidth = 1;
                        p.Color = borderColor;
                        canvas.DrawPath(path, p);
                        #endregion

                        p.ImageFilter = null;
                    }
                }
                #endregion
                #region Text
                if (drawText)
                {
                    DrawText(canvas, rtValue, valueFontName, valueFontSize, valueFontStyle, text, foreColor, DvContentAlignment.MiddleCenter);
                    DrawText(canvas, rtTitle, titleFontName, titleFontSize, titleFontStyle, title, foreColor, DvContentAlignment.MiddleCenter);
                }
                #endregion
            }
        }
        #endregion
        #region DrawKnob
        public override void DrawKnob(SKCanvas canvas,
            SKRect rtContent, SKRect rtKnobBack, SKRect rtKnob, SKRect rtCursor,
            SKColor knobColor, SKColor knobBackColor, SKColor fillColor, SKColor onLampColor, SKColor offLampColor, SKColor foreColor, SKColor backColor,
            double minimum, double maximum, double value, double? tick,
            float startAngle, float sweepAngle, bool cursorDownState, float knobPadding)
        {
            var backBorderColor = GetBorderColor(knobBackColor, backColor);
            var borderColor = GetBorderColor(knobColor, knobBackColor);
            DrawBox(canvas, rtKnobBack, knobBackColor, backBorderColor, DvRoundType.Ellipse, BS_C_BACK, 0);

            using (var p = new SKPaint { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                var cp = new SKPoint(rtContent.MidX, rtContent.MidY);

                using (var path = PathTool.Knob(rtContent, rtKnob))
                {
                    #region Gradient
                    using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtKnob.Left, rtKnob.Top),
                                                                        new SKPoint(rtKnob.Right, rtKnob.Bottom),
                                                                        new SKColor[] { knobColor.BrightnessTransmit(GradientLight), knobColor.BrightnessTransmit(GradientDark) },
                                                                        new float[] { 0, 1 },
                                                                        SKShaderTileMode.Clamp))
                    {
                        p.Shader = lg;
                        canvas.DrawPath(path, p);
                        p.Shader = null;
                    }
                    #endregion
                    #region Fill
                    {
                        var ang = Convert.ToSingle(MathTool.Map(MathTool.Constrain(value, minimum, maximum), minimum, maximum, 0, sweepAngle));
                        var rtFill = Util.FromRect(rtKnobBack); rtFill.Inflate(-knobPadding / 2F, -knobPadding / 2F);
                        p.IsStroke = true;
                        p.StrokeWidth = knobPadding;
                        p.Color = fillColor;
                        canvas.DrawArc(rtFill, startAngle, ang, false, p);
                    }
                    #endregion
                    #region Radial
                    var cS = SKColors.White.WithAlpha(30);
                    var cE = SKColors.White.WithAlpha(0);
                    var cx = rtKnob.Left + rtKnob.Width / 4F;
                    var cy = rtKnob.Top + rtKnob.Height / 4F;
                    using (var sh = SKShader.CreateRadialGradient(new SKPoint(cx, cy), rtKnob.Width / 2F, new SKColor[] { cS, cE }, SKShaderTileMode.Clamp))
                    {
                        p.Shader = sh;

                        canvas.DrawPath(path, p);

                        p.Shader = null;
                    }
                    #endregion
                    #region Bevel
                    var sp = canvas.Save();
                    canvas.ClipPath(path);
                    using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtKnob.Left, rtKnob.Top), new SKPoint(rtKnob.Left, rtKnob.Bottom),
                                                                new SKColor[] { Util.FromArgb(20, SKColors.White), Util.FromArgb(90, SKColors.Black) },
                                                                new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                    {
                        p.Shader = lg;
                        p.IsStroke = true;
                        p.StrokeWidth = 6;

                        canvas.DrawPath(path, p);

                        p.Shader = null;
                    }
                    canvas.RestoreToCount(sp);
                    #endregion
                    #region Border
                    p.IsStroke = true;
                    p.StrokeWidth = 1;
                    p.Color = borderColor;
                    canvas.DrawPath(path, p);
                    #endregion
                    #region Cursor
                    {
                        DrawLamp(canvas, rtCursor,
                            onLampColor, offLampColor, foreColor, ButtonColor, borderColor,
                            "", "", 10, DvFontStyle.Normal, 24, DvTextIconAlignment.LeftRight,
                            DvContentAlignment.MiddleCenter, cursorDownState,
                            false, new Animation());

                    }
                    #endregion
                }
            }

            DrawBox(canvas, rtKnobBack, knobBackColor, backBorderColor, DvRoundType.Ellipse, BoxStyle.Border, 0);

        }
        #endregion
        #region DrawPictureBox
        public override void DrawPictureBox(SKCanvas canvas,
            SKRect rtContent, SKColor boxColor, SKColor backColor, PictureScaleMode scaleMode, SKBitmap image)
        {
            var borderColor = GetBorderColor(boxColor, backColor);

            var rt = new SKRoundRect(rtContent, Corner);

            var sp = canvas.Save();
            canvas.ClipRoundRect(rt);
            DrawBox(canvas, rtContent, boxColor, borderColor, DvRoundType.All, BS_C_BACK, Corner);
            if (image != null)
            {
                #region Image
                var cx = rtContent.MidX;
                var cy = rtContent.MidY;
                switch (scaleMode)
                {
                    case PictureScaleMode.Real:
                        canvas.DrawBitmap(image, Util.FromRect(rtContent.Left, rtContent.Top, image.Width, image.Height));
                        break;
                    case PictureScaleMode.CenterImage:
                        canvas.DrawBitmap(image, Util.FromRect(cx - (image.Width / 2), cy - (image.Height / 2), image.Width, image.Height));
                        break;
                    case PictureScaleMode.Strech:
                        canvas.DrawBitmap(image, rtContent);
                        break;
                    case PictureScaleMode.Zoom:
                        double imgratio = 1D;
                        if ((image.Width - rtContent.Width) > (image.Height - rtContent.Height)) imgratio = (double)rtContent.Width / (double)image.Width;
                        else imgratio = (double)rtContent.Height / (double)image.Height;

                        int szw = Convert.ToInt32((double)image.Width * imgratio);
                        int szh = Convert.ToInt32((double)image.Height * imgratio);

                        canvas.DrawBitmap(image, Util.FromRect(rtContent.Left + (rtContent.Width / 2) - (szw / 2), rtContent.Top + (rtContent.Height / 2) - (szh / 2), szw, szh));
                        break;
                }
                #endregion
            }
            else
            {
                var c = boxColor.BrightnessTransmit(1F);
                Util.DrawText(canvas, "No Image", "NanumGothic", 12, DvFontStyle.Normal, c, rtContent);
            }
            canvas.RestoreToCount(sp);

            DrawBox(canvas, rtContent, boxColor, borderColor, DvRoundType.All, BoxStyle.Border, Corner);
        }
        #endregion
        #region DrawSelector
        public override void DrawSelector(SKCanvas canvas,
            SKRect rtContent, SKRect rtPrev, SKRect rtValue, SKRect rtNext, List<SKRect> rtItems,
            SKColor selectorColor, SKColor foreColor, SKColor backColor,
            string fontName, float fontSize, DvFontStyle fontStyle,
            float iconSize, float iconGap, DvTextIconAlignment iconAlignment,
            List<SelectorItem> items, bool backgroundDraw, DvDirectionHV direction, DvRoundType? Round,
            int selectedIndex,
            bool bPrev, bool bNext, bool useAnimation, Animation ani)
        {
            var borderColor = GetBorderColor(selectorColor, backColor);
            var round = Round ?? DvRoundType.All;
            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Background
                if (backgroundDraw)
                {
                    DrawBox(canvas, rtContent, selectorColor, borderColor, round, BS_BOX, Corner);

                    p.StrokeWidth = 1F;
                    p.IsStroke = true;
                    p.PathEffect = SKPathEffect.CreateDash(new float[] { 1, 1, }, 1);
                    if (direction == DvDirectionHV.Horizon)
                    {
                        var y1 = rtContent.Top + 10F;
                        var y2 = rtContent.Bottom - 10F;
                        var xP = Convert.ToSingle(Math.Round(rtPrev.Right)) + 0.5F;
                        var xN = Convert.ToSingle(Math.Round(rtNext.Left)) + 0.5F;

                        p.Color = Util.FromArgb(GetBevelAlpha(selectorColor), SKColors.White);
                        canvas.DrawLine(xP + 1F, y1, xP + 1F, y2, p);
                        canvas.DrawLine(xN + 1F, y1, xN + 1F, y2, p);

                        p.Color = Util.FromArgb(90, SKColors.Black);
                        canvas.DrawLine(xP, y1, xP, y2, p);
                        canvas.DrawLine(xN, y1, xN, y2, p);
                    }
                    else if (direction == DvDirectionHV.Vertical)
                    {
                        var x1 = rtContent.Left + 10F;
                        var x2 = rtContent.Right - 10F;
                        var yP = Convert.ToSingle(Math.Round(rtPrev.Bottom)) + 0.5F;
                        var yN = Convert.ToSingle(Math.Round(rtNext.Top)) + 0.5F;

                        p.Color = Util.FromArgb(GetBevelAlpha(selectorColor), SKColors.White);
                        canvas.DrawLine(x1, yP + 1F, x2, yP + 1F, p);
                        canvas.DrawLine(x1, yN + 1F, x2, yN + 1F, p);

                        p.Color = borderColor;
                        canvas.DrawLine(x1, yP, x2, yP, p);
                        canvas.DrawLine(x1, yN, x2, yN, p);
                    }
                    p.PathEffect = null;
                }
                #endregion
                #region Button
                if (direction == DvDirectionHV.Horizon)
                {
                    DrawText(canvas, rtPrev, new DvTextIcon { IconString = "fa-chevron-left", IconSize = iconSize }, foreColor, DvContentAlignment.MiddleCenter, bPrev);
                    DrawText(canvas, rtNext, new DvTextIcon { IconString = "fa-chevron-right", IconSize = iconSize }, foreColor, DvContentAlignment.MiddleCenter, bNext);
                }
                else if (direction == DvDirectionHV.Vertical)
                {
                    DrawText(canvas, rtPrev, new DvTextIcon { IconString = "fa-chevron-up", IconSize = iconSize }, foreColor, DvContentAlignment.MiddleCenter, bPrev);
                    DrawText(canvas, rtNext, new DvTextIcon { IconString = "fa-chevron-down", IconSize = iconSize }, foreColor, DvContentAlignment.MiddleCenter, bNext);
                }
                #endregion
                #region Values
                if (selectedIndex >= 0)
                {
                    var sp = canvas.Save();
                    canvas.ClipRect(rtValue);
                    var lsr = rtItems.Where(x => CollisionTool.Check(rtValue, x));
                    foreach (var rt in lsr)
                    {
                        var idx = rtItems.IndexOf(rt);
                        if (idx >= 0 && idx < rtItems.Count)
                        {
                            var v = items[idx];
                            var c = foreColor;

                            var ti = new DvTextIcon
                            {
                                Text = v.Text,
                                FontName = fontName,
                                FontSize = fontSize,
                                FontStyle = fontStyle,
                                IconString = v.IconString,
                                IconSize = iconSize,
                                IconGap = iconGap,
                                IconAlignment = iconAlignment,

                            };

                            DrawText(canvas, rt, ti, c, DvContentAlignment.MiddleCenter, false);
                        }
                    }
                    canvas.RestoreToCount(sp);
                }
                #endregion
                #region Border
                if (backgroundDraw) DrawBox(canvas, rtContent, selectorColor, borderColor, round, BoxStyle.Border, Corner);
                #endregion
            }
        }
        #endregion
        #region DrawCalendar
        public override void DrawCalendar(SKCanvas canvas,
           SKRect rtContent, SKRect rtMonthly, SKRect rtWeekly, SKRect rtDays, SKRect rtMonthPrev, SKRect rtMonthNext, SKRect rtMonthText,
           Dictionary<string, SKRect> dicBoxes, Dictionary<string, SKRect> dicWeeks,
           SKColor boxColor, SKColor selectColor, SKColor foreColor, SKColor backColor,
           string monthText, string fontName, float fontSize, DvFontStyle fontStyle,
           int currentYear, int currentMonth, List<DateTime> selectedDays,
           bool backgroundDraw,
           bool bMonthPrev, bool bMonthNext)
        {
            var borderColor = GetBorderColor(boxColor, backColor);
            var round = DvRoundType.All;

            #region DayList
            int Days = DateTime.DaysInMonth(currentYear, currentMonth);
            DateTime dt = new DateTime(currentYear, currentMonth, 1);
            int ndw = (int)dt.DayOfWeek;
            DateTime[] d = new DateTime[42];
            int startidx = ndw == 0 ? 7 : ndw;
            int endidx = startidx + Days;
            if (dt.Date.Year == 1 && dt.Date.Month == 1 && dt.Date.Day == 1) { }
            else dt -= new TimeSpan(startidx, 0, 0, 0);

            for (int i = 0; i < 42; i++)
            {
                d[i] = dt;
                dt += new TimeSpan(1, 0, 0, 0);
            }
            #endregion

            #region Background
            if (backgroundDraw) DrawBox(canvas, rtContent, boxColor, borderColor, round, BS_BOX, Corner);
            #endregion
            #region Prev / Next
            var cP = bMonthPrev ? foreColor.BrightnessTransmit(DownBrightness) : foreColor;
            var cN = bMonthNext ? foreColor.BrightnessTransmit(DownBrightness) : foreColor;
            DrawText(canvas, rtMonthPrev, "fa-chevron-left", fontSize, cP, DvContentAlignment.MiddleCenter, bMonthPrev);
            DrawText(canvas, rtMonthNext, "fa-chevron-right", fontSize, cN, DvContentAlignment.MiddleCenter, bMonthNext);
            DrawText(canvas, rtMonthText, fontName, fontSize, fontStyle, monthText, foreColor);
            #endregion
            #region Week
            for (int ix = 0; ix < 7; ix++)
            {
                var rt = dicWeeks["rtWeek_" + ix];
                string s = "";
                var c = foreColor;
                switch ((DayOfWeek)ix)
                {
                    case DayOfWeek.Sunday: s = "SUN"; c = SKColors.Red; break;
                    case DayOfWeek.Monday: s = "MON"; break;
                    case DayOfWeek.Tuesday: s = "TUE"; break;
                    case DayOfWeek.Wednesday: s = "WED"; break;
                    case DayOfWeek.Thursday: s = "THR"; break;
                    case DayOfWeek.Friday: s = "FRI"; break;
                    case DayOfWeek.Saturday: s = "SAT"; c = SKColors.DeepSkyBlue; break;
                }

                DrawText(canvas, rt, fontName, fontSize, fontStyle, s, c);
            }
            #endregion
            #region Days
            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                for (int iy = 0; iy < 6; iy++)
                {
                    for (int ix = 0; ix < 7; ix++)
                    {
                        var rt = dicBoxes["rtBox_" + iy + "_" + ix];
                        var idx = iy * 7 + ix;
                        var tm = d[idx];
                        if (!selectedDays.Contains(tm))
                        {
                            var ctext = foreColor;
                            var s = tm.Day.ToString();

                            if (idx >= startidx && idx < endidx)
                            {
                                ctext = (ix == 0 ? SKColors.Red : (ix == 6 ? SKColors.DeepSkyBlue : foreColor));
                                DrawText(canvas, rt, fontName, fontSize, fontStyle, s, ctext);
                            }
                            else
                            {
                                ctext = Util.FromArgb(90, foreColor);
                                DrawText(canvas, rt, fontName, fontSize, fontStyle, s, ctext);
                            }
                        }
                    }
                }
            }
            #endregion
            #region SelectDays
            foreach (var v in selectedDays)
            {
                var sidx = d.ToList().IndexOf(v.Date);
                if (sidx >= 0)
                {
                    #region Bounds
                    var iy = sidx / 7;
                    var ix = sidx - (iy * 7);

                    var rt = dicBoxes["rtBox_" + iy + "_" + ix];
                    var rtsh = rt; rtsh.Offset(0, 1);
                    var idx = iy * 7 + ix;
                    var tm = d[idx];
                    #endregion

                    var c = selectColor;
                    var ctext = foreColor;

                    var rtv = rt;
                    DrawBox(canvas, rtv, c, c, DvRoundType.Rect, BoxStyle.Fill, Corner);

                    var s = tm.Day.ToString();
                    DrawText(canvas, rt, fontName, fontSize, fontStyle, s, ctext);
                }
            }
            #endregion
        }
        #endregion
        #region DrawInput
        public override void DrawInput(SKCanvas canvas,
            SKRect rtContent,
            SKColor inputColor, SKColor borderColor, SKColor foreColor, SKColor backColor,
            DvRoundType round, bool backgroundDraw,
            bool drawBorder = true)
        {
            var style = drawBorder ? BS_LBL : BS_LBL_VALUE;
            if (backgroundDraw) DrawBox(canvas, rtContent, inputColor, borderColor, round, style, Corner);

        }
        #endregion
        #region DrawBarGraph
        public override void DrawBarGraph(SKCanvas canvas,
            SKRect rtContent, SKRect rtRemark, SKRect rtNameAxis, SKRect rtValueAxis, SKRect rtGraph, SKRect rtScroll, List<SKSize> szRemarks,
            SKColor gridColor, SKColor graphBackColor, SKColor foreColor, SKColor backColor,
            string fontName, float fontSize, DvFontStyle fontStyle,
            BarGraphMode graphMode, DvDirectionHV direction, bool valueDraw, string formatString,
            bool gradient, int barSize, int barGap, int dataWH,
            List<GraphSeries> series, double graduation, double minimum, double maximum,
            Scroll scroll, bool scrollable, bool touchMode,
            List<GraphValue> graphDatas)
        {
            #region var
            var scrollBorderColor = GetBorderColor(ScrollBarColor, backColor);
            var seriasBorderColor = GetBorderColor(backColor, backColor);
            var barBorderColor = GetBorderColor(graphBackColor, backColor);
            var TGP = 7;
            #region spos
            var spos = 0;
            float DataWH = dataWH;
            if (scrollable)
            {
                spos = Convert.ToInt32(scroll.ScrollPositionWithOffset);
            }
            else
            {
                if (direction == DvDirectionHV.Vertical) DataWH = rtNameAxis.Width / graphDatas.Count;
                else if (direction == DvDirectionHV.Horizon) DataWH = rtNameAxis.Height / graphDatas.Count;
            }
            #endregion
            #region min / max
            var Minimum = minimum;
            var Maximum = maximum;
            if (graphDatas.Count > 0)
            {
                var rMinimum = graphMode == BarGraphMode.LIST ? graphDatas.Select(x => x.Values.Min(x2 => x2.Value)).Min() : graphDatas.Select(x => x.Values.Sum(x2 => x2.Value)).Min();
                var rMaximum = graphMode == BarGraphMode.LIST ? graphDatas.Select(x => x.Values.Max(x2 => x2.Value)).Max() : graphDatas.Select(x => x.Values.Sum(x2 => x2.Value)).Max();
                Minimum = Math.Min(Minimum, rMinimum);
                Maximum = Math.Max(Maximum, Math.Ceiling(rMaximum / graduation) * graduation);
            }
            #endregion
            #endregion

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Font
                p.Typeface = FontTool.GetFont(fontName, fontStyle);
                p.TextSize = fontSize;
                #endregion
                #region GraphBG
                if (backColor != SKColors.Transparent)
                {
                    p.IsStroke = false;
                    p.Color = backColor;
                    canvas.DrawRect(rtGraph, p);
                }
                #endregion

                if (direction == DvDirectionHV.Horizon)
                {
                    #region Remark
                    if (series.Count > 0)
                    {
                        DrawBox(canvas, rtRemark, gridColor, gridColor, DvRoundType.All, BoxStyle.Border | BoxStyle.OutShadow, Corner);

                        var iy = GP;
                        for (int i = 0; i < series.Count; i++)
                        {
                            var s = series[i];
                            var sz = szRemarks[i];
                            var rt = Util.FromRect(rtRemark.Left + GP, rtRemark.Top + iy, rtRemark.Width, sz.Height + GP);
                            var rticon = Util.FromRect(0, 0, 10, 10);

                            Util.TextIconBounds(s.Alias, fontName, fontSize, fontStyle, 7,
                                rt, rticon, DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleLeft,
                                (rtico, rtText) =>
                                {
                                    var rti = Util.INT(rtico); rti.Offset(0, 1);
                                    DrawBox(canvas, rti, s.SeriesColor, seriasBorderColor, DvRoundType.Rect, BoxStyle.Fill | BoxStyle.Border, Corner);
                                    Util.DrawText(canvas, s.Alias, fontName, fontSize, fontStyle, foreColor, rtText, DvContentAlignment.MiddleLeft);
                                });

                            iy += rt.Height;
                        }
                    }
                    #endregion
                    #region Value Axis
                    if (graduation > 0)
                    {
                        for (var i = Minimum; i <= Maximum; i += graduation)
                        {
                            var s = string.IsNullOrWhiteSpace(formatString) ? i.ToString("0") : i.ToString(formatString);
                            var x = Convert.ToSingle(MathTool.Map(i, Minimum, Maximum, rtGraph.Left, rtGraph.Right));
                            var y = rtValueAxis.Top + (rtValueAxis.Height / 2);
                            var mrt = new SKRect(); p.MeasureText(s, ref mrt);
                            var sz = mrt.Size;
                            var rt = MathTool.MakeRectangle(new SKPoint(x, y), sz.Width, sz.Height); rt.Inflate(2, 2);
                            Util.DrawText(canvas, s, fontName, fontSize, fontStyle, GridColor, rt, DvContentAlignment.MiddleCenter);

                            p.IsStroke = true;
                            var oo = 0.5f;
                            if (i == Minimum)
                            {
                                p.Color = GridColor; p.StrokeWidth = 1;
                                canvas.DrawLine(x - 1 + oo, rtGraph.Top + oo,
                                                x - 1 + oo, rtGraph.Bottom + oo, p);
                            }
                            else if (i == Maximum)
                            {
                                p.Color = GridColor; p.StrokeWidth = 1;
                                canvas.DrawLine(x - 0 + oo, rtGraph.Top + oo,
                                                x - 0 + oo, rtGraph.Bottom + oo, p);
                            }
                            else
                            {
                                p.Color = GridColor; p.StrokeWidth = 1;
                                using (var pe = SKPathEffect.CreateDash(new float[] { 2, 2 }, 0))
                                {
                                    p.PathEffect = pe;
                                    canvas.DrawLine(x - 0 + oo, rtGraph.Top + oo,
                                                    x - 0 + oo, rtGraph.Bottom + oo, p);
                                    p.PathEffect = null;
                                }
                            }
                        }
                    }
                    #endregion
                    #region Name Axis
                    if (graphDatas.Count > 0)
                    {
                        var sp = canvas.Save();
                        canvas.ClipRect(rtNameAxis);

                        for (int i = 0; i < graphDatas.Count; i++)
                        {
                            var itm = graphDatas[i];
                            var rt = Util.FromRect(rtNameAxis.Left, spos + rtNameAxis.Top + (DataWH * i), rtNameAxis.Width, DataWH);
                            if (CollisionTool.Check(rt, rtNameAxis))
                                Util.DrawText(canvas, itm.Name, fontName, fontSize, fontStyle, GridColor, rt, DvContentAlignment.MiddleCenter);
                        }

                        canvas.RestoreToCount(sp);
                    }
                    #endregion
                    #region Data
                    if (series.Count > 0 && graphDatas.Count > 0)
                    {
                        var sp = canvas.Save();
                        canvas.ClipRect(rtGraph);

                        var dicSer = series.ToDictionary(x => x.Name);
                        if (graphMode == BarGraphMode.LIST)
                        {
                            #region List
                            for (int i = 0; i < graphDatas.Count; i++)
                            {
                                var itm = graphDatas[i];
                                var rt = Util.FromRect(rtGraph.Left, spos + rtNameAxis.Top + (DataWH * i), rtGraph.Width, (DataWH));
                                rt.Inflate(0, -barGap);

                                if (CollisionTool.Check(rt, rtGraph))
                                {
                                    var ih = Math.Min(barSize, (rt.Height / series.Count));
                                    var ic = 0;
                                    var tgp = !scrollable ? ((rt.Height) - (ih * series.Count)) / 2 : 0;

                                    foreach (var vk in itm.Values.Keys)
                                    {
                                        if (dicSer.ContainsKey(vk))
                                        {
                                            var n = itm.Values[vk];
                                            var w = Convert.ToSingle(MathTool.Map(n, Minimum, Maximum, 0, rtGraph.Width));
                                            var rtv = Util.FromRect(rtGraph.Left, tgp + rt.Top + (ic * ih), (w), ih);
                                            var ser = dicSer[vk];

                                            var rtv2 = Util.INT(rtv);
                                            DrawBox(canvas, rtv2, ser.SeriesColor, barBorderColor, DvRoundType.Rect, BoxStyle.Border | BoxStyle.InBevelLT | (gradient ? BoxStyle.GradientV : BoxStyle.Fill), Corner);
                                            if (valueDraw)
                                            {
                                                if (n > 0)
                                                {
                                                    var txt = string.IsNullOrWhiteSpace(formatString) ? n.ToString() : n.ToString(formatString);
                                                    rtv2.Inflate(-TGP, 0);
                                                    Util.DrawText(canvas, txt, fontName, fontSize, fontStyle, foreColor, rtv2, DvContentAlignment.MiddleRight);
                                                }
                                            }
                                        }
                                        ic++;
                                    }
                                }
                            }
                            #endregion
                        }
                        else if (graphMode == BarGraphMode.STACK)
                        {
                            #region Stack
                            for (int i = 0; i < graphDatas.Count; i++)
                            {
                                var itm = graphDatas[i];
                                var rt = Util.FromRect(rtGraph.Left, spos + rtNameAxis.Top + (DataWH * i), rtGraph.Width, (DataWH));
                                rt.Inflate(0, -barGap);

                                if (CollisionTool.Check(rt, rtGraph))
                                {
                                    var BarSize = !scrollable ? Math.Min(barSize, rt.Height) : barSize;
                                    var ix = rt.Left;
                                    foreach (var vk in itm.Values.Keys)
                                    {
                                        if (dicSer.ContainsKey(vk))
                                        {
                                            var n = itm.Values[vk];
                                            var w = Convert.ToSingle(MathTool.Map(n, Minimum, Maximum, 0, rtGraph.Width));
                                            var rtv = !scrollable ? Util.FromRect(ix, rt.Top + (rt.Height / 2F) - (BarSize / 2F), w, BarSize) : Util.FromRect(ix, rt.Top, w, rt.Height);
                                            var ser = dicSer[vk];

                                            var rtv2 = Util.INT(rtv);
                                            DrawBox(canvas, rtv2, ser.SeriesColor, barBorderColor, DvRoundType.Rect, BoxStyle.Border | BoxStyle.InBevelLT | (gradient ? BoxStyle.GradientV : BoxStyle.Fill), Corner);
                                            if (valueDraw)
                                            {
                                                if (n > 0)
                                                {
                                                    var txt = string.IsNullOrWhiteSpace(formatString) ? n.ToString() : n.ToString(formatString);
                                                    rtv2.Inflate(-TGP, 0);
                                                    Util.DrawText(canvas, txt, fontName, fontSize, fontStyle, foreColor, rtv2, DvContentAlignment.MiddleRight);
                                                }
                                            }
                                            ix = rtv.Right;
                                        }
                                    }
                                }
                            }
                            #endregion
                        }

                        canvas.RestoreToCount(sp);
                    }
                    #endregion
                }
                else if (direction == DvDirectionHV.Vertical)
                {
                    #region Remark
                    if (series.Count > 0)
                    {
                        DrawBox(canvas, rtRemark, gridColor, gridColor, DvRoundType.All, BoxStyle.Border | BoxStyle.OutShadow, Corner);

                        var ix = GP;
                        for (int i = 0; i < series.Count; i++)
                        {
                            var s = series[i];
                            var sz = szRemarks[i];
                            var rt = Util.FromRect(rtRemark.Left + ix, rtRemark.Top, 10 + 7 + sz.Width + GP, rtRemark.Height);
                            var rticon = Util.FromRect(0, 0, 10, 10);

                            Util.TextIconBounds(s.Alias, fontName, fontSize, fontStyle, 7,
                                rt, rticon, DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleLeft,
                                (rtico, rtText) =>
                                {
                                    var rti = Util.INT(rtico); rti.Offset(0, 1);
                                    DrawBox(canvas, rti, s.SeriesColor, seriasBorderColor, DvRoundType.Rect, BoxStyle.Fill | BoxStyle.Border, Corner);
                                    Util.DrawText(canvas, s.Alias, fontName, fontSize, fontStyle, foreColor, rtText, DvContentAlignment.MiddleLeft);
                                });

                            ix += rt.Width;
                        }
                    }
                    #endregion
                    #region Value Axis
                    if (graduation > 0)
                    {
                        for (var i = Minimum; i <= Maximum; i += graduation)
                        {
                            var s = string.IsNullOrWhiteSpace(formatString) ? i.ToString("0") : i.ToString(formatString);
                            var y = Convert.ToInt32(MathTool.Map(i, Minimum, Maximum, rtGraph.Bottom, rtGraph.Top));
                            var mrt = new SKRect(); p.MeasureText(s, ref mrt);
                            var sz = mrt.Size;
                            var rt = MathTool.MakeRectangle(new SKPoint(0, y), Convert.ToInt32(10), Convert.ToInt32(sz.Height));
                            rt.Left = rtValueAxis.Left; rt.Right = rtValueAxis.Right;
                            Util.DrawText(canvas, s, fontName, fontSize, fontStyle, GridColor, rt, DvContentAlignment.MiddleRight);

                            p.IsStroke = true;
                            var oo = 0.5f;
                            if (i == Minimum)
                            {
                                p.Color = GridColor; p.StrokeWidth = 1;
                                canvas.DrawLine(rtGraph.Left + oo, y + 1 + oo,
                                                rtGraph.Right + oo, y + 1 + oo, p);
                            }
                            else if (i == Maximum)
                            {
                                p.Color = GridColor; p.StrokeWidth = 1;
                                canvas.DrawLine(rtGraph.Left + oo, y + oo,
                                                rtGraph.Right + oo, y + oo, p);
                            }
                            else
                            {
                                p.Color = GridColor; p.StrokeWidth = 1;
                                using (var pe = SKPathEffect.CreateDash(new float[] { 2, 2 }, 0))
                                {
                                    p.PathEffect = pe;
                                    canvas.DrawLine(rtGraph.Left + oo, y + oo,
                                                    rtGraph.Right + oo, y + oo, p);
                                    p.PathEffect = null;
                                }
                            }
                        }
                    }
                    #endregion
                    #region Name Axis
                    if (graphDatas.Count > 0)
                    {
                        var sp = canvas.Save();
                        canvas.ClipRect(rtNameAxis);

                        p.Color = GridColor; p.StrokeWidth = 1;

                        for (int i = 0; i < graphDatas.Count; i++)
                        {
                            var itm = graphDatas[i];
                            var rt = Util.FromRect(spos + rtNameAxis.Left + (DataWH * i), rtNameAxis.Top, (DataWH), rtNameAxis.Height);

                            if (CollisionTool.Check(rt, rtNameAxis))
                                Util.DrawText(canvas, itm.Name, fontName, fontSize, fontStyle, GridColor, rt, DvContentAlignment.MiddleCenter);
                        }

                        canvas.RestoreToCount(sp);
                    }
                    #endregion
                    #region Data
                    if (series.Count > 0 && graphDatas.Count > 0)
                    {
                        var sp = canvas.Save();
                        canvas.ClipRect(new SKRect(rtGraph.Left, rtGraph.Top, rtGraph.Right, rtGraph.Bottom + 1));

                        var dicSer = series.ToDictionary(x => x.Name);
                        if (graphMode == BarGraphMode.LIST)
                        {
                            #region List
                            for (int i = 0; i < graphDatas.Count; i++)
                            {
                                var itm = graphDatas[i];
                                var rt = Util.FromRect(spos + rtGraph.Left + (DataWH * i), rtGraph.Top, (DataWH), rtGraph.Height);
                                rt.Inflate(-barGap, 0);

                                if (CollisionTool.Check(rt, rtGraph))
                                {
                                    var iw = !scrollable ? Math.Min(barSize, rt.Width / series.Count) : (rt.Width / series.Count);
                                    var ic = 0;

                                    var lgp = !scrollable ? (rt.Width - (iw * series.Count)) / 2F : 0;
                                    foreach (var vk in itm.Values.Keys)
                                    {
                                        if (dicSer.ContainsKey(vk))
                                        {
                                            var n = itm.Values[vk];
                                            var h = Convert.ToSingle(MathTool.Map(n, Minimum, Maximum, 0, rtGraph.Height));
                                            var rtv = Util.FromRect(lgp + rt.Left + (ic * iw), rtGraph.Bottom - (h), iw, (h + 1));
                                            var ser = dicSer[vk];

                                            var rtv2 = Util.INT(rtv);
                                            DrawBox(canvas, rtv2, ser.SeriesColor, barBorderColor, DvRoundType.Rect, BoxStyle.Border | BoxStyle.InBevelLT | (gradient ? BoxStyle.GradientH : BoxStyle.Fill), Corner);

                                            if (valueDraw)
                                            {
                                                p.IsStroke = false;
                                                var txt = string.IsNullOrWhiteSpace(formatString) ? n.ToString() : n.ToString(formatString);
                                                rtv2.Inflate(0, -TGP);
                                                Util.DrawText(canvas, txt, fontName, fontSize, fontStyle, foreColor, rtv2, DvContentAlignment.TopCenter);
                                            }
                                        }
                                        ic++;
                                    }
                                }
                            }
                            #endregion
                        }
                        else if (graphMode == BarGraphMode.STACK)
                        {
                            #region Stack
                            for (int i = 0; i < graphDatas.Count; i++)
                            {
                                var itm = graphDatas[i];
                                var rt = Util.FromRect(spos + rtGraph.Left + (DataWH * i), rtGraph.Top, DataWH, rtGraph.Height);
                                rt.Inflate(-barGap, 0);
                                var iy = rt.Bottom;
                                var BarSize = !scrollable ? Math.Min(barSize, rt.Width) : barSize;
                                if (CollisionTool.Check(rt, rtGraph))
                                {
                                    foreach (var vk in itm.Values.Keys)
                                    {
                                        if (dicSer.ContainsKey(vk))
                                        {
                                            var n = itm.Values[vk];
                                            var h = Convert.ToSingle(MathTool.Map(n, Minimum, Maximum, 0, rtGraph.Height));
                                            var rtv = !scrollable ? Util.FromRect(rt.Left + (rt.Width / 2) - (BarSize / 2), iy - (h), BarSize, (h + 1)) : Util.FromRect(rt.Left, iy - (h), rt.Width, (h + 1));
                                            var ser = dicSer[vk];

                                            var rtv2 = Util.INT(rtv);
                                            DrawBox(canvas, rtv2, ser.SeriesColor, barBorderColor, DvRoundType.Rect, BoxStyle.Border | BoxStyle.InBevelLT | (gradient ? BoxStyle.GradientH : BoxStyle.Fill), Corner);

                                            if (valueDraw)
                                            {
                                                if (n > 0)
                                                {
                                                    p.IsStroke = false;
                                                    var txt = string.IsNullOrWhiteSpace(formatString) ? n.ToString() : n.ToString(formatString);
                                                    rtv2.Inflate(0, -TGP);
                                                    Util.DrawText(canvas, txt, fontName, fontSize, fontStyle, foreColor, rtv2, DvContentAlignment.TopCenter);
                                                }
                                            }
                                            iy = rtv.Top;
                                        }
                                    }
                                }
                            }
                            #endregion
                        }

                        canvas.RestoreToCount(sp);
                    }
                    #endregion
                }
            }
        }
        #endregion
        #region DrawLineGraph
        public override void DrawLineGraph(SKCanvas canvas,
            SKRect rtContent, SKRect rtRemark, SKRect rtNameAxis, SKRect rtValueAxis, SKRect rtGraph, SKRect rtScroll, List<SKSize> szRemarks,
            SKColor gridColor, SKColor graphBackColor, SKColor foreColor, SKColor backColor,
            string fontName, float fontSize, DvFontStyle fontStyle,
            bool valueDraw, string formatString,
            bool pointDraw, int dataWH,
            List<GraphSeries> series, double graduation, double minimum, double maximum,
            Scroll scroll, bool scrollable, bool touchMode,
            List<GraphValue> graphDatas)
        {
            #region var
            var scrollBorderColor = GetBorderColor(ScrollBarColor, backColor);
            var seriasBorderColor = GetBorderColor(backColor, backColor);
            var barBorderColor = GetBorderColor(graphBackColor, backColor);
            var TGP = 7;
            #region spos
            var spos = 0;
            var startidx = 0;
            var endidx = graphDatas.Count;
            float DataWH = dataWH;
            if (scrollable)
            {
                spos = Convert.ToInt32(scroll.ScrollPositionWithOffset);

                var sc = scroll.ScrollPosition;
                var si = Convert.ToInt32(Math.Floor((double)(sc - scroll.TouchOffset) / (double)DataWH));
                var cnt = Convert.ToInt32(Math.Ceiling((double)(rtGraph.Width - Math.Min(0, scroll.TouchOffset)) / (double)DataWH));
                var ei = si + cnt;

                startidx = Math.Max(0, si - 1);
                endidx = ei + 1;
            }
            else
            {
                DataWH = rtNameAxis.Height / graphDatas.Count;
            }
            #endregion
            #region min / max
            var Minimum = minimum;
            var Maximum = maximum;
            #endregion
            #endregion

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Font
                p.Typeface = FontTool.GetFont(fontName, fontStyle);
                p.TextSize = fontSize;
                #endregion
                #region GraphBG
                if (backColor != SKColors.Transparent)
                {
                    p.IsStroke = false;
                    p.Color = backColor;
                    canvas.DrawRect(rtGraph, p);
                }
                #endregion

                #region Remark
                if (series.Count > 0)
                {
                    DrawBox(canvas, rtRemark, gridColor, gridColor, DvRoundType.All, BoxStyle.Border | BoxStyle.OutShadow, Corner);

                    var ix = GP;
                    for (int i = 0; i < series.Count; i++)
                    {
                        var s = series[i];
                        var sz = szRemarks[i];
                        var rt = Util.FromRect(rtRemark.Left + ix, rtRemark.Top, 10 + 7 + sz.Width + GP, rtRemark.Height);
                        var rticon = Util.FromRect(0, 0, 10, 10);

                        Util.TextIconBounds(s.Alias, fontName, fontSize, fontStyle, 7,
                            rt, rticon, DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleLeft,
                            (rtico, rtText) =>
                            {
                                var rti = Util.INT(rtico); rti.Offset(0, 1);
                                DrawBox(canvas, rti, s.SeriesColor, seriasBorderColor, DvRoundType.Rect, BoxStyle.Fill | BoxStyle.Border, Corner);
                                Util.DrawText(canvas, s.Alias, fontName, fontSize, fontStyle, foreColor, rtText, DvContentAlignment.MiddleLeft);
                            });

                        ix += rt.Width;
                    }
                }
                #endregion
                #region Value Axis
                if (graduation > 0)
                {
                    for (var i = Minimum; i <= Maximum; i += graduation)
                    {
                        var s = string.IsNullOrWhiteSpace(formatString) ? i.ToString("0") : i.ToString(formatString);
                        var y = Convert.ToInt32(MathTool.Map(i, Minimum, Maximum, rtGraph.Bottom, rtGraph.Top));
                        var mrt = new SKRect(); p.MeasureText(s, ref mrt);
                        var sz = mrt.Size;
                        var rt = MathTool.MakeRectangle(new SKPoint(0, y), Convert.ToInt32(10), Convert.ToInt32(sz.Height));
                        rt.Left = rtValueAxis.Left; rt.Right = rtValueAxis.Right;
                        Util.DrawText(canvas, s, fontName, fontSize, fontStyle, GridColor, rt, DvContentAlignment.MiddleRight);

                        p.IsStroke = true;
                        var oo = 0.5f;
                        if (i == Minimum)
                        {
                            p.Color = GridColor; p.StrokeWidth = 1;
                            canvas.DrawLine(rtGraph.Left + oo, y + 1 + oo,
                                            rtGraph.Right + oo, y + 1 + oo, p);
                        }
                        else if (i == Maximum)
                        {
                            p.Color = GridColor; p.StrokeWidth = 1;
                            canvas.DrawLine(rtGraph.Left + oo, y + oo,
                                            rtGraph.Right + oo, y + oo, p);
                        }
                        else
                        {
                            p.Color = GridColor; p.StrokeWidth = 1;
                            using (var pe = SKPathEffect.CreateDash(new float[] { 2, 2 }, 0))
                            {
                                p.PathEffect = pe;
                                canvas.DrawLine(rtGraph.Left + oo, y + oo,
                                                rtGraph.Right + oo, y + oo, p);
                                p.PathEffect = null;
                            }
                        }
                    }
                }
                #endregion
                #region Name Axis
                if (graphDatas.Count > 0)
                {
                    var sp = canvas.Save();
                    canvas.ClipRect(rtNameAxis);

                    p.Color = GridColor; p.StrokeWidth = 1;

                    for (int i = 0; i < graphDatas.Count; i++)
                    {
                        var itm = graphDatas[i];
                        var rt = Util.FromRect(spos + rtNameAxis.Left + (DataWH * i), rtNameAxis.Top, (DataWH), rtNameAxis.Height);

                        if (CollisionTool.Check(rt, rtNameAxis))
                            Util.DrawText(canvas, itm.Name, fontName, fontSize, fontStyle, GridColor, rt, DvContentAlignment.MiddleCenter);
                    }

                    canvas.RestoreToCount(sp);
                }
                #endregion
                #region Data
                if (series.Count > 0 && graphDatas.Count > 0)
                {
                    using (var fOut = SKImageFilter.CreateDropShadow(2, 2, 1, 1, Util.FromArgb(90, SKColors.Black)))
                    {

                        #region set
                        var dicSer = series.ToDictionary(x => x.Name);
                        var sp = canvas.Save();

                        canvas.ClipRect(rtGraph);
                        #endregion

                        foreach (var vk in dicSer.Keys)
                        {
                            p.ImageFilter = fOut;
                            #region Lines
                            var ser = dicSer[vk];
                            var ls = new List<LGV>();

                            for (int i = startidx; i < endidx && i < graphDatas.Count; i++)
                            {
                                var itm = graphDatas[i];
                                var x = spos + rtGraph.Left + (DataWH * i) + (DataWH / 2F);
                                var y = Convert.ToInt32(MathTool.Map(itm.Values[vk], Minimum, Maximum, rtGraph.Bottom, rtGraph.Top));
                                ls.Add(new LGV() { Position = new SKPoint(x, y), Value = itm.Values[vk] });
                            }

                            p.StrokeWidth = 2F;
                            p.Color = ser.SeriesColor;
                            p.IsStroke = false;
                            if (ls.Count >= 2)
                            {
                                var pts = ls.Select(x => x.Position).ToArray();
                                canvas.DrawPoints(SKPointMode.Polygon, pts, p);
                            }
                            #endregion
                            p.ImageFilter = null;

                            if (pointDraw || valueDraw)
                            {
                                foreach (var v in ls)
                                {
                                    #region var
                                    var s = string.IsNullOrWhiteSpace(formatString) ? v.Value.ToString() : v.Value.ToString(formatString);
                                    var mrt = new SKRect();
                                    p.MeasureText(s, ref mrt);
                                    var sz = mrt.Size;
                                    var rt = MathTool.MakeRectangle(v.Position, 7);
                                    var rtIN = Util.FromRect(rt.Left, rt.Top, rt.Width, rt.Height); rtIN.Inflate(-2, -2);
                                    var rtTxt = MathTool.MakeRectangle(v.Position, Convert.ToInt32(sz.Width) + 2, Convert.ToInt32(sz.Height) + 2);
                                    rtTxt.Offset(0, -(fontSize + 2));
                                    #endregion

                                    p.ImageFilter = fOut;
                                    #region Point
                                    if (pointDraw)
                                    {
                                        p.IsStroke = false;
                                        p.Color = ser.SeriesColor.BrightnessTransmit(0.5F);
                                        canvas.DrawCircle(MathTool.CenterPoint(rt), rt.Width / 2F, p);

                                        p.IsStroke = true;
                                        p.StrokeWidth = 1;
                                        p.Color = GetBorderColor(ser.SeriesColor, backColor);
                                        canvas.DrawCircle(MathTool.CenterPoint(rt), rt.Width / 2F, p);
                                    }
                                    #endregion
                                    p.ImageFilter = null;

                                    #region Value
                                    if (valueDraw)
                                    {
                                        var tc = ser.SeriesColor.BrightnessTransmit(0.5F);
                                        Util.DrawText(canvas, s, fontName, fontSize, fontStyle, tc, rtTxt, DvContentAlignment.MiddleCenter);
                                    }
                                    #endregion

                                }
                            }

                        }

                        canvas.RestoreToCount(sp);

                    }
                }
                #endregion

            }
        }
        #endregion
        #region DrawCircleGraph
        public override void DrawCircleGraph(SKCanvas canvas,
            SKRect rtContent, SKRect rtGraph, SKRect rtSelectLabel, SKRect rtSelectLeft, SKRect rtSelectRight,
            SKColor foreColor, SKColor backColor,
            List<GraphSeries> series, bool gradient,
            string fontName, float fontSize, float nameFontSize, float valueFontSize, DvFontStyle fontStyle,
            List<GraphValue> graphDatas, string formatString,
            bool bLeftSel, bool bRightSel, SKPoint mpt, int nSelectedIndex, DateTime prev)
        {
            var borderColor = GetBorderColor(foreColor, backColor);

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Font
                p.Typeface = FontTool.GetFont(fontName, fontStyle);
                p.TextSize = fontSize;
                #endregion

                using (var pf = SKImageFilter.CreateDropShadow(2, 2, 1, 1, Util.FromArgb(90, SKColors.Black)))
                {
                    if (series.Count > 0)
                    {
                        #region Selector
                        if (series.Count > 1)
                        {
                            if (bLeftSel) rtSelectLeft.Offset(0, 1);
                            if (bRightSel) rtSelectRight.Offset(0, 1);

                            var isz = fontSize * 1.5F;
                            DrawText(canvas, rtSelectLeft, "fa-chevron-left", isz, foreColor, DvContentAlignment.MiddleCenter, bLeftSel);
                            DrawText(canvas, rtSelectRight, "fa-chevron-right", isz, foreColor, DvContentAlignment.MiddleCenter, bRightSel);
                        }
                        if (nSelectedIndex >= 0 && nSelectedIndex < series.Count)
                            DrawText(canvas, rtSelectLabel, fontName, fontSize, fontStyle, series[nSelectedIndex].Alias, foreColor);
                        #endregion

                        #region Graph
                        if (nSelectedIndex >= 0)
                        {
                            if (graphDatas.Count > 0)
                            {
                                #region var
                                var ls = graphDatas.Select(x => new CGV() { Name = x.Name, Value = x.Values[series[nSelectedIndex].Name], Color = x.Color });
                                var cp = MathTool.CenterPoint(rtGraph);
                                var sum = ls.Sum(x => x.Value);
                                #endregion

                                var startAngle = 315F;
                                foreach (var v in ls)
                                {
                                    #region bounds
                                    var rtGO = Util.FromRect(rtGraph.Left, rtGraph.Top, rtGraph.Width, rtGraph.Height);
                                    var rtGI = Util.FromRect(rtGraph.Left, rtGraph.Top, rtGraph.Width, rtGraph.Height);
                                    rtGI.Inflate(-(rtGI.Width / 4F), -(rtGI.Height / 4F));
                                    #endregion

                                    if (v.Value > 0)
                                    {
                                        #region Graph
                                        var sp0 = canvas.Save();

                                        #region var
                                        var sweepAngle = Convert.ToSingle(MathTool.Map(v.Value, 0, sum, 0, 360));
                                        var dist = rtGO.Width / 2F + GP;
                                        var mcp = MathTool.GetPointWithAngle(cp, Convert.ToSingle(MathTool.Map(5, 0, 10, startAngle, startAngle + sweepAngle)), dist);
                                        var ang = MathTool.GetAngle(cp, mpt);
                                        var bSel = CollisionTool.CheckCircle(rtGO, mpt) && !CollisionTool.CheckCircle(rtGI, mpt) && MathTool.CompareAngle(ang, startAngle, startAngle + sweepAngle);
                                        #endregion
                                        #region offset
                                        if (bSel)
                                        {
                                            var nc = 250F;
                                            var vg = nc - Math.Abs((DateTime.Now - prev).TotalMilliseconds % (nc * 2F + 1F) - nc);
                                            var offset = Convert.ToSingle(MathTool.Map(vg, 0, nc, 0, GP));
                                            var ptOff = MathTool.GetPointWithAngle(cp, (startAngle + sweepAngle / 2.0F), offset);

                                            canvas.Translate((ptOff.X - cp.X), (ptOff.Y - cp.Y));
                                        }
                                        #endregion

                                        using (var pth = PathTool.CircleGraphItem(rtGO, rtGI, startAngle, sweepAngle))
                                        {
                                            #region Fill
                                            {
                                                //p.ImageFilter = pf;

                                                p.Color = v.Color;
                                                p.IsStroke = false;
                                                canvas.DrawPath(pth, p);

                                                //p.ImageFilter = null;
                                            }
                                            #endregion
                                            #region Gradient
                                            if (gradient)
                                            {
                                                var giw = rtGI.Width / 2F;
                                                var grw = rtGO.Width / 2F;
                                                var va1 = giw / grw;
                                                var vam = ((grw - giw) / 12F) / giw;
                                                var cs = new SKColor[] { Util.FromArgb(90, SKColors.Black), Util.FromArgb(20, SKColors.Black), Util.FromArgb(50, SKColors.White), Util.FromArgb(0, SKColors.White), Util.FromArgb(30, SKColors.Black) };
                                                var ps = new float[] { 0F + va1, 0F + va1 + vam, 0.01F + va1 + vam, 0.05F + va1 + vam, 1F };

                                                using (var lg = SKShader.CreateRadialGradient(cp, rtGO.Width / 2F, cs, ps, SKShaderTileMode.Clamp))
                                                {
                                                    p.Shader = lg;

                                                    canvas.DrawPath(pth, p);

                                                    p.Shader = null;
                                                }
                                            }
                                            #endregion
                                            #region Bevel
                                            {
                                                var sp = canvas.Save();
                                                {
                                                    using (var f = SKImageFilter.CreateBlur(2, 2))
                                                    {
                                                        p.ImageFilter = f;
                                                        p.IsStroke = true;
                                                        p.StrokeWidth = 2F;
                                                        p.Color = Util.FromArgb(GetBevelAlpha(v.Color), SKColors.White);

                                                        canvas.ClipPath(pth);
                                                        canvas.Translate(1, 1);
                                                        canvas.DrawPath(pth, p);
                                                        p.ImageFilter = null;
                                                    }
                                                }
                                                canvas.RestoreToCount(sp);
                                            }
                                            #endregion
                                            #region Border
                                            {
                                                p.Color = v.Color.BrightnessTransmit(BorderBrightness * 1.5F);
                                                p.IsStroke = true;
                                                p.StrokeWidth = 1F;
                                                canvas.DrawPath(pth, p);
                                            }
                                            #endregion
                                            #region Text
                                            {
                                                var ang2 = MathTool.GetAngle(cp, mcp) + 90;
                                                if (ang2 > 90 && ang2 < 270) ang2 -= 180;

                                                var sp = canvas.Save();
                                                {
                                                    canvas.Translate(mcp);
                                                    canvas.RotateDegrees(Convert.ToSingle(ang2));

                                                    var cT = v.Color.BrightnessTransmit(0.75F);
                                                    var rtv = new SKRect(); p.MeasureText(v.Name, ref rtv);
                                                    var rtt = MathTool.MakeRectangle(new SKPoint(0, 0), rtv.Width, rtv.Height);
                                                    DrawText(canvas, rtt, fontName, fontSize, fontStyle, v.Name, cT);
                                                }
                                                canvas.RestoreToCount(sp);
                                            }
                                            #endregion
                                        }

                                        startAngle += sweepAngle;

                                        canvas.RestoreToCount(sp0);
                                        #endregion

                                        #region Select
                                        if (bSel)
                                        {
                                            var c = v.Color.BrightnessTransmit(0.75F);

                                            var lsr = new List<SizeInfo>();
                                            lsr.Add(new SizeInfo(DvSizeMode.Pixel, nameFontSize + GP));
                                            lsr.Add(new SizeInfo(DvSizeMode.Pixel, valueFontSize + GP));
                                            lsr.Add(new SizeInfo(DvSizeMode.Pixel, fontSize + GP));

                                            var rt = Util.MakeRectangleAlign(rtGI, new SKSize(rtGI.Width, lsr.Sum(x => x.Size)), DvContentAlignment.MiddleCenter);
                                            var rts2 = Util.DevideSizeV(rt, lsr);
                                            DrawText(canvas, rts2[0], fontName, nameFontSize, fontStyle, v.Name, c);
                                            DrawText(canvas, rts2[1], fontName, valueFontSize, fontStyle, !string.IsNullOrWhiteSpace(formatString) ? v.Value.ToString(formatString) : v.Value.ToString(), c);
                                            DrawText(canvas, rts2[2], fontName, fontSize, fontStyle, (v.Value / sum).ToString("0.0%"), c);
                                        }
                                        #endregion
                                    }
                                }
                            }
                            else
                            {
                                #region NO DATA
                                var c = foreColor.BrightnessTransmit(DownBrightness);

                                p.IsStroke = true;
                                p.StrokeWidth = 3;
                                p.Color = c;

                                canvas.DrawCircle(MathTool.CenterPoint(rtGraph), rtGraph.Width / 2F, p);
                                DrawText(canvas, rtGraph, fontName, fontSize, fontStyle, "NO DATA", c);
                                #endregion
                            }
                        }
                        else
                        {
                            #region Not Selected
                            var c = foreColor.BrightnessTransmit(DownBrightness);

                            p.IsStroke = true;
                            p.StrokeWidth = 3;
                            p.Color = c;

                            canvas.DrawCircle(MathTool.CenterPoint(rtGraph), rtGraph.Width / 2F, p);
                            DrawText(canvas, rtGraph, fontName, fontSize, fontStyle, "NOT SELECTED", c);
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        #region Selector
                        if (series.Count > 1)
                        {
                            var c = foreColor.BrightnessTransmit(DownBrightness);

                            var isz = fontSize * 1.5F;
                            DrawText(canvas, rtSelectLeft, "fa-chevron-left", isz, c);
                            DrawText(canvas, rtSelectRight, "fa-chevron-right", isz, c);
                        }
                        #endregion

                        #region Empty
                        {
                            var c = foreColor.BrightnessTransmit(DownBrightness);

                            p.IsStroke = true;
                            p.StrokeWidth = 3;
                            p.Color = c;

                            canvas.DrawCircle(MathTool.CenterPoint(rtGraph), rtGraph.Width / 2F, p);
                            DrawText(canvas, rtGraph, fontName, fontSize, fontStyle, "EMPTY", c);
                        }
                        #endregion
                    }
                }
            }
        }
        #endregion
        #region DrawTimeGraph
        public override void DrawTimeGraph(SKCanvas canvas,
            SKRect rtContent, SKRect rtRemark, SKRect rtTimeAxis, SKRect rtValueTitle, SKRect rtValueAxis, SKRect rtGraph, SKRect rtScroll, List<SKSize> szRemarks,
            SKColor gridColor, SKColor graphBackColor, SKColor foreColor, SKColor backColor,
            string fontName, float fontSize, DvFontStyle fontStyle,
            TimeSpan xAxisGraduation, int yAxisGraduationCount,
            bool xAxisGridDraw, bool yAxisGridDraw, string valueFormatString, string timeFormatString,
            TimeSpan xScale, List<GraphSeries2> series,
            Scroll scroll, bool scrollable, bool touchMode,
            List<TimeGraphValue> graphDatas)
        {
            #region var
            var scrollBorderColor = GetBorderColor(ScrollBarColor, backColor);
            var seriasBorderColor = GetBorderColor(backColor, backColor);
            var barBorderColor = GetBorderColor(graphBackColor, backColor);

            var spos = scroll.ScrollPositionWithOffset;
            #endregion

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Font
                p.Typeface = FontTool.GetFont(fontName, fontStyle);
                p.TextSize = fontSize;
                #endregion
                #region GraphBG
                if (backColor != SKColors.Transparent)
                {
                    p.IsStroke = false;
                    p.Color = backColor;
                    canvas.DrawRect(rtGraph, p);
                }
                #endregion
                #region Min / Max / Remark
                var dic = new Dictionary<string, _ValueAxisBounds_>();
                foreach (var x in series)
                {
                    var vrt = new _ValueAxisBounds_();

                    var sMin = string.IsNullOrWhiteSpace(valueFormatString) ? x.Minimum.ToString() : x.Minimum.ToString(valueFormatString);
                    var sMax = string.IsNullOrWhiteSpace(valueFormatString) ? x.Maximum.ToString() : x.Maximum.ToString(valueFormatString);
                    var sTxt = x.Alias;

                    p.MeasureText(sMin, ref vrt.rtMin);
                    p.MeasureText(sMax, ref vrt.rtMax);
                    p.MeasureText(sTxt, ref vrt.rtAlias);

                    dic.Add(x.Name, vrt);
                }
                #endregion

                #region Remark
                if (series.Count > 0)
                {
                    DrawBox(canvas, rtRemark, gridColor, gridColor, DvRoundType.All, BoxStyle.Border | BoxStyle.OutShadow, Corner);

                    var ix = GP;
                    for (int i = 0; i < series.Count; i++)
                    {
                        var s = series[i];
                        var sz = szRemarks[i];
                        var rt = Util.FromRect(rtRemark.Left + ix, rtRemark.Top, 10 + 7 + sz.Width + GP, rtRemark.Height);
                        var rticon = Util.FromRect(0, 0, 10, 10);

                        Util.TextIconBounds(s.Alias, fontName, fontSize, fontStyle, 7,
                            rt, rticon, DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleLeft,
                            (rtico, rtText) =>
                            {
                                var rti = Util.INT(rtico); rti.Offset(0, 1);
                                DrawBox(canvas, rti, s.SeriesColor, seriasBorderColor, DvRoundType.Rect, BoxStyle.Fill | BoxStyle.Border, Corner);
                                Util.DrawText(canvas, s.Alias, fontName, fontSize, fontStyle, foreColor, rtText, DvContentAlignment.MiddleLeft);
                            });

                        ix += rt.Width;
                    }
                }
                #endregion
                #region Value Axis
                if (yAxisGraduationCount > 0)
                {
                    for (var i = 0; i <= yAxisGraduationCount; i++)
                    {
                        var y = Convert.ToInt32(MathTool.Map(i, 0, yAxisGraduationCount, rtGraph.Bottom, rtGraph.Top));
                        #region Grid
                        p.IsStroke = true;
                        var oo = 0.5f;
                        if (i == 0)
                        {
                            p.Color = GridColor; p.StrokeWidth = 1;
                            canvas.DrawLine(rtGraph.Left + oo, y + 1 + oo,
                                            rtGraph.Right + oo, y + 1 + oo, p);
                        }
                        else if (i == yAxisGraduationCount)
                        {
                            p.Color = GridColor; p.StrokeWidth = 1;
                            canvas.DrawLine(rtGraph.Left + oo, y + oo,
                                            rtGraph.Right + oo, y + oo, p);
                        }
                        else if (yAxisGridDraw)
                        {
                            using (var pe = SKPathEffect.CreateDash(new float[] { 2, 2 }, 0))
                            {
                                p.PathEffect = pe;

                                p.Color = GridColor; p.StrokeWidth = 1;
                                canvas.DrawLine(rtGraph.Left + oo, y + oo,
                                                rtGraph.Right + oo, y + oo, p);

                                p.PathEffect = null;
                            }
                        }
                        #endregion

                        var vx = 0F;
                        foreach (var ser in series)
                        {
                            if (dic.ContainsKey(ser.Name))
                            {
                                var c = ser.SeriesColor;
                                var v = dic[ser.Name];
                                var vw = Math.Max(Math.Max(v.rtMin.Width, v.rtMax.Width), v.rtAlias.Width);
                                var val = MathTool.Map(i, 0, yAxisGraduationCount, ser.Minimum, ser.Maximum);
                                var sval = val.ToString(valueFormatString ?? "0");
                                var rtsz = new SKRect();
                                var sz = p.MeasureText(sval, ref rtsz);
                                var rt = Util.FromRect(vx, y - ((rtsz.Height + 2F) / 2F), vw, (rtsz.Height) + 2F);
                                Util.DrawText(canvas, sval, fontName, fontSize, fontStyle, c, rt, DvContentAlignment.MiddleRight);

                                if (i == 0)
                                {
                                    var rtTitle = Util.FromRect(rt.Left, rtContent.Top, rt.Width, rt.Height);
                                    Util.DrawText(canvas, ser.Alias, fontName, fontSize, fontStyle, c, rtTitle, DvContentAlignment.MiddleRight);
                                }

                                vx += vw + GP;
                            }
                        }
                    }
                }
                #endregion
                #region Time Axis
                if (graphDatas.Count > 0)
                {

                    p.Color = GridColor; p.StrokeWidth = 1;
                    using (var pe = SKPathEffect.CreateDash(new float[] { 2, 2 }, 0))
                    {
                        p.PathEffect = pe;
                        var st = new DateTime(graphDatas.First().Time.Ticks / xAxisGraduation.Ticks * xAxisGraduation.Ticks);
                        var ed = new DateTime(Convert.ToInt64(Math.Ceiling(Convert.ToDouble(graphDatas.Last().Time.Ticks) / Convert.ToDouble(xAxisGraduation.Ticks))) * xAxisGraduation.Ticks);

                        for (DateTime i = st; i <= ed; i += xAxisGraduation)
                        {
                            var x = Convert.ToSingle(MathTool.Map(i.Ticks + spos, st.Ticks, st.Ticks + xScale.Ticks, rtGraph.Left, rtGraph.Right));

                            var sval = i.ToString(timeFormatString ?? "yyyy.MM.dd\r\nHH:mm:ss");
                            var rtsz = new SKRect(); p.MeasureText(sval, ref rtsz);
                            var rt = MathTool.MakeRectangle(new SKPoint(x, rtTimeAxis.Top + (rtTimeAxis.Height / 2)), (rtsz.Width) + 2, (rtsz.Height) + 2);

                            if (x >= rtGraph.Left && x <= rtGraph.Right)
                            {
                                if (xAxisGridDraw) canvas.DrawLine(x, rtGraph.Top, x, rtGraph.Bottom, p);

                                Util.DrawText(canvas, sval, fontName, fontSize, fontStyle, GridColor, rt, DvContentAlignment.MiddleCenter);
                            }
                        }
                        p.PathEffect = null;
                    }

                }
                #endregion
                #region Data
                if (series.Count > 0 && graphDatas.Count > 0)
                {
                    using (var fOut = SKImageFilter.CreateDropShadow(2, 2, 1, 1, Util.FromArgb(90, SKColors.Black)))
                    {
                        p.ImageFilter = fOut;

                        var st = new DateTime(graphDatas.First().Time.Ticks / xAxisGraduation.Ticks * xAxisGraduation.Ticks);
                        var ed = new DateTime(Convert.ToInt64(Math.Ceiling(Convert.ToDouble(graphDatas.Last().Time.Ticks) / Convert.ToDouble(xAxisGraduation.Ticks))) * xAxisGraduation.Ticks);

                        var sp = canvas.Save();
                        canvas.ClipRect(rtGraph);
                        foreach (var v in series)
                        {
                            var pts = graphDatas.Select(x => new SKPoint(Convert.ToSingle(MathTool.Map((double)x.Time.Ticks + (double)spos, st.Ticks, st.Ticks + xScale.Ticks, rtGraph.Left, rtGraph.Right)),
                                                                         Convert.ToSingle(MathTool.Map((double)x.Values[v.Name], v.Minimum, v.Maximum, rtGraph.Bottom, rtGraph.Top)))).ToArray();

                            pts = pts.Where(x => x.X >= rtGraph.Left && x.X <= rtGraph.Right).ToArray();
                            p.StrokeWidth = 2F;
                            p.Color = v.SeriesColor;

                            if (pts.Length > 1) canvas.DrawPoints(SKPointMode.Polygon, pts, p);
                        }
                        canvas.RestoreToCount(sp);

                        p.ImageFilter = null;
                    }
                }
                #endregion
            }
        }
        #endregion
        #region DrawTrendGraph
        public override void DrawTrendGraph(SKCanvas canvas,
            SKRect rtContent, SKRect rtRemark, SKRect rtTimeAxis, SKRect rtValueTitle, SKRect rtValueAxis, SKRect rtGraph, SKRect rtScroll, List<SKSize> szRemarks,
            SKColor gridColor, SKColor graphBackColor, SKColor foreColor, SKColor backColor,
            string fontName, float fontSize, DvFontStyle fontStyle,
            TimeSpan xAxisGraduation, int yAxisGraduationCount,
            bool xAxisGridDraw, bool yAxisGridDraw, string valueFormatString, string timeFormatString,
            TimeSpan xScale, List<GraphSeries2> series,
            Scroll scroll, bool scrollable, bool touchMode,
            List<TimeGraphValue> graphDatas, DateTime firstAppendTime)
        {
            #region var
            var scrollBorderColor = GetBorderColor(ScrollBarColor, backColor);
            var seriasBorderColor = GetBorderColor(backColor, backColor);
            var barBorderColor = GetBorderColor(graphBackColor, backColor);

            var spos = scroll.ScrollPositionWithOffsetR;
            #endregion

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Font
                p.Typeface = FontTool.GetFont(fontName, fontStyle);
                p.TextSize = fontSize;
                #endregion
                #region GraphBG
                if (backColor != SKColors.Transparent)
                {
                    p.IsStroke = false;
                    p.Color = backColor;
                    canvas.DrawRect(rtGraph, p);
                }
                #endregion
                #region Min / Max / Remark
                var dic = new Dictionary<string, _ValueAxisBounds_>();
                foreach (var x in series)
                {
                    var vrt = new _ValueAxisBounds_();

                    var sMin = string.IsNullOrWhiteSpace(valueFormatString) ? x.Minimum.ToString() : x.Minimum.ToString(valueFormatString);
                    var sMax = string.IsNullOrWhiteSpace(valueFormatString) ? x.Maximum.ToString() : x.Maximum.ToString(valueFormatString);
                    var sTxt = x.Alias;

                    p.MeasureText(sMin, ref vrt.rtMin);
                    p.MeasureText(sMax, ref vrt.rtMax);
                    p.MeasureText(sTxt, ref vrt.rtAlias);

                    dic.Add(x.Name, vrt);
                }
                #endregion

                #region Remark
                if (series.Count > 0)
                {
                    DrawBox(canvas, rtRemark, gridColor, gridColor, DvRoundType.All, BoxStyle.Border | BoxStyle.OutShadow, Corner);

                    var ix = GP;
                    for (int i = 0; i < series.Count; i++)
                    {
                        var s = series[i];
                        var sz = szRemarks[i];
                        var rt = Util.FromRect(rtRemark.Left + ix, rtRemark.Top, 10 + 7 + sz.Width + GP, rtRemark.Height);
                        var rticon = Util.FromRect(0, 0, 10, 10);

                        Util.TextIconBounds(s.Alias, fontName, fontSize, fontStyle, 7,
                            rt, rticon, DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleLeft,
                            (rtico, rtText) =>
                            {
                                var rti = Util.INT(rtico); rti.Offset(0, 1);
                                DrawBox(canvas, rti, s.SeriesColor, seriasBorderColor, DvRoundType.Rect, BoxStyle.Fill | BoxStyle.Border, Corner);
                                Util.DrawText(canvas, s.Alias, fontName, fontSize, fontStyle, foreColor, rtText, DvContentAlignment.MiddleLeft);
                            });

                        ix += rt.Width;
                    }
                }
                #endregion
                #region Value Axis
                if (yAxisGraduationCount > 0)
                {
                    for (var i = 0; i <= yAxisGraduationCount; i++)
                    {
                        var y = Convert.ToInt32(MathTool.Map(i, 0, yAxisGraduationCount, rtGraph.Bottom, rtGraph.Top));
                        #region Grid
                        p.IsStroke = true;
                        var oo = 0.5f;
                        if (i == 0)
                        {
                            p.Color = GridColor; p.StrokeWidth = 1;
                            canvas.DrawLine(rtGraph.Left + oo, y + 1 + oo,
                                            rtGraph.Right + oo, y + 1 + oo, p);
                        }
                        else if (i == yAxisGraduationCount)
                        {
                            p.Color = GridColor; p.StrokeWidth = 1;
                            canvas.DrawLine(rtGraph.Left + oo, y + oo,
                                            rtGraph.Right + oo, y + oo, p);
                        }
                        else if (yAxisGridDraw)
                        {
                            using (var pe = SKPathEffect.CreateDash(new float[] { 2, 2 }, 0))
                            {
                                p.PathEffect = pe;

                                p.Color = GridColor; p.StrokeWidth = 1;
                                canvas.DrawLine(rtGraph.Left + oo, y + oo,
                                                rtGraph.Right + oo, y + oo, p);

                                p.PathEffect = null;
                            }
                        }
                        #endregion

                        var vx = 0F;
                        foreach (var ser in series)
                        {
                            if (dic.ContainsKey(ser.Name))
                            {
                                var c = ser.SeriesColor;
                                var v = dic[ser.Name];
                                var vw = Math.Max(Math.Max(v.rtMin.Width, v.rtMax.Width), v.rtAlias.Width);
                                var val = MathTool.Map(i, 0, yAxisGraduationCount, ser.Minimum, ser.Maximum);
                                var sval = val.ToString(valueFormatString ?? "0");
                                var rtsz = new SKRect();
                                var sz = p.MeasureText(sval, ref rtsz);
                                var rt = Util.FromRect(vx, y - ((rtsz.Height + 2F) / 2F), vw, (rtsz.Height) + 2F);
                                Util.DrawText(canvas, sval, fontName, fontSize, fontStyle, c, rt, DvContentAlignment.MiddleRight);

                                if (i == 0)
                                {
                                    var rtTitle = Util.FromRect(rt.Left, rtContent.Top, rt.Width, rt.Height);
                                    Util.DrawText(canvas, ser.Alias, fontName, fontSize, fontStyle, c, rtTitle, DvContentAlignment.MiddleRight);
                                }

                                vx += vw + GP;
                            }
                        }
                    }
                }
                #endregion
                #region Time Axis
                if (graphDatas.Count > 0)
                {

                    p.Color = GridColor; p.StrokeWidth = 1;
                    using (var pe = SKPathEffect.CreateDash(new float[] { 2, 2 }, 0))
                    {
                        p.PathEffect = pe;
                        var st = graphDatas.First().Time;
                        var ed = graphDatas.Last().Time;

                        var ots = TimeSpan.FromTicks((ed.Ticks - firstAppendTime.Ticks) % xAxisGraduation.Ticks);
                        var ox = Convert.ToSingle(MathTool.Map(ots.Ticks, 0, xScale.Ticks, 0, rtGraph.Width));

                        for (DateTime i = ed; i >= st; i -= xAxisGraduation)
                        {
                            var vx = Convert.ToSingle(MathTool.Map(i.Ticks + spos, ed.Ticks, ed.Ticks - xScale.Ticks, rtGraph.Right, rtGraph.Left));
                            var x = vx - ox;
                            var vs = i - ots;
                            var sval = vs.ToString(timeFormatString ?? "yyyy.MM.dd\r\nHH:mm:ss");
                            var rtsz = new SKRect(); p.MeasureText(sval, ref rtsz);
                            var rt = MathTool.MakeRectangle(new SKPoint(x, rtTimeAxis.Top + (rtTimeAxis.Height / 2)), (rtsz.Width) + 2, (rtsz.Height) + 2);

                            if (x > rtGraph.Left && x < rtGraph.Right)
                            {
                                if (xAxisGridDraw) canvas.DrawLine(x, rtGraph.Top, x, rtGraph.Bottom, p);

                                Util.DrawText(canvas, sval, fontName, fontSize, fontStyle, GridColor, rt, DvContentAlignment.MiddleCenter);
                            }
                        }
                        p.PathEffect = null;
                    }

                }
                #endregion
                #region Data
                if (series.Count > 0 && graphDatas.Count > 0)
                {
                    using (var fOut = SKImageFilter.CreateDropShadow(2, 2, 1, 1, Util.FromArgb(90, SKColors.Black)))
                    {
                        p.ImageFilter = fOut;

                        var ls = graphDatas.ToList();
                        var st = ls.First().Time;
                        var ed = ls.Last().Time;

                        var sp = canvas.Save();
                        canvas.ClipRect(rtGraph);
                        foreach (var v in series)
                        {
                            var pts = ls.Select(x => new SKPoint(Convert.ToSingle(MathTool.Map((double)x.Time.Ticks + (double)spos, ed.Ticks, ed.Ticks - xScale.Ticks, rtGraph.Right, rtGraph.Left)),
                                                                 Convert.ToSingle(MathTool.Map(x.Values[v.Name], v.Minimum, v.Maximum, rtGraph.Bottom, rtGraph.Top)))).ToArray();

                            pts = pts.Where(x => x.X >= rtGraph.Left - 10 && x.X <= rtGraph.Right + 10).ToArray();
                            p.StrokeWidth = 2F;
                            p.Color = v.SeriesColor;

                            if (pts.Length > 1) canvas.DrawPoints(SKPointMode.Polygon, pts, p);
                        }
                        canvas.RestoreToCount(sp);

                        p.ImageFilter = null;
                    }
                }
                #endregion
            }
        }
        #endregion
        #region DrawListBox
        public override void DrawListBox(SKCanvas canvas,
            SKRect rtContent, SKRect rtBox, SKRect rtScroll,
            SKColor boxColor, SKColor selectedColor, SKColor foreColor, SKColor backColor,
            bool backgroundDraw, DvContentAlignment itemAlignment, DvRoundType round,
            Scroll scroll, bool drawScroll,
            Action<SKRect, Action<SKRect, ListBoxItem>> loop)
        {
            #region var
            var borderColor = GetBorderColor(boxColor, backColor);
            var belectedBorderColor = GetBorderColor(selectedColor, backColor);
            var bcrollBorderColor = GetBorderColor(ScrollBarColor, backColor);

            var spos = Convert.ToSingle(scroll.ScrollPositionWithOffset);
            #endregion

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                var rt = backgroundDraw ? rtBox : rtContent;

                #region Box
                var rndsH = Util.Rounds(DvDirectionHV.Horizon, round, drawScroll ? 2 : 1);

                var rtr = new SKRoundRect(rt, Corner);
                Util.SetRound(rtr, rndsH[0], Corner);

                if (backgroundDraw)
                {
                    p.Color = boxColor;
                    canvas.DrawRoundRect(rtr, p);
                }
                #endregion

                #region Items
                var sp = canvas.Save();

                if (backgroundDraw) canvas.ClipRoundRect(rtr);
                else canvas.ClipRect(rt);

                canvas.Translate(0, Convert.ToSingle(scroll.ScrollPositionWithOffset));

                loop(rt, (rt, v) => v._Draw(canvas, this, rt));

                canvas.RestoreToCount(sp);
                #endregion

                #region Scroll
                if (drawScroll) DrawScroll(canvas, rtScroll, backColor, scroll, backgroundDraw ? rndsH[1] : DvRoundType.All);
                #endregion

                #region Border
                if (backgroundDraw)
                {
                    rtr.Inflate(-0.5F, -0.5F);
                    p.Color = borderColor;
                    p.IsStroke = true;
                    p.StrokeWidth = 1F;
                    canvas.DrawRoundRect(rtr, p);
                }
                #endregion
            }
        }
        #endregion
        #region DrawToolBox
        public override void DrawToolBox(SKCanvas canvas,
            SKRect rtContent, SKRect rtBox, SKRect rtScroll,
            SKColor boxColor, SKColor categoryColor, SKColor foreColor, SKColor backColor,
            bool backgroundDraw, float iconSize, int itemHeight, DvRoundType round,
            string fontName, float fontSize, DvFontStyle fontStyle,
            Scroll scroll,
            Action<SKRect, Action<SKRect, ToolCategoryItem>> loop)
        {
            #region var
            var borderColor = GetBorderColor(boxColor, backColor);
            var categoryBorderColor = GetBorderColor(categoryColor, backColor);
            var scrollBorderColor = GetBorderColor(ScrollBarColor, backColor);

            var Animation = DvDesign.ActiveDesign?.Animation ?? false;
            #endregion

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Box
                var rndsH = Util.Rounds(DvDirectionHV.Horizon, round, 2);

                var rtrBox = new SKRoundRect(rtBox, Corner);
                Util.SetRound(rtrBox, rndsH[0], Corner);

                if (backgroundDraw)
                {
                    p.Color = boxColor;
                    canvas.DrawRoundRect(rtrBox, p);
                }
                #endregion

                #region Items
                var sp = canvas.Save();

                if (backgroundDraw) canvas.ClipRoundRect(rtrBox);
                else canvas.ClipRect(rtBox);
                canvas.Translate(0, Convert.ToSingle(scroll.ScrollPositionWithOffset));

                loop(rtBox, (rtv, v) => v._Draw(canvas, this, rtv));

                canvas.RestoreToCount(sp);
                #endregion

                #region Scroll
                DrawScroll(canvas, rtScroll, BackColor, scroll, backgroundDraw ? rndsH[1] : DvRoundType.All);
                #endregion

                #region Border
                if (backgroundDraw)
                {
                    rtrBox.Inflate(-0.5F, -0.5F);
                    p.Color = borderColor;
                    p.IsStroke = true;
                    p.StrokeWidth = 1F;
                    canvas.DrawRoundRect(rtrBox, p);
                }
                #endregion
            }
        }
        #endregion
        #region DrawToolBoxCategory
        public override void DrawToolBoxCategory(SKCanvas canvas,
            SKRect rtRow, SKRect rtt, SKRect rti,
            SKColor boxColor, SKColor categoryColor, SKColor foreColor, SKColor backColor,
            string text, string fontName, float fontSize, DvFontStyle fontStyle,
            string iconString, float iconSize, float iconGap, DvTextIconAlignment iconAlignment,
            bool expands, float itemHeight,
            bool animation, Animation ani)
        {
            #region var
            var borderColor = GetBorderColor(boxColor, backColor);

            var ic = expands ? "fa-minus" : "fa-plus";
            var ics = iconSize;
            var ica = animation && ani.IsPlaying ?
                     (float)Math.Abs(ani.Value(AnimationAccel.DCL, ics * 2F, 0F) - ics) :
                     ics;

            #endregion
            #region Box
            DrawBox(canvas, rtRow, categoryColor, borderColor, DvRoundType.Rect, BS_TOOL_CATEGORY, Corner);
            #endregion
            #region Text
            var ti = new DvTextIcon
            {
                Text = text,
                FontName = fontName,
                FontSize = fontSize,
                FontStyle = fontStyle,

                IconString = iconString,
                IconSize = iconSize,
                IconGap = iconGap,
                IconAlignment = iconAlignment,
            };

            DrawText(canvas, rtt, ti, foreColor, DvContentAlignment.MiddleLeft);
            DrawText(canvas, rti, ic, ica, foreColor);
            #endregion
        }
        #endregion
        #region DrawToolBoxItem
        public override void DrawToolBoxItem(SKCanvas canvas,
           SKRect rtv, SKRect rti, SKRect rtt,
           SKColor boxColor, SKColor foreColor,
           DvTextIcon v,
           float mx, float my, bool isDrag)
        {


            #region Box
            if (CollisionTool.Check(rtv, mx, my) && !isDrag)
            {
                DrawBox(canvas, rtv,
                  boxColor.BrightnessTransmit(0.5F), boxColor.BrightnessTransmit(-0.5F),
                  DvRoundType.All, BoxStyle.Fill | BoxStyle.Border, Corner);
            }
            #endregion
            #region Text
            var rtc = Util.MakeRectangle(rti, new SKSize(4, 4));
            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                var cp = MathTool.CenterPoint(rti);
                p.IsStroke = false;
                p.Color = foreColor;
                canvas.DrawCircle(cp, 2, p);

                DrawText(canvas, rtt, v, foreColor, DvContentAlignment.MiddleLeft);
            }
            #endregion
        }
        #endregion
        #region DrawTreeView
        public override void DrawTreeView(SKCanvas canvas,
            SKRect rtContent, SKRect rtBox, SKRect rtScroll,
            SKColor boxColor, SKColor selectedColor, SKColor radioColor, SKColor foreColor, SKColor backColor,
            bool backgroundDraw, float iconSize, int itemHeight, DvRoundType round,
            string fontName, float fontSize, DvFontStyle fontStyle,
            Scroll scroll, Action<SKRect, Action<SKRect, TreeViewNode>> loop)
        {
            #region var
            var borderColor = GetBorderColor(boxColor, backColor);
            var scrollBorderColor = GetBorderColor(ScrollBarColor, backColor);
            #endregion

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Box
                var rndsH = Util.Rounds(DvDirectionHV.Horizon, round, 2);

                var rtrBox = new SKRoundRect(rtBox, Corner);
                Util.SetRound(rtrBox, rndsH[0], Corner);

                if (backgroundDraw)
                {
                    p.Color = boxColor;
                    canvas.DrawRoundRect(rtrBox, p);
                }
                #endregion

                #region Items
                var sp = canvas.Save();

                if (backgroundDraw) canvas.ClipRoundRect(rtrBox);
                else canvas.ClipRect(rtBox);
                canvas.Translate(0, Convert.ToSingle(scroll.ScrollPositionWithOffset));

                loop(rtBox, (rtItem, v) => v._Draw(canvas, this, rtItem));

                canvas.RestoreToCount(sp);
                #endregion

                #region Scroll
                DrawScroll(canvas, rtScroll, BackColor, scroll, backgroundDraw ? rndsH[1] : DvRoundType.All);
                #endregion

                #region Border
                if (backgroundDraw)
                {
                    rtrBox.Inflate(-0.5F, -0.5F);
                    p.Color = borderColor;
                    p.IsStroke = true;
                    p.StrokeWidth = 1F;
                    canvas.DrawRoundRect(rtrBox, p);
                }
                #endregion
            }
        }
        #endregion
        #region DrawTreeViewNode
        public override void DrawTreeViewNode(SKCanvas canvas,
            SKRect rtRow, SKRect rti, SKRect rtt,
            SKColor boxColor, SKColor selectedColor, SKColor radioColor, SKColor radioBackColor, SKColor foreColor, SKColor backColor,
            DvTextIcon v, bool sel, TreeViewNodeCollection nodes, bool expands,
            bool animation, Animation ani)
        {
            #region var
            var borderColor = GetBorderColor(radioBackColor, boxColor);
            #endregion

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Selected
                if (sel) DrawBox(canvas, rtRow, selectedColor, selectedColor, DvRoundType.Rect, BoxStyle.Fill, Corner);
                #endregion

                #region Text
                DrawText(canvas, rtt, v, foreColor, DvContentAlignment.MiddleLeft);
                #endregion

                #region Radio
                if (nodes.Count > 0)
                {
                    var rtrb = Util.MakeRectangle(rti, new SKSize(16, 16));
                    DrawBox(canvas, rtrb, radioBackColor, borderColor, DvRoundType.Ellipse, BS_C_BACK, Corner);

                    var ics = 10F;
                    var ica = animation && ani.IsPlaying ?
                            Convert.ToSingle(expands ? ani.Value(AnimationAccel.DCL, ics, 0F) :
                                                       ani.Value(AnimationAccel.DCL, 0F, ics)) : ics;

                    var rtrc = Util.MakeRectangle(rti, new SKSize(ica, ica));
                    if (!expands || (animation && ani.IsPlaying))
                        DrawBox(canvas, rtrc, radioColor, borderColor, DvRoundType.Ellipse, BoxStyle.Fill, Corner);
                }
                else
                {
                    var cp = MathTool.CenterPoint(rti);
                    p.IsStroke = false;
                    p.Color = radioColor;
                    canvas.DrawCircle(cp, 2, p);
                }
                #endregion
            }
        }
        #endregion
        #region DrawComboBox
        public override void DrawComboBox(SKCanvas canvas,
            SKRect rtContent, SKRect rtIco, SKRect rtText,
            SKColor boxColor, SKColor foreColor, SKColor backColor, SKColor selectedColor, SKColor listBackColor,
            string fontName, float fontSize, DvFontStyle fontStyle, DvRoundType rnd,
            float iconSize, DvTextIconAlignment iconAlignment, float iconGap,
            List<ComboBoxItem> items, int selectedIndex,
            bool bOpen, bool reverse, bool backgroundDraw)
        {
            #region var
            var BorderColor = GetBorderColor(boxColor, backColor);

            var round = bOpen ? (reverse ? DvRoundType.B : DvRoundType.T) : rnd;
            #endregion

            #region Box
            if (backgroundDraw)
                DrawBox(canvas, rtContent, boxColor, BorderColor, round, BS_BTN_UP_FILL, Corner);
            #endregion
            #region Text
            if (selectedIndex >= 0 && selectedIndex < items.Count)
            {
                var v = items[selectedIndex];
                Util.DrawTextIcon(canvas, v.IconString, iconSize, iconAlignment, iconGap,
                    v.Text, fontName, fontSize, fontStyle, Padding.Zero,
                    foreColor, foreColor, rtText);
            }
            #endregion
            #region Icon
            var nisz = Convert.ToInt32(rtIco.Height / 3);
            Util.DrawIcon(canvas, bOpen ? "fa-chevron-up" : "fa-chevron-down", nisz, foreColor, rtIco);
            #endregion
            #region Unit Sep
            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                var szh = Convert.ToInt32(rtIco.Height / 2);

                p.StrokeWidth = 1;

                p.Color = boxColor.BrightnessTransmit(-0.25F);
                canvas.DrawLine(rtIco.Left + 0.5F, (rtContent.Top + (rtContent.Height / 2)) - (szh / 2) + 1, rtIco.Left + 0.5F, (rtContent.Top + (rtContent.Height / 2)) + (szh / 2) + 1, p);

                p.Color = boxColor.BrightnessTransmit(0.25F);
                canvas.DrawLine(rtIco.Left + 1.5F, (rtContent.Top + (rtContent.Height / 2)) - (szh / 2) + 1, rtIco.Left + 1.5F, (rtContent.Top + (rtContent.Height / 2)) + (szh / 2) + 1, p);
            }
            #endregion
        }
        #endregion
        #region DrawDataGridColumn
        public override void DrawDataGridColumn(SKCanvas canvas,
            SKRect bounds, SKRect? boundsFilter,
            SKColor foreColor, SKColor backColor, SKColor boxColor, SKColor borderColor, SKColor inputBoxColor, SKColor inputBorderColor,
            string text, string fontName, float fontSize, DvFontStyle fontStyle,
            bool useFilter, string filterText, bool useSort, DvDataGridColumnSortState sortState, bool dash = true)
        {
            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                using (var pe = SKPathEffect.CreateDash(new float[] { 3, 3 }, 0))
                {
                    #region Cell
                    {
                        #region var
                        var oxy = 0F;

                        var rt = bounds;
                        var T = rt.Top + oxy;
                        var B = rt.Bottom - oxy;
                        var L = rt.Left + oxy;
                        var R = rt.Right - oxy;
                        #endregion
                        #region Background
                        p.IsStroke = false;
                        p.Color = boxColor;
                        canvas.DrawRect(rt, p);
                        #endregion
                        #region Border
                        p.IsAntialias = false;

                        p.IsStroke = true;
                        p.StrokeWidth = 1F;
                        p.Color = borderColor;
                        canvas.DrawLine(L, B, R, B, p);

                        if (dash) p.PathEffect = pe;
                        canvas.DrawLine(L, T, L, B, p);
                        canvas.DrawLine(R, T, R, B, p);
                        if (dash) p.PathEffect = null;
                        #endregion
                        #region Text
                        var sp = canvas.Save();
                        canvas.ClipRect(bounds);

                        DrawText(canvas, bounds,
                            fontName, fontSize, fontStyle,
                            text, foreColor);

                        canvas.RestoreToCount(sp);
                        #endregion
                    }
                    #endregion
                    #region Filter
                    if (boundsFilter.HasValue)
                    {
                        #region var
                        var oxy = 0F;
                        var rt = boundsFilter.Value;
                        var T = rt.Top + oxy;
                        var B = rt.Bottom - oxy;
                        var L = rt.Left + oxy;
                        var R = rt.Right - oxy;
                        #endregion
                        #region Background
                        p.IsStroke = false;
                        p.Color = boxColor;
                        canvas.DrawRect(rt, p);
                        #endregion
                        #region Border
                        p.IsAntialias = false;

                        p.IsStroke = true;
                        p.StrokeWidth = 1F;
                        p.Color = borderColor;
                        canvas.DrawLine(L, B, R, B, p);

                        if (dash) p.PathEffect = pe;
                        canvas.DrawLine(L, T, L, B, p);
                        canvas.DrawLine(R, T, R, B, p);
                        if (dash) p.PathEffect = null;
                        #endregion
                        #region box
                        if (useFilter)
                        {
                            #region Box
                            var rtFilter = DvDataGridTool.FilterBox(rt);

                            DrawBox(canvas, rtFilter, inputBoxColor, inputBorderColor, DvRoundType.Rect, BS_C_BACK, Corner);
                            #endregion
                            #region Text
                            var sp = canvas.Save();
                            canvas.ClipRect(rtFilter);

                            DrawText(canvas, rtFilter,
                                fontName, fontSize, fontStyle,
                                filterText, foreColor);

                            canvas.RestoreToCount(sp);
                            #endregion
                        }
                        #endregion
                    }
                    #endregion
                    #region Sort
                    if (useSort)
                    {
                        var rtSort = Util.FromRect(bounds.Right - 20, bounds.Top, 20, bounds.Height);
                        Util.DrawIcon(canvas, "fa-sort", fontSize, Util.FromArgb(30, foreColor), rtSort);

                        switch (sortState)
                        {
                            case DvDataGridColumnSortState.ASC:
                                Util.DrawIcon(canvas, "fa-sort-up", fontSize, foreColor, rtSort);
                                break;
                            case DvDataGridColumnSortState.DESC:
                                Util.DrawIcon(canvas, "fa-sort-down", fontSize, foreColor, rtSort);
                                break;
                        }
                    }
                    #endregion
                }
            }
        }
        #endregion
        #region DrawDataGridCell
        public override void DrawDataGridCell(SKCanvas canvas,
            SKRect bounds,
            SKColor foreColor, SKColor backColor, SKColor boxColor, SKColor borderColor, SKColor inputBoxColor, SKColor inputBoxBorderColor,
            bool dash = true)
        {
            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                using (var pe = SKPathEffect.CreateDash(new float[] { 3, 3 }, 0))
                {

                    #region var
                    var oxy = 0F;

                    var rt = bounds;
                    var T = rt.Top + oxy;
                    var B = rt.Bottom - oxy;
                    var L = rt.Left + oxy;
                    var R = rt.Right - oxy;
                    #endregion
                    #region Background
                    p.IsStroke = false;
                    p.Color = boxColor;
                    canvas.DrawRect(rt, p);
                    #endregion
                    #region Border
                    p.IsAntialias = false;
                    p.IsStroke = true;
                    p.StrokeWidth = 1F;
                    p.Color = borderColor;
                    //canvas.DrawLine(L, B, R, B, p);

                    if (dash) p.PathEffect = pe;
                    canvas.DrawLine(L, T, L, B, p);
                    canvas.DrawLine(R, T, R, B, p);
                    if (dash) p.PathEffect = null;
                    #endregion
                }
            }
        }
        #endregion
        #region DrawDataGridInputBox
        public override void DrawDataGridInputBox(SKCanvas canvas, SKRect bounds)
        {
            var rtb = bounds;
            var rt = DvDataGridTool.InputBox(bounds);

            using (var p = new SKPaint() { IsAntialias = false, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                p.IsStroke = false;
                p.Color = Util.FromArgb(90, SKColors.Black);
                canvas.DrawRect(rt, p);

                p.IsStroke = true;
                p.StrokeWidth = 1F;
                p.Color = Util.FromArgb(180, SKColors.Black);
                canvas.DrawRect(rt, p);
            }
        }
        #endregion

        #region DrawPanel
        public override void DrawPanel(SKCanvas canvas,
            SKRect rtContent, SKRect rtPanel, SKRect rtTitle, SKRect rtText,
            SKColor panelColor, SKColor foreColor, SKColor backColor,
            DvTextIcon textIcon,
            bool drawTitle)
        {
            var borderColor = GetBorderColor(panelColor, backColor);

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                DrawBox(canvas, rtContent, panelColor, borderColor, DvRoundType.All, BS_BOX_FILL, Corner);

                if (drawTitle)
                {
                    using (var lg = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(0, rtTitle.Bottom),
                                                       new SKColor[] { PanelColor.BrightnessTransmit(GradientLight * 2), PanelColor },
                                                       new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                    {
                        p.Shader = lg;
                        p.IsStroke = false;
                        var rt = new SKRoundRect(rtTitle);
                        rt.SetNinePatch(rtTitle, Corner, Corner, Corner, 0);
                        canvas.DrawRoundRect(rt, p);
                        p.Shader = null;
                    }

                    DrawText(canvas, rtText, textIcon, foreColor, DvContentAlignment.MiddleLeft);
                    using (var mf = SKImageFilter.CreateDropShadow(0, 1, 0, 0, Util.FromArgb(OutBevelAlpha, SKColors.White)))
                    {
                        p.ImageFilter = mf;

                        p.IsStroke = true;
                        p.StrokeWidth = 1;
                        p.Color = borderColor;

                        var y = Convert.ToSingle(Math.Round(rtTitle.Bottom) + 0.5F);
                        canvas.DrawLine(rtContent.Left + 5, y, rtContent.Right - 5, y, p);

                        p.ImageFilter = null;
                    }
                }

                DrawBox(canvas, rtContent, panelColor, borderColor, DvRoundType.All, BS_BOX_BORDER, Corner);

            }
        }
        #endregion
        #region DrawBoxPanel
        public override void DrawBoxPanel(SKCanvas canvas,
            SKRect rtContent, SKRect rtPanel, SKRect rtText,
            SKColor panelColor, SKColor foreColor, SKColor backColor,
            DvTextIcon textIcon,
            float corner, DvRoundType round)
        {

            var borderColor = GetBorderColor(panelColor, backColor);

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                DrawBox(canvas, rtContent, panelColor, borderColor, DvRoundType.All, BoxStyle.Fill | BoxStyle.Border, Corner);

                DrawText(canvas, rtText, textIcon, foreColor, DvContentAlignment.MiddleLeft);
            }
        }
        #endregion
        #region DrawBorderPanel
        public override void DrawBorderPanel(SKCanvas canvas,
            SKRect rtContent, SKRect rtPanel, SKRect rtTitle, SKRect rtText,
            SKColor borderPanelColor, SKColor foreColor, SKColor backColor,
            DvTextIcon textIcon,
            bool drawTitle, float borderWidth)
        {
            rtContent.Inflate(-0.5F, -0.5F);

            var borderColor = GetBorderColor(borderPanelColor, backColor);

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                using (var path = PathTool.BorderPanel(rtContent, rtTitle, drawTitle, borderWidth, Corner))
                {
                    DrawPath(canvas, path, borderPanelColor, borderColor, BoxStyle.Fill | BoxStyle.Border | BoxStyle.InBevelLT);
                }

                #region Text
                if (drawTitle)
                {
                    DrawText(canvas, rtTitle, textIcon, foreColor);
                }
                #endregion
            }
        }
        #endregion
        #region DrawGroupBox
        public override void DrawGroupBox(SKCanvas canvas,
            SKRect rtContent, SKRect rtPanel, SKRect rtText,
            SKColor borderColor, SKColor foreColor, SKColor backColor,
            DvTextIcon textIcon,
            float borderWidth)
        {
            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                using (var mf = SKImageFilter.CreateDropShadow(1, 1, 0, 0, Util.FromArgb(OutBevelAlpha, SKColors.White)))
                {
                    p.ImageFilter = mf;

                    var rtPnl = new SKRoundRect(rtPanel, Corner);
                    p.IsStroke = true;
                    p.StrokeWidth = borderWidth;
                    p.Color = borderColor;

                    rtPnl.Inflate(-borderWidth / 2F, -borderWidth / 2F);
                    canvas.DrawRoundRect(rtPnl, p);

                    p.ImageFilter = null;
                }

                if (!string.IsNullOrWhiteSpace(textIcon.Text) || !string.IsNullOrWhiteSpace(textIcon.IconString))
                {
                    p.IsStroke = false;
                    p.Color = backColor;
                    canvas.DrawRect(rtText, p);

                    DrawText(canvas, rtText, textIcon, foreColor);
                }
            }
        }
        #endregion
        #region DrawTabControl
        public override void DrawTabControl(SKCanvas canvas,
            SKRect rtContent, SKRect rtPage, SKRect rtNavi, Dictionary<string, SKRect> dicTab,
            SKColor foreColor, SKColor backColor, SKColor pointColor, SKColor tabColor, SKColor tabBackColor,
            string fontName, float fontSize, DvFontStyle fontStyle, float iconSize, float iconGap, DvTextIconAlignment iconAlignment,
            DvRoundType round, DvPosition tabPosition, int tabSize,
            DvSubPageCollection tabPages, DvSubPage nowSelTab, DvSubPage prevSelTab,
            bool useAnimation, Animation ani)
        {
            var tabBorderColor = GetBorderColor(tabColor, backColor);
            var tabBackBorderColor = GetBorderColor(tabBackColor, backColor);
            var pointBorderColor = GetBorderColor(pointColor, backColor);
            var pointBackColor = pointColor.WithAlpha(60);

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Round
                var rndTitle = DvRoundType.All;
                var rndPage = DvRoundType.All;
                switch (tabPosition)
                {
                    case DvPosition.Left:
                        {
                            var rnds = Util.Rounds(DvDirectionHV.Horizon, round, 2);
                            rndTitle = rnds[0];
                            rndPage = rnds[1];
                        }
                        break;
                    case DvPosition.Right:
                        {
                            var rnds = Util.Rounds(DvDirectionHV.Horizon, round, 2);
                            rndTitle = rnds[1];
                            rndPage = rnds[0];
                        }
                        break;
                    case DvPosition.Top:
                        {
                            var rnds = Util.Rounds(DvDirectionHV.Vertical, round, 2);
                            rndTitle = rnds[0];
                            rndPage = rnds[1];
                        }
                        break;
                    case DvPosition.Bottom:
                        {
                            var rnds = Util.Rounds(DvDirectionHV.Vertical, round, 2);
                            rndTitle = rnds[1];
                            rndPage = rnds[0];
                        }
                        break;
                }
                #endregion

                #region Tab
                {
                    DrawBox(canvas, rtNavi, tabBackColor, tabBackBorderColor, rndTitle, BoxStyle.Fill, Corner);

                    #region Border
                    {
                        p.IsStroke = true;
                        p.StrokeWidth = 1F;
                        p.Color = tabBackBorderColor;

                        var rt = rtNavi;
                        var o = 0.5F;
                        switch (tabPosition)
                        {
                            case DvPosition.Left: canvas.DrawLine(rt.Right - o, rt.Top, rt.Right - o, rt.Bottom, p); break;
                            case DvPosition.Right: canvas.DrawLine(rt.Left + o, rt.Top, rt.Left + o, rt.Bottom, p); break;
                            case DvPosition.Top: canvas.DrawLine(rt.Left, rt.Bottom - o, rt.Right, rt.Bottom - o, p); break;
                            case DvPosition.Bottom: canvas.DrawLine(rt.Left, rt.Top + o, rt.Right, rt.Top + o, p); break;
                        }
                    }
                    #endregion
                    #region Cursor
                    if (useAnimation && ani.IsPlaying)
                    {
                        TabTool.AniTabCursor(tabPages, nowSelTab, prevSelTab,
                            tabPosition, dicTab, ani,
                            (rt, rtv) =>
                            {
                                DrawBox(canvas, rt, pointBackColor, SKColors.White, DvRoundType.Rect, BoxStyle.Fill, Corner);
                                DrawBox(canvas, rtv, pointColor, pointBorderColor, DvRoundType.Rect, BS_BTN_UP_FILL, Corner);
                            }
                        );
                    }
                    else
                    {
                        if (nowSelTab != null && dicTab.ContainsKey(nowSelTab.Name))
                        {
                            var rt = dicTab[nowSelTab.Name];
                            var rtv = TabTool.TabCur(dicTab[nowSelTab.Name], tabPosition);

                            DrawBox(canvas, rt, pointBackColor, SKColors.White, DvRoundType.Rect, BoxStyle.Fill, Corner);
                            DrawBox(canvas, rtv, pointColor, pointBorderColor, DvRoundType.Rect, BS_BTN_UP_FILL, Corner);
                        }
                    }
                    #endregion
                    #region Text
                    foreach (var vk in tabPages.Keys)
                    {
                        if (dicTab.ContainsKey(vk))
                        {
                            var page = tabPages[vk];
                            TabTool.AniTabText(tabPages, nowSelTab, prevSelTab, page, ani, (a) =>
                            {
                                var rt = dicTab[vk];
                                var rtc = TabTool.TabCur(rt, tabPosition);
                                var rtt = TabTool.TabText(rt, tabPosition);
                                var ctxt = foreColor.WithAlpha(a);

                                #region TextIcon
                                var ti = new DvTextIcon
                                {
                                    Text = page.Text,
                                    FontName = fontName,
                                    FontSize = fontSize,
                                    FontStyle = fontStyle,
                                    IconString = page.IconString,
                                    IconSize = iconSize,
                                    IconGap = iconGap,
                                    IconAlignment = iconAlignment,
                                };

                                DrawText(canvas, rtt, ti, ctxt);
                                #endregion
                            });
                        }
                    }
                    #endregion
                }
                #endregion
                #region Page
                {
                    DrawBox(canvas, rtPage, tabColor, tabBorderColor, rndPage, BoxStyle.Fill, Corner);
                }
                #endregion
                #region Border2
                {
                    DrawBox(canvas, rtContent, tabBackColor, tabBackBorderColor, round, BoxStyle.Border, Corner);
                }
                #endregion
            }
        }
        #endregion

        #region DrawScroll
        public override void DrawScroll(SKCanvas canvas, SKRect rtScroll, SKColor backColor, Scroll scroll, DvRoundType round = DvRoundType.All)
        {
            DrawBox(canvas, rtScroll, ScrollBarColor, ScrollBarColor, round, BoxStyle.Fill, Corner);

            if (scroll.ScrollVisible)
            {
                var sp = canvas.Save();
                canvas.ClipRect(rtScroll);
                var cCur = (scroll.IsScrolling || scroll.IsTouchMoving) ? ScrollCursorOnColor : ScrollCursorOffColor;
                var rtcur = scroll.GetScrollCursorRect(rtScroll);
                if (rtcur.HasValue)
                {
                    var rtc = Util.INT(Util.FromRect(rtcur.Value));
                    DrawBox(canvas, rtc, cCur, cCur, round, BoxStyle.Fill, Corner);
                }
                canvas.RestoreToCount(sp);
            }
        }

        public override void DrawScrollR(SKCanvas canvas, SKRect rtScroll, SKColor backColor, Scroll scroll, DvRoundType round = DvRoundType.All)
        {
            DrawBox(canvas, rtScroll, ScrollBarColor, ScrollBarColor, round, BoxStyle.Fill, Corner);

            if (scroll.ScrollVisible)
            {
                var sp = canvas.Save();
                canvas.ClipRect(rtScroll);
                var cCur = (scroll.IsScrolling || scroll.IsTouchMoving) ? ScrollCursorOnColor : ScrollCursorOffColor;
                var rtcur = scroll.GetScrollCursorRectR(rtScroll);
                if (rtcur.HasValue)
                {
                    var rtc = Util.INT(Util.FromRect(rtcur.Value));
                    DrawBox(canvas, rtc, cCur, cCur, round, BoxStyle.Fill, Corner);
                }
                canvas.RestoreToCount(sp);
            }
        }
        #endregion
        #endregion

        #region Method
        #region DrawBox
        public override void DrawBox(SKCanvas canvas, SKRect rect, SKColor boxColor, SKColor borderColor, DvRoundType round, BoxStyle style, float corner, float inWidth = 1, float outWidth = 1, float borderWidth = 1)
        {
            #region Var
            SKRect vrt = rect;
            SKRect vrtIn = rect; vrtIn.Inflate(-1, -1);

            SKRoundRect rt = new SKRoundRect(vrt, 0);
            SKRoundRect rtIn = new SKRoundRect(vrtIn, 0);
            SKImageFilter fOut = null;
            #endregion

            #region Round
            switch (round)
            {
                #region case DvRoundType.Rect:
                case DvRoundType.Rect:
                    rt.SetNinePatch(vrt, 0, 0, 0, 0);
                    rtIn.SetNinePatch(vrtIn, 0, 0, 0, 0);
                    break;
                #endregion
                #region case DvRoundType.All:
                case DvRoundType.All:
                    rt.SetNinePatch(vrt, corner, corner, corner, corner);
                    rtIn.SetNinePatch(vrtIn, corner, corner, corner, corner);
                    break;
                #endregion
                #region   case DvRoundType.L:
                case DvRoundType.L:
                    rt.SetNinePatch(vrt, corner, corner, 0, corner);
                    rtIn.SetNinePatch(vrtIn, corner, corner, 0, corner);
                    break;
                #endregion
                #region case DvRoundType.R:
                case DvRoundType.R:
                    rt.SetNinePatch(vrt, 0, corner, corner, corner);
                    rtIn.SetNinePatch(vrtIn, 0, corner, corner, corner);
                    break;
                #endregion
                #region case DvRoundType.T:
                case DvRoundType.T:
                    rt.SetNinePatch(vrt, corner, corner, corner, 0);
                    rtIn.SetNinePatch(vrtIn, corner, corner, corner, 0);
                    break;
                #endregion
                #region case DvRoundType.B:
                case DvRoundType.B:
                    rt.SetNinePatch(vrt, corner, 0, corner, corner);
                    rtIn.SetNinePatch(vrtIn, corner, 0, corner, corner);
                    break;
                #endregion
                #region  case DvRoundType.LT:
                case DvRoundType.LT:
                    rt.SetNinePatch(vrt, corner, corner, 0, 0);
                    rtIn.SetNinePatch(vrtIn, corner, corner, 0, 0);
                    break;
                #endregion
                #region case DvRoundType.RT:
                case DvRoundType.RT:
                    rt.SetNinePatch(vrt, 0, corner, corner, 0);
                    rtIn.SetNinePatch(vrtIn, 0, corner, corner, 0);
                    break;
                #endregion
                #region case DvRoundType.LB:
                case DvRoundType.LB:
                    rt.SetNinePatch(vrt, corner, 0, 0, corner);
                    rtIn.SetNinePatch(vrtIn, corner, 0, 0, corner);
                    break;
                #endregion
                #region case DvRoundType.RB:
                case DvRoundType.RB:
                    rt.SetNinePatch(vrt, 0, 0, corner, corner);
                    rtIn.SetNinePatch(vrtIn, 0, 0, corner, corner);
                    break;
                #endregion
                #region case DvRoundType.Ellipse:
                case DvRoundType.Ellipse:
                    rt.SetOval(vrt);
                    rtIn.SetOval(vrtIn);
                    break;
                    #endregion
            }
            #endregion

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region OutShadow / OutBevel
                if ((style & BoxStyle.OutShadow) == BoxStyle.OutShadow)
                {
                    fOut = SKImageFilter.CreateDropShadow(outWidth, outWidth, 0, 0, Util.FromArgb(OutShadowAlpha, SKColors.Black));
                    p.ImageFilter = fOut;
                }
                else if ((style & BoxStyle.OutBevel) == BoxStyle.OutBevel)
                {
                    fOut = SKImageFilter.CreateDropShadow(0, outWidth, 0, 0, Util.FromArgb(OutBevelAlpha, SKColors.White));
                    p.ImageFilter = fOut;
                }
                #endregion
                #region Fill / Gradient
                if ((style & BoxStyle.Fill) == BoxStyle.Fill)
                {
                    #region Fill
                    if (boxColor.Alpha != 0)
                    {
                        p.Color = boxColor;
                        p.IsStroke = false;
                        var rtv = new SKRoundRect(rt);
                        //rtv.Inflate(-0.5F, -0.5F);
                        canvas.DrawRoundRect(rtv, p);
                    }
                    #endregion
                }
                else
                {
                    #region Gradient
                    SKPoint? sp = null;
                    SKPoint? ep = null;
                    if ((style & BoxStyle.GradientV) == BoxStyle.GradientV)
                    {
                        sp = new SKPoint(rect.Left, rect.Top);
                        ep = new SKPoint(rect.Left, rect.Bottom);
                    }
                    else if ((style & BoxStyle.GradientV_R) == BoxStyle.GradientV_R)
                    {
                        ep = new SKPoint(rect.Left, rect.Top);
                        sp = new SKPoint(rect.Left, rect.Bottom);
                    }
                    else if ((style & BoxStyle.GradientH) == BoxStyle.GradientH)
                    {
                        sp = new SKPoint(rect.Left, rect.Top);
                        ep = new SKPoint(rect.Right, rect.Top);
                    }
                    else if ((style & BoxStyle.GradientH_R) == BoxStyle.GradientH_R)
                    {
                        ep = new SKPoint(rect.Left, rect.Top);
                        sp = new SKPoint(rect.Right, rect.Top);
                    }

                    if (sp.HasValue && ep.HasValue)
                    {
                        using (var lg = SKShader.CreateLinearGradient(sp.Value, ep.Value,
                                                               new SKColor[] { boxColor.BrightnessTransmit(GradientLight), boxColor.BrightnessTransmit(GradientDark) },
                                                               new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                        {

                            p.IsStroke = false;
                            p.Shader = lg;
                            canvas.DrawRoundRect(rt, p);
                            p.Shader = null;
                        }
                    }
                    #endregion
                }
                p.ImageFilter = null;
                #endregion
                #region Bevel / InShadow
                if ((style & BoxStyle.InBevelLT) == BoxStyle.InBevelLT)
                {
                    var sp = canvas.Save();
                    {
                        p.IsStroke = true;
                        p.StrokeWidth = inWidth;
                        p.Color = Util.FromArgb(GetBevelAlpha(boxColor), SKColors.White);

                        if (round == DvRoundType.Rect)
                        {
                            var rtv = Util.FromRect(rect);
                            canvas.ClipRect(rtv);
                            canvas.Translate(1, 1);
                            canvas.DrawRect(rtv, p);
                        }
                        else
                        {
                            canvas.ClipRoundRect(rt);
                            canvas.Translate(1, 1);
                            canvas.DrawRoundRect(rt, p);
                        }
                    }
                    canvas.RestoreToCount(sp);
                }
                else if ((style & BoxStyle.InBevel) == BoxStyle.InBevel)
                {
                    using (var lg = SKShader.CreateLinearGradient(new SKPoint(rect.Left, rect.Top), new SKPoint(rect.Left, rect.Bottom),
                                                                new SKColor[] { Util.FromArgb(GetBevelAlpha(boxColor), SKColors.White), Util.FromArgb(0, SKColors.White) },
                                                                new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                    {
                        p.Shader = lg;
                        p.IsStroke = true;
                        p.StrokeWidth = inWidth;

                        if (round == DvRoundType.Rect)
                        {
                            var rtv = Util.FromRect(((style & BoxStyle.Border) == BoxStyle.Border ? vrtIn : vrt));
                            rtv.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                            canvas.DrawRect(rtv, p);
                        }
                        else
                        {
                            var rtv = new SKRoundRect(((style & BoxStyle.Border) == BoxStyle.Border ? rtIn : rt));
                            rtv.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                            canvas.DrawRoundRect(rtv, p);
                        }
                        p.Shader = null;
                    }
                }
                else if ((style & BoxStyle.InShadow) == BoxStyle.InShadow)
                {
                    using (var mf = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1))
                    {
                        p.IsStroke = true;
                        p.StrokeWidth = inWidth;
                        p.Color = Util.FromArgb(InShadowAlpha, SKColors.Black);
                        p.MaskFilter = mf;

                        var rtv = new SKRoundRect(((style & BoxStyle.Border) == BoxStyle.Border ? rtIn : rt));
                        rtv.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);

                        canvas.DrawRoundRect(rtv, p);
                        p.MaskFilter = null;
                    }
                }
                #endregion
                #region Border
                if ((style & BoxStyle.Border) == BoxStyle.Border)
                {
                    p.StrokeWidth = borderWidth;
                    p.IsStroke = true;
                    p.Color = borderColor;

                    rt.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                    canvas.DrawRoundRect(rt, p);
                }
                #endregion
            }

            #region Dispose
            if (fOut != null) fOut.Dispose();
            fOut = null;
            #endregion
        }
        #endregion
        #region DrawPath
        public override void DrawPath(SKCanvas canvas, SKPath path, SKColor boxColor, SKColor borderColor, BoxStyle style, float inWidth = 1, float outWidth = 1, float borderWidth = 1)
        {
            SKImageFilter fOut = null;
            var rect = path.Bounds;
            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region OutShadow / OutBevel
                if ((style & BoxStyle.OutShadow) == BoxStyle.OutShadow)
                {
                    fOut = SKImageFilter.CreateDropShadow(outWidth, outWidth, 0, 0, Util.FromArgb(OutShadowAlpha, SKColors.Black));
                    p.ImageFilter = fOut;
                }
                else if ((style & BoxStyle.OutBevel) == BoxStyle.OutBevel)
                {
                    fOut = SKImageFilter.CreateDropShadow(0, outWidth, 0, 0, Util.FromArgb(OutBevelAlpha, SKColors.White));
                    p.ImageFilter = fOut;
                }
                #endregion
                #region Fill / Gradient
                if ((style & BoxStyle.Fill) == BoxStyle.Fill)
                {
                    #region Fill
                    if (boxColor.Alpha != 0)
                    {
                        p.Color = boxColor;
                        p.IsStroke = false;
                        canvas.DrawPath(path, p);
                    }
                    #endregion
                }
                else
                {
                    #region Gradient
                    SKPoint? sp = null;
                    SKPoint? ep = null;
                    if ((style & BoxStyle.GradientV) == BoxStyle.GradientV)
                    {
                        sp = new SKPoint(rect.Left, rect.Top);
                        ep = new SKPoint(rect.Left, rect.Bottom);
                    }
                    else if ((style & BoxStyle.GradientV_R) == BoxStyle.GradientV_R)
                    {
                        ep = new SKPoint(rect.Left, rect.Top);
                        sp = new SKPoint(rect.Left, rect.Bottom);
                    }
                    else if ((style & BoxStyle.GradientH) == BoxStyle.GradientH)
                    {
                        sp = new SKPoint(rect.Left, rect.Top);
                        ep = new SKPoint(rect.Right, rect.Top);
                    }
                    else if ((style & BoxStyle.GradientH_R) == BoxStyle.GradientH_R)
                    {
                        ep = new SKPoint(rect.Left, rect.Top);
                        sp = new SKPoint(rect.Right, rect.Top);
                    }

                    if (sp.HasValue && ep.HasValue)
                    {
                        using (var lg = SKShader.CreateLinearGradient(sp.Value, ep.Value,
                                                               new SKColor[] { boxColor.BrightnessTransmit(GradientLight), boxColor.BrightnessTransmit(GradientDark) },
                                                               new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                        {

                            p.IsStroke = false;
                            p.Shader = lg;
                            canvas.DrawPath(path, p);
                            p.Shader = null;
                        }
                    }
                    #endregion
                }
                p.ImageFilter = null;
                #endregion
                #region Bevel / InShadow
                if ((style & BoxStyle.InBevelLT) == BoxStyle.InBevelLT)
                {
                    var sp = canvas.Save();
                    {
                        p.IsStroke = true;
                        p.StrokeWidth = inWidth;
                        p.Color = Util.FromArgb(GetBevelAlpha(boxColor), SKColors.White);

                        canvas.ClipPath(path);
                        canvas.Translate(1, 1);
                        canvas.DrawPath(path, p);
                    }
                    canvas.RestoreToCount(sp);
                }
                else if ((style & BoxStyle.InBevel) == BoxStyle.InBevel)
                {
                    var rt = path.Bounds;
                    var cp = MathTool.CenterPoint(rt);
                    var sx = 1F / DvDesign.ActiveDesign.Width;
                    var sy = 1F / DvDesign.ActiveDesign.Height;

                    using (var lg = SKShader.CreateLinearGradient(new SKPoint(rect.Left, rect.Top), new SKPoint(rect.Left, rect.Bottom),
                                                               new SKColor[] { Util.FromArgb(GetBevelAlpha(boxColor), SKColors.White), Util.FromArgb(GetBevelAlpha(boxColor), SKColors.Black) },
                                                               new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                    {
                        p.Shader = lg;
                        p.IsStroke = true;
                        p.StrokeWidth = inWidth;

                        var sp = canvas.Save();
                        canvas.ClipPath(path);
                        canvas.Translate(1, 1);
                        canvas.DrawPath(path, p);
                        canvas.RestoreToCount(sp);

                        sp = canvas.Save();
                        canvas.ClipPath(path);
                        canvas.Translate(-1, -1);
                        canvas.DrawPath(path, p);
                        canvas.RestoreToCount(sp);

                        p.Shader = null;
                    }
                }
                else if ((style & BoxStyle.InShadow) == BoxStyle.InShadow)
                {
                    using (var mf = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1))
                    {
                        p.IsStroke = true;
                        p.StrokeWidth = inWidth;
                        p.Color = Util.FromArgb(InShadowAlpha, SKColors.Black);
                        p.MaskFilter = mf;

                        canvas.DrawPath(path, p);

                        p.MaskFilter = null;
                    }
                }
                #endregion
                #region Border
                if ((style & BoxStyle.Border) == BoxStyle.Border)
                {
                    p.StrokeWidth = borderWidth;
                    p.IsStroke = true;
                    p.Color = borderColor;

                    canvas.DrawPath(path, p);
                }
                #endregion
            }

            #region Dispose
            if (fOut != null) fOut.Dispose();
            fOut = null;
            #endregion

        }
        #endregion

        #region GetBorderColor
        public override SKColor GetBorderColor(SKColor fillColor, SKColor backColor)
        {
            float hf, sf, vf, hb, sb, vb;
            fillColor.ToHsv(out hf, out sf, out vf);
            backColor.ToHsv(out hb, out sb, out vb);

            var c = SKColor.FromHsv(hf, sf, Math.Min(vf, vb));
            return c.BrightnessTransmit(BorderBrightness);
        }
        #endregion
        #region GetBevelAlpha
        public override byte GetBevelAlpha(SKColor BaseColor)
        {
            float hb, sb, vb;
            BaseColor.ToHsv(out hb, out sb, out vb);

            return Convert.ToByte(MathTool.Constrain(vb * 2.55 * 0.4, 0, 255));
        }
        #endregion
        #region GetLampColors
        void GetLampColors(SKColor BackColor, SKColor OnLampColor, SKColor OffLampColor, bool OnOff, bool Animation, Animation ani, out SKColor BackLightColor, out SKColor BackDarkColor, out SKColor LampLightColor, out SKColor LampDarkColor, out SKColor LampColor)
        {
            #region Brightness
            var vBS = 0.1F;
            var vBE = -0.3F;
            var vS = 0.2F;
            var vE = -0.2F;

            if (OnOff)
            {
                vS = 0.5F;
                vE = -0.2F;
            }
            #endregion

            var cM = OnOff ? OnLampColor : OffLampColor;
            if (Animation && ani.IsPlaying) cM = ani.Value(AnimationAccel.Linear, ani.Variable == "On" ? OffLampColor : OnLampColor, ani.Variable == "On" ? OnLampColor : OffLampColor);

            BackDarkColor = BackColor.BrightnessTransmit(vBS);
            BackLightColor = BackColor.BrightnessTransmit(vBE);
            LampLightColor = cM.BrightnessTransmit(vS);
            LampDarkColor = cM.BrightnessTransmit(vE);
            LampColor = cM;
        }
        #endregion
        #region GetSwitchColors
        void GetSwitchColors(SKColor Color, out SKColor c1, out SKColor c2, out SKColor c3, out SKColor c4)
        {
            c1 = Color.BrightnessTransmit(0.3F);
            c2 = Color.BrightnessTransmit(0F);
            c3 = Color.BrightnessTransmit(-0.2F);
            c4 = Color.BrightnessTransmit(-0.4F);
        }
        #endregion
        #endregion
    }
}

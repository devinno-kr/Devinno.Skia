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
    public abstract class DvTheme
    {
        public const float GP = 6F;
        public const float GP2 = 10F;

        public abstract ThemeBrightness Brightness { get; }
        public abstract float DownBrightness { get; set; }
        public abstract float BorderBrightness { get; set; }
        public abstract float GradientLight { get; set; }
        public abstract float GradientDark { get; set; }
        public abstract float Corner { get; set; }
        public abstract float KeySpecialButtonBrightness { get; }
        public abstract byte OutShadowAlpha { get; set; }
        public abstract byte OutBevelAlpha { get; set; }
        public abstract byte InShadowAlpha { get; set; }
        public abstract byte GradientLightAlpha { get; }
        public abstract byte GradientDarkAlpha { get; }
        public abstract float DataGridInputBright { get; }
        public abstract float DataGridCheckBoxBright { get; }
        public abstract float DataGridColumnBevelBright { get; }
        public abstract float DataGridRowBevelBright { get; }

        public abstract SKColor ForeColor { get; }
        public abstract SKColor BackColor { get; }
        public abstract SKColor ButtonColor { get; }

        public abstract SKColor LabelColor { get; }
        public abstract SKColor InputColor { get; }
        public abstract SKColor CheckBoxColor { get; }
        public abstract SKColor LampOnColor { get; }
        public abstract SKColor LampOffColor { get; }
        public abstract SKColor StepOnColor { get; }
        public abstract SKColor StepOffColor { get; }
        public abstract SKColor KnobColor { get; }
        public abstract SKColor KnobCursorColor { get; }
        public abstract SKColor NeedleColor { get; }
        public abstract SKColor NeedlePointColor { get; }
        public abstract SKColor ConcaveBoxColor { get; }
        public abstract SKColor GridColor { get; }
        public abstract SKColor PanelColor { get; }
        public abstract SKColor BorderPanelColor { get; }
        public abstract SKColor GroupBoxColor { get; }
        public abstract SKColor TabBackColor { get; }
        public abstract SKColor TabPageColor { get; }
        public abstract SKColor RowColor { get; }
        public abstract SKColor ColumnColor { get; }
        public abstract SKColor ListBackColor { get; }
        public abstract SKColor WindowTitleColor { get; }
        public abstract SKColor CalendarDaysColor { get; }
        public abstract SKColor CalendarWeeksColor { get; }
        public abstract SKColor CalendarMonthColor { get; }
        public abstract SKColor CalendarSelectColor { get; }
        public abstract SKColor PointColor { get; }
        public abstract SKColor ScrollBarColor { get; }
        public abstract SKColor ScrollCursorOffColor { get; }
        public abstract SKColor ScrollCursorOnColor { get; }

        public abstract void DrawBox(SKCanvas canvas, SKRect rect, SKColor boxColor, SKColor borderColor, RoundType round, BoxStyle style, float? Corner = null);
        public abstract SKColor GetBorderColor(SKColor fillColor, SKColor backColor);
        public abstract SKColor GetInBevelColor(SKColor BaseColor);
        public abstract void GetSwitchColors(SKColor Color, out SKColor c1, out SKColor c2, out SKColor c3, out SKColor c4);
        public abstract void GetLampColors(SKColor BackColor, SKColor OnLampColor, SKColor OffLampColor, bool OnOff, bool Animation, Animation ani, out SKColor BackLightColor, out SKColor BackDarkColor, out SKColor LampLightColor, out SKColor LampDarkColor, out SKColor LampColor);
    }

    public class BlackTheme : DvTheme
    {
        public override ThemeBrightness Brightness =>  ThemeBrightness.Dark;

        public override float DownBrightness { get; set; } = -0.25F;
        public override float BorderBrightness { get; set; } = -0.6F;
        public override float GradientLight { get; set; } = 0.2F;
        public override float GradientDark { get; set; } = -0.2F;
        public override float Corner { get; set; } = 6F;
        public override byte OutShadowAlpha { get; set; } = 60;
        public override byte OutBevelAlpha { get; set; } = 20;
        public override byte InShadowAlpha { get; set; } = 120;
        public override float KeySpecialButtonBrightness => -0.2F;
        public override byte GradientLightAlpha => 30;
        public override byte GradientDarkAlpha => 30;
        public override float DataGridInputBright => -0.2F;
        public override float DataGridCheckBoxBright => -0.5F;
        public override float DataGridColumnBevelBright => 0.3F;
        public override float DataGridRowBevelBright => 0.3F;

        public override SKColor ForeColor => SKColors.White;
        public override SKColor BackColor => Util.FromArgb(50, 50, 50);
        public override SKColor ButtonColor => Util.FromArgb(90, 90, 90);
        public override SKColor LabelColor => Util.FromArgb(60, 60, 60);
        public override SKColor InputColor => Util.FromArgb(30, 30, 30);
        public override SKColor CheckBoxColor => Util.FromArgb(30, 30, 30);
        public override SKColor LampOnColor => SKColors.Red;
        public override SKColor LampOffColor => Util.FromArgb(90, 90, 90);
        public override SKColor StepOnColor => SKColors.Red;
        public override SKColor StepOffColor => Util.FromArgb(90, 90, 90);
        public override SKColor ConcaveBoxColor => Util.FromArgb(30, 30, 30);
        public override SKColor KnobColor => Util.FromArgb(60, 60, 60);
        public override SKColor KnobCursorColor => SKColors.White;
        public override SKColor NeedleColor => SKColors.White;
        public override SKColor NeedlePointColor => SKColors.Red;
        public override SKColor GridColor => Util.FromArgb(120, 120, 120);
        public override SKColor PanelColor => Util.FromArgb(60, 60, 60);
        public override SKColor BorderPanelColor => Util.FromArgb(90, 90, 90);
        public override SKColor GroupBoxColor => SKColors.Black;
        public override SKColor TabBackColor => Util.FromArgb(30, 30, 30);
        public override SKColor TabPageColor => Util.FromArgb(60, 60, 60);
        public override SKColor RowColor => Util.FromArgb(90, 90, 90);
        public override SKColor ColumnColor => Util.FromArgb(40, 45, 50);
        public override SKColor ListBackColor => Util.FromArgb(60, 60, 60);
        public override SKColor WindowTitleColor => Util.FromArgb(30, 30, 30);
        public override SKColor CalendarDaysColor => Util.FromArgb(60, 60, 60);
        public override SKColor CalendarWeeksColor => Util.FromArgb(30, 30, 30);
        public override SKColor CalendarMonthColor => Util.FromArgb(60, 60, 60);
        public override SKColor CalendarSelectColor => SKColors.Cyan;
        public override SKColor PointColor => SKColors.DarkRed;
        public override SKColor ScrollBarColor => Util.FromArgb(30, 30, 30);
        public override SKColor ScrollCursorOffColor => Util.FromArgb(180, 180, 180);
        public override SKColor ScrollCursorOnColor => SKColors.DarkRed;

        #region DrawBox
        public override void DrawBox(SKCanvas canvas, SKRect rect, SKColor boxColor, SKColor borderColor, RoundType round, BoxStyle style, float? Corner = null)
        {
            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                var corner = Corner ?? this.Corner;
                var _rt = Util.FromRect(rect.Left, rect.Top, rect.Width, rect.Height);
                var _rtIn = Util.FromRect(rect.Left, rect.Top, rect.Width, rect.Height); _rtIn.Inflate(-1, -1);
                var rt = new SKRoundRect(_rt, corner);
                var rtIn = new SKRoundRect(_rtIn, corner);

                #region Round
                switch (round)
                {
                    #region case RoundType.Rect:
                    case RoundType.Rect:
                        rt.SetNinePatch(_rt, 0, 0, 0, 0);
                        rtIn.SetNinePatch(_rtIn, 0, 0, 0, 0);
                        break;
                    #endregion
                    #region case RoundType.Round:
                    case RoundType.Round:
                        rt.SetNinePatch(_rt, corner, corner, corner, corner);
                        rtIn.SetNinePatch(_rtIn, corner, corner, corner, corner);
                        break;
                    #endregion
                    #region case RoundType.Round_L:
                    case RoundType.Round_L:
                        rt.SetNinePatch(_rt, corner, corner, 0, corner);
                        rtIn.SetNinePatch(_rtIn, corner, corner, 0, corner);
                        break;
                    #endregion
                    #region case RoundType.Round_R:
                    case RoundType.Round_R:
                        rt.SetNinePatch(_rt, 0, corner, corner, corner);
                        rtIn.SetNinePatch(_rtIn, 0, corner, corner, corner);
                        break;
                    #endregion
                    #region case RoundType.Round_T:
                    case RoundType.Round_T:
                        rt.SetNinePatch(_rt, corner, corner, corner, 0);
                        rtIn.SetNinePatch(_rtIn, corner, corner, corner, 0);
                        break;
                    #endregion
                    #region case RoundType.Round_B:
                    case RoundType.Round_B:
                        rt.SetNinePatch(_rt, corner, 0, corner, corner);
                        rtIn.SetNinePatch(_rtIn, corner, 0, corner, corner);
                        break;
                    #endregion
                    #region case RoundType.Round_LT:
                    case RoundType.Round_LT:
                        rt.SetNinePatch(_rt, corner, corner, 0, 0);
                        rtIn.SetNinePatch(_rtIn, corner, corner, 0, 0);
                        break;
                    #endregion
                    #region case RoundType.Round_RT:
                    case RoundType.Round_RT:
                        rt.SetNinePatch(_rt, 0, corner, corner, 0);
                        rtIn.SetNinePatch(_rtIn, 0, corner, corner, 0);
                        break;
                    #endregion
                    #region case RoundType.Round_LB:
                    case RoundType.Round_LB:
                        rt.SetNinePatch(_rt, corner, 0, 0, corner);
                        rtIn.SetNinePatch(_rtIn, corner, 0, 0, corner);
                        break;
                    #endregion
                    #region case RoundType.Round_RB:
                    case RoundType.Round_RB:
                        rt.SetNinePatch(_rt, 0, 0, corner, corner);
                        rtIn.SetNinePatch(_rtIn, 0, 0, corner, corner);
                        break;
                    #endregion
                    #region case RoundType.Ellipse:
                    case RoundType.Ellipse:
                        rt.SetOval(_rt);
                        rtIn.SetOval(_rtIn);
                        break;
                        #endregion
                }
                #endregion

                #region Style

                var fOut = (SKImageFilter)null;

                #region OutShadow / OutBevel
                if ((style & BoxStyle.OutShadow) == BoxStyle.OutShadow)
                {
                    fOut = SKImageFilter.CreateDropShadow(1, 1, 0, 0, Util.FromArgb(OutShadowAlpha, SKColors.Black));
                    p.ImageFilter = fOut;
                }
                else if ((style & BoxStyle.OutBevel) == BoxStyle.OutBevel)
                {
                    fOut = SKImageFilter.CreateDropShadow(0, 1, 0, 0, Util.FromArgb(OutBevelAlpha, SKColors.White));
                    p.ImageFilter = fOut;
                }
                #endregion
                #region Fill / Gradient
                if ((style & BoxStyle.Fill) == BoxStyle.Fill)
                {
                    if (boxColor.Alpha != 0)
                    {
                        #region Fill
                        p.Color = boxColor;
                        canvas.DrawRoundRect(rt, p);
                        #endregion
                    }
                }
                else
                {
                    #region Gradient Point
                    SKPoint? sp = null;
                    SKPoint? ep = null;
                    if ((style & BoxStyle.GradientV) == BoxStyle.GradientV)
                    {
                        sp = new SKPoint(_rt.Left, _rt.Top);
                        ep = new SKPoint(_rt.Left, _rt.Bottom);
                    }
                    else if ((style & BoxStyle.GradientV_R) == BoxStyle.GradientV_R)
                    {
                        ep = new SKPoint(_rt.Left, _rt.Top);
                        sp = new SKPoint(_rt.Left, _rt.Bottom);
                    }
                    else if ((style & BoxStyle.GradientH) == BoxStyle.GradientH)
                    {
                        sp = new SKPoint(_rt.Left, _rt.Top);
                        ep = new SKPoint(_rt.Right, _rt.Top);
                    }
                    else if ((style & BoxStyle.GradientH_R) == BoxStyle.GradientH_R)
                    {
                        ep = new SKPoint(_rt.Left, _rt.Top);
                        sp = new SKPoint(_rt.Right, _rt.Top);
                    }
                    else if ((style & BoxStyle.GradientLT) == BoxStyle.GradientLT)
                    {
                        sp = new SKPoint(_rt.Left, _rt.Top);
                        ep = new SKPoint(_rt.Right, _rt.Bottom);
                    }
                    else if ((style & BoxStyle.GradientLT_R) == BoxStyle.GradientLT_R)
                    {
                        ep = new SKPoint(_rt.Left, _rt.Top);
                        sp = new SKPoint(_rt.Right, _rt.Bottom);
                    }
                    else if ((style & BoxStyle.GradientRT) == BoxStyle.GradientRT)
                    {
                        sp = new SKPoint(_rt.Right, _rt.Top);
                        ep = new SKPoint(_rt.Left, _rt.Bottom);
                    }
                    else if ((style & BoxStyle.GradientRT_R) == BoxStyle.GradientRT_R)
                    {
                        ep = new SKPoint(_rt.Right, _rt.Top);
                        sp = new SKPoint(_rt.Left, _rt.Bottom);
                    }
                    #endregion
                    #region Gradient Fill
                    if (sp.HasValue && ep.HasValue)
                    {
                        using (var lg = SKShader.CreateLinearGradient(sp.Value, ep.Value,
                                                                new SKColor[] { boxColor.BrightnessTransmit(GradientLight), boxColor.BrightnessTransmit(GradientDark) },
                                                                new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                        {

                            p.Shader = lg;
                            canvas.DrawRoundRect(rt, p);
                            p.Shader = null;
                        }
                    }
                    #endregion
                }
                #endregion

                #region ImageFilter Free
                p.ImageFilter = null;
                if (fOut != null) fOut.Dispose();
                fOut = null;
                #endregion

                #region InBevel / InShadow
                if ((style & BoxStyle.InBevel) == BoxStyle.InBevel)
                {

                    using (var lg = SKShader.CreateLinearGradient(new SKPoint(_rtIn.Left, _rtIn.Top), new SKPoint(_rtIn.Left, _rtIn.Bottom),
                                                                new SKColor[] { GetInBevelColor(boxColor), Util.FromArgb(0, boxColor) },
                                                                new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                    {
                        p.Shader = lg;
                        p.IsStroke = true;
                        p.StrokeWidth = 1;
                        rtIn.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                        canvas.DrawRoundRect(rtIn, p);
                        p.Shader = null;
                    }
                }
                else if ((style & BoxStyle.InShadow) == BoxStyle.InShadow)
                {
                    using (var mf = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1))
                    {
                        p.IsStroke = true;
                        p.StrokeWidth = 1;
                        p.Color = Util.FromArgb(InShadowAlpha, SKColors.Black);
                        p.MaskFilter = mf;
                        rtIn.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                        canvas.DrawRoundRect(rtIn, p);
                        p.MaskFilter = null;
                    }
                }
                #endregion

                #region Border
                if ((style & BoxStyle.Border) == BoxStyle.Border)
                {
                    p.StrokeWidth = 1;
                    p.IsStroke = true;
                    p.Color = borderColor;
                    rt.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                    canvas.DrawRoundRect(rt, p);
                }
                #endregion
                #endregion
            }
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
        #region GetInBevelColor
        public override SKColor GetInBevelColor(SKColor BaseColor)
        {
            float hb, sb, vb;
            BaseColor.ToHsv(out hb, out sb, out vb);

            var v = Convert.ToByte(MathTool.Constrain(vb * 2.55 * 0.6, 0, 255));

            return Util.FromArgb(v, SKColors.White);
        }
        #endregion
        #region GetSwitchColors
        public override void GetSwitchColors(SKColor Color, out SKColor c1, out SKColor c2, out SKColor c3, out SKColor c4)
        {
            c1 = Color.BrightnessTransmit(0.3F);
            c2 = Color.BrightnessTransmit(0F);
            c3 = Color.BrightnessTransmit(-0.2F);
            c4 = Color.BrightnessTransmit(-0.4F);
        }
        #endregion
        #region GetLampColors
        public override void GetLampColors(SKColor BackColor, SKColor OnLampColor, SKColor OffLampColor, bool OnOff, bool Animation, Animation ani, out SKColor BackLightColor, out SKColor BackDarkColor, out SKColor LampLightColor, out SKColor LampDarkColor, out SKColor LampColor)
        {
            #region Brightness
            var vBS = 0.2F;
            var vBE = -0.6F;
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
    }

    public class WhiteTheme : DvTheme
    {
        public override ThemeBrightness Brightness => ThemeBrightness.Light;

        public override float DownBrightness { get; set; } = -0.1F;
        public override float BorderBrightness { get; set; } = -0.35F;
        public override float GradientLight { get; set; } = 0.2F;
        public override float GradientDark { get; set; } = -0.1F;
        public override float Corner { get; set; } = 6F;
        public override byte OutShadowAlpha { get; set; } = 20;
        public override byte OutBevelAlpha { get; set; } = 120;
        public override byte InShadowAlpha { get; set; } = 40;
        public override float KeySpecialButtonBrightness => 0.2F;
        public override byte GradientLightAlpha => 50;
        public override byte GradientDarkAlpha => 20;
        public override float DataGridInputBright => 0.3F;
        public override float DataGridCheckBoxBright => 0.5F;
        public override float DataGridColumnBevelBright => 0.1F;
        public override float DataGridRowBevelBright => 0.1F;

        public override SKColor ForeColor => SKColors.Black;
        public override SKColor BackColor => Util.FromArgb(220, 220, 220);
        public override SKColor ButtonColor => Util.FromArgb(230, 230, 230);
        public override SKColor LabelColor => Util.FromArgb(240, 240, 240);
        public override SKColor InputColor => Util.FromArgb(255, 255, 255);
        public override SKColor CheckBoxColor => Util.FromArgb(240, 240, 240);
        public override SKColor LampOnColor => SKColors.Red;
        public override SKColor LampOffColor => Util.FromArgb(180, 180, 180);
        public override SKColor StepOnColor => SKColors.Red;
        public override SKColor StepOffColor => Util.FromArgb(230, 230, 230);
        public override SKColor ConcaveBoxColor => Util.FromArgb(180, 180, 180);
        public override SKColor KnobColor => Util.FromArgb(210, 210, 210);
        public override SKColor KnobCursorColor => SKColors.White;
        public override SKColor NeedleColor => SKColors.White;
        public override SKColor NeedlePointColor => SKColors.Red;
        public override SKColor GridColor => Util.FromArgb(90, 90, 90);
        public override SKColor PanelColor => Util.FromArgb(230, 230, 230);
        public override SKColor BorderPanelColor => Util.FromArgb(255, 255, 255);
        public override SKColor GroupBoxColor => Util.FromArgb(150, 150, 150);
        public override SKColor TabBackColor => Util.FromArgb(240, 240, 240);
        public override SKColor TabPageColor => Util.FromArgb(230, 230, 230);
        public override SKColor RowColor => Util.FromArgb(230, 230, 230);
        public override SKColor ColumnColor => Util.FromArgb(210, 215, 220);
        public override SKColor ListBackColor => Util.FromArgb(220, 220, 220);
        public override SKColor WindowTitleColor => Util.FromArgb(240, 240, 240);
        public override SKColor CalendarDaysColor => Util.FromArgb(230, 230, 230);
        public override SKColor CalendarWeeksColor => Util.FromArgb(220, 220, 220);
        public override SKColor CalendarMonthColor => Util.FromArgb(230, 230, 230);
        public override SKColor CalendarSelectColor => SKColors.Magenta;
        public override SKColor PointColor => SKColors.DeepSkyBlue;
        public override SKColor ScrollBarColor => Util.FromArgb(180, 180, 180);
        public override SKColor ScrollCursorOffColor => Util.FromArgb(255, 255, 255);
        public override SKColor ScrollCursorOnColor => SKColors.DeepSkyBlue;


        #region DrawBox
        public override void DrawBox(SKCanvas canvas, SKRect rect, SKColor boxColor, SKColor borderColor, RoundType round, BoxStyle style, float? Corner = null)
        {
            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                var corner = Corner ?? this.Corner;
                var _rt = Util.FromRect(rect.Left, rect.Top, rect.Width, rect.Height);
                var _rtIn = Util.FromRect(rect.Left, rect.Top, rect.Width, rect.Height); _rtIn.Inflate(-1, -1);
                var rt = new SKRoundRect(_rt, corner);
                var rtIn = new SKRoundRect(_rtIn, corner);

                #region Round
                switch (round)
                {
                    #region case RoundType.Rect:
                    case RoundType.Rect:
                        rt.SetNinePatch(_rt, 0, 0, 0, 0);
                        rtIn.SetNinePatch(_rtIn, 0, 0, 0, 0);
                        break;
                    #endregion
                    #region case RoundType.Round:
                    case RoundType.Round:
                        rt.SetNinePatch(_rt, corner, corner, corner, corner);
                        rtIn.SetNinePatch(_rtIn, corner, corner, corner, corner);
                        break;
                    #endregion
                    #region case RoundType.Round_L:
                    case RoundType.Round_L:
                        rt.SetNinePatch(_rt, corner, corner, 0, corner);
                        rtIn.SetNinePatch(_rtIn, corner, corner, 0, corner);
                        break;
                    #endregion
                    #region case RoundType.Round_R:
                    case RoundType.Round_R:
                        rt.SetNinePatch(_rt, 0, corner, corner, corner);
                        rtIn.SetNinePatch(_rtIn, 0, corner, corner, corner);
                        break;
                    #endregion
                    #region case RoundType.Round_T:
                    case RoundType.Round_T:
                        rt.SetNinePatch(_rt, corner, corner, corner, 0);
                        rtIn.SetNinePatch(_rtIn, corner, corner, corner, 0);
                        break;
                    #endregion
                    #region case RoundType.Round_B:
                    case RoundType.Round_B:
                        rt.SetNinePatch(_rt, corner, 0, corner, corner);
                        rtIn.SetNinePatch(_rtIn, corner, 0, corner, corner);
                        break;
                    #endregion
                    #region case RoundType.Round_LT:
                    case RoundType.Round_LT:
                        rt.SetNinePatch(_rt, corner, corner, 0, 0);
                        rtIn.SetNinePatch(_rtIn, corner, corner, 0, 0);
                        break;
                    #endregion
                    #region case RoundType.Round_RT:
                    case RoundType.Round_RT:
                        rt.SetNinePatch(_rt, 0, corner, corner, 0);
                        rtIn.SetNinePatch(_rtIn, 0, corner, corner, 0);
                        break;
                    #endregion
                    #region case RoundType.Round_LB:
                    case RoundType.Round_LB:
                        rt.SetNinePatch(_rt, corner, 0, 0, corner);
                        rtIn.SetNinePatch(_rtIn, corner, 0, 0, corner);
                        break;
                    #endregion
                    #region case RoundType.Round_RB:
                    case RoundType.Round_RB:
                        rt.SetNinePatch(_rt, 0, 0, corner, corner);
                        rtIn.SetNinePatch(_rtIn, 0, 0, corner, corner);
                        break;
                    #endregion
                    #region case RoundType.Ellipse:
                    case RoundType.Ellipse:
                        rt.SetOval(_rt);
                        rtIn.SetOval(_rtIn);
                        break;
                        #endregion
                }
                #endregion

                #region Style

                var fOut = (SKImageFilter)null;

                #region OutShadow / OutBevel
                if ((style & BoxStyle.OutShadow) == BoxStyle.OutShadow)
                {
                    fOut = SKImageFilter.CreateDropShadow(1, 1, 0, 0, Util.FromArgb(OutShadowAlpha, SKColors.Black));
                    p.ImageFilter = fOut;
                }
                else if ((style & BoxStyle.OutBevel) == BoxStyle.OutBevel)
                {
                    fOut = SKImageFilter.CreateDropShadow(0, 1, 0, 0, Util.FromArgb(OutBevelAlpha, SKColors.White));
                    p.ImageFilter = fOut;
                }
                #endregion
                #region Fill / Gradient
                if ((style & BoxStyle.Fill) == BoxStyle.Fill)
                {
                    if (boxColor.Alpha != 0)
                    {
                        #region Fill
                        p.Color = boxColor;
                        canvas.DrawRoundRect(rt, p);
                        #endregion
                    }
                }
                else
                {
                    #region Gradient Point
                    SKPoint? sp = null;
                    SKPoint? ep = null;
                    if ((style & BoxStyle.GradientV) == BoxStyle.GradientV)
                    {
                        sp = new SKPoint(_rt.Left, _rt.Top);
                        ep = new SKPoint(_rt.Left, _rt.Bottom);
                    }
                    else if ((style & BoxStyle.GradientV_R) == BoxStyle.GradientV_R)
                    {
                        ep = new SKPoint(_rt.Left, _rt.Top);
                        sp = new SKPoint(_rt.Left, _rt.Bottom);
                    }
                    else if ((style & BoxStyle.GradientH) == BoxStyle.GradientH)
                    {
                        sp = new SKPoint(_rt.Left, _rt.Top);
                        ep = new SKPoint(_rt.Right, _rt.Top);
                    }
                    else if ((style & BoxStyle.GradientH_R) == BoxStyle.GradientH_R)
                    {
                        ep = new SKPoint(_rt.Left, _rt.Top);
                        sp = new SKPoint(_rt.Right, _rt.Top);
                    }
                    else if ((style & BoxStyle.GradientLT) == BoxStyle.GradientLT)
                    {
                        sp = new SKPoint(_rt.Left, _rt.Top);
                        ep = new SKPoint(_rt.Right, _rt.Bottom);
                    }
                    else if ((style & BoxStyle.GradientLT_R) == BoxStyle.GradientLT_R)
                    {
                        ep = new SKPoint(_rt.Left, _rt.Top);
                        sp = new SKPoint(_rt.Right, _rt.Bottom);
                    }
                    else if ((style & BoxStyle.GradientRT) == BoxStyle.GradientRT)
                    {
                        sp = new SKPoint(_rt.Right, _rt.Top);
                        ep = new SKPoint(_rt.Left, _rt.Bottom);
                    }
                    else if ((style & BoxStyle.GradientRT_R) == BoxStyle.GradientRT_R)
                    {
                        ep = new SKPoint(_rt.Right, _rt.Top);
                        sp = new SKPoint(_rt.Left, _rt.Bottom);
                    }
                    #endregion
                    #region Gradient Fill
                    if (sp.HasValue && ep.HasValue)
                    {
                        using (var lg = SKShader.CreateLinearGradient(sp.Value, ep.Value,
                                                                new SKColor[] { boxColor.BrightnessTransmit(GradientLight), boxColor.BrightnessTransmit(GradientDark) },
                                                                new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                        {

                            p.Shader = lg;
                            canvas.DrawRoundRect(rt, p);
                            p.Shader = null;
                        }
                    }
                    #endregion
                }
                #endregion

                #region ImageFilter Free
                p.ImageFilter = null;
                if (fOut != null) fOut.Dispose();
                fOut = null;
                #endregion

                #region InBevel / InShadow
                if ((style & BoxStyle.InBevel) == BoxStyle.InBevel)
                {

                    using (var lg = SKShader.CreateLinearGradient(new SKPoint(_rtIn.Left, _rtIn.Top), new SKPoint(_rtIn.Left, _rtIn.Bottom),
                                                                new SKColor[] { GetInBevelColor(boxColor), Util.FromArgb(0, boxColor) },
                                                                new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                    {
                        p.Shader = lg;
                        p.IsStroke = true;
                        p.StrokeWidth = 1;
                        rtIn.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                        canvas.DrawRoundRect(rtIn, p);
                        p.Shader = null;
                    }
                }
                else if ((style & BoxStyle.InShadow) == BoxStyle.InShadow)
                {
                    using (var mf = SKMaskFilter.CreateBlur(SKBlurStyle.Normal, 1))
                    {
                        p.IsStroke = true;
                        p.StrokeWidth = 1;
                        p.Color = Util.FromArgb(InShadowAlpha, SKColors.Black);
                        p.MaskFilter = mf;
                        rtIn.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                        canvas.DrawRoundRect(rtIn, p);
                        p.MaskFilter = null;
                    }
                }
                #endregion

                #region Border
                if ((style & BoxStyle.Border) == BoxStyle.Border)
                {
                    p.StrokeWidth = 1;
                    p.IsStroke = true;
                    p.Color = borderColor;
                    rt.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                    canvas.DrawRoundRect(rt, p);
                }
                #endregion
                #endregion
            }
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
        #region GetInBevelColor
        public override SKColor GetInBevelColor(SKColor BaseColor)
        {
            return BaseColor.BrightnessTransmit(0.5F);
        }
        #endregion
        #region GetSwitchColors
        public override void GetSwitchColors(SKColor Color, out SKColor c1, out SKColor c2, out SKColor c3, out SKColor c4)
        {
            c1 = Color.BrightnessTransmit(0.05F);
            c2 = Color.BrightnessTransmit(0F);
            c3 = Color.BrightnessTransmit(-0.1F);
            c4 = Color.BrightnessTransmit(-0.2F);
        }
        #endregion
        #region GetLampColors
        public override void GetLampColors(SKColor BackColor, SKColor OnLampColor, SKColor OffLampColor, bool OnOff, bool Animation, Animation ani, out SKColor BackLightColor, out SKColor BackDarkColor, out SKColor LampLightColor, out SKColor LampDarkColor, out SKColor LampColor)
        {
            #region Brightness
            var vBS = 0.3F;
            var vBE = -0.3F;
            var vS = 0.2F;
            var vE = -0.2F;

            if (OnOff)
            {
                vS = 0.7F;
                vE = -0.1F;
            }
            #endregion

            var cM = OnOff ? OnLampColor : OffLampColor;
            if (Animation && ani.IsPlaying) cM = ani.Value(AnimationAccel.DCL, ani.Variable == "On" ? OffLampColor : OnLampColor, ani.Variable == "On" ? OnLampColor : OffLampColor);

            BackDarkColor = BackColor.BrightnessTransmit(vBS);
            BackLightColor = BackColor.BrightnessTransmit(vBE);
            LampLightColor = cM.BrightnessTransmit(vS);
            LampDarkColor = cM.BrightnessTransmit(vE);
            LampColor = cM;
        }
        #endregion
    }

    #region enum : BoxStyle
    public enum BoxStyle : int
    {
        Fill = 1,
        GradientV = 2, GradientV_R = 4, GradientH = 8, GradientH_R = 16, GradientLT = 32, GradientLT_R = 64, GradientRT = 128, GradientRT_R = 256,
        Border = 512,
        OutShadow = 1024, OutBevel = 2048,
        InShadow = 4096, InBevel = 8192,
    }
    #endregion
    #region enum : RoundType
    public enum RoundType
    {
        Rect,
        Round,
        Round_L, Round_R, Round_T, Round_B,
        Round_LT, Round_RT, Round_LB, Round_RB,
        Ellipse,
    }
    #endregion
    #region enum : ThemeBrightness
    public enum ThemeBrightness { Light, Dark }
    #endregion
}

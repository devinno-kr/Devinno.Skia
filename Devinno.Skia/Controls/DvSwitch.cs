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
    public class DvSwitch : DvControl
    {
        #region Properties
        public SKColor? SwitchColor { get; set; } = null;
        public SKColor? OnLampColor { get; set; } = null;
        public SKColor? OffLampColor { get; set; } = null;
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;

        public bool Animation => Design != null ? Design.Animation : false;
        #region OnOff
        private bool bOnOff = false;
        public bool OnOff
        {
            get => bOnOff;
            set
            {
                if (bOnOff != value)
                {
                    bOnOff = value;
                    OnOffChanged?.Invoke(this, null);

                    if (Animation)
                    {
                        ani.Stop();
                        ani.Start(DvDesign.ANI, OnOff ? "On" : "Off");
                    }
                }
            }
        }
        #endregion
        #region Text
        public string OnText { get; set; } = "ON";
        public string OffText { get; set; } = "OFF";
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #endregion

        #region Event
        public event EventHandler OnOffChanged;
        #endregion

        #region Member Variable
        private Animation ani = new Animation();
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            var ds = Design;
            var thm = Design?.Theme;
            if (ds != null && thm != null)
            {
                bounds((rtContent, rtSwitch, rtOn, rtOff, rtLamp, rtOnText, rtOffText, VC) =>
                {
                    #region Color
                    var SwitchColor = this.SwitchColor ?? thm.ButtonColor;
                    var OnLampColor = this.OnLampColor ?? thm.LampOnColor;
                    var OffLampColor = this.OffLampColor ?? thm.LampOffColor;
                    var BoxColor = this.BoxColor ?? thm.ConcaveBoxColor;
                    var BorderColor = thm.GetBorderColor(BoxColor, ParentContainer.GetBackColor());
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var Corner = thm.Corner;
                    var rtvBorder = new SKRoundRect(rtSwitch, Corner);
                    #endregion

                    thm.DrawBox(Canvas, rtContent, BoxColor, BorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel);

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, IsDither = DvDesign.DI, FilterQuality = DvDesign.FQ })
                    {
                        #region Fill
                        SKColor c1, c2, c3, c4;
                        thm.GetSwitchColors(SwitchColor, out c1, out c2, out c3, out c4);

                        var ca1 = !OnOff ? c2 : c4;
                        var ca2 = !OnOff ? c3 : c1;
                        var ca3 = !OnOff ? c1 : c3;
                        var ca4 = !OnOff ? c4 : c2;

                        var va1 = !OnOff ? 0 : 0;
                        var va2 = !OnOff ? 0.5F : 0.1F - VC;
                        var va3 = !OnOff ? 0.9F + VC : 0.5F;
                        var va4 = !OnOff ? 1 : 1;

                        if (Animation && ani.IsPlaying)
                        {
                            if (ani.Variable == "On")
                            {
                                ca1 = ani.Value(AnimationAccel.DCL, c2, c4);
                                ca2 = ani.Value(AnimationAccel.DCL, c3, c1);
                                ca3 = ani.Value(AnimationAccel.DCL, c1, c3);
                                ca4 = ani.Value(AnimationAccel.DCL, c4, c2);

                                va2 = ani.Value(AnimationAccel.DCL, 0.5F, 0.1F - VC);
                                va3 = ani.Value(AnimationAccel.DCL, 0.9F + VC, 0.5F);
                            }
                            else if (ani.Variable == "Off")
                            {
                                ca1 = ani.Value(AnimationAccel.DCL, c4, c2);
                                ca2 = ani.Value(AnimationAccel.DCL, c1, c3);
                                ca3 = ani.Value(AnimationAccel.DCL, c3, c1);
                                ca4 = ani.Value(AnimationAccel.DCL, c2, c4);

                                va2 = ani.Value(AnimationAccel.DCL, 0.1F - VC, 0.5F);
                                va3 = ani.Value(AnimationAccel.DCL, 0.5F, 0.9F + VC);
                            }
                        }

                        using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtSwitch.Left, rtSwitch.Top),
                                                                    new SKPoint(rtSwitch.Right, rtSwitch.Top),
                                                                    new SKColor[] { ca1, ca2, ca3, ca4 },
                                                                    new float[] { va1, va2, va3, va4 },
                                                                    SKShaderTileMode.Clamp))
                        {
                            p.Shader = lg;
                            Canvas.DrawRoundRect(new SKRoundRect(rtSwitch, Corner), p);
                            p.Shader = null;
                        }
                        #endregion
                        #region Bevel
                        {
                            var VLC = 0.1F - VC;
                            var VRC = 0.9F + VC;

                            var vLx = MathTool.Map(VLC, 0D, 1D, rtSwitch.Left, rtSwitch.Right) - (rtSwitch.Width * 0.0005);
                            var vRx = MathTool.Map(VRC, 0D, 1D, rtSwitch.Left, rtSwitch.Right) + (rtSwitch.Width * 0.0005);
                            var cpx = MathTool.CenterPoint(rtSwitch).X;
                            var rtL = Util.FromRect(rtOff.Left, rtOff.Top, rtOff.Width, rtOff.Height); rtL.Inflate(-1, -1);
                            var rtR = Util.FromRect(rtOn.Left, rtOn.Top, rtOn.Width, rtOn.Height); rtR.Inflate(-1, -1);
                            var rtLV = Util.FromRect(Convert.ToInt32(vLx), rtOn.Top, Convert.ToInt32(cpx - vLx), rtOn.Height);
                            var rtRV = Util.FromRect(Convert.ToInt32(cpx), rtOff.Top, Convert.ToInt32(vRx - cpx), rtOff.Height);

                            p.IsStroke = true;
                            p.StrokeWidth = 2;
                            var bc = thm.GetInBevelColor(c1);
                            if (OnOff)
                            {
                                #region Right
                                using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtR.Right + 1, rtR.Top),
                                                                   new SKPoint(rtR.Left, rtR.Bottom),
                                                                    new SKColor[] { bc, SKColors.Transparent },
                                                                   SKShaderTileMode.Clamp))
                                {
                                    var rtm = (Animation && ani.IsPlaying ? ani.Value(AnimationAccel.DCL, rtRV, rtR) : rtR);
                                    var rt = new SKRoundRect(rtm, Corner);
                                    rt.SetNinePatch(rtm, 0, Corner, Corner, Corner);

                                    var sp = Canvas.Save();

                                    p.Shader = lg;
                                    Canvas.DrawRoundRect(rt, p);
                                    p.Shader = null;

                                    Canvas.RestoreToCount(sp);
                                }
                                #endregion
                                #region Left
                                using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtLV.Left, rtLV.Top),
                                                                   new SKPoint(rtLV.Right, rtLV.Bottom),
                                                                    new SKColor[] { bc, SKColors.Transparent },
                                                                   SKShaderTileMode.Clamp))
                                {
                                    var rtm = (Animation && ani.IsPlaying ? ani.Value(AnimationAccel.DCL, rtL, rtLV) : rtLV);
                                    var rt = new SKRoundRect(rtm, Corner);
                                    rt.SetNinePatch(rtm, Corner, Corner, 0, Corner);
                                    rt.Inflate(-1, -1);

                                    var sp = Canvas.Save();

                                    p.Shader = lg;
                                    Canvas.DrawRoundRect(rt, p);
                                    p.Shader = null;

                                    Canvas.RestoreToCount(sp);
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
                                    var rtm = (Animation && ani.IsPlaying ? ani.Value(AnimationAccel.DCL, rtLV, rtL) : rtL);
                                    var rt = new SKRoundRect(rtm, Corner);
                                    rt.SetNinePatch(rtm, Corner, Corner, 0, Corner);

                                    var sp = Canvas.Save();

                                    p.Shader = lg;
                                    Canvas.DrawRoundRect(rt, p);
                                    p.Shader = null;

                                    Canvas.RestoreToCount(sp);
                                }
                                #endregion
                                #region Right
                                using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtRV.Right, rtRV.Top),
                                                                   new SKPoint(rtRV.Left, rtRV.Bottom),
                                                                    new SKColor[] { bc, SKColors.Transparent },
                                                                   SKShaderTileMode.Clamp))
                                {
                                    var rtm = (Animation && ani.IsPlaying ? ani.Value(AnimationAccel.DCL, rtR, rtRV) : rtRV);
                                    var rt = new SKRoundRect(rtm, Corner);
                                    rt.SetNinePatch(rtm, 0, Corner, Corner, Corner);
                                    rt.Inflate(-1, -1);

                                    var sp = Canvas.Save();

                                    p.Shader = lg;
                                    Canvas.DrawRoundRect(rt, p);
                                    p.Shader = null;

                                    Canvas.RestoreToCount(sp);
                                }
                                #endregion
                            }

                            p.StrokeWidth = 2;
                            p.Color = Util.FromArgb(30, SKColors.Black);
                            Canvas.DrawLine(rtSwitch.MidX, rtSwitch.Top, rtSwitch.MidX, rtSwitch.Bottom, p);
                        }
                        #endregion
                        #region Lamp
                        rtLamp.Offset(0, 1);
                        {
                            SKColor cBS, cBE, cS, cE, cM;
                            thm.GetLampColors(SwitchColor, OnLampColor, OffLampColor, OnOff, Animation, ani, out cBE, out cBS, out cS, out cE, out cM);

                            #region Back
                            p.IsStroke = false;
                            using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtLamp.Left, rtLamp.Top),
                                                                          new SKPoint(rtLamp.Left, rtLamp.Bottom),
                                                                          new SKColor[] { cBE, cBS },
                                                                          SKShaderTileMode.Clamp))
                            {
                                p.Shader = lg;
                                p.Color = SKColors.White;
                                Canvas.DrawCircle(MathTool.CenterPoint(rtLamp), rtLamp.Width / 2F, p);
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

                                Canvas.DrawCircle(rtLamp.MidX, rtLamp.MidY, rtLamp.Width / 2F, p);

                                p.Shader = null;
                            }

                            p.IsStroke = true;
                            p.StrokeWidth = 1;
                            p.Color = cM.BrightnessTransmit(thm.BorderBrightness);
                            Canvas.DrawCircle(rtLamp.MidX, rtLamp.MidY, rtLamp.Width / 2F, p);
                            #endregion
                        }
                        #endregion
                        #region Border
                        p.IsStroke = true;
                        p.StrokeWidth = 1;
                        p.Color = BorderColor;
                        rtvBorder.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                        Canvas.DrawRoundRect(rtvBorder, p);
                        #endregion

                        #region Text
                        Util.DrawText(Canvas, OnText, FontName, FontSize, DvFontStyle.Normal,
                                     OnOff ? ForeColor : ForeColor.BrightnessTransmit(-0.4F),
                                     rtOnText, DvContentAlignment.MiddleCenter, true);

                        Util.DrawText(Canvas, OffText, FontName, FontSize, DvFontStyle.Normal,
                                     !OnOff ? ForeColor : ForeColor.BrightnessTransmit(-0.4F),
                                     rtOffText, DvContentAlignment.MiddleCenter, true);
                        #endregion
                    }

                });
            }
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            bounds((rtContent, rtSwitch, rtOn, rtOff, rtLamp, rtOnText, rtOffText, VC) =>
            {
                if (CollisionTool.Check(rtOff, x, y)) OnOff = false;
                if (CollisionTool.Check(rtOn, x, y)) OnOff = true;
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, float> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
            var ng = Convert.ToInt32(rtContent.Height * 0.1F);

            var rtSwitch = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, rtContent.Height); rtSwitch.Inflate(-ng, -ng);
            var rtOff = Util.FromRect(rtSwitch.Left, rtSwitch.Top, rtSwitch.Width / 2, rtSwitch.Height);
            var rtOn = Util.FromRect(rtSwitch.Left + (rtSwitch.Width / 2), rtSwitch.Top, rtSwitch.Width / 2, rtSwitch.Height);

            var whLamp = Convert.ToInt32(rtSwitch.Height * 0.5F);
            var VC = 0.025F;
            var n = Convert.ToInt32(this.Width * (0.1 - VC)) / 2;
            var GAP = Animation && ani.IsPlaying ? ani.Value(AnimationAccel.DCL, (OnOff ? 6F : 8F), (OnOff ? 8F : 6F)) : (OnOff ? 8F : 6F);

            Util.TextIconBounds(OnText, FontName, FontSize, DvFontStyle.Normal,
                GAP, rtOn, new SKRect(0, 0, whLamp, whLamp),
                DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleCenter,
                (_rtLamp, _rtOnText) =>
                {
                    var rtLamp = _rtLamp;
                    var rtOnText = _rtOnText;
                    var rtOffText = Util.FromRect(rtOff.Left, rtOnText.Top, rtOff.Width, rtOnText.Height);

                    if (Animation && ani.IsPlaying)
                    {
                        if (ani.Variable == "On")
                        {
                            rtLamp.Offset(ani.Value(AnimationAccel.DCL, -n, 0), 0);
                            rtOnText.Offset(ani.Value(AnimationAccel.DCL, -n, 0), 0);
                            rtOffText.Offset(ani.Value(AnimationAccel.DCL, 0, n), 0);
                        }
                        else if (ani.Variable == "Off")
                        {
                            rtLamp.Offset(ani.Value(AnimationAccel.DCL, 0, -n), 0);
                            rtOnText.Offset(ani.Value(AnimationAccel.DCL, 0, -n), 0);
                            rtOffText.Offset(ani.Value(AnimationAccel.DCL, n, 0), 0);
                        }
                    }
                    else
                    {
                        if (!OnOff)
                        {
                            rtLamp.Offset(-n, 0);
                            rtOnText.Offset(-n, 0);
                            rtOffText.Offset(0, 0);
                        }
                        else
                        {
                            rtLamp.Offset(0, 0);
                            rtOnText.Offset(0, 0);
                            rtOffText.Offset(n, 0);
                        }
                    }
                    act(rtContent, rtSwitch, rtOn, rtOff, rtLamp, rtOnText, rtOffText, VC);
                });

        }
        #endregion
        #endregion
    }
}

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
    public class DvOnOff : DvControl
    {
        #region Properties
        public SKColor? CursorColor { get; set; } = null;
        public SKColor? OnBoxColor { get; set; } = null;
        public SKColor? OffBoxColor { get; set; } = null;
        public SKColor? OnTextColor { get; set; } = null;
        public SKColor? OffTextColor { get; set; } = null;

        public bool Animation => Design != null ? Design.Animation : false;
        public bool DrawText { get; set; } = true;

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
        bool bDown = false;

        private Animation ani = new Animation();
        #endregion

        #region Constructor
        public DvOnOff()
        {
            if (DvDesign.ActiveDesign != null) OnBoxColor = DvDesign.ActiveDesign.Theme.PointColor;
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtCursor, rtOnText, rtOffText, rtA1, rtA2, rtA3, onX, offX) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var CursorColor = this.CursorColor ?? thm.ButtonColor;
                    var OnBoxColor = this.OnBoxColor ?? thm.PointColor;
                    var OffBoxColor = this.OffBoxColor ?? thm.ConcaveBoxColor;
                    var OnTextColor = this.OnTextColor ?? thm.ForeColor;
                    var OffTextColor = this.OffBoxColor ?? Util.FromArgb(128, OnTextColor);
                    var BorderColor = thm.GetBorderColor(OffBoxColor, ParentContainer.GetBackColor());
                    var CursorBorderColor = (OnOff ? OnBoxColor : OffBoxColor).BrightnessTransmit(-0.5F);

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, IsDither = DvDesign.DI, FilterQuality = DvDesign.FQ })
                    {
                        if (bDown) CursorColor = CursorColor.BrightnessTransmit(0.5F);
                 
                        #region Box
                        thm.DrawBox(Canvas, rtContent, OnOff ? OnBoxColor : OffBoxColor, BorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel, Convert.ToInt32(rtContent.Height));
                        #endregion
                        #region Text
                        if (DrawText)
                        {
                            if (OnOff) Util.DrawText(Canvas, OnText, FontName, FontSize, DvFontStyle.Normal, OnTextColor, rtOnText, DvContentAlignment.MiddleCenter, true);
                            else Util.DrawText(Canvas, OffText, FontName, FontSize, DvFontStyle.Normal, OffTextColor, rtOffText, DvContentAlignment.MiddleCenter, true);
                        }
                        #endregion
                        #region Cursor
                        {
                            thm.DrawBox(Canvas, rtCursor, CursorColor, CursorBorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow, rtContent.Height / 2F);

                            var rt = new SKRoundRect(rtCursor, rtContent.Height / 2F);
                            using (var lg = SKShader.CreateLinearGradient(new SKPoint(rtCursor.Left, rtCursor.Top), new SKPoint(rtCursor.Left, rtCursor.Bottom),
                                                               new SKColor[] { Util.FromArgb(50, SKColors.White), Util.FromArgb(50, SKColors.Black) },
                                                               new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                            {
                                p.Shader = lg;
                                p.IsStroke = true;
                                p.StrokeWidth = 2;
                                rt.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                                Canvas.DrawRoundRect(rt, p);
                                p.Shader = null;
                            }
                        }
                        #endregion
                        #region Ach
                        {
                            var cc = CursorColor.BrightnessTransmit(-0.1F);
                            var cD = cc.BrightnessTransmit(-0.6F);
                            var cL = cc.BrightnessTransmit(0.3F);

                            var rt1 = Util.INT(rtA1);
                            var rt2 = Util.INT(rtA2);
                            var rt3 = Util.INT(rtA3);

                            p.IsStroke = false;
                            p.Color = cc;
                            Canvas.DrawRect(rt1, p);
                            Canvas.DrawRect(rt2, p);
                            Canvas.DrawRect(rt3, p);

                            p.IsStroke = true;
                            p.StrokeWidth = 1;

                            rt1.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                            rt2.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);
                            rt3.Inflate(-p.StrokeWidth / 2F, -p.StrokeWidth / 2F);

                            p.Color = cD;
                            Canvas.DrawLine(rt1.Left, rt1.Top, rt1.Left, rt1.Bottom, p); Canvas.DrawLine(rt1.Left, rt1.Top, rt1.Right, rt1.Top, p);
                            Canvas.DrawLine(rt2.Left, rt2.Top, rt2.Left, rt2.Bottom, p); Canvas.DrawLine(rt2.Left, rt2.Top, rt2.Right, rt2.Top, p);
                            Canvas.DrawLine(rt3.Left, rt3.Top, rt3.Left, rt3.Bottom, p); Canvas.DrawLine(rt3.Left, rt3.Top, rt3.Right, rt3.Top, p);

                            p.Color = cL;
                            Canvas.DrawLine(rt1.Right, rt1.Top, rt1.Right, rt1.Bottom, p); Canvas.DrawLine(rt1.Left, rt1.Bottom, rt1.Right, rt1.Bottom, p);
                            Canvas.DrawLine(rt2.Right, rt2.Top, rt2.Right, rt2.Bottom, p); Canvas.DrawLine(rt2.Left, rt2.Bottom, rt2.Right, rt2.Bottom, p);
                            Canvas.DrawLine(rt3.Right, rt3.Top, rt3.Right, rt3.Bottom, p); Canvas.DrawLine(rt3.Left, rt3.Bottom, rt3.Right, rt3.Bottom, p);
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
            if (Design != null)
            {
                bounds((rtContent, rtCursor, rtOnText, rtOffText, rtC1, rtC2, rtC3, onX, offX) =>
                {
                    if (CollisionTool.Check(rtCursor, x, y)) bDown = true;
                    else if (CollisionTool.Check(rtOnText, x, y) || CollisionTool.Check(rtOffText, x, y))
                    {
                        OnOff = !OnOff;

                        if (Animation)
                        {
                            ani.Stop();
                            ani.Start(DvDesign.ANI, OnOff ? "On" : "Off");
                        }
                    }
                });
            }
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
        #region OnMouseMove
        protected override void OnMouseMove(int x, int y)
        {
            if (Design != null)
            {
                bounds((rtContent, rtCursor, rtOnText, rtOffText, rtC1, rtC2, rtC3, onX, offX) =>
                {
                    if (bDown)
                    {
                        OnOff = x > rtContent.MidX;
                    }
                });
            }
            base.OnMouseMove(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, float, float> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);

            var w = Convert.ToInt32(rtContent.Width * 0.6);
            var ng = Convert.ToInt32(rtContent.Height * 0.1F);
            var onX = (rtContent.Right - ng - w);
            var offX = (rtContent.Left + ng);

            var rtCursor = Util.FromRect(rtContent.Left, rtContent.Top, w, rtContent.Height); rtCursor.Inflate(-ng, -ng);
            var rtOnText = new SKRect(rtContent.Left, rtContent.Top, onX, rtContent.Bottom);
            var rtOffText = new SKRect(offX + w, rtContent.Top, rtContent.Right, rtContent.Bottom);

            if (Animation && ani.IsPlaying)
            {
                var v = ani.Value(AnimationAccel.DCL, OnOff ? offX : onX, OnOff ? onX : offX);
                rtCursor.Left = Convert.ToSingle(MathTool.Constrain(v, offX, onX));
                rtCursor.Right = rtCursor.Left + w;
            }
            else
            {
                var v = (OnOff ? onX : offX);
                rtCursor.Left = Convert.ToSingle(MathTool.Constrain(v, offX, onX));
                rtCursor.Right = rtCursor.Left + w;
            }

            var rg = rtCursor.Height / 3 / 3;
            var rtCursorAch = Util.MakeRectangleAlign(rtCursor, new SKSize(rg * 3 + 4, rg * 3 + 4), DvContentAlignment.MiddleCenter);
            var rtCursorAch1 = Util.FromRect(rtCursorAch.Left, rtCursorAch.Top, rg, rtCursorAch.Height);
            var rtCursorAch2 = Util.FromRect(rtCursorAch.Left + rg + 2, rtCursorAch.Top, rg, rtCursorAch.Height);
            var rtCursorAch3 = Util.FromRect(rtCursorAch.Left + rg + 2 + rg + 2, rtCursorAch.Top, rg, rtCursorAch.Height);

            act(rtContent, rtCursor, rtOnText, rtOffText, rtCursorAch1, rtCursorAch2, rtCursorAch3, onX, offX);
        }
        #endregion
        #endregion
    }
}

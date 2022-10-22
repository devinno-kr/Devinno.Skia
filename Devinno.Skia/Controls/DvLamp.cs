using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
using Devinno.Skia.Theme;
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
    public class DvLamp : DvControl
    {
        #region Properties
        public SKColor? OnLampColor { get; set; } = null;
        public SKColor? OffLampColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;

        public DvLampStyle LampStyle { get; set; } = DvLampStyle.CIRCLE;
        public DvTextIconAlignment LampAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        public int LampSize { get; set; } = 24;
        public DvContentAlignment ContentAlignment { get; set; } = DvContentAlignment.MiddleCenter;

        public bool Animation => Design != null ? Design.Animation : false;

        #region OnOff
        private bool bOnOff = false;
        public bool OnOff
        {
            get { return bOnOff; }
            set
            {
                if (bOnOff != value)
                {
                    bOnOff = value;
                    
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
        public string Text { get; set; } = "Text";
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #endregion

        #region Member Variable
        private Animation ani = new Animation();
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtLamp, rtText) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var OnLampColor = this.OnLampColor ?? thm.LampOnColor;
                    var OffLampColor = this.OffLampColor ?? thm.LampOffColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var BorderColor =  thm.GetBorderColor(BackColor, BackColor);
                    var Corner = thm.Corner;

                    rtLamp.Offset(0, 1);
                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        SKColor cBS, cBE, cS, cE, cM;
                        thm.GetLampColors(BackColor, OnLampColor, OffLampColor, OnOff, Animation, ani, out cBE, out cBS, out cS, out cE, out cM);

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

                    Util.DrawText(Canvas, Text, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtText, DvContentAlignment.MiddleCenter, true);
                }
            });

            base.OnDraw(Canvas);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect> act)
        {
            var INF = LampSize / 8;
            var GAP = 8;

            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);

            Util.TextIconBounds(Text, FontName, FontSize, DvFontStyle.Normal, GAP, rtContent, Util.FromRect(0, 0, LampSize, LampSize), LampAlignment, ContentAlignment, (rtFA, rtTX) =>
            {
                act(rtContent, rtFA, rtTX);

            });
        }
        #endregion
        #endregion
    }

    #region enum : DvLampStyle
    public enum DvLampStyle { CIRCLE = 0, RECT = 1 }
    #endregion
}

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
using System.Threading;

namespace Devinno.Skia.Controls
{
    public class DvNumberBox : DvControl
    {
        #region Const 
        const int CHKTM = 500;
        #endregion

        #region Properties
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? ValueBoxColor { get; set; } = null;
        public SKColor? ButtonColor { get; set; } = null;

        public DvNumberBoxStyle Style { get; set; } = DvNumberBoxStyle.Right;

        #region public double Value { get; set; } = 0D;
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
        public double Minimum { get; set; } = 0D;
        public double Maximum { get; set; } = 100D;
        public double Tick { get; set; } = 1D;

        public string FormatString { get; set; } = "0";
        public int ButtonWidth { get; set; } = 60;

        #region Text
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #endregion

        #region Member Variable
        int maxinterval = 250, mininterval = 10;
        
        bool bPlusDown = false;
        bool bMinusDown = false;
        bool bValueDown = false;
        DateTime dtMinusDown;
        DateTime dtPlusDown;
        #endregion

        #region Event
        public event EventHandler ValueChanged;
        public event EventHandler ValueClick;
        public event EventHandler ValueDoubleClick;
        #endregion

        #region Constructor
        public DvNumberBox()
        {
           
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtPlus, rtMinus, rtValue) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var ValueBoxColor = this.ValueBoxColor ?? thm.LabelColor;
                    var ButtonColor = this.ButtonColor ?? thm.ButtonColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BorderColor = thm.GetBorderColor(ButtonColor, ParentContainer.GetBackColor());

                    var Corner = thm.Corner;

                    #region Bounds
                    var rtTxt = rtValue;
                    var rtP = rtPlus;
                    var rtM = rtMinus;
                    #endregion

                    #region Draw
                    switch (Style)
                    {
                        case DvNumberBoxStyle.LeftRight:
                            {
                                string STRING_UP = "fa-plus";
                                string STRING_DOWN = "fa-minus";

                                var isz = Convert.ToInt32(Math.Max(Math.Min(rtM.Height, rtM.Width) / 2F, 1));
                               
                                #region Plus
                                if (!bPlusDown)
                                {
                                    var ico = STRING_UP;
                                    var icoSize = isz;
                                    var cv = ButtonColor;
                                    var ct = ForeColor;
                                    thm.DrawBox(Canvas, rtP, cv, BorderColor, RoundType.Round_R, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow);
                                    Util.DrawIconFA(Canvas, ico, icoSize, ct, rtP, DvContentAlignment.MiddleCenter, true);
                                }
                                else
                                {
                                    var ico = STRING_UP;
                                    var icoSize = isz;
                                    var cv = ButtonColor.BrightnessTransmit(thm.DownBrightness);
                                    var ct = ForeColor.BrightnessTransmit(thm.DownBrightness);
                                    thm.DrawBox(Canvas, rtP, cv, BorderColor, RoundType.Round_R, BoxStyle.Border | BoxStyle.GradientV_R | BoxStyle.InShadow);
                                    rtP.Offset(0, 1);
                                    Util.DrawIconFA(Canvas, ico, icoSize, ct, rtP, DvContentAlignment.MiddleCenter, true);
                                }
                                #endregion
                                #region Minus
                                if (!bMinusDown)
                                {
                                    var ico = STRING_DOWN;
                                    var icoSize = isz;
                                    var cv = ButtonColor;
                                    var ct = ForeColor;
                                    thm.DrawBox(Canvas, rtM, cv, BorderColor, RoundType.Round_L, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow);
                                    Util.DrawIconFA(Canvas, ico, icoSize, ct, rtM, DvContentAlignment.MiddleCenter, true);
                                }
                                else
                                {
                                    var ico = STRING_DOWN;
                                    var icoSize = isz; 
                                    var cv = ButtonColor.BrightnessTransmit(thm.DownBrightness);
                                    var ct = ForeColor.BrightnessTransmit(thm.DownBrightness);
                                    thm.DrawBox(Canvas, rtM, cv, BorderColor, RoundType.Round_L, BoxStyle.Border | BoxStyle.GradientV_R | BoxStyle.InShadow);
                                    rtM.Offset(0, 1);
                                    Util.DrawIconFA(Canvas, ico, icoSize, ct, rtM, DvContentAlignment.MiddleCenter, true);
                                }
                                #endregion
                                #region Back
                                thm.DrawBox(Canvas, rtTxt, ValueBoxColor, BorderColor, RoundType.Rect, BoxStyle.Fill | BoxStyle.OutShadow);
                                #endregion
                            }
                            break;

                        case DvNumberBoxStyle.Right:
                            {
                                string STRING_UP = "fa-chevron-up";
                                string STRING_DOWN = "fa-chevron-down";

                                var isz = Convert.ToInt32(Math.Max(Math.Min(rtM.Height, rtM.Width) / 2F, 1));
                               
                                #region Plus
                                if (!bPlusDown)
                                {
                                    var ico = STRING_UP;
                                    var icoSize = isz;
                                    var cv = ButtonColor;
                                    var ct = ForeColor;
                                    thm.DrawBox(Canvas, rtP, cv, BorderColor, RoundType.Round_RT, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow);
                                    Util.DrawIconFA(Canvas, ico, icoSize, ct, rtP, DvContentAlignment.MiddleCenter, true);
                                }
                                else
                                {
                                    var ico = STRING_UP;
                                    var icoSize = isz;
                                    var cv = ButtonColor.BrightnessTransmit(thm.DownBrightness);
                                    var ct = ForeColor.BrightnessTransmit(thm.DownBrightness);
                                    thm.DrawBox(Canvas, rtP, cv, BorderColor, RoundType.Round_RT, BoxStyle.Border | BoxStyle.GradientV_R | BoxStyle.InShadow);
                                    rtP.Offset(0, 1);
                                    Util.DrawIconFA(Canvas, ico, icoSize, ct, rtP, DvContentAlignment.MiddleCenter, true);
                                }
                                #endregion
                                #region Minus
                                if (!bMinusDown)
                                {
                                    var ico = STRING_DOWN;
                                    var icoSize = isz;
                                    var cv = ButtonColor;
                                    var ct = ForeColor;
                                    thm.DrawBox(Canvas, rtM, cv, BorderColor, RoundType.Round_RB, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow);
                                    Util.DrawIconFA(Canvas, ico, icoSize, ct, rtM, DvContentAlignment.MiddleCenter, true);
                                }
                                else
                                {
                                    var ico = STRING_DOWN;
                                    var icoSize = isz;
                                    var cv = ButtonColor.BrightnessTransmit(thm.DownBrightness);
                                    var ct = ForeColor.BrightnessTransmit(thm.DownBrightness);
                                    thm.DrawBox(Canvas, rtM, cv, BorderColor, RoundType.Round_RB, BoxStyle.Border | BoxStyle.GradientV_R | BoxStyle.InShadow);
                                    rtM.Offset(0, 1);
                                    Util.DrawIconFA(Canvas, ico, icoSize, ct, rtM, DvContentAlignment.MiddleCenter, true);
                                }
                                #endregion
                                #region Back
                                thm.DrawBox(Canvas, rtTxt, ValueBoxColor, BorderColor, RoundType.Round_L, BoxStyle.Fill | BoxStyle.OutShadow);
                                #endregion
                            }
                            break;

                        case DvNumberBoxStyle.UpDown:
                            {
                                string STRING_UP = "fa-chevron-up";
                                string STRING_DOWN = "fa-chevron-down";

                                var isz = Convert.ToInt32(Math.Max(Math.Min(rtM.Height, rtM.Width) / 2F, 1));
                                
                                #region Plus
                                if (!bPlusDown)
                                {
                                    var ico = STRING_UP;
                                    var icoSize = isz;
                                    var cv = ButtonColor;
                                    var ct = ForeColor;
                                    thm.DrawBox(Canvas, rtP, cv, BorderColor, RoundType.Round_T, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow);
                                    Util.DrawIconFA(Canvas, ico, icoSize, ct, rtP, DvContentAlignment.MiddleCenter, true);
                                }
                                else
                                {
                                    var ico = STRING_UP;
                                    var icoSize = isz;
                                    var cv = ButtonColor.BrightnessTransmit(thm.DownBrightness);
                                    var ct = ForeColor.BrightnessTransmit(thm.DownBrightness);
                                    thm.DrawBox(Canvas, rtP, cv, BorderColor, RoundType.Round_T, BoxStyle.Border | BoxStyle.GradientV_R | BoxStyle.InShadow);
                                    rtP.Offset(0, 1);
                                    Util.DrawIconFA(Canvas, ico, icoSize, ct, rtP, DvContentAlignment.MiddleCenter, true);
                                }
                                #endregion
                                #region Minus
                                if (!bMinusDown)
                                {
                                    var ico = STRING_DOWN;
                                    var icoSize = isz;
                                    var cv = ButtonColor;
                                    var ct = ForeColor;
                                    thm.DrawBox(Canvas, rtM, cv, BorderColor, RoundType.Round_B, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow);
                                    Util.DrawIconFA(Canvas, ico, icoSize, ct, rtM, DvContentAlignment.MiddleCenter, true);
                                }
                                else
                                {
                                    var ico = STRING_DOWN;
                                    var icoSize = isz;
                                    var cv = ButtonColor.BrightnessTransmit(thm.DownBrightness);
                                    var ct = ForeColor.BrightnessTransmit(thm.DownBrightness);
                                    thm.DrawBox(Canvas, rtM, cv, BorderColor, RoundType.Round_B, BoxStyle.Border | BoxStyle.GradientV_R | BoxStyle.InShadow | BoxStyle.OutShadow);
                                    rtM.Offset(0, 1);
                                    Util.DrawIconFA(Canvas, ico, icoSize, ct, rtM, DvContentAlignment.MiddleCenter, true);
                                }
                                #endregion
                                #region Back
                                thm.DrawBox(Canvas, rtTxt, ValueBoxColor, BorderColor, RoundType.Rect, BoxStyle.Fill | BoxStyle.OutShadow);
                                #endregion
                            }
                            break;

                    }
                    #endregion

                    thm.DrawBox(Canvas, rtContent, ValueBoxColor, BorderColor, RoundType.Round, BoxStyle.Border);

                    #region Text
                    Util.DrawText(Canvas, Value.ToString(FormatString), FontName, FontSize, DvFontStyle.Normal, ForeColor, rtValue, DvContentAlignment.MiddleCenter, true);
                    #endregion
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            bounds((rtContent, rtPlus, rtMinus, rtValue) =>
            {
                if (CollisionTool.Check(rtMinus, x, y))
                {
                    bMinusDown = true; dtMinusDown = DateTime.Now;
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        while (bMinusDown)
                        {
                            var tmm = (DateTime.Now - dtMinusDown).TotalMilliseconds;

                            if (bMinusDown && tmm >= CHKTM)
                            {
                                var delay = (int)MathTool.Constrain(MathTool.Map(tmm, 2000, CHKTM, mininterval, maxinterval), mininterval, maxinterval);
                                { Value = MathTool.Constrain(Value - Tick, Minimum, Maximum); };
                                Thread.Sleep(delay);
                            }
                            else Thread.Sleep(100);
                        }
                    });
                }
                if (CollisionTool.Check(rtPlus, x, y))
                {
                    bPlusDown = true; dtPlusDown = DateTime.Now;
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        while (bPlusDown | bMinusDown)
                        {
                            var tmp = (DateTime.Now - dtPlusDown).TotalMilliseconds;

                            if (bPlusDown && tmp >= CHKTM)
                            {
                                var delay = (int)MathTool.Constrain(MathTool.Map(tmp, 2000, CHKTM, mininterval, maxinterval), mininterval, maxinterval);
                                { Value = MathTool.Constrain(Value + Tick, Minimum, Maximum); };
                                Thread.Sleep(delay);
                            }
                            else Thread.Sleep(100);
                        }
                    });
                }
                if (CollisionTool.Check(rtValue, x, y)) { ValueClick?.Invoke(this, new EventArgs()); }

            });

            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            bounds((rtContent, rtPlus, rtMinus, rtValue) =>
            {
                if (bMinusDown)
                {
                    bMinusDown = false;
                    if ((DateTime.Now - dtMinusDown).TotalMilliseconds < CHKTM) Value = MathTool.Constrain(Value - Tick, Minimum, Maximum);
                }
                if (bPlusDown)
                {
                    bPlusDown = false;
                    if ((DateTime.Now - dtPlusDown).TotalMilliseconds < CHKTM) Value = MathTool.Constrain(Value + Tick, Minimum, Maximum);
                }
                if (bValueDown)
                {
                    bValueDown = false;
                    if (CollisionTool.Check(rtValue, x, y)) ValueClick?.Invoke(this, null);
                }
            });

            base.OnMouseUp(x, y);
        }
        #endregion
        #region OnMouseDoubleClick
        protected override void OnMouseDoubleClick(int x, int y)
        {
            bounds((rtContent, rtPlus, rtMinus, rtValue) =>
            {
                if (CollisionTool.Check(rtValue, x, y)) { ValueDoubleClick?.Invoke(this, new EventArgs()); }

            });
            base.OnMouseDoubleClick(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
            var rtPlus = new SKRect();
            var rtMinus = new SKRect();
            var rtValue = new SKRect();

            switch (Style)
            {
                case DvNumberBoxStyle.LeftRight:
                    {
                        rtMinus = Util.FromRect(rtContent.Left, rtContent.Top, ButtonWidth, rtContent.Height);
                        rtPlus = Util.FromRect(rtContent.Right - ButtonWidth, rtContent.Top, ButtonWidth, rtContent.Height);
                        rtValue = Util.FromRect(rtContent.Left + ButtonWidth, rtContent.Top, rtContent.Width - (ButtonWidth * 2), rtContent.Height);
                    }
                    break;
                case DvNumberBoxStyle.Right:
                    {
                        var rtR = Util.FromRect(rtContent.Right - ButtonWidth, rtContent.Top, ButtonWidth, rtContent.Height);

                        rtPlus = Util.FromRect(rtR.Left, rtContent.Top, rtR.Width, rtR.Height / 2);
                        rtMinus = Util.FromRect(rtR.Left, rtPlus.Bottom, rtR.Width, rtContent.Bottom - rtPlus.Bottom);
                        rtValue = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width - ButtonWidth, rtContent.Height);
                    }
                    break;
                case DvNumberBoxStyle.UpDown:
                    {
                        int h = ButtonWidth;
                        rtPlus = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, h);
                        rtMinus = Util.FromRect(rtContent.Left, rtContent.Bottom - h, rtContent.Width, h);
                        rtValue = Util.FromRect(rtContent.Left, rtPlus.Bottom, rtContent.Width, rtMinus.Top - rtPlus.Bottom);
                    }
                    break;
            }

            act(rtContent, rtPlus, rtMinus, rtValue);
        }
        #endregion
        #endregion

    }

    #region enum : DvNumberBoxStyle
    public enum DvNumberBoxStyle { UpDown, LeftRight, Right };
    #endregion
}

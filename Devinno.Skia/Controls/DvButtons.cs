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
    public class DvButtons : DvControl
    {
        #region Properties
        public SKColor? OnButtonColor { get; set; } = null;
        public SKColor? OffButtonColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;

        public DvDirectionHV Direction { get; set; } = DvDirectionHV.Horizon;

        public List<ButtonInfo> Buttons { get; private set; } = new List<ButtonInfo>();

        public bool SelectorMode { get; set; } = false;

        public bool Gradient { get; set; } = true;

        public bool Animation => Design != null ? Design.Animation : false;
        #endregion

        #region Events
        public event EventHandler<ButtonsClickventArgs> ButtonClick;
        public event EventHandler<ButtonsSelectedventArgs> SelectedChanged;
        #endregion

        #region Member Variable
        private Animation ani = new Animation();
        private ButtonInfo prevBtn = null;
        private ButtonInfo nowBtn = null;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, lsBtns) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var OnButtonColor = this.OnButtonColor ?? thm.PointColor;
                    var OffButtonColor = this.OffButtonColor ?? thm.ButtonColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BorderColor = thm.GetBorderColor(OffButtonColor, ParentContainer.GetBackColor());

                    var Corner = thm.Corner;

                    foreach (var btn in lsBtns)
                    {
                        using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                        {
                            var rt = btn.Bounds;
                            var bDown = btn.Button.DownState;
                            var ButtonColor = btn.Button.Checked ? OnButtonColor : OffButtonColor;
                           
                            if (Animation && ani.IsPlaying)
                            {
                                if (!btn.Button.DownState)
                                {
                                    if (btn.Button == nowBtn) ButtonColor = ani.Value(AnimationAccel.Linear, OffButtonColor.BrightnessTransmit(thm.DownBrightness), OnButtonColor);
                                    else if (btn.Button == prevBtn) ButtonColor = ani.Value(AnimationAccel.Linear, OnButtonColor, OffButtonColor);
                                }
                            }
                            
                            var cF = bDown ? ButtonColor.BrightnessTransmit(thm.DownBrightness) : ButtonColor;
                            var cB = bDown ? BorderColor.BrightnessTransmit(thm.DownBrightness) : BorderColor;
                            var cT = bDown ? ForeColor.BrightnessTransmit(thm.DownBrightness) : ForeColor;

                            cB = cF.BrightnessTransmit(thm.BorderBrightness);

                            var round = RoundType.Rect;
                            var gradient = Gradient ? (Direction == DvDirectionHV.Horizon ? BoxStyle.GradientV : BoxStyle.GradientH) : BoxStyle.Fill;

                            if (btn.Index == 0) round = Direction == DvDirectionHV.Horizon ? RoundType.Round_L : RoundType.Round_T;
                            else if(btn.Index == lsBtns.Count-1) round = Direction == DvDirectionHV.Horizon ? RoundType.Round_R : RoundType.Round_B;

                            if (!bDown) thm.DrawBox(Canvas, rt, cF, cB, round, gradient | BoxStyle.InBevel | BoxStyle.OutShadow | BoxStyle.Border);
                            else thm.DrawBox(Canvas, rt, cF, cB, round, BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel | BoxStyle.Border);
                          
                            
                            var rtText = btn.TextBounds;
                            if (bDown) rtText.Offset(0, 1);
                            Util.DrawTextIcon(Canvas, 
                                              btn.Button.Text, btn.Button.FontName, btn.Button.FontSize, DvFontStyle.Normal, 
                                              btn.Button.IconGap, btn.Button.IconString, btn.Button.IconSize, 
                                              cT, rtText, btn.Button.IconAlignment, DvContentAlignment.MiddleCenter, true, thm.OutShadowAlpha);
                        
                        }
                    }
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            bounds((rtContent, lsBtns) =>
            {
                foreach (var btn in lsBtns)
                    if (CollisionTool.Check(btn.Bounds, x, y)) 
                        btn.Button.DownState = true;
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            bounds((rtContent, lsBtns) =>
            {
                ButtonInfo v = null;
                foreach (var btn in lsBtns)
                {
                    if (btn.Button.DownState)
                    {
                        btn.Button.DownState = false;
                        if (CollisionTool.Check(btn.Bounds, x, y))
                        {
                            v = btn.Button;
                            ButtonClick?.Invoke(this, new ButtonsClickventArgs(btn.Button));
                            if (SelectorMode)
                            {
                                var pbtn = lsBtns.Where(x => x.Button.Checked).Select(x => x.Button).FirstOrDefault();
                                if (pbtn != v)
                                {
                                    if (pbtn != null) pbtn.Checked = false;
                                    btn.Button.Checked = true;
                                    SelectedChanged?.Invoke(this, new ButtonsSelectedventArgs(btn.Button));

                                    if (Animation)
                                    {
                                        prevBtn = pbtn;
                                        nowBtn = btn.Button;

                                        ani.Stop();
                                        ani.Start(DvDesign.ANI2, null);
                                    }
                                }
                            }
                        }
                    }
                }
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, List<Btn>> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);

            if (Direction == DvDirectionHV.Horizon)
            {
                var w = Convert.ToInt32(rtContent.Width / (float)Buttons.Count);

                var ls = new List<Btn>();
                for (int i = 0; i < Buttons.Count; i++)
                {
                    var nw = w;
                    if (i == Buttons.Count - 1)
                        nw = Convert.ToInt32(rtContent.Width - (w * i));
                    var rt = Util.FromRect(rtContent.Left + (w * i), rtContent.Top, nw, rtContent.Height);
                    var rtText = Util.FromRect(rt.Left, rt.Top, rt.Width, rt.Height - 1);

                    var btn = Buttons[i];
                    ls.Add(new Btn() { Button = btn, Bounds = rt, TextBounds = rtText, Index = i });
                }

                act(rtContent, ls);
            }
            else if (Direction == DvDirectionHV.Vertical)
            {
                var h = Convert.ToInt32(rtContent.Height / (float)Buttons.Count);

                var ls = new List<Btn>();
                for (int i = 0; i < Buttons.Count; i++)
                {
                    var nh = h;
                    if (i == Buttons.Count - 1)
                        nh = Convert.ToInt32(rtContent.Height - (h * i));
                    var rt = Util.FromRect(rtContent.Left, rtContent.Top + (h * i), rtContent.Width, nh);
                    var rtText = Util.FromRect(rt.Left, rt.Top, rt.Width, rt.Height - 1);

                    var btn = Buttons[i];
                    ls.Add(new Btn() { Button = btn, Bounds = rt, TextBounds = rtText, Index = i });
                }

                act(rtContent, ls);
            }
            
        }
        #endregion
        #endregion
    }


    #region class : ButtonInfo
    public class ButtonInfo
    {
        #region Properties
        #region Name
        public string Name { get; set; }
        #endregion
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

        public bool Checked { get; set; }
        internal bool DownState { get; set; }
        #endregion


        public ButtonInfo(string Name)
        {
            this.Name = Name;
        }
    }

    internal class Btn
    {
        internal ButtonInfo Button { get; set; }
        internal SKRect Bounds { get; set; }
        internal SKRect TextBounds { get; set; }
        internal int Index { get; set; }
    }
    #endregion
}

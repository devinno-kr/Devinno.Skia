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
    public class DvButtons : Design.DvControl
    {
        #region Const
        bool Tight = false;
        #endregion

        #region Properties
        #region Color
        public SKColor? ButtonColor { get; set; } = null;
        public SKColor? CheckedButtonColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        #endregion

        #region Buttons
        public List<ButtonInfo> Buttons { get; private set; } = new List<ButtonInfo>();
        #endregion

        #region Round
        public DvRoundType? Round { get; set; } = null;
        #endregion

        #region Directions
        public DvDirectionHV Direction { get; set; } = DvDirectionHV.Horizon;
        #endregion

        #region Gradient
        public bool Gradient { get; set; } = false;
        #endregion

        #region Clickable
        public bool Clickable { get; set; } = true;
        #endregion

        #region SelectorMode
        public bool SelectorMode { get; set; } = false;
        #endregion

        #region Animation
        public bool Animation => Design != null ? Design.Animation : false;
        #endregion
        #endregion

        #region Member Variable
        private Animation ani = new Animation();
        private ButtonInfo prevBtn = null;
        private ButtonInfo nowBtn = null;
        #endregion

        #region Event
        public event EventHandler<ButtonsClickedEventArgs> ButtonClick;
        public event EventHandler<ButtonsClickedEventArgs> ButtonDown;
        public event EventHandler<ButtonsClickedEventArgs> ButtonUp;
        public event EventHandler<ButtonsSelectedventArgs> SelectedChanged;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            var ds = Design;
            var thm = Design?.Theme;
            if (ds != null && thm != null)
            {
                Areas((rtContent, rtButtons) =>
                {
                    #region var
                    var cButton = ButtonColor ?? thm.ButtonColor;
                    var cCheckedButton = CheckedButtonColor ?? thm.PointColor;
                    var cFore = ForeColor ?? thm.ForeColor;
                    var cBack = ParentContainer.GetBackColor();
                    var round = Round ?? DvRoundType.All;
                    var rnds = Util.Rounds(Direction, round, Buttons.Count);
                    var cBorder = thm.GetBorderColor(cButton, ParentContainer.GetBackColor());
                    #endregion

                    if (Buttons.Count > 0)
                    {
                        for (int i = 0; i < Buttons.Count; i++)
                        {
                            #region var
                            var rt = rtButtons[i];
                            var btn = Buttons[i];
                            var rnd = rnds[i];
                            var cBTN = btn.Checked ? cCheckedButton : cButton;
                            var cBOR = thm.GetBorderColor(cBTN, ParentContainer.GetBackColor());
                            #endregion
                            #region Animation Color
                            if (Animation && ani.IsPlaying)
                            {
                                if (!btn.ButtonDownState)
                                {
                                    if (btn == nowBtn) cBTN = ani.Value(AnimationAccel.Linear, cButton.BrightnessTransmit(thm.DownBrightness), cCheckedButton);
                                    else if (btn == prevBtn) cBTN = ani.Value(AnimationAccel.Linear, cCheckedButton, cButton);
                                }
                            }
                            #endregion

                            thm.DrawButton(Canvas,
                                rt,
                                cBTN, cBOR, cFore, cBack,
                                btn,
                                rnd, Gradient, true,
                                DvContentAlignment.MiddleCenter,
                                btn.ButtonDownState);
                        }
                    }
                    else
                    {
                        var cBTN = cButton;
                        var cBOR = thm.GetBorderColor(cBTN, ParentContainer.GetBackColor());

                        thm.DrawButton(Canvas,
                            rtContent,
                            cBTN, cBOR, cFore, cBack,
                            new DvTextIcon(),
                            DvRoundType.All, Gradient, true,
                            DvContentAlignment.MiddleCenter,
                            false);
                    }

                });
            }
            base.OnDraw(Canvas);
        }
        #endregion

        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            if (Clickable)
            {
                Areas((rtContent, rtButtons) =>
                {
                    #region Buttons
                    for (int i = 0; i < Buttons.Count; i++)
                    {
                        var btn = Buttons[i];
                        if (rtButtons != null && i < rtButtons.Length)
                        {
                            if (CollisionTool.Check(rtButtons[i], x, y))
                            {
                                btn.ButtonDownState = true;
                                ButtonDown?.Invoke(this, new ButtonsClickedEventArgs(btn));
                            }
                        }
                    }
                    #endregion

                    Design?.Input(this);
                });
            }    

            base.OnMouseDown(x, y);
        }
        #endregion

        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtButtons) =>
            {
                #region Buttons
                for (int i = 0; i < Buttons.Count; i++)
                {
                    var btn = Buttons[i];
                    if (rtButtons != null && i < rtButtons.Length)
                    {
                        if (btn.ButtonDownState)
                        {
                            btn.ButtonDownState = false;
                            ButtonUp?.Invoke(this, new ButtonsClickedEventArgs(btn));

                            if (Clickable && CollisionTool.Check(rtButtons[i], x, y))
                            {
                                ButtonClick?.Invoke(this, new ButtonsClickedEventArgs(btn));

                                #region Selector
                                if (SelectorMode)
                                {
                                    var pbtn = Buttons.Where(x => x.Checked).FirstOrDefault();
                                    if (pbtn != btn)
                                    {
                                        if (pbtn != null) pbtn.Checked = false;
                                        btn.Checked = true;
                                        SelectedChanged?.Invoke(this, new ButtonsSelectedventArgs(btn));

                                        if (Animation)
                                        {
                                            prevBtn = pbtn;
                                            nowBtn = btn;

                                            ani.Stop();
                                            ani.Start(DvDesign.ANI2, null);
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                }
                #endregion
            });

            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect[]> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);

            var ls = this.Buttons.Select(x => x.Size).ToList();
            if (Direction == DvDirectionHV.Horizon)
            {
                var rts = Util.DevideSizeH(rtContent, ls);
                act(rtContent, rts);
            }
            else
            {
                var rts = Util.DevideSizeV(rtContent, ls);
                act(rtContent, rts);
            }
        }
        #endregion
        #endregion
    }
}

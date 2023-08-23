using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvLampButton : Design.DvControl
    {
        #region Properties
        #region Text / Icon
        public DvTextIcon TextIcon { get; } = new DvTextIcon();

        public SKBitmap IconImage { get => TextIcon.IconImage; set => TextIcon.IconImage = value; }
        public string IconString { get => TextIcon.IconString; set => TextIcon.IconString = value; }
        public float IconSize { get => TextIcon.IconSize; set => TextIcon.IconSize = value; }
        public float IconGap { get => TextIcon.IconGap; set => TextIcon.IconGap = value; }
        public DvTextIconAlignment IconAlignment { get => TextIcon.IconAlignment; set => TextIcon.IconAlignment = value; }

        public string Text { get => TextIcon.Text; set => TextIcon.Text = value; }
        public Padding TextPadding { get => TextIcon.TextPadding; set => TextIcon.TextPadding = value; }
        public string FontName { get => TextIcon.FontName; set => TextIcon.FontName = value; }
        public float FontSize { get => TextIcon.FontSize; set => TextIcon.FontSize = value; }
        public DvFontStyle FontStyle { get => TextIcon.FontStyle; set => TextIcon.FontStyle = value; }
        #endregion
        #region ContentAlignment
        public DvContentAlignment ContentAlignment { get; set; } = DvContentAlignment.MiddleCenter;
        #endregion
        #region Color
        public SKColor? OnLampColor { get; set; } = null;
        public SKColor? OffLampColor { get; set; } = null;
        public SKColor? ButtonColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;
        #endregion
        #region Round
        public DvRoundType? Round { get; set; } = null;
        #endregion
        #region Lamp
        public int LampSize { get; set; } = 24;
        public DvTextIconAlignment LampAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        #endregion

        #region Gradient
        public bool Gradient { get; set; } = false;
        #endregion
        #region Clickable
        public bool Clickable { get; set; } = true;
        #endregion
        #region ButtonDownState
        public bool ButtonDownState { get; private set; } = false;
        #endregion
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

        #region Animation
        public bool Animation => Design != null ? Design.Animation : false;
        #endregion
        #endregion

        #region Member Variable
        private Animation ani = new Animation();
        #endregion

        #region Event
        public event EventHandler ButtonClick;
        public event EventHandler ButtonDown;
        public event EventHandler ButtonUp;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            var ds = Design;
            var thm = Design?.Theme;
            if (ds != null && thm != null)
            {
                Areas((rtContent) =>
                {
                    #region var
                    var cOn = this.OnLampColor ?? thm.PointColor;
                    var cOff = this.OffLampColor ?? thm.ButtonColor;
                    var cButton = ButtonColor ?? thm.ButtonColor;
                    var cBack = ParentContainer.GetBackColor();
                    var cFore = ForeColor ?? thm.ForeColor;
                    var cBorder = thm.GetBorderColor(cBack, cBack);
                    
                    var round = Round ?? DvRoundType.All;
                    #endregion

                    thm.DrawButton(Canvas, 
                        rtContent, 
                        cButton, cBorder, cFore, cBack, 
                        null,
                        round, Gradient, true, ContentAlignment,
                        ButtonDownState);

                    var cTXT = cFore;
                    var cONL = cOn;
                    var cOFFL = cOff;
                    var cBTN = cButton;
                    var cBOR = cBorder;
                    var rt = Util.FromRect(rtContent);
                    
                    if (ButtonDownState)
                    {
                        rt.Offset(0, 1);
                        cTXT = cFore.BrightnessTransmit(thm.DownBrightness);
                        cONL = cOn.BrightnessTransmit(thm.DownBrightness);
                        cOFFL = cOff.BrightnessTransmit(thm.DownBrightness);
                        cBTN = cButton.BrightnessTransmit(thm.DownBrightness);
                        cBOR = cBorder.BrightnessTransmit(thm.DownBrightness);
                    }

                    thm.DrawLamp(Canvas, rt,
                                cONL, cOFFL, cTXT, cBTN, cBOR,
                                Text, FontName, FontSize, FontStyle,
                                LampSize, LampAlignment,
                                ContentAlignment, OnOff, Animation, ani);

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
                ButtonDownState = true;
                ButtonDown?.Invoke(this, null);

                Design?.Input(this);
            }
            base.OnMouseDown(x, y);
        }
        #endregion

        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            if (ButtonDownState)
            {
                ButtonDownState = false;
                ButtonUp?.Invoke(this, null);

                if (Clickable) ButtonClick?.Invoke(this, null);
            }
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);
            act(rtContent);
        }
        #endregion
        #endregion
    }
}

using Devinno.Skia.Design;
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
    public class DvSwitch : Design.DvControl
    {
        #region Properties
        #region Color
        public SKColor? OnBoxColor { get; set; } = null;
        public SKColor? OffBoxColor { get; set; } = null;
        public SKColor? SwitchColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        #endregion

        #region OnTextIcon
        public DvTextIcon OnTextIcon { get; } = new DvTextIcon() { Text = "ON" };

        public SKBitmap OnIconImage { get => OnTextIcon.IconImage; set => OnTextIcon.IconImage = value; }
        public string OnIconString { get => OnTextIcon.IconString; set => OnTextIcon.IconString = value; }
        public float OnIconSize { get => OnTextIcon.IconSize; set => OnTextIcon.IconSize = value; }
        public float OnIconGap { get => OnTextIcon.IconGap; set => OnTextIcon.IconGap = value; }
        public DvTextIconAlignment OnIconAlignment { get => OnTextIcon.IconAlignment; set => OnTextIcon.IconAlignment = value; }

        public string OnText { get => OnTextIcon.Text; set => OnTextIcon.Text = value; }
        public Padding OnTextPadding { get => OnTextIcon.TextPadding; set => OnTextIcon.TextPadding = value; }
        public string OnFontName { get => OnTextIcon.FontName; set => OnTextIcon.FontName = value; }
        public float OnFontSize { get => OnTextIcon.FontSize; set => OnTextIcon.FontSize = value; }
        public DvFontStyle OnFontStyle { get => OnTextIcon.FontStyle; set => OnTextIcon.FontStyle = value; }
        #endregion

        #region OffTextIcon
        public DvTextIcon OffTextIcon { get; } = new DvTextIcon() { Text = "OFF" };

        public SKBitmap OffIconImage { get => OffTextIcon.IconImage; set => OffTextIcon.IconImage = value; }
        public string OffIconString { get => OffTextIcon.IconString; set => OffTextIcon.IconString = value; }
        public float OffIconSize { get => OffTextIcon.IconSize; set => OffTextIcon.IconSize = value; }
        public float OffIconGap { get => OffTextIcon.IconGap; set => OffTextIcon.IconGap = value; }
        public DvTextIconAlignment OffIconAlignment { get => OffTextIcon.IconAlignment; set => OffTextIcon.IconAlignment = value; }

        public string OffText { get => OffTextIcon.Text; set => OffTextIcon.Text = value; }
        public Padding OffTextPadding { get => OffTextIcon.TextPadding; set => OffTextIcon.TextPadding = value; }
        public string OffFontName { get => OffTextIcon.FontName; set => OffTextIcon.FontName = value; }
        public float OffFontSize { get => OffTextIcon.FontSize; set => OffTextIcon.FontSize = value; }
        public DvFontStyle OffFontStyle { get => OffTextIcon.FontStyle; set => OffTextIcon.FontStyle = value; }
        #endregion

        #region Corner
        public float? Corner { get; set; } = null;
        #endregion

        #region SwitchPadding
        public float SwitchPadding { get; set; } = 5;
        #endregion

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
                        ani.Start(DvDesign.ANI2, bOnOff ? "ON" : "OFF");
                    }
                }
            }
        }
        #endregion

        #region Animation
        public bool Animation => Design != null ? Design.Animation : false;
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
                Areas((rtContent, rtSwitch, rtOn, rtOff, rtOnText, rtOffText) =>
                {
                    #region var
                    var SwitchColor = this.SwitchColor ?? thm.ButtonColor;
                    var OnBoxColor = this.OnBoxColor ?? thm.PointColor;
                    var OffBoxColor = this.OffBoxColor ?? thm.ControlBackColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;

                    var Corner = this.Corner ?? thm.Corner;
                    #endregion

                    thm.DrawSwitch(Canvas,
                        rtContent, rtSwitch, rtOn, rtOff, rtOnText, rtOffText,
                        SwitchColor, OnBoxColor, OffBoxColor, ForeColor,
                        OnTextIcon, OffTextIcon,
                        Corner, OnOff, Animation, ani);
                });
            }
            base.OnDraw(Canvas);
        }
        #endregion

        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtSwitch, rtOn, rtOff, rtOnText, rtOffText) =>
            {
                if (CollisionTool.Check(rtOff, x, y)) OnOff = false;
                if (CollisionTool.Check(rtOn, x, y)) OnOff = true;

                if (CollisionTool.Check(rtSwitch, x, y)) Design?.Input(this);
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width, this.Height);
            var rtSwitch = Util.FromRect(rtContent); rtSwitch.Inflate(-SwitchPadding, -SwitchPadding);
            var szp = new SizeInfo(DvSizeMode.Percent, 50F);
            var rts = Util.DevideSizeH(rtSwitch, new SizeInfo[] { szp, szp }.ToList());
            var rtOff = rts[0];
            var rtOn = rts[1];

            var n = rtSwitch.Width * 0.075F;
        
            var rtOnText = rtOn;
            var rtOffText = rtOff;

            if (Animation && ani.IsPlaying)
            {
                if (ani.Variable == "ON")
                {
                    rtOnText.Offset(ani.Value(AnimationAccel.DCL, -n, 0), 0);
                    rtOffText.Offset(ani.Value(AnimationAccel.DCL, 0, n), 0);
                }
                else if (ani.Variable == "OFF")
                {
                    rtOnText.Offset(ani.Value(AnimationAccel.DCL, 0, -n), 0);
                    rtOffText.Offset(ani.Value(AnimationAccel.DCL, n, 0), 0);
                }
            }
            else
            {
                if (!OnOff)
                {
                    rtOnText.Offset(-n, 0);
                    rtOffText.Offset(0, 0);
                }
                else
                {
                    rtOnText.Offset(0, 0);
                    rtOffText.Offset(n, 0);
                }
            }

            act(rtContent, rtSwitch, rtOn, rtOff, rtOnText, rtOffText);
        }
        #endregion
        #endregion
    }
}

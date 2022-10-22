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
    public class DvToggleButton : DvControl
    {
        #region Properties
        public SKColor? ButtonColor { get; set; } = null;
        public SKColor? CheckedButtonColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;

        public bool AutoSet { get; set; } = true;
        public bool Gradient { get; set; } = true;

        #region Checked
        private bool bChecked = false;
        public bool Checked
        {
            get => bChecked;
            set
            {
                if (bChecked != value)
                {
                    bChecked = value;
                    CheckedChanged?.Invoke(this, null);
                }
            }
        }
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
        #endregion

        #region Event
        public event EventHandler CheckedChanged;
        #endregion

        #region Member Variable
        bool bDown = false;
        #endregion

        #region Constructor
        public DvToggleButton()
        {
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtText) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var OffButtonColor = this.ButtonColor ?? thm.ButtonColor;
                    var OnButtonColor = this.CheckedButtonColor ?? thm.PointColor;
                    var ButtonColor = Checked ? OnButtonColor : OffButtonColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BorderColor = thm.GetBorderColor(ButtonColor, ParentContainer.GetBackColor());
                    var Corner = thm.Corner;

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        var rt = rtContent;

                        var cF = bDown ? ButtonColor.BrightnessTransmit(thm.DownBrightness) : ButtonColor;
                        var cB = bDown ? BorderColor.BrightnessTransmit(thm.DownBrightness) : BorderColor;
                        var cT = bDown ? ForeColor.BrightnessTransmit(thm.DownBrightness) : ForeColor;

                        if (!bDown) thm.DrawBox(Canvas, rt, cF, cB, RoundType.Round, (Gradient ? BoxStyle.GradientV : BoxStyle.Fill) | BoxStyle.InBevel | BoxStyle.OutShadow | BoxStyle.Border);
                        else thm.DrawBox(Canvas, rt, cF, cB, RoundType.Round, BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel | BoxStyle.Border);

                        if (bDown) rtText.Offset(0, 1);
                        Util.DrawTextIcon(Canvas, Text, FontName, FontSize, DvFontStyle.Normal, IconGap, IconString, IconSize, cT, rtText, IconAlignment, DvContentAlignment.MiddleCenter);
                    }
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            bDown = true;
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            bDown = false;

            if (AutoSet) Checked = !Checked;

            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
            var rtText = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, rtContent.Height - 1);

            act(rtContent, rtText);
        }
        #endregion
        #endregion
    }
}

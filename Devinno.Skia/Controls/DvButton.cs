using Devinno.Skia.Design;
using Devinno.Skia.Icon;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvButton : Design.DvControl
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

        #region BackgroundDraw
        public bool BackgroundDraw { get; set; } = true;
        #endregion

        #region Color
        public SKColor? ButtonColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;
        #endregion

        #region Round
        public DvRoundType? Round { get; set; } = null;
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
            Areas((rtContent) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region var
                    var ButtonColor = this.ButtonColor ?? thm.ButtonColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var BorderColor = this.BorderColor ?? thm.GetBorderColor(ButtonColor, BackColor);
                    var Round = this.Round ?? DvRoundType.All;
                    #endregion

                    thm.DrawButton(Canvas, 
                        rtContent,
                        ButtonColor, BorderColor, ForeColor, BackColor,
                        TextIcon, 
                        Round, Gradient, BackgroundDraw, ContentAlignment,
                        ButtonDownState);
                }
            });

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

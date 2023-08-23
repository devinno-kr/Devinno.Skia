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
    public class DvCircleButton : DvControl
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
        #region Color
        public SKColor? ButtonColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? ButtonBackColor { get; set; } = null;
        #endregion

        #region ButtonPadding
        public float ButtonPadding { get; set; } = 7;
        #endregion
        #region ButtonBackPadding
        public float ButtonBackPadding { get; set; } = 0;
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
            var ds = Design;
            var thm = Design?.Theme;
            if (ds != null && thm != null)
            {
                Areas((rtContent, rtCircleBack, rtCircle, rtText) =>
                {
                    #region var
                    var cButton = ButtonColor ?? thm.ButtonColor;
                    var cButtonBack = ButtonBackColor ?? thm.ControlBackColor;
                    var cFore = ForeColor ?? thm.ForeColor;
                    #endregion

                    thm.DrawCircleButton(Canvas, rtContent, rtCircleBack, rtCircle, rtText,
                                                 cButton, cButtonBack, Gradient, ButtonDownState);

                    thm.DrawText(Canvas, rtContent, TextIcon, cFore, DvContentAlignment.MiddleCenter, ButtonDownState);
                });
            }
            base.OnDraw(Canvas);
        }
        #endregion

        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtCircleBack, rtCircle, rtText) =>
            {
                if (Clickable && CollisionTool.CheckCircle(rtCircle, new SKPoint(x, y)))
                {
                    ButtonDownState = true;
                    ButtonDown?.Invoke(this, null);

                    Design?.Input(this);
                }
            });

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

                Areas((rtContent, rtCircleBack, rtCircle, rtText) =>
                {
                    if (Clickable && CollisionTool.CheckCircle(rtCircle, new SKPoint(x, y))) 
                        ButtonClick?.Invoke(this, null);
                });
            }

            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);
            var rtText = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, rtContent.Height - 1);
            var wh = Convert.ToInt32(Math.Min(rtContent.Width, rtContent.Height)) - (ButtonBackPadding * 2);
            var whb = Convert.ToInt32(Math.Min(rtContent.Width, rtContent.Height)) - ((ButtonBackPadding + ButtonPadding) * 2);
            var rtCircleBack = Util.MakeRectangle(rtContent, new SKSize(wh, wh));
            var rtCircle = Util.MakeRectangle(rtContent, new SKSize(whb, whb));
            act(rtContent, rtCircleBack, rtCircle, rtText);
        }
        #endregion
        #endregion
    }
}

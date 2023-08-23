using Devinno.Skia.Design;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Containers
{
    public class DvBorderPanel : DvContainer
    {
        #region Properties
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;
        public int TitleHeight { get; set; } = 30;
        public bool DrawTitle { get; set; } = true;
        public float BorderWidth { get; set; } = 6;

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
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtPanel, rtTitle, rtText) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var BorderPanelColor = this.BorderColor ?? thm.ButtonColor;

                    thm.DrawBorderPanel(Canvas,
                        rtContent, rtPanel, rtTitle, rtText,
                        BorderPanelColor, ForeColor, BackColor,
                        TextIcon, 
                        DrawTitle, BorderWidth);
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region GetBackColor
        public override SKColor GetBackColor()
        {
            var ret = (ParentContainer != null ? ParentContainer.GetBackColor() : Design.Theme.BackColor);
            return ret;
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);
            var rtTitle = Util.FromRect(rtContent.Left + 1, rtContent.Top, rtContent.Width - 2, TitleHeight);
            var rtPanel = rtContent;
            var rtText = Util.FromRect(rtTitle.Left + TextPadding.Left, rtTitle.Top + TextPadding.Top, rtTitle.Width - (TextPadding.Left + TextPadding.Right), rtTitle.Height - (TextPadding.Top + TextPadding.Bottom));

            if (DrawTitle)
            {
                var sz = Util.MeasureTextIcon(Text, FontName, FontSize, DvFontStyle.Normal, IconGap, IconString, IconSize, DvTextIconAlignment.LeftRight);
                var TitleWidth = sz.Width + 20;

                rtTitle = Util.FromRect(rtContent.Left + 1, rtContent.Top, TitleWidth, TitleHeight);
                rtPanel = Util.FromRect(rtContent.Left, rtTitle.Bottom, rtContent.Width, rtContent.Height - rtTitle.Bottom);
                rtText = Util.FromRect(rtTitle.Left + TextPadding.Left, rtTitle.Top + TextPadding.Top, rtTitle.Width - (TextPadding.Left + TextPadding.Right), rtTitle.Height - (TextPadding.Top + TextPadding.Bottom));
            }

            act(rtContent, rtPanel, rtTitle, rtText);
        }
        #endregion
        #endregion
    }
}

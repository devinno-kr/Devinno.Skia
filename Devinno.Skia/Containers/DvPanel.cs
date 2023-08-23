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
    public class DvPanel : DvContainer
    {
        #region Properties
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? PanelColor { get; set; } = null;
        public bool DrawTitle { get; set; } = true;
        public int TitleHeight { get; set; } = 30;

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
                    var PanelColor = this.PanelColor ?? thm.PanelColor;

                    thm.DrawPanel(Canvas,
                        rtContent, rtPanel, rtTitle, rtText,
                        PanelColor, ForeColor, BackColor, 
                        TextIcon,
                        DrawTitle);
                }
            });

            base.OnDraw(Canvas);
        }
        #endregion
        #region GetBackColor
        public override SKColor GetBackColor()
        {
            var ret = this.PanelColor ?? SKColors.Transparent;
            if (ret == SKColors.Transparent) ret = (ParentContainer != null ? ParentContainer.GetBackColor() : Design.Theme.PanelColor);
            return ret;
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);
            var rtTitle = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, TitleHeight); rtTitle.Inflate(-1, 0);
            var rtPanel = rtContent;
            var rtText = Util.FromRect(rtTitle); rtText.Inflate(-10, 0);

            if (DrawTitle)
            {
                rtPanel = Util.FromRect(rtContent.Left, rtTitle.Bottom, rtContent.Width, rtContent.Height - rtTitle.Bottom);
            }

            act(rtContent, rtPanel, rtTitle, rtText);
        }
        #endregion
        #endregion
    }
}

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
    public class DvGroupBox : DvContainer
    {
        #region Properties
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;
        public float BorderWidth { get; set; } = 1F;

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
            Areas((rtContent, rtPanel, rtText) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var BorderColor = this.BorderColor ?? thm.GroupBoxColor;

                    thm.DrawGroupBox(Canvas,
                       rtContent, rtPanel, rtText,
                       BorderColor, ForeColor, BackColor,
                       TextIcon, BorderWidth);
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
        public void Areas(Action<SKRect, SKRect, SKRect> act)
        {
            var sz = Util.MeasureTextIcon(TextIcon);

            var top = Convert.ToSingle(Math.Max(sz.Height, BorderWidth) / 2F);
            var h = Math.Max(sz.Height, BorderWidth); 

            var rtContent = Util.FromRect(0, 0, Width, Height);
            var rtPanel = rtContent;
            var rtText = Util.MakeRectangle(new SKPoint(10 + sz.Width / 2F, top + BorderWidth/2F), sz.Width + 20, h); rtText.Offset(10, 0);

            rtPanel.Top = top;
               
            act(rtContent, rtPanel, rtText);
        }
        #endregion
        #endregion
    }
}

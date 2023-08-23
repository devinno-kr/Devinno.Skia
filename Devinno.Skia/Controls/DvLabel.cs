using Devinno.Skia.Design;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvLabel : Design.DvControl
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
        public SKColor? LabelColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;
        #endregion
        #region Round
        public DvRoundType? Round { get; set; } = null;
        #endregion
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
                    var LabelColor = this.LabelColor ?? thm.LabelColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var BorderColor = this.BorderColor ?? thm.GetBorderColor(LabelColor, BackColor);
                    var Round = this.Round ?? DvRoundType.All;
                    #endregion

                    thm.DrawLabel(Canvas, 
                        rtContent, 
                        LabelColor, BorderColor, ForeColor, BackColor, 
                        TextIcon,
                        Round, BackgroundDraw, ContentAlignment);

                });
            }
            base.OnDraw(Canvas);
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

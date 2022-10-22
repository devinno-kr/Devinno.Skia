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

namespace Devinno.Skia.Containers
{
    public class DvBoxPanel : DvContainer
    {
        #region Properties
        public SKColor? BackColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public Padding TextPadding { get; set; } = new Padding(10, 10, 10, 10);
        public DvContentAlignment TitlePosition { get; set; } = DvContentAlignment.TopLeft;
        public int? Corner { get; set; } = null;
        #region Text
        public string Text { get; set; } = "Text";
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #region Icon
        public string IconString { get; set; } = null;
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        #endregion
        #endregion

        #region Constructor
        public DvBoxPanel()
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
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var PanelColor = this.BackColor ?? thm.PanelColor;
                    var BorderColor = thm.GetBorderColor(PanelColor, ParentContainer.GetBackColor());
                    var Corner = this.Corner ?? thm.Corner;

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        thm.DrawBox(Canvas, rtContent, PanelColor, BorderColor, RoundType.Round, BoxStyle.Fill | BoxStyle.OutShadow, Corner);

                        Util.DrawTextIcon(Canvas, Text, FontName, FontSize, DvFontStyle.Normal,
                               IconGap, IconString, IconSize, ForeColor, rtText,
                               DvTextIconAlignment.LeftRight, TitlePosition, true);

                    }
                }
            });

            base.OnDraw(Canvas);
        }
        #endregion
        #region GetBackColor
        public override SKColor GetBackColor()
        {
            var ret = this.BackColor ?? SKColors.Transparent;
            if (ret == SKColors.Transparent) ret = (ParentContainer != null ? ParentContainer.GetBackColor() : Design.Theme.PanelColor);
            return ret;
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
            var rtText = Util.FromRect(rtContent.Left + TextPadding.Left, rtContent.Top + TextPadding.Top, rtContent.Width - (TextPadding.Left + TextPadding.Right), rtContent.Height - (TextPadding.Top + TextPadding.Bottom));
             
            act(rtContent, rtText);
        }
        #endregion
        #endregion
    }
}

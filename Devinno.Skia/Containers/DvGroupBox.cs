using Devinno.Skia.Design;
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
    public class DvGroupBox : DvContainer
    {
        #region Properties
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;
        public float BorderWidth { get; set; } = 1F;
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
        public DvGroupBox()
        {
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtPanel, rtText) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BorderColor = this.BorderColor ?? thm.GroupBoxColor;
                    var Corner = thm.Corner;

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        using (var mf = SKImageFilter.CreateDropShadow(1, 1, 0, 0, Util.FromArgb(thm.OutBevelAlpha, SKColors.White)))
                        {
                            p.ImageFilter = mf;
                     
                            var rtPnl = new SKRoundRect(rtPanel, Corner);
                            p.IsStroke = true;
                            p.StrokeWidth = BorderWidth;
                            p.Color = BorderColor;

                            rtPnl.Inflate(BorderWidth / 2F, BorderWidth / 2F);
                            Canvas.DrawRoundRect(rtPnl, p);

                            p.ImageFilter = null;
                        }

                        if(!string.IsNullOrWhiteSpace(Text))
                        {
                            p.IsStroke = false;
                            p.Color = ParentContainer.GetBackColor();
                            Canvas.DrawRect(rtText, p);

                            Util.DrawTextIcon(Canvas, Text, FontName, FontSize, DvFontStyle.Normal,
                                IconGap, IconString, IconSize, ForeColor, rtText,
                                DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleCenter, true);
                        }
                    }
                }
            });

            base.OnDraw(Canvas);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect> act)
        {
            var sz = Util.MeasureTextIcon(Text, FontName, FontSize, DvFontStyle.Normal, IconGap, IconString, IconSize, DvTextIconAlignment.LeftRight);

            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
            var rtPanel = rtContent;
            var rtText = Util.FromRect(10, 0, sz.Width + 20, sz.Height);

            rtPanel.Top = Convert.ToInt32(sz.Height / 2);

            act(rtContent, rtPanel, rtText);
        }
        #endregion
        #endregion
    }
}

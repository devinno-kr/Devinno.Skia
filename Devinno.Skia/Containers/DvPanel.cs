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
    public class DvPanel : DvContainer
    {
        #region Properties
        public SKColor? BackColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public bool DrawTitle { get; set; } = true;
        public int TitleHeight { get; set; } = 30;
        public Padding TextPadding { get; set; } = new Padding(5, 0, 0, 0);

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
        public DvPanel()
        {
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtPanel, rtTitle, rtText) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var PanelColor = this.BackColor ?? thm.PanelColor;
                    var BorderColor = thm.GetBorderColor(PanelColor, ParentContainer.GetBackColor());
                    var Corner = thm.Corner;

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        thm.DrawBox(Canvas, rtContent, PanelColor, BorderColor, RoundType.Round, BoxStyle.Fill | BoxStyle.OutShadow);

                        if (DrawTitle)
                        {
                            using (var lg = SKShader.CreateLinearGradient(new SKPoint(0, 0), new SKPoint(0, rtTitle.Bottom),
                                                               new SKColor[] { PanelColor.BrightnessTransmit(thm.GradientLight*2), PanelColor },
                                                               new float[] { 0, 1 }, SKShaderTileMode.Clamp))
                            {
                                p.Shader = lg;
                                p.IsStroke = false;
                                var rt = new SKRoundRect(rtTitle);
                                rt.SetNinePatch(rtTitle, Corner, Corner, Corner, 0);
                                Canvas.DrawRoundRect(rt, p);
                                p.Shader = null;
                            }

                            Util.DrawTextIcon(Canvas, Text, FontName, FontSize, DvFontStyle.Normal,
                                IconGap, IconString, IconSize, ForeColor, rtText,
                                DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleLeft, true);

                            using (var mf = SKImageFilter.CreateDropShadow(0, 1, 0, 0, Util.FromArgb(thm.OutBevelAlpha, SKColors.White)))
                            {
                                p.ImageFilter = mf;

                                p.IsStroke = true;
                                p.StrokeWidth = 1;
                                p.Color = BorderColor;

                                Canvas.DrawLine(rtContent.Left + 5, rtTitle.Bottom + 0.5F, rtContent.Right - 5, rtTitle.Bottom + 0.5F, p);

                                p.ImageFilter = null;
                            }
                        }

                        thm.DrawBox(Canvas, rtContent, PanelColor, BorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.InBevel);

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
        void bounds(Action<SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);
            var rtTitle = Util.FromRect(rtContent.Left + 1, rtContent.Top, rtContent.Width - 2, TitleHeight);
            var rtPanel = rtContent;
            var rtText = Util.FromRect(rtTitle.Left + TextPadding.Left, rtTitle.Top + TextPadding.Top, rtTitle.Width - (TextPadding.Left + TextPadding.Right), rtTitle.Height - (TextPadding.Top + TextPadding.Bottom));

            if (DrawTitle)
            {
                rtTitle = Util.FromRect(rtContent.Left + 1, rtContent.Top, rtContent.Width - 2, TitleHeight);
                rtPanel = Util.FromRect(rtContent.Left, rtTitle.Bottom, rtContent.Width, rtContent.Height - rtTitle.Bottom);
                rtText = Util.FromRect(rtTitle.Left + TextPadding.Left, rtTitle.Top + TextPadding.Top, rtTitle.Width - (TextPadding.Left + TextPadding.Right), rtTitle.Height - (TextPadding.Top + TextPadding.Bottom));
            }

            act(rtContent, rtPanel, rtTitle, rtText);
        }
        #endregion
        #endregion
    }
}

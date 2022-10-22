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
    public class DvBorderPanel : DvContainer
    {
        #region Properties
        public SKColor? BackColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;

        public int BorderWidth { get; set; } = 4;
        public int? Corner { get; set; }
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
        public DvBorderPanel()
        {
        }
        #endregion

        #region Override
        #region GetBackColor
        public override SKColor GetBackColor()
        {
            var ret = this.BackColor ?? SKColors.Transparent;
            if (ret == SKColors.Transparent) ret = (ParentContainer != null ? ParentContainer.GetBackColor() : Design.Theme.BackColor);
            return ret;
        }
        #endregion
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
                    var PanelColor = this.BackColor ?? SKColors.Transparent;
                    var BorderColor = this.BorderColor ?? thm.BorderPanelColor;
                    var Corner = this.Corner ?? thm.Corner;
                    var TitleBorderColor = PanelColor == SKColors.Transparent ? thm.GetBorderColor(BorderColor, ParentContainer.GetBackColor())
                                                                              : thm.GetBorderColor(BorderColor, PanelColor);
                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        var rti = rtContent; rti.Inflate(-BorderWidth, -BorderWidth);
                        var rt = new SKRoundRect(rtContent, Corner);
                        var rt2 = new SKRoundRect(rti, Corner);

                        #region Fill
                        if (PanelColor != SKColors.Transparent)
                        {
                            p.IsStroke = false;
                            p.Color = PanelColor;
                            Canvas.DrawRoundRect(rt, p);
                        }
                        #endregion
                        #region Border
                        using (var mf = SKImageFilter.CreateDropShadow(1, 1, 1, 1, Util.FromArgb(thm.OutShadowAlpha, SKColors.Black)))
                        {
                            p.ImageFilter = mf;

                            using (var path = new SKPath())
                            {
                                path.AddRoundRect(rt);

                                using (var path2 = new SKPath())
                                {
                                    if (DrawTitle)
                                    {
                                        var TitleWidth = rtTitle.Width;

                                        path2.AddArc(Util.FromRect(rti.Left, TitleHeight, Corner * 2, Corner * 2), 180, 90);
                                        path2.LineTo(TitleWidth, TitleHeight);
                                        path2.CubicTo(TitleWidth + 10, TitleHeight,
                                                     TitleWidth + 15, TitleHeight / 2F,
                                                     TitleWidth + 30, rti.Top);
                                        path2.ArcTo(Util.FromRect(rti.Right - Corner * 2, rti.Top, Corner * 2, Corner * 2), -90, 90, false);
                                        path2.ArcTo(Util.FromRect(rti.Right - Corner * 2, rti.Bottom - Corner * 2, Corner * 2, Corner * 2), 0, 90, false);
                                        path2.ArcTo(Util.FromRect(rti.Left, rti.Bottom - Corner * 2, Corner * 2, Corner * 2), 90, 90, false);
                                        path2.Close();
                                    }
                                    else
                                    {
                                        path2.AddRoundRect(rt2);
                                    }
                                    path.AddPathReverse(path2);
                                }


                                p.IsStroke = false;
                                p.Color = BorderColor;
                                Canvas.DrawPath(path, p);
                            }

                            p.ImageFilter = null;
                        }
                        #endregion
                        #region Text
                        if (DrawTitle)
                        {
                            Util.DrawTextIcon(Canvas, Text, FontName, FontSize, DvFontStyle.Normal,
                                IconGap, IconString, IconSize, ForeColor, rtText,
                                DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleCenter, true);
                        }
                        #endregion
                    }
                }
            });

            base.OnDraw(Canvas);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width - 1, this.Height - 1);
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

using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
using Devinno.Skia.Theme;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvLabel : DvControl
    {
        #region Properties
        public SKColor? LabelColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
         
        public DvLabelStyle Style { get; set; } = DvLabelStyle.FlatConvex;
        public DvContentAlignment ContentAlignment { get; set; } = DvContentAlignment.MiddleCenter;

        public Padding TextPadding { get; set; } = new Padding();
        public string Unit { get; set; }
        public int UnitWidth { get; set; } = 36;
        public bool BackgroundDraw { get; set; } = true;
        
        #region Text
        public string Text { get; set; } = "Text";
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #region Icon
        public string IconString { get; set; } = null;
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        #endregion
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtText, rtUnit) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var LabelColor = this.LabelColor ?? thm.LabelColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BorderColor = this.BorderColor ?? thm.GetBorderColor(LabelColor, ParentContainer.GetBackColor());
                    var Corner = thm.Corner;

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        var rt = rtContent;

                        var cF = LabelColor;
                        var cB = BorderColor;
                        var cT = ForeColor;

                        if (BackgroundDraw)
                        {
                            switch (Style)
                            {
                                case DvLabelStyle.FlatConcave:
                                    thm.DrawBox(Canvas, rt, cF, cB, RoundType.Round, BoxStyle.Fill | BoxStyle.Border | BoxStyle.OutBevel);
                                    break;
                                case DvLabelStyle.FlatConvex:
                                    thm.DrawBox(Canvas, rt, cF, cB, RoundType.Round, BoxStyle.Fill | BoxStyle.Border | BoxStyle.OutShadow);
                                    break;
                                case DvLabelStyle.Concave:
                                    thm.DrawBox(Canvas, rt, cF, cB, RoundType.Round, BoxStyle.Fill | BoxStyle.Border | BoxStyle.OutBevel | BoxStyle.InShadow);
                                    break;
                                case DvLabelStyle.Convex:
                                    thm.DrawBox(Canvas, rt, cF, cB, RoundType.Round, BoxStyle.Fill | BoxStyle.Border | BoxStyle.OutShadow | BoxStyle.InBevel);
                                    break;
                            }
                        }
                       
                        Util.DrawTextIcon(Canvas, Text, FontName, FontSize, DvFontStyle.Normal, IconGap, IconString, IconSize, cT, rtText, IconAlignment, ContentAlignment, true, thm.OutShadowAlpha);

                        if (UnitWidth > 0 && !string.IsNullOrWhiteSpace(Unit))
                        {
                            #region Unit Sep
                            if (BackgroundDraw)
                            {
                                var szh = Convert.ToInt32(rtUnit.Height / 2);

                                p.StrokeWidth = 1;

                                p.Color = Util.FromArgb(thm.OutBevelAlpha, SKColors.White);
                                Canvas.DrawLine(rtUnit.Left + 1.5F, (rtContent.Top + (rtContent.Height / 2)) - (szh / 2) + 1.5F, rtUnit.Left + 1, (rtContent.Top + (rtContent.Height / 2)) + (szh / 2) + 1, p);

                                p.Color = Util.FromArgb(thm.OutShadowAlpha, SKColors.Black);
                                Canvas.DrawLine(rtUnit.Left + 0.5F, (rtContent.Top + (rtContent.Height / 2)) - (szh / 2) + 0.5F, rtUnit.Left, (rtContent.Top + (rtContent.Height / 2)) + (szh / 2) + 1, p);
                            }
                            #endregion

                            Util.DrawText(Canvas, Unit, FontName, FontSize, DvFontStyle.Normal, cT, rtUnit, DvContentAlignment.MiddleCenter);
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
            var szUnitW = 0;
            if (!string.IsNullOrWhiteSpace(Unit)) szUnitW = UnitWidth;

            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
            var rtTextAll = Util.FromRect(rtContent.Left + TextPadding.Left, rtContent.Top + TextPadding.Top, rtContent.Width - (TextPadding.Left + TextPadding.Right), rtContent.Height - (TextPadding.Top + TextPadding.Bottom));
            var rtUnit = Util.FromRect(rtTextAll.Right - szUnitW, rtTextAll.Top, szUnitW, rtTextAll.Height);
            var rtText = Util.FromRect(rtTextAll.Left, rtTextAll.Top, rtTextAll.Width - rtUnit.Width, rtTextAll.Height-1); 
            

            act(rtContent, rtText, rtUnit);
        }
        #endregion
        #endregion
    }

    #region enum : DvLabelStyle 
    public enum DvLabelStyle 
    {
        /// <summary>
        /// 평평한 
        /// </summary>
        FlatConvex,
        /// <summary>
        /// 평평한
        /// </summary>
        FlatConcave,
        /// <summary>
        /// 오목한
        /// </summary>
        Concave, 
        /// <summary>
        /// 볼록한
        /// </summary>
        Convex
    }
    #endregion
}

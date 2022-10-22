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

namespace Devinno.Skia.Controls
{
    public class DvValueLabel : DvControl
    {
        #region Properties
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? TitleBoxColor { get; set; } = null;
        public SKColor? ValueBoxColor { get; set; } = null;

        public string Unit { get; set; } = null;
        public int UnitWidth { get; set; } = 36;
        public int TitleWidth { get; set; } = 60;
        public DvLabelStyle Style { get; set; } = DvLabelStyle.FlatConcave;

        #region Text
        public string Title { get; set; } = "Title";
        public string Value { get; set; } = "Value";
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #region Icon
        public string TitleIconString { get; set; } = null;
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        #endregion
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtTitle, rtValueAll, rtUnit, rtValue) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var TitleBoxColor = this.TitleBoxColor ?? thm.ButtonColor;
                    var ValueBoxColor = this.ValueBoxColor ?? thm.LabelColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var TitleBorderColor = thm.GetBorderColor(TitleBoxColor, ParentContainer.GetBackColor());
                    var ValueBorderColor = thm.GetBorderColor(ValueBoxColor, ParentContainer.GetBackColor());
                    var Corner = thm.Corner;

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        #region Title
                        thm.DrawBox(Canvas, rtTitle, TitleBoxColor, TitleBorderColor, RoundType.Round_L, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.OutShadow);
                        #endregion
                        #region Value
                        switch (Style)
                        {
                            case DvLabelStyle.FlatConcave:
                                thm.DrawBox(Canvas, rtValueAll, ValueBoxColor, ValueBorderColor, RoundType.Round_R, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutBevel);
                                break;
                            case DvLabelStyle.FlatConvex:
                                thm.DrawBox(Canvas, rtValueAll, ValueBoxColor, ValueBorderColor, RoundType.Round_R, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutShadow);
                                break;
                            case DvLabelStyle.Concave:
                                thm.DrawBox(Canvas, rtValueAll, ValueBoxColor, ValueBorderColor, RoundType.Round_R, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel);
                                break;
                            case DvLabelStyle.Convex:
                                thm.DrawBox(Canvas, rtValueAll, ValueBoxColor, ValueBorderColor, RoundType.Round_R, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.OutShadow);
                                break;
                        }
                        #endregion
                        #region Text
                        Util.DrawTextIcon(Canvas, Title, FontName, FontSize, DvFontStyle.Normal, IconGap, TitleIconString, IconSize, ForeColor, rtTitle, IconAlignment, DvContentAlignment.MiddleCenter, true);
                        Util.DrawText(Canvas, Value, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtValue, DvContentAlignment.MiddleCenter, true);
                        #endregion
                        #region Unit
                        if (UnitWidth > 0 && !string.IsNullOrWhiteSpace(Unit))
                        {
                            #region Unit Sep
                            {
                                float h, s, b;
                                ValueBoxColor.ToHsv(out h, out s, out b);

                                var szh = Convert.ToInt32(rtUnit.Height / 2);

                                p.StrokeWidth = 1;

                                p.Color = b < 50 ? thm.GetInBevelColor(ValueBoxColor) : ValueBoxColor.BrightnessTransmit(thm.BorderBrightness);
                                Canvas.DrawLine(rtUnit.Left + 0.5F, (rtContent.Top + (rtContent.Height / 2)) - (szh / 2) + 1, rtUnit.Left + 0.5F, (rtContent.Top + (rtContent.Height / 2)) + (szh / 2) + 1, p);

                                p.Color = b < 50 ? ValueBoxColor.BrightnessTransmit(thm.BorderBrightness) : thm.GetInBevelColor(ValueBoxColor);
                                Canvas.DrawLine(rtUnit.Left + 1.5F, (rtContent.Top + (rtContent.Height / 2)) - (szh / 2) + 1, rtUnit.Left + 1.5F, (rtContent.Top + (rtContent.Height / 2)) + (szh / 2) + 1, p);
                            }
                            #endregion

                            Util.DrawText(Canvas, Unit, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtUnit, DvContentAlignment.MiddleCenter, true);
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
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
            var rtTitle = Util.FromRect(rtContent.Left, rtContent.Top, TitleWidth + 1, rtContent.Height);
            var rtValueAll = Util.FromRect(rtContent.Left + TitleWidth, rtContent.Top, rtContent.Width - TitleWidth, rtContent.Height);

            var szUnitW = 0;
            if (!string.IsNullOrWhiteSpace(Unit)) szUnitW = UnitWidth;
            var rtUnit = Util.FromRect(rtValueAll.Right - szUnitW, rtValueAll.Top, szUnitW, rtValueAll.Height);
            var rtValue = Util.FromRect(rtValueAll.Left, rtValueAll.Top, rtValueAll.Width - szUnitW, rtValueAll.Height);
            
            act(rtContent, rtTitle, rtValueAll, rtUnit, rtValue);
        }
        #endregion
        #endregion
    }
}

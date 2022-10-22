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
    public class DvValueLabelButton : DvControl
    {
        #region Properties
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? TitleBoxColor { get; set; } = null;
        public SKColor? ValueBoxColor { get; set; } = null;
        public SKColor? ButtonColor { get; set; } = null;

        public bool UseButton { get; set; } = true;
        public string Unit { get; set; } = null;
        public int UnitWidth { get; set; } = 36;
        public int ButtonWidth { get; set; } = 60;
        public int TitleWidth { get; set; } = 60;
        public DvLabelStyle Style { get; set; } = DvLabelStyle.FlatConcave;

        #region Text
        public string Title { get; set; } = "Title";
        public string Value { get; set; } = "Value";
        public string ButtonText { get; set; } = "Button";
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #region Icon
        public string TitleIconString { get; set; } = null;
        public string ButtonIconString { get; set; } = null;
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        #endregion
        #endregion

        #region Event
        public event EventHandler ButtonClick;
        public event EventHandler ButtonDown;
        public event EventHandler ButtonUp;
        #endregion

        #region Member Variable
        bool bDown = false;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtTitle, rtValueAll, rtUnit, rtValue, rtButton) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var TitleBoxColor = this.TitleBoxColor ?? thm.ButtonColor;
                    var ValueBoxColor = this.ValueBoxColor ?? thm.LabelColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var ButtonColor = this.ButtonColor ?? thm.ButtonColor;
                    var TitleBorderColor = thm.GetBorderColor(TitleBoxColor, ParentContainer.GetBackColor());
                    var ValueBorderColor = thm.GetBorderColor(ValueBoxColor, ParentContainer.GetBackColor());
                    var ButtonBorderColor = thm.GetBorderColor(ButtonColor, ParentContainer.GetBackColor());
                    var Corner = thm.Corner;

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        #region Title
                        thm.DrawBox(Canvas, rtTitle, TitleBoxColor, TitleBorderColor, RoundType.Round_L, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.OutShadow);
                        #endregion
                        #region Value
                        var round = UseButton ? RoundType.Rect: RoundType.Round_R;
                        switch (Style)
                        {
                            case DvLabelStyle.FlatConcave:
                                thm.DrawBox(Canvas, rtValueAll, ValueBoxColor, ValueBorderColor, round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutBevel);
                                break;
                            case DvLabelStyle.FlatConvex:
                                thm.DrawBox(Canvas, rtValueAll, ValueBoxColor, ValueBorderColor, round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutShadow);
                                break;
                            case DvLabelStyle.Concave:
                                thm.DrawBox(Canvas, rtValueAll, ValueBoxColor, ValueBorderColor, round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel);
                                break;
                            case DvLabelStyle.Convex:
                                thm.DrawBox(Canvas, rtValueAll, ValueBoxColor, ValueBorderColor, round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.OutShadow);
                                break;
                        }
                        #endregion
                        #region Button
                        if (UseButton)
                        {
                            var rt = rtButton.Value;

                            if (!bDown)
                            {
                                var cv = ButtonColor;
                                var ct = ForeColor;
                                thm.DrawBox(Canvas, rtButton.Value, cv, ButtonBorderColor, RoundType.Round_R, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow);
                                Util.DrawTextIcon(Canvas, ButtonText, FontName, FontSize, DvFontStyle.Normal, IconGap, ButtonIconString, IconSize, ct, rt, IconAlignment, DvContentAlignment.MiddleCenter, true);
                            }
                            else
                            {
                                rt.Offset(0, 1);

                                var cv = ButtonColor.BrightnessTransmit(thm.DownBrightness);
                                var ct = ForeColor.BrightnessTransmit(thm.DownBrightness);
                                thm.DrawBox(Canvas, rtButton.Value, cv, ButtonBorderColor, RoundType.Round_R, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel);
                                Util.DrawTextIcon(Canvas, ButtonText, FontName, FontSize, DvFontStyle.Normal, IconGap, ButtonIconString, IconSize, ct, rt, IconAlignment, DvContentAlignment.MiddleCenter, true);
                            }
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
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            bounds((rtContent, rtTitle, rtValueAll, rtUnit, rtValue, rtButton) =>
            {
                if(rtButton.HasValue)
                {
                    if (CollisionTool.Check(rtButton.Value, x, y))
                    {
                        bDown = true;
                        ButtonDown?.Invoke(this, null);
                    }
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            bounds((rtContent, rtTitle, rtValueAll, rtUnit, rtValue, rtButton) =>
            {
                if (rtButton.HasValue)
                {
                    var b = bDown;
                    if (bDown)
                    {
                        bDown = false;
                        ButtonUp?.Invoke(this, null);
                    }

                    if (b && CollisionTool.Check(rtButton.Value, x, y))
                    {
                        ButtonClick?.Invoke(this, null);
                    }
                }

                
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect?> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);

            if (UseButton)
            {
                var rtTitle = Util.FromRect(rtContent.Left, rtContent.Top, TitleWidth + 1, rtContent.Height);
                var rtValueAll = Util.FromRect(rtContent.Left + TitleWidth, rtContent.Top, rtContent.Width - TitleWidth - ButtonWidth + 1, rtContent.Height);
                var rtButton = Util.FromRect(rtContent.Right - ButtonWidth, rtContent.Top, ButtonWidth, rtContent.Height);

                var szUnitW = 0;
                if (!string.IsNullOrWhiteSpace(Unit)) szUnitW = UnitWidth;
                var rtUnit = Util.FromRect(rtValueAll.Right - szUnitW, rtValueAll.Top, szUnitW, rtValueAll.Height);

                var rtValue = Util.FromRect(rtValueAll.Left, rtValueAll.Top, rtValueAll.Width - szUnitW, rtValueAll.Height);

                act(rtContent, rtTitle, rtValueAll, rtUnit, rtValue, rtButton);

            }
            else
            {
                var rtTitle = Util.FromRect(rtContent.Left, rtContent.Top, TitleWidth + 1, rtContent.Height);
                var rtValueAll = Util.FromRect(rtContent.Left + TitleWidth, rtContent.Top, rtContent.Width - TitleWidth, rtContent.Height);

                var szUnitW = 0;
                if (!string.IsNullOrWhiteSpace(Unit)) szUnitW = UnitWidth;
                var rtUnit = Util.FromRect(rtValueAll.Right - szUnitW, rtValueAll.Top, szUnitW, rtValueAll.Height);

                var rtValue = Util.FromRect(rtValueAll.Left, rtValueAll.Top, rtValueAll.Width - szUnitW, rtValueAll.Height);

                act(rtContent, rtTitle, rtValueAll, rtUnit, rtValue, null);
            }
        }
        #endregion
        #endregion
    }
}

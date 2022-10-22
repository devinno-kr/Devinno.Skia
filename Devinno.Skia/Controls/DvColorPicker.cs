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
    public class DvColorPicker : DvControl
    {
        #region Properties
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? ButtonColor { get; set; } = null;

        public int ButtonWidth { get; set; } = 40;

        public int PickerBoxWidth { get; set; } = 600;
        public int PickerBoxHeight { get; set; } = 400;

        public ColorCodeType CodeType { get; set; } = ColorCodeType.CodeRGB;

        #region Text
        private SKColor cColor = SKColors.White;
        public SKColor SelectedColor
        {
            get => cColor;
            set
            {
                if(cColor != value )
                {
                    cColor = value;
                    SelectedColorChanged?.Invoke(this, null);
                }
            }
        }
        
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #region Icon
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        #endregion
        #endregion

        #region Event
        public event EventHandler SelectedColorChanged;
        #endregion

        #region Member Variable
        bool bDown = false;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtValue, rtButton) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var BoxColor = this.BoxColor ?? thm.InputColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BorderColor = thm.GetBorderColor(BoxColor, ParentContainer.GetBackColor());
                    var ButtonColor = this.ButtonColor ?? thm.ButtonColor;
                    var Corner = thm.Corner;

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        #region Value
                        thm.DrawBox(Canvas, rtValue, BoxColor, BorderColor, RoundType.Round_L, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutBevel);
                        #endregion
                        #region Button
                        {
                            var rt = rtButton;

                            if (!bDown)
                            {
                                var cv = ButtonColor;
                                var ct = ForeColor;
                                thm.DrawBox(Canvas, rtButton, cv, BorderColor, RoundType.Round_R, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow);
                                Util.DrawIconFA(Canvas, "fa-palette", IconSize, ct, rt);
                            }
                            else
                            {
                                rt.Offset(0, 1);

                                var cv = ButtonColor.BrightnessTransmit(thm.DownBrightness);
                                var ct = ForeColor.BrightnessTransmit(thm.DownBrightness);
                                thm.DrawBox(Canvas, rtButton, cv, BorderColor, RoundType.Round_R, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel);
                                Util.DrawIconFA(Canvas, "fa-palette", IconSize, ct, rt);
                            }
                        }
                        #endregion
                        #region Text
                        {
                            var sz = Height / 3;
                            var s = ColorTool.GetName(SelectedColor, CodeType);

                            var sp = Canvas.Save();
                            Canvas.ClipRect(rtValue);

                            Util.TextIconBounds(s, FontName, FontSize, DvFontStyle.Normal, IconGap, rtValue, Util.FromRect(0, 0, sz, sz), DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleCenter, (rtIcon, rtText) =>
                            {
                                rtIcon.Offset(0, 1);
                                thm.DrawBox(Canvas, rtIcon, SelectedColor, SKColors.Black, RoundType.Rect, BoxStyle.Fill | BoxStyle.Border | BoxStyle.OutShadow);
                                Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtText, DvContentAlignment.MiddleCenter, true);
                            });

                            Canvas.RestoreToCount(sp);
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
            bounds((rtContent, rtValue, rtButton) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    if (CollisionTool.Check(rtButton, x, y))
                    {
                        bDown = true;
                    }
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            bounds((rtContent, rtValue, rtButton) =>
            {
                if (bDown)
                {
                    bDown = false;
                    if (CollisionTool.Check(rtButton, x, y))
                    {
                        var ds = Design;
                        var thm = Design?.Theme;
                        if (ds != null && thm != null)
                        {
                            ds.ColorPickerBox.ShowColorPicker("색상 선택", SelectedColor, (color) => { if (color.HasValue) SelectedColor = color.Value; });
                        }
                    }
                }
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);
            var rtValue = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width - ButtonWidth+1, rtContent.Height);
            var rtButton = Util.FromRect(rtContent.Right - ButtonWidth, rtContent.Top, ButtonWidth, rtContent.Height);

            act(rtContent, rtValue, rtButton);
        }
        #endregion
        #endregion
    }
}

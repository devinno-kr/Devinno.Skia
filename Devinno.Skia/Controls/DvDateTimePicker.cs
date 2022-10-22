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
    public class DvDateTimePicker : DvControl
    {
        #region Properties
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? ButtonColor { get; set; } = null;

        public int ButtonWidth { get; set; } = 40;

        public int PickerBoxWidth { get; set; } = 400;
        public int PickerBoxHeight { get; set; } = 400;

        public DateTimePickerType Type { get; set; } =  DateTimePickerType.DateTime;

        #region Text
        private DateTime dtTime = DateTime.Now;
        public DateTime SelectedValue
        {
            get => dtTime;
            set
            {
                if (dtTime != value)
                {
                    dtTime = value;
                    SelectedValueChanged?.Invoke(this, null);
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
        public event EventHandler SelectedValueChanged;
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

                            var sico = "fa-calendar-check";
                            switch(Type)
                            {
                                case DateTimePickerType.DateTime: sico = "fa-calendar-check"; break;
                                case DateTimePickerType.Date: sico = "fa-calendar-check"; break;
                                case DateTimePickerType.Time: sico = "fa-clock"; break;
                            }

                            if (!bDown)
                            {
                                var cv = ButtonColor;
                                var ct = ForeColor;
                                thm.DrawBox(Canvas, rtButton, cv, BorderColor, RoundType.Round_R, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow);
                                Util.DrawIconFA(Canvas, sico, IconSize, ct, rt);
                            }
                            else
                            {
                                rt.Offset(0, 1);

                                var cv = ButtonColor.BrightnessTransmit(thm.DownBrightness);
                                var ct = ForeColor.BrightnessTransmit(thm.DownBrightness);
                                thm.DrawBox(Canvas, rtButton, cv, BorderColor, RoundType.Round_R, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel);
                                Util.DrawIconFA(Canvas, sico, IconSize, ct, rt);
                            }
                        }
                        #endregion
                        #region Text
                        {
                            var sp = Canvas.Save();
                            Canvas.ClipRect(rtValue);

                            var sz = Height / 3;
                            var s = "";
                            switch (Type)
                            {
                                case DateTimePickerType.Date: s = dtTime.ToString("yyyy.MM.dd"); break;
                                case DateTimePickerType.Time: s = dtTime.ToString("HH:mm:ss"); break;
                                case DateTimePickerType.DateTime: s = dtTime.ToString("yyyy.MM.dd HH:mm:ss"); break;
                            }
                            Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtValue);

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
                            if (Type == DateTimePickerType.DateTime) ds.DateTimePickerBox.ShowDateTimePicker("날짜/시간 선택", SelectedValue, (time) => { if (time.HasValue) SelectedValue = time.Value; });
                            if (Type == DateTimePickerType.Date) ds.DateTimePickerBox.ShowDatePicker("날짜 선택", SelectedValue, (time) => { if (time.HasValue) SelectedValue = time.Value.Date; });
                            if (Type == DateTimePickerType.Time) ds.DateTimePickerBox.ShowTimePicker("시간 선택", SelectedValue, (time) => { if (time.HasValue) SelectedValue = time.Value; });
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
            var rtValue = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width - ButtonWidth + 1, rtContent.Height);
            var rtButton = Util.FromRect(rtContent.Right - ButtonWidth, rtContent.Top, ButtonWidth, rtContent.Height);

            act(rtContent, rtValue, rtButton);
        }
        #endregion
        #endregion
    }

    public enum DateTimePickerType { Date, Time, DateTime }
}

using Devinno.Skia.Design;
using Devinno.Skia.Dialogs;
using Devinno.Skia.Theme;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvDateTimePicker : DvValueLabel
    {
        #region Properties
        #region Value
        private DateTime dtValue = DateTime.Now;
        public DateTime Value
        {
            get => dtValue;
            set
            {
                if (dtValue != value)
                {
                    dtValue = value;
                    ValueChanged?.Invoke(this, null);
                }
            }
        }
        #endregion
        #region Font
        public DateTimePickerMode Type { get; set; } = DateTimePickerMode.DateTime;

        public string Text
        {
            get
            {
                var ret = "";
                if (Type == DateTimePickerMode.Date) ret = Value.ToString("yyyy-MM-dd");
                else if (Type == DateTimePickerMode.Time) ret = Value.ToString("HH:mm:ss");
                else if (Type == DateTimePickerMode.DateTime) ret = Value.ToString("yyyy-MM-dd HH:mm:ss");
                return ret;
            }
        }
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        #endregion
        #region ButtonIconSize
        private int nButtonIconSize = 14;
        public int ButtonIconSize
        {
            get => nButtonIconSize;
            set
            {
                if (nButtonIconSize != value)
                {
                    nButtonIconSize = value;
                    foreach (var btn in Buttons) btn.IconSize = value;
                }
            }
        }
        #endregion
        #region Buttons
        private new List<ButtonInfo> Buttons => base.Buttons;
        #endregion
        #region Direction
        public override DvDirectionHV Direction
        {
            get => base.Direction;
            set
            {
                base.Direction = value;
                if (base.Direction == DvDirectionHV.Horizon)
                {
                    ButtonAreaSize = Height * 2;
                }
                else if (base.Direction == DvDirectionHV.Vertical)
                {
                    ButtonAreaSize = 40;
                }
            }
        }
        #endregion
        #endregion

        #region Event
        public event EventHandler ValueChanged;
        #endregion

        #region Constructor
        public DvDateTimePicker()
        {
            Buttons.Add(new ButtonInfo("calendar") { IconString = "fa-calendar-check", IconSize = ButtonIconSize, Size = new SizeInfo(DvSizeMode.Percent, 100) });

            if (Direction == DvDirectionHV.Horizon) ButtonAreaSize = Height;
            else if (Direction == DvDirectionHV.Vertical) ButtonAreaSize = Height / 3;

            #region ButtonDown
            ButtonDown += (o, s) => Design?.Input(this);
            ButtonClick += (o, s) =>
            {
                if (s.Button.Name == "calendar")
                {
                    if (Type == DateTimePickerMode.DateTime) DvDialogs.DateTimePickerBox.ShowDateTimePicker(Title ?? "날짜/시간 선택", Value, (datetime) => { if (datetime.HasValue) Value = datetime.Value; });
                    if (Type == DateTimePickerMode.Date) DvDialogs.DateTimePickerBox.ShowDatePicker(Title ?? "날짜 선택", Value, (datetime) => { if (datetime.HasValue) Value = datetime.Value.Date; });
                    if (Type == DateTimePickerMode.Time) DvDialogs.DateTimePickerBox.ShowTimePicker(Title ?? "시간 선택", Value, (datetime) => { if (datetime.HasValue) Value = datetime.Value; });
                }
            };
            #endregion
        }
        #endregion

        #region DrawValue
        protected override void DrawValue(SKCanvas canvas, DvTheme thm, SKRect rtValue)
        {
            if (Direction == DvDirectionHV.Horizon) ButtonAreaSize = Height;
            else if (Direction == DvDirectionHV.Vertical) ButtonAreaSize = Height / 3;

            var ForeColor = this.ForeColor ?? thm.ForeColor;
            var ValueColor = this.ValueColor ?? thm.LabelColor;

            var s = Text;
            thm.DrawText(canvas, rtValue, FontName, FontSize, FontStyle, s, ForeColor);
        }
        #endregion
    }
}

using Devinno.Skia.Design;
using Devinno.Skia.Dialogs;
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
    public class DvColorPicker : DvValueLabel
    {
        #region Properties
        #region Value
        private SKColor cValue = SKColors.White;
        public SKColor Value
        {
            get => cValue;
            set
            {
                if (cValue != value)
                {
                    cValue = value;
                    ValueChanged?.Invoke(this, null);
                }
            }
        }
        #endregion
        #region Code
        public ColorCodeType Code { get; set; } = ColorCodeType.CodeRGB;
        #endregion
        #region Font
        public string Text => ColorTool.GetName(Value, Code);
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
        public DvColorPicker()
        {
            Buttons.Add(new ButtonInfo("palette") { IconString = "fa-palette", IconSize = ButtonIconSize, Size = new SizeInfo(DvSizeMode.Percent, 100) });
 
            if (Direction == DvDirectionHV.Horizon) ButtonAreaSize = Height;
            else if (Direction == DvDirectionHV.Vertical) ButtonAreaSize = Height / 3;

            #region ButtonDown
            ButtonDown += (o, s) => Design?.Input(this);
            ButtonClick += (o, s) =>
            {
                if (s.Button.Name == "palette")
                {
                    DvDialogs.ColorPickerBox.ShowColorPicker(Title ?? "색상 선택", Value,
                        (color) =>
                        {
                            if (color.HasValue)
                                Value = color.Value;
                        });
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

            var sz = ButtonIconSize;
            var s = Text;
            Util.TextIconBounds(s, FontName, FontSize, DvFontStyle.Normal, 10, rtValue, 
                Util.FromRect(0, 0, sz, sz), DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleCenter,
                (rtIcon, rtText) =>
                {
                    rtIcon.Offset(0, 1);
                    thm.DrawBox(canvas, rtIcon, Value, SKColors.Black, DvRoundType.Rect, BoxStyle.Fill | BoxStyle.Border, thm.Corner);
                    thm.DrawText(canvas, rtText, FontName, FontSize, FontStyle, s, ForeColor);
                });
        }
        #endregion
    }
}

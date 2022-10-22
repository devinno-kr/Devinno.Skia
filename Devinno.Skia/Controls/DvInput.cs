using Devinno.Skia.Design;
using Devinno.Skia.Dialogs;
using Devinno.Skia.Extensions;
using Devinno.Skia.Icon;
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
    public abstract class DvInput : DvControl
    {
        #region Properties
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;

        public DvContentAlignment ContentAlignment { get; set; } = DvContentAlignment.MiddleCenter;

        public Padding TextPadding { get; set; } = new Padding();
        public string Unit { get; set; }
        public int UnitWidth { get; set; } = 36;
        public bool BackgroundDraw { get; set; } = true;

        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
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
                    var BoxColor = this.BoxColor ?? thm.InputColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BorderColor = this.BorderColor ?? thm.GetBorderColor(BoxColor, ParentContainer.GetBackColor());
                    var Corner = thm.Corner;

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        var rt = rtContent;

                        var cF = BoxColor;
                        var cB = BorderColor;
                        var cT = ForeColor;

                        if (BackgroundDraw)
                        {
                            thm.DrawBox(Canvas, rt, cF, cB, RoundType.Round, BoxStyle.Fill | BoxStyle.Border | BoxStyle.OutBevel);
                        }

                        DrawValue(Canvas, rtContent, rtText, rtUnit);
                                               
                        if (UnitWidth > 0 && !string.IsNullOrWhiteSpace(Unit))
                        {
                            #region Unit Sep
                            if (BackgroundDraw)
                            {
                                float h, s, b;
                                BoxColor.ToHsv(out h, out s, out b);

                                var szh = Convert.ToInt32(rtUnit.Height / 2);

                                p.StrokeWidth = 1;

                                
                                p.Color = b < 50 ? thm.GetInBevelColor(BoxColor) : cF.BrightnessTransmit(thm.BorderBrightness);
                                Canvas.DrawLine(rtUnit.Left + 0.5F, (rtContent.Top + (rtContent.Height / 2)) - (szh / 2) + 1, rtUnit.Left + 0.5F, (rtContent.Top + (rtContent.Height / 2)) + (szh / 2) + 1, p);

                                p.Color = b < 50 ? cF.BrightnessTransmit(thm.BorderBrightness) : thm.GetInBevelColor(BoxColor);
                                Canvas.DrawLine(rtUnit.Left + 1.5F, (rtContent.Top + (rtContent.Height / 2)) - (szh / 2) + 1, rtUnit.Left + 1.5F, (rtContent.Top + (rtContent.Height / 2)) + (szh / 2) + 1, p);
                            }
                            #endregion

                            if (FA.Contains(Unit)) Util.DrawIconFA(Canvas, Unit, FontSize, cT, rtUnit);
                            else Util.DrawText(Canvas, Unit, FontName, FontSize, DvFontStyle.Normal, cT, rtUnit);
                        }
                    }
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            bounds((rtContent, rtText, rtUnit) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    MouseEV(rtContent, rtText, rtUnit, x, y);
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #endregion
        #region Abstract
        protected abstract string Text { get; }
        protected abstract void MouseEV(SKRect rtContent, SKRect rtText, SKRect rtUnit, int x, int y);
        protected abstract void DrawValue(SKCanvas Canvas, SKRect rtContent, SKRect rtText, SKRect rtUnit);
        #endregion
        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect> act)
        {
            var szUnitW = 0;
            if (!string.IsNullOrWhiteSpace(Unit)) szUnitW = UnitWidth;

            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);
            var rtTextAll = Util.FromRect(rtContent.Left + TextPadding.Left, rtContent.Top + TextPadding.Top, rtContent.Width - (TextPadding.Left + TextPadding.Right), rtContent.Height - (TextPadding.Top + TextPadding.Bottom));
            var rtUnit = Util.FromRect(rtTextAll.Right - szUnitW, rtTextAll.Top, szUnitW, rtTextAll.Height);
            var rtText = Util.FromRect(rtTextAll.Left, rtTextAll.Top, rtTextAll.Width - rtUnit.Width, rtTextAll.Height - 1);

            act(rtContent, rtText, rtUnit);
        }
        #endregion
        #endregion
    }

    public class DvTextInput : DvInput
    {
        #region Properties
        public event EventHandler ValueChanged;
        private string sVal = "";
        public string Value
        {
            get => sVal;
            set { if (sVal != value) { sVal = value; ValueChanged?.Invoke(this, null); } }
        }

        protected override string Text => Value;
        #endregion
        #region MouseEV
        public int KeyboardWidth { get; set; } = 600;
        public int KeyboardHeight { get; set; } = 400;
        public KeyboardMode DefaultKeyboard { get; set; } = KeyboardMode.Korea;

        protected override void MouseEV(SKRect rtContent, SKRect rtText, SKRect rtUnit, int x, int y)
        {
            Design.Keyboard.Width = KeyboardWidth;
            Design.Keyboard.Height = KeyboardHeight;
            Design.Keyboard.ShowKeyboard("키보드", DefaultKeyboard, Value, (result) => { if (result != null) Value = result; });
        }
        #endregion
        #region DrawValue
        protected override void DrawValue(SKCanvas Canvas, SKRect rtContent, SKRect rtText, SKRect rtUnit)
        {
            var cT = ForeColor ?? (Design != null && Design.Theme != null ? Design.Theme.ForeColor : SKColors.Black);
            Util.DrawText(Canvas, Text, FontName, FontSize, DvFontStyle.Normal, cT, rtText, ContentAlignment);
        }
        #endregion
    }

    public class DvNumberInput<T> : DvInput where T : struct
    {
        #region Constructor
        public DvNumberInput()
        {
            if (typeof(T) == typeof(sbyte)) { }
            else if (typeof(T) == typeof(short)) { }
            else if (typeof(T) == typeof(int)) { }
            else if (typeof(T) == typeof(long)) { }
            else if (typeof(T) == typeof(byte)) { }
            else if (typeof(T) == typeof(ushort)) { }
            else if (typeof(T) == typeof(uint)) { }
            else if (typeof(T) == typeof(ulong)) { }
            else if (typeof(T) == typeof(float)) { }
            else if (typeof(T) == typeof(double)) { }
            else if (typeof(T) == typeof(decimal)) { }
            else throw new Exception("숫자 자료형이 아닙니다");
        }
        #endregion
        #region Properties
        public T? Minimum { get; set; } = null;
        public T? Maximum { get; set; } = null;

        public event EventHandler ValueChanged;
        private T sVal = default(T);
        public T Value
        {
            get => sVal;
            set { if (!sVal.Equals(value)) { sVal = value; ValueChanged?.Invoke(this, null); } }
        }

        public string FormatString { get; set; } = null;
        protected override string Text
        {
            get
            {
                var ret = "";
                if (typeof(T) == typeof(sbyte)) ret = FormatString != null ? ((sbyte)(object)Value).ToString(FormatString) : Value.ToString();
                else if (typeof(T) == typeof(short)) ret = FormatString != null ? ((short)(object)Value).ToString(FormatString) : Value.ToString();
                else if (typeof(T) == typeof(int)) ret = FormatString != null ? ((int)(object)Value).ToString(FormatString) : Value.ToString();
                else if (typeof(T) == typeof(long)) ret = FormatString != null ? ((long)(object)Value).ToString(FormatString) : Value.ToString();
                else if (typeof(T) == typeof(byte)) ret = FormatString != null ? ((byte)(object)Value).ToString(FormatString) : Value.ToString();
                else if (typeof(T) == typeof(ushort)) ret = FormatString != null ? ((ushort)(object)Value).ToString(FormatString) : Value.ToString();
                else if (typeof(T) == typeof(uint)) ret = FormatString != null ? ((uint)(object)Value).ToString(FormatString) : Value.ToString();
                else if (typeof(T) == typeof(ulong)) ret = FormatString != null ? ((ulong)(object)Value).ToString(FormatString) : Value.ToString();
                else if (typeof(T) == typeof(float)) ret = FormatString != null ? ((float)(object)Value).ToString(FormatString) : Value.ToString();
                else if (typeof(T) == typeof(double)) ret = FormatString != null ? ((double)(object)Value).ToString(FormatString) : Value.ToString();
                else if (typeof(T) == typeof(decimal)) ret = FormatString != null ? ((decimal)(object)Value).ToString(FormatString) : Value.ToString();

                return ret;
            }
        }
        #endregion
        #region MouseEV
        public int KeypadWidth { get; set; } = 300;
        public int KeypadHeight { get; set; } = 400;

        protected override void MouseEV(SKRect rtContent, SKRect rtText, SKRect rtUnit, int x, int y)
        {
            Design.Keypad.Width = KeypadWidth;
            Design.Keypad.Height = KeypadHeight;

            if (typeof(T) == typeof(sbyte))
            {
                Design.Keypad.ShowKeypad<sbyte>("키패드", (sbyte)(object)Value, (sbyte?)(object)Minimum, (sbyte?)(object)Maximum, (result) =>
                {
                    if (result.HasValue)
                    {
                        if (Maximum.HasValue && Minimum.HasValue) Value = (T)(object)MathTool.Constrain(Convert.ToSByte(result.Value), (sbyte)(object)Minimum.Value, (sbyte)(object)Maximum.Value);
                        else Value = (T)(object)Convert.ToSByte(result.Value);
                    }
                });
            }
            else if (typeof(T) == typeof(short))
            {
                Design.Keypad.ShowKeypad<short>("키패드", (short)(object)Value, (short?)(object)Minimum, (short?)(object)Maximum, (result) =>
                {
                    if (result.HasValue)
                    {
                        if (Maximum.HasValue && Minimum.HasValue) Value = (T)(object)MathTool.Constrain(Convert.ToInt16(result.Value), (short)(object)Minimum.Value, (short)(object)Maximum.Value);
                        else Value = (T)(object)Convert.ToInt16(result.Value);
                    }
                });
            }
            else if (typeof(T) == typeof(int))
            {
                Design.Keypad.ShowKeypad<int>("키패드", (int)(object)Value, (int?)(object)Minimum, (int?)(object)Maximum, (result) =>
                {
                    if (result.HasValue)
                    {
                        if (Maximum.HasValue && Minimum.HasValue) Value = (T)(object)MathTool.Constrain(Convert.ToInt32(result.Value), (int)(object)Minimum.Value, (int)(object)Maximum.Value);
                        else Value = (T)(object)Convert.ToInt32(result.Value);
                    }
                });
            }
            else if (typeof(T) == typeof(long))
            {
                Design.Keypad.ShowKeypad<long>("키패드", (long)(object)Value, (long?)(object)Minimum, (long?)(object)Maximum, (result) =>
                {
                    if (result.HasValue)
                    {
                        if (Maximum.HasValue && Minimum.HasValue) Value = (T)(object)MathTool.Constrain(Convert.ToInt64(result.Value), (long)(object)Minimum.Value, (long)(object)Maximum.Value);
                        else Value = (T)(object)Convert.ToInt64(result.Value);
                    }
                });
            }
            else if (typeof(T) == typeof(byte))
            {
                Design.Keypad.ShowKeypad<byte>("키패드", (byte)(object)Value, (byte?)(object)Minimum, (byte?)(object)Maximum, (result) =>
                {
                    if (result.HasValue)
                    {
                        if (Maximum.HasValue && Minimum.HasValue) Value = (T)(object)MathTool.Constrain(Convert.ToByte(result.Value), (byte)(object)Minimum.Value, (byte)(object)Maximum.Value);
                        else Value = (T)(object)Convert.ToByte(result.Value);
                    }
                });
            }
            else if (typeof(T) == typeof(ushort))
            {
                Design.Keypad.ShowKeypad<ushort>("키패드", (ushort)(object)Value, (ushort?)(object)Minimum, (ushort?)(object)Maximum, (result) =>
                {
                    if (result.HasValue)
                    {
                        if (Maximum.HasValue && Minimum.HasValue) Value = (T)(object)MathTool.Constrain(Convert.ToUInt16(result.Value), (ushort)(object)Minimum.Value, (ushort)(object)Maximum.Value);
                        else Value = (T)(object)Convert.ToUInt16(result.Value);
                    }
                });
            }
            else if (typeof(T) == typeof(uint))
            {
                Design.Keypad.ShowKeypad<uint>("키패드", (uint)(object)Value, (uint?)(object)Minimum, (uint?)(object)Maximum, (result) =>
                {
                    if (result.HasValue)
                    {
                        if (Maximum.HasValue && Minimum.HasValue) Value = (T)(object)MathTool.Constrain(Convert.ToUInt32(result.Value), (uint)(object)Minimum.Value, (uint)(object)Maximum.Value);
                        else Value = (T)(object)Convert.ToUInt32(result.Value);
                    }
                });
            }
            else if (typeof(T) == typeof(ulong))
            {
                Design.Keypad.ShowKeypad<ulong>("키패드", (ulong)(object)Value, (ulong?)(object)Minimum, (ulong?)(object)Maximum, (result) =>
                {
                    if (result.HasValue)
                    {
                        if (Maximum.HasValue && Minimum.HasValue) Value = (T)(object)MathTool.Constrain(Convert.ToUInt64(result.Value), (ulong)(object)Minimum.Value, (ulong)(object)Maximum.Value);
                        else Value = (T)(object)Convert.ToUInt64(result.Value);
                    }
                });
            }
            else if (typeof(T) == typeof(float))
            {
                Design.Keypad.ShowKeypad<float>("키패드", (float)(object)Value, (float?)(object)Minimum, (float?)(object)Maximum, (result) =>
                {
                    if (result.HasValue)
                    {
                        if (Maximum.HasValue && Minimum.HasValue) Value = (T)(object)MathTool.Constrain(Convert.ToSingle(result.Value), (float)(object)Minimum.Value, (float)(object)Maximum.Value);
                        else Value = (T)(object)Convert.ToSingle(result.Value);
                    }
                });
            }
            else if (typeof(T) == typeof(double))
            {
                Design.Keypad.ShowKeypad<double>("키패드", (double)(object)Value, (double?)(object)Minimum, (double?)(object)Maximum, (result) =>
                {
                    if (result.HasValue)
                    {
                        if (Maximum.HasValue && Minimum.HasValue) Value = (T)(object)MathTool.Constrain(Convert.ToDouble(result.Value), (double)(object)Minimum.Value, (double)(object)Maximum.Value);
                        else Value = (T)(object)Convert.ToDouble(result.Value);
                    }
                });
            }
            else if (typeof(T) == typeof(decimal))
            {
                Design.Keypad.ShowKeypad<decimal>("키패드", (decimal)(object)Value, (decimal?)(object)Minimum, (decimal?)(object)Maximum, (result) =>
                {
                    if (result.HasValue)
                    {
                        if (Maximum.HasValue && Minimum.HasValue) Value = (T)(object)MathTool.Constrain(Convert.ToDecimal(result.Value), (decimal)(object)Minimum.Value, (decimal)(object)Maximum.Value);
                        else Value = (T)(object)Convert.ToDecimal(result.Value);
                    }
                });
            }
        }

        #endregion
        #region DrawValue
        protected override void DrawValue(SKCanvas Canvas, SKRect rtContent, SKRect rtText, SKRect rtUnit)
        {
            var cT = ForeColor ?? (Design != null && Design.Theme != null ? Design.Theme.ForeColor : SKColors.Black);
            Util.DrawText(Canvas, Text, FontName, FontSize, DvFontStyle.Normal, cT, rtText, ContentAlignment);
        }
        #endregion
    }

    public class DvHexInput : DvInput
    {
        #region Properties
        public int? Minimum { get; set; } = null;
        public int? Maximum { get; set; } = null;

        public event EventHandler ValueChanged;
        private int sVal = 0;
        public int Value
        {
            get => sVal;
            set { if (sVal != value) { sVal = value; ValueChanged?.Invoke(this, null); } }
        }

        protected override string Text =>  Value.ToString("X");
        #endregion
        #region MouseEV
        public int KeypadWidth { get; set; } = 360;
        public int KeypadHeight { get; set; } = 400;

        protected override void MouseEV(SKRect rtContent, SKRect rtText, SKRect rtUnit, int x, int y)
        {
            Design.Keypad.Width = KeypadWidth;
            Design.Keypad.Height = KeypadHeight;
            Design.Keypad.ShowHex("키패드", Value.ToString("X"), (result) =>
            {
                if (!string.IsNullOrWhiteSpace(result)) Value = Convert.ToInt32(result, 16);
            });
        }
        #endregion
        #region DrawValue
        protected override void DrawValue(SKCanvas Canvas, SKRect rtContent, SKRect rtText, SKRect rtUnit)
        {
            var cT = ForeColor ?? (Design != null && Design.Theme != null ? Design.Theme.ForeColor : SKColors.Black);
            Util.DrawText(Canvas, Text, FontName, FontSize, DvFontStyle.Normal, cT, rtText, ContentAlignment);
        }
        #endregion
    }

    public class DvBoolInput : DvInput
    {
        #region Member Variable
        Animation ani = new Animation();
        #endregion
        #region Properties
        public string OnText { get; set; } = "ON";
        public string OffText { get; set; } = "OFF";
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;

        public event EventHandler ValueChanged;
        private bool sVal = false;
        public bool Value
        {
            get => sVal;
            set { if (sVal != value) { sVal = value; ValueChanged?.Invoke(this, null); } }
        }

        protected override string Text => Value ? OnText : OffText;
        #endregion
        #region MouseEV
        protected override void MouseEV(SKRect rtContent, SKRect rtText, SKRect rtUnit, int x, int y)
        {
            bounds(rtContent, (rtOn, rtOff) =>
            {
                if (CollisionTool.Check(rtOn, x, y) && !Value) { Value = true; if (Design.Animation) { ani.Stop(); ani.Start(DvDesign.ANI); } }
                if (CollisionTool.Check(rtOff, x, y) && Value) { Value = false; if (Design.Animation) { ani.Stop(); ani.Start(DvDesign.ANI); } }
            });
        }
        #endregion
        #region DrawValue
        protected override void DrawValue(SKCanvas Canvas, SKRect rtContent, SKRect rtText, SKRect rtUnit)
        {
            bounds(rtContent, (rtOn, rtOff) =>
            {
                using (var p = new SKPaint { IsAntialias = true })
                {
                    var thm = Design.Theme;
                    var BoxColor = this.BoxColor ?? thm.InputColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var cL = ForeColor;
                    var cD = Util.FromArgb(75, cL);
                    var cOn = Value ? cL : cD;
                    var cOff = Value ? cD : cL;

                    var isOn = Value ? 12 : 0;
                    var isOff = Value ? 0 : 12;
                    var igOn = Value ? 5 : 0;
                    var igOff = Value ? 0 : 5;

                    if (Design.Animation && ani.IsPlaying)
                    {
                        if (Value)
                        {
                            cOn = ani.Value(AnimationAccel.DCL, cD, cL);
                            cOff = ani.Value(AnimationAccel.DCL, cL, cD);
                            isOn = ani.Value(AnimationAccel.DCL, 0, 12);
                            isOff = ani.Value(AnimationAccel.DCL, 12, 0);
                            igOn = ani.Value(AnimationAccel.DCL, 0, 5);
                            igOff = ani.Value(AnimationAccel.DCL, 5, 0);
                        }
                        else
                        {
                            cOn = ani.Value(AnimationAccel.DCL, cL, cD);
                            cOff = ani.Value(AnimationAccel.DCL, cD, cL);
                            isOff = ani.Value(AnimationAccel.DCL, 0, 12);
                            isOn = ani.Value(AnimationAccel.DCL, 12, 0);
                            igOff = ani.Value(AnimationAccel.DCL, 0, 5);
                            igOn = ani.Value(AnimationAccel.DCL, 5, 0);
                        }
                    }

                    Util.DrawTextIcon(Canvas, OnText, FontName, FontSize, DvFontStyle.Normal, igOn, "fa-check", isOn, cOn, rtOn, IconAlignment);
                    Util.DrawTextIcon(Canvas, OffText, FontName, FontSize, DvFontStyle.Normal, igOff, "fa-check", isOff, cOff, rtOff, IconAlignment);

                    #region Unit Sep
                    if (BackgroundDraw)
                    {
                        float h, s, b;
                        BoxColor.ToHsv(out h, out s, out b);

                        var szh = Convert.ToInt32(rtUnit.Height / 2);
                        var x = Convert.ToInt32(rtContent.MidX);

                        p.StrokeWidth = 1;

                        p.Color = b < 50 ? thm.GetInBevelColor(BoxColor) : BoxColor.BrightnessTransmit(thm.BorderBrightness);
                        Canvas.DrawLine(x + 0.5F, (rtContent.Top + (rtContent.Height / 2)) - (szh / 2) + 1, x + 0.5F, (rtContent.Top + (rtContent.Height / 2)) + (szh / 2) + 1, p);

                        p.Color = b < 50 ? BoxColor.BrightnessTransmit(thm.BorderBrightness) : thm.GetInBevelColor(BoxColor);
                        Canvas.DrawLine(x + 1.5F, (rtContent.Top + (rtContent.Height / 2)) - (szh / 2) + 1, x + 1.5F, (rtContent.Top + (rtContent.Height / 2)) + (szh / 2) + 1, p);
                    }
                    #endregion
                }
            });
        }
        #endregion
        #region bounds
        void bounds(SKRect rtValue, Action<SKRect, SKRect> act)
        {
            var w = rtValue.Width / 2F;
            var rtOn = Util.FromRect(rtValue.Left, rtValue.Top, w, rtValue.Height);
            var rtOff = Util.FromRect(rtValue.Left + w, rtValue.Top, w, rtValue.Height);
            act(rtOn, rtOff);
        }
        #endregion
    }

    public class DvSelectorInput : DvInput
    {
        #region Member Variable
        bool bExpand = false;
        #endregion
        #region Properties
        public List<TextIconItem> Items { get; private set; } = new List<TextIconItem>();
        public int ItemViewCount { get; set; } = 5;
        public int ItemHeight { get; set; } = 30;
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;

        public event EventHandler SelectedIndexChanged;
        private int sVal = -1;
        public int SelectedIndex
        {
            get => sVal;
            set
            {
                if (sVal != value)
                {
                    sVal = value;
                    SelectedIndexChanged?.Invoke(this, null);
                }
            }
        }

        protected override string Text => SelectedIndex >= 0 && SelectedIndex < Items.Count ? Items[SelectedIndex].Text : "";
        #endregion
        #region MouseEV
        protected override void MouseEV(SKRect rtContent, SKRect rtText, SKRect rtUnit, int x, int y)
        {
            if (CollisionTool.Check(rtContent, x, y) && Items.Count > 0)
            {
                var ds = Design;
                bExpand = true;

                ds.SelectorBox.ItemViewCount = ItemViewCount;
                ds.SelectorBox.ItemHeight = ItemHeight;

                ds.SelectorBox.ShowSelector("항목 선택", SelectedIndex, Items, (result) => { if (result.HasValue) SelectedIndex = result.Value; bExpand = false; });
            }
        }
        #endregion
        #region DrawValue
        protected override void DrawValue(SKCanvas Canvas, SKRect rtContent, SKRect rtText, SKRect rtUnit)
        {
            if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
            {
                //if (string.IsNullOrWhiteSpace(Unit)) Unit = "fa-check";
                var cT = ForeColor ?? Design.Theme.ForeColor;
                var v = Items[SelectedIndex];
                Util.DrawTextIcon(Canvas, v.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, v.IconString, IconSize, cT, rtText, IconAlignment);
            }
        }
        #endregion
    }
}

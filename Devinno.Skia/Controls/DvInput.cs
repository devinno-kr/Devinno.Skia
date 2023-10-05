using Devinno.Collections;
using Devinno.Skia.Collections;
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
    public abstract class DvInput : Design.DvControl
    {
        #region Properties
        #region BackgroundDraw
        public bool BackgroundDraw { get; set; } = true;
        #endregion

        #region Color
        public SKColor? InputColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;
        #endregion

        #region Round
        public DvRoundType? Round { get; set; } = null;
        #endregion
        #endregion

        #region Abstract
        protected abstract void DrawValue(SKCanvas canvas, DvTheme thm, SKRect rtValue);
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region var
                    var InputColor = this.InputColor ?? thm.InputColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var BorderColor = this.BorderColor ?? thm.GetBorderColor(InputColor, BackColor);
                    var Round = this.Round ?? DvRoundType.All;
                    #endregion

                    thm.DrawInput(Canvas,
                        rtContent,
                        InputColor, BorderColor, ForeColor, BackColor,
                        Round, BackgroundDraw);

                    DrawValue(Canvas, thm, rtContent);
                }

            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent) =>
            {
                if (CollisionTool.Check(rtContent, x, y))
                {
                    Design?.Input(this);
                }
            });
         
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);
            act(rtContent);
        }
        #endregion
        #endregion
    }

    public class DvInputText : DvInput
    {
        #region Properties
        #region Value
        private string sVal = "";
        public string Value
        {
            get => sVal;
            set
            {
                if (sVal != value)
                {
                    sVal = value;
                    ValueChanged?.Invoke(this, null);
                }
            }
        }
        #endregion

        public DvContentAlignment ContentAlignment { get; set; } = DvContentAlignment.MiddleCenter;
        public string Text => Value;
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;

        public int KeyboardWidth { get; set; } = 600;
        public int KeyboardHeight { get; set; } = 400;
        #endregion

        #region Event
        public event EventHandler ValueChanged;
        #endregion

        #region DrawValue
        protected override void DrawValue(SKCanvas canvas, DvTheme thm, SKRect rtValue)
        {
            if (!string.IsNullOrWhiteSpace(Text))
            {
                var cF = ForeColor ?? thm.ForeColor;
                Util.DrawText(canvas, Text, FontName, FontSize, FontStyle, cF, rtValue, ContentAlignment);
            }
        }
        #endregion

        #region Mouse
        protected override void OnMouseClick(float x, float y)
        {
            Areas((rtContent) => {
            
                if(CollisionTool.Check(rtContent, x, y))
                {
                    DvDialogs.Keyboard.ShowKeyboard("키보드", Value, (result) =>
                    {
                        if (result != null)
                        {
                            Value = result;
                        }
                    });
                }

            });
            base.OnMouseClick(x, y);
        }
        #endregion
    }

    public class DvInputNumber<T> : DvInput where T : struct
    {
        #region Properties
        #region Value
        private T sVal = default(T);
        public T Value
        {
            get => sVal;
            set
            {
                if (!sVal.Equals(value))
                {
                    sVal = value; 
                    ValueChanged?.Invoke(this, null);
                }
            }
        }

        public T? Minimum { get; set; } = null;
        public T? Maximum { get; set; } = null;
        #endregion

        public DvContentAlignment ContentAlignment { get; set; } = DvContentAlignment.MiddleCenter;
        public string FormatString { get; set; } = null;
        public string Text => ValueTool.Text<T>(Value, FormatString);
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;

        public int KeypadWidth { get; set; } = 300;
        public int KeypadHeight { get; set; } = 400;
        #endregion

        #region Event
        public event EventHandler ValueChanged;
        #endregion

        #region Constructor
        public DvInputNumber()
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

        #region DrawValue
        protected override void DrawValue(SKCanvas canvas, DvTheme thm, SKRect rtValue)
        {
            if (!string.IsNullOrWhiteSpace(Text))
            {
                var cF = ForeColor ?? thm.ForeColor;
                Util.DrawText(canvas, Text, FontName, FontSize, FontStyle, cF, rtValue, ContentAlignment);
            }
        }
        #endregion

        #region Mouse
        protected override void OnMouseClick(float x, float y)
        {
            Areas((rtContent) => {

                if (CollisionTool.Check(rtContent, x, y))
                {
                    DvDialogs.Keypad.ShowKeypad<T>("키패드", Value, Minimum, Maximum, (result) =>
                    {
                        if(result.HasValue)
                        {
                            Value = result.Value;
                        }

                    });
                }

            });
            base.OnMouseClick(x, y);
        }
        #endregion
    }

    public class DvInputBool : DvInput
    {
        #region Properties
        #region Value
        private bool bValue = false;
        public bool Value
        {
            get { return bValue; }
            set
            {
                if (bValue != value)
                {
                    bValue = value;
                    ValueChanged?.Invoke(this, null);

                    if (Animation)
                    {
                        ani.Stop();
                        ani.Start(DvDesign.ANI, bValue ? "On" : "Off");
                    }
                }
            }
        }
        #endregion

        #region Text
        public string OnText { get; set; } = "ON";
        public string OffText { get; set; } = "OFF";

        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        #endregion

        public bool Animation => Design != null ? Design.Animation : false;
        #endregion

        #region Member Variable
        Animation ani = new Animation();
        #endregion

        #region Event
        public event EventHandler ValueChanged;
        #endregion

        #region DrawValue
        protected override void DrawValue(SKCanvas canvas, DvTheme thm, SKRect rtValue)
        {
            Areas2(rtValue, (rtOn, rtOff) =>
            {
                var foreColor = ForeColor ?? thm.ForeColor;

                thm.DrawValueOnOff(canvas,
                    rtValue, rtOn, rtOff,
                    foreColor,
                    OnText, OffText, FontName, FontSize, FontStyle,
                    Value,
                    Animation, ani);
            });
        }
        #endregion

        #region Mouse
        protected override void OnMouseClick(float x, float y)
        {
            Areas((rtContent) => {

                Areas2(rtContent, (rtOn, rtOff) =>
                {
                    if (CollisionTool.Check(rtOn, x, y)) Value = true;
                    else if (CollisionTool.Check(rtOff, x, y)) Value = false;
                });

            });
            base.OnMouseClick(x, y);
        }
        #endregion

        #region Areas2
        void Areas2(SKRect rtValue, Action<SKRect, SKRect> act)
        {
            var w = rtValue.Width / 2F;
            var rtOn = Util.FromRect(rtValue.Left, rtValue.Top, w, rtValue.Height);
            var rtOff = Util.FromRect(rtValue.Left + w, rtValue.Top, w, rtValue.Height);
            act(rtOn, rtOff);
        }
        #endregion
    }

    public class DvInputSelector : DvInput
    {
        #region Properties
        #region Items
        public List<SelectorItem> Items { get; private set; } = new List<SelectorItem>();
        #endregion

        #region Text / Icon
        public SelectorItem TextIcon => SelectedIndex >= 0 && SelectedIndex < Items.Count ? Items[SelectedIndex] : null; 
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;

        public float IconSize { get; set; } = 12;
        public float IconGap { get; set; } = 5;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        #endregion

        #region SelectedIndex
        private int nSelectedIndex = -1;
        public int SelectedIndex
        {
            get => nSelectedIndex;
            set
            {
                if (nSelectedIndex != value)
                {
                    nSelectedIndex = value;
                    SelectedIndexChanged?.Invoke(this, null);
                }
            }
        }
        #endregion

        #region Animation
        public bool Animation => Design != null ? Design.Animation : false;
        #endregion
        #endregion

        #region Member Variable
        private bool bPrev = false, bNext = false;

        private Animation ani = new Animation();
        #endregion

        #region Event
        public event EventHandler SelectedIndexChanged;
        #endregion

        #region Constructor
        public DvInputSelector()
        {
        }
        #endregion

        #region DrawValue
        protected override void DrawValue(SKCanvas canvas, DvTheme thm, SKRect rtValue)
        {
            Areas2(rtValue, (rtPrev, rtValue2, rtNext, rtItems) =>
            {
                var ForeColor = this.ForeColor ?? thm.ForeColor;
                var BackColor = ParentContainer.GetBackColor();

                thm.DrawSelector(canvas,
                            rtValue, rtPrev, rtValue2, rtNext, rtItems,
                            ForeColor, ForeColor, BackColor,
                            FontName, FontSize, FontStyle,
                            IconSize, IconGap, IconAlignment,
                            Items, false, DvDirectionHV.Horizon, Round,
                            SelectedIndex,
                            bPrev, bNext, Animation, ani);
            });
        }
        #endregion

        #region Mouse
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent) => {

                Areas2(rtContent, (rtPrev, rtValue, rtNext, lsv) =>
                {
                    if (CollisionTool.Check(rtPrev, x, y)) bPrev = true;
                    if (CollisionTool.Check(rtNext, x, y)) bNext = true;
                });

            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent) =>
            {
                Areas2(rtContent, (rtPrev, rtValue, rtNext, lsv) =>
                {
                    #region PrevButton
                    if (bPrev)
                    {
                        if (CollisionTool.Check(rtPrev, x, y))
                        {
                            if (Items.Count > 0)
                            {
                                if (SelectedIndex > 0)
                                {
                                    SelectedIndex--;

                                    if (Animation)
                                    {
                                        ani.Stop();
                                        ani.Start(DvDesign.ANI2, "Prev");
                                    }
                                }
                            }
                            else SelectedIndex = -1;

                        }
                        bPrev = false;
                    }
                    #endregion
                    #region NextButton
                    if (bNext)
                    {
                        if (CollisionTool.Check(rtNext, x, y))
                        {
                            if (Items.Count > 0)
                            {
                                if (SelectedIndex < Items.Count - 1)
                                {
                                    SelectedIndex++;

                                    if (Animation)
                                    {
                                        ani.Stop();
                                        ani.Start(DvDesign.ANI2, "Next");
                                    }
                                }
                            }


                        }
                        bNext = false;
                    }
                    #endregion
                });
            });
         
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        public void Areas2(SKRect rtContent, Action<SKRect, SKRect, SKRect, List<SKRect>> act)
        {
            var btnSize = Height;
            var ls = new List<SizeInfo>();
            ls.Add(new SizeInfo(DvSizeMode.Pixel, btnSize));
            ls.Add(new SizeInfo(DvSizeMode.Percent, 100F));
            ls.Add(new SizeInfo(DvSizeMode.Pixel, btnSize));

            var rts = Util.DevideSizeH(rtContent, ls);
            var rtPrev = rts[0];
            var rtValue = rts[1];
            var rtNext = rts[2];

            var lsv = new List<SKRect>();
            var sx = rtValue.Left;

            if (Animation && ani.IsPlaying)
            {
                var v = Convert.ToSingle(ani.Value(AnimationAccel.DCL, rtValue.Width, 0));
                sx = rtValue.Left + -(SelectedIndex * rtValue.Width) + (ani.Variable == "Prev" ? -v : v);
            }
            else
            {
                sx = rtValue.Left + -(SelectedIndex * rtValue.Width);
            }

            foreach (var v in Items)
            {
                lsv.Add(Util.FromRect(sx, rtValue.Top, rtValue.Width, rtValue.Height));
                sx += rtValue.Width;
            }

            act(rtPrev, rtValue, rtNext, lsv);
        }
        #endregion
    }
}

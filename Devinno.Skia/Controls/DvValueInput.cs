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
    public abstract class DvValueInput : DvControl
    {
        #region Properties
        #region Title
        public DvTextIcon TitleTextIcon { get; } = new DvTextIcon();

        public string TitleIconString { get => TitleTextIcon.IconString; set => TitleTextIcon.IconString = value; }
        public float TitleIconSize { get => TitleTextIcon.IconSize; set => TitleTextIcon.IconSize = value; }
        public float TitleIconGap { get => TitleTextIcon.IconGap; set => TitleTextIcon.IconGap = value; }
        public DvTextIconAlignment TitleIconAlignment { get => TitleTextIcon.IconAlignment; set => TitleTextIcon.IconAlignment = value; }

        public string Title { get => TitleTextIcon.Text; set => TitleTextIcon.Text = value; }
        public string TitleFontName { get => TitleTextIcon.FontName; set => TitleTextIcon.FontName = value; }
        public float TitleFontSize { get => TitleTextIcon.FontSize; set => TitleTextIcon.FontSize = value; }
        public DvFontStyle TitleFontStyle { get => TitleTextIcon.FontStyle; set => TitleTextIcon.FontStyle = value; }

        public float? TitleAreaSize { get; set; } = null;
        #endregion

        #region Buttons
        public List<ButtonInfo> Buttons { get; } = new List<ButtonInfo>();

        public float? ButtonAreaSize { get; set; } = null;
        #endregion

        #region Direction
        public virtual DvDirectionHV Direction { get; set; } = DvDirectionHV.Horizon;
        #endregion

        #region BackgroundDraw
        public bool BackgroundDraw { get; set; } = true;
        #endregion

        #region Color
        public SKColor? TitleColor { get; set; } = null;
        public SKColor? InputColor { get; set; } = null;
        public SKColor? ButtonColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        #endregion

        #region Round
        public DvRoundType? Round { get; set; } = null;
        #endregion
        #endregion

        #region Member Variable
        bool bValueDown = false;
        #endregion

        #region Events
        public event EventHandler<ButtonsClickedEventArgs> ButtonClick;
        public event EventHandler<ButtonsClickedEventArgs> ButtonDown;
        public event EventHandler<ButtonsClickedEventArgs> ButtonUp;
        #endregion

        #region Abstract
        protected abstract void DrawValue(SKCanvas canvas, DvTheme thm, SKRect rtValue);
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            var ds = Design;
            var thm = Design?.Theme;
            if (ds != null && thm != null)
            {
                Areas((rtContent, rtTitle, rtValue, rtButtons) =>
                {
                    #region var
                    var TitleColor = this.TitleColor ?? thm.TitleColor;
                    var InputColor = this.InputColor ?? thm.InputColor;
                    var ButtonColor = this.ButtonColor ?? thm.ButtonColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var BorderColor = this.BorderColor ?? thm.GetBorderColor(InputColor, BackColor);
                    var Round = this.Round ?? DvRoundType.All;
                    #endregion
                    #region round
                    var useT = TitleAreaSize.HasValue && TitleAreaSize.Value > 0;
                    var useB = ButtonAreaSize.HasValue && ButtonAreaSize.Value > 0 && Buttons.Count > 0;

                    var rnds = Util.Rounds(Direction, Round, (TitleAreaSize.HasValue ? 1 : 0) + 1 + (ButtonAreaSize.HasValue ? 1 : 0));

                    var rndTitle = TitleAreaSize.HasValue ? rnds[0] : DvRoundType.Rect;
                    var rndValue = TitleAreaSize.HasValue ? rnds[1] : rnds[0];
                    var rndButton = ButtonAreaSize.HasValue ? rnds[rnds.Length - 1] : DvRoundType.Rect;
                    var rndButtons = Util.Rounds(DvDirectionHV.Horizon, BackgroundDraw ? rndButton : Round, (ButtonAreaSize.HasValue ? Buttons.Count : 0));
                    #endregion

                    #region Title
                    if (TitleAreaSize.HasValue)
                    {
                        thm.DrawTitle(Canvas,
                            rtTitle.Value,
                            TitleColor, BorderColor, ForeColor, BackColor,
                            TitleTextIcon,
                            rndTitle, BackgroundDraw);
                    }
                    #endregion
                    #region Value
                    {
                        thm.DrawInput(Canvas,
                            rtValue,
                            InputColor, BorderColor, ForeColor, BackColor,
                            rndValue, BackgroundDraw, false);

                        DrawValue(Canvas, thm, rtValue);
                    }
                    #endregion
                    #region Buttons
                    if (ButtonAreaSize.HasValue)
                    {
                        for (int i = 0; i < Buttons.Count; i++)
                        {
                            var rt = rtButtons[i];
                            var btn = Buttons[i];
                            var rnd = rndButtons[i];
                            var cBTN = ButtonColor;
                            var cBOR = thm.GetBorderColor(cBTN, BackColor);

                            thm.DrawButton(Canvas, rt, ButtonColor, BorderColor, ForeColor, BackColor,
                                btn, rnd, true, true, DvContentAlignment.MiddleCenter, btn.ButtonDownState);
                        }
                    }
                    #endregion
                    #region Border
                    if (BackgroundDraw) thm.DrawBox(Canvas, rtContent, InputColor, BorderColor, Round, BoxStyle.Border, thm.Corner);
                    #endregion

                });
            }
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtTitle, rtValue, rtButtons) =>
            {
                if (ButtonAreaSize.HasValue)
                {
                    for (int i = 0; i < Buttons.Count; i++)
                    {
                        var btn = Buttons[i];
                        var rt = rtButtons[i];
                        if (CollisionTool.Check(rt, x, y))
                        {
                            btn.ButtonDownState = true;
                            ButtonDown?.Invoke(this, new ButtonsClickedEventArgs(btn));
                        }
                    }

                    if (Buttons.Where(x => x.ButtonDownState).Count() > 0)
                    {
                        Design?.Input(this);
                    }
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtTitle, rtValue, rtButtons) =>
            {
                if (ButtonAreaSize.HasValue)
                {
                    for (int i = 0; i < Buttons.Count; i++)
                    {
                        var btn = Buttons[i];
                        var rt = rtButtons[i];
                        if (btn.ButtonDownState)
                        {
                            btn.ButtonDownState = false;

                            ButtonUp?.Invoke(this, new ButtonsClickedEventArgs(btn));
                            if (CollisionTool.Check(rt, x, y)) ButtonClick?.Invoke(this, new ButtonsClickedEventArgs(btn));
                        }
                    }
                }

            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #region OnMouseDoubleClick
        protected override void OnMouseDoubleClick(float x, float y)
        {
            Areas((rtContent, rtTitle, rtValue, rtButtons) =>
            {
            });
            base.OnMouseDoubleClick(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect?, SKRect, SKRect[]> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);

            if (Direction == DvDirectionHV.Horizon)
            {
                var ls = new List<SizeInfo>();
                if (TitleAreaSize.HasValue) ls.Add(new SizeInfo(DvSizeMode.Pixel, TitleAreaSize.Value));
                ls.Add(new SizeInfo(DvSizeMode.Percent, 100));
                if (ButtonAreaSize.HasValue) ls.Add(new SizeInfo(DvSizeMode.Pixel, ButtonAreaSize.Value));
                var rts = Util.DevideSizeH(rtContent, ls);

                SKRect? rtTitle = TitleAreaSize.HasValue ? rts.FirstOrDefault() : null;
                SKRect rtValue = TitleAreaSize.HasValue ? rts[1] : rts[0];
                SKRect? rtButton = ButtonAreaSize.HasValue ? rts.LastOrDefault() : null;

                SKRect[] rtButtons = ButtonAreaSize.HasValue ? Util.DevideSizeH(rtButton.Value, Buttons.Select(x => x.Size).ToList()) : null;
                act(rtContent, rtTitle, rtValue, rtButtons);
            }
            else
            {
                var ls = new List<SizeInfo>();
                if (TitleAreaSize.HasValue) ls.Add(new SizeInfo(DvSizeMode.Pixel, TitleAreaSize.Value));
                ls.Add(new SizeInfo(DvSizeMode.Percent, 100));
                if (ButtonAreaSize.HasValue) ls.Add(new SizeInfo(DvSizeMode.Pixel, ButtonAreaSize.Value));
                var rts = Util.DevideSizeV(rtContent, ls);

                SKRect? rtTitle = TitleAreaSize.HasValue ? rts.FirstOrDefault() : null;
                SKRect rtValue = TitleAreaSize.HasValue ? rts[1] : rts[0];
                SKRect? rtButton = ButtonAreaSize.HasValue ? rts.LastOrDefault() : null;

                SKRect[] rtButtons = ButtonAreaSize.HasValue ? Util.DevideSizeH(rtButton.Value, Buttons.Select(x => x.Size).ToList()) : null;
                act(rtContent, rtTitle, rtValue, rtButtons);
            }
        }
        #endregion
        #endregion
    }

    public class DvValueInputText : DvValueInput
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
        private string Text => Value;
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;

        public int KeyboardWidth { get; set; } = 600;
        public int KeyboardHeight { get; set; } = 400;
        public KeyboardMode KeyboardMode { get; set; } = KeyboardMode.English;
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
            Areas((rtContent, rtTitle, rtValue, rtButtons) =>
            {
                if (CollisionTool.Check(rtValue, x, y))
                {
                    DvDialogs.Keyboard.ShowKeyboard(Title, KeyboardMode, Value, (result) =>
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

    public class DvValueInputNumber<T> : DvValueInput where T : struct
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
        public DvValueInputNumber()
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
            Areas((rtContent, rtTitle, rtValue, rtButtons) =>
            {
                if (CollisionTool.Check(rtValue, x, y))
                {
                    DvDialogs.Keypad.ShowKeypad<T>(Title, Value, Minimum, Maximum, (result) =>
                    {
                        if (result.HasValue)
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

    public class DvValueInputBool : DvValueInput
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
            Areas((rtContent, rtTitle, rtValue, rtButtons) =>
            {
                Areas2(rtValue, (rtOn, rtOff) =>
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

    public class DvValueInputSelector : DvValueInput
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
        public DvValueInputSelector()
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
                            Items, false, Direction, Round,
                            SelectedIndex,
                            bPrev, bNext, Animation, ani);
            });
        }
        #endregion

        #region Mouse
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtTitle, rtValue, rtButtons) =>
            {
                Areas2(rtValue, (rtPrev, rtValue, rtNext, lsv) =>
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
            Areas((rtContent, rtTitle, rtValue, rtButtons) =>
            {
                Areas2(rtValue, (rtPrev, rtValue, rtNext, lsv) =>
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

    public class DvValueInputCombo : DvValueInput
    {
        #region Properties
        #region Items
        public EventList2<ComboBoxItem> Items { get; private set; } = new EventList2<ComboBoxItem>();
        #endregion

        #region Text / Icon
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

        #region Shape
        public int MaximumViewCount { get; set; } = 8;
        public int ItemHeight { get; set; } = 30;
        public SKColor? SelectedColor { get; set; } = null;
        #endregion

        #region Animation
        public bool Animation => Design != null ? Design.Animation : false;
        #endregion
        #endregion

        #region Member Variable
        bool bOpen => Design.VisibleDropDownWindow(ddwnd);
        DropDownWindow ddwnd = new DropDownWindow();
        #endregion

        #region Event
        public event EventHandler SelectedIndexChanged;
        #endregion

        #region Constructor
        public DvValueInputCombo()
        {
            
        }
        #endregion

        #region DrawValue
        protected override void DrawValue(SKCanvas canvas, DvTheme thm, SKRect rtValue)
        {
            Areas2(rtValue, (rtIco, rtText) =>
            {
                var SelectedColor = thm.PointColor;
                var ListBackColor = thm.ListBackColor;
                var InputColor = this.InputColor ?? thm.InputColor;
                var ForeColor = this.ForeColor ?? thm.ForeColor;
                var BackColor = ParentContainer.GetBackColor();

                thm.DrawComboBox(canvas,
                     rtValue, rtIco, rtText,
                     InputColor, ForeColor, BackColor, SelectedColor, ListBackColor,
                     FontName, FontSize, FontStyle, IconSize, IconAlignment, IconGap,
                     Items, SelectedIndex,
                     bOpen, ddwnd.Reverse, true);
            });
        }
        #endregion

        #region Override
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtTitle, rtValue, rtButtons) =>
            {
                Areas2(rtValue, (rtIco, rtText) =>
                {
                    var ds = Design;
                    var thm = Design?.Theme;
                    if (ds != null && thm != null)
                    {
                        if (CollisionTool.Check(rtValue, x, y) && Items.Count > 0)
                        {
                            var rt = rtValue;

                            var sx = ScreenX;
                            var sy = ScreenY - 1;
                            var sw = rt.Width;
                            var sh = rt.Height;

                            var vh = Math.Max(Math.Min(Items.Count, MaximumViewCount) * 30, sh) + 2;

                            var rts = Util.FromRect(sx + rt.Left, sy + rt.Bottom, sw, sh);
                            var rte = Util.FromRect(sx + rt.Left, sy + rt.Bottom, sw, vh);

                            if (rte.Bottom > Design.Height) rte = Util.FromRect(sx, sy + sh - vh, sw, vh);

                            ddwnd.ShowDropDown(rts, rte, Items, ItemHeight, SelectedIndex,
                                InputColor ?? thm.InputColor, SelectedColor ?? thm.PointColor, rte.Bottom > Design.Height,
                                (result) => SelectedIndex = result);

                             
                        }
                    }
                });
            });
            base.OnMouseDown(x, y);
        }
        #endregion

        #region Method
        public void Areas2(SKRect rtContent, Action<SKRect, SKRect> act)
        {
            var ButtonWidth = rtContent.Height;
            var rtIco = Util.FromRect(rtContent.Right - ButtonWidth, rtContent.Top, ButtonWidth, rtContent.Height);
            var rtText = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width - rtIco.Width, rtContent.Height);
            
            act(rtIco, rtText);
        }
        #endregion
    }
}

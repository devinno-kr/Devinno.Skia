using Devinno.Skia.Design;
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
    public abstract class DvValueLabel : DvControl
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
        public SKColor? ValueColor { get; set; } = null;
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
        public event EventHandler ValueClick;
        public event EventHandler ValueDoubleClick;
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
                    var ValueColor = this.ValueColor ?? thm.LabelColor;
                    var ButtonColor = this.ButtonColor ?? thm.ButtonColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var BorderColor = this.BorderColor ?? thm.GetBorderColor(ValueColor, BackColor);
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
                        thm.DrawLabel(Canvas, 
                            rtValue, 
                            ValueColor, BorderColor, ForeColor, BackColor, 
                            null, 
                            rndValue, BackgroundDraw, DvContentAlignment.MiddleCenter, false);
                        
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
                    if (BackgroundDraw) thm.DrawBox(Canvas, rtContent, ValueColor, BorderColor, Round, BoxStyle.Border, thm.Corner);
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
                }

                if (Buttons.Where(x => x.ButtonDownState).Count() > 0)
                {
                    Design?.Input(this);
                }

                if (CollisionTool.Check(rtValue, x, y)) bValueDown = true;
            
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

                if(bValueDown)
                {
                    bValueDown = false;
                    if (CollisionTool.Check(rtValue, x, y)) ValueClick?.Invoke(this, null);
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
                if (CollisionTool.Check(rtValue, x, y)) ValueDoubleClick?.Invoke(this, null);
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

    public class DvValueLabelText : DvValueLabel
    {
        #region Properties
        public string Value { get; set; } = "";

        public DvContentAlignment ContentAlignment { get; set; } = DvContentAlignment.MiddleCenter;
        private string Text => Value;
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        #endregion

        #region DrawValue
        protected override void DrawValue(SKCanvas canvas, DvTheme thm, SKRect rtValue)
        {
            Areas((rtContent, rtTitle, rtValue, rtButtons) =>
            {
                if (!string.IsNullOrWhiteSpace(Text))
                {
                    var cF = ForeColor ?? thm.ForeColor;
                    Util.DrawText(canvas, Text, FontName, FontSize, FontStyle, cF, rtValue, ContentAlignment);
                }
            });
        }
        #endregion
    }

    public class DvValueLabelNumber<T> : DvValueLabel where T : struct
    {
        #region Properties
        public T Value { get; set; } = default(T);

        public string FormatString { get; set; } = null;
        public DvContentAlignment ContentAlignment { get; set; } = DvContentAlignment.MiddleCenter;
        private string Text => ValueTool.Text<T>(Value, FormatString);
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        #endregion

        #region Constructor
        public DvValueLabelNumber()
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
            Areas((rtContent, rtTitle, rtValue, rtButtons) =>
            {
                if (!string.IsNullOrWhiteSpace(Text))
                {
                    var cF = ForeColor ?? thm.ForeColor;
                    Util.DrawText(canvas, Text, FontName, FontSize, FontStyle, cF, rtValue, ContentAlignment);
                }
            });
        }
        #endregion
    }

    public class DvValueLabelBool : DvValueLabel
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

        #region DrawValue
        protected override void DrawValue(SKCanvas canvas, DvTheme thm, SKRect rtValue)
        {
            Areas((rtContent, rtTitle, rtValue, rtButtons) =>
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
            });
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
}

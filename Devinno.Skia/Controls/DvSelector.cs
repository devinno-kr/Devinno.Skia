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
    public class DvSelector : DvControl
    {
        #region Properties
        public SKColor? SelectorColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;

        public List<TextIconItem> Items { get; private set; } = new List<TextIconItem>();
        public bool BackgroundDraw { get; set; } = true;
        public bool Animation => Design != null ? Design.Animation : false;
        public int? ButtonWidth { get; set; } = null;
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
        #region Text
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #region Icon
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        #endregion
        #endregion

        #region Event
        public event EventHandler SelectedIndexChanged;
        #endregion

        #region Member Variable
        private bool bLeft = false;
        private bool bRight = false;

        private Animation ani = new Animation();
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtLeft, rtRight, rtText, rtTextP, rtTextN) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        #region Color
                        var SelectorColor = this.SelectorColor ?? thm.ButtonColor;
                        var ForeColor = this.ForeColor ?? thm.ForeColor;
                        var Corner = thm.Corner;
                        var BorderColor = thm.GetBorderColor(SelectorColor, ParentContainer.GetBackColor());
                        #endregion

                        #region Background
                        if (BackgroundDraw) thm.DrawBox(Canvas, rtContent, SelectorColor, BorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.OutShadow);
                        #endregion
                        #region Button
                        var rtL = rtLeft;
                        var rtR = rtRight;
                        if (bLeft) rtL.Offset(0, 1);
                        if (bRight) rtR.Offset(0, 1);

                        Util.DrawIconFA(Canvas, "fa-chevron-left", IconSize, ForeColor, rtL, DvContentAlignment.MiddleCenter, true);
                        Util.DrawIconFA(Canvas, "fa-chevron-right", IconSize, ForeColor, rtR, DvContentAlignment.MiddleCenter, true);

                        var c1 = BorderColor;
                        var c2 = SelectorColor.BrightnessTransmit(0.2F);
                        var gny = Convert.ToInt32(rtContent.Height / 4);
                        var cy = Convert.ToInt32(rtContent.MidY);

                        if (BackgroundDraw)
                        {
                            p.StrokeWidth = 1;
                            p.Color = c1;
                            p.IsStroke = true;
                            Canvas.DrawLine(rtL.Right, cy - gny, rtL.Right, cy + gny, p);
                            Canvas.DrawLine(rtR.Left, cy - gny, rtR.Left, cy + gny, p);

                            p.Color = c2;
                            Canvas.DrawLine(rtL.Right + 0.5F, cy - gny, rtL.Right + 0.5F, cy + gny, p);
                            Canvas.DrawLine(rtR.Left + 0.5F, cy - gny, rtR.Left + 0.5F, cy + gny, p);
                        }

                        #endregion
                        #region Text
                        var sp = Canvas.Save();
                        Canvas.ClipRect(rtText);
                        if(Items.Count > 0)
                        {
                            if (Animation && ani.IsPlaying)
                            {
                                var SelectedIndex = ani.Variable == "Left" ? this.SelectedIndex + 1 : this.SelectedIndex - 1;
                                if (SelectedIndex == -1) SelectedIndex = Items.Count - 1;
                                if (SelectedIndex >= Items.Count) SelectedIndex = 0;

                                var itm = SelectedIndex == -1 ? null : Items[SelectedIndex];
                                var itmN = Items[SelectedIndex + 1 >= Items.Count ? 0 : SelectedIndex + 1];
                                var itmP = Items[SelectedIndex - 1 <= -1 ? Items.Count - 1 : SelectedIndex - 1];

                                var w = rtText.Width;

                                var rt = rtText;
                                var rtP = rtTextP;
                                var rtN = rtTextN;

                                var c = ForeColor;
                                var cP = ForeColor;
                                var cN = ForeColor;

                                var eR = AnimationAccel.DCL;
                                var eA = AnimationAccel.Linear;
                                if (ani.Variable == "Left")
                                {
                                    rt.Offset(ani.Value(eR, 0, w), 0);
                                    rtP.Offset(ani.Value(eR, 0, w), 0);
                                    rtN.Offset(ani.Value(eR, 0, w), 0);

                                    cP = cP.WithAlpha(Convert.ToByte(ani.Value(eA, 0, 150)));
                                    c = c.WithAlpha(Convert.ToByte(ani.Value(eA, 255, 0)));
                                    cN = cN.WithAlpha(Convert.ToByte(ani.Value(eA, 0, 0)));
                                }
                                else if (ani.Variable == "Right")
                                {
                                    rt.Offset(ani.Value(eR, 0, -w), 0);
                                    rtP.Offset(ani.Value(eR, 0, -w), 0);
                                    rtN.Offset(ani.Value(eR, 0, -w), 0);

                                    cP = cP.WithAlpha(Convert.ToByte(ani.Value(eA, 0, 0)));
                                    c = c.WithAlpha(Convert.ToByte(ani.Value(eA, 150, 0)));
                                    cN = cN.WithAlpha(Convert.ToByte(ani.Value(eA, 0, 255)));
                                }

                                if (itm != null)
                                {
                                    Util.DrawTextIcon(Canvas, itm.Text, FontName, FontSize, DvFontStyle.Normal, 8,
                                                     itm.IconString, IconSize, c, rt,
                                                     DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleCenter, true);
                                }

                                Util.DrawTextIcon(Canvas, itmP.Text, FontName, FontSize, DvFontStyle.Normal, 8,
                                                 itmP.IconString, IconSize, cP, rtP,
                                                 DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleCenter, true);

                                Util.DrawTextIcon(Canvas, itmN.Text, FontName, FontSize, DvFontStyle.Normal, 8,
                                                 itmN.IconString, IconSize, cN, rtN,
                                                 DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleCenter, true);
                            }
                            else
                            {
                                var itm = SelectedIndex == -1 ? null : Items[SelectedIndex];

                                if (itm != null)
                                {
                                    Util.DrawTextIcon(Canvas, itm.Text, FontName, FontSize, DvFontStyle.Normal, 8,
                                                      itm.IconString, IconSize, ForeColor, rtText,
                                                      DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleCenter, true);
                                }
                            }
                        }
                        Canvas.RestoreToCount(sp);
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
            bounds((rtContent, rtLeft, rtRight, rtText, rtTextP, rtTextN) =>
            {
                if (CollisionTool.Check(rtLeft, x, y)) bLeft = true;
                if (CollisionTool.Check(rtRight, x, y)) bRight = true;
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            bounds((rtContent, rtLeft, rtRight, rtText, rtTextP, rtTextN) =>
            {
                #region LeftButton
                if (bLeft)
                {
                    if (CollisionTool.Check(rtLeft, x, y))
                    {
                        if (Items.Count > 0)
                        {
                            SelectedIndex--;
                            if (SelectedIndex < 0) SelectedIndex = Items.Count - 1;
                        }
                        else SelectedIndex = -1;

                        if (Animation)
                        {
                            ani.Stop();
                            ani.Start(DvDesign.ANI2, "Left");
                        }
                    }
                    bLeft = false;
                }
                #endregion
                #region RightButton
                if (bRight)
                {
                    if (CollisionTool.Check(rtRight, x, y))
                    {
                        if (Items.Count > 0)
                        {
                            SelectedIndex++;
                            if (SelectedIndex >= Items.Count) SelectedIndex = 0;
                        }
                        else SelectedIndex = -1;

                        if (Animation)
                        {
                            ani.Stop();
                            ani.Start(DvDesign.ANI2, "Right");
                        }
                    }    
                    bRight = false;
                }
                #endregion
            });
           
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);

            var bwh = ButtonWidth ?? rtContent.Height;
            var w = Convert.ToInt32(bwh * 1.2);
            var h = Convert.ToInt32(bwh);

            var rtLeft = Util.MakeRectangleAlign(rtContent, new SKSize(w, h), DvContentAlignment.MiddleLeft);
            var rtRight = Util.MakeRectangleAlign(rtContent, new SKSize(w, h), DvContentAlignment.MiddleRight);
            var rtText = new SKRect(rtLeft.Right, rtContent.Top, rtRight.Left, rtContent.Bottom);
            var rtTextP = Util.FromRect(rtText.Left - rtText.Width, rtText.Top, rtText.Width, rtText.Height);
            var rtTextN = Util.FromRect(rtText.Right, rtText.Top, rtText.Width, rtText.Height);

            act(rtContent, rtLeft, rtRight, rtText, rtTextP, rtTextN);
        }
        #endregion
        #endregion
    }

    #region class : TextIconItem
    public class TextIconItem
    {
        public string IconString { get; set; }
        public virtual string Text { get; set; }
        public object Value { get; set; }
        public object Tag { get; set; }

        public TextIconItem() { }
        public TextIconItem(string Text) => this.Text = Text;
        public TextIconItem(string Text, string IconString) : this(Text) => this.IconString = IconString;
        public TextIconItem(string Text, string IconString, object Value) : this(Text, IconString) => this.Value = Value;
    }
    #endregion
}

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
    public class DvListBox : DvControl
    {
        #region Properties
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? SelectedColor { get; set; } = null;
        public SKColor? RowColor { get; set; } = null;

        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;

        public Padding ItemPadding { get; set; } = new Padding(0);
        public DvContentAlignment ItemAlignment { get; set; } = DvContentAlignment.MiddleCenter;
        public int ItemHeight { get; set; } = 30;
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;

        public List<TextIconItem> Items { get; } = new List<TextIconItem>();
        public List<TextIconItem> SelectedItems { get; } = new List<TextIconItem>();

        public ItemSelectionMode SelectionMode { get; set; } = ItemSelectionMode.SINGLE;

        public bool BackgroundDraw { get; set; } = true;

        private bool _IsScrolling => scroll.IsTouchMoving || (scroll.IsTouchScrolling && scroll.TouchOffset != 0);
        public bool IsScrolling { get; private set; }
        public bool TouchMode { get => scroll.TouchMode; set => scroll.TouchMode = value; }
        
        internal int? Corner { get; set; } = null;
        internal bool DrawScroll { get; set; } = true;
        internal bool Scrollable => scroll.ScrollView < scroll.ScrollTotal;
        internal double ScrollPosition
        {
            get => scroll.ScrollPosition;
            set => scroll.ScrollPosition = value;
        }
        
        #endregion

        #region Member Variable
        private Scroll scroll = new Scroll() { Direction = ScrollDirection.Vertical, TouchMode = true };
        #endregion

        #region Event 
        public event EventHandler SelectedChanged;
        public event EventHandler<ListBoxItemClickedEventArgs> ItemClicked;
        public event EventHandler<ListBoxItemClickedEventArgs> ItemDoubleClicked;
        #endregion

        #region Constructor
        public DvListBox()
        {
            scroll.GetScrollTotal = () => Items.Count * ItemHeight;
            scroll.GetScrollTick = () => ItemHeight;
            scroll.GetScrollView = () => this.Height - 2;
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtBox, rtScroll) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region Color
                    var BoxColor = this.BoxColor ?? thm.ListBackColor;
                    var SelectedItemColor = this.SelectedColor ?? thm.PointColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var RowColor = this.RowColor ?? thm.RowColor;
                    var BorderColor = thm.GetBorderColor(BoxColor, ParentContainer.GetBackColor());
                    var SelectedBorderColor = thm.GetBorderColor(SelectedItemColor, ParentContainer.GetBackColor());
                    var ScrollBorderColor = thm.GetBorderColor(thm.ScrollBarColor, ParentContainer.GetBackColor());
                    var Corner = this.Corner ?? thm.Corner;
                    #endregion

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        if (BackgroundDraw) thm.DrawBox(Canvas, rtBox, BoxColor, BorderColor, RoundType.Round_L, BoxStyle.Fill | BoxStyle.OutShadow, Corner);

                        #region Items
                        {
                            var sp = Canvas.Save();
                            Canvas.ClipRoundRect(new SKRoundRect(rtContent, Corner));
                            var last = Items.LastOrDefault();
                            loop(rtBox, (i, rt, itm) =>
                            {
                                var rtRow = rt;
                                if (!BackgroundDraw)
                                {
                                    var sz = Util.MeasureTextIcon(itm.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, itm.IconString, IconSize, IconAlignment);
                                    rtRow = Util.MakeRectangle(rt, new SKSize(sz.Width + 20, rt.Height));
                                }

                                if (SelectedItems.Contains(itm))
                                    thm.DrawBox(Canvas, rtRow, SelectedItemColor, SelectedBorderColor, BackgroundDraw ? RoundType.Rect : RoundType.Round, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow, Corner);
                                else
                                {
                                    if (BackgroundDraw)
                                    {
                                        thm.DrawBox(Canvas, rtRow, RowColor, BorderColor, RoundType.Rect, BoxStyle.Fill, Corner);

                                        p.IsStroke = true;
                                        p.StrokeWidth = 1;

                                        {
                                            var x1 = (int)rt.Left + 1;
                                            var x2 = (int)rt.Right - 1;
                                            var y = (int)rt.Top + 1.5F;

                                            p.Color = thm.GetInBevelColor(RowColor);
                                            Canvas.DrawLine(x1, y, x2, y, p);
                                        }

                                        {
                                            var x1 = (int)rt.Left + 0;
                                            var x2 = (int)rt.Right - 0;
                                            var y = (int)rt.Top + 0.5F;

                                            p.Color = BorderColor;
                                            Canvas.DrawLine(x1, y, x2, y, p);
                                        }

                                        if (itm == last && IsScrolling)
                                        {
                                            var x1 = (int)rt.Left + 0;
                                            var x2 = (int)rt.Right - 0;
                                            var y = (int)rt.Bottom + 0.5F;

                                            p.Color = BorderColor;
                                            Canvas.DrawLine(x1, y, x2, y, p);
                                        }
                                    }
                                }
                                var rtTxt = new SKRect(rt.Left + ItemPadding.Left, rt.Top + ItemPadding.Top, rt.Right - ItemPadding.Right, rt.Bottom - ItemPadding.Bottom);
                                Util.DrawTextIcon(Canvas, itm.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, itm.IconString, IconSize, ForeColor, rtTxt, IconAlignment, ItemAlignment);
                            });
                            Canvas.RestoreToCount(sp);
                        }
                        #endregion
                        #region Scroll
                        if (DrawScroll)
                        {
                            var sp = Canvas.Save();
                            thm.DrawBox(Canvas, rtScroll, thm.ScrollBarColor, SKColors.Black, BackgroundDraw ? RoundType.Round_R : RoundType.Round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutShadow);

                            Canvas.ClipRect(Util.FromRect(rtScroll.Left, rtScroll.Top + 0, rtScroll.Width, rtScroll.Height - 0));

                            var cCur = thm.ScrollCursorOffColor;
                            if (scroll.IsScrolling || scroll.IsTouchMoving) cCur = thm.ScrollCursorOnColor;

                            var rtcur = scroll.GetScrollCursorRect(rtScroll);
                            if (rtcur.HasValue) thm.DrawBox(Canvas, rtcur.Value, cCur, ScrollBorderColor, RoundType.Round, BoxStyle.Fill);

                            Canvas.RestoreToCount(sp);
                        }
                        #endregion

                        if (BackgroundDraw) thm.DrawBox(Canvas, rtBox, BoxColor, BorderColor, RoundType.Round_L, BoxStyle.Border, Corner);
                    }
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            bounds((rtContent, rtBox, rtScroll) =>
            {
                scroll.MouseDown(x, y, rtScroll);
                if (scroll.TouchMode && CollisionTool.Check(rtBox, x, y)) scroll.TouchDown(x, y);

                IsScrolling = _IsScrolling;

            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(int x, int y)
        {
            bounds((rtContent, rtBox, rtScroll) =>
            {
                scroll.MouseMove(x, y, rtScroll);
                if (scroll.TouchMode) scroll.TouchMove(x, y);

                IsScrolling = _IsScrolling;
            });
            base.OnMouseMove(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            bounds((rtContent, rtBox, rtScroll) =>
            {
                IsScrolling = _IsScrolling;

                scroll.MouseUp(x, y);
                if (scroll.TouchMode) scroll.TouchUp(x, y);
                
                if (CollisionTool.Check(rtBox, x,y))
                {
                    loop(rtBox, (i, rt, v) =>
                    {
                        if (!BackgroundDraw)
                        {
                            var sz = Util.MeasureTextIcon(v.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, v.IconString, IconSize, IconAlignment);
                            rt = Util.MakeRectangle(rt, new SKSize(sz.Width + 20, rt.Height));
                        }

                        if (CollisionTool.Check(rt, x, y) && !IsScrolling)
                        {
                            #region Single Selection
                            if (SelectionMode == ItemSelectionMode.SINGLE)
                            {
                                SelectedItems.Clear();
                                SelectedItems.Add(v);
                                if (SelectedChanged != null) SelectedChanged.Invoke(this, new EventArgs());
                            }
                            #endregion
                            #region Multi Selection
                            else if (SelectionMode == ItemSelectionMode.MULTI)
                            {
                                if (SelectedItems.Contains(v))
                                {
                                    SelectedItems.Remove(v);
                                    if (SelectedChanged != null) SelectedChanged.Invoke(this, new EventArgs());
                                }
                                else
                                {
                                    SelectedItems.Add(v);
                                    if (SelectedChanged != null) SelectedChanged.Invoke(this, new EventArgs());
                                }
                            }
                            #endregion
                        }
                    });
                }
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #region OnMouseClick
        protected override void OnMouseClick(int x, int y)
        {
            bounds((rtContent, rtBox, rtScroll) =>
            {
                if (CollisionTool.Check(rtBox, x, y) && !IsScrolling)
                {
                    loop(rtBox, (i, rt, v) =>
                    {
                        if (!BackgroundDraw)
                        {
                            var sz = Util.MeasureTextIcon(v.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, v.IconString, IconSize, IconAlignment);
                            rt = Util.MakeRectangle(rt, new SKSize(sz.Width + 20, rt.Height));
                        }

                        if (CollisionTool.Check(rt, x, y))
                        {
                            ItemClicked?.Invoke(this, new ListBoxItemClickedEventArgs(v));
                        }
                    });
                }
            });

            base.OnMouseClick(x, y);
        }
        #endregion
        #region OnMouseDoubleClick
        protected override void OnMouseDoubleClick(int x, int y)
        {
            var issc = IsScrolling;

            bounds((rtContent, rtBox, rtScroll) =>
            {
                if (CollisionTool.Check(rtBox, x, y) && !IsScrolling)
                {
                    loop(rtBox, (i, rt, v) =>
                    {
                        if (!BackgroundDraw)
                        {
                            var sz = Util.MeasureTextIcon(v.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, v.IconString, IconSize, IconAlignment);
                            rt = Util.MakeRectangle(rt, new SKSize(sz.Width + 20, rt.Height));
                        }

                        if (CollisionTool.Check(rt, x, y))
                        {
                            ItemDoubleClicked?.Invoke(this, new ListBoxItemClickedEventArgs(v));
                        }
                    });
                }
            });

            base.OnMouseDoubleClick(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect> act)
        {
            var GAP = 0;
            var scwh = DrawScroll ? Convert.ToInt32(Scroll.SC_WH) : 0;
            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);
            var rtBox = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width - scwh - GAP, rtContent.Height);
            var rtScroll = Util.FromRect(rtBox.Right+ GAP, rtBox.Top, scwh, rtBox.Height);

            act(rtContent, rtBox, (rtScroll));
        }
        #endregion
        #region loop
        private void loop(SKRect rtBox, Action<int, SKRect, TextIconItem> act)
        {
            var sc = scroll.ScrollPosition;
            var spos = Convert.ToInt32(scroll.ScrollPositionWithOffset);

            var si = Convert.ToInt32(Math.Floor((double)(sc - scroll.TouchOffset) / (double)ItemHeight));
            var cnt = Convert.ToInt32(Math.Ceiling((double)(rtBox.Height - Math.Min(0, scroll.TouchOffset)) / (double)ItemHeight));
            var ei = si + cnt;

            for (int i = Math.Max(0, si); i < ei + 1 && i < Items.Count; i++)
            {
                var itm = Items[i];
                var rt = Util.FromRect(rtBox.Left, spos + rtBox.Top + (ItemHeight * i), rtBox.Width, ItemHeight);
                if (CollisionTool.Check(Util.FromRect(rt.Left + 1, rt.Top + 1, rt.Width - 2, rt.Height - 2), rtBox)) act(i, rt, itm);
            }
        }
        #endregion
        #endregion
    }

    #region enum : ItemSelectionMode 
    public enum ItemSelectionMode { NONE, SINGLE, MULTI }
    #endregion
    #region class : ListBoxItemClickedEventArgs
    public class ListBoxItemClickedEventArgs : EventArgs
    {
        public TextIconItem Item { get; private set; }

        public ListBoxItemClickedEventArgs(TextIconItem Item) { this.Item = Item; }
    }
    #endregion

}

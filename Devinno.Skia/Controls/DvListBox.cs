using Devinno.Collections;
using Devinno.Skia.Collections;
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
    public class DvListBox : DvControl
    {
        #region Properties
        #region Color
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? SelectedColor { get; set; } = null;
        #endregion
        #region Items
        public EventList2<ListBoxItem> Items { get; } = new EventList2<ListBoxItem>();
        public List<ListBoxItem> SelectedItems { get; } = new List<ListBoxItem>();
        #endregion
        #region Font
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;

        public float IconSize { get; set; } = 12;
        public float IconGap { get; set; } = 5;
        #endregion
        #region Item
        public int ItemHeight { get; set; } = 30;
        public DvContentAlignment ItemAlignment { get; set; } = DvContentAlignment.MiddleCenter;
        #endregion
        #region Selection
        public ItemSelectionMode SelectionMode { get; set; } = ItemSelectionMode.SINGLE;
        #endregion
        #region Shape
        public DvRoundType Round { get; set; } = DvRoundType.All;
        public bool BackgroundDraw { get; set; } = true;
        #endregion
        #region Scroll
        public bool TouchMode { get => scroll.TouchMode; set => scroll.TouchMode = value; }

        private bool IsScrolling => scroll.IsTouchMoving || scroll.IsTouchScrolling || scroll.IsScrolling;

        internal double ScrollPosition
        {
            get => scroll.ScrollPosition;
            set => scroll.ScrollPosition = value;
        }
        #endregion
        #region DrawScroll
        public bool DrawScroll { get; set; } = true;
        #endregion
        #endregion

        #region Member Variable
        private Scroll scroll = new Scroll() { Direction = ScrollDirection.Vertical, TouchMode = true };
        private bool isc = false;
        #endregion

        #region Event 
        public event EventHandler SelectedChanged;
        public event EventHandler<ListBoxItemClickedEventArgs> ItemClicked;
        public event EventHandler<ListBoxItemClickedEventArgs> ItemDoubleClicked;
        public event EventHandler<ListBoxItemClickedEventArgs> ItemLongClicked;
        #endregion

        #region Constructor
        public DvListBox()
        {
            scroll.GetScrollTotal = () => ItemHeight * Items.Count;
            scroll.GetScrollTick = () => ItemHeight;
            scroll.GetScrollView = () => Height;

            Items.Changed += (o, s) => calcPos();
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtBox, rtScroll) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region Color
                    var BoxColor = this.BoxColor ?? thm.ListBackColor;
                    var SelectedColor = this.SelectedColor ?? thm.PointColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    #endregion

                    thm.DrawListBox(Canvas,
                        rtContent, rtBox, rtScroll,
                        BoxColor, SelectedColor, ForeColor, BackColor,
                        BackgroundDraw, ItemAlignment, Round,
                        scroll, DrawScroll,
                        loop);
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion

        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtBox, rtScroll) =>
            {
                Design?.Input(this);

                scroll.MouseDown(x, y, rtScroll);
                if (scroll.TouchMode && CollisionTool.Check(rtBox, x, y)) scroll.TouchDown(x, y);

                if (CollisionTool.Check(rtBox, x, y)) 
                    loop(rtBox, (rt, v) => v._MouseDown(rt, x, y - Convert.ToSingle(scroll.ScrollPositionWithOffset)));
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(float x, float y)
        {
            Areas((rtContent, rtBox, rtScroll) =>
            {
                scroll.MouseMove(x, y, rtScroll);
                if (scroll.TouchMode) scroll.TouchMove(x, y);

                if (CollisionTool.Check(rtBox, x, y)) 
                    loop(rtBox, (rt, v) => v._MouseMove(rt, x, y - Convert.ToSingle(scroll.ScrollPositionWithOffset)));
            });
            base.OnMouseMove(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtBox, rtScroll) =>
            {
                isc = IsScrolling && scroll.TouchOffset != 0;

                scroll.MouseUp(x, y);
                if (scroll.TouchMode) scroll.TouchUp(x, y);

                if (CollisionTool.Check(rtBox, x, y)) 
                    loop(rtBox, (rt, v) => v._MouseUp(rt, x, y - Convert.ToSingle(scroll.ScrollPositionWithOffset)));
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #region OnMouseClick
        protected override void OnMouseClick(float x, float y)
        {
            Areas((rtContent, rtBox, rtScroll) =>
            {
                if (CollisionTool.Check(rtBox, x, y) && !isc)
                    loop(rtBox, (rt, v) =>
                    {
                        if (CollisionTool.Check(rt, x, y - Convert.ToSingle(scroll.ScrollPositionWithOffset)))
                        {
                            select(v);
                            ItemClicked?.Invoke(this, new ListBoxItemClickedEventArgs(v));
                        }
                    });
            });

            base.OnMouseClick(x, y);
        }
        #endregion
        #region OnMouseLongClick
        protected override void OnMouseLongClick(float x, float y)
        {
            Areas((rtContent, rtBox, rtScroll) =>
            {
                if (CollisionTool.Check(rtBox, x, y) && !isc)
                    loop(rtBox, (rt, v) =>
                    {
                        if (CollisionTool.Check(rt, x, y - Convert.ToSingle(scroll.ScrollPositionWithOffset)))
                            ItemLongClicked?.Invoke(this, new ListBoxItemClickedEventArgs(v));
                    });
            });
            base.OnMouseLongClick(x, y);
        }
        #endregion
        #region OnMouseDoubleClick
        protected override void OnMouseDoubleClick(float x, float y)
        {
            Areas((rtContent, rtBox, rtScroll) =>
            {
               if (CollisionTool.Check(rtBox, x, y))
                    loop(rtBox, (rt, v) =>
                    {
                        if (CollisionTool.Check(rt, x, y - Convert.ToSingle(scroll.ScrollPositionWithOffset)))
                            ItemDoubleClicked?.Invoke(this, new ListBoxItemClickedEventArgs(v));
                    });
            });

            base.OnMouseDoubleClick(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect> act)
        {
            var scwh = Scroll.SC_WH;
            var rtContent = Util.FromRect(0, 0, Width, Height);
            var rtBox = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width - scwh, rtContent.Height);
            var rtScroll = Util.FromRect(rtBox.Right, rtBox.Top, scwh, rtBox.Height);

            act(rtContent, rtBox, rtScroll);
        }
        #endregion

        #region search
        private int search(int si, int ei, float pos)
        {
            var spos = Convert.ToSingle(scroll.ScrollPositionWithOffset);
            int idx = MathTool.Center(si, ei);
            if (si != ei && idx != si && idx != ei)
            {
                if (idx >= 0 && idx < Items.Count)
                {
                    if (pos > Items[idx].Bottom + spos)
                    {
                        return search(idx, ei, pos);
                    }
                    else if (pos < Items[idx].Top + spos)
                    {
                        return search(si, idx, pos);
                    }
                    else return idx;
                }
                return idx;
            }
            else return idx;
        }
        #endregion
        #region calcPos
        private void calcPos()
        {
            int i = 0;
            foreach (var v in Items)
            {
                v.ListBox = this;
                v.Top = i * ItemHeight;
                v.Bottom = v.Top + ItemHeight;
                i++;
            }
        }
        #endregion
        #region loop
        private void loop(SKRect rtBox, Action<SKRect, ListBoxItem> act)
        {
            var spos = Convert.ToSingle(scroll.ScrollPositionWithOffset);

            var si = search(0, Items.Count - 1, rtBox.Top);
            var vc = Convert.ToInt32(Math.Ceiling(rtBox.Height / ItemHeight)) + 1;

            var ls = Items.GetRange(si, Math.Min(Items.Count - si, vc));
            foreach (var v in ls) act(new SKRect(rtBox.Left, v.Top, rtBox.Right, v.Bottom), v);
        }
        #endregion
        #region select
        private void select(ListBoxItem item)
        {
            #region Single Selection
            if (SelectionMode == ItemSelectionMode.SINGLE)
            {
                SelectedItems.Clear();
                SelectedItems.Add(item);
                if (SelectedChanged != null) SelectedChanged.Invoke(this, new EventArgs());
            }
            #endregion
            #region Multi Selection
            else if (SelectionMode == ItemSelectionMode.MULTI)
            {
                if (SelectedItems.Contains(item))
                {
                    SelectedItems.Remove(item);
                    if (SelectedChanged != null) SelectedChanged.Invoke(this, new EventArgs());
                }
                else
                {
                    SelectedItems.Add(item);
                    if (SelectedChanged != null) SelectedChanged.Invoke(this, new EventArgs());
                }
            }
            #endregion
        }
        #endregion
        #endregion
    }

    #region class : ListBoxItem
    public class ListBoxItem
    {
        #region Properties
        public DvListBox ListBox { get; internal set; }

        public float Top { get; set; }
        public float Bottom { get; set; }
        public string Text { get; set; }
        public string IconString { get; set; }
        public object Tag { get; set; }
        #endregion

        #region Method
        protected virtual void Draw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (ListBox != null)
            {
                #region Category
                #region var
                var lb = ListBox;
                var foreColor = lb.ForeColor ?? thm.ForeColor;
                var selectedColor = lb.SelectedColor ?? thm.PointColor;
                #endregion
                using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                {
                    #region Selected
                    if (lb.SelectedItems.Contains(this))
                    {
                        var rtv = Util.FromRect(bounds); rtv.Inflate(0, 0.5F);
                        p.Color = selectedColor;
                        p.IsStroke = false;
                        canvas.DrawRect(rtv, p);
                    }
                    #endregion
                    #region Text
                    Util.DrawTextIcon(canvas, IconString, lb.IconSize, DvTextIconAlignment.LeftRight, lb.IconGap,
                                               Text, lb.FontName, lb.FontSize, lb.FontStyle,
                                               Padding.Zero, foreColor, foreColor, bounds, lb.ItemAlignment);
                    #endregion
                }
                #endregion
            }
        }
        protected virtual void MouseDown(SKRect bounds, float x, float y) { }
        protected virtual void MouseMove(SKRect bounds, float x, float y) { }
        protected virtual void MouseUp(SKRect bounds, float x, float y) { }


        internal void _Draw(SKCanvas canvas, DvTheme thm, SKRect bounds) => Draw(canvas, thm, bounds);
        internal void _MouseDown(SKRect bounds, float x, float y) => MouseDown(bounds, x, y);
        internal void _MouseMove(SKRect bounds, float x, float y) => MouseMove(bounds, x, y);
        internal void _MouseUp(SKRect bounds, float x, float y) => MouseUp(bounds, x, y);
        #endregion
    }
    #endregion

}

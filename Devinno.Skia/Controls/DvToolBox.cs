using Devinno.Collections;
using Devinno.Skia.Collections;
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
    public class DvToolBox : DvControl
    {
        #region Properties
        #region Color
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? CategoryColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        #endregion
        #region Items
        public EventList2<ToolCategoryItem> Categories { get; } = new EventList2<ToolCategoryItem>();
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
        #endregion
        #region Shape
        public DvRoundType Round { get; set; } = DvRoundType.All;
        public bool BackgroundDraw { get; set; } = true;
        #endregion
        #region Scroll
        public bool TouchMode { get => scroll.TouchMode; set => scroll.TouchMode = value; }

        private bool IsScrolling => scroll.IsTouchMoving || scroll.IsTouchScrolling || scroll.IsScrolling;
        #endregion
        #region Animation
        public bool Animation => Design != null ? Design.Animation : false;
        #endregion
        #endregion

        #region Member Variable
        private Scroll scroll = new Scroll() { Direction = ScrollDirection.Vertical, TouchMode = true };
        internal float mx, my;
        #endregion

        #region Event
        public event EventHandler<ToolItemDragEventArgs> ItemDragStart;
        public event EventHandler<ToolItemClickedEventArgs> ItemClicked;
        public event EventHandler<ToolItemClickedEventArgs> ItemDoubleClicked;
        public event EventHandler<ToolItemClickedEventArgs> ItemLongClicked;
        #endregion

        #region Constructor
        public DvToolBox()
        {
            scroll.GetScrollTotal = () => Categories.Sum(x => ItemHeight + x.ChildHeight);
            scroll.GetScrollTick = () => ItemHeight;
            scroll.GetScrollView = () => Height;

            Categories.Appended += (o, s) => { foreach (var v in s.Items) v.ToolBox = this; };
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
                    var CategoryColor = this.CategoryColor ?? thm.ButtonColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    #endregion

                    thm.DrawToolBox(Canvas,
                        rtContent, rtBox, rtScroll,
                        BoxColor, CategoryColor, ForeColor, BackColor,
                        BackgroundDraw, IconSize, ItemHeight, Round,
                        FontName, FontSize, FontStyle,
                        scroll,
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
                if (scroll.ScrollTotal > scroll.ScrollView) Design?.Input(this);

                scroll.MouseDown(x, y, rtScroll);
                if (scroll.TouchMode && CollisionTool.Check(rtBox, x, y)) scroll.TouchDown(x, y);

                var spos = Convert.ToSingle(scroll.ScrollPositionWithOffset);
                loop(rtBox, (rtv, v) => v._MouseDown(rtv, x, y - spos));

            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(float x, float y)
        {
            Areas((rtContent, rtBox, rtScroll) =>
            {
                mx = x; my = y - Convert.ToSingle(scroll.ScrollPositionWithOffset);
               
                scroll.MouseMove(x, y, rtScroll);
                if (scroll.TouchMode) scroll.TouchMove(x, y);

                var spos = Convert.ToSingle(scroll.ScrollPositionWithOffset);
                loop(rtBox, (rtv, v) => v._MouseMove(rtv, x, y - spos));
            });
            base.OnMouseMove(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtBox, rtScroll) =>
            {
                scroll.MouseUp(x, y);
                if (scroll.TouchMode) scroll.TouchUp(x, y);

                var spos = Convert.ToSingle(scroll.ScrollPositionWithOffset);
                loop(rtBox, (rtv, v) => v._MouseUp(rtv, x, y - spos));
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #region OnMouseClick 
        protected override void OnMouseClick(float x, float y)
        {
            Areas((rtContent, rtBox, rtScroll) =>
            {
                var spos = Convert.ToSingle(scroll.ScrollPositionWithOffset);
                loop(rtBox, (rtv, v) => v._MouseClick(rtv, x, y - spos));
            });

            base.OnMouseClick(x, y);
        }
        #endregion
        #region OnMouseLongClick
        protected override void OnMouseLongClick(float x, float y)
        {
            Areas((rtContent, rtBox, rtScroll) =>
            {
                var spos = Convert.ToSingle(scroll.ScrollPositionWithOffset);
                loop(rtBox, (rtv, v) => v._MouseLongClick(rtv, x, y - spos));
            });
            base.OnMouseLongClick(x, y);
        }
        #endregion
        #region OnMouseDoubleClick
        protected override void OnMouseDoubleClick(float x, float y)
        {
            Areas((rtContent, rtBox, rtScroll) =>
            {
                var spos = Convert.ToSingle(scroll.ScrollPositionWithOffset);
                loop(rtBox, (rtv, v) => v._MouseDoubleClick(rtv, x, y - spos));
            });
            base.OnMouseDoubleClick(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect> act)
        {
            var scwh = Convert.ToInt32(Scroll.SC_WH);
            var rtContent = Util.FromRect(0, 0, Width, Height);
            var rtBox = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width - scwh, rtContent.Height);
            var rtScroll = Util.FromRect(rtBox.Right, rtBox.Top, scwh, rtBox.Height);

            act(rtContent, rtBox, rtScroll);
        }
        #endregion

        #region loop
        private void loop(SKRect rtBox, Action<SKRect, ToolCategoryItem> act)
        {
            var spos = Convert.ToSingle(scroll.ScrollPositionWithOffset);
            var y = 0F;
            foreach (var v in Categories)
            {
                v.Top = y;
                v.Bottom = y + ItemHeight + v.ChildHeight;

                if (CollisionTool.CheckVertical(rtBox.Top, rtBox.Bottom, v.Top + spos, v.Bottom))
                    act(new SKRect(rtBox.Left, v.Top, rtBox.Right, v.Bottom), v);
                else if (y > rtBox.Bottom)
                    break;

                y += ItemHeight + v.ChildHeight;
            }
        }
        #endregion
        #region invoke
        internal void InvokeItemClick(ToolItem item, float x, float y) => ItemClicked?.Invoke(this, new ToolItemClickedEventArgs(x, y, item));
        internal void InvokeItemLongClick(ToolItem item, float x, float y) => ItemLongClicked?.Invoke(this, new ToolItemClickedEventArgs(x, y, item));
        internal void InvokeItemDoubleClick(ToolItem item, float x, float y) => ItemDoubleClicked?.Invoke(this, new ToolItemClickedEventArgs(x, y, item));
        internal void InvokeDragStart(ToolItem item, float x, float y)
        {
            var arg = new ToolItemDragEventArgs(x, y, item);
            ItemDragStart?.Invoke(this, arg);

            if (arg.Drag) Design.Drag(item);

            scroll.Clear();
        }
        #endregion
        #endregion
    }

    #region class : ToolItem
    public class ToolItem  
    {
        #region Properties
        internal DvToolBox ToolBox => Category?.ToolBox;
        public ToolCategoryItem Category { get; internal set; }

        public float Top { get; set; }
        public float Bottom { get; set; }
        public string Text { get; set; }
        public string IconString { get; set; }
        public object Tag { get; set; }
        #endregion

        #region Method
        protected virtual void Draw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (ToolBox != null)
            {
                var tb = ToolBox;
                var boxColor = tb.BoxColor ?? thm.ListBackColor;
                var foreColor = tb.ForeColor ?? thm.ForeColor;

                item(bounds, (rtv, rti, rtt, v) =>
                {
                    thm.DrawToolBoxItem(canvas,
                        rtv, rti, rtt,
                        boxColor, foreColor, v,
                        tb.mx, tb.my, (tb.Design?.IsDrag ?? false));

                });
            }
        }
        protected virtual void MouseDown(SKRect bounds, float x, float y)
        {
            if (ToolBox != null && CollisionTool.Check(bounds, x, y))
            {
                item(bounds, (rtv, rti, rtt, v) =>
                {
                    if (CollisionTool.Check(rtv, x, y) && !(ToolBox.Design?.IsDrag ?? false))
                        ToolBox.InvokeDragStart(this, x, y);
                });
            }
        }
        protected virtual void MouseUp(SKRect bounds, float x, float y) { }
        protected virtual void MouseMove(SKRect bounds, float x, float y) { }
        protected virtual void MouseClick(SKRect bounds, float x, float y) { if (ToolBox != null && CollisionTool.Check(bounds, x, y)) ToolBox.InvokeItemClick(this, x, y); }
        protected virtual void MouseLongClick(SKRect bounds, float x, float y) { if (ToolBox != null && CollisionTool.Check(bounds, x, y)) ToolBox.InvokeItemLongClick(this, x, y); }
        protected virtual void MouseDoubleClick(SKRect bounds, float x, float y) { if (ToolBox != null && CollisionTool.Check(bounds, x, y)) ToolBox.InvokeItemDoubleClick(this, x, y); }

        internal void _Draw(SKCanvas canvas, DvTheme thm, SKRect bounds) => Draw(canvas, thm, bounds);
        internal void _MouseDown(SKRect bounds, float x, float y) => MouseDown(bounds, x, y);
        internal void _MouseUp(SKRect bounds, float x, float y) => MouseUp(bounds, x, y);
        internal void _MouseMove(SKRect bounds, float x, float y) => MouseMove(bounds, x, y);
        internal void _MouseClick(SKRect bounds, float x, float y) => MouseClick(bounds, x, y);
        internal void _MouseLongClick(SKRect bounds, float x, float y) => MouseLongClick(bounds, x, y);
        internal void _MouseDoubleClick(SKRect bounds, float x, float y) => MouseDoubleClick(bounds, x, y);

        #region item
        private void item(SKRect rtRow, Action<SKRect, SKRect, SKRect, DvTextIcon> act)
        {
            if (ToolBox != null)
            {
                var tb = ToolBox;
                var itm = new DvTextIcon
                {
                    Text = Text,
                    FontSize = tb.FontSize,
                    FontName = tb.FontName,
                    FontStyle = tb.FontStyle,
                    IconGap = tb.IconGap,
                    IconString = IconString,
                    IconSize = tb.IconSize,
                    IconAlignment = DvTextIconAlignment.LeftRight,
                };

                var gap = 10;
                var rh = 6;
                var sz = Util.MeasureTextIcon(itm); sz.Width += rh + gap;
               
               
                var rtv = Util.MakeRectangleAlign(rtRow, sz, DvContentAlignment.MiddleCenter);
                var rti = Util.FromRect(rtv.Left, rtv.Top, rh, rtv.Height);
                var rtt = Util.FromRect(rti.Right + gap, rtv.Top, rtv.Width - rh - gap, rtv.Height);
                rtv.Inflate(10, ((tb.ItemHeight - rtv.Height) / 2F) - 2F);
                act(rtv, rti, rtt, itm);
            }
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : ToolCategoryItem
    public class ToolCategoryItem  
    {
        #region Properties 
        public DvToolBox ToolBox { get; internal set; }

        public float Top { get; set; }
        public float Bottom { get; set; }
        public string Text { get; set; }
        public string IconString { get; set; }
        public object Tag { get; set; }

        public EventList2<ToolItem> Items { get; } = new EventList2<ToolItem>();
        public bool Expands { get; set; } = true;
        #region ChildHeight
        internal float ChildHeight
        {
            get
            {
                var ret = 0F;

                if (ToolBox != null)
                {
                    var ih = ToolBox.ItemHeight * Items.Count;
                    if (ToolBox.Animation && ani.IsPlaying)
                    {
                        if (Expands) ret = ani.Value(AnimationAccel.DCL, 0, ih);
                        else ret = ani.Value(AnimationAccel.DCL, ih, 0);
                    }
                    else
                    {
                        if (Expands) ret = ih;
                    }
                }

                return ret;
            }
        }
        #endregion
        #endregion

        #region Member Variable
        Animation ani = new Animation();
        #endregion

        #region Constructor
        public ToolCategoryItem()
        {
            Items.Appended += (o, s) => { foreach (var v in s.Items) v.Category = this; };
        }
        #endregion

        #region Method
        #region internal
        protected virtual void Draw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (ToolBox != null)
            {
                #region Category
                {
                    #region var
                    var tb = ToolBox;
                    var foreColor = tb.ForeColor ?? thm.ForeColor;
                    var boxColor = tb.BoxColor ?? thm.ListBackColor;
                    var categoryColor = tb.CategoryColor ?? thm.ButtonColor;
                    var backColor = tb.ParentContainer.GetBackColor();

                    var rtRow = Util.FromRect(bounds.Left, bounds.Top, bounds.Width, tb.ItemHeight);
                    var rti = Util.FromRect(rtRow.Left, rtRow.Top, rtRow.Height, rtRow.Height);
                    var rtt = Util.FromRect(rti.Right, rtRow.Top, rtRow.Width - rtRow.Height, rtRow.Height);
                    #endregion
 
                    thm.DrawToolBoxCategory(canvas,
                        rtRow, rtt, rti,
                        boxColor, categoryColor, foreColor, backColor,
                        Text, tb.FontName, tb.FontSize, tb.FontStyle,
                        IconString, tb.IconSize, tb.IconGap, DvTextIconAlignment.LeftRight,
                        Expands, tb.ItemHeight,
                        tb.Animation, ani);

                }
                #endregion

                #region Items
                loop(bounds, (rtv, rtclip, v) =>
                {
                    var sp = canvas.Save();
                    canvas.ClipRect(rtclip);

                    v._Draw(canvas, thm, rtv);

                    canvas.RestoreToCount(sp);
                });
                #endregion
            }
        }

        protected virtual void MouseDown(SKRect bounds, float x, float y)
        {
            if (ToolBox != null && CollisionTool.Check(bounds, x, y))
            {
                loop(bounds, (rtv, rtclip, v) => v._MouseDown(rtv, x, y));
            }
        }

        protected virtual void MouseUp(SKRect bounds, float x, float y)
        {
            if (ToolBox != null && CollisionTool.Check(bounds, x, y))
            {
                loop(bounds, (rtv, rtclip, v) => v._MouseUp(rtv, x, y));
            }
        }

        protected virtual void MouseMove(SKRect bounds, float x, float y)
        {
            if (ToolBox != null && CollisionTool.Check(bounds, x, y))
            {
                loop(bounds, (rtv, rtclip, v) => v._MouseMove(rtv, x, y));
            }
        }

        protected virtual void MouseClick(SKRect bounds, float x, float y)
        {
            if (ToolBox != null && CollisionTool.Check(bounds, x, y))
            {
                #region Category
                #region var
                var tb = ToolBox;
                var ics = tb.IconSize;
                var ih = tb.ItemHeight;

                var rtBox = Util.FromRect(bounds.Left, bounds.Top, bounds.Width, ih);
                var rti = Util.FromRect(rtBox.Left, rtBox.Top, rtBox.Height, rtBox.Height);
                var rtt = Util.FromRect(rti.Right, rtBox.Top, rtBox.Width - rtBox.Height, rtBox.Height);
                #endregion
                var rt = Util.FromRect(rti); rt.Inflate(5, 5);
                if (CollisionTool.Check(rt, x, y))
                {
                    Expands = !Expands;
                    ani.Start(DvDesign.ANI);
                }
                #endregion

                loop(bounds, (rtv, rtclip, v) => v._MouseClick(rtv, x, y));
            }
        }

        protected virtual void MouseLongClick(SKRect bounds, float x, float y)
        {
            if (ToolBox != null && CollisionTool.Check(bounds, x, y))
            {
                loop(bounds, (rtv, rtclip, v) => v._MouseLongClick(rtv, x, y));
            }
        }

        protected virtual void MouseDoubleClick(SKRect bounds, float x, float y)
        {
            if (ToolBox != null && CollisionTool.Check(bounds, x, y))
            {
                loop(bounds, (rtv, rtclip, v) => v._MouseDoubleClick(rtv, x, y));
            }
        }

        internal void _Draw(SKCanvas canvas, DvTheme thm, SKRect bounds) => Draw(canvas, thm, bounds);
        internal void _MouseDown(SKRect bounds, float x, float y) => MouseDown(bounds, x, y);
        internal void _MouseUp(SKRect bounds, float x, float y) => MouseUp(bounds, x, y);
        internal void _MouseMove(SKRect bounds, float x, float y) => MouseMove(bounds, x, y);
        internal void _MouseClick(SKRect bounds, float x, float y) => MouseClick(bounds, x, y);
        internal void _MouseLongClick(SKRect bounds, float x, float y) => MouseLongClick(bounds, x, y);
        internal void _MouseDoubleClick(SKRect bounds, float x, float y) => MouseDoubleClick(bounds, x, y);
        #endregion

        #region loop
        private void loop(SKRect rtBox, Action<SKRect, SKRect, ToolItem> act)
        {
            if (ToolBox != null)
            {
                var ih = ToolBox.ItemHeight;
                var rtclip = Util.FromRect(rtBox.Left, Top + ih, rtBox.Width, ChildHeight);
                var y = Top + ih;
                foreach (var v in Items)
                {
                    v.Top = y;
                    v.Bottom = y + ih;

                    if (CollisionTool.CheckVertical(rtBox.Top, rtBox.Bottom, y, y + ih))
                        act(Util.FromRect(rtBox.Left, y, rtBox.Width, ih), rtclip, v);
                    else if (y > rtBox.Bottom) 
                        break;
                    
                    y += ih;
                }
            }
        }
        #endregion
        #endregion
    }
    #endregion
}

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
    public class DvTreeView : DvControl
    {
        #region Const
        internal const int IndentWidth = 20;
        #endregion

        #region Properties
        #region Color
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? RadioColor { get; set; } = null;
        public SKColor? SelectedColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        #endregion
        #region Items
        public TreeViewNodeCollection Nodes { get; private set; } = null;
        public List<TreeViewNode> SelectedNodes { get; } = new List<TreeViewNode>();
        public ItemSelectionMode SelectionMode { get; set; } = ItemSelectionMode.SINGLE;
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
        internal float SPWO => Convert.ToSingle(scroll.ScrollPositionWithOffset);
        private bool IsScrolling => scroll.IsTouchMoving || scroll.IsTouchScrolling || scroll.IsScrolling;
        #endregion
        #region Animation
        public bool Animation => Design != null ? Design.Animation : false;
        #endregion
        #endregion

        #region Member Variable
        private Scroll scroll = new Scroll() { Direction = ScrollDirection.Vertical, TouchMode = true };
        internal float mx, my;
        private bool isc = false;
        #endregion

        #region Event
        public event EventHandler SelectedChanged;
        #endregion

        #region Constructor
        public DvTreeView()
        {
            Nodes = new TreeViewNodeCollection(this);
            
            scroll.GetScrollTotal = () => Nodes.Sum(x => ItemHeight + x.ChildHeight);
            scroll.GetScrollTick = () => ItemHeight;
            scroll.GetScrollView = () => Height;
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
                    var RadioColor = this.RadioColor ?? thm.ForeColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    #endregion

                    thm.DrawTreeView(Canvas,
                        rtContent, rtBox, rtScroll,
                        BoxColor, SelectedColor, RadioColor, ForeColor, BackColor,
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
                Design?.Input(this);

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
                isc = IsScrolling && scroll.TouchOffset != 0;

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
                if (CollisionTool.Check(rtBox, x, y) && !isc)
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
        private void loop(SKRect rtBox, Action<SKRect, TreeViewNode> act)
        {
            var spos = Convert.ToSingle(scroll.ScrollPositionWithOffset);
            var y = 0F;
            foreach (var v in Nodes)
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
        #region select
        internal void select(TreeViewNode item)
        {
            #region Single Selection
            if (SelectionMode == ItemSelectionMode.SINGLE)
            {
                SelectedNodes.Clear();
                SelectedNodes.Add(item);
                if (SelectedChanged != null) SelectedChanged.Invoke(this, new EventArgs());
            }
            #endregion
            #region Multi Selection
            else if (SelectionMode == ItemSelectionMode.MULTI)
            {
                if (SelectedNodes.Contains(item))
                {
                    SelectedNodes.Remove(item);
                    if (SelectedChanged != null) SelectedChanged.Invoke(this, new EventArgs());
                }
                else
                {
                    SelectedNodes.Add(item);
                    if (SelectedChanged != null) SelectedChanged.Invoke(this, new EventArgs());
                }
            }
            #endregion
        }
        #endregion
        #endregion
    }

    #region class : TreeViewNode
    public class TreeViewNode
    {
        #region Properties
        public DvTreeView TreeView => tree ?? Parent.TreeView;
        public TreeViewNode Parent { get; internal set; } = null;

        public float Top { get; set; }
        public float Bottom { get; set; }
        public string Text { get; set; }
        public string IconString { get; set; }
        public object Tag { get; set; }

        public TreeViewNodeCollection Nodes { get; private set; }
        public bool Expands { get; set; } = true;
        public int Depth => (Parent != null ? Parent.Depth + 1 : 0);

        #region ChildHeight
        internal float ChildHeight
        {
            get
            {
                var ret = 0F;

                if (TreeView != null)
                {
                    var ih = TreeView.ItemHeight;
                    var vh = Nodes.Sum(x => x.ChildHeight + ih);

                    if (TreeView.Animation && ani.IsPlaying)
                    {
                        if (Expands) ret = ani.Value(AnimationAccel.DCL, 0, vh);
                        else ret = ani.Value(AnimationAccel.DCL, vh, 0);
                    }
                    else
                    {
                        if (Expands) ret = vh;
                    }
                }

                return ret;
            }
        }
        #endregion
        #endregion

        #region Member Variable
        Animation ani = new Animation();
        DvTreeView tree = null;
        #endregion

        #region Constructor
        public TreeViewNode()
        {
            Nodes = new TreeViewNodeCollection(this);
        }
        #endregion

        #region Method
        #region internal
        protected virtual void Draw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            var tv = TreeView;
            if (tv != null)
            {
                using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                {
                    #region var
                    var foreColor = tv.ForeColor ?? thm.ForeColor;
                    var backColor = tv.ParentContainer.GetBackColor();
                    var selectedColor = tv.SelectedColor ?? thm.PointColor;
                    var boxColor = tv.BoxColor ?? (tv.BackgroundDraw ? thm.ListBackColor : backColor);
                    var radioColor = tv.RadioColor ?? thm.ForeColor;
                    var radioBackColor = thm.InputColor;
                 
                    var ih = tv.ItemHeight;
                    var sel = tv.SelectedNodes.Contains(this);

                    var rtRow = Util.FromRect(bounds.Left, bounds.Top, bounds.Width, tv.ItemHeight);
                    #endregion
                  
                    #region Radio
                    item(rtRow,
                        (rti, rtt, v) =>
                        {
                            thm.DrawTreeViewNode(canvas,
                                   rtRow, rti, rtt,
                                   boxColor, selectedColor, radioColor, radioBackColor, foreColor, backColor,
                                   v, sel, this.Nodes, Expands,
                                   tv.Animation, ani);
                        });
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
        }

        protected virtual void MouseDown(SKRect bounds, float x, float y)
        {
            var tv = TreeView;
            if (tv != null && CollisionTool.Check(bounds, x, y))
            {
                loop(bounds, (rtv, rtclip, v) => v._MouseDown(rtv, x, y));
            }
        }

        protected virtual void MouseUp(SKRect bounds, float x, float y)
        {
            var tv = TreeView;
            if (tv != null && CollisionTool.Check(bounds, x, y))
            {
                loop(bounds, (rtv, rtclip, v) => v._MouseUp(rtv, x, y));
            }
        }

        protected virtual void MouseMove(SKRect bounds, float x, float y)
        {
            var tv = TreeView;
            if (tv != null && CollisionTool.Check(bounds, x, y))
            {
                loop(bounds, (rtv, rtclip, v) => v._MouseMove(rtv, x, y));
            }
        }

        protected virtual void MouseClick(SKRect bounds, float x, float y)
        {
            var tv = TreeView;
            if (tv != null && CollisionTool.Check(bounds, x, y))
            {
                #region Radio
                bool bsel = false;
                item(Util.FromRect(bounds.Left, bounds.Top, bounds.Width, tv.ItemHeight),
                    (rti, rtt, v) =>
                    {
                        if (CollisionTool.Check(rti, x, y) )
                        {
                            Expands = !Expands;
                            bsel = true;
                            ani.Start(DvDesign.ANI);
                        }
                    });

                if (!bsel && CollisionTool.Check(Util.FromRect(bounds.Left, bounds.Top, bounds.Width, tv.ItemHeight), x, y)) tv.select(this);
                #endregion

                loop(bounds, (rtv, rtclip, v) => v._MouseClick(rtv, x, y));
            }
        }

        protected virtual void MouseLongClick(SKRect bounds, float x, float y)
        {
            var tv = TreeView;
            if (tv != null && CollisionTool.Check(bounds, x, y))
            {
                loop(bounds, (rtv, rtclip, v) => v._MouseLongClick(rtv, x, y));
            }
        }

        protected virtual void MouseDoubleClick(SKRect bounds, float x, float y)
        {
            var tv = TreeView;
            if (tv != null && CollisionTool.Check(bounds, x, y))
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

        #region SetTree
        internal void SetTree(DvTreeView tree)
        {
            this.tree = tree;
            foreach (var v in Nodes) v.SetTree(tree);
        }
        #endregion
        #region loop
        private void loop(SKRect rtRow, Action<SKRect, SKRect, TreeViewNode> act)
        {
            var tv = TreeView;
            if (tv != null)
            {
                tv.Areas((rtContent, rtBox, rtScroll) =>
                {
                    var spos = tv.SPWO;
                    var ih = tv.ItemHeight;
                    var rtclip = Util.FromRect(rtRow.Left, Top + ih, rtRow.Width, ChildHeight);
                    var y = Top + ih;
                    foreach (var v in Nodes)
                    {
                        var t = v.Text;
                        v.Top = y;
                        v.Bottom = y + ih + v.ChildHeight;

                        if (CollisionTool.CheckVertical(rtBox.Top, rtBox.Bottom, v.Top + spos, v.Bottom))
                            act(new SKRect(rtRow.Left, v.Top, rtRow.Right, v.Bottom), rtclip, v);
                        else if (y > rtBox.Bottom)
                            break;

                        y += ih + v.ChildHeight;
                    }
                });
            }
        }
        #endregion
        #region item
        private void item(SKRect rtRow, Action<SKRect, SKRect, DvTextIcon> act)
        {
            var tv = TreeView;
            if (tv != null)
            {
                var itm = new DvTextIcon
                {
                    Text = Text,
                    FontSize = tv.FontSize,
                    FontName = tv.FontName,
                    FontStyle = tv.FontStyle,
                    IconGap = tv.IconGap,
                    IconString = IconString,
                    IconSize = tv.IconSize,
                    IconAlignment = DvTextIconAlignment.LeftRight,
                };

                var sti = (tv.ItemHeight - 18F) / 2F;
                var gap = 10;
                var rh = Nodes.Count > 0 ? 18 : 6;
                var dpw = (Depth) * DvTreeView.IndentWidth;
                var rtv = Util.FromRect(rtRow.Left + dpw, rtRow.Top, rtRow.Width - dpw, rtRow.Height);
                var rti = Util.FromRect(rtv.Left + sti, rtv.Top, rh, rtv.Height);
                var rtt = Util.FromRect(rti.Right + gap, rtv.Top, rtv.Width - sti - rh - gap, rtv.Height);
                act(rti, rtt, itm);
            }
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : TreeViewNodeCollection
    public class TreeViewNodeCollection : EventList2<TreeViewNode>
    {
        public DvTreeView TreeView { get; private set; } = null;
        public TreeViewNode Parent { get; private set; } = null;

        public TreeViewNodeCollection(DvTreeView treeview)
        {
            this.TreeView = treeview;
        }

        public TreeViewNodeCollection(TreeViewNode node)
        {
            this.Parent = node;
        }

        protected override void OnAppeded(IEnumerable<TreeViewNode> items)
        {
            foreach (var v in items)
            {
                v.SetTree(TreeView);
                v.Parent = Parent;
            }
            base.OnAppeded(items);
        }

        #region Reset
        public void Reset()
        {
            foreach (var v in this)
            {
                v.SetTree(TreeView);
                v.Parent = Parent;
            }
        }
        #endregion
    }
    #endregion

}

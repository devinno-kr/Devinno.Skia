using Devinno.Collections;
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
    public class DvTreeView : DvControl
    {
        #region Const
        const int IndentWidth = 20;
        #endregion

        #region Properties
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? RadioColor { get; set; } = null;
        public SKColor? SelectedColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        public int RadioSize { get; set; } = 16;

        public Padding ItemPadding { get; set; } = new Padding(0);
        public DvContentAlignment ItemAlignment { get; set; } = DvContentAlignment.MiddleCenter;
        public int ItemHeight { get; set; } = 30;
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;

        public TreeViewNodeCollection Nodes { get; } = new TreeViewNodeCollection(null);
        public List<TreeViewNode> SelectedNodes { get; } = new List<TreeViewNode>();
        public ItemSelectionMode SelectionMode { get; set; } = ItemSelectionMode.SINGLE;

        private bool _IsScrolling => scroll.IsTouchMoving || (scroll.IsTouchScrolling && scroll.TouchOffset != 0);
        public bool IsScrolling { get; private set; }
        public bool TouchMode { get => scroll.TouchMode; set => scroll.TouchMode = value; }

        public bool Animation => Design != null ? Design.Animation : false;
        #endregion

        #region Member Variable
        private Scroll scroll = new Scroll() { Direction = ScrollDirection.Vertical, TouchMode = true };
        private Animation ani = new Animation();
        private List<TreeViewNode> ls = new List<TreeViewNode>();
        private int mx = 0, my = 0;
        private TreeViewNode aniItem = null;
        private List<TreeViewNode> anils = new List<TreeViewNode>();
        #endregion

        #region Event
        public event EventHandler<TreeViewNodeMouseEventArgs> NodeDown;
        public event EventHandler<TreeViewNodeMouseEventArgs> NodeUp;
        public event EventHandler<TreeViewNodeMouseEventArgs> NodeClicked;
        public event EventHandler<TreeViewNodeMouseEventArgs> NodeDoubleClicked;
        public event EventHandler SelectedChanged;
        #endregion

        #region Constructor
        public DvTreeView()
        {
            scroll.GetScrollTotal = () => ls.Count * ItemHeight + (Animation && ani.IsPlaying ?
            (aniItem.Expands ? -ani.Value(AnimationAccel.DCL, anils.Count * ItemHeight, 0)
                             : -ani.Value(AnimationAccel.DCL, 0, anils.Count * ItemHeight))
            : 0);
            scroll.GetScrollTick = () => ItemHeight;
            scroll.GetScrollView = () => (this.Height - 2);
            scroll.GetConstrainIgnore = () => Animation && ani.IsPlaying;
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
                    var SelectedColor = this.SelectedColor ?? thm.PointColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var RadioColor = this.RadioColor ?? thm.ForeColor;
                    var RadioBoxColor = thm.CheckBoxColor;
                    var BorderColor = thm.GetBorderColor(BoxColor, ParentContainer.GetBackColor());
                    var SelectedBorderColor = thm.GetBorderColor(SelectedColor, ParentContainer.GetBackColor());
                    var ScrollBorderColor = thm.GetBorderColor(thm.ScrollBarColor, ParentContainer.GetBackColor());
                    var Corner = thm.Corner;
                    #endregion

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        thm.DrawBox(Canvas, rtBox, BoxColor, BorderColor, RoundType.Round_L, BoxStyle.Fill | BoxStyle.OutShadow);
                     
                        var sp = Canvas.Save();
                        Canvas.ClipRoundRect(new SKRoundRect(rtBox, Corner));
                       
                        #region Items
                        loop((ls, i, rt, itm, rtRow, rtRadio, rtText) =>
                        {
                            if (CollisionTool.Check(Util.FromRect(rt.Left + 1, rt.Top + 1, rt.Width - 2, rt.Height - 2), rtBox))
                            {
                                var rtBounds = Util.INT(rtText); rtBounds.Inflate(5, 0);
                                var rtRad = Util.INT(rtRadio);
                                if (SelectedNodes.Contains(itm))
                                    thm.DrawBox(Canvas, rtBounds, SelectedColor, SelectedBorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.OutShadow);
                         
                                if (itm.Nodes.Count > 0)
                                {
                                    var ns = 4;
                                    var ic = Convert.ToInt32(ItemHeight / 1.5);
                                    var c = BoxColor.BrightnessTransmit(thm.DownBrightness);
                                    thm.DrawBox(Canvas, rtRad, RadioBoxColor, BorderColor, RoundType.Ellipse, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutBevel);

                                    if (Animation && ani.IsPlaying && aniItem == itm)
                                    {
                                        var wh = aniItem.Expands ? ani.Value(AnimationAccel.DCL, RadioSize - (ns * 2), 0) : ani.Value(AnimationAccel.DCL, 0, RadioSize - (ns * 2));
                                        var rtb = Util.MakeRectangleAlign(rtRad, new SKSize(wh, wh), DvContentAlignment.MiddleCenter);
                                        thm.DrawBox(Canvas, rtb, RadioColor, BorderColor, RoundType.Ellipse, BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow);
                                    }
                                    else
                                    { 
                                        if (!itm.Expands)
                                        {
                                            var rtb = rtRad;
                                            rtb.Inflate(-ns, -ns);
                                            thm.DrawBox(Canvas, rtb, RadioColor, BorderColor, RoundType.Ellipse, BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow);
                                        }
                                    }
                                }
                                else
                                {
                                    var dot = Util.MakeRectangleAlign(rtRad, new SKSize(5, 5), DvContentAlignment.MiddleCenter);
                                    thm.DrawBox(Canvas, dot, RadioColor, BorderColor, RoundType.Ellipse, BoxStyle.Fill | BoxStyle.OutShadow);
                                }

                                if (CollisionTool.Check(rtBounds, mx, my)) thm.DrawBox(Canvas, rtBounds, Util.FromArgb(30, SKColors.White), BorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutBevel);

                                Util.DrawTextIcon(Canvas, itm.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, itm.IconString, IconSize, ForeColor, rtText);
                            }
                        });
                        #endregion
                       
                        Canvas.RestoreToCount(sp);
                       
                        #region Scroll
                        {
                            var sp2 = Canvas.Save();
                            thm.DrawBox(Canvas, rtScroll, thm.ScrollBarColor, SKColors.Black, RoundType.Round_R, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutShadow);

                            Canvas.ClipRect(Util.FromRect(rtScroll.Left, rtScroll.Top + 0, rtScroll.Width, rtScroll.Height - 0));

                            var cCur = thm.ScrollCursorOffColor;
                            if (scroll.IsScrolling || scroll.IsTouchMoving) cCur = thm.ScrollCursorOnColor;

                            var rtcur = scroll.GetScrollCursorRect(rtScroll);
                            if (rtcur.HasValue) thm.DrawBox(Canvas, rtcur.Value, cCur, ScrollBorderColor, RoundType.Round, BoxStyle.Fill);

                            Canvas.RestoreToCount(sp2);
                        }
                        #endregion

                        thm.DrawBox(Canvas, rtBox, BoxColor, BorderColor, RoundType.Round_L, BoxStyle.Border);
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
        
                if (!TouchMode)
                {
                    if (CollisionTool.Check(rtBox, x, y))
                    {
                        loop((ls, i, rt, itm, rtRow, rtRadio, rtText) =>
                        {
                            if (itm != null && CollisionTool.Check(Util.FromRect(rt.Left + 1, rt.Top + 1, rt.Width - 2, rt.Height - 2), rtBox))
                            {
                                if (CollisionTool.Check(rtText, x, y))
                                {
                                    NodeDown?.Invoke(this, new TreeViewNodeMouseEventArgs(x, y, itm));
                                }
                            }
                        });
                    }
                }

            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(int x, int y)
        {
            mx = x;
            my = y;

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

                if (CollisionTool.Check(rtBox, x, y))
                {
                    loop((ls, i, rt, itm, rtRow, rtRadio, rtText) =>
                    {
                        if (CollisionTool.Check(Util.FromRect(rt.Left + 1, rt.Top + 1, rt.Width - 2, rt.Height - 2), rtBox))
                        {
                            if (CollisionTool.Check(rtRadio, x, y))
                            {
                                itm.Expands = !itm.Expands;

                                if (Animation)
                                {
                                    aniItem = itm;
                                    anils.Clear();
                                    MS2(itm, anils);
                                   
                                    ani.Stop();
                                    ani.Start(DvDesign.ANI2);
                                }
                            }
                            if (CollisionTool.Check(rtText, x, y)) NodeUp?.Invoke(this, new TreeViewNodeMouseEventArgs(x, y, itm));

                            #region Selection
                            if (CollisionTool.Check(rtText, x, y) && !IsScrolling)
                            {
                                #region Single Selection
                                if (SelectionMode == ItemSelectionMode.SINGLE)
                                {
                                    SelectedNodes.Clear();
                                    SelectedNodes.Add(itm);
                                    if (SelectedChanged != null) SelectedChanged.Invoke(this, new EventArgs());
                                }
                                #endregion
                                #region Multi Selection
                                else if (SelectionMode == ItemSelectionMode.MULTI)
                                {
                                    if (SelectedNodes.Contains(itm))
                                    {
                                        SelectedNodes.Remove(itm);
                                        if (SelectedChanged != null) SelectedChanged.Invoke(this, new EventArgs());
                                    }
                                    else
                                    {
                                        SelectedNodes.Add(itm);
                                        if (SelectedChanged != null) SelectedChanged.Invoke(this, new EventArgs());
                                    }
                                }
                                #endregion
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
                    loop((ls, i, rt, itm, rtRow, rtRadio, rtText) =>
                    {
                        if (CollisionTool.Check(rtText, x, y))
                        {
                            NodeClicked?.Invoke(this, new TreeViewNodeMouseEventArgs(x, y, itm));
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
                    loop((ls, i, rt, itm, rtRow, rtRadio, rtText) =>
                    {
                        if (CollisionTool.Check(rtText, x, y))
                        {
                            NodeDoubleClicked?.Invoke(this, new TreeViewNodeMouseEventArgs(x, y, itm));
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
            var scwh = Convert.ToInt32(Scroll.SC_WH);
            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);
            var rtBox = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width - scwh - GAP, rtContent.Height);
            var rtScroll = Util.FromRect(rtBox.Right + GAP, rtBox.Top, scwh, rtBox.Height);

            act(rtContent, rtBox, rtScroll);
        }
        #endregion
        #region GetTreeNode
        public TreeViewNode GetTreeNode(int x, int y)
        {
            TreeViewNode ret = null;
            #region Items
            loop((ls, i, rt, itm, rtRow, rtRadio, rtText) =>
            {
                if (CollisionTool.Check(rtText, x, y)) ret = itm;
            });
            #endregion
            return ret;
        }
        #endregion
        #region MakeList
        void MakeList(List<TreeViewNode> ls)
        {
            ls.Clear();
            for (int i = 0; i < Nodes.Count; i++) MS(Nodes[i], ls);
        }

        void MS(TreeViewNode nd, List<TreeViewNode> lst)
        {
            if (Animation && ani.IsPlaying && aniItem == nd)
            {
                lst.Add(nd);
                for (int i = 0; i < nd.Nodes.Count; i++) MS(nd.Nodes[i], lst);
            }
            else
            {
                lst.Add(nd);
                if (nd.Expands) for (int i = 0; i < nd.Nodes.Count; i++) MS(nd.Nodes[i], lst);
            }
        }

        void MS2(TreeViewNode nd, List<TreeViewNode> lst)
        {
            for (int i = 0; i < nd.Nodes.Count; i++) MS(nd.Nodes[i], lst);
        }
        #endregion
        #region loop
        private void loop(Action<List<TreeViewNode>, int, SKRect, TreeViewNode, SKRect, SKRect, SKRect> Func)
        {
            var vls = new List<TreeViewNode>();
            MakeList(vls);
            ls = vls;

            bounds((rtContent, rtBox, rtScroll) =>
            {
                var sc = scroll.ScrollPosition;
                var spos = Convert.ToInt32(scroll.ScrollPositionWithOffset);
                
                if (Animation && ani.IsPlaying)
                {
                    if (aniItem != null)
                    {
                        var nh = anils.Count * ItemHeight;
                        var anih = aniItem.Expands ? ani.Value(AnimationAccel.DCL, 0, nh) : ani.Value(AnimationAccel.DCL, nh, 0);

                        var si = Convert.ToInt32(Math.Floor((double)Math.Abs(spos) / (double)ItemHeight));   //0;
                        var ei = ls.Count - 1;

                        var i = Math.Max(0, si);
                        var y = spos + rtBox.Top + (si * ItemHeight);

                        while (i < ei + 1 && i < ls.Count && y <= rtBox.Bottom)
                        {
                            var itm = ls[i];
                            var rt = Util.FromRect(rtBox.Left, y, rtBox.Width, ItemHeight);
                            if (CollisionTool.Check(Util.FromRect(rt.Left + 1, rt.Top + 1, rt.Width - 2, rt.Height - 2), rtBox))
                            {
                                if(anils.Contains(itm))
                                {
                                    var idx = anils.IndexOf(itm);
                                    var bView = false;
                                    if (anih >= idx * ItemHeight + ItemHeight)
                                    {
                                        bView = true;
                                        y += ItemHeight;
                                    }
                                    else if(anih >= idx * ItemHeight && anih < idx * ItemHeight + ItemHeight)
                                    {
                                        //bView = true;
                                        rt = Util.FromRect(rtBox.Left, y, rtBox.Width, anih % ItemHeight);
                                        y += anih % ItemHeight;
                                    }

                                    if (bView)
                                    {
                                        var idnt = IndentWidth * itm.Depth;
                                        var sz = Util.MeasureTextIcon(itm.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, itm.IconString, IconSize, IconAlignment);
                                        var w = sz.Width;
                                        var h = sz.Height;

                                        var SW = 10;
                                        var GP = 10;
                                        var rtv = Util.FromRect(rt.Left + idnt, rt.Top, rt.Width - idnt, rt.Height);
                                        var rtRadio = Util.MakeRectangleAlign(Util.FromRect(rtv.Left, rt.Top, rt.Height, rt.Height), new SKSize(RadioSize, RadioSize), DvContentAlignment.MiddleCenter);
                                        var rtText = Util.FromRect(rtRadio.Right + GP, rt.Top, w + SW, rt.Height);
                                        var rtRow = new SKRect(rtRadio.Left, rtText.Top, rtText.Right, rtText.Bottom);
                                        Func(ls, i, rt, itm, rtRow, rtRadio, rtText);
                                    }
                                }
                                else
                                {
                                    var idnt = IndentWidth * itm.Depth;
                                    var sz = Util.MeasureTextIcon(itm.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, itm.IconString, IconSize, IconAlignment);
                                    var w = sz.Width;
                                    var h = sz.Height;

                                    var SW = 10;
                                    var GP = 10;
                                    var rtv = Util.FromRect(rt.Left + idnt, rt.Top, rt.Width - idnt, rt.Height);
                                    var rtRadio = Util.MakeRectangleAlign(Util.FromRect(rtv.Left, rt.Top, rt.Height, rt.Height), new SKSize(RadioSize, RadioSize), DvContentAlignment.MiddleCenter);
                                    var rtText = Util.FromRect(rtRadio.Right + GP, rt.Top, w + SW, rt.Height);
                                    var rtRow = new SKRect(rtRadio.Left, rtText.Top, rtText.Right, rtText.Bottom);
                                    Func(ls, i, rt, itm, rtRow, rtRadio, rtText);

                                    y += ItemHeight;
                                }
                            }
                            else  y += ItemHeight;
                            i++;
                        }
                    }
                }
                else
                {
                    var si = Convert.ToInt32(Math.Floor((double)(sc - scroll.TouchOffset) / (double)ItemHeight));
                    var cnt = Convert.ToInt32(Math.Ceiling((double)(rtBox.Height - Math.Min(0, scroll.TouchOffset)) / (double)ItemHeight));
                    var ei = si + cnt;

                    for (int i = Math.Max(0, si); i < ei + 1 && i < ls.Count; i++)
                    {
                        var itm = ls[i];
                        var rt = Util.FromRect(rtBox.Left, spos + rtBox.Top + (ItemHeight * i), rtBox.Width, ItemHeight);
                        if (CollisionTool.Check(Util.FromRect(rt.Left + 1, rt.Top + 1, rt.Width - 2, rt.Height - 2), rtBox))
                        {
                            var idnt = IndentWidth * itm.Depth;
                            var sz = Util.MeasureTextIcon(itm.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, itm.IconString, IconSize, IconAlignment);
                            var w = sz.Width;
                            var h = sz.Height;

                            var SW = 10;
                            var GP = 10;
                            var rtv = Util.FromRect(rt.Left + idnt, rt.Top, rt.Width - idnt, rt.Height);
                            var rtRadio = Util.MakeRectangleAlign(Util.FromRect(rtv.Left, rt.Top, rt.Height, rt.Height), new SKSize(RadioSize, RadioSize), DvContentAlignment.MiddleCenter);
                            var rtText = Util.FromRect(rtRadio.Right + GP, rt.Top, w + SW, rt.Height);
                            var rtRow = new SKRect(rtRadio.Left, rtText.Top, rtText.Right, rtText.Bottom);
                            Func(ls, i, rt, itm, rtRow, rtRadio, rtText);
                        }
                    }
                }
            });
        }
        #endregion
        #endregion
    }


    #region class : TreeViewNode
    public class TreeViewNode : TextIconItem
    {
        #region Properties
        public TreeViewNode Parents { get; internal set; }
        public TreeViewNodeCollection Nodes { get; private set; }
        public bool Expands { get; set; } = true;
        public int Depth { get => (Parents != null ? Parents.Depth + 1 : 0); }
        #endregion
        #region Event
        internal event EventHandler Changed;
        #endregion
        #region Constructor
        public TreeViewNode(string Text) : base(Text)
        {
            Nodes = new TreeViewNodeCollection(this);
            Nodes.Changed += (o, s) =>
            {
                foreach (var v in Nodes) v.Parents = this;
                Changed?.Invoke(this, null);
            };
        }

        public TreeViewNode(string Text, string IconString) : base(Text, IconString)
        {
            Nodes = new TreeViewNodeCollection(this);
            Nodes.Changed += (o, s) =>
            {
                foreach (var v in Nodes) v.Parents = this;
                Changed?.Invoke(this, null);
            };
        }

        public TreeViewNode(string Text, string IconString, object Value) : base(Text, IconString, Value)
        {
            Nodes = new TreeViewNodeCollection(this);
            Nodes.Changed += (o, s) =>
            {
                foreach (var v in Nodes) v.Parents = this;
                Changed?.Invoke(this, null);
            };
        }
        #endregion
    }
    #endregion
    #region class : TreeViewNodeCollection
    public class TreeViewNodeCollection : EventList<TreeViewNode>
    {
        public TreeViewNode Parent { get; private set; } = null;

        public TreeViewNodeCollection(TreeViewNode node)
        {
            this.Parent = node;
        }
    }
    #endregion
    #region class : TreeViewNodeMouseEventArgs
    public class TreeViewNodeMouseEventArgs : EventArgs
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public TreeViewNode Node { get; private set; }

        public TreeViewNodeMouseEventArgs(int X, int Y, TreeViewNode Node)
        {
            this.X = X;
            this.Y = Y;
            this.Node = Node;
        }
    }
    #endregion
}

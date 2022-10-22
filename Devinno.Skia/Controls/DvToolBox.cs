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
    public class DvToolBox : DvControl
    {
        #region Const
        const int IndentWidth = 30;
        const int StartIndent = 10;
        //const int RadioSize = 6;
        #endregion

        #region Properties
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? CategoryColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;

        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;

        public EventList<ToolCategoryItem> Categories { get; } = new EventList<ToolCategoryItem>();

        public int ItemHeight { get; set; } = 30;
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;

        private bool _IsScrolling => scroll.IsTouchMoving || (scroll.IsTouchScrolling && scroll.TouchOffset != 0);
        public bool IsScrolling { get; private set; }
        public bool TouchMode { get => scroll.TouchMode; set => scroll.TouchMode = value; }

        public int RadioSize { get; set; } = 16;

        public bool Animation => Design != null ? Design.Animation : false;
        #endregion

        #region Member Variable
        private Scroll scroll = new Scroll() { Direction = ScrollDirection.Vertical, TouchMode = true };
        private Animation ani = new Animation();
        private int mx = 0, my = 0;
        private ToolCategoryItem aniItem = null;
        private List<TextIconItem> ls = new List<TextIconItem>();
        #endregion

        #region Event
        public event EventHandler<ToolItemMouseDownEventArgs> ItemDown;
        public event EventHandler<ToolItemMouseEventArgs> ItemUp;
        public event EventHandler<ToolItemMouseEventArgs> ItemClicked;
        public event EventHandler<ToolItemMouseEventArgs> ItemDoubleClicked;
        #endregion

        #region Constructor
        public DvToolBox()
        {
            scroll.GetScrollTotal = () => ls.Count * ItemHeight + (Animation && ani.IsPlaying ?
            (aniItem.Expands ? -ani.Value(AnimationAccel.DCL, aniItem.Items.Count * ItemHeight, 0)
                             : -ani.Value(AnimationAccel.DCL, 0, aniItem.Items.Count * ItemHeight))
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
                    var CategoryColor = this.CategoryColor ?? thm.RowColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BorderColor = thm.GetBorderColor(BoxColor, ParentContainer.GetBackColor());
                    var ScrollBorderColor = thm.GetBorderColor(thm.ScrollBarColor, ParentContainer.GetBackColor());
                    var Corner = thm.Corner;
                    #endregion

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        thm.DrawBox(Canvas, rtBox, BoxColor, BorderColor, RoundType.Round_L, BoxStyle.Fill | BoxStyle.OutShadow);

                        var sp = Canvas.Save();
                        Canvas.ClipRoundRect(new SKRoundRect(rtBox, Corner));

                        #region Items
                        loop((i, rt, itm, rtv, rtRadio, rtText, rtRow) =>
                        {
                            if (CollisionTool.Check(Util.FromRect(rt.Left + 1, rt.Top + 1, rt.Width - 2, rt.Height - 2), rtBox))
                            {
                                var cRow = itm is ToolCategoryItem ? CategoryColor : BoxColor;
                                var idnt = (itm is ToolCategoryItem ? StartIndent : StartIndent + (IndentWidth / 2));

                                if (itm is ToolCategoryItem)
                                {
                                    var v = itm as ToolCategoryItem;

                                    thm.DrawBox(Canvas, rtv, cRow, BorderColor, RoundType.Rect, BoxStyle.Border | BoxStyle.GradientV);
                                    Util.DrawText(Canvas, v.Text, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtText);

                                    #region Radio
                                    var wh = 12;
                                    var bw = 3;
                                    var cp = new SKPoint((int)rtRadio.MidX, (int)rtRadio.MidY);
                                    p.IsStroke = false;
                                    p.Color = ForeColor;

                                    if (Animation && ani.IsPlaying && v == aniItem)
                                    {
                                        wh = Math.Abs(ani.Value(AnimationAccel.DCL, -9, 9)) + 3;

                                        var rtH = new SKRoundRect(Util.MakeRectangle(cp, wh, bw), bw); rtH.Offset(0.5F, 0.5F);
                                        var rtV = new SKRoundRect(Util.MakeRectangle(cp, bw, wh), bw); rtV.Offset(0.5F, 0.5F);

                                        if (v.Expands)
                                        {
                                            Canvas.DrawRoundRect(rtH, p);
                                        }
                                        else
                                        {
                                            Canvas.DrawRoundRect(rtV, p);
                                            Canvas.DrawRoundRect(rtH, p);
                                        }
                                    }
                                    else
                                    {
                                        var rtH = new SKRoundRect(Util.MakeRectangle(cp, wh, bw), bw); rtH.Offset(0.5F, 0.5F);
                                        var rtV = new SKRoundRect(Util.MakeRectangle(cp, bw, wh), bw); rtV.Offset(0.5F, 0.5F);

                                        if (v.Expands)
                                        {
                                            Canvas.DrawRoundRect(rtH, p);
                                        }
                                        else
                                        {
                                            Canvas.DrawRoundRect(rtV, p);
                                            Canvas.DrawRoundRect(rtH, p);
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    var dragItem = Design.GetDragItem();
                                    if ((dragItem == null && CollisionTool.Check(rtRow, mx, my)) || dragItem == itm) thm.DrawBox(Canvas, rtRow, Util.FromArgb(30, SKColors.White), BorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutBevel);
                                    var rtdot = Util.MakeRectangle(rtRadio, new SKSize(5, 5));
                                    Util.DrawTextIcon(Canvas, itm.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, itm.IconString, IconSize, ForeColor, rtText);
                                    thm.DrawBox(Canvas, rtdot, ForeColor, BorderColor, RoundType.Ellipse, BoxStyle.Fill | BoxStyle.OutShadow);
                                }
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


                if (CollisionTool.Check(rtBox, x, y))
                {
                    loop((i, rt, itm, rtv, rtRadio, rtText, rtRow) =>
                    {
                        var v = itm as ToolItem;
                        if (v != null && CollisionTool.Check(Util.FromRect(rt.Left + 1, rt.Top + 1, rt.Width - 2, rt.Height - 2), rtBox))
                        {
                            if (CollisionTool.Check(rtRow, x, y))
                            {
                                var arg = new ToolItemMouseDownEventArgs(x, y, v);
                                ItemDown?.Invoke(this, arg);

                                if (arg.Drag)
                                {
                                    Design.Drag(v);

                                    scroll.MouseUp(x, y);
                                    if (scroll.TouchMode && CollisionTool.Check(rtBox, x, y)) scroll.TouchUp(x, y);
                                }
                            }
                        }
                    });
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
                    loop((i, rt, itm, rtv, rtRadio, rtText, rtRow) =>
                    {
                        if (CollisionTool.Check(Util.FromRect(rt.Left + 1, rt.Top + 1, rt.Width - 2, rt.Height - 2), rtBox))
                        {
                            var vcat = itm as ToolCategoryItem;
                            var v = itm as ToolItem;

                            if (vcat != null && CollisionTool.Check(rtRadio, x, y))
                            {
                                vcat.Expands = !vcat.Expands;

                                if (Animation)
                                {
                                    aniItem = vcat;
                                  
                                    ani.Stop();
                                    ani.Start(DvDesign.ANI2);
                                }
                            }
                            if (CollisionTool.Check(rtRow, x, y)) ItemUp?.Invoke(this, new ToolItemMouseEventArgs(x, y, v));

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
                    loop((i, rt, itm, rtv, rtRadio, rtText, rtRow) =>
                    {
                        var v = itm as ToolItem;
                        if (v != null && CollisionTool.Check(Util.FromRect(rt.Left + 1, rt.Top + 1, rt.Width - 2, rt.Height - 2), rtBox))
                        {
                            if (CollisionTool.Check(rtRow, x, y)) ItemClicked?.Invoke(this, new ToolItemMouseEventArgs(x, y, v));
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
            bounds((rtContent, rtBox, rtScroll) =>
            {
                if (CollisionTool.Check(rtBox, x, y) && !IsScrolling)
                {
                    loop((i, rt, itm, rtv, rtRadio, rtText, rtRow) =>
                    {
                        var v = itm as ToolItem;
                        if (v != null && CollisionTool.Check(Util.FromRect(rt.Left + 1, rt.Top + 1, rt.Width - 2, rt.Height - 2), rtBox))
                        {
                            if (CollisionTool.Check(rtRow, x, y)) ItemDoubleClicked?.Invoke(this, new ToolItemMouseEventArgs(x, y, v));
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
        #region GetToolItem
        public ToolItem GetToolItem(int mx, int my)
        {
            ToolItem ret = null;
            #region Items
            loop((i, rt, v, rtv, rtRadio, rtText, rtRow) =>
            {
                #region Category
                if (v is ToolItem)
                {
                    var itm = (ToolItem)v;
                    if (CollisionTool.Check(rtRow, mx, my))
                    {
                        ret = itm;
                    }
                }
                #endregion
            });
            #endregion
            return ret;
        }
        #endregion
        #region MakeList
        void MakeList(List<TextIconItem> ls)
        {
            ls.Clear();
            foreach (var v in Categories)
            {
                if (v.Expands || (Animation && ani.IsPlaying && aniItem == v)) { ls.Add(v); ls.AddRange(v.Items); }
                else ls.Add(v);
            }
        }
        #endregion
        #region GetListCount
        int GetListCount() => ls.Count;
        #endregion
        #region loop
        private void loop(Action<int, SKRect, TextIconItem, SKRect, SKRect, SKRect, SKRect> Func)
        {
            var vls = new List<TextIconItem>();
            MakeList(vls);
            ls = vls;

            bounds((rtContent, rtBox, rtScroll) =>
            {
                var sc = scroll.ScrollPosition;
                var spos = Convert.ToInt32(scroll.ScrollPositionWithOffset);

                if (Animation && ani.IsPlaying)
                {
                    if(aniItem != null)
                    {
                        var anils = aniItem.Items.Cast<TextIconItem>().ToList();
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
                                if (anils.Contains(itm))
                                {
                                    var idx = anils.IndexOf(itm);
                                    var bView = false;
                                    if (anih >= idx * ItemHeight + ItemHeight)
                                    {
                                        bView = true;
                                        y += ItemHeight;
                                    }
                                    else if (anih >= idx * ItemHeight && anih < idx * ItemHeight + ItemHeight)
                                    {
                                        //bView = true;
                                        rt = Util.FromRect(rtBox.Left, y, rtBox.Width, anih % ItemHeight);
                                        y += anih % ItemHeight;
                                    }

                                    if (bView)
                                    {
                                        var sz = Util.MeasureTextIcon(itm.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, itm.IconString, IconSize, IconAlignment);
                                        var w = Convert.ToInt32(sz.Width);
                                        var h = Convert.ToInt32(sz.Height);

                                        var SW = 10;
                                        var GP = 10;
                                        var rtv = Util.FromRect(rt.Left, rt.Top, rt.Width, rt.Height);

                                        if (itm is ToolCategoryItem)
                                        {
                                            var rtRadio = Util.MakeRectangleAlign(Util.FromRect(rtv.Left, rt.Top, rt.Height, rt.Height), new SKSize(RadioSize, RadioSize), DvContentAlignment.MiddleCenter);
                                            var rtText = Util.FromRect(rtRadio.Right + GP, rt.Top, w + 2, rt.Height);
                                            var rtRow = new SKRect(rtRadio.Left, rtText.Top, rtText.Right, rtText.Bottom);

                                            Func(i, rt, itm, rtv, rtRadio, rtText, rtRow);
                                        }
                                        else
                                        {
                                            var ro = 5;
                                            var rtRow = Util.MakeRectangleAlign(rtv, new SKSize(ro + GP + w, rtv.Height), DvContentAlignment.MiddleCenter);
                                            var rtRadio = Util.FromRect(rtRow.Left, rtRow.MidY - (ro / 2), ro, ro);
                                            var rtText = Util.FromRect(rtRadio.Right + GP, rt.Top, w + 2, rt.Height);
                                            rtRow.Inflate(8, 0);

                                            Func(i, rt, itm, rtv, rtRadio, rtText, rtRow);
                                        }
                                    }
                                }
                                else
                                {
                                    var sz = Util.MeasureTextIcon(itm.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, itm.IconString, IconSize, IconAlignment);
                                    var w = Convert.ToInt32(sz.Width);
                                    var h = Convert.ToInt32(sz.Height);

                                    var SW = 10;
                                    var GP = 10;
                                    var rtv = Util.FromRect(rt.Left, rt.Top, rt.Width, rt.Height);

                                    if (itm is ToolCategoryItem)
                                    {
                                        var rtRadio = Util.MakeRectangleAlign(Util.FromRect(rtv.Left, rt.Top, rt.Height, rt.Height), new SKSize(RadioSize, RadioSize), DvContentAlignment.MiddleCenter);
                                        var rtText = Util.FromRect(rtRadio.Right + GP, rt.Top, w + 2, rt.Height);
                                        var rtRow = new SKRect(rtRadio.Left, rtText.Top, rtText.Right, rtText.Bottom);

                                        Func(i, rt, itm, rtv, rtRadio, rtText, rtRow);
                                    }
                                    else
                                    {
                                        var ro = 5;
                                        var rtRow = Util.MakeRectangleAlign(rtv, new SKSize(ro + GP + w, rtv.Height), DvContentAlignment.MiddleCenter);
                                        var rtRadio = Util.FromRect(rtRow.Left, rtRow.MidY - (ro / 2), ro, ro);
                                        var rtText = Util.FromRect(rtRadio.Right + GP, rt.Top, w + 2, rt.Height);
                                        rtRow.Inflate(8, 0);

                                        Func(i, rt, itm, rtv, rtRadio, rtText, rtRow);
                                    }

                                    y += ItemHeight;
                                }
                            }
                            else y += ItemHeight;
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
                            var sz = Util.MeasureTextIcon(itm.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, itm.IconString, IconSize, IconAlignment);
                            var w = Convert.ToInt32(sz.Width);
                            var h = Convert.ToInt32(sz.Height);

                            var SW = 10;
                            var GP = 10;
                            var rtv = Util.FromRect(rt.Left, rt.Top, rt.Width, rt.Height);

                            if (itm is ToolCategoryItem)
                            {
                                var rtRadio = Util.MakeRectangleAlign(Util.FromRect(rtv.Left, rt.Top, rt.Height, rt.Height), new SKSize(RadioSize, RadioSize), DvContentAlignment.MiddleCenter);
                                var rtText = Util.FromRect(rtRadio.Right + GP, rt.Top, w + 2, rt.Height);
                                var rtRow = new SKRect(rtRadio.Left, rtText.Top, rtText.Right, rtText.Bottom);

                                Func(i, rt, itm, rtv, rtRadio, rtText, rtRow);
                            }
                            else
                            {
                                var ro = 5;
                                var rtRow = Util.MakeRectangleAlign(rtv, new SKSize(ro + GP + w, rtv.Height), DvContentAlignment.MiddleCenter);
                                var rtRadio = Util.FromRect(rtRow.Left, rtRow.MidY - (ro / 2), ro, ro);
                                var rtText = Util.FromRect(rtRadio.Right + GP, rt.Top, w + 2, rt.Height);
                                rtRow.Inflate(8, 0);

                                Func(i, rt, itm, rtv, rtRadio, rtText, rtRow);
                            }
                        }
                    }
                }
            });
        }
        #endregion
        #endregion
    }

    #region class : ToolCategoryItem
    public class ToolCategoryItem : TextIconItem
    {
        #region Properties
        public bool Expands { get; set; } = true;
        public EventList<ToolItem> Items { get; } = new EventList<ToolItem>();
        #endregion
        #region Constructor
        public ToolCategoryItem(string Text) : base(Text) => Items.Changed += (o, s) => Changed?.Invoke(this, null);
        public ToolCategoryItem(string Text, string IconString) : base(Text, IconString) => Items.Changed += (o, s) => Changed?.Invoke(this, null);
        #endregion
        #region Event
        internal event EventHandler Changed;
        #endregion
    }
    #endregion
    #region class : ToolItem
    public class ToolItem : TextIconItem
    {
        #region Constructor
        public ToolItem(string Text) : base(Text) { }
        public ToolItem(string Text, string IconString) : base(Text, IconString) { }
        #endregion
    }
    #endregion
    #region class : ToolItemMouseEventArgs
    public class ToolItemMouseEventArgs : EventArgs
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public ToolItem Item { get; private set; }

        public ToolItemMouseEventArgs(int X, int Y, ToolItem Item)
        {
            this.X = X;
            this.Y = Y;
            this.Item = Item;
        }
    }

    public class ToolItemMouseDownEventArgs : ToolItemMouseEventArgs
    {
        public bool Drag { get; set; }
        public ToolItemMouseDownEventArgs(int X, int Y, ToolItem Item) : base(X, Y, Item) { }
    }
    #endregion
}

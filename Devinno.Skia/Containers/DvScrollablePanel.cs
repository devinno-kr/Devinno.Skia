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

namespace Devinno.Skia.Containers
{
    public class DvScrollablePanel : DvContainer
    {
        #region Properties
        private bool _IsScrolling => vscroll.IsTouchMoving || hscroll.IsTouchMoving || (vscroll.IsTouchScrolling && vscroll.TouchOffset != 0) || (hscroll.IsTouchScrolling && hscroll.TouchOffset != 0);
        public bool IsScrolling { get; private set; }
        public bool TouchMode { get => vscroll.TouchMode; set => vscroll.TouchMode = hscroll.TouchMode = value; }
        #endregion

        #region Member Variable
        Scroll vscroll = new Scroll() { Direction = ScrollDirection.Vertical, TouchMode = true };
        Scroll hscroll = new Scroll() { Direction = ScrollDirection.Horizon, TouchMode = true };

        double hST = 0, hSV = 0;
        double vST = 0, vSV = 0;
        #endregion

        #region Constructor
        public DvScrollablePanel()
        {
            #region Scroll
            hscroll.GetScrollTotal = () => hST;
            hscroll.GetScrollTick = () => 1;
            hscroll.GetScrollView = () => hSV;
            
            vscroll.GetScrollTotal = () => vST;
            vscroll.GetScrollTick = () => 1;
            vscroll.GetScrollView = () => vSV;
            #endregion

            Controls.Changed += (o, s) =>
            {
                hST = Controls.Max(x => x.Value.X + x.Value.Width);
                vST = Controls.Max(x => x.Value.Y + x.Value.Height);
            };
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtPanel, rtScrollV, rtScrollH) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var BorderColor = thm.GetBorderColor(thm.ScrollBarColor, ParentContainer.GetBackColor());
                    var ScrollBorderColor = thm.GetBorderColor(thm.ScrollBarColor, ParentContainer.GetBackColor());

                    #region ScrollV
                    if (vscroll.ScrollVisible)
                    {
                        var rtScroll = rtScrollV;
                        var scroll = vscroll;

                        var sp = Canvas.Save();
                        thm.DrawBox(Canvas, rtScroll, thm.ScrollBarColor, BorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill);

                        Canvas.ClipRect(rtScroll);

                        var cCur = thm.ScrollCursorOffColor;
                        if (scroll.IsScrolling || scroll.IsTouchMoving) cCur = thm.ScrollCursorOnColor;

                        var rtcur = scroll.GetScrollCursorRect(rtScroll);
                        if (rtcur.HasValue) thm.DrawBox(Canvas, rtcur.Value, cCur, ScrollBorderColor, RoundType.Round, BoxStyle.Fill);

                        Canvas.RestoreToCount(sp);
                    }
                    #endregion
                    #region ScrollH
                    if(hscroll.ScrollVisible)
                    {
                        var rtScroll = rtScrollH;
                        var scroll = hscroll;

                        var sp = Canvas.Save();
                        thm.DrawBox(Canvas, rtScroll, thm.ScrollBarColor, BorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill);

                        Canvas.ClipRect(rtScroll);

                        var cCur = thm.ScrollCursorOffColor;
                        if (scroll.IsScrolling || scroll.IsTouchMoving) cCur = thm.ScrollCursorOnColor;

                        var rtcur = scroll.GetScrollCursorRect(rtScroll);
                        if (rtcur.HasValue) thm.DrawBox(Canvas, rtcur.Value, cCur, ScrollBorderColor, RoundType.Round, BoxStyle.Fill);

                        Canvas.RestoreToCount(sp);
                    }
                    #endregion

                    if (Controls != null)
                    {
                        var vc = Convert.ToInt32(vscroll.ScrollPositionWithOffset);
                        var hc = Convert.ToInt32(hscroll.ScrollPositionWithOffset);
                        var rtPnl = rtPanel;
                        rtPnl.Offset(-hc, -vc);

                        foreach (var v in Controls.Values.Where(x=>CollisionTool.Check(Util.FromRect(x), rtPnl)))
                        {
                            if (v.Visible)
                            {
                                var sp = Canvas.Save();
                                Canvas.Translate(v.X + hc, v.Y + vc);
                                //Canvas.ClipRect(Util.FromRect(0, 0, v.Width, v.Height));

                                v._Draw(Canvas);
                                Canvas.RestoreToCount(sp);
                            }
                        }
                    }
                }
            });

            //base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            var vc = Convert.ToInt32(vscroll.ScrollPositionWithOffset);
            var hc = Convert.ToInt32(hscroll.ScrollPositionWithOffset);

            bounds((rtContent, rtPanel, rtScrollV, rtScrollH) =>
            {
                #region Scroll
                vscroll.MouseDown(x, y, rtScrollV);
                hscroll.MouseDown(x, y, rtScrollH);

                if (vscroll.TouchMode && CollisionTool.Check(rtPanel, x, y)) { vscroll.TouchDown(x, y); }
                if (hscroll.TouchMode && CollisionTool.Check(rtPanel, x, y)) { hscroll.TouchDown(x, y); }
                #endregion
                IsScrolling = _IsScrolling;
                #region Controls
                bool bcl = false;
                var _x = x;
                var _y = y;
                if (Controls != null)
                {
                    var rtPnl = rtPanel;
                    rtPnl.Offset(-hc, -vc);
                    _x -= hc;
                    _y -= vc;

                    foreach (var v in Controls.Values.Where(x => CollisionTool.Check(Util.FromRect(x), rtPnl)))
                    {
                        var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), _x, _y);
                        if (bCheck)
                        {
                            v._bMouseDown_ = true;
                            if (v.Visible && v.Enabled && this.Enabled) v._MouseDown(_x - v.X, _y - v.Y);
                            bcl = bCheck;
                        }
                    }
                }
                #endregion
            });
         
            //base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(int x, int y)
        {
            var vc = Convert.ToInt32(vscroll.ScrollPositionWithOffset);
            var hc = Convert.ToInt32(hscroll.ScrollPositionWithOffset);

            bounds((rtContent, rtPanel, rtScrollV, rtScrollH) =>
            {
                #region Scroll
                vscroll.MouseMove(x, y, rtScrollV);
                hscroll.MouseMove(x, y, rtScrollH);
                if (vscroll.TouchMode) vscroll.TouchMove(x, y);
                if (hscroll.TouchMode) hscroll.TouchMove(x, y);
                #endregion
                IsScrolling = _IsScrolling;
                #region Controls
                bool bcl = false;
                if (Controls != null)
                {
                    var rtPnl = rtPanel;
                    rtPnl.Offset(-hc, -vc);
                    x -= hc;
                    y -= vc;

                    foreach (var v in Controls.Values.Where(x => CollisionTool.Check(Util.FromRect(x), rtPnl)))
                    {
                        var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y);
                        if (bCheck || v._bMouseDown_)
                        {
                            if (v.Visible && v.Enabled && this.Enabled) v._MouseMove(x - v.X, y - v.Y);
                            bcl = bCheck;
                        }
                    }
                }
                #endregion
            });
            //base.OnMouseMove(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            var vc = Convert.ToInt32(vscroll.ScrollPositionWithOffset);
            var hc = Convert.ToInt32(hscroll.ScrollPositionWithOffset);

            bounds((rtContent, rtPanel, rtScrollV, rtScrollH) =>
            {
                IsScrolling = _IsScrolling;
                #region Scroll
                vscroll.MouseUp(x, y);
                hscroll.MouseUp(x, y);
                if (vscroll.TouchMode) vscroll.TouchUp(x, y);
                if (hscroll.TouchMode) hscroll.TouchUp(x, y);
                #endregion

                #region Controls
                bool bcl = false;
                if (Controls != null)
                {
                    var rtPnl = rtPanel;
                    rtPnl.Offset(-hc, -vc);
                    x -= hc;
                    y -= vc;

                    foreach (var v in Controls.Values.Where(x => CollisionTool.Check(Util.FromRect(x), rtPnl)))
                    {
                        var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y);
                        if (bCheck || v._bMouseDown_)
                        {
                            if (v.Visible && v.Enabled && this.Enabled)
                                v._MouseUp(x - v.X, y - v.Y);
                            bcl = bCheck;
                        }
                        v._bMouseDown_ = false;
                    }
                }

                #endregion
            });

            //base.OnMouseUp(x, y);
        }
        #endregion
        #region OnMouseDoubleClick
        protected override void OnMouseDoubleClick(int x, int y)
        {
            var vc = Convert.ToInt32(vscroll.ScrollPositionWithOffset);
            var hc = Convert.ToInt32(hscroll.ScrollPositionWithOffset);

            bounds((rtContent, rtPanel, rtScrollV, rtScrollH) =>
            {
                #region Controls
                bool bcl = false;
                if (Controls != null)
                {
                    var rtPnl = rtPanel;
                    rtPnl.Offset(-hc, -vc);
                    x -= hc;
                    y -= vc;

                    foreach (var v in Controls.Values.Where(x => CollisionTool.Check(Util.FromRect(x), rtPnl)))
                    {
                        var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y);
                        if (bCheck)
                        {
                            if (v.Visible && v.Enabled && this.Enabled) v._MouseDoubleClick(x - v.X, y - v.Y);
                            bcl = bCheck;
                        }
                    }
                }
                #endregion
            });

            //base.OnMouseDoubleClick(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect> act)
        {
            var GP = DvTheme.GP / 2F;

            var scwh = Convert.ToInt32(Scroll.SC_WH);

            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);
            var rtPanel = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width - GP - scwh, rtContent.Height - GP - scwh);
            var rtScrollV = Util.FromRect(rtPanel.Right, rtPanel.Top, scwh, rtPanel.Height);
            var rtScrollH = Util.FromRect(rtPanel.Left, rtPanel.Bottom, rtPanel.Width, scwh);

            hSV = rtContent.Width;
            vSV = rtContent.Height;

            act(rtContent, rtPanel, rtScrollV, rtScrollH);
        }
        #endregion
        #endregion
    }
}

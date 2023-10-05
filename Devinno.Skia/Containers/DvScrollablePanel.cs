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

namespace Devinno.Skia.Containers
{
    public class DvScrollablePanel : DvContainer
    {
        #region Properties
        public bool IsScrolling => vscroll.IsTouchMoving || hscroll.IsTouchMoving || (vscroll.IsTouchScrolling && vscroll.TouchOffset != 0) || (hscroll.IsTouchScrolling && hscroll.TouchOffset != 0);
        public bool TouchMode
        {
            get => vscroll.TouchMode;
            set => vscroll.TouchMode = hscroll.TouchMode = value;
        }
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
            Areas((rtContent, rtPanel, rtScrollV, rtScrollH) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region var 
                    var BackColor = ParentContainer.GetBackColor();
                    var BorderColor = thm.GetBorderColor(thm.ScrollBarColor, BackColor);
                    var Corner = thm.Corner;
                    #endregion

                    #region Scroll
                    if (vscroll.ScrollTotal > vscroll.ScrollView) thm.DrawScroll(Canvas, rtScrollV, BackColor, vscroll);
                    if (hscroll.ScrollTotal > hscroll.ScrollView) thm.DrawScroll(Canvas, rtScrollH, BackColor, hscroll);
                    #endregion

                    #region Controls
                    if (Controls != null)
                    {
                        var vc = Convert.ToInt32(vscroll.ScrollPositionWithOffset);
                        var hc = Convert.ToInt32(hscroll.ScrollPositionWithOffset);
                        var rtPnl = rtPanel;
                        rtPnl.Offset(-hc, -vc);

                        var lsc = Controls.Values.Where(x => CollisionTool.Check(Util.FromRect(x), rtPnl)).ToList();
                        foreach (var v in lsc)
                        {
                            if (v.Visible)
                            {
                                var sp = Canvas.Save();

                                Canvas.Translate(v.X + hc, v.Y + vc);
                                v._Draw(Canvas);

                                Canvas.RestoreToCount(sp);
                            }
                        }
                    }
                    #endregion
                }
            });

            //base.OnDraw(Canvas);
        }
        #endregion

        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            var vc = Convert.ToSingle(vscroll.ScrollPositionWithOffset);
            var hc = Convert.ToSingle(hscroll.ScrollPositionWithOffset);

            Areas((rtContent, rtPanel, rtScrollV, rtScrollH) =>
            {
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

                #region Scroll
                if (Design?.InputControl == null)
                {
                    vscroll.MouseDown(x, y, rtScrollV);
                    hscroll.MouseDown(x, y, rtScrollH);
                    if (hscroll.TouchMode && CollisionTool.Check(rtPanel, x, y)) { hscroll.TouchDown(x, y); }
                    if (vscroll.TouchMode && CollisionTool.Check(rtPanel, x, y)) { vscroll.TouchDown(x, y); }
                }
                #endregion
            });
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(float x, float y)
        {
            var vc = Convert.ToSingle(vscroll.ScrollPositionWithOffset);
            var hc = Convert.ToSingle(hscroll.ScrollPositionWithOffset);

            Areas((rtContent, rtPanel, rtScrollV, rtScrollH) =>
            {
                #region Scroll
                vscroll.MouseMove(x, y, rtScrollV);
                hscroll.MouseMove(x, y, rtScrollH);
                if (vscroll.TouchMode) vscroll.TouchMove(x, y);
                if (hscroll.TouchMode) hscroll.TouchMove(x, y);
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
                            if (v.Visible && v.Enabled && this.Enabled) v._MouseMove(x - v.X, y - v.Y);
                            bcl = bCheck;
                        }
                    }
                }
                #endregion

            });
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            var vc = Convert.ToSingle(vscroll.ScrollPositionWithOffset);
            var hc = Convert.ToSingle(hscroll.ScrollPositionWithOffset);

            Areas((rtContent, rtPanel, rtScrollV, rtScrollH) =>
            {
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
        }
        #endregion
        #region OnMouseDoubleClick
        protected override void OnMouseDoubleClick(float x, float y)
        {
            var vc = Convert.ToSingle(vscroll.ScrollPositionWithOffset);
            var hc = Convert.ToSingle(hscroll.ScrollPositionWithOffset);

            Areas((rtContent, rtPanel, rtScrollV, rtScrollH) =>
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
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect> act)
        {
            var GP = 5;

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

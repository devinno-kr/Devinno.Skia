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
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Devinno.Skia.Containers
{
    public class DvSlidePanel : DvContainer
    {
        #region Properties
        public DvSubPageCollection Pages { get; private set; }
        #region public DvSubPage SelectedPage { get; set; }
        [JsonIgnore]
        public DvSubPage SelectedPage
        {
            get => nowSelPage;
            set
            {
                if (Pages.Values.Contains(value))
                {
                    if (nowSelPage != value)
                    {
                        nowSelPage = value;

                        SelectedPageChanged?.Invoke(this, null);
                    }
                }
            }
        }
        #endregion
        #region public string SelectedPageName { get; set; }
        [JsonIgnore]
        public string SelectedPageName
        {
            get => nowSelPage?.Name;
            set
            {
                if (Pages.ContainsKey(value)) SelectedPage = Pages[value];
            }
        }
        #endregion
        public override DvControlCollection Controls => SelectedPage?.Controls;
        public bool Animation => Design != null ? Design.Animation : false;
        public AnimationType AnimationType { get; set; } = AnimationType.SlideH;

        public SKColor? ForeColor { get; set; } = null;
        public bool UseMoveButton { get; set; } = true;
        public bool UsePageButton { get; set; } = true;
        public DvDirectionHV Direction { get; set; } = DvDirectionHV.Horizon;

        #endregion

        #region Member Variable
        private DvSubPage nowSelPage = null;
        private Animation ani = new Animation();

        private SKPoint? downPoint = null;
        private SKPoint? movePoint = null;
        private DateTime downTime;

        private bool bPrevDown = false;
        private bool bNextDown = false;

        private float sx, nx;
        #endregion

        #region Event
        public EventHandler SelectedPageChanged;
        #endregion

        #region Constructor
        public DvSlidePanel()
        {
            Pages = new DvSubPageCollection(this);
            Pages.Changed += (o, s) => OnLayout();
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtPage, rtNavi, rtPrev, rtNext) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var Corner = thm.Corner;

                    OnLayout();

                    #region Navi
                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        #region Button
                        if (UseMoveButton)
                        {
                            var rtp = rtPrev; if (bPrevDown) rtp.Offset(0, 1);
                            var rtn = rtNext; if (bNextDown) rtn.Offset(0, 1);

                            var cP = bPrevDown ? ForeColor.BrightnessTransmit(thm.DownBrightness) : ForeColor;
                            var cN = bNextDown ? ForeColor.BrightnessTransmit(thm.DownBrightness) : ForeColor;

                            Util.DrawIconFA(Canvas, Direction == DvDirectionHV.Horizon ? "fa-chevron-left" : "fa-chevron-up", Convert.ToInt32(rtPrev.Width * 0.9F), cP, rtp, DvContentAlignment.MiddleCenter, true);
                            Util.DrawIconFA(Canvas, Direction == DvDirectionHV.Horizon ? "fa-chevron-right" : "fa-chevron-down", Convert.ToInt32(rtNext.Width * 0.9F), cN, rtn, DvContentAlignment.MiddleCenter, true);
                        }
                        #endregion

                        #region Navi
                        if(UsePageButton)
                        {
                            var wh = rtNavi.Height;
                            var rtv = Util.MakeRectangleAlign(rtNavi, new SKSize((wh * Pages.Count) + (DvTheme.GP2 * (Pages.Count - 1)), wh), DvContentAlignment.MiddleCenter);

                            int i = 0;
                            foreach (var page in Pages.Values)
                            {
                                var rt = Util.FromRect(rtv.Left + ((wh + DvTheme.GP2) * i), rtv.Top, wh, wh);
                                var cp = MathTool.CenterPoint(rt);
                                var dist = wh / 2F * 0.75F;
                                var dist2 = wh / 2F * 0.4F;

                                p.IsStroke = true;
                                p.Color = ForeColor;
                                p.StrokeWidth = 2;
                                Canvas.DrawCircle(cp, dist, p);

                                if (SelectedPage == page)
                                {
                                    p.IsStroke = false;
                                    Canvas.DrawCircle(cp, dist2, p);
                                }
                                i++;
                            }
                        }
                        #endregion
                    }
                    #endregion

                    var ls = Pages.Values.ToList();
                    if (Animation)
                    {
                        if (ani.IsPlaying)
                        {
                            if (Direction == DvDirectionHV.Horizon)
                            {
                                #region Go / Back
                                var nv = ani.Value(AnimationAccel.DCL, sx, nx);

                                foreach (var c in Pages.Values.Where(c => CollisionTool.Check(Util.FromRect(c.X - nv, c.Y, c.Width, c.Height), Util.FromRect(10, 10, rtPage.Width - 20, rtPage.Height - 20))))
                                {
                                    var sp = Canvas.Save();
                                    Canvas.ClipRect(Util.FromRect(rtPage.Left - 1, rtPage.Top - 1, rtPage.Width + 2, rtPage.Height + 2));
                                    Canvas.Translate(rtPage.Left + c.Left - nv, 0);

                                    c._Draw(Canvas);

                                    Canvas.RestoreToCount(sp);
                                }
                                #endregion
                            }
                            else if (Direction == DvDirectionHV.Vertical)
                            {
                                #region Go / Back
                                var nv = ani.Value(AnimationAccel.DCL, sx, nx);

                                foreach (var c in Pages.Values.Where(c => CollisionTool.Check(Util.FromRect(c.X - nv, c.Y, c.Width, c.Height), Util.FromRect(10, 10, rtPage.Width - 20, rtPage.Height - 20))))
                                {
                                    var sp = Canvas.Save();
                                    Canvas.ClipRect(Util.FromRect(rtPage.Left - 1, rtPage.Top - 1, rtPage.Width + 2, rtPage.Height + 2));
                                    Canvas.Translate(0, rtPage.Top + c.Top - nv);

                                    c._Draw(Canvas);

                                    Canvas.RestoreToCount(sp);
                                }
                                #endregion
                            }
                        }
                        else
                        {
                            if (Direction == DvDirectionHV.Horizon)
                            {
                                #region Normal / Drag
                                var sv = (SelectedPage != null ? ls.IndexOf(SelectedPage) : -1) * rtPage.Width;
                                var ov = 0F;
                                if (downPoint.HasValue && movePoint.HasValue) ov = downPoint.Value.X - movePoint.Value.X;

                                foreach (var c in Pages.Values.Where(c => CollisionTool.Check(Util.FromRect(c.X - ov - sv, c.Y, c.Width, c.Height), Util.FromRect(10, 10, rtPage.Width - 20, rtPage.Height - 20))))
                                {
                                    var sp = Canvas.Save();
                                    Canvas.ClipRect(Util.FromRect(rtPage.Left - 1, rtPage.Top - 1, rtPage.Width + 2, rtPage.Height + 2));
                                    Canvas.Translate(rtPage.Left + c.Left - ov - sv, 0);

                                    c._Draw(Canvas);

                                    Canvas.RestoreToCount(sp);
                                }
                                #endregion
                            }
                            else if (Direction == DvDirectionHV.Vertical)
                            {
                                #region Normal / Drag
                                var sv = (SelectedPage != null ? ls.IndexOf(SelectedPage) : -1) * rtPage.Height;
                                var ov = 0F;
                                if (downPoint.HasValue && movePoint.HasValue) ov = downPoint.Value.Y - movePoint.Value.Y;

                                foreach (var c in Pages.Values.Where(c => CollisionTool.Check(Util.FromRect(c.X - ov - sv, c.Y, c.Width, c.Height), Util.FromRect(10, 10, rtPage.Width - 20, rtPage.Height - 20))))
                                {
                                    var sp = Canvas.Save();
                                    Canvas.ClipRect(Util.FromRect(rtPage.Left - 1, rtPage.Top - 1, rtPage.Width + 2, rtPage.Height + 2));
                                    Canvas.Translate(0, rtPage.Top + c.Top - ov - sv);

                                    c._Draw(Canvas);

                                    Canvas.RestoreToCount(sp);
                                }
                                #endregion
                            }
                        }
                    }
                    else
                    {
                        #region SelectedPage Draw
                        var sv = (SelectedPage != null ? ls.IndexOf(SelectedPage) : -1) * rtPage.Width;

                        var sp = Canvas.Save();
                        Canvas.ClipRect(Util.FromRect(rtPage.Left - 1, rtPage.Top - 1, rtPage.Width + 2, rtPage.Height + 2));
                        Canvas.Translate(rtPage.Left + SelectedPage.Left - sv, 0);

                        SelectedPage._Draw(Canvas);

                        Canvas.RestoreToCount(sp);
                        #endregion
                    }
                }
            });
        }
        #endregion
        #region OnMouse[Down/Move/Up/DoubleClick]
        protected override void OnMouseDown(int x, int y)
        {
            bounds((rtContent, rtPage, rtNavi, rtPrev, rtNext) =>
            {
                #region Slide
                if (Animation)
                {
                    if (CollisionTool.Check(rtPage, x, y))
                    {
                        ani.Stop();

                        movePoint = downPoint = new SKPoint(x, y);
                        downTime = DateTime.Now;
                    }
                }
                #endregion
                #region Button
                if(UseMoveButton)
                {
                    if (CollisionTool.Check(rtPrev, x, y)) bPrevDown = true;
                    if (CollisionTool.Check(rtNext, x, y)) bNextDown = true;
                }
                #endregion
                #region Navi
                if (UsePageButton)
                {
                    var wh = rtNavi.Height;
                    var rtv = Util.MakeRectangleAlign(rtNavi, new SKSize((wh * Pages.Count) + (DvTheme.GP2 * (Pages.Count - 1)), wh), DvContentAlignment.MiddleCenter);
                    var ls = Pages.Values.ToList();

                    int i = 0;
                    foreach (var page in Pages.Values)
                    {
                        var rt = Util.FromRect(rtv.Left + ((wh + DvTheme.GP2) * i), rtv.Top, wh, wh);
                        var cp = MathTool.CenterPoint(rt);

                        if (CollisionTool.Check(rt, x, y))
                        {
                            var si = SelectedPage != null ? ls.IndexOf(SelectedPage) : -1;
                            var ni = Convert.ToInt32(MathTool.Constrain(i, 0, Pages.Count - 1));
                            SelectedPage = ls[ni];

                            if (Animation)
                            {
                                if (Direction == DvDirectionHV.Horizon)
                                {
                                    sx = si != -1 ? ls[si].Left : 0;
                                    nx = ni != -1 ? ls[ni].Left : 0;
                                }
                                else if (Direction == DvDirectionHV.Vertical)
                                {
                                    sx = si != -1 ? ls[si].Top : 0;
                                    nx = ni != -1 ? ls[ni].Top : 0;
                                }

                                ani.Stop();
                                ani.Start(DvDesign.ANI * 2, "go");
                            }
                        }

                        i++;
                    }
                }
                #endregion
                base.OnMouseDown(x - (int)rtPage.Left, y - (int)rtPage.Top);
            });
        }
        protected override void OnMouseMove(int x, int y)
        {
            bounds((rtContent, rtPage, rtNavi, rtPrev, rtNext) =>
            {
                #region Slide
                if (Animation)
                {
                    if (downPoint.HasValue && MathTool.GetDistance(downPoint.Value, new SKPoint(x, y)) >= 10) movePoint = new SKPoint(x, y);
                }
                #endregion

                base.OnMouseMove(x - (int)rtPage.Left, y - (int)rtPage.Top);
            });
        }
        protected override void OnMouseUp(int x, int y)
        {
            bounds((rtContent, rtPage, rtNavi, rtPrev, rtNext) =>
            {
                #region Slide
                var ls = Pages.Values.ToList();

                if (Animation)
                {
                    movePoint = new SKPoint(x, y);
                    if (downPoint.HasValue && movePoint.HasValue && MathTool.GetDistance(downPoint.Value, new SKPoint(x, y)) >= 10)
                    {
                        var vel = (Direction == DvDirectionHV.Horizon ? (movePoint.Value.X - downPoint.Value.X) / (DateTime.Now - downTime).TotalMilliseconds
                                                                      : (movePoint.Value.Y - downPoint.Value.Y) / (DateTime.Now - downTime).TotalMilliseconds);

                        if (Math.Abs(vel) >= 1)
                        {
                            var si = SelectedPage != null ? ls.IndexOf(SelectedPage) : -1;
                            var ni = Convert.ToInt32(MathTool.Constrain(vel > 0 ? si - 1 : si + 1, 0, Pages.Count - 1));

                            if (si != ni)
                            {
                                #region Go
                                if (Direction == DvDirectionHV.Horizon)
                                {
                                    var sv = (SelectedPage != null ? ls.IndexOf(SelectedPage) : -1) * rtPage.Width;

                                    sx = sv + downPoint.Value.X - movePoint.Value.X;
                                    nx = ni != -1 ? ls[ni].Left : 0;
                                }
                                else if (Direction == DvDirectionHV.Vertical)
                                {
                                    var sv = (SelectedPage != null ? ls.IndexOf(SelectedPage) : -1) * rtPage.Height;

                                    sx = sv + downPoint.Value.Y - movePoint.Value.Y;
                                    nx = ni != -1 ? ls[ni].Top : 0;
                                }
                             
                                SelectedPage = ls[ni];

                                ani.Start(DvDesign.ANI * 2, "go");
                                #endregion
                            }
                            else
                            {
                                #region Back
                                if (Direction == DvDirectionHV.Horizon && downPoint.HasValue && movePoint.HasValue)
                                {
                                    var sv = (SelectedPage != null ? ls.IndexOf(SelectedPage) : -1) * rtPage.Width;

                                    sx = sv + downPoint.Value.X - movePoint.Value.X;
                                    nx = sv;
                                }
                                else if (Direction == DvDirectionHV.Vertical && downPoint.HasValue && movePoint.HasValue)
                                {
                                    var sv = (SelectedPage != null ? ls.IndexOf(SelectedPage) : -1) * rtPage.Height;

                                    sx = sv + downPoint.Value.Y - movePoint.Value.Y;
                                    nx = sv;
                                }

                                ani.Start(DvDesign.ANI * 2, "back");
                                #endregion
                            }
                        }
                        else
                        {
                            #region Back
                            if (Direction == DvDirectionHV.Horizon && downPoint.HasValue && movePoint.HasValue)
                            {
                                var sv = (SelectedPage != null ? ls.IndexOf(SelectedPage) : -1) * rtPage.Width;

                                sx = sv + downPoint.Value.X - movePoint.Value.X;
                                nx = sv;
                            }
                            else if (Direction == DvDirectionHV.Vertical && downPoint.HasValue && movePoint.HasValue)
                            {
                                var sv = (SelectedPage != null ? ls.IndexOf(SelectedPage) : -1) * rtPage.Height;

                                sx = sv + downPoint.Value.Y - movePoint.Value.Y;
                                nx = sv;
                            }

                            ani.Start(DvDesign.ANI * 2, "back");
                            #endregion
                        }

                    }
                    movePoint = downPoint = null;
                }
                #endregion
                #region Button
                if (bPrevDown && CollisionTool.Check(rtPrev, x, y))
                {
                    #region Go
                    var si = SelectedPage != null ? ls.IndexOf(SelectedPage) : -1;
                    var ni = Convert.ToInt32(MathTool.Constrain(si - 1, 0, Pages.Count - 1));

                    if (Direction == DvDirectionHV.Horizon)
                    {
                        sx = si != -1 ? ls[si].Left : 0;
                        nx = ni != -1 ? ls[ni].Left : 0;
                    }
                    else if (Direction == DvDirectionHV.Vertical)
                    {
                        sx = si != -1 ? ls[si].Top : 0;
                        nx = ni != -1 ? ls[ni].Top : 0;
                    }

                    SelectedPage = ls[ni];

                    ani.Start(DvDesign.ANI * 2, "go");
                    #endregion
                }
                else if (bNextDown && CollisionTool.Check(rtNext, x, y))
                {
                    #region Go
                    var si = SelectedPage != null ? ls.IndexOf(SelectedPage) : -1;
                    var ni = Convert.ToInt32(MathTool.Constrain(si + 1, 0, Pages.Count - 1));

                    if (Direction == DvDirectionHV.Horizon)
                    {
                        sx = si != -1 ? ls[si].Left : 0;
                        nx = ni != -1 ? ls[ni].Left : 0;
                    }
                    else if (Direction == DvDirectionHV.Vertical)
                    {
                        sx = si != -1 ? ls[si].Top : 0;
                        nx = ni != -1 ? ls[ni].Top : 0;
                    }

                    SelectedPage = ls[ni];

                    ani.Start(DvDesign.ANI * 2, "go");
                    #endregion
                }
                bPrevDown = bNextDown = false;
                #endregion

                base.OnMouseUp(x - (int)rtPage.Left, y - (int)rtPage.Top);
            });
        }
        protected override void OnMouseDoubleClick(int x, int y)
        {
            bounds((rtContent, rtPage, rtNavi, rtPrev, rtNext) =>
            {
                base.OnMouseDoubleClick(x - (int)rtPage.Left, y - (int)rtPage.Top);
            });
        }
        #endregion
        #region OnLayout
        protected override void OnLayout()
        {
            bounds((rtContent, rtPage, rtNavi, rtPrev, rtNext) =>
            {
                int s = 0;

                foreach (var v in Pages.Values)
                {
                    v.X = (int)(Direction == DvDirectionHV.Horizon ? s : 0) + v.Margin.Left;
                    v.Y = (int)(Direction == DvDirectionHV.Vertical ? s : 0) + v.Margin.Top;
                    v.Width = (int)rtPage.Width - (v.Margin.Left + v.Margin.Right);
                    v.Height = (int)rtPage.Height - (v.Margin.Top + v.Margin.Bottom);
                 
                    s += Convert.ToInt32(Direction == DvDirectionHV.Horizon ? rtPage.Width : rtPage.Height);
                }
            });
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect> act)
        {
            var GP = DvTheme.GP;
            var NH = UsePageButton ? 20 : 0;

            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);
            var rtPrev = new SKRect();
            var rtNext = new SKRect();
            var rtPage = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, rtContent.Height - NH - (UsePageButton? GP : 0));
            var rtNavi = Util.FromRect(rtContent.Left, rtContent.Bottom - NH, rtContent.Width, NH);

            if (UseMoveButton)
            {
                var wh = Math.Min(50, Math.Min(rtContent.Width, rtContent.Height) / 10);

                if (Direction == DvDirectionHV.Horizon)
                {
                    rtPrev = Util.MakeRectangleAlign(Util.FromRect(rtContent.Left, rtPage.Top, wh, rtPage.Height), new SKSize(wh, wh), DvContentAlignment.MiddleCenter);
                    rtNext = Util.MakeRectangleAlign(Util.FromRect(rtContent.Right - wh, rtPage.Top, wh, rtPage.Height), new SKSize(wh, wh), DvContentAlignment.MiddleCenter);

                    rtPage.Left = rtPrev.Right + GP; rtPage.Right = rtNext.Left - GP;
                    rtNavi.Left = rtPrev.Right + GP; rtNavi.Right = rtNext.Left - GP;
                }
                else if (Direction == DvDirectionHV.Vertical)
                {
                    rtPrev = Util.MakeRectangleAlign(Util.FromRect(rtPage.Left, rtContent.Top, rtPage.Width, wh), new SKSize(wh, wh), DvContentAlignment.MiddleCenter);
                    rtNext = Util.MakeRectangleAlign(Util.FromRect(rtPage.Left, rtContent.Bottom - wh, rtPage.Width, wh), new SKSize(wh, wh), DvContentAlignment.MiddleCenter);

                    rtPage.Top = rtPrev.Bottom + GP; rtNavi.Bottom = rtNext.Top - GP;
                    rtPage.Bottom = rtNavi.Bottom - NH - GP; rtNavi.Top = rtNavi.Bottom - NH;
                }
            }

            act(rtContent, rtPage, rtNavi, rtPrev, rtNext);
        }
        #endregion
        #endregion
    }

    #region class : selindex
    class selindex
    {
        public DvSubPage prev;
        public DvSubPage now;
    }
    #endregion
}

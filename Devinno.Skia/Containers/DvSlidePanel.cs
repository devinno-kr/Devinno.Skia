using Devinno.Skia.Collections;
using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public override DvControlCollection Controls => SelectedPage?.Controls;
        public bool Animation => Design != null ? Design.Animation : false;

        public SKColor? ForeColor { get; set; } = null;
        public bool UseMoveButton { get; set; } = true;
        public bool UsePageButton { get; set; } = true;
        public DvDirectionHV Direction { get; set; } = DvDirectionHV.Horizon;

        internal DvControl InputControl { get; set; } = null;
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
        public event EventHandler SelectedPageChanged;
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
            Areas((rtContent, rtPage, rtNavis, rtPrev, rtNext) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                    {
                        #region var
                        var ForeColor = this.ForeColor ?? thm.ForeColor;
                        var Corner = thm.Corner;
                        #endregion

                        #region Layout
                        OnLayout();
                        #endregion
                        #region Button
                        if (UseMoveButton)
                        {
                            var rtp = rtPrev; if (bPrevDown) rtp.Offset(0, 1);
                            var rtn = rtNext; if (bNextDown) rtn.Offset(0, 1);

                            var cP = bPrevDown ? ForeColor.BrightnessTransmit(thm.DownBrightness) : ForeColor;
                            var cN = bNextDown ? ForeColor.BrightnessTransmit(thm.DownBrightness) : ForeColor;

                            var sP = Direction == DvDirectionHV.Horizon ? "fa-chevron-left" : "fa-chevron-up";
                            var sN = Direction == DvDirectionHV.Horizon ? "fa-chevron-right" : "fa-chevron-down";

                            var iP = rtPrev.Width * 0.9F;
                            var iN = rtNext.Width * 0.9F;

                            thm.DrawText(Canvas, rtp, sP, iP, cP, DvContentAlignment.MiddleCenter);
                            thm.DrawText(Canvas, rtn, sN, iN, cN, DvContentAlignment.MiddleCenter);
                        }
                        #endregion
                        #region Navi
                        if (UsePageButton)
                        {
                            int i = 0;
                            foreach (var page in Pages.Values)
                            {
                                if (rtNavis.ContainsKey(page.Name))
                                {
                                    var v = rtNavis[page.Name];

                                    p.IsStroke = true;
                                    p.Color = ForeColor;
                                    p.StrokeWidth = 2;
                                    Canvas.DrawCircle(v.CenterPoint, v.Dist1, p);

                                    if (SelectedPage == page)
                                    {
                                        p.IsStroke = false;
                                        Canvas.DrawCircle(v.CenterPoint, v.Dist2, p);
                                    }
                                    i++;
                                }
                            }
                        }
                        #endregion
                        #region Page
                        if (SelectedPage != null)
                        {
                            var lspage = Pages.Values.Where(x => CollisionTool.Check(rtPage, x.Bounds)).ToList();
                            foreach (var page in lspage)
                            {
                                var sp = Canvas.Save();
                                Canvas.ClipRect(rtPage);
                                Canvas.Translate(page.Left, page.Top);

                                page._Draw(Canvas);

                                Canvas.RestoreToCount(sp);
                            }
                        }
                        #endregion
                    }
                }
            });

            //base.OnDraw(Canvas);
        }
        #endregion
        
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtPage, rtNavis, rtPrev, rtNext) =>
            {
                base.OnMouseDown(x - rtPage.Left, y - rtPage.Top);

                var ls = Pages.Values.ToList();
                #region Slide
                if (Animation)
                {
                    if (CollisionTool.Check(rtPage, x, y))
                    {
                        ani.Stop();

                        if (Design?.InputControl == null)
                        {
                            movePoint = downPoint = new SKPoint(x, y);
                            downTime = DateTime.Now;

                            Design?.Input(this);
                        }
                    }
                }
                #endregion
                #region Button
                if (UseMoveButton)
                {
                    if (CollisionTool.Check(rtPrev, x, y)) bPrevDown = true;
                    if (CollisionTool.Check(rtNext, x, y)) bNextDown = true;
                }
                #endregion
                #region Navi
                if (UsePageButton)
                {
                    int i = 0;
                    foreach (var page in Pages.Values)
                    {
                        if (rtNavis.ContainsKey(page.Name))
                        {
                            var v = rtNavis[page.Name];
                            var rt = v.Bounds; rt.Inflate(5, 5);

                            if (CollisionTool.CheckCircle(rt, new SKPoint(x, y)))
                            {
                                var gi = i - (nowSelPage == null ? 0 : ls.IndexOf(nowSelPage));
                                var wh = (Direction == DvDirectionHV.Horizon ? rtPage.Width : rtPage.Height) * gi;
                                ani.Start(DvDesign.ANI * 2F, $"go:{wh}");

                                SelectedPage = page;
                            }
                        }

                        i++;
                    }
                }
                #endregion
               
            });
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtPage, rtNavis, rtPrev, rtNext) =>
            {
                base.OnMouseUp(x - rtPage.Left, y - rtPage.Top);

                var ls = Pages.Values.ToList();
                #region Slide
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
                                var gi = ni - si;
                                var wh = (Direction == DvDirectionHV.Horizon ? rtPage.Width : rtPage.Height) * gi;

                                SelectedPage = ls[ni];

                                var r = 0F;
                                if (Direction == DvDirectionHV.Horizon) r = (movePoint.Value.X - downPoint.Value.X);
                                else if (Direction == DvDirectionHV.Vertical) r = (movePoint.Value.Y - downPoint.Value.Y);
                                ani.Start(DvDesign.ANI * 2F, $"go:{wh + r}");
                                #endregion
                            }
                            else
                            {
                                #region Back
                                var r = 0F;
                                if (Direction == DvDirectionHV.Horizon) r = (movePoint.Value.X - downPoint.Value.X);
                                else if (Direction == DvDirectionHV.Vertical) r = (movePoint.Value.Y - downPoint.Value.Y);
                                ani.Start(DvDesign.ANI, $"back:{r}");
                                #endregion
                            }
                        }
                        else
                        {
                            #region Back
                            var r = 0F;
                            if (Direction == DvDirectionHV.Horizon) r = (movePoint.Value.X - downPoint.Value.X);
                            else if (Direction == DvDirectionHV.Vertical) r = (movePoint.Value.Y - downPoint.Value.Y);
                            ani.Start(DvDesign.ANI, $"back:{r}");
                            #endregion
                        }
                    }
                }

                movePoint = downPoint = null;
                #endregion
                #region Button
                if (bPrevDown)
                {
                    bPrevDown = false;
                    if (CollisionTool.Check(rtPrev, x, y))
                    {
                        var si = SelectedPage != null ? ls.IndexOf(SelectedPage) : -1;
                        if (si - 1 >= 0)
                        {
                            SelectedPage = ls[si - 1];
                            ani.Start(DvDesign.ANI * 2F, $"go:{(Direction == DvDirectionHV.Horizon ? -rtPage.Width : -rtPage.Height)}");
                        }
                    }
                }
                else if (bNextDown)
                {
                    bNextDown = false;
                    if (CollisionTool.Check(rtNext, x, y))
                    {
                        var si = SelectedPage != null ? ls.IndexOf(SelectedPage) : -1;
                        if (si + 1 < Pages.Count)
                        {
                            SelectedPage = ls[si + 1];
                            ani.Start(DvDesign.ANI * 2F, $"go:{(Direction == DvDirectionHV.Horizon ? rtPage.Width : rtPage.Height)}");
                        }
                    }
                }
                #endregion
            });
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(float x, float y)
        {
            Areas((rtContent, rtPage, rtNavis, rtPrev, rtNext) =>
            {
                base.OnMouseMove(x - rtPage.Left, y - rtPage.Top);

                #region Slide
                if (Animation)
                {
                    if (downPoint.HasValue && MathTool.GetDistance(downPoint.Value, new SKPoint(x, y)) >= 10)
                    {
                        movePoint = new SKPoint(x, y);
                    }
                }
                #endregion
            });
        }
        #endregion
        #region OnMouseDoubleClick
        protected override void OnMouseDoubleClick(float x, float y)
        {
            Areas((rtContent, rtPage, rtNavis, rtPrev, rtNext) =>
            {
                base.OnMouseDoubleClick(x - rtPage.Left, y - rtPage.Top);
            });
        }
        #endregion

        #region OnLayout
        protected override void OnLayout()
        {
            Areas((rtContent, rtPage, rtNavis, rtPrev, rtNext) =>
            {
                var si = Pages.Values.ToList().IndexOf(nowSelPage);
                if (si != -1)
                {
                    float sx = 0F, sy = 0F;
                    float ox = 0F, oy = 0F;
                    float mx = 0F, my = 0F;
                    float swh = 0F;

                    var aa = AnimationAccel.DCL;
                    if (Direction == DvDirectionHV.Horizon)
                    {
                        sx = rtPage.Left + -(si * rtPage.Width);
                        if (downPoint.HasValue && movePoint.HasValue) mx = movePoint.Value.X - downPoint.Value.X;
                    }
                    else if (Direction == DvDirectionHV.Vertical)
                    {
                        sy = rtPage.Top + -(si * rtPage.Height);
                        if (downPoint.HasValue && movePoint.HasValue) my = movePoint.Value.Y - downPoint.Value.Y;
                    }
                    
                    if (Animation && ani.IsPlaying)
                    {
                        if (float.TryParse(ani.Variable.Split(':').LastOrDefault(), out swh))
                        {
                            if (Direction == DvDirectionHV.Horizon) ox = ani.Value(aa, swh, 0);
                            else if (Direction == DvDirectionHV.Vertical) oy = ani.Value(aa, swh, 0);
                        }
                    }

                    foreach (var v in Pages.Values)
                    {
                        v.Left = sx + ox + mx + v.Margin.Left;
                        v.Top = sy + oy + my + v.Margin.Top;
                        v.Width = rtPage.Width - (v.Margin.Left + v.Margin.Right);
                        v.Height = rtPage.Height - (v.Margin.Top + v.Margin.Bottom);

                        if (Direction == DvDirectionHV.Horizon) sx += rtPage.Width;
                        else if (Direction == DvDirectionHV.Vertical) sy += rtPage.Height;
                    }
                }
            });
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, Dictionary<string, SlidePanelNavi>, SKRect, SKRect> act)
        {
            #region var
            var GP = 5F;
            var GP2 = 20F;
            var NH = UsePageButton ? 20 : 0;

            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);
            var rtPrev = new SKRect();
            var rtNext = new SKRect();
            var rtPage = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, rtContent.Height - NH - (UsePageButton ? GP : 0));
            var rtNavi = Util.FromRect(rtContent.Left, rtContent.Bottom - NH, rtContent.Width, NH);
            var rtNavis = new Dictionary<string, SlidePanelNavi>();
            #endregion

            #region Button
            if (UseMoveButton)
            {
                var wh = Math.Min(50, Math.Min(rtContent.Width, rtContent.Height) / 10);

                if (Direction == DvDirectionHV.Horizon)
                {
                    #region Horizon
                    rtPrev = Util.MakeRectangleAlign(Util.FromRect(rtContent.Left, rtPage.Top, wh, rtPage.Height), new SKSize(wh, wh), DvContentAlignment.MiddleCenter);
                    rtNext = Util.MakeRectangleAlign(Util.FromRect(rtContent.Right - wh, rtPage.Top, wh, rtPage.Height), new SKSize(wh, wh), DvContentAlignment.MiddleCenter);

                    rtPage.Left = rtPrev.Right + GP; rtPage.Right = rtNext.Left - GP;
                    rtNavi.Left = rtPrev.Right + GP; rtNavi.Right = rtNext.Left - GP;

                    #endregion
                }
                else if (Direction == DvDirectionHV.Vertical)
                {
                    #region Vertical
                    rtPrev = Util.MakeRectangleAlign(Util.FromRect(rtPage.Left, rtContent.Top, rtPage.Width, wh), new SKSize(wh, wh), DvContentAlignment.MiddleCenter);
                    rtNext = Util.MakeRectangleAlign(Util.FromRect(rtPage.Left, rtContent.Bottom - wh, rtPage.Width, wh), new SKSize(wh, wh), DvContentAlignment.MiddleCenter);

                    rtPage.Top = rtPrev.Bottom + GP; rtNavi.Bottom = rtNext.Top - GP;
                    rtPage.Bottom = rtNavi.Bottom - NH - GP; rtNavi.Top = rtNavi.Bottom - NH;
                    #endregion
                }
            }
            #endregion
            #region Navi
            if (UsePageButton)
            {
                var vwh = rtNavi.Height;
                var rtv = Util.MakeRectangleAlign(rtNavi, new SKSize((vwh * Pages.Count) + (GP2 * (Pages.Count - 1)), vwh), DvContentAlignment.MiddleCenter);
                var i = 0;
                foreach (var page in Pages.Values)
                {
                    var rt = Util.FromRect(rtv.Left + ((vwh + GP2) * i), rtv.Top, vwh, vwh);
                    rtNavis.Add(page.Name, new SlidePanelNavi
                    {
                        Bounds = rt,
                        CenterPoint = MathTool.CenterPoint(rt),
                        Dist1 = vwh / 2F * 0.75F,
                        Dist2 = vwh / 2F * 0.4F
                    });

                    i++;
                }
            }
            #endregion

            act(rtContent, rtPage, rtNavis, rtPrev, rtNext);
        }
        #endregion
        #endregion
    }

    #region class : SPNavi
    public class SlidePanelNavi
    {
        public SKRect Bounds { get; set; }
        public SKPoint CenterPoint { get; set; }
        public float Dist1 { get; set; }
        public float Dist2 { get; set; }
    }
    #endregion
}

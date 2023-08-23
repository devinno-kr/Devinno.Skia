using Devinno.Collections;
using Devinno.Skia.Collections;
using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvWheelPicker : DvControl
    {
        #region Const
        const float GapSize = 10;
        const float GapTime = 1;
        const double decelerationRate = 0.996;
        const int ThreadInterval = 10;
        #endregion

        #region Properties
        #region Color
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;
        #endregion
        #region Border
        public int BorderWidth { get; set; } = 1;
        #endregion
        #region Item
        public EventList2<DvTextIcon> Items { get; private set; } = new EventList2<DvTextIcon>();
        public int ItemHeight { get; set; } = 30;
        #endregion
        #region SelectedIndex
        private int nSelectedIndex = -1;
        public int SelectedIndex
        {
            get => nSelectedIndex;
            set
            {
                if (nSelectedIndex != value)
                {
                    nSelectedIndex = value;
                    SelectedIndexChanged?.Invoke(this, null);
                }
            }
        }
        #endregion
        #region TextIcon
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        public float IconSize { get; set; } = 12;
        public float IconGap { get; set; } = 5;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        #endregion
        #region Animation
        public bool Animation => Design != null ? Design.Animation : false;
        #endregion
        #region Scroll
        public bool TouchMode { get; set; } = true;
        private double TouchOffset => tcDown != null ? (tcDown.MovePoint.Y - tcDown.DownPoint.Y) : 0;
        private double ScrollPosition { get; set; }
        private double ScrollPositionWithOffset => ScrollPosition + TouchOffset;
        #endregion
        #endregion

        #region Member Varaible
        private TCDI tcDown = null;

        private double initPos;
        private double initVel;
        private double destPos;
        private double destTime;
        private double dCoeff = 1000.0 * Math.Log(decelerationRate);
        private double threshold = 0.1;
        private bool isScroll = false;

        private Animation ani = new Animation();
        #endregion

        #region Event
        public event EventHandler SelectedIndexChanged;
        #endregion

        #region Constructor
        public DvWheelPicker()
        {
            Items.Appended += (o, s) =>
            {
                foreach (var v in s.Items)
                {
                    v.FontName = FontName;
                    v.FontSize = FontSize;
                    v.FontStyle = FontStyle;
                    v.IconSize = IconSize;
                    v.IconAlignment = IconAlignment;
                    v.IconGap = IconGap;
                }
            };
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtView, rtCenter) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        #region Color
                        var ForeColor = this.ForeColor ?? thm.ForeColor;
                        var Corner = thm.Corner;
                        var BorderColor = this.BorderColor ?? thm.ForeColor.BrightnessTransmit(thm.BorderBrightness);
                        #endregion
                        #region Box
                        var rt = new SKRoundRect(rtCenter, Corner);

                        p.IsStroke = true;
                        p.StrokeWidth = BorderWidth;
                        p.Color = BorderColor;

                        rt.Inflate(-0.5F, -0.5F);

                        Canvas.DrawRoundRect(rt, p);
                        #endregion
                        #region Draw
                        if (Items.Count > 0)
                        {
                            var vcnt = Convert.ToInt32(Math.Ceiling((rtCenter.Top - rtView.Top) / (float)ItemHeight));
                            var si = SelectedIndex - vcnt - (ScrollPositionWithOffset < 0 ? Math.Ceiling(ScrollPositionWithOffset / ItemHeight) : Math.Floor(ScrollPositionWithOffset / ItemHeight));
                            var sy = rtCenter.Top - (vcnt * ItemHeight);
                            var vo = ScrollPositionWithOffset % ItemHeight;

                            var ao = 0F;
                            if (Animation && ani.IsPlaying)
                            {
                                if (ani.Variable.Substring(0, 1) == "E") ao = ani.Value(AnimationAccel.DCL, Convert.ToSingle(ani.Variable.Substring(1)), 0);
                                if (ani.Variable.Substring(0, 1) == "B") ao = ani.Value(AnimationAccel.DCL, Convert.ToSingle(ani.Variable.Substring(1)), 0);
                            }

                            while (sy < rtView.Bottom)
                            {
                                var rtv = Util.FromRect(rtView.Left, Convert.ToSingle(sy + vo + ao), rtView.Width, ItemHeight);
                                var a = Convert.ToByte(MathTool.Constrain(MathTool.Map(Math.Abs(rtv.MidY - rtCenter.MidY), 0, rtView.Height / 2F, 255, 0), 0, 255));
                                var c = ForeColor.WithAlpha(a);
                                var v = Items[Index(si)];

                                thm.DrawText(Canvas, rtv, v, c, DvContentAlignment.MiddleCenter);
                                sy += ItemHeight;
                                si++;
                            }
                        }
                        #endregion
                    }
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtView, rtCenter) =>
            {
                if (TouchMode)
                {
                    tcDown = new TCDI() { DownPoint = new SKPoint(x, y), MovePoint = new SKPoint(x, y), DownTime = DateTime.Now };
                    tcDown.List.Add(new TCMI() { Time = DateTime.Now, Point = new SKPoint(x, y) });
                    isScroll = false;
                    Design?.Input(this);

                    Thread.Sleep(15);
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(float x, float y)
        {
            Areas((rtContent, rtView, rtCenter) =>
            {
                if (TouchMode)
                {
                    if (tcDown != null && MathTool.GetDistance(tcDown.DownPoint, new SKPoint(x, y)) >= GapSize)
                    {
                        tcDown.MovePoint = new SKPoint(x, y);
                        tcDown.List.Add(new TCMI() { Time = DateTime.Now, Point = new SKPoint(x, y) });
                    }
                }
            });
            base.OnMouseMove(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtView, rtCenter) =>
            {
                if (Items.Count > 0)
                {
                    if (TouchMode)
                    {
                        if (tcDown != null)
                        {
                            #region var
                            var sp = tcDown.List.Where(x => (DateTime.Now - x.Time).TotalMilliseconds < Scroll.GestureTime).FirstOrDefault();
                            if (sp == null) sp = tcDown.List.Count > 0 ? tcDown.List.Last() : new TCMI() { Time = tcDown.DownTime, Point = tcDown.DownPoint };

                            initPos = TouchOffset;
                            initVel = ((y - sp.Point.Y)) / ((double)(DateTime.Now - sp.Time).TotalMilliseconds / 1000.0);
                            destPos = initPos - initVel / dCoeff;
                            destTime = Math.Log(-dCoeff * threshold / Math.Abs(initVel)) / dCoeff;
                            #endregion

                            if ((DateTime.Now - sp.Time).TotalSeconds <= 0.25 && (Math.Abs(initPos - destPos) > GapSize && destTime > GapTime))
                            {
                                #region Thread
                                if (!isScroll)
                                    ThreadPool.QueueUserWorkItem((o) =>
                                    {
                                        isScroll = true;
                                        var stime = DateTime.Now;
                                        var time = 0.0;

                                        while (isScroll && time < destTime * 1000 && Convert.ToInt64(ScrollPosition) != Convert.ToInt64(destPos))
                                        {
                                            time = (DateTime.Now - stime).TotalMilliseconds;
                                            var oldV = ScrollPosition;
                                            var newV = (initPos + (Math.Pow(decelerationRate, time) - 1) / dCoeff * initVel);
                                            ScrollPosition = newV;
                                            Thread.Sleep(10);
                                        }
                                        isScroll = false;

                                        var vv = ScrollPositionWithOffset;
                                        var ti = SelectedIndex - Math.Round(vv / ItemHeight);
                                        ScrollPosition = 0;
                                        SelectedIndex = Index(ti);
                                        if (Animation)
                                        {
                                            var off = (vv % ItemHeight);
                                            if (off > ItemHeight / 2F) off = ItemHeight - off;
                                            else if (off < -ItemHeight / 2F) off = off + ItemHeight;

                                            ani.Stop();
                                            ani.Start(Math.Abs(off * 5), "E" + off.ToString());
                                        }

                                    });
                                #endregion
                            }
                            else
                            {
                                if (Math.Abs(tcDown.DownPoint.Y - y) < 5 && (DateTime.Now - tcDown.DownTime).TotalMilliseconds < 300)
                                {
                                    #region Select
                                    if (CollisionTool.Check(rtView, x, y))
                                    {
                                        var oy = y - rtCenter.Top;
                                        var ni = Convert.ToInt32(oy < 0 ? Math.Floor(oy / ItemHeight) : Math.Floor(oy / ItemHeight));

                                        SelectedIndex = Index(SelectedIndex + ni);
                                        ani.Stop();
                                        ani.Start(Math.Abs(ItemHeight * ni) * 5, "B" + (ItemHeight * ni));
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region Set
                                    var vv = ScrollPositionWithOffset;
                                    var ti = SelectedIndex - Math.Round(vv / ItemHeight);
                                    ScrollPosition = 0;
                                    SelectedIndex = Index(ti);
                                    #endregion
                                }
                            }

                            tcDown = null;
                        }
                    }
                }
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Index
        int Index(double i)
        {
            var idx = Convert.ToInt32(i) % Items.Count;

            if (idx >= Items.Count) idx -= Items.Count;
            if (idx < 0) idx += Items.Count;

            return idx;
        }
        #endregion
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);
            var rtView = rtContent;
            var rtCenter = Util.MakeRectangle(rtView, new SKSize(rtContent.Width - BorderWidth, ItemHeight));

            act(rtContent, rtView, rtCenter);
        }
        #endregion
        #endregion
    }
}

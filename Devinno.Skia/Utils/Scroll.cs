using Devinno.Skia.Design;
using Devinno.Skia.Tools;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Utils
{
    public class Scroll
    {
        #region Const
        const float GapSize = 10;
        const float GapTime = 1;
        const double decelerationRate = 0.996;
        const int ThreadInterval = 10;
        internal const int GestureTime = 100;
        #endregion

        #region Static
        private static DvControl own = null;
        internal static void Set(DvControl c) { if (own == null) own = c; }
        internal static void Free(DvControl c) { if (own == c) own = null; }
        internal static bool Check(DvControl c) => own == c || own == null;
        #endregion

        #region Properties
        public static int SC_WH { get; set; } = 5;

        public bool IsScrolling => scDown != null;
        public bool IsTouchScrolling => tcDown != null;
        public bool IsTouchMoving => IsTouchStart;
        public bool TouchMode { get; set; } = false;
        public bool Cut { get; set; } = false;
        public ScrollDirection Direction { get; set; } = ScrollDirection.Vertical;

        public Func<double> GetScrollTotal { get; set; }
        public Func<double> GetScrollView { get; set; }
        public Func<double> GetScrollTick { get; set; }
        public Func<double> GetScrollScaleFactor { get; set; }
        public Func<bool> GetConstrainIgnore { get; set; }

        public bool ConstrainIgnore => GetConstrainIgnore != null ? GetConstrainIgnore() : false;
        public double ScrollTotal { get { return GetScrollTotal != null ? GetScrollTotal() : 0; } }
        public double ScrollView { get { return GetScrollView != null ? GetScrollView() : 0; } }
        public double ScrollTick { get { return GetScrollTick != null ? GetScrollTick() : 0; } }
        public double ScrollScaleFactor { get { return GetScrollScaleFactor != null ? GetScrollScaleFactor() : 1; } }
        private double _ScrollPosition = 0;
        public double ScrollPosition
        {
            get
            {
                if (ScrollView < ScrollTotal)
                {
                    if (!ConstrainIgnore) _ScrollPosition = (MathTool.Constrain(_ScrollPosition, 0, ScrollTotal - ScrollView));
                }
                else _ScrollPosition = 0;
                return _ScrollPosition;
            }
            set
            {
                if (ScrollView < ScrollTotal) _ScrollPosition = (MathTool.Constrain(value, 0, ScrollTotal - ScrollView));
                else _ScrollPosition = 0;
            }
        }

        public double TouchOffset
        {
            get
            {
                if (TouchMode)
                {
                    double ret;

                    if (Direction == ScrollDirection.Vertical)
                        ret = tcDown != null && ScrollView < ScrollTotal ? (Convert.ToDouble(tcDown.MovePoint.Y - tcDown.DownPoint.Y) * ScrollScaleFactor) : 0;
                    else
                        ret = tcDown != null && ScrollView < ScrollTotal ? (Convert.ToDouble(tcDown.MovePoint.X - tcDown.DownPoint.X) * ScrollScaleFactor) : 0;

                    return ret;
                }
                else return 0;
            }
        }

        public double ScrollPositionWithOffset
        {
            get
            {
                var ao = 0.0;
                if (ani.IsPlaying)
                {
                    var sp = ani.Variable.Split(':');
                    var avar = Convert.ToDouble(sp[1]);
                    if (sp[0] == "S") ao = ani.Value(AnimationAccel.DCL, -avar, 0F);
                    else if (sp[0] == "E") ao = ani.Value(AnimationAccel.DCL, (-avar - (ScrollTotal - ScrollView)), 0);
                }

                var ret = !Cut ?
                   -ScrollPosition + TouchOffset - ao :
                   (ScrollTotal > ScrollView ? (MathTool.Constrain((-ScrollPosition + TouchOffset), -(ScrollTotal - ScrollView), 0)) : 0);
                return ret;
            }
        }

        public double ScrollPositionWithOffsetR
        {
            get
            {
                var ao = 0.0;
                if (ani.IsPlaying)
                {
                    var sp = ani.Variable.Split(':');
                    var avar = Convert.ToDouble(sp[1]);
                    if (sp[0] == "S") ao = ani.Value(AnimationAccel.DCL, -avar, 0F);
                    else if (sp[0] == "E") ao = ani.Value(AnimationAccel.DCL, (-avar - (ScrollTotal - ScrollView)), 0);
                }

                var ret = !Cut ?
                    -(-ScrollPosition - TouchOffset) - ao :
                    (ScrollTotal > ScrollView ? (MathTool.Constrain(-(-ScrollPosition - TouchOffset), 0, (ScrollTotal - ScrollView))) : 0);
                return ret;
            }
        }


        public bool ScrollVisible => ScrollTotal > ScrollView;
        #endregion

        #region Member Variable
        SCDI scDown = null;
        TCDI tcDown = null;

        double initPos;
        double initVel;
        double destPos;
        double destTime;
        double dCoeff = 1000 * Math.Log(decelerationRate);
        double threshold = 0.1;

        bool IsTouchStart;
        Animation ani = new Animation();
        #endregion

        #region Event
        public event EventHandler ScrollChanged;
        public event EventHandler ScrollEnded;
        #endregion

        #region Constructor
        public Scroll()
        {

        }
        #endregion

        #region Method
        #region GetScrollCursorRect(rtScroll)
        public SKRect? GetScrollCursorRect(SKRect rtScroll)
        {
            var nsc = (SC_WH / 4F);
            rtScroll.Inflate(-nsc, -nsc);
            if (ScrollView < ScrollTotal)
            {
                var ao = 0.0;
                if (ani.IsPlaying)
                {
                    var sp = ani.Variable.Split(':');
                    var avar = Convert.ToDouble(sp[1]);
                    if (sp[0] == "S") ao = ani.Value(AnimationAccel.DCL, -avar, 0F);
                    else if (sp[0] == "E") ao = ani.Value(AnimationAccel.DCL, (-avar - (ScrollTotal - ScrollView)), 0);
                }

                if (Direction == ScrollDirection.Vertical)
                {
                    var h = (MathTool.Map(ScrollView, 0, ScrollTotal, 0, rtScroll.Height));
                    h = Math.Max(h, 30);

                    var y = 0D;
                    y = !Cut ? (MathTool.Map(                   ScrollPosition - TouchOffset + ao,                               0, ScrollTotal - ScrollView, rtScroll.Top, rtScroll.Bottom - h))
                             : (MathTool.Map(MathTool.Constrain(ScrollPosition - TouchOffset, 0, ScrollTotal - ScrollView), 0, ScrollTotal - ScrollView, rtScroll.Top, rtScroll.Bottom - h));

                    return Util.FromRect(rtScroll.Left, Convert.ToSingle(y), rtScroll.Width, Convert.ToSingle(h));
                }
                else
                {
                    var w = (MathTool.Map(ScrollView, 0, ScrollTotal, 0, rtScroll.Width));
                    w = Math.Max(w, 30);

                    var x = 0D;
                    x = !Cut ? (MathTool.Map(                   ScrollPosition - TouchOffset + ao,                               0, ScrollTotal - ScrollView, rtScroll.Left, rtScroll.Right - w))
                             : (MathTool.Map(MathTool.Constrain(ScrollPosition - TouchOffset, 0, ScrollTotal - ScrollView), 0, ScrollTotal - ScrollView, rtScroll.Left, rtScroll.Right - w));

                    return Util.FromRect(Convert.ToSingle(x), rtScroll.Top, Convert.ToSingle(w), rtScroll.Height);
                }
            }

            else return null;
        }
        #endregion

        #region MouseWheel
        public void MouseWheel(float x, float y, int delta)
        {
            ScrollPosition += ((delta / -120) * ScrollTick);
        }
        #endregion
        #region MouseDown
        public void MouseDown(float x, float y, SKRect rtScroll)
        {
            var rtcur = GetScrollCursorRect(rtScroll);
            if (!IsTouchStart && rtcur.HasValue && CollisionTool.Check(rtcur.Value, x, y)) scDown = new SCDI() { DownPoint = new SKPoint(x, y), CursorBounds = rtcur.Value };
        }
        #endregion
        #region MouseUp
        public void MouseUp(float x, float y)
        {
            if (scDown != null) scDown = null;
        }
        #endregion
        #region MouseMove
        public void MouseMove(float x, float y, SKRect rtScroll)
        {
            if (scDown != null)
            {
                if (Direction == ScrollDirection.Vertical)
                {
                    var h = (MathTool.Map(ScrollView, 0, ScrollTotal, 0, rtScroll.Height));
                    h = Math.Max(h, 30);
                    var v = (double)y - ((double)scDown.DownPoint.Y - (double)scDown.CursorBounds.Top);
                    ScrollPosition = (MathTool.Map(MathTool.Constrain(v, rtScroll.Top, rtScroll.Bottom - h), rtScroll.Top, rtScroll.Bottom - h, 0, ScrollTotal - ScrollView));
                }
                else if (Direction == ScrollDirection.Horizon)
                {
                    var w = (MathTool.Map(ScrollView, 0, ScrollTotal, 0, rtScroll.Width));
                    w = Math.Max(w, 30);
                    var v = (double)x - ((double)scDown.DownPoint.X - (double)scDown.CursorBounds.Left);
                    ScrollPosition = (MathTool.Map(MathTool.Constrain(v, rtScroll.Left, rtScroll.Right - w), rtScroll.Left, rtScroll.Right - w, 0, ScrollTotal - ScrollView));
                }
            }
        }
        #endregion

        #region TouchDown
        public void TouchDown(float x, float y)
        {
            if (TouchMode)
            {
                tcDown = new TCDI() { DownPoint = new SKPoint(x, y), MovePoint = new SKPoint(x, y), DownTime = DateTime.Now };
                tcDown.List.Add(new TCMI() { Time = DateTime.Now, Point = new SKPoint(x, y) });
                IsTouchStart = false;
                Thread.Sleep(15);
            }
        }
        #endregion
        #region TouchMove
        public void TouchMove(float x, float y)
        {
            if (TouchMode)
            {
                if (tcDown != null && MathTool.GetDistance(tcDown.DownPoint, new SKPoint(x, y)) >= GapSize)
                {
                    tcDown.MovePoint = new SKPoint(x, y);
                    tcDown.List.Add(new TCMI() { Time = DateTime.Now, Point = new SKPoint(x, y) });
                }
            }
        }
        #endregion
        #region TouchUp
        public void TouchUp(float x, float y)
        {
            if (TouchMode && tcDown != null)
            {
                if (ScrollView < ScrollTotal && MathTool.GetDistance(tcDown.DownPoint, new SKPoint(x, y)) >= GapSize)
                {
                    var sp = tcDown.List.Where(x => (DateTime.Now - x.Time).TotalMilliseconds < GestureTime).FirstOrDefault();
                    if (sp == null) sp = tcDown.List.Count > 0 ? tcDown.List.Last() : new TCMI() { Time = tcDown.DownTime, Point = tcDown.DownPoint };

                    if (Direction == ScrollDirection.Vertical)
                    {
                        ScrollPosition = (MathTool.Constrain(ScrollPosition - TouchOffset + ((sp.Point.Y - y) * ScrollScaleFactor), 0, (ScrollTotal - ScrollView)));
                        initPos = ScrollPosition;
                        initVel = ((sp.Point.Y - y) * ScrollScaleFactor) / ((double)(DateTime.Now - sp.Time).TotalMilliseconds / 1000.0);
                        destPos = MathTool.Constrain(initPos - initVel / dCoeff, 0, (ScrollTotal - ScrollView));
                        destTime = Math.Log(-dCoeff * threshold / Math.Abs(initVel)) / dCoeff;
                    }
                    else
                    {
                        ScrollPosition = (MathTool.Constrain(ScrollPosition - TouchOffset + ((sp.Point.X - x) * ScrollScaleFactor), 0, (ScrollTotal - ScrollView)));
                        initPos = ScrollPosition;
                        initVel = ((sp.Point.X - x) * ScrollScaleFactor) / ((double)(DateTime.Now - sp.Time).TotalMilliseconds / 1000.0);
                        destPos = MathTool.Constrain(initPos - initVel / dCoeff, 0, (ScrollTotal - ScrollView));
                        destTime = Math.Log(-dCoeff * threshold / Math.Abs(initVel)) / dCoeff;
                    }

                    if ((DateTime.Now - sp.Time).TotalSeconds <= 0.25 && (Math.Abs(initPos - destPos) > GapSize && destTime > GapTime))
                    {
                        ThreadPool.QueueUserWorkItem((o) =>
                        {
                            IsTouchStart = true;

                            var stime = DateTime.Now;
                            var time = 0.0;
                            var tot = (ScrollTotal - ScrollView);
                            while (IsTouchStart && time < destTime * 1000 && Convert.ToInt32(ScrollPosition / ScrollScaleFactor) != Convert.ToInt32(destPos / ScrollScaleFactor))
                            {
                                time = (DateTime.Now - stime).TotalMilliseconds;
                                var oldV = ScrollPosition;
                                var newV = (MathTool.Constrain((initPos + (Math.Pow(decelerationRate, time) - 1) / dCoeff * initVel), 0, tot));
                                if (oldV != newV) { ScrollPosition = (newV); try { ScrollChanged?.Invoke(this, null); } catch { } }

                                Thread.Sleep(ThreadInterval);
                            }

                            IsTouchStart = false;
                            try
                            {
                                ScrollChanged?.Invoke(this, null);
                                ScrollEnded?.Invoke(this, null);
                            }
                            catch { }

                        });
                    }
                    else
                    {
                        if (ScrollPosition - TouchOffset < 0)
                            ani.Start(100, $"S:{ScrollPositionWithOffset}");
                        else if (ScrollPosition - TouchOffset > ScrollTotal - ScrollView)
                            ani.Start(100, $"E:{ScrollPositionWithOffset}");
                    }

                }
                tcDown = null;
            }
        }
        #endregion
        #region TouchStop
        public void TouchStop()
        {
            IsTouchStart = false;
        }
        #endregion

        #region GetScrollCursorRectR(rtScroll)
        public SKRect? GetScrollCursorRectR(SKRect rtScroll)
        {
            var nsc = (SC_WH / 4F);
            rtScroll.Inflate(-nsc, -nsc);
            if (ScrollView < ScrollTotal)
            {
                var ao = 0.0;
                if (ani.IsPlaying)
                {
                    var sp = ani.Variable.Split(':');
                    var avar = Convert.ToDouble(sp[1]);
                    if (sp[0] == "S") ao = ani.Value(AnimationAccel.DCL, -avar, 0F);
                    else if (sp[0] == "E") ao = ani.Value(AnimationAccel.DCL, (-avar - (ScrollTotal - ScrollView)), 0);
                }

                if (Direction == ScrollDirection.Vertical)
                {
                    var h = (MathTool.Map(ScrollView, 0, ScrollTotal, 0, rtScroll.Height));
                    h = Math.Max(h, 30);

                    var y = 0D;
                    y = !Cut ? (MathTool.Map(                   ScrollPosition + TouchOffset - ao,                          0, ScrollTotal - ScrollView, rtScroll.Bottom - h, rtScroll.Top))
                             : (MathTool.Map(MathTool.Constrain(ScrollPosition + TouchOffset, 0, ScrollTotal - ScrollView), 0, ScrollTotal - ScrollView, rtScroll.Bottom - h, rtScroll.Top));
                    return Util.FromRect(rtScroll.Left, Convert.ToSingle(y), rtScroll.Width, Convert.ToSingle(h));
                }
                else
                {
                    var w = (MathTool.Map(ScrollView, 0, ScrollTotal, 0, rtScroll.Width));
                    w = Math.Max(w, 30);

                    var x = 0D;
                    x = !Cut ? (MathTool.Map(                   ScrollPosition + TouchOffset - ao,                          0, ScrollTotal - ScrollView, rtScroll.Right - w, rtScroll.Left))
                             : (MathTool.Map(MathTool.Constrain(ScrollPosition + TouchOffset, 0, ScrollTotal - ScrollView), 0, ScrollTotal - ScrollView, rtScroll.Right - w, rtScroll.Left));
                    return Util.FromRect(Convert.ToSingle(x), rtScroll.Top, Convert.ToSingle(w), rtScroll.Height);
                }
            }

            else return null;
        }
        #endregion

        #region MouseDownR
        public void MouseDownR(float x, float y, SKRect rtScroll)
        {
            var rtcur = GetScrollCursorRectR(rtScroll);
            if (!IsTouchStart && rtcur.HasValue && CollisionTool.Check(rtcur.Value, x, y)) scDown = new SCDI() { DownPoint = new SKPoint(x, y), CursorBounds = rtcur.Value };
        }
        #endregion
        #region MouseUpR
        public void MouseUpR(float x, float y)
        {
            if (scDown != null) scDown = null;
        }
        #endregion
        #region MouseMoveR
        public void MouseMoveR(float x, float y, SKRect rtScroll)
        {
            if (scDown != null)
            {
                if (Direction == ScrollDirection.Vertical)
                {
                    var h = (MathTool.Map(ScrollView, 0, ScrollTotal, 0, rtScroll.Height));
                    h = Math.Max(h, 30);
                    var v = (double)y - ((double)scDown.DownPoint.Y - (double)scDown.CursorBounds.Top);
                    ScrollPosition = MathTool.Map(MathTool.Constrain(v, rtScroll.Top, rtScroll.Bottom - h), rtScroll.Bottom - h, rtScroll.Top, 0, ScrollTotal - ScrollView);
                }
                else if (Direction == ScrollDirection.Horizon)
                {
                    var w = (MathTool.Map(ScrollView, 0, ScrollTotal, 0, rtScroll.Width));
                    w = Math.Max(w, 30);
                    var v = (double)x - ((double)scDown.DownPoint.X - (double)scDown.CursorBounds.Left);
                    ScrollPosition = MathTool.Map(MathTool.Constrain(v, rtScroll.Left, rtScroll.Right - w), rtScroll.Right - w, rtScroll.Left, 0, ScrollTotal - ScrollView);
                }
            }
        }
        #endregion

        #region TouchDownR
        public void TouchDownR(float x, float y)
        {
            if (TouchMode)
            {
                tcDown = new TCDI() { DownPoint = new SKPoint(x, y), MovePoint = new SKPoint(x, y), DownTime = DateTime.Now };
                tcDown.List.Add(new TCMI() { Time = DateTime.Now, Point = new SKPoint(x, y) });
                IsTouchStart = false;
                Thread.Sleep(15);
            }
        }
        #endregion
        #region TouchMoveR
        public void TouchMoveR(float x, float y)
        {
            if (TouchMode)
            {
                if (tcDown != null && MathTool.GetDistance(tcDown.DownPoint, new SKPoint(x, y)) >= GapSize)
                {
                    tcDown.MovePoint = new SKPoint(x, y);
                    tcDown.List.Add(new TCMI() { Time = DateTime.Now, Point = new SKPoint(x, y) });
                }
            }
        }
        #endregion
        #region TouchUpR
        public void TouchUpR(float x, float y)
        {
            if (TouchMode && tcDown != null)
            {
                if (ScrollView < ScrollTotal && MathTool.GetDistance(tcDown.DownPoint, new SKPoint(x, y)) >= GapSize)
                {
                    var sp = tcDown.List.Where(x => (DateTime.Now - x.Time).TotalMilliseconds < GestureTime).FirstOrDefault();
                    if (sp == null) sp = tcDown.List.Count > 0 ? tcDown.List.Last() : new TCMI() { Time = tcDown.DownTime, Point = tcDown.DownPoint };

                    if (Direction == ScrollDirection.Vertical)
                    {
                        ScrollPosition = (MathTool.Constrain(ScrollPosition + TouchOffset + ((y - sp.Point.Y) * ScrollScaleFactor), 0, (ScrollTotal - ScrollView)));
                        initPos = ScrollPosition;
                        initVel = ((y - sp.Point.Y) * ScrollScaleFactor) / ((double)(DateTime.Now - sp.Time).TotalMilliseconds / 1000.0);
                        destPos = MathTool.Constrain(initPos - initVel / dCoeff, 0, (ScrollTotal - ScrollView));
                        destTime = Math.Log(-dCoeff * threshold / Math.Abs(initVel)) / dCoeff;
                    }
                    else
                    {
                        ScrollPosition = (MathTool.Constrain(ScrollPosition + TouchOffset + ((x - sp.Point.X) * ScrollScaleFactor), 0, (ScrollTotal - ScrollView)));
                        initPos = ScrollPosition;
                        initVel = ((x - sp.Point.X) * ScrollScaleFactor) / ((double)(DateTime.Now - sp.Time).TotalMilliseconds / 1000.0);
                        destPos = MathTool.Constrain(initPos - initVel / dCoeff, 0, (ScrollTotal - ScrollView));
                        destTime = Math.Log(-dCoeff * threshold / Math.Abs(initVel)) / dCoeff;
                    }

                    if ((DateTime.Now - sp.Time).TotalSeconds <= 0.25 && (Math.Abs(initPos - destPos) > GapSize && destTime > GapTime))
                    {
                        ThreadPool.QueueUserWorkItem((o) =>
                        {
                            IsTouchStart = true;

                            var stime = DateTime.Now;
                            var time = 0.0;
                            var tot = (ScrollTotal - ScrollView);
                            while (IsTouchStart && time < destTime * 1000 && Convert.ToInt32(ScrollPosition / ScrollScaleFactor) != Convert.ToInt32(destPos / ScrollScaleFactor))
                            {
                                time = (DateTime.Now - stime).TotalMilliseconds;
                                var oldV = ScrollPosition;
                                var newV = (MathTool.Constrain((initPos + (Math.Pow(decelerationRate, time) - 1) / dCoeff * initVel), 0, tot));
                                if (oldV != newV) { ScrollPosition = (newV); try { ScrollChanged?.Invoke(this, null); } catch { } }

                                Thread.Sleep(ThreadInterval);
                            }

                            IsTouchStart = false;
                            try
                            {
                                ScrollChanged?.Invoke(this, null);
                                ScrollEnded?.Invoke(this, null);
                            }
                            catch { }

                        });
                    }
                    else
                    {
                        if (ScrollPosition + TouchOffset < 0)
                            ani.Start(100, $"S:{ScrollPositionWithOffset}");
                        else if (ScrollPosition + TouchOffset > ScrollTotal - ScrollView)
                            ani.Start(100, $"E:{ScrollPositionWithOffset}");
                    }
                }
                tcDown = null;
            }
        }
        #endregion

        #region Clear
        public void Clear()
        {
            scDown = null;
            tcDown = null;
            IsTouchStart = false;
        }
        #endregion
        #endregion
    }

    #region enum : ScrollDirection 
    public enum ScrollDirection { Horizon, Vertical }
    #endregion
    #region enum : ScrollMode 
    public enum ScrollMode { Horizon, Vertical, Both }
    #endregion
    #region class : SCDI
    internal class SCDI
    {
        internal SKPoint DownPoint { get; set; }
        internal SKRect CursorBounds { get; set; }
    }
    #endregion
    #region class : TCDI
    internal class TCDI
    {
        internal SKPoint DownPoint { get; set; }
        internal SKPoint MovePoint { get; set; }
        internal DateTime DownTime { get; set; }
        internal List<TCMI> List { get; } = new List<TCMI>();
    }
    #endregion
    #region class : TCMI
    class TCMI
    {
        public DateTime Time { get; set; }
        public SKPoint Point { get; set; }
    }
    #endregion
}

using Devinno.Skia.Tools;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Utils
{
    public class Animation
    {
        #region Properties
        public double TotalMillls { get; private set; }
        public double PlayMillis => tmStart.HasValue ? (DateTime.Now - tmStart.Value).TotalMilliseconds : 0;
        public bool IsPlaying => tmStart.HasValue && PlayMillis < TotalMillls;
        public string Variable { get; private set; }
        #endregion

        #region Member Variable
        DateTime? tmStart = null;
        #endregion

        #region Method
        #region Start
        public void Start(double totalMillis, string variable = null, Action act = null)
        {
            this.tmStart = DateTime.Now;
            this.TotalMillls = totalMillis;
            this.Variable = variable;

            if (act != null)
            {
                ThreadPool.QueueUserWorkItem((o) => { Thread.Sleep((int)totalMillis); act(); });
            }
        }
        #endregion
        #region Stop
        public void Stop()
        {
            this.tmStart = null;
            this.TotalMillls = 0;
        }
        #endregion
        #region Linear / Accel / Decel
        double Linear(double now, double goal) => now + (goal - now) * MathTool.Constrain(PlayMillis / TotalMillls, 0, 1);
        double Accel(double now, double goal) => goal - (goal - now) * Math.Sqrt(1.0 - (Math.Pow(MathTool.Constrain(PlayMillis / TotalMillls, 0.0, 1.0), 2.0) / Math.Pow(1.0, 2.0)));
        double Decel(double now, double goal) => now + (goal - now) * Math.Sqrt(1.0 - (Math.Pow(1.0 - MathTool.Constrain(PlayMillis / TotalMillls, 0.0, 1.0), 2.0) / Math.Pow(1.0, 2.0)));
        #endregion

        #region Value
        #region Value(int, int)
        public int Value(AnimationAccel velocity, int start, int end)
        {
            int ret = end;
            if(IsPlaying)
            {
                switch (velocity)
                {
                    case AnimationAccel.Linear: ret = Convert.ToInt32(Linear(start, end)); break;
                    case AnimationAccel.ACL: ret = Convert.ToInt32(Accel(start, end)); break;
                    case AnimationAccel.DCL: ret = Convert.ToInt32(Decel(start, end)); break;
                }
            }
            return ret;
        }
        #endregion
        #region Value(float, float)
        public float Value(AnimationAccel velocity, float start, float end)
        {
            float ret = end;
            if (IsPlaying)
            {
                switch (velocity)
                {
                    case AnimationAccel.Linear: ret = Convert.ToSingle(Linear(start, end)); break;
                    case AnimationAccel.ACL: ret = Convert.ToSingle(Accel(start, end)); break;
                    case AnimationAccel.DCL: ret = Convert.ToSingle(Decel(start, end)); break;
                }
            }
            return ret;
        }
        #endregion
        #region Value(float, float)
        public double Value(AnimationAccel velocity, double start, double end)
        {
            double ret = end;
            if (IsPlaying)
            {
                switch (velocity)
                {
                    case AnimationAccel.Linear: ret = (Linear(start, end)); break;
                    case AnimationAccel.ACL: ret = (Accel(start, end)); break;
                    case AnimationAccel.DCL: ret = (Decel(start, end)); break;
                }
            }
            return ret;
        }
        #endregion
        #region Value(Point, Rect)
        public SKRect Value(AnimationAccel velocity, SKPoint start, SKRect end)
        {
            SKRect ret = Util.FromRect(end);
            if (IsPlaying)
            {
                switch (velocity)
                {
                    case AnimationAccel.Linear:
                        ret.Left = Convert.ToSingle(Linear(start.X, end.Left));
                        ret.Top = Convert.ToSingle(Linear(start.Y, end.Top));
                        ret.Right = Convert.ToSingle(Linear(start.X, end.Right));
                        ret.Bottom = Convert.ToSingle(Linear(start.Y, end.Bottom));
                        break;
                    case AnimationAccel.ACL:
                        ret.Left = Convert.ToSingle(Accel(start.X, end.Left));
                        ret.Top = Convert.ToSingle(Accel(start.Y, end.Top));
                        ret.Right = Convert.ToSingle(Accel(start.X, end.Right));
                        ret.Bottom = Convert.ToSingle(Accel(start.Y, end.Bottom));
                        break;
                    case AnimationAccel.DCL:
                        ret.Left = Convert.ToSingle(Decel(start.X, end.Left));
                        ret.Top = Convert.ToSingle(Decel(start.Y, end.Top));
                        ret.Right = Convert.ToSingle(Decel(start.X, end.Right));
                        ret.Bottom = Convert.ToSingle(Decel(start.Y, end.Bottom));
                        break;
                }
            }
            return ret;
        }
        #endregion
        #region Value(Rect, Point)
        public SKRect Value(AnimationAccel velocity, SKRect start, SKPoint end)
        {
            SKRect ret = Util.FromRect(end.X, end.Y, 0, 0);
            if (IsPlaying)
            {
                switch (velocity)
                {
                    case AnimationAccel.Linear:
                        ret.Left = Convert.ToSingle(Linear(start.Left, end.X));
                        ret.Top = Convert.ToSingle(Linear(start.Top, end.Y));
                        ret.Right = Convert.ToSingle(Linear(start.Right, end.X));
                        ret.Bottom = Convert.ToSingle(Linear(start.Bottom, end.Y));
                        break;
                    case AnimationAccel.ACL:
                        ret.Left = Convert.ToSingle(Accel(start.Left, end.X));
                        ret.Top = Convert.ToSingle(Accel(start.Top, end.Y));
                        ret.Right = Convert.ToSingle(Accel(start.Right, end.X));
                        ret.Bottom = Convert.ToSingle(Accel(start.Bottom, end.Y));
                        break;
                    case AnimationAccel.DCL:
                        ret.Left = Convert.ToSingle(Decel(start.Left, end.X));
                        ret.Top = Convert.ToSingle(Decel(start.Top, end.Y));
                        ret.Right = Convert.ToSingle(Decel(start.Right, end.X));
                        ret.Bottom = Convert.ToSingle(Decel(start.Bottom, end.Y));
                        break;
                }
            }
            return ret;
        }
        #endregion
        #region Value(Rect, Rect)
        public SKRect Value(AnimationAccel velocity, SKRect start, SKRect end)
        {
            SKRect ret = Util.FromRect(end);
            if (IsPlaying)
            {
                switch (velocity)
                {
                    case AnimationAccel.Linear:
                        ret.Left = Convert.ToSingle(Linear(start.Left, end.Left));
                        ret.Top = Convert.ToSingle(Linear(start.Top, end.Top));
                        ret.Right = Convert.ToSingle(Linear(start.Right, end.Right));
                        ret.Bottom = Convert.ToSingle(Linear(start.Bottom, end.Bottom));
                        break;
                    case AnimationAccel.ACL:
                        ret.Left = Convert.ToSingle(Accel(start.Left, end.Left));
                        ret.Top = Convert.ToSingle(Accel(start.Top, end.Top));
                        ret.Right = Convert.ToSingle(Accel(start.Right, end.Right));
                        ret.Bottom = Convert.ToSingle(Accel(start.Bottom, end.Bottom));
                        break;
                    case AnimationAccel.DCL:
                        ret.Left = Convert.ToSingle(Decel(start.Left, end.Left));
                        ret.Top = Convert.ToSingle(Decel(start.Top, end.Top));
                        ret.Right = Convert.ToSingle(Decel(start.Right, end.Right));
                        ret.Bottom = Convert.ToSingle(Decel(start.Bottom, end.Bottom));
                        break;
                }
            }
            return ret;
        }
        #endregion
        #region Value(Color, Color)
        public SKColor Value(AnimationAccel velocity, SKColor start, SKColor end)
        {
            byte a = end.Alpha, r = end.Red, g = end.Green, b = end.Blue;
            if (IsPlaying)
            {
                switch (velocity)
                {
                    case AnimationAccel.Linear:
                        a = Convert.ToByte(Linear(start.Alpha, end.Alpha));
                        r = Convert.ToByte(Linear(start.Red, end.Red));
                        g = Convert.ToByte(Linear(start.Green, end.Green));
                        b = Convert.ToByte(Linear(start.Blue, end.Blue));
                        break;
                    case AnimationAccel.ACL:
                        a = Convert.ToByte(Accel(start.Alpha, end.Alpha));
                        r = Convert.ToByte(Accel(start.Red, end.Red));
                        g = Convert.ToByte(Accel(start.Green, end.Green));
                        b = Convert.ToByte(Accel(start.Blue, end.Blue));
                        break;
                    case AnimationAccel.DCL:
                        a = Convert.ToByte(Decel(start.Alpha, end.Alpha));
                        r = Convert.ToByte(Decel(start.Red, end.Red));
                        g = Convert.ToByte(Decel(start.Green, end.Green));
                        b = Convert.ToByte(Decel(start.Blue, end.Blue));
                        break;
                }
            }
            return Util.FromArgb(a, r, g, b);
        }
        #endregion
        #endregion

        #endregion
    }

    #region enum : AnimationAccel 
    public enum AnimationAccel { Linear, ACL, DCL }
    #endregion
    #region enum : AnimationType 
    public enum AnimationType { SlideV, SlideH, Drill, Fade, None }
    #endregion
}

using Devinno.Skia.Design;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Tools
{
    public class PathTool
    {
        #region Box
        public static SKPath Box(SKRect rect, DvRoundType round, float corner)
        {
            var path = new SKPath();

            var rt = new SKRoundRect(rect, corner);
            switch (round)
            {
                case DvRoundType.Rect: rt.SetNinePatch(rect, 0, 0, 0, 0); break;
                case DvRoundType.All: rt.SetNinePatch(rect, corner, corner, corner, corner); break;
                case DvRoundType.L: rt.SetNinePatch(rect, corner, corner, 0, corner); break;
                case DvRoundType.R: rt.SetNinePatch(rect, 0, corner, corner, corner); break;
                case DvRoundType.T: rt.SetNinePatch(rect, corner, corner, corner, 0); break;
                case DvRoundType.B: rt.SetNinePatch(rect, corner, 0, corner, corner); break;
                case DvRoundType.LT: rt.SetNinePatch(rect, corner, corner, 0, 0); break;
                case DvRoundType.RT: rt.SetNinePatch(rect, 0, corner, corner, 0); break;
                case DvRoundType.LB: rt.SetNinePatch(rect, corner, 0, 0, corner); break;
                case DvRoundType.RB: rt.SetNinePatch(rect, 0, 0, corner, corner); break;
                case DvRoundType.Ellipse: rt.SetOval(rect); break;
            }

            path.AddRoundRect(rt);

            return path;
        }
        #endregion
        #region Check
        public static SKPath Check(SKRect rect)
        {
            var INF = rect.Width / 4;
            var rtCheck = Util.FromRect(rect.Left, rect.Top, rect.Width, rect.Height);
            rtCheck.Inflate(-INF, -INF);

            var path = new SKPath();

            var points = new SKPoint[] { new SKPoint(rtCheck.Left, rtCheck.MidY),
                                         new SKPoint(rtCheck.MidX, rtCheck.Bottom),
                                         new SKPoint(rtCheck.Right, rtCheck.Top)};


            path.MoveTo(points[0]);
            path.LineTo(points[1]);
            path.LineTo(points[2]);

            return path;
        }
        #endregion
        #region Circle
        public static SKPath Circle(float x, float y , float r)
        {
            var path = new SKPath();

            path.AddCircle(x, y, r);

            return path;
        }
        #endregion
        #region BorderPanel
        public static SKPath BorderPanel(SKRect rtContent, SKRect rtTitle, bool drawTitle, float borderWidth, float corner)
        {
            var path = new SKPath();

            var rti = rtContent; rti.Inflate(-borderWidth, -borderWidth);
            var rt = new SKRoundRect(rtContent, corner);
            var rt2 = new SKRoundRect(rti, corner);
            var titleHeight = rtTitle.Height;

            path.AddRoundRect(rt);

            using (var path2 = new SKPath())
            {
                if (drawTitle)
                {
                    var TitleWidth = rtTitle.Width;

                    path2.AddArc(Util.FromRect(rti.Left, titleHeight, corner * 2, corner * 2), 180, 90);
                    path2.LineTo(TitleWidth, titleHeight);
                    path2.CubicTo(TitleWidth + 10, titleHeight,
                                 TitleWidth + 15, titleHeight / 2F,
                                 TitleWidth + 30, rti.Top);
                    path2.ArcTo(Util.FromRect(rti.Right - corner * 2, rti.Top, corner * 2, corner * 2), -90, 90, false);
                    path2.ArcTo(Util.FromRect(rti.Right - corner * 2, rti.Bottom - corner * 2, corner * 2, corner * 2), 0, 90, false);
                    path2.ArcTo(Util.FromRect(rti.Left, rti.Bottom - corner * 2, corner * 2, corner * 2), 90, 90, false);
                    path2.Close();
                }
                else
                {
                    path2.AddRoundRect(rt2);
                }

                path.AddPathReverse(path2);
            }

            return path;
        }
        #endregion
        #region SliderCursor
        public static SKPath SliderCursor(SKRect rtCur, DvDirectionHV direction, float corner, out SKRect rt)
        {
            var path = new SKPath();

            if (direction == DvDirectionHV.Horizon)
            {
                #region var
                var ax = 5F;
                var ay = 7F;
                var r = corner * 2F;

                var bp = new SKPoint(rtCur.MidX, rtCur.Bottom);
                rt = Util.FromRect(rtCur.Left, rtCur.Top, rtCur.Width, rtCur.Height - ay);
                #endregion
                #region Path
                path.MoveTo(bp);
                path.LineTo(bp.X - ax, bp.Y - ay);
                path.ArcTo(Util.FromRect(rt.Left, rt.Bottom - r, r, r), 90, 90, false);
                path.ArcTo(Util.FromRect(rt.Left, rt.Top, r, r), 180, 90, false);
                path.ArcTo(Util.FromRect(rt.Right - r, rt.Top, r, r), 270, 90, false);
                path.ArcTo(Util.FromRect(rt.Right - r, rt.Bottom - r, r, r), 360, 90, false);
                path.LineTo(bp.X + ax, bp.Y - ay);
                path.Close();
                #endregion
            }
            else if (direction == DvDirectionHV.Vertical)
            {
                #region var
                var ax = 7F;
                var ay = 5F;
                var r = corner * 2F;

                var bp = new SKPoint(rtCur.Right, rtCur.MidY);
                rt = Util.FromRect(rtCur.Left, rtCur.Top, rtCur.Width - ax, rtCur.Height);
                #endregion
                #region Path
                path.MoveTo(bp);
                path.LineTo(bp.X - ax, bp.Y + ay);
                path.ArcTo(Util.FromRect(rt.Right - r, rt.Bottom - r, r, r), 360, 90, false);
                path.ArcTo(Util.FromRect(rt.Left, rt.Bottom - r, r, r), 90, 90, false);
                path.ArcTo(Util.FromRect(rt.Left, rt.Top, r, r), 180, 90, false);
                path.ArcTo(Util.FromRect(rt.Right - r, rt.Top, r, r), 270, 90, false);
                path.LineTo(bp.X - ax, bp.Y - ay);
                path.Close();
                #endregion
            }
            else rt = rtCur;

            return path;
        }
        #endregion
        #region RangeSliderCursorStart
        public static SKPath RangeSliderCursorStart(SKRect rtCurStart, DvDirectionHV direction, bool reverse, float corner, out SKRect rt)
        {
            var path = new SKPath();


            if (direction == DvDirectionHV.Horizon)
            {
                #region var
                var ax = 10F;
                var ay = 7F;
                var r = corner * 2F;
                #endregion

                var bp = new SKPoint(rtCurStart.Right, rtCurStart.Bottom);
                rt = Util.FromRect(rtCurStart.Left, rtCurStart.Top, rtCurStart.Width, rtCurStart.Height - ay);
                if (reverse) bp = new SKPoint(rtCurStart.Left, rtCurStart.Bottom);

                #region Path
                if (reverse)
                {
                    path.MoveTo(bp);
                    path.ArcTo(Util.FromRect(rt.Left, rt.Top, r, r), 180, 90, false);
                    path.ArcTo(Util.FromRect(rt.Right - r, rt.Top, r, r), 270, 90, false);
                    path.ArcTo(Util.FromRect(rt.Right - r, rt.Bottom - r, r, r), 360, 90, false);
                    path.LineTo(bp.X + ax, bp.Y - ay);
                    path.Close();
                }
                else
                {
                    path.MoveTo(bp);
                    path.LineTo(bp.X - ax, bp.Y - ay);
                    path.ArcTo(Util.FromRect(rt.Left, rt.Bottom - r, r, r), 90, 90, false);
                    path.ArcTo(Util.FromRect(rt.Left, rt.Top, r, r), 180, 90, false);
                    path.ArcTo(Util.FromRect(rt.Right - r, rt.Top, r, r), 270, 90, false);
                    path.Close();
                }
                #endregion
            }
            else if (direction == DvDirectionHV.Vertical)
            {
                #region var
                var ax = 7F;
                var ay = 10F;
                var r = corner * 2F;
                #endregion

                var bp = new SKPoint(rtCurStart.Right, rtCurStart.Top);
                rt = Util.FromRect(rtCurStart.Left, rtCurStart.Top, rtCurStart.Width - ax, rtCurStart.Height);
                if (reverse) bp = new SKPoint(rtCurStart.Right, rtCurStart.Bottom);

                #region Path
                if (reverse)
                {
                    path.MoveTo(bp);
                    path.ArcTo(Util.FromRect(rt.Left, rt.Bottom - r, r, r), 90, 90, false);
                    path.ArcTo(Util.FromRect(rt.Left, rt.Top, r, r), 180, 90, false);
                    path.ArcTo(Util.FromRect(rt.Right - r, rt.Top, r, r), 270, 90, false);
                    path.LineTo(bp.X - ax, bp.Y - ay);
                    path.Close();
                }
                else
                {
                    path.MoveTo(bp);
                    path.LineTo(bp.X - ax, bp.Y + ay);
                    path.ArcTo(Util.FromRect(rt.Right - r, rt.Bottom - r, r, r), 360, 90, false);
                    path.ArcTo(Util.FromRect(rt.Left, rt.Bottom - r, r, r), 90, 90, false);
                    path.ArcTo(Util.FromRect(rt.Left, rt.Top, r, r), 180, 90, false);
                    path.Close();
                }
                #endregion
            }
            else rt = rtCurStart;

            return path;
        }
        #endregion
        #region RangeSliderCursorEnd
        public static SKPath RangeSliderCursorEnd(SKRect rtCurEnd, DvDirectionHV direction, bool reverse, float corner, out SKRect rt)
        {
            var path = new SKPath();

            if (direction == DvDirectionHV.Horizon)
            {
                #region var
                var ax = 10F;
                var ay = 7F;
                var r = corner * 2F;
                #endregion

                var bp = new SKPoint(rtCurEnd.Left, rtCurEnd.Bottom);
                rt = Util.FromRect(rtCurEnd.Left, rtCurEnd.Top, rtCurEnd.Width, rtCurEnd.Height - ay);
                if (reverse) bp = new SKPoint(rtCurEnd.Right, rtCurEnd.Bottom);

                #region Path
                if (reverse)
                {
                    path.MoveTo(bp);
                    path.LineTo(bp.X - ax, bp.Y - ay);
                    path.ArcTo(Util.FromRect(rt.Left, rt.Bottom - r, r, r), 90, 90, false);
                    path.ArcTo(Util.FromRect(rt.Left, rt.Top, r, r), 180, 90, false);
                    path.ArcTo(Util.FromRect(rt.Right - r, rt.Top, r, r), 270, 90, false);
                    path.Close();
                }
                else
                {
                    path.MoveTo(bp);
                    path.ArcTo(Util.FromRect(rt.Left, rt.Top, r, r), 180, 90, false);
                    path.ArcTo(Util.FromRect(rt.Right - r, rt.Top, r, r), 270, 90, false);
                    path.ArcTo(Util.FromRect(rt.Right - r, rt.Bottom - r, r, r), 360, 90, false);
                    path.LineTo(bp.X + ax, bp.Y - ay);
                    path.Close();
                }
                #endregion
            }
            else if (direction == DvDirectionHV.Vertical)
            {
                #region var
                var ax = 7F;
                var ay = 10F;
                var r = corner * 2F;
                #endregion

                var bp = new SKPoint(rtCurEnd.Right, rtCurEnd.Bottom);
                rt = Util.FromRect(rtCurEnd.Left, rtCurEnd.Top, rtCurEnd.Width - ax, rtCurEnd.Height);
                if (reverse) bp = new SKPoint(rtCurEnd.Right, rtCurEnd.Top);

                #region Path
                if (reverse)
                {
                    path.MoveTo(bp);
                    path.LineTo(bp.X - ax, bp.Y + ay);
                    path.ArcTo(Util.FromRect(rt.Right - r, rt.Bottom - r, r, r), 360, 90, false);
                    path.ArcTo(Util.FromRect(rt.Left, rt.Bottom - r, r, r), 90, 90, false);
                    path.ArcTo(Util.FromRect(rt.Left, rt.Top, r, r), 180, 90, false);
                    path.Close();
                }
                else
                {
                    path.MoveTo(bp);
                    path.ArcTo(Util.FromRect(rt.Left, rt.Bottom - r, r, r), 90, 90, false);
                    path.ArcTo(Util.FromRect(rt.Left, rt.Top, r, r), 180, 90, false);
                    path.ArcTo(Util.FromRect(rt.Right - r, rt.Top, r, r), 270, 90, false);
                    path.LineTo(bp.X - ax, bp.Y - ay);
                    path.Close();
                }
                #endregion
            }
            else rt = rtCurEnd;

            return path;
        }
        #endregion
        #region Gauge
        public static SKPath Gauge(SKRect rtGauge, float startAngle, float sweepAngle, float barSize)
        {
            var path = new SKPath();

            #region var
            var cp = MathTool.CenterPoint(rtGauge);
            var rtGaugeIn = Util.FromRect(rtGauge); rtGaugeIn.Inflate(-barSize, -barSize);

            var rtOut = rtGauge;
            var rtIn = rtGaugeIn;

            var pl1_1 = MathTool.GetPointWithAngle(rtOut, startAngle);
            var pl1_2 = MathTool.GetPointWithAngle(rtIn, startAngle);
            var pl1_C = MathTool.CenterPoint(pl1_1, pl1_2);
            var pl2_1 = MathTool.GetPointWithAngle(rtOut, startAngle + sweepAngle);
            var pl2_2 = MathTool.GetPointWithAngle(rtIn, startAngle + sweepAngle);
            var pl2_C = MathTool.CenterPoint(pl2_1, pl2_2);
            #endregion

            #region Path
            path.ArcTo(MathTool.MakeRectangle(pl1_C, barSize), startAngle, -180, false);
            path.ArcTo(rtIn, startAngle, sweepAngle, false);
            path.ArcTo(MathTool.MakeRectangle(pl2_C, barSize), startAngle + sweepAngle + 180, -180, false);
            path.ArcTo(rtOut, startAngle + sweepAngle, -sweepAngle, false);
            path.Close();
            #endregion

            return path;
        }
        #endregion
        #region MeterNeedle
        public static SKPath MeterNeedle(SKRect rtContent, SKRect rtGauge, SKRect rtCircleIn, 
            double value, double minimum, double maximum, float startAngle, float sweepAngle, 
            float remarkFontSize, float gT, float gL, float gS)
        {
            var path = new SKPath();

            #region var
            var rwh = rtCircleIn.Width / 2F;
            var distN = rwh + gL;
            var cp = new SKPoint(rtContent.MidX, rtContent.MidY);
            #endregion

            #region Path
            var vang = Convert.ToSingle(MathTool.Map(MathTool.Constrain(value, minimum, maximum), minimum, maximum, 0, sweepAngle));
            var pt = MathTool.GetPointWithAngle(cp, vang + startAngle, distN);

            var rtS = MathTool.MakeRectangle(pt, 3);
            var rtL = MathTool.MakeRectangle(cp, MathTool.Constrain(distN / 5F, 10, 30));

            path.AddArc(rtL, vang + startAngle + 90, 180);
            path.ArcTo(rtS, vang + startAngle + 90 + 180, 180, false);
            path.Close();
            #endregion

            return path;
        }
        #endregion
        #region Knob
        public static SKPath Knob(SKRect rtContent, SKRect rtKnob)
        {
            var path = new SKPath();

            var cp = new SKPoint(rtContent.MidX, rtContent.MidY);
            path.AddCircle(cp.X, cp.Y, rtKnob.Width / 2F);

            return path;
        }
        #endregion
        #region CircleGraphItem
        public static SKPath CircleGraphItem(SKRect rtOut, SKRect rtIn, float startAngle, float sweepAngle)
        {
            var pth = new SKPath();

            pth.AddArc(rtOut, startAngle, sweepAngle);
            pth.ArcTo(rtIn, startAngle + sweepAngle, -sweepAngle, false);
            pth.Close();

            return pth;
        }
        #endregion

    }
}

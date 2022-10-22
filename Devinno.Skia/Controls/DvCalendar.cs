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
    public class DvCalendar : DvControl
    {
        #region Properties
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? DaysBoxColor { get; set; } = null;
        public SKColor? WeeklyBoxColor { get; set; } = null;
        public SKColor? MonthlyBoxColor { get; set; } = null;
        public SKColor? SelectColor { get; set; } = null;
        public bool MultiSelect { get; set; } = false;
        public bool NoneSelect { get; set; } = false;

        public int CurrentYear { get; private set; } = DateTime.Now.Year;
        public int CurrentMonth { get; private set; } = DateTime.Now.Month;
        public string CurrentMonthText => CurrentYear + "." + CurrentMonth;
        
        public List<DateTime> SelectedDays { get; } = new List<DateTime>();

        #region Text
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #endregion

        #region Member Variable
        bool bMonthPrev = false, bMonthNext = false;
        #endregion

        #region Event
        public event EventHandler SelectedDaysChanged;
        #endregion

        #region Constructor
        public DvCalendar()
        {
            Width = 300;
            Height = 200;
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            var ds = Design;
            var thm = Design?.Theme;
            if (ds != null && thm != null)
            {
                #region Color
                var ForeColor = this.ForeColor ?? thm.ForeColor;
                var DaysBoxColor = this.DaysBoxColor ?? thm.CalendarDaysColor;
                var WeeklyBoxColor = this.WeeklyBoxColor ?? thm.CalendarWeeksColor;
                var MonthlyBoxColor = this.MonthlyBoxColor ?? thm.CalendarMonthColor;
                var SelectColor = this.SelectColor ?? thm.CalendarSelectColor;
                var BorderColor = thm.GetBorderColor(WeeklyBoxColor, ParentContainer.GetBackColor());
                var Corner = thm.Corner;
                #endregion

                bounds((rtContent, rtMonthly, rtWeekly, rtDays, rtMonthPrev, rtMonthNext, rtMonthText, dicBoxes, dicWeeks) =>
                {
                    #region DayList
                    int Days = DateTime.DaysInMonth(CurrentYear, CurrentMonth);
                    DateTime dt = new DateTime(CurrentYear, CurrentMonth, 1);
                    int ndw = (int)dt.DayOfWeek;
                    DateTime[] d = new DateTime[42];
                    int startidx = ndw == 0 ? 7 : ndw;
                    int endidx = startidx + Days;
                    if (dt.Date.Year == 1 && dt.Date.Month == 1 && dt.Date.Day == 1) { }
                    else dt -= new TimeSpan(startidx, 0, 0, 0);

                    for (int i = 0; i < 42; i++)
                    {
                        d[i] = dt;
                        dt += new TimeSpan(1, 0, 0, 0);
                    }
                    #endregion
                    #region Draw
                    #region BG
                    thm.DrawBox(Canvas, rtContent, DaysBoxColor, BorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutShadow);
                    thm.DrawBox(Canvas, rtMonthly, MonthlyBoxColor, BorderColor, RoundType.Round_T, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel);
                    thm.DrawBox(Canvas, rtWeekly, WeeklyBoxColor, BorderColor, RoundType.Rect, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InShadow);
                    #endregion
                    #region Month Text / Prev / Next
                    {
                        var cp = bMonthPrev ? ForeColor.BrightnessTransmit(thm.DownBrightness) : ForeColor;
                        var cn = bMonthNext ? ForeColor.BrightnessTransmit(thm.DownBrightness) : ForeColor;

                        if (bMonthPrev) { rtMonthPrev.Offset(0, 1); }
                        if (bMonthNext) { rtMonthNext.Offset(0, 1); }

                        Util.DrawText(Canvas, CurrentMonthText, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtMonthText);
                        Util.DrawIconFA(Canvas, "fas fa-chevron-left", 14, cp, rtMonthPrev);
                        Util.DrawIconFA(Canvas, "fas fa-chevron-right", 14, cn, rtMonthNext);
                    }
                    #endregion
                    #region Week
                    for (int ix = 0; ix < 7; ix++)
                    {
                        var rt = dicWeeks["rtWeek_" + ix];
                        string s = "";
                        var c = ForeColor;
                        switch ((DayOfWeek)ix)
                        {
                            case DayOfWeek.Sunday: s = "SUN"; c = SKColors.Red; break;
                            case DayOfWeek.Monday: s = "MON"; break;
                            case DayOfWeek.Tuesday: s = "TUE"; break;
                            case DayOfWeek.Wednesday: s = "WED"; break;
                            case DayOfWeek.Thursday: s = "THR"; break;
                            case DayOfWeek.Friday: s = "FRI"; break;
                            case DayOfWeek.Saturday: s = "SAT"; c = SKColors.DodgerBlue; break;
                        }

                        Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Bold, c, rt);
                    }
                    #endregion
                    #region Days
                    using (var p = new SKPaint() { IsAntialias = true })
                    {
                        p.StrokeWidth = 1;
                        p.Color = thm.GetInBevelColor(DaysBoxColor);
                        Canvas.DrawLine(rtDays.Left + 1, rtDays.Top + 0.5F, rtDays.Right - 1, rtDays.Top + 0.5F, p);

                        for (int iy = 0; iy < 6; iy++)
                        {
                            for (int ix = 0; ix < 7; ix++)
                            {
                                #region Bounds
                                var rt = dicBoxes["rtBox_" + iy + "_" + ix];
                                var idx = iy * 7 + ix;
                                var tm = d[idx];
                                #endregion
                                #region Text
                                if (!SelectedDays.Contains(tm))
                                {
                                    var ctext = ForeColor;
                                    var s = tm.Day.ToString();

                                    if (idx >= startidx && idx < endidx)
                                    {
                                        ctext = (ix == 0 ? SKColors.Red : (ix == 6 ? SKColors.DodgerBlue : ForeColor));
                                        Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, ctext, rt);

                                    }
                                    else
                                    {
                                        ctext = Util.FromArgb(120, SKColors.Black);
                                        Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, ctext, rt, DvContentAlignment.MiddleCenter, false);
                                    }
                                }
                                #endregion
                            }
                        }
                    }
                    #endregion
                    #region SelectDays
                    foreach (var v in SelectedDays)
                    {
                        var sidx = d.ToList().IndexOf(v.Date);
                        if (sidx >= 0)
                        {
                            #region Bounds
                            var iy = sidx / 7;
                            var ix = sidx - (iy * 7);

                            var rt = dicBoxes["rtBox_" + iy + "_" + ix];
                            var rtsh = rt; rtsh.Offset(0, 1);
                            var idx = iy * 7 + ix;
                            var tm = d[idx];
                            #endregion

                            var c = SelectColor;
                            #region Border
                            {
                                var rtv = rt; rtv.Inflate(-1, -1);

                                if (ix == 0 && iy == 5)
                                    thm.DrawBox(Canvas, rtv, DaysBoxColor, c, RoundType.Round_LB, BoxStyle.Border);
                                else if (ix == 6 && iy == 5)
                                    thm.DrawBox(Canvas, rtv, DaysBoxColor, c, RoundType.Round_RB, BoxStyle.Border);
                                else
                                    thm.DrawBox(Canvas, rtv, DaysBoxColor, c, RoundType.Rect, BoxStyle.Border);
                            }
                            #endregion
                            #region Text
                            {
                                var ctext = c;
                                var s = tm.Day.ToString();

                                Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, ctext, rt);
                            }
                            #endregion
                        }
                    }
                    #endregion
                    #endregion


                });

                base.OnDraw(Canvas);
            }
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            bounds((rtContent, rtMonthly, rtWeekly, rtDays, rtMonthPrev, rtMonthNext, rtMonthText, dicBoxes, dicWeeks) =>
            {
                #region DayList
                int Days = DateTime.DaysInMonth(CurrentYear, CurrentMonth);
                DateTime dt = new DateTime(CurrentYear, CurrentMonth, 1);
                int ndw = (int)dt.DayOfWeek;
                DateTime[] d = new DateTime[42];
                int startidx = ndw == 0 ? 7 : ndw;
                int endidx = startidx + Days;
                if (dt.Date.Year == 1 && dt.Date.Month == 1 && dt.Date.Day == 1) { }
                else dt -= new TimeSpan(startidx, 0, 0, 0);

                for (int i = 0; i < 42; i++)
                {
                    d[i] = dt;
                    dt += new TimeSpan(1, 0, 0, 0);
                }
                #endregion
                #region Month Prev / Next
                if (CollisionTool.Check(rtMonthPrev, x, y)) bMonthPrev = true;
                if (CollisionTool.Check(rtMonthNext, x, y)) bMonthNext = true;
                #endregion
                #region Days
                if (!NoneSelect)
                {
                    for (int iy = 0; iy < 6; iy++)
                    {
                        for (int ix = 0; ix < 7; ix++)
                        {
                            var rt = dicBoxes["rtBox_" + iy + "_" + ix];
                            var idx = iy * 7 + ix;
                            var tm = d[idx];

                            if (CollisionTool.Check(rt, x, y))
                            {
                                if (idx >= startidx && idx < endidx)
                                {
                                    if (MultiSelect)
                                    {
                                        if (SelectedDays.Contains(tm)) SelectedDays.Remove(tm);
                                        else SelectedDays.Add(tm);
                                        SelectedDaysChanged?.Invoke(this, null);
                                    }
                                    else
                                    {
                                        SelectedDays.Clear();
                                        SelectedDays.Add(tm);
                                        SelectedDaysChanged?.Invoke(this, null);
                                    }
                                }
                                else
                                {
                                    CurrentYear = tm.Year;
                                    CurrentMonth = tm.Month;
                                }
                            }
                        }
                    }
                }
                #endregion
            });
         
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            #region Month Prev
            if (bMonthPrev)
            {
                bMonthPrev = false;
                CurrentMonth--;
                if (CurrentMonth < 1)
                {
                    CurrentYear--;
                    CurrentMonth = 12;
                }
            }
            #endregion
            #region Month Next
            if (bMonthNext)
            {
                bMonthNext = false;
                CurrentMonth++;
                if (CurrentMonth > 12)
                {
                    CurrentYear++;
                    CurrentMonth = 1;
                }
            }
            #endregion
            base.OnMouseUp(x, y);
        }
        #endregion 
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, Dictionary<string, SKRect>, Dictionary<string, SKRect>> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);

            var nh = rtContent.Height / 8;
            var rtMonthly = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, nh + 1);
            var rtWeekly = Util.FromRect(rtContent.Left, rtContent.Top + nh, rtContent.Width, nh);
            var rtDays = Util.FromRect(rtContent.Left, rtWeekly.Bottom, rtContent.Width, rtContent.Height - (rtMonthly.Height + rtWeekly.Height));
            var rtMonthPrev = Util.FromRect(rtMonthly.Left, rtMonthly.Top, rtMonthly.Height, rtMonthly.Height);
            var rtMonthNext = Util.FromRect(rtMonthly.Right - rtMonthly.Height, rtMonthly.Top, rtMonthly.Height, rtMonthly.Height);
            var rtMonthText = Util.FromRect(rtMonthPrev.Right, rtMonthly.Top, rtMonthly.Width - (rtMonthPrev.Width + rtMonthNext.Width), rtMonthly.Height);

            var vw = (float)rtDays.Width / 7F;
            var vh = (float)rtDays.Height / 6F;

            var xs = new List<int>();
            var ys = new List<int>();

            for (int i = 0; i < 7; i++) xs.Add(Convert.ToInt32(rtDays.Left + Math.Round(vw * i)));
            for (int i = 0; i < 6; i++) ys.Add(Convert.ToInt32(rtDays.Top + Math.Round(vh * i)));

            var dicBoxes = new Dictionary<string, SKRect>();
            var dicWeeks = new Dictionary<string, SKRect>();
            for (int iy = 0; iy < 6; iy++)
            {
                int y = ys[iy];
                int h = Convert.ToInt32(iy == 5 ? rtDays.Bottom : ys[iy + 1]) - y;
                for (int ix = 0; ix < 7; ix++)
                {
                    int x = xs[ix];
                    int w = Convert.ToInt32(ix == 6 ? rtDays.Right : xs[ix + 1]) - x;
                    var rt = Util.FromRect(x, y, w, h);
                    dicBoxes.Add("rtBox_" + iy + "_" + ix, rt);

                    if (iy == 0) dicWeeks.Add("rtWeek_" + ix, Util.FromRect(x, rtWeekly.Top, w, rtWeekly.Height));
                }
            }

            act(rtContent, rtMonthly, rtWeekly, rtDays, rtMonthPrev, rtMonthNext, rtMonthText, dicBoxes, dicWeeks);
        }
        #endregion
        #endregion
    }
}

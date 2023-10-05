using Devinno.Skia.Design;
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
    public class DvCalendar : Design.DvControl
    {
        #region Properties
        #region Color
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? SelectColor { get; set; } = null;
        #endregion

        #region Select
        public bool MultiSelect { get; set; } = false;
        public bool NoneSelect { get; set; } = false;
        public List<DateTime> SelectedDays { get; } = new List<DateTime>();
        #endregion

        #region Current
        public int CurrentYear { get; private set; } = DateTime.Now.Year;
        public int CurrentMonth { get; private set; } = DateTime.Now.Month;
        private string CurrentMonthText => CurrentYear + "." + CurrentMonth;
        #endregion

        #region BackgroundDraw
        public bool BackgroundDraw { get; set; } = true;
        #endregion

        #region Text
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        #endregion
        #endregion

        #region Member Variable
        bool bMonthPrev = false, bMonthNext = false;
        #endregion

        #region Event
        public event EventHandler SelectedDaysChanged;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtMonthly, rtWeekly, rtDays, rtMonthPrev, rtMonthNext, rtMonthText, dicBoxes, dicWeeks) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var BoxColor = this.BoxColor ?? thm.ButtonColor;
                    var SelectColor = this.SelectColor ?? SKColors.Green;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();

                    thm.DrawCalendar(Canvas,
                        rtContent, rtMonthly, rtWeekly, rtDays, rtMonthPrev, rtMonthNext, rtMonthText, dicBoxes, dicWeeks,
                        BoxColor, SelectColor, ForeColor, BackColor,
                        CurrentMonthText, FontName, FontSize, FontStyle,
                        CurrentYear, CurrentMonth, SelectedDays,
                        BackgroundDraw,
                        bMonthPrev, bMonthNext);
                }
            });
         
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtMonthly, rtWeekly, rtDays, rtMonthPrev, rtMonthNext, rtMonthText, dicBoxes, dicWeeks) =>
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

                if (bMonthNext || bMonthPrev || !NoneSelect)
                {
                    Design?.Input(this);
                }
            
            });

            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
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
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, Dictionary<string, SKRect>, Dictionary<string, SKRect>> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);

            var nh = rtContent.Height / 8F;
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

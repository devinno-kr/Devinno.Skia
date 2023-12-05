using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devinno.Skia.Utils;
using Devinno.Skia.Controls;
using Devinno.Skia.Containers;
using Devinno.Skia.Design;

namespace SampleRPi.Pages
{
    partial class PageGraph
    {
        #region declare
        private DvTableLayoutPanel tbl;
        private DvButtons btnsMenus;
        private DvSwitchPanel swpnl;
        private DvSubPage tpGraphBarH;
        private DvBarGraph barH;
        private DvSubPage tpGraphBarV;
        private DvBarGraph barV;
        private DvSubPage tpGraphCircle;
        private DvCircleGraph circle;
        private DvSubPage tpGraphLine;
        private DvLineGraph line;
        private DvSubPage tpGraphTime;
        private DvTimeGraph time;
        private DvSubPage tpGraphTrend;
        private DvTrendGraph trend;
        #endregion

        public void InitializeComponent()
        {
            #region base
            UseMasterPage = true;
            BackgroundDraw = false;
            BackColor = null;
            BackgroundImage = null;
            AnimationType = null;
            Text = "Graph";
            IconString = "";
            #endregion

            #region new
            tbl = new DvTableLayoutPanel { Name = nameof(tbl) };
            btnsMenus = new DvButtons { Name = nameof(btnsMenus) };
            swpnl = new DvSwitchPanel { Name = nameof(swpnl) };
            tpGraphBarH = new DvSubPage { Name = nameof(tpGraphBarH) };
            barH = new DvBarGraph { Name = nameof(barH) };
            tpGraphBarV = new DvSubPage { Name = nameof(tpGraphBarV) };
            barV = new DvBarGraph { Name = nameof(barV) };
            tpGraphCircle = new DvSubPage { Name = nameof(tpGraphCircle) };
            circle = new DvCircleGraph { Name = nameof(circle) };
            tpGraphLine = new DvSubPage { Name = nameof(tpGraphLine) };
            line = new DvLineGraph { Name = nameof(line) };
            tpGraphTime = new DvSubPage { Name = nameof(tpGraphTime) };
            time = new DvTimeGraph { Name = nameof(time) };
            tpGraphTrend = new DvSubPage { Name = nameof(tpGraphTrend) };
            trend = new DvTrendGraph { Name = nameof(trend) };
            #endregion

            #region controls
            #region tbl
            tbl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 100F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 100F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 10F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 40F));
            tbl.Name = "tbl";
            tbl.X = 10F;
            tbl.Y = 60F;
            tbl.Width = 780F;
            tbl.Height = 410F;
            tbl.Visible = true;
            tbl.Enabled = true;
            tbl.Fill = true;
            tbl.Margin = new Padding(10, 60, 10, 10);
            #endregion
            #region btnsMenus
            btnsMenus.ButtonColor = null;
            btnsMenus.CheckedButtonColor = null;
            btnsMenus.ForeColor = null;
            btnsMenus.Buttons.Add(new ButtonInfo("BarH") { Size = new SizeInfo(DvSizeMode.Percent, 16.66F), IconString = "fa-chart-bar", IconSize = 12, IconAlignment = DvTextIconAlignment.LeftRight, IconGap = 5, Text = "Bar H", FontName = "NanumGothic", FontSize = 12, FontStyle = DvFontStyle.Normal, TextPadding = new Padding(0, 0, 0, 0) });
            btnsMenus.Buttons.Add(new ButtonInfo("BarV") { Size = new SizeInfo(DvSizeMode.Percent, 16.67F), IconString = "fa-chart-column", IconSize = 12, IconAlignment = DvTextIconAlignment.LeftRight, IconGap = 5, Text = "Bar V", FontName = "NanumGothic", FontSize = 12, FontStyle = DvFontStyle.Normal, TextPadding = new Padding(0, 0, 0, 0) });
            btnsMenus.Buttons.Add(new ButtonInfo("Circle") { Size = new SizeInfo(DvSizeMode.Percent, 16.67F), IconString = "fa-chart-pie", IconSize = 12, IconAlignment = DvTextIconAlignment.LeftRight, IconGap = 5, Text = "Circle", FontName = "NanumGothic", FontSize = 12, FontStyle = DvFontStyle.Normal, TextPadding = new Padding(0, 0, 0, 0) });
            btnsMenus.Buttons.Add(new ButtonInfo("Line") { Size = new SizeInfo(DvSizeMode.Percent, 16.67F), IconString = "fa-chart-line", IconSize = 12, IconAlignment = DvTextIconAlignment.LeftRight, IconGap = 5, Text = "Line", FontName = "NanumGothic", FontSize = 12, FontStyle = DvFontStyle.Normal, TextPadding = new Padding(0, 0, 0, 0) });
            btnsMenus.Buttons.Add(new ButtonInfo("Time") { Size = new SizeInfo(DvSizeMode.Percent, 16.67F), IconString = "far fa-clock", IconSize = 12, IconAlignment = DvTextIconAlignment.LeftRight, IconGap = 5, Text = "Time", FontName = "NanumGothic", FontSize = 12, FontStyle = DvFontStyle.Normal, TextPadding = new Padding(0, 0, 0, 0) });
            btnsMenus.Buttons.Add(new ButtonInfo("Trend") { Size = new SizeInfo(DvSizeMode.Percent, 16.66F), IconString = "fa-arrow-trend-up", IconSize = 12, IconAlignment = DvTextIconAlignment.LeftRight, IconGap = 5, Text = "Trend", FontName = "NanumGothic", FontSize = 12, FontStyle = DvFontStyle.Normal, TextPadding = new Padding(0, 0, 0, 0) });
            btnsMenus.Round = null;
            btnsMenus.Direction = Devinno.Skia.Design.DvDirectionHV.Horizon;
            btnsMenus.Gradient = true;
            btnsMenus.Clickable = true;
            btnsMenus.SelectorMode = true;
            btnsMenus.Name = "btnsMenus";
            btnsMenus.X = 150F;
            btnsMenus.Y = 373F;
            btnsMenus.Width = 480F;
            btnsMenus.Height = 34F;
            btnsMenus.Visible = true;
            btnsMenus.Enabled = true;
            btnsMenus.Fill = false;
            btnsMenus.Margin = new Padding(150, 3, 150, 3);
            #endregion
            #region swpnl
            swpnl.Pages.Add(tpGraphBarH);
            swpnl.Pages.Add(tpGraphBarV);
            swpnl.Pages.Add(tpGraphCircle);
            swpnl.Pages.Add(tpGraphLine);
            swpnl.Pages.Add(tpGraphTime);
            swpnl.Pages.Add(tpGraphTrend);
            swpnl.AnimationType = Devinno.Skia.Utils.AnimationType.SlideH;
            swpnl.Name = "swpnl";
            swpnl.X = 3F;
            swpnl.Y = 3F;
            swpnl.Width = 774F;
            swpnl.Height = 354F;
            swpnl.Visible = true;
            swpnl.Enabled = true;
            swpnl.Fill = false;
            swpnl.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region tpGraphBarH
            tpGraphBarH.Text = "BarH";
            tpGraphBarH.IconString = "";
            tpGraphBarH.Name = "tpGraphBarH";
            tpGraphBarH.X = 0F;
            tpGraphBarH.Y = 0F;
            tpGraphBarH.Width = 774F;
            tpGraphBarH.Height = 354F;
            tpGraphBarH.Visible = true;
            tpGraphBarH.Enabled = true;
            tpGraphBarH.Fill = false;
            tpGraphBarH.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region barH
            barH.GraphBackColor = null;
            barH.GridColor = null;
            barH.ForeColor = null;
            barH.Series.Add(new GraphSeries { Alias = "C++", Name = "Cpp", SeriesColor = Util.FromArgb(255, 180, 0, 0) });
            barH.Series.Add(new GraphSeries { Alias = "C#", Name = "CSharp", SeriesColor = Util.FromArgb(255, 0, 0, 180) });
            barH.Series.Add(new GraphSeries { Alias = "Java", Name = "Java", SeriesColor = Util.FromArgb(255, 0, 180, 0) });
            barH.Graduation = 10;
            barH.Minimum = 0;
            barH.Maximum = 100;
            barH.FormatString = "";
            barH.ValueDraw = true;
            barH.GraphMode = Devinno.Skia.Design.BarGraphMode.LIST;
            barH.Gradient = true;
            barH.BarSize = 25;
            barH.BarGap = 10;
            barH.Scrollable = true;
            barH.TouchMode = true;
            barH.FontName = "NanumGothic";
            barH.FontSize = 10F;
            barH.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            barH.Direction = Devinno.Skia.Design.DvDirectionHV.Horizon;
            barH.Name = "barH";
            barH.X = 3F;
            barH.Y = 3F;
            barH.Width = 768F;
            barH.Height = 348F;
            barH.Visible = true;
            barH.Enabled = true;
            barH.Fill = true;
            barH.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region tpGraphBarV
            tpGraphBarV.Text = "BarV";
            tpGraphBarV.IconString = "";
            tpGraphBarV.Name = "tpGraphBarV";
            tpGraphBarV.X = 0F;
            tpGraphBarV.Y = 0F;
            tpGraphBarV.Width = 774F;
            tpGraphBarV.Height = 354F;
            tpGraphBarV.Visible = true;
            tpGraphBarV.Enabled = true;
            tpGraphBarV.Fill = false;
            tpGraphBarV.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region barV
            barV.GraphBackColor = null;
            barV.GridColor = null;
            barV.ForeColor = null;
            barV.Series.Add(new GraphSeries { Alias = "C++", Name = "Cpp", SeriesColor = Util.FromArgb(255, 180, 0, 0) });
            barV.Series.Add(new GraphSeries { Alias = "C#", Name = "CSharp", SeriesColor = Util.FromArgb(255, 0, 0, 180) });
            barV.Series.Add(new GraphSeries { Alias = "Java", Name = "Java", SeriesColor = Util.FromArgb(255, 0, 180, 0) });
            barV.Graduation = 10;
            barV.Minimum = 0;
            barV.Maximum = 100;
            barV.FormatString = "";
            barV.ValueDraw = true;
            barV.GraphMode = Devinno.Skia.Design.BarGraphMode.LIST;
            barV.Gradient = true;
            barV.BarSize = 25;
            barV.BarGap = 10;
            barV.Scrollable = true;
            barV.TouchMode = true;
            barV.FontName = "NanumGothic";
            barV.FontSize = 10F;
            barV.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            barV.Direction = Devinno.Skia.Design.DvDirectionHV.Vertical;
            barV.Name = "barV";
            barV.X = 3F;
            barV.Y = 3F;
            barV.Width = 768F;
            barV.Height = 348F;
            barV.Visible = true;
            barV.Enabled = true;
            barV.Fill = true;
            barV.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region tpGraphCircle
            tpGraphCircle.Text = "Circle";
            tpGraphCircle.IconString = "";
            tpGraphCircle.Name = "tpGraphCircle";
            tpGraphCircle.X = 0F;
            tpGraphCircle.Y = 0F;
            tpGraphCircle.Width = 774F;
            tpGraphCircle.Height = 354F;
            tpGraphCircle.Visible = true;
            tpGraphCircle.Enabled = true;
            tpGraphCircle.Fill = false;
            tpGraphCircle.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region circle
            circle.ForeColor = null;
            circle.Series.Add(new GraphSeries { Alias = "C++", Name = "Cpp", SeriesColor = Util.FromArgb(255, 150, 0, 0) });
            circle.Series.Add(new GraphSeries { Alias = "C#", Name = "CSharp", SeriesColor = Util.FromArgb(255, 0, 0, 150) });
            circle.Series.Add(new GraphSeries { Alias = "Java", Name = "Java", SeriesColor = Util.FromArgb(255, 0, 150, 0) });
            circle.Gradient = true;
            circle.FormatString = "";
            circle.FontName = "NanumGothic";
            circle.FontSize = 12F;
            circle.NameFontSize = 18F;
            circle.ValueFontSize = 15F;
            circle.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            circle.SelectedIndex = -1;
            circle.Name = "circle";
            circle.X = 3F;
            circle.Y = 3F;
            circle.Width = 768F;
            circle.Height = 348F;
            circle.Visible = true;
            circle.Enabled = true;
            circle.Fill = true;
            circle.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region tpGraphLine
            tpGraphLine.Text = "Line";
            tpGraphLine.IconString = "";
            tpGraphLine.Name = "tpGraphLine";
            tpGraphLine.X = 0F;
            tpGraphLine.Y = 0F;
            tpGraphLine.Width = 774F;
            tpGraphLine.Height = 354F;
            tpGraphLine.Visible = true;
            tpGraphLine.Enabled = true;
            tpGraphLine.Fill = false;
            tpGraphLine.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region line
            line.GraphBackColor = null;
            line.GridColor = null;
            line.ForeColor = null;
            line.Series.Add(new GraphSeries { Alias = "C++", Name = "Cpp", SeriesColor = Util.FromArgb(255, 180, 0, 0) });
            line.Series.Add(new GraphSeries { Alias = "C#", Name = "CSharp", SeriesColor = Util.FromArgb(255, 0, 0, 180) });
            line.Series.Add(new GraphSeries { Alias = "Java", Name = "Java", SeriesColor = Util.FromArgb(255, 0, 180, 0) });
            line.Graduation = 10;
            line.Minimum = 0;
            line.Maximum = 100;
            line.FormatString = "";
            line.ValueDraw = true;
            line.PointDraw = true;
            line.PointWidth = 50;
            line.Scrollable = true;
            line.TouchMode = true;
            line.FontName = "NanumGothic";
            line.FontSize = 10F;
            line.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            line.Name = "line";
            line.X = 3F;
            line.Y = 3F;
            line.Width = 768F;
            line.Height = 348F;
            line.Visible = true;
            line.Enabled = true;
            line.Fill = true;
            line.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region tpGraphTime
            tpGraphTime.Text = "Time";
            tpGraphTime.IconString = "";
            tpGraphTime.Name = "tpGraphTime";
            tpGraphTime.X = 0F;
            tpGraphTime.Y = 0F;
            tpGraphTime.Width = 774F;
            tpGraphTime.Height = 354F;
            tpGraphTime.Visible = true;
            tpGraphTime.Enabled = true;
            tpGraphTime.Fill = false;
            tpGraphTime.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region time
            time.GraphBackColor = null;
            time.GridColor = null;
            time.ForeColor = null;
            time.Series.Add(new GraphSeries2 { Alias = "C++", Name = "Cpp", SeriesColor = Util.FromArgb(255, 180, 0, 0), Minimum = 0, Maximum = 100, Visible = true });
            time.Series.Add(new GraphSeries2 { Alias = "C#", Name = "CSharp", SeriesColor = Util.FromArgb(255, 0, 0, 180), Minimum = 0, Maximum = 100, Visible = true });
            time.Series.Add(new GraphSeries2 { Alias = "Java", Name = "Java", SeriesColor = Util.FromArgb(255, 0, 180, 0), Minimum = 0, Maximum = 100, Visible = true });
            time.YAxisGraduationCount = 10;
            time.XAxisGridDraw = false;
            time.YAxisGridDraw = true;
            time.ValueFormatString = "";
            time.TimeFormatString = "HH:mm:ss";
            time.Scrollable = true;
            time.TouchMode = true;
            time.FontName = "NanumGothic";
            time.FontSize = 10F;
            time.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            time.Name = "time";
            time.X = 3F;
            time.Y = 3F;
            time.Width = 768F;
            time.Height = 348F;
            time.Visible = true;
            time.Enabled = true;
            time.Fill = true;
            time.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region tpGraphTrend
            tpGraphTrend.Text = "Trend";
            tpGraphTrend.IconString = "";
            tpGraphTrend.Name = "tpGraphTrend";
            tpGraphTrend.X = 0F;
            tpGraphTrend.Y = 0F;
            tpGraphTrend.Width = 774F;
            tpGraphTrend.Height = 354F;
            tpGraphTrend.Visible = true;
            tpGraphTrend.Enabled = true;
            tpGraphTrend.Fill = false;
            tpGraphTrend.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region trend
            trend.GraphBackColor = null;
            trend.GridColor = null;
            trend.ForeColor = null;
            trend.Series.Add(new GraphSeries2 { Alias = "C++", Name = "Cpp", SeriesColor = Util.FromArgb(255, 180, 0, 0), Minimum = 0, Maximum = 100, Visible = true });
            trend.Series.Add(new GraphSeries2 { Alias = "C#", Name = "CSharp", SeriesColor = Util.FromArgb(255, 0, 0, 180), Minimum = 0, Maximum = 100, Visible = true });
            trend.Series.Add(new GraphSeries2 { Alias = "Java", Name = "Java", SeriesColor = Util.FromArgb(255, 0, 180, 0), Minimum = 0, Maximum = 100, Visible = true });
            trend.YAxisGraduationCount = 10;
            trend.XAxisGridDraw = false;
            trend.YAxisGridDraw = true;
            trend.ValueFormatString = "";
            trend.TimeFormatString = "HH:mm:ss";
            trend.Scrollable = true;
            trend.TouchMode = true;
            trend.FontName = "NanumGothic";
            trend.FontSize = 10F;
            trend.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            trend.Interval = 1000;
            trend.Pause = false;
            trend.Name = "trend";
            trend.X = 3F;
            trend.Y = 3F;
            trend.Width = 768F;
            trend.Height = 348F;
            trend.Visible = true;
            trend.Enabled = true;
            trend.Fill = true;
            trend.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #endregion

            #region add
            Controls.Add(tbl);
            tbl.Controls.Add(btnsMenus, 0, 2, 1, 1);
            tbl.Controls.Add(swpnl, 0, 0, 1, 1);
            tpGraphBarH.Controls.Add(barH);
            tpGraphBarV.Controls.Add(barV);
            tpGraphCircle.Controls.Add(circle);
            tpGraphLine.Controls.Add(line);
            tpGraphTime.Controls.Add(time);
            tpGraphTrend.Controls.Add(trend);
            #endregion
        }
    }
}

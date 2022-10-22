using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using Devinno.Timers;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Thread = System.Threading.Thread;
using ThreadStart = System.Threading.ThreadStart;

namespace SampleRPi.Pages
{
    public class PageTimeGraph : DvPage
    {
        #region Member Variable
        DvTimeGraph timeGraph;
        DvTrendGraph trendGraph;
        DvCheckBox chkPause;
        DvTableLayoutPanel tpnl;

        Random rnd = new Random();
        Data2 v = new Data2();

        DateTime prev = DateTime.Now;
        #endregion

        #region Constructor
        public PageTimeGraph()
        {
            UseMasterPage = true;

            #region TimeGraph
            timeGraph = new DvTimeGraph() { Name = nameof(timeGraph), X = 20, Y = 100, Width = 760, Height = 230, FontSize = 10, XAxisGridDraw = true, YAxisGridDraw = true };
            timeGraph.Series.Add(new GraphSeries2() { Name = "Cpp", Alias = "C++", SeriesColor = SKColors.Red, Minimum = 0, Maximum = 100 });
            timeGraph.Series.Add(new GraphSeries2() { Name = "Java", Alias = "Java", SeriesColor = SKColors.Green, Minimum = 0, Maximum = 100 });
            timeGraph.Series.Add(new GraphSeries2() { Name = "CSharp", Alias = "C#", SeriesColor = SKColors.DodgerBlue, Minimum = 0, Maximum = 100 });
            #endregion
            #region TrendGraph
            trendGraph = new DvTrendGraph() { Name = nameof(trendGraph), X = 20, Y = 350, Width = 760, Height = 230, FontSize = 10, XAxisGridDraw = true, YAxisGridDraw = true };
            trendGraph.Series.Add(new GraphSeries2() { Name = "Cpp", Alias = "C++", SeriesColor = SKColors.Red, Minimum = 0, Maximum = 100 });
            trendGraph.Series.Add(new GraphSeries2() { Name = "Java", Alias = "Java", SeriesColor = SKColors.Green, Minimum = 0, Maximum = 100 });
            trendGraph.Series.Add(new GraphSeries2() { Name = "CSharp", Alias = "C#", SeriesColor = SKColors.DodgerBlue, Minimum = 0, Maximum = 100 });
            trendGraph.MaximumXScale = TimeSpan.FromSeconds(5);
            trendGraph.XScale = TimeSpan.FromSeconds(1);
            trendGraph.XAxisGraduation = TimeSpan.FromSeconds(0.2);
            trendGraph.Interval = 10;
            #endregion
            #region Pause
            chkPause = new DvCheckBox() { Name = nameof(chkPause), X = 700, Y = MainWindow.H - 20 - 30, Width = 80, Height = 30, Text = "PAUSE" };
            #endregion
            #region tpnl
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Fill = true, Margin = MainWindow.BaseMargin };
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 50F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 50F });

            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 100F });
            #endregion

            #region Controls.Add
            tpnl.Controls.Add(timeGraph, 0, 0);
            tpnl.Controls.Add(trendGraph, 0, 1);
            Controls.Add(chkPause);
            Controls.Add(tpnl);
            #endregion

            #region Event
            chkPause.CheckedChanged += (o, s) => trendGraph.Pause = chkPause.Checked;
            #endregion
            
            trendGraph.Start<Data2>(v);
            TimeGraphSet();
        }
        #endregion

        #region Method
        #region TimeGraphSet
        void TimeGraphSet()
        {
            var ls1 = new List<Data2>();
            var java = 70D;
            var cpp = 50D;
            var csharp = 30D;
            for (var dt = DateTime.Now.Date; dt <= DateTime.Now.Date + TimeSpan.FromHours(5); dt += TimeSpan.FromSeconds(5 * 2))
            {
                cpp = MathTool.Constrain(cpp + (rnd.Next() % 2 == 0 ? 3 : -3), 0, 100);
                java = MathTool.Constrain(java + (rnd.Next() % 2 == 0 ? 3 : -3), 0, 100);
                csharp = MathTool.Constrain(csharp + (rnd.Next() % 2 == 0 ? 3 : -3), 0, 100);

                ls1.Add(new Data2() { Time = dt, Cpp = cpp, CSharp = csharp, Java = java });
            }

            timeGraph.SetDataSource<Data2>(ls1);
        }
        #endregion

        #region Tick
        public void Tick()
        {
            int n = 10;
            v.Cpp = MathTool.Constrain(v.Cpp + rnd.Next(-n, n), 0, 100);
            v.CSharp = MathTool.Constrain(v.CSharp + rnd.Next(-n, n), 0, 100);
            v.Java = MathTool.Constrain(v.Java + rnd.Next(-n, n), 0, 100);

            v.Cpp = MathTool.Map(Math.Sin(MathTool.Map((DateTime.Now - prev).TotalMilliseconds, 0, 500, 0, 360) * Math.PI / 180D), -1, 1, 0, 100);

            trendGraph.SetData<Data2>(v);
        }
        #endregion
        #endregion
    }

    #region class : Data2
    class Data2 : TimeGraphData
    {
        public override DateTime Time { get; set; }
        public double CSharp { get; set; }
        public double Cpp { get; set; }
        public double Java { get; set; }
    }
    #endregion
}

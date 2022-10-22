//using Devinno.Skia.Containers;
using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleRPi.Pages
{
    public class PageGraph : DvPage
    {
        #region Member Variable
        DvLineGraph lineGraph;
        DvBarGraphV vbarGraph;
        DvBarGraphH hbarGraph;
        DvCircleGraph circleGraph;

        DvSwitchPanel spnl;
        DvSubPage tpLine, tpVBar, tpHBar, tpCircle;
        DvButtons menus;
        DvTableLayoutPanel tpnl;

        Random rnd = new Random();
        #endregion

        #region Constructor
        public PageGraph()
        {
            UseMasterPage = true;

            #region Tabless
            spnl = new Devinno.Skia.Containers.DvSwitchPanel() { Name = nameof(spnl) };

            tpLine = new DvSubPage() { Name = nameof(tpLine) };
            tpVBar = new DvSubPage() { Name = nameof(tpVBar) };
            tpHBar = new DvSubPage() { Name = nameof(tpHBar) };
            tpCircle = new DvSubPage() { Name = nameof(tpCircle) };

            spnl.Pages.Add(tpLine);
            spnl.Pages.Add(tpVBar);
            spnl.Pages.Add(tpHBar);
            spnl.Pages.Add(tpCircle);
            #endregion
            #region LineGraph
            lineGraph = new DvLineGraph() { Name = nameof(lineGraph), Fill = true, GapWidth = 50, FontSize = 10 };
            lineGraph.Series.Add(new GraphSeries() { Name = "Cpp", Alias = "C++", SeriesColor = SKColors.Red });
            lineGraph.Series.Add(new GraphSeries() { Name = "Java", Alias = "Java", SeriesColor = SKColors.Green });
            lineGraph.Series.Add(new GraphSeries() { Name = "CSharp", Alias = "C#", SeriesColor = SKColors.DodgerBlue });
            #endregion
            #region BarGraphV
            vbarGraph = new DvBarGraphV() { Name = nameof(vbarGraph), Fill = true, BarSize = 20, BarGap = 10, FontSize = 10  };
            vbarGraph.Series.Add(new GraphSeries() { Name = "Cpp", Alias = "C++", SeriesColor = SKColors.Red });
            vbarGraph.Series.Add(new GraphSeries() { Name = "Java", Alias = "Java", SeriesColor = SKColors.Green });
            vbarGraph.Series.Add(new GraphSeries() { Name = "CSharp", Alias = "C#", SeriesColor = SKColors.DodgerBlue });
            #endregion
            #region BarGraphH
            hbarGraph = new DvBarGraphH() { Name = nameof(hbarGraph), Fill = true, BarSize = 20, BarGap = 10, FontSize = 10 };
            hbarGraph.Series.Add(new GraphSeries() { Name = "Cpp", Alias = "C++", SeriesColor = SKColors.Red });
            hbarGraph.Series.Add(new GraphSeries() { Name = "Java", Alias = "Java", SeriesColor = SKColors.Green });
            hbarGraph.Series.Add(new GraphSeries() { Name = "CSharp", Alias = "C#", SeriesColor = SKColors.DodgerBlue });
            #endregion
            #region CircleGraph
            circleGraph = new DvCircleGraph() { Name = nameof(circleGraph), Fill = true };
            circleGraph.Series.Add(new GraphSeries() { Name = "Cpp", Alias = "C++", SeriesColor = SKColors.Red });
            circleGraph.Series.Add(new GraphSeries() { Name = "Java", Alias = "Java", SeriesColor = SKColors.Green });
            circleGraph.Series.Add(new GraphSeries() { Name = "CSharp", Alias = "C#", SeriesColor = SKColors.DodgerBlue });
            #endregion
            #region Menus
            menus = new DvButtons() { Name = nameof(menus), SelectorMode = true };
            menus.Buttons.Add(new ButtonInfo(tpLine.Name) { Text = "Line", Checked = true, IconString = "fa-chart-line" });
            menus.Buttons.Add(new ButtonInfo(tpVBar.Name) { Text = "BarV", IconString = "fa-chart-bar" });
            menus.Buttons.Add(new ButtonInfo(tpHBar.Name) { Text = "BarH", IconString = "fa-align-left" });
            menus.Buttons.Add(new ButtonInfo(tpCircle.Name) { Text = "Circle", IconString = "fa-chart-pie" });
            #endregion
            #region tpnl
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Margin = MainWindow.BaseMargin, Fill = true };
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 100F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Pixel, Size = 10 });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Pixel, Size = 36 });

            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 50F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Pixel, Size = 300 });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 50F });
            #endregion
            #region Controls.Add
            tpLine.Controls.Add(lineGraph);
            tpVBar.Controls.Add(vbarGraph);
            tpHBar.Controls.Add(hbarGraph);
            tpCircle.Controls.Add(circleGraph);

            tpnl.Controls.Add(menus, 1, 2);
            tpnl.Controls.Add(spnl, 0, 0, 3, 1);

            Controls.Add(tpnl);
            #endregion

            spnl.SelectedPage = tpLine;
            
            menus.ButtonClick += (o, s) => spnl.SelectedPageName = s.Button.Name;

            GraphSet();
        }
        #endregion

        #region Method
        #region GraphSet
        void GraphSet()
        {
            var ls1 = new List<Data1>();
            var java = 70D;
            var cpp = 50D;
            var csharp = 30D;
            var vcs = new SKColor[] { SKColors.Red, SKColors.DarkOrange, SKColors.Goldenrod, SKColors.Green, SKColors.Teal, SKColors.SteelBlue, SKColors.DodgerBlue, SKColors.DeepSkyBlue,  SKColors.Violet, SKColors.Crimson, SKColors.Brown, SKColors.Maroon };
            for (int y = 2018; y <= 2021; y++)
                for (int m = 1; m <= 12; m++)
                {
                    int n = 5;
                    cpp = MathTool.Constrain(cpp + (rnd.Next() % 2 == 0 ? n : -n), 0, 100);
                    java = MathTool.Constrain(java + (rnd.Next() % 2 == 0 ? n : -n), 0, 100);
                    csharp = MathTool.Constrain(csharp + (rnd.Next() % 2 == 0 ? n : -n), 0, 100);

                    ls1.Add(new Data1() { Name = (y ) + "." + m, Cpp = cpp, CSharp = csharp, Java = java, Color = vcs[m - 1] });
                }

            lineGraph.SetDataSource<Data1>(ls1);
            vbarGraph.SetDataSource<Data1>(ls1);
            hbarGraph.SetDataSource<Data1>(ls1);
            circleGraph.SetDataSource<Data1>(ls1.GetRange(ls1.Count - 12, 12));
        }
        #endregion
        #endregion
    }

    #region class : Data1 
    class Data1 : GraphData
    {
        public override string Name { get; set; }
        public double CSharp { get; set; }
        public double Cpp { get; set; }
        public double Java { get; set; }
    }
    #endregion
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devinno.Skia.Design;
using Devinno.Skia.Tools;
using SkiaSharp;

namespace SampleRPi.Pages
{
    public partial class PageGraph : DvPage
    {
        #region Member Variable
        Random rnd = new Random();
        Data2 v = new Data2();
        #endregion

        #region Contructor
        public PageGraph()
        {
            InitializeComponent();

            #region Set
            swpnl.SelectedPage = tpGraphBarH;

            trend.MaximumXScale = TimeSpan.FromMinutes(60);
            trend.XScale = TimeSpan.FromSeconds(1);
            trend.XAxisGraduation = TimeSpan.FromSeconds(0.1);
            trend.Interval = 10;
            trend.XAxisGridDraw = true;
            #endregion

            #region Event
            btnsMenus.ButtonClick += (o, s) =>
            {
                switch (s.Button.Name)
                {
                    case "BarH": swpnl.SelectedPage = tpGraphBarH; break;
                    case "BarV": swpnl.SelectedPage = tpGraphBarV; break;
                    case "Circle": swpnl.SelectedPage = tpGraphCircle; break;
                    case "Line": swpnl.SelectedPage = tpGraphLine; break;
                    case "Time": swpnl.SelectedPage = tpGraphTime; break;
                    case "Trend": swpnl.SelectedPage = tpGraphTrend; break;
                }
            };
            #endregion
        }
        #endregion

        #region Override
        #region OnShow
        protected override void OnShow()
        {
            GraphSet();
            TimeGraphSet();

            if (!trend.IsStart) trend.Start<Data2>(v);

            base.OnShow();
        }
        #endregion
        #region OnUpdate
        protected override void OnUpdate()
        {
            var dic = btnsMenus.Buttons.ToDictionary(x => x.Name);

            dic["BarH"].Checked = swpnl.SelectedPage == tpGraphBarH;
            dic["BarV"].Checked = swpnl.SelectedPage == tpGraphBarV;
            dic["Circle"].Checked = swpnl.SelectedPage == tpGraphCircle;
            dic["Line"].Checked = swpnl.SelectedPage == tpGraphLine;
            dic["Time"].Checked = swpnl.SelectedPage == tpGraphTime;
            dic["Trend"].Checked = swpnl.SelectedPage == tpGraphTrend;

            TrendGraphSet();

            base.OnUpdate();
        }
        #endregion
        #endregion

        #region Method
        #region GraphSet
        void GraphSet()
        {
            var ls1 = new List<Data1>();
            var java = 50D;
            var cpp = 70D;
            var csharp = 60D;
            var vcs = new SKColor[] { SKColors.Red, SKColors.DarkOrange, SKColors.LimeGreen, SKColors.Green, SKColors.Teal, SKColors.SteelBlue, SKColors.DodgerBlue, SKColors.DeepSkyBlue, SKColors.Violet, SKColors.Crimson, SKColors.Brown, SKColors.Maroon };
            for (int y = 2020; y <= 2023; y++)
                for (int m = 1; m <= 12; m++)
                {
                    int n = 5;
                    java = MathTool.Constrain(java + (rnd.Next() % 2 == 0 ? n : -n), 0, 100);
                    cpp = MathTool.Constrain(cpp + (rnd.Next() % 2 == 0 ? n : -n), 0, 100);
                    csharp = MathTool.Constrain(csharp + (rnd.Next() % 2 == 0 ? n : -n), 0, 100);

                    ls1.Add(new Data1() { Name = (y) + "." + m, Cpp = cpp, CSharp = csharp, Java = java, Color = vcs[m - 1] });
                }

            line.SetDataSource<Data1>(ls1);
            barV.SetDataSource<Data1>(ls1);
            barH.SetDataSource<Data1>(ls1);
            circle.SetDataSource<Data1>(ls1.GetRange(ls1.Count - 12, 12));
            circle.SelectedIndex = 0;
        }
        #endregion
        #region TimeGraphSet
        void TimeGraphSet()
        {
            var ls1 = new List<Data2>();
            var java = 50D;
            var cpp = 70D;
            var csharp = 60D;
            for (var dt = DateTime.Now.Date + TimeSpan.FromHours(5); dt <= DateTime.Now.Date + TimeSpan.FromHours(10); dt += TimeSpan.FromSeconds(10))
            {
                int n = 5;
                java = MathTool.Constrain(java + (rnd.Next() % 2 == 0 ? n : -n), 0, 100);
                cpp = MathTool.Constrain(cpp + (rnd.Next() % 2 == 0 ? n : -n), 0, 100);
                csharp = MathTool.Constrain(csharp + (rnd.Next() % 2 == 0 ? n : -n), 0, 100);
                 
                ls1.Add(new Data2() { Time = dt, Cpp = cpp, CSharp = csharp, Java = java });
            }

            time.SetDataSource<Data2>(ls1);
        }
        #endregion
        #region TrendGraphSet
        public void TrendGraphSet()
        {
            if (trend.IsStart)
            {
                int n = 5;

                v.Java = MathTool.Constrain(v.Java + rnd.Next(-n, n + 1), 0, 100);
                v.Cpp = MathTool.Constrain(v.Cpp + rnd.Next(-n, n + 1), 0, 100);
                v.CSharp = MathTool.Constrain(v.CSharp + rnd.Next(-n, n + 1), 0, 100);
                 
                trend.SetData<Data2>(v);
            }
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
    #region class : Data2
    class Data2 : TimeGraphData
    {
        public long ID { get; set; }
        public override DateTime Time { get; set; }
        public double CSharp { get; set; }
        public double Cpp { get; set; }
        public double Java { get; set; }
    }
    #endregion
}

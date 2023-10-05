using System;
using System.Linq;
using Devinno.Skia.Design;

namespace SampleRPi.Pages
{
    public partial class PageGraph : DvPage
    {
        public PageGraph()
        {
            InitializeComponent();

            swpnl.SelectedPage = tpGraphBarH;
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
        }

        protected override void OnUpdate()
        {
            var dic = btnsMenus.Buttons.ToDictionary(x => x.Name);

            dic["BarH"].Checked = swpnl.SelectedPage == tpGraphBarH;
            dic["BarV"].Checked = swpnl.SelectedPage == tpGraphBarV;
            dic["Circle"].Checked = swpnl.SelectedPage == tpGraphCircle;
            dic["Line"].Checked = swpnl.SelectedPage == tpGraphLine;
            dic["Time"].Checked = swpnl.SelectedPage == tpGraphTime;
            dic["Trend"].Checked = swpnl.SelectedPage == tpGraphTrend;

            base.OnUpdate();
        }
    }
}

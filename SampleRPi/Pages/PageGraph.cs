using Devinno.Skia.Design;
using System.Linq;

namespace SampleRPi.Pages
{
    public partial class PageGraph : DvPage
    {
        public PageGraph()
        {
            InitializeComponent();

            swpnl.SelectedPage = tpGraphBarH;
            btnsMenus.Buttons.Where(x => x.Name == "BarH").FirstOrDefault().Checked = true;
            btnsMenus.SelectedChanged += (o, s) =>
            {
                switch(s.Button.Name)
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
    }
}

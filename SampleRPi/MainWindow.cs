using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.OpenTK;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SampleRPi.Pages;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SampleRPi
{
    public class MainWindow : DvViewWindow
    {
        #region Const
        public const int W = 800;
        public const int H = 480;
        #endregion

        #region Member Variable
        internal PageControl pageControl;
        internal PageInput pageInput;
        internal PageGauge pageGauge;
        internal PageGraph pageGraph;
        internal PageTimeGraph pageTimeGraph;
        internal PageContainer pageContainer;
        internal PageDialogs pageDialogs;
        internal PageSlide pageSlide;
        internal PageTable pageTable;
        internal PageAnimation pageAni;
        internal PageInformation pageInformation;
        internal TestWindow testwnd;
        private DvNavBar nav;
        #endregion

        #region Constructor
        public MainWindow() : base(W, H)
        {
            testwnd = new TestWindow() { Width = 400, Height = 300 };
            #region MasterPage
            Design.MasterPage.BackgroundDraw = true;
            Design.MasterPage.BackColor = Design.Theme.BackColor;

            nav = new DvNavBar()
            {
                Name = nameof(nav),
                Title = "DEVINNO",
                IconAlignment = DvTextIconAlignment.LeftRight,
                LogoImage = Util.FromBitmap(Path.Combine(AppTool.CurrentPath, "Images", "logo2.png")),
                DropDownWidth = 100,
                DropDownViewCount = 5
            };

            for (int i = 1; i <= 3; i++) nav.FoldingMenus.Add(new DvTextIcon() { Text = "테스트" + i });
            nav.FoldingMenus.Add(new DvTextIcon() { Text = "종료" });
            Design.MasterPage.Controls.Add(nav);

            nav.FoldingMenuClicked += (o, s) => {

                if (s.Item.Text == "종료")
                {
                    Close();
                }
                else if(s.Item.Text == "테스트1")
                {
                    testwnd.Show();
                }
            };
            #endregion

            #region Page
            pageControl = new PageControl { Name = nameof(pageControl), Text = "Control", IconString = "fa-cube" };
            pageInput = new PageInput { Name = nameof(pageInput), Text = "Input", IconString = "fa-pen-to-square" };
            pageGauge = new PageGauge { Name = nameof(pageGauge), Text = "Gauge", IconString = "fa-gauge-high" };
            pageGraph = new PageGraph { Name = nameof(pageGraph), Text = "Graph", IconString = "fa-chart-bar" };
            pageTimeGraph = new PageTimeGraph { Name = nameof(pageTimeGraph), Text = "TGraph", IconString = "fa-chart-line" };
            pageContainer = new PageContainer { Name = nameof(pageContainer), Text = "Container", IconString = "fa-layer-group" };
            pageDialogs = new PageDialogs { Name = nameof(pageDialogs), Text = "Dialog", IconString = "far fa-window-maximize" };
            pageSlide = new PageSlide { Name = nameof(pageSlide), Text = "Slide", IconString = "fa-hand-point-down" };
            pageTable = new PageTable { Name = nameof(pageTable), Text = "Table", IconString = "fa-table" };
            pageAni = new PageAnimation { Name = nameof(pageAni), Text = "Movie", IconString = "fa-film" };
            pageInformation = new PageInformation { Name = nameof(pageInformation), Text = "Information" };

            Design.Pages.Add(pageControl);
            Design.Pages.Add(pageInput);
            Design.Pages.Add(pageGauge);
            Design.Pages.Add(pageGraph);
            Design.Pages.Add(pageTimeGraph);
            Design.Pages.Add(pageContainer);
            Design.Pages.Add(pageSlide);
            Design.Pages.Add(pageDialogs);
            Design.Pages.Add(pageTable);
            Design.Pages.Add(pageAni);
            Design.Pages.Add(pageInformation);

            Design.SetPage(pageControl);

            //foreach (var vk in Design.Pages.Keys) Design.Pages[vk].IconString = null;
            #endregion

            #region Set
            nav.PageNames.AddRange(Design.Pages.Where(x => x.Key != "pageInformation").Select(x => x.Value.Name));
            Design.Animation = true;
            #endregion
        }
        #endregion


        
    }
}

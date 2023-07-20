using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Dialogs;
using Devinno.Skia.OpenTK;
using Devinno.Skia.Theme;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using Devinno.Timers;
using SampleRPi.Pages;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SampleRPi
{
    public class MainWindow : DvViewWindow
    {
        #region Const
        public const int W = 800;
        public const int H = 480;
        public const int GAP = 2;

        public static Padding BaseMargin = new Padding(20, 70, 20, 20); 
        #endregion

        #region Member Variable
        private DvNavBar navBar;

        internal PageControl pageControl;
        internal PageGauge pageGauge;
        internal PageGraph pageGraph;
        internal PageTimeGraph pageTimeGraph;
        internal PageContainer pageContainer;
        internal PageDialogs pageDialogs;
        internal PageSlide pageSlide;
        internal PageTable pageTable;
        internal PageAnimation pageAni;
        internal PageInformation pageInformation;

        private HiResTimer tmr;
        private DvMessageBox MessageBox;
        #endregion

        #region Constructor
        public MainWindow() : base(W, H)
        {
            Design.Animation = false;
            //Design.SetTheme(new WhiteTheme());

            #region Pages
            #region Page
            pageControl = new PageControl() { Name = nameof(pageControl), Text = "Control", IconString = "fa-cube" };
            pageGauge = new PageGauge() { Name = nameof(pageGauge), Text = "Gauge", IconString = "fa-tachometer-alt" };
            pageGraph = new PageGraph() { Name = nameof(pageGraph), Text = "Graph", IconString = "fa-chart-bar" };
            pageTimeGraph = new PageTimeGraph() { Name = nameof(pageTimeGraph), Text = "TGraph", IconString = "fa-chart-line" };
            pageContainer = new PageContainer() { Name = nameof(pageContainer), Text = "Container", IconString = "fa-layer-group" };
            pageDialogs = new PageDialogs() { Name = nameof(pageDialogs), Text = "Dialog", IconString = "far fa-window-maximize" };
            pageSlide = new PageSlide() { Name = nameof(pageSlide), Text = "Slide", IconString = "fa-hand-point-down" };
            pageTable = new PageTable() { Name = nameof(pageTable), Text = "Table", IconString = "fa-table" };
            pageAni = new PageAnimation() { Name = nameof(pageAni), Text = "Movie", IconString = "fa-film" };
            pageInformation = new PageInformation() { Name = nameof(pageInformation), Text = "Information" };

            Design.Pages.Add(pageControl);
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

            foreach (var vk in Design.Pages.Keys) Design.Pages[vk].IconString = null;
            #endregion

            #region MasterPage
            Design.MasterPage.BackgroundDraw = true;
            Design.MasterPage.BackColor = Design.Theme.BackColor;
            navBar = new DvNavBar()
            {
                Name = "btns",
                Fill = true,
                Margin = new Padding(0, 0, 0, H - 50),
                Title = "DEVINNO",
                IconAlignment = DvTextIconAlignment.TopBottom,
                LogoImage = Util.FromBitmap(Path.Combine(PathTool.CurrentPath, "Images", "logo2.png")),
                DropDownWidth = 100,
                DropDownViewCount = 5
            };

            for (int i = 1; i <= 3; i++) navBar.FoldingMenus.Add(new TextIconItem() { Text = "테스트" + i });
            navBar.FoldingMenus.Add(new TextIconItem() { Text = "종료" });

            navBar.PageNames.AddRange(Design.Pages.Where(x => pageInformation != x.Value).Select(x => x.Value.Name));
            Design.MasterPage.Controls.Add(navBar);
            #endregion
            #endregion

            #region Dialog
            MessageBox = new DvMessageBox() { Name = nameof(MessageBox) };
            #endregion

            #region Event
            navBar.FoldingMenuClicked += (o, s) => {

                if (s.Item.Text == "종료")
                {
                    tmr.Stop();
                    Close();
                }
                else if(s.Item.Text == "테스트1") MessageBox.ShowMessageBoxOk("폴딩 메뉴", s.Item.Text,(result)=> { });
            };
            #endregion

            #region Timer
            tmr = new HiResTimer(5);
            tmr.Elapsed += (o, s) =>
            {
                if (Design.CurrentPage == pageGauge) pageGauge.Tick();
                pageTimeGraph.Tick();
            };
            tmr.Start();
            #endregion

            Design.Animation = true;
            Debug = true;

            navBar.IconGap = 0;
            navBar.IconAlignment = DvTextIconAlignment.LeftRight;
        }
        #endregion
    }
}

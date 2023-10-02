using Devinno.Skia.Controls;
using SampleRPi.Pages;

namespace SampleRPi
{
    partial class MainWindow
    {
        public const int W = 800;
        public const int H = 480;

        internal PageControl PageControl { get; set; }
        internal PageContainer PageContainer { get; set; }
        internal PageInput PageInput { get; set; }
        internal PageGraph PageGraph { get; set; }
        internal PageDialog PageDialog { get; set; }
        internal PageTable PageTable { get; set; }

        private DvNavBar nav;

        private void InitializeComponent()
        {
            PageControl = new PageControl { Name = nameof(PageControl) };
            PageContainer = new PageContainer { Name = nameof(PageContainer) };
            PageInput = new PageInput { Name = nameof(PageInput) };
            PageGraph = new PageGraph { Name = nameof(PageGraph) };
            PageDialog = new PageDialog { Name = nameof(PageDialog) };
            PageTable = new PageTable { Name = nameof(PageTable) };

            Design.Pages.Add(PageControl);
            Design.Pages.Add(PageContainer);
            Design.Pages.Add(PageInput);
            Design.Pages.Add(PageGraph);
            Design.Pages.Add(PageDialog);
            Design.Pages.Add(PageTable);

            nav = new DvNavBar { Name = nameof(nav) };

            nav.Title = "SAMPLE";
            nav.TitleFontName = "NanumGothic";
            nav.TitleFontSize = 18;
            nav.FontName = "NanumGothic";
            nav.FontSize = 12;
            nav.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            nav.IconSize = 12;
            nav.IconGap = 5;
            nav.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            nav.DropDownViewCount = 8;
            nav.DropDownItemHeight = 30;
            nav.DropDownWidth = null;
            nav.BarHeight = 50;
            nav.Name = "nav";
            nav.X = 0;
            nav.Y = 0;
            nav.Width = 800;
            nav.Height = 50;
            nav.Visible = true;
            nav.Enabled = true;
            nav.Fill = true;
            nav.Margin = new Devinno.Skia.Design.Padding(0, 0, 0, 430);

            Design.MasterPage.Controls.Add(nav);

        }
    }
}


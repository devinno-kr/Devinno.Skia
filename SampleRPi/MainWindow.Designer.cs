using System;
using Devinno.Skia.Utils;
using Devinno.Skia.Controls;
using Devinno.Skia.Containers;
using Devinno.Skia.Design;
using SampleUI.Pages;

namespace SampleUI
{
    partial class MainWindow
    {
        public const int W = 800;
        public const int H = 480;

        internal PageMonitor PageMonitor { get; set; }
        internal PageSetting PageSetting { get; set; }
        internal PageControl PageControl { get; set; }

        private DvNavBar dvNavBar1;
        private void InitializeComponent()
        {
            PageMonitor = new PageMonitor { Name = nameof(PageMonitor) };
            PageSetting = new PageSetting { Name = nameof(PageSetting) };
            PageControl = new PageControl { Name = nameof(PageControl) };

            Design.Pages.Add(PageMonitor);
            Design.Pages.Add(PageSetting);
            Design.Pages.Add(PageControl);

            #region new
            dvNavBar1 = new DvNavBar { Name = nameof(dvNavBar1) };
            #endregion

            #region controls
            #region dvNavBar1
            dvNavBar1.BoxColor = null;
            dvNavBar1.ForeColor = null;
            dvNavBar1.BorderColor = null;
            dvNavBar1.LogoImage = Util.FromBitmap64("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");
            dvNavBar1.Title = "SAMPLE";
            dvNavBar1.TitleFontName = "NanumGothic";
            dvNavBar1.TitleFontSize = 18;
            dvNavBar1.FontName = "NanumGothic";
            dvNavBar1.FontSize = 12;
            dvNavBar1.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            dvNavBar1.IconSize = 12F;
            dvNavBar1.IconGap = 5;
            dvNavBar1.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            dvNavBar1.PageNames.Add("PageMonitor");
            dvNavBar1.PageNames.Add("PageControl");
            dvNavBar1.PageNames.Add("PageSetting");
            dvNavBar1.FoldingMenus.Add(new DvTextIcon { IconString = "", IconSize = 12, IconAlignment = DvTextIconAlignment.LeftRight, IconGap = 5, Text = "Test1", FontName = "NanumGothic", FontSize = 12, FontStyle = DvFontStyle.Normal, TextPadding = new Padding(0, 0, 0, 0) });
            dvNavBar1.FoldingMenus.Add(new DvTextIcon { IconString = "", IconSize = 12, IconAlignment = DvTextIconAlignment.LeftRight, IconGap = 5, Text = "Test2", FontName = "NanumGothic", FontSize = 12, FontStyle = DvFontStyle.Normal, TextPadding = new Padding(0, 0, 0, 0) });
            dvNavBar1.FoldingMenus.Add(new DvTextIcon { IconString = "", IconSize = 12, IconAlignment = DvTextIconAlignment.LeftRight, IconGap = 5, Text = "Test3", FontName = "NanumGothic", FontSize = 12, FontStyle = DvFontStyle.Normal, TextPadding = new Padding(0, 0, 0, 0) });
            dvNavBar1.FoldingMenus.Add(new DvTextIcon { IconString = "", IconSize = 12, IconAlignment = DvTextIconAlignment.LeftRight, IconGap = 5, Text = "Exit", FontName = "NanumGothic", FontSize = 12, FontStyle = DvFontStyle.Normal, TextPadding = new Padding(0, 0, 0, 0) });
            dvNavBar1.DropDownViewCount = 8;
            dvNavBar1.DropDownItemHeight = 30;
            dvNavBar1.DropDownWidth = null;
            dvNavBar1.BarHeight = 50;
            dvNavBar1.Name = "dvNavBar1";
            dvNavBar1.X = 0F;
            dvNavBar1.Y = 0F;
            dvNavBar1.Width = 800F;
            dvNavBar1.Height = 50F;
            dvNavBar1.Visible = true;
            dvNavBar1.Enabled = true;
            dvNavBar1.Fill = true;
            dvNavBar1.Margin = new Padding(0, 0, 0, 430);
            #endregion
            #endregion

            #region add
            Design.MasterPage.Controls.Add(dvNavBar1);
            #endregion
        }
    }
}


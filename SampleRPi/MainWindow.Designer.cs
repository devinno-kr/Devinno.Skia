using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devinno.Skia.Utils;
using Devinno.Skia.Controls;
using Devinno.Skia.Containers;
using Devinno.Skia.Design;
using SampleRPi.Pages;
using SampleRPi.Windows;

namespace SampleRPi
{
    partial class MainWindow
    {
        public const int W = 800;
        public const int H = 480;

        internal PageControl PageControl { get; set; }
        internal PageGauge PageGauge { get; set; }
        internal PageInput PageInput { get; set; }
        internal PageGraph PageGraph { get; set; }
        internal PageTable PageTable { get; set; }
        internal PageContainer PageContainer { get; set; }
        internal PageDialogs PageDialogs { get; set; }
        internal PageTest PageTest { get; set; }

        internal CustomWindow CustomWindow { get; set; }

        private DvNavBar nav;
        private void InitializeComponent()
        {
            PageControl = new PageControl { Name = nameof(PageControl) };
            PageGauge = new PageGauge { Name = nameof(PageGauge) };
            PageInput = new PageInput { Name = nameof(PageInput) };
            PageGraph = new PageGraph { Name = nameof(PageGraph) };
            PageTable = new PageTable { Name = nameof(PageTable) };
            PageContainer = new PageContainer { Name = nameof(PageContainer) };
            PageDialogs = new PageDialogs { Name = nameof(PageDialogs) };
            PageTest = new PageTest { Name = nameof(PageTest) };

            Design.Pages.Add(PageControl);
            Design.Pages.Add(PageGauge);
            Design.Pages.Add(PageInput);
            Design.Pages.Add(PageGraph);
            Design.Pages.Add(PageTable);
            Design.Pages.Add(PageContainer);
            Design.Pages.Add(PageDialogs);
            Design.Pages.Add(PageTest);

            CustomWindow = new CustomWindow { Name = nameof(CustomWindow) };

            #region new
            nav = new DvNavBar { Name = nameof(nav) };
            #endregion

            #region controls
            #region nav
            nav.BoxColor = null;
            nav.ForeColor = null;
            nav.BorderColor = null;
            nav.LogoImage = Util.FromBitmap64("iVBORw0KGgoAAAANSUhEUgAAACMAAAAoCAYAAAB0HkOaAAAABHNCSVQICAgIfAhkiAAABcdJREFUWIW9mF1sHFcVx3/nzszOrr07m9imjoMppTiIkCJEhYSq0iRWAxIVQg0flVo1gBRMU1e0pQSkgARCDRUVH2qLFEhaWgifhYfyACpFqe1EAqok8ECFRLp2EidtUpPgrmfJxruzM4eHtWV7d8cef8D/ZVdzz/2f3849986ZhVVqJJ8fGPG8fw7l87eu1ktWOnEom91iGfMjhffPuclvTRAMbi2XL/5fYF7o7m53r149AHxqEdPHJnx/7x0Q/s9gRvL5fag+kjC8KvDlbb7/+JrCDOVyNxuRp4F3JDWeyyAvo7p7u++fWBXMsba2nsi2DwEfWTZEc6LfR7a9p39y8tVlwSiYo573HeALCfJUgAngLXF+C7xVD1IqPdgP041jpvHCSD4/cNTzSglBzqjqju2+/1YRuRO4vNQEEblHPK84kss91DQ2+2XY8/oEngVuTAAB8MB233+i8eKw5+0X+GpCj3FRvWtbqfRnACmA+9fOzie7arVdjiq61HSRp1TkS/3FYjEu5Egu12kZ86io7k5ClA6C4Qh2SQivnE6nN51Op6mKkA1DDDRBCbwYGfPZ/mLxbJIEAMMdHb1Sqx0EbmscU8ANQ6woopjJ8Houd1EUfCBXMaZSSKf1rOs6tqrVFkWzQK8Y2LXV94+3Snhy48a2Uql049VS6cRt9WJu0rFs9l1qzCGFmxVIhSFOFOlUOl15PZvVyUwmExrzH1G4BHTNzKtddpzgVDqdKdr2dEZ1363F4mOxv7xeZ88AH0DkmKru7vf90bj4Y/n8x90geLzsOG+eyGanL7W3W1XLctxaDUt1qhFmVuOn4N3vhFIr0yO5XKctcgC4o2lQ5DnLmAdveeONc3FQBzZteuGK43woHQTYc3U61bS1Z/RqK5Bf9/Zmjubz37ZFLrcEAVDdGYbh+LDn/fh4e/uGViGBMa9lq1Wshg0TB+M0XjifSt2dKpcnr4jsXexkU8BSpT0IPl0x5uLha6/d12QehnarXRsH06TeavXr23w/vS4Mq1OWFVWMWXDczoMAqL7meZWXu7uZzGS++L3e3kySHHZSGCBaV6txU6nERcepnspkrKJlOW1RRDqKSNVqVG07uOB50b/a2ym5rmOHIW4YRkE6Ha05zMxnqicI9JparTruutOnXdcpG8NUNlu9kMuZkus6dhSZTP0Ooc1HVqwSL1ODxFJ1r5+edndMTdVw3drfNmxIlR3HzQSBccIFPVXinmk5d6YlFOCuC0M0ikiFy2rsmrTSO7PQRHXlzfR8nzXwWDMtB6bp7Ekoyy2XrbWGGV4hzPF7LlworwamqRIFBoCbgL8nhDgfwe2Do6MfbhoRaXnuxO2mlksi8BLwHq036E8B3S3CJhH5/GCh8ItYTNWWyxYH8z6F3wD3Sou+VuB3wAaFzwHfBbLAFUT2DhYKP4xjeOa669Jly3oUuLvVuAHiiusTwCWFr8SZCxwC3quwPwrDG5YCuWrbp0Tk/pgQE9fPNOrfwEMCh5eIi9UTfX29NowTX6e+AW4HTi/h1Qn8RKGg8MGVwLiOEzL3fGtUSUX2GIE/Cbyd+m5p2dnNUx/wR4WTCje0CvhBX9+uA319sUvboEhVH+4aHV1/X6HwywWnuIIL/BT4ZEKzw9SLvPzNzZt71gfBz4H+upmeReTOwdHRlwAObt7cEwbBOeY2zZAThncNnDkzMWsW93q7ZSZRkhe68yfXrx95fuPGj3VVKu0txv+QqdV2ltvabKlWS9Rr9KOzkPO16PNNYSdwEHjTYnEnOzp4vqeHrkrLN5XZRE+i+qs9Y2PDEtPjLPo4EHhO4Brg4cXillCAyPex7UfuHRsbigOBhP2MwNe0fuL+DLglKYWIvBiofub+QiH2b5Blw8wAnQO2Kuygfti9bZHwfxgY2FMo/CWp/7Jg5kEdAa7X+l8m3wJSs2MKE6g+cN/Y2LPL9V21FFIKh090dOg3tmzZv1q//wKFvR+wQmP/5AAAAABJRU5ErkJggg==");
            nav.Title = "SAMPLE";
            nav.TitleFontName = "NanumGothic";
            nav.TitleFontSize = 18;
            nav.FontName = "NanumGothic";
            nav.FontSize = 12;
            nav.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            nav.IconSize = 12F;
            nav.IconGap = 5;
            nav.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            nav.PageNames.Add("PageControl");
            nav.PageNames.Add("PageGauge");
            nav.PageNames.Add("PageInput");
            nav.PageNames.Add("PageGraph");
            nav.PageNames.Add("PageTable");
            nav.PageNames.Add("PageContainer");
            nav.PageNames.Add("PageDialogs");
            nav.PageNames.Add("PageTest");
            nav.FoldingMenus.Add(new DvTextIcon { IconString = "", IconSize = 12, IconAlignment = DvTextIconAlignment.LeftRight, IconGap = 5, Text = "Test1", FontName = "NanumGothic", FontSize = 12, FontStyle = DvFontStyle.Normal, TextPadding = new Padding(0, 0, 0, 0) });
            nav.FoldingMenus.Add(new DvTextIcon { IconString = "", IconSize = 12, IconAlignment = DvTextIconAlignment.LeftRight, IconGap = 5, Text = "Test2", FontName = "NanumGothic", FontSize = 12, FontStyle = DvFontStyle.Normal, TextPadding = new Padding(0, 0, 0, 0) });
            nav.FoldingMenus.Add(new DvTextIcon { IconString = "", IconSize = 12, IconAlignment = DvTextIconAlignment.LeftRight, IconGap = 5, Text = "Test3", FontName = "NanumGothic", FontSize = 12, FontStyle = DvFontStyle.Normal, TextPadding = new Padding(0, 0, 0, 0) });
            nav.FoldingMenus.Add(new DvTextIcon { IconString = "", IconSize = 12, IconAlignment = DvTextIconAlignment.LeftRight, IconGap = 5, Text = "Exit", FontName = "NanumGothic", FontSize = 12, FontStyle = DvFontStyle.Normal, TextPadding = new Padding(0, 0, 0, 0) });
            nav.DropDownViewCount = 8;
            nav.DropDownItemHeight = 30;
            nav.DropDownWidth = 120;
            nav.BarHeight = 50;
            nav.Name = "nav";
            nav.X = 0F;
            nav.Y = 0F;
            nav.Width = 800F;
            nav.Height = 50F;
            nav.Visible = true;
            nav.Enabled = true;
            nav.Fill = true;
            nav.Margin = new Padding(0, 0, 0, 430);
            #endregion
            #endregion

            #region add
            Design.MasterPage.Controls.Add(nav);
            #endregion
        }
    }
}


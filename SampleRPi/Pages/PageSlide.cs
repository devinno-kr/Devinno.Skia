using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleRPi.Pages
{
    public class PageSlide : DvPage
    {
        #region Member Variable
        private DvSlidePanel spnl;
        private DvSubPage sp1, sp2, sp3, sp4, sp5;
        private DvLabel lbl1, lbl2, lbl3, lbl4, lbl5;
        #endregion

        #region Constructor
        public PageSlide()
        {
            UseMasterPage = true;

            #region spnl
            spnl = new DvSlidePanel() { Name = nameof(spnl), Fill = true, Margin = MainWindow.BaseMargin };

            sp1 = new DvSubPage() { Name = nameof(sp1) };
            sp2 = new DvSubPage() { Name = nameof(sp2) };
            sp3 = new DvSubPage() { Name = nameof(sp3) };
            sp4 = new DvSubPage() { Name = nameof(sp4) };
            sp5 = new DvSubPage() { Name = nameof(sp5) };
            #endregion
            #region label
            lbl1 = new DvLabel() { Name = nameof(lbl1), Fill = true, Text = "PAGE 1", FontName = "impact", FontSize = 80, BackgroundDraw = false };
            lbl2 = new DvLabel() { Name = nameof(lbl2), Fill = true, Text = "PAGE 2", FontName = "impact", FontSize = 80, BackgroundDraw = false };
            lbl3 = new DvLabel() { Name = nameof(lbl3), Fill = true, Text = "PAGE 3", FontName = "impact", FontSize = 80, BackgroundDraw = false };
            lbl4 = new DvLabel() { Name = nameof(lbl4), Fill = true, Text = "PAGE 4", FontName = "impact", FontSize = 80, BackgroundDraw = false };
            lbl5 = new DvLabel() { Name = nameof(lbl5), Fill = true, Text = "PAGE 5", FontName = "impact", FontSize = 80, BackgroundDraw = false };
            #endregion
            #region Controls.Add
            Controls.Add(spnl);

            spnl.Pages.Add(sp1);
            spnl.Pages.Add(sp2);
            spnl.Pages.Add(sp3);
            spnl.Pages.Add(sp4);
            spnl.Pages.Add(sp5);

            sp1.Controls.Add(lbl1);
            sp2.Controls.Add(lbl2);
            sp3.Controls.Add(lbl3);
            sp4.Controls.Add(lbl4);
            sp5.Controls.Add(lbl5);
            #endregion

            spnl.SelectedPage = sp1;
            spnl.UseMoveButton = true;
            spnl.Direction = DvDirectionHV.Horizon;
            //spnl.Direction = DvDirectionHV.Vertical; 
            //spnl.UsePageButton = false;   
        }
        #endregion
    }
}

using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleRPi.Pages
{
    public class PageContainer : DvPage
    {
        #region Member Variable
        private DvScrollablePanel spnl;
        private DvTableLayoutPanel qpnl;
        private DvPanel pnl;
        private DvBorderPanel bpnl;
        private DvGroupBox gbox;
        private DvBoxPanel xpnl;
        private DvTabControl tab;
        private DvSubPage tp1, tp2, tp3;
        private DvTableLayoutPanel tpnl;

        private DvButton btn1;
        private DvButton btn2;
        private DvButton btn3;
        private DvButton btn4;

        private DvButton btn_t1;
        private DvButton btn_t2;
        private DvButton btn_t3;

        private DvLabel l_0_0;
        private DvLabel l_2_0;
        private DvLabel l_4_0;
        private DvLabel l_5_0;
        private DvLabel l_6_0;
        private DvLabel l_0_1;
        private DvLabel l_2_1;
        private DvLabel l_6_1;
        private DvLabel l_2_2;
        private DvLabel l_3_2;
        private DvLabel l_4_2;
        private DvLabel l_0_3;
        private DvLabel l_1_3;
        private DvLabel l_3_3;
        private DvLabel l_0_4;
        private DvLabel l_4_4;
        private DvLabel l_6_4;
        #endregion

        #region Constructor
        public PageContainer()
        {
            UseMasterPage = true;

            var vW = MainWindow.W - 20 - 20 - 15;
            var vH = MainWindow.H - 60 - 38;

            #region New
            #region spnl
            spnl = new DvScrollablePanel() { Name = nameof(spnl), Fill = true, Margin = MainWindow.BaseMargin };
            #endregion
            #region qpnl
            qpnl = new DvTableLayoutPanel() { Name = nameof(qpnl), Width = vW, Height = vH };
            qpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 50 });
            qpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 50 });
            qpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 50 });
            qpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 50 });
            #endregion
            #region pnl / bpnl / gbox / xpnl
            pnl = new DvPanel() { Name = nameof(pnl), Text = "패널", IconString = "fa-check" };
            bpnl = new DvBorderPanel() { Name = nameof(bpnl), Text = "보더 패널", IconString = "fa-check" };
            gbox = new DvGroupBox() { Name = nameof(gbox), Text = "그룹 박스", IconString = "fa-check", BorderWidth = 1F };
            xpnl = new DvBoxPanel() { Name = nameof(xpnl), Text = "박스 패널", IconString = "fa-check" };
            #endregion
            #region tab
            tab = new DvTabControl() { Name = nameof(tab), X = MainWindow.GAP+1, Y = qpnl.Bottom + 10, Width = vW - (MainWindow.GAP * 2 + 2), Height = vH };
            tp1 = new DvSubPage() { Name = nameof(tp1), Text = "테스트 1", IconString = "fa-tablet-alt" };
            tp2 = new DvSubPage() { Name = nameof(tp2), Text = "테스트 2", IconString = "fa-cube" };
            tp3 = new DvSubPage() { Name = nameof(tp3), Text = "테스트 3", IconString = "fa-bell" };
            #endregion
            #region tpnl
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Y = tab.Bottom + 10, Width = vW, Height = vH };

            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 10 });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 10 });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 15 });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 15 });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 20 });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 15 });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 15 });

            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 15 });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 20 });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 25 });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 20 });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 20 });
            #endregion
            #region buttons
            btn1 = new DvButton() { Name = nameof(btn1), X = 10, Y = pnl.TitleHeight + 10, Width = 80, Height = 30, Text = "버튼" };
            btn2 = new DvButton() { Name = nameof(btn2), X = 10, Y = bpnl.TitleHeight + 10, Width = 80, Height = 30, Text = "버튼" };
            btn3 = new DvButton() { Name = nameof(btn3), X = 10, Y = gbox.FontSize + 15, Width = 80, Height = 30, Text = "버튼" };
            btn4 = new DvButton() { Name = nameof(btn4), X = 10, Y = xpnl.TextPadding.Top + xpnl.FontSize + 15, Width = 80, Height = 30, Text = "버튼" };

            btn_t1 = new DvButton() { Name = nameof(btn_t1), X = 10, Y = 10, Width = 80, Height = 30, Text = "버튼" };
            btn_t2 = new DvButton() { Name = nameof(btn_t2), X = 10, Y = 10, Width = 80 * 2, Height = 30 * 2, Text = "버튼" };
            btn_t3 = new DvButton() { Name = nameof(btn_t3), X = 10, Y = 10, Width = 80 * 3, Height = 30 * 3, Text = "버튼" };
            #endregion
            #region labels
            l_0_0 = new DvLabel() { Name = nameof(l_0_0) };
            l_2_0 = new DvLabel() { Name = nameof(l_2_0) };
            l_4_0 = new DvLabel() { Name = nameof(l_4_0) };
            l_5_0 = new DvLabel() { Name = nameof(l_5_0) };
            l_6_0 = new DvLabel() { Name = nameof(l_6_0) };
            l_0_1 = new DvLabel() { Name = nameof(l_0_1) };
            l_2_1 = new DvLabel() { Name = nameof(l_2_1) };
            l_6_1 = new DvLabel() { Name = nameof(l_6_1) };
            l_2_2 = new DvLabel() { Name = nameof(l_2_2) };
            l_3_2 = new DvLabel() { Name = nameof(l_3_2) };
            l_4_2 = new DvLabel() { Name = nameof(l_4_2) };
            l_0_3 = new DvLabel() { Name = nameof(l_0_3) };
            l_1_3 = new DvLabel() { Name = nameof(l_1_3) };
            l_3_3 = new DvLabel() { Name = nameof(l_3_3) };
            l_0_4 = new DvLabel() { Name = nameof(l_0_4) };
            l_4_4 = new DvLabel() { Name = nameof(l_4_4) };
            l_6_4 = new DvLabel() { Name = nameof(l_6_4) };
            #endregion
            #endregion
            #region Controls.Add
            tp1.Controls.Add(btn_t1);
            tp2.Controls.Add(btn_t2);
            tp3.Controls.Add(btn_t3);

            tab.TabPages.Add(tp1);
            tab.TabPages.Add(tp2);
            tab.TabPages.Add(tp3);

            tpnl.Controls.Add(l_0_0, 0, 0, 2, 1);
            tpnl.Controls.Add(l_2_0, 2, 0, 2, 1);
            tpnl.Controls.Add(l_4_0, 4, 0);
            tpnl.Controls.Add(l_5_0, 5, 0);
            tpnl.Controls.Add(l_6_0, 6, 0);
            tpnl.Controls.Add(l_0_1, 0, 1, 2, 2);
            tpnl.Controls.Add(l_2_1, 2, 1, 4, 1);
            tpnl.Controls.Add(l_6_1, 6, 1);
            tpnl.Controls.Add(l_2_2, 2, 2, 1, 3);
            tpnl.Controls.Add(l_3_2, 3, 2);
            tpnl.Controls.Add(l_4_2, 4, 2, 3, 2);
            tpnl.Controls.Add(l_0_3, 0, 3);
            tpnl.Controls.Add(l_1_3, 1, 3);
            tpnl.Controls.Add(l_3_3, 3, 3, 1, 2);
            tpnl.Controls.Add(l_0_4, 0, 4, 2, 1);
            tpnl.Controls.Add(l_4_4, 4, 4, 2, 1);
            tpnl.Controls.Add(l_6_4, 6, 4);

            pnl.Controls.Add(btn1);
            bpnl.Controls.Add(btn2);
            gbox.Controls.Add(btn3);
            xpnl.Controls.Add(btn4);

            qpnl.Controls.Add(pnl, 0, 0);
            qpnl.Controls.Add(bpnl, 1, 0);
            qpnl.Controls.Add(gbox, 0, 1);
            qpnl.Controls.Add(xpnl, 1, 1);

            spnl.Controls.Add(qpnl);
            spnl.Controls.Add(tab);
            spnl.Controls.Add(tpnl);

            Controls.Add(spnl);
            #endregion

            #region Set
            foreach (var v in tpnl.Controls.Values)
            {
                var c = v as DvLabel;
                var m = tpnl.GetCellPosition(v);

                c.Text = string.Format("( {0} , {1} )", m.Column, m.Row);
                c.BackgroundDraw = true;
                c.MouseClick += (o, s) =>
                {
                    if (!spnl.IsScrolling)
                    {
                        Design.SetPage(Program.MainWindow.pageInformation);
                        var ss = ((DvLabel)o).Text;
                        Program.MainWindow.pageInformation.Title = ss.Substring(1, ss.Length - 2);
                    }
                };
            }

            tab.SelectedTab = tp1;
            tab.TabPosition = DvPosition.Left; tab.TabSize = 100;
            #endregion

            spnl.BufferDraw = true;

            qpnl.BufferDraw = false;
            tab.BufferDraw = false;
            tpnl.BufferDraw = false;
            
        }
        #endregion
    }
}

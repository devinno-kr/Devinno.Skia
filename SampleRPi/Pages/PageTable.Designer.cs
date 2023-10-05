using System;
using Devinno.Skia.Utils;
using Devinno.Skia.Controls;
using Devinno.Skia.Containers;
using Devinno.Skia.Design;

namespace SampleRPi.Pages
{
    partial class PageTable
    {
        #region declare
        private DvSlidePanel sldpnl;
        private DvSubPage spList;
        private DvTableLayoutPanel tbl;
        private DvListBox list;
        private DvToolBox tool;
        private DvTreeView tree;
        private DvSubPage spGrid;
        private DvDataGrid grid;
        #endregion

        public void InitializeComponent()
        {
            #region base
            UseMasterPage = true;
            BackgroundDraw = false;
            BackColor = null;
            BackgroundImage = null;
            AnimationType = null;
            Text = "Table";
            IconString = "";
            #endregion

            #region new
            sldpnl = new DvSlidePanel { Name = nameof(sldpnl) };
            spList = new DvSubPage { Name = nameof(spList) };
            tbl = new DvTableLayoutPanel { Name = nameof(tbl) };
            list = new DvListBox { Name = nameof(list) };
            tool = new DvToolBox { Name = nameof(tool) };
            tree = new DvTreeView { Name = nameof(tree) };
            spGrid = new DvSubPage { Name = nameof(spGrid) };
            grid = new DvDataGrid { Name = nameof(grid) };
            #endregion

            #region controls
            #region sldpnl
            sldpnl.Pages.Add(spList);
            sldpnl.Pages.Add(spGrid);
            sldpnl.ForeColor = null;
            sldpnl.UseMoveButton = true;
            sldpnl.UsePageButton = true;
            sldpnl.Direction = Devinno.Skia.Design.DvDirectionHV.Horizon;
            sldpnl.Name = "sldpnl";
            sldpnl.X = 10F;
            sldpnl.Y = 60F;
            sldpnl.Width = 780F;
            sldpnl.Height = 410F;
            sldpnl.Visible = true;
            sldpnl.Enabled = true;
            sldpnl.Fill = true;
            sldpnl.Margin = new Padding(10, 60, 10, 10);
            #endregion
            #region spList
            spList.Text = "Page";
            spList.IconString = "";
            spList.Name = "spList";
            spList.X = 49F;
            spList.Y = 3F;
            spList.Width = 682F;
            spList.Height = 379F;
            spList.Visible = true;
            spList.Enabled = true;
            spList.Fill = false;
            spList.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region tbl
            tbl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 33F));
            tbl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 34F));
            tbl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 33F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 100F));
            tbl.Name = "tbl";
            tbl.X = 3F;
            tbl.Y = 3F;
            tbl.Width = 676F;
            tbl.Height = 373F;
            tbl.Visible = true;
            tbl.Enabled = true;
            tbl.Fill = true;
            tbl.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region list
            list.ForeColor = null;
            list.BoxColor = null;
            list.SelectedColor = null;
            list.Items.Add(new ListBoxItem { Text = "Item 1",IconString = "" });
            list.Items.Add(new ListBoxItem { Text = "Item 2",IconString = "" });
            list.Items.Add(new ListBoxItem { Text = "Item 3",IconString = "" });
            list.Items.Add(new ListBoxItem { Text = "Item 4",IconString = "" });
            list.Items.Add(new ListBoxItem { Text = "Item 5",IconString = "" });
            list.Items.Add(new ListBoxItem { Text = "Item 6",IconString = "" });
            list.Items.Add(new ListBoxItem { Text = "Item 7",IconString = "" });
            list.FontName = "NanumGothic";
            list.FontSize = 12F;
            list.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            list.IconSize = 12F;
            list.IconGap = 5F;
            list.ItemHeight = 30;
            list.ItemAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            list.SelectionMode = Devinno.Skia.Design.ItemSelectionMode.SINGLE;
            list.Round = Devinno.Skia.Design.DvRoundType.All;
            list.BackgroundDraw = true;
            list.TouchMode = true;
            list.DrawScroll = true;
            list.Name = "list";
            list.X = 3F;
            list.Y = 3F;
            list.Width = 217.08F;
            list.Height = 367F;
            list.Visible = true;
            list.Enabled = true;
            list.Fill = false;
            list.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region tool
            tool.BoxColor = null;
            tool.CategoryColor = null;
            tool.ForeColor = null;
            {
                var tci = new ToolCategoryItem { Text = "Control", IconString = "" };
                tci.Items.Add(new ToolItem { Text = "Button", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "Label", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "OnOff", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "Switch", IconString = "" });
                tool.Categories.Add(tci);
            }
            {
                var tci = new ToolCategoryItem { Text = "Container", IconString = "" };
                tci.Items.Add(new ToolItem { Text = "Panel", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "BoxPanel", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "BorderPanel", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "GroupBox", IconString = "" });
                tool.Categories.Add(tci);
            }
            tool.FontName = "NanumGothic";
            tool.FontSize = 12F;
            tool.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            tool.IconSize = 12F;
            tool.IconGap = 5F;
            tool.ItemHeight = 30;
            tool.Round = Devinno.Skia.Design.DvRoundType.All;
            tool.BackgroundDraw = true;
            tool.TouchMode = true;
            tool.Name = "tool";
            tool.X = 226.08F;
            tool.Y = 3F;
            tool.Width = 223.83998F;
            tool.Height = 367F;
            tool.Visible = true;
            tool.Enabled = true;
            tool.Fill = false;
            tool.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region tree
            tree.BoxColor = null;
            tree.RadioColor = null;
            tree.SelectedColor = null;
            tree.ForeColor = null;
            {
                var tvn0 = new TreeViewNode { Text = "T1", IconString = "" };
                var tvn0_0 = new TreeViewNode { Text = "T1.1", IconString = "" };
                var tvn0_0_0 = new TreeViewNode { Text = "T1.1.1", IconString = "" };
                var tvn0_0_1 = new TreeViewNode { Text = "T1.1.2", IconString = "" };
                var tvn0_0_2 = new TreeViewNode { Text = "T1.1.3", IconString = "" };
                var tvn0_1 = new TreeViewNode { Text = "T1.2", IconString = "" };
                var tvn0_1_0 = new TreeViewNode { Text = "T1.2.1", IconString = "" };
                var tvn0_1_1 = new TreeViewNode { Text = "T1.2.2", IconString = "" };
                var tvn0_1_2 = new TreeViewNode { Text = "T1.2.3", IconString = "" };
                var tvn0_2 = new TreeViewNode { Text = "T1.3", IconString = "" };
                var tvn0_2_0 = new TreeViewNode { Text = "T1.3.1", IconString = "" };
                var tvn0_2_1 = new TreeViewNode { Text = "T1.3.2", IconString = "" };
                var tvn0_2_2 = new TreeViewNode { Text = "T1.3.3", IconString = "" };
                var tvn1 = new TreeViewNode { Text = "T2", IconString = "" };
                var tvn1_0 = new TreeViewNode { Text = "T2.1", IconString = "" };
                var tvn1_0_0 = new TreeViewNode { Text = "T2.1.1", IconString = "" };
                var tvn1_0_1 = new TreeViewNode { Text = "T2.1.2", IconString = "" };
                var tvn1_1 = new TreeViewNode { Text = "T2.2", IconString = "" };
                var tvn1_1_0 = new TreeViewNode { Text = "T2.2.1", IconString = "" };
                var tvn1_1_1 = new TreeViewNode { Text = "T2.2.2", IconString = "" };
                var tvn2 = new TreeViewNode { Text = "T3", IconString = "" };
                var tvn2_0 = new TreeViewNode { Text = "T3.1", IconString = "" };
                var tvn2_0_0 = new TreeViewNode { Text = "T3.1.1", IconString = "" };

                tree.Nodes.Add(tvn0);
                tvn0.Nodes.Add(tvn0_0);
                tvn0_0.Nodes.Add(tvn0_0_0);
                tvn0_0.Nodes.Add(tvn0_0_1);
                tvn0_0.Nodes.Add(tvn0_0_2);
                tvn0.Nodes.Add(tvn0_1);
                tvn0_1.Nodes.Add(tvn0_1_0);
                tvn0_1.Nodes.Add(tvn0_1_1);
                tvn0_1.Nodes.Add(tvn0_1_2);
                tvn0.Nodes.Add(tvn0_2);
                tvn0_2.Nodes.Add(tvn0_2_0);
                tvn0_2.Nodes.Add(tvn0_2_1);
                tvn0_2.Nodes.Add(tvn0_2_2);
                tree.Nodes.Add(tvn1);
                tvn1.Nodes.Add(tvn1_0);
                tvn1_0.Nodes.Add(tvn1_0_0);
                tvn1_0.Nodes.Add(tvn1_0_1);
                tvn1.Nodes.Add(tvn1_1);
                tvn1_1.Nodes.Add(tvn1_1_0);
                tvn1_1.Nodes.Add(tvn1_1_1);
                tree.Nodes.Add(tvn2);
                tvn2.Nodes.Add(tvn2_0);
                tvn2_0.Nodes.Add(tvn2_0_0);
            }
            tree.SelectionMode = Devinno.Skia.Design.ItemSelectionMode.SINGLE;
            tree.FontName = "NanumGothic";
            tree.FontSize = 12F;
            tree.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            tree.IconSize = 12F;
            tree.IconGap = 5F;
            tree.ItemHeight = 30;
            tree.Round = Devinno.Skia.Design.DvRoundType.All;
            tree.BackgroundDraw = true;
            tree.TouchMode = true;
            tree.Name = "tree";
            tree.X = 455.91998F;
            tree.Y = 3F;
            tree.Width = 217.08002F;
            tree.Height = 367F;
            tree.Visible = true;
            tree.Enabled = true;
            tree.Fill = false;
            tree.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region spGrid
            spGrid.Text = "Page";
            spGrid.IconString = "";
            spGrid.Name = "spGrid";
            spGrid.X = 737F;
            spGrid.Y = 3F;
            spGrid.Width = 682F;
            spGrid.Height = 379F;
            spGrid.Visible = true;
            spGrid.Enabled = true;
            spGrid.Fill = false;
            spGrid.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region grid
            grid.ForeColor = null;
            grid.BoxColor = null;
            grid.ColumnColor = null;
            grid.SelectedColor = null;
            grid.SummaryColor = null;
            grid.SelectorCellBackColor = null;
            grid.RowHeight = 30F;
            grid.ColumnHeight = 40F;
            grid.FontName = "NanumGothic";
            grid.FontSize = 12;
            grid.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            grid.SelectionMode = Devinno.Skia.Controls.DvDataGridSelectionMode.SINGLE;
            grid.Round = Devinno.Skia.Design.DvRoundType.All;
            grid.ScrollMode = Devinno.Skia.Utils.ScrollMode.Vertical;
            grid.VScrollPosition = 0;
            grid.HScrollPosition = 0;
            grid.TouchMode = true;
            grid.Name = "grid";
            grid.X = 6F;
            grid.Y = 6F;
            grid.Width = 670F;
            grid.Height = 367F;
            grid.Visible = true;
            grid.Enabled = true;
            grid.Fill = true;
            grid.Margin = new Padding(6, 6, 6, 6);
            #endregion
            #endregion

            #region add
            Controls.Add(sldpnl);
            spList.Controls.Add(tbl);
            tbl.Controls.Add(list, 0, 0, 1, 1);
            tbl.Controls.Add(tool, 1, 0, 1, 1);
            tbl.Controls.Add(tree, 2, 0, 1, 1);
            spGrid.Controls.Add(grid);
            #endregion
        }
    }
}

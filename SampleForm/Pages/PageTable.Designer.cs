using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devinno.Skia.Utils;
using Devinno.Skia.Controls;
using Devinno.Skia.Containers;
using Devinno.Skia.Design;

namespace SampleForm.Pages
{
    partial class PageTable
    {
        #region declare
        private DvSlidePanel sldpnl;
        private DvSubPage spList;
        private DvTableLayoutPanel tbl;
        private DvListBox list;
        private DvTreeView tree;
        private DvToolBox tool;
        private DvLabel lblList;
        private DvLabel lblTree;
        private DvLabel lblTool;
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
            tree = new DvTreeView { Name = nameof(tree) };
            tool = new DvToolBox { Name = nameof(tool) };
            lblList = new DvLabel { Name = nameof(lblList) };
            lblTree = new DvLabel { Name = nameof(lblTree) };
            lblTool = new DvLabel { Name = nameof(lblTool) };
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
            spList.X = -639F;
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
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 40F));
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
            list.Height = 327F;
            list.Visible = true;
            list.Enabled = true;
            list.Fill = false;
            list.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region tree
            tree.BoxColor = null;
            tree.RadioColor = null;
            tree.SelectedColor = null;
            tree.ForeColor = null;
            {
                var tvn0 = new TreeViewNode { Text = "Item 1", IconString = "" };
                var tvn0_0 = new TreeViewNode { Text = "Item 1.1", IconString = "" };
                var tvn0_0_0 = new TreeViewNode { Text = "Item 1.1.1", IconString = "" };
                var tvn0_0_1 = new TreeViewNode { Text = "Item 1.1.2", IconString = "" };
                var tvn0_0_2 = new TreeViewNode { Text = "Item 1.1.3", IconString = "" };
                var tvn0_1 = new TreeViewNode { Text = "Item 1.2", IconString = "" };
                var tvn0_1_0 = new TreeViewNode { Text = "Item 1.2.1", IconString = "" };
                var tvn0_1_1 = new TreeViewNode { Text = "Item 1.2.2", IconString = "" };
                var tvn0_1_2 = new TreeViewNode { Text = "Item 1.2.3", IconString = "" };
                var tvn0_2 = new TreeViewNode { Text = "Item 1.3", IconString = "" };
                var tvn0_2_0 = new TreeViewNode { Text = "Item 1.3.1", IconString = "" };
                var tvn0_2_1 = new TreeViewNode { Text = "Item 1.3.2", IconString = "" };
                var tvn0_2_2 = new TreeViewNode { Text = "Item 1.3.3", IconString = "" };
                var tvn1 = new TreeViewNode { Text = "Item 2", IconString = "" };
                var tvn1_0 = new TreeViewNode { Text = "Item 2.1", IconString = "" };
                var tvn1_0_0 = new TreeViewNode { Text = "Item 2.1.1", IconString = "" };
                var tvn1_0_1 = new TreeViewNode { Text = "Item 2.1.2", IconString = "" };
                var tvn1_1 = new TreeViewNode { Text = "Item 2.2", IconString = "" };
                var tvn1_1_0 = new TreeViewNode { Text = "Item 2.2.1", IconString = "" };
                var tvn1_1_1 = new TreeViewNode { Text = "Item 2.2.2", IconString = "" };
                var tvn2 = new TreeViewNode { Text = "Item 3", IconString = "" };
                var tvn2_0 = new TreeViewNode { Text = "Item 3.1", IconString = "" };
                var tvn2_0_0 = new TreeViewNode { Text = "Item 3.1.1", IconString = "" };

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
            tree.X = 226.08F;
            tree.Y = 3F;
            tree.Width = 223.83998F;
            tree.Height = 327F;
            tree.Visible = true;
            tree.Enabled = true;
            tree.Fill = false;
            tree.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region tool
            tool.BoxColor = null;
            tool.CategoryColor = null;
            tool.ForeColor = null;
            {
                var tci = new ToolCategoryItem { Text = "Controls", IconString = "" };
                tci.Items.Add(new ToolItem { Text = "Button", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "Label", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "Input", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "Check", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "Radio", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "Switch", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "OnOff", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "Progress", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "Slider", IconString = "" });
                tool.Categories.Add(tci);
            }
            {
                var tci = new ToolCategoryItem { Text = "Containers", IconString = "" };
                tci.Items.Add(new ToolItem { Text = "Panel", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "BorderPanel", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "BoxPanel", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "GroupBox", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "SwitchPanel", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "SliderPanel", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "TabControl", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "GridLayoutPanel", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "TableLayoutPanel", IconString = "" });
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
            tool.X = 455.91998F;
            tool.Y = 3F;
            tool.Width = 217.08002F;
            tool.Height = 327F;
            tool.Visible = true;
            tool.Enabled = true;
            tool.Fill = false;
            tool.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region lblList
            lblList.IconString = "";
            lblList.IconSize = 12F;
            lblList.IconGap = 5F;
            lblList.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            lblList.Text = "";
            lblList.TextPadding = new Padding(0, 0, 0, 0);
            lblList.FontName = "NanumGothic";
            lblList.FontSize = 12F;
            lblList.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            lblList.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            lblList.BackgroundDraw = true;
            lblList.LabelColor = null;
            lblList.ForeColor = null;
            lblList.BorderColor = null;
            lblList.Round = null;
            lblList.Name = "lblList";
            lblList.X = 3F;
            lblList.Y = 336F;
            lblList.Width = 217.08F;
            lblList.Height = 34F;
            lblList.Visible = true;
            lblList.Enabled = true;
            lblList.Fill = false;
            lblList.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region lblTree
            lblTree.IconString = "";
            lblTree.IconSize = 12F;
            lblTree.IconGap = 5F;
            lblTree.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            lblTree.Text = "";
            lblTree.TextPadding = new Padding(0, 0, 0, 0);
            lblTree.FontName = "NanumGothic";
            lblTree.FontSize = 12F;
            lblTree.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            lblTree.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            lblTree.BackgroundDraw = true;
            lblTree.LabelColor = null;
            lblTree.ForeColor = null;
            lblTree.BorderColor = null;
            lblTree.Round = null;
            lblTree.Name = "lblTree";
            lblTree.X = 226.08F;
            lblTree.Y = 336F;
            lblTree.Width = 223.83998F;
            lblTree.Height = 34F;
            lblTree.Visible = true;
            lblTree.Enabled = true;
            lblTree.Fill = false;
            lblTree.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region lblTool
            lblTool.IconString = "";
            lblTool.IconSize = 12F;
            lblTool.IconGap = 5F;
            lblTool.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            lblTool.Text = "";
            lblTool.TextPadding = new Padding(0, 0, 0, 0);
            lblTool.FontName = "NanumGothic";
            lblTool.FontSize = 12F;
            lblTool.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            lblTool.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            lblTool.BackgroundDraw = true;
            lblTool.LabelColor = null;
            lblTool.ForeColor = null;
            lblTool.BorderColor = null;
            lblTool.Round = null;
            lblTool.Name = "lblTool";
            lblTool.X = 455.91998F;
            lblTool.Y = 336F;
            lblTool.Width = 217.08002F;
            lblTool.Height = 34F;
            lblTool.Visible = true;
            lblTool.Enabled = true;
            lblTool.Fill = false;
            lblTool.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region spGrid
            spGrid.Text = "Page";
            spGrid.IconString = "";
            spGrid.Name = "spGrid";
            spGrid.X = 49F;
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
            tbl.Controls.Add(tree, 1, 0, 1, 1);
            tbl.Controls.Add(tool, 2, 0, 1, 1);
            tbl.Controls.Add(lblList, 0, 1, 1, 1);
            tbl.Controls.Add(lblTree, 1, 1, 1, 1);
            tbl.Controls.Add(lblTool, 2, 1, 1, 1);
            spGrid.Controls.Add(grid);
            #endregion
        }
    }
}

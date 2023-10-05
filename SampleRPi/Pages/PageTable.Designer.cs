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
        private DvScrollablePanel scrpnl;
        private DvTreeView tree;
        private DvToolBox tool;
        private DvListBox list;
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
            scrpnl = new DvScrollablePanel { Name = nameof(scrpnl) };
            tree = new DvTreeView { Name = nameof(tree) };
            tool = new DvToolBox { Name = nameof(tool) };
            list = new DvListBox { Name = nameof(list) };
            grid = new DvDataGrid { Name = nameof(grid) };
            #endregion

            #region controls
            #region scrpnl
            scrpnl.TouchMode = true;
            scrpnl.Name = "scrpnl";
            scrpnl.X = 10F;
            scrpnl.Y = 60F;
            scrpnl.Width = 780F;
            scrpnl.Height = 410F;
            scrpnl.Visible = true;
            scrpnl.Enabled = true;
            scrpnl.Fill = false;
            scrpnl.Margin = new Padding(10, 60, 10, 10);
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
            tree.X = 252.79999F;
            tree.Y = 10F;
            tree.Width = 244.79999F;
            tree.Height = 300F;
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
                var tci = new ToolCategoryItem { Text = "Control", IconString = "" };
                tci.Items.Add(new ToolItem { Text = "Button", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "Label", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "Input", IconString = "" });
                tool.Categories.Add(tci);
            }
            {
                var tci = new ToolCategoryItem { Text = "Container", IconString = "" };
                tci.Items.Add(new ToolItem { Text = "Panel", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "GroupBox", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "BorderPanel", IconString = "" });
                tool.Categories.Add(tci);
            }
            {
                var tci = new ToolCategoryItem { Text = "Dialogs", IconString = "" };
                tci.Items.Add(new ToolItem { Text = "MessageBox", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "InputBox", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "SelectorBox", IconString = "" });
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
            tool.X = 507.59998F;
            tool.Y = 10F;
            tool.Width = 252.40002F;
            tool.Height = 300F;
            tool.Visible = true;
            tool.Enabled = true;
            tool.Fill = false;
            tool.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region list
            list.ForeColor = null;
            list.BoxColor = null;
            list.SelectedColor = null;
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
            list.X = 10F;
            list.Y = 10F;
            list.Width = 232.79999F;
            list.Height = 300F;
            list.Visible = true;
            list.Enabled = true;
            list.Fill = false;
            list.Margin = new Padding(3, 3, 3, 3);
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
            grid.X = 10F;
            grid.Y = 320F;
            grid.Width = 750F;
            grid.Height = 426F;
            grid.Visible = true;
            grid.Enabled = true;
            grid.Fill = false;
            grid.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #endregion

            #region add
            this.Controls.Add(scrpnl);
            scrpnl.Controls.Add(tree);
            scrpnl.Controls.Add(tool);
            scrpnl.Controls.Add(list);
            scrpnl.Controls.Add(grid);
            #endregion
        }
    }
}

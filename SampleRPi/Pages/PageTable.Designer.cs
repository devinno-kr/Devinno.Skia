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
        private DvToolBox dvToolBox1;
        private DvTreeView dvTreeView1;
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
            dvToolBox1 = new DvToolBox { Name = nameof(dvToolBox1) };
            dvTreeView1 = new DvTreeView { Name = nameof(dvTreeView1) };
            #endregion

            #region controls
            #region dvToolBox1
            dvToolBox1.BoxColor = null;
            dvToolBox1.CategoryColor = null;
            dvToolBox1.ForeColor = null;
            {
                var tci = new ToolCategoryItem { Text = "Control", IconString = "" };
                tci.Items.Add(new ToolItem { Text = "Button", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "Label", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "Input", IconString = "" });
                dvToolBox1.Categories.Add(tci);
            }
            {
                var tci = new ToolCategoryItem { Text = "Container", IconString = "" };
                tci.Items.Add(new ToolItem { Text = "Panel", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "GroupBox", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "BorderPanel", IconString = "" });
                dvToolBox1.Categories.Add(tci);
            }
            {
                var tci = new ToolCategoryItem { Text = "Dialogs", IconString = "" };
                tci.Items.Add(new ToolItem { Text = "MessageBox", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "InputBox", IconString = "" });
                tci.Items.Add(new ToolItem { Text = "SelectorBox", IconString = "" });
                dvToolBox1.Categories.Add(tci);
            }
            dvToolBox1.FontName = "NanumGothic";
            dvToolBox1.FontSize = 12F;
            dvToolBox1.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            dvToolBox1.IconSize = 12F;
            dvToolBox1.IconGap = 5F;
            dvToolBox1.ItemHeight = 30;
            dvToolBox1.Round = Devinno.Skia.Design.DvRoundType.All;
            dvToolBox1.BackgroundDraw = true;
            dvToolBox1.TouchMode = true;
            dvToolBox1.Name = "dvToolBox1";
            dvToolBox1.X = 10F;
            dvToolBox1.Y = 60F;
            dvToolBox1.Width = 221F;
            dvToolBox1.Height = 410F;
            dvToolBox1.Visible = true;
            dvToolBox1.Enabled = true;
            dvToolBox1.Fill = false;
            dvToolBox1.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region dvTreeView1
            dvTreeView1.BoxColor = null;
            dvTreeView1.RadioColor = null;
            dvTreeView1.SelectedColor = null;
            dvTreeView1.ForeColor = null;
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

                dvTreeView1.Nodes.Add(tvn0);
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
                dvTreeView1.Nodes.Add(tvn1);
                tvn1.Nodes.Add(tvn1_0);
                tvn1_0.Nodes.Add(tvn1_0_0);
                tvn1_0.Nodes.Add(tvn1_0_1);
                tvn1.Nodes.Add(tvn1_1);
                tvn1_1.Nodes.Add(tvn1_1_0);
                tvn1_1.Nodes.Add(tvn1_1_1);
                dvTreeView1.Nodes.Add(tvn2);
                tvn2.Nodes.Add(tvn2_0);
                tvn2_0.Nodes.Add(tvn2_0_0);
            }
            dvTreeView1.SelectionMode = Devinno.Skia.Design.ItemSelectionMode.SINGLE;
            dvTreeView1.FontName = "NanumGothic";
            dvTreeView1.FontSize = 12F;
            dvTreeView1.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            dvTreeView1.IconSize = 12F;
            dvTreeView1.IconGap = 5F;
            dvTreeView1.ItemHeight = 30;
            dvTreeView1.Round = Devinno.Skia.Design.DvRoundType.All;
            dvTreeView1.BackgroundDraw = true;
            dvTreeView1.TouchMode = true;
            dvTreeView1.Name = "dvTreeView1";
            dvTreeView1.X = 241F;
            dvTreeView1.Y = 60F;
            dvTreeView1.Width = 245F;
            dvTreeView1.Height = 410F;
            dvTreeView1.Visible = true;
            dvTreeView1.Enabled = true;
            dvTreeView1.Fill = false;
            dvTreeView1.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #endregion

            #region add
            this.Controls.Add(dvToolBox1);
            this.Controls.Add(dvTreeView1);
            #endregion
        }
    }
}

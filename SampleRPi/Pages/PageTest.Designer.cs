using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devinno.Skia.Utils;
using Devinno.Skia.Controls;
using Devinno.Skia.Containers;
using Devinno.Skia.Design;

namespace SampleRPi.Pages
{
    partial class PageTest
    {
        #region declare
        private DvTabControl dvTabControl1;
        private DvSubPage T1;
        private DvTableLayoutPanel dvTableLayoutPanel1;
        private DvButton dvButton1;
        private DvSubPage T2;
        private DvSubPage T3;
        #endregion

        public void InitializeComponent()
        {
            #region base
            UseMasterPage = true;
            BackgroundDraw = false;
            BackColor = null;
            BackgroundImage = null;
            AnimationType = null;
            Text = "Test";
            IconString = "";
            #endregion

            #region new
            dvTabControl1 = new DvTabControl { Name = nameof(dvTabControl1) };
            T1 = new DvSubPage { Name = nameof(T1) };
            dvTableLayoutPanel1 = new DvTableLayoutPanel { Name = nameof(dvTableLayoutPanel1) };
            dvButton1 = new DvButton { Name = nameof(dvButton1) };
            T2 = new DvSubPage { Name = nameof(T2) };
            T3 = new DvSubPage { Name = nameof(T3) };
            #endregion

            #region controls
            #region dvTabControl1
            dvTabControl1.ForeColor = null;
            dvTabControl1.PageColor = null;
            dvTabControl1.TabBackColor = null;
            dvTabControl1.PointColor = null;
            dvTabControl1.Round = null;
            dvTabControl1.TabPosition = Devinno.Skia.Design.DvPosition.Top;
            dvTabControl1.TabSize = 40;
            dvTabControl1.TabPages.Add(T1);
            dvTabControl1.TabPages.Add(T2);
            dvTabControl1.TabPages.Add(T3);
            dvTabControl1.FontName = "NanumGothic";
            dvTabControl1.FontSize = 12F;
            dvTabControl1.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            dvTabControl1.IconSize = 12F;
            dvTabControl1.IconGap = 5F;
            dvTabControl1.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            dvTabControl1.Name = "dvTabControl1";
            dvTabControl1.X = 10F;
            dvTabControl1.Y = 60F;
            dvTabControl1.Width = 780F;
            dvTabControl1.Height = 410F;
            dvTabControl1.Visible = true;
            dvTabControl1.Enabled = true;
            dvTabControl1.Fill = false;
            dvTabControl1.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region T1
            T1.Text = "Page 1";
            T1.IconString = "";
            T1.Name = "T1";
            T1.X = 0F;
            T1.Y = 39F;
            T1.Width = 780F;
            T1.Height = 370F;
            T1.Visible = true;
            T1.Enabled = true;
            T1.Fill = false;
            T1.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region dvTableLayoutPanel1
            dvTableLayoutPanel1.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25F));
            dvTableLayoutPanel1.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25F));
            dvTableLayoutPanel1.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25F));
            dvTableLayoutPanel1.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25F));
            dvTableLayoutPanel1.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20F));
            dvTableLayoutPanel1.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20F));
            dvTableLayoutPanel1.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20F));
            dvTableLayoutPanel1.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20F));
            dvTableLayoutPanel1.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20F));
            dvTableLayoutPanel1.Name = "dvTableLayoutPanel1";
            dvTableLayoutPanel1.X = 10F;
            dvTableLayoutPanel1.Y = 10F;
            dvTableLayoutPanel1.Width = 671F;
            dvTableLayoutPanel1.Height = 350F;
            dvTableLayoutPanel1.Visible = true;
            dvTableLayoutPanel1.Enabled = true;
            dvTableLayoutPanel1.Fill = false;
            dvTableLayoutPanel1.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region dvButton1
            dvButton1.IconString = "";
            dvButton1.IconSize = 12F;
            dvButton1.IconGap = 5F;
            dvButton1.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            dvButton1.Text = "dvButton1";
            dvButton1.TextPadding = new Padding(0, 0, 0, 0);
            dvButton1.FontName = "NanumGothic";
            dvButton1.FontSize = 12F;
            dvButton1.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            dvButton1.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            dvButton1.BackgroundDraw = true;
            dvButton1.ButtonColor = null;
            dvButton1.ForeColor = null;
            dvButton1.BorderColor = null;
            dvButton1.Round = null;
            dvButton1.Gradient = true;
            dvButton1.Clickable = true;
            dvButton1.Name = "dvButton1";
            dvButton1.X = 3F;
            dvButton1.Y = 3F;
            dvButton1.Width = 161.75F;
            dvButton1.Height = 64F;
            dvButton1.Visible = true;
            dvButton1.Enabled = true;
            dvButton1.Fill = false;
            dvButton1.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region T2
            T2.Text = "Page 2";
            T2.IconString = "";
            T2.Name = "T2";
            T2.X = 0F;
            T2.Y = 39F;
            T2.Width = 780F;
            T2.Height = 370F;
            T2.Visible = true;
            T2.Enabled = true;
            T2.Fill = false;
            T2.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region T3
            T3.Text = "Page 3";
            T3.IconString = "";
            T3.Name = "T3";
            T3.X = 0F;
            T3.Y = 39F;
            T3.Width = 780F;
            T3.Height = 370F;
            T3.Visible = true;
            T3.Enabled = true;
            T3.Fill = false;
            T3.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #endregion

            #region add
            Controls.Add(dvTabControl1);
            T1.Controls.Add(dvTableLayoutPanel1);
            dvTableLayoutPanel1.Controls.Add(dvButton1, 0, 0, 1, 1);
            #endregion
        }
    }
}

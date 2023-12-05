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
        private DvTableLayoutPanel dvTableLayoutPanel1;
        private DvButton dvButton1;
        private DvButton dvButton2;
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
            dvTableLayoutPanel1 = new DvTableLayoutPanel { Name = nameof(dvTableLayoutPanel1) };
            dvButton1 = new DvButton { Name = nameof(dvButton1) };
            dvButton2 = new DvButton { Name = nameof(dvButton2) };
            #endregion

            #region controls
            #region dvTableLayoutPanel1
            dvTableLayoutPanel1.Columns.Add(new SizeInfo(DvSizeMode.Percent, 33F));
            dvTableLayoutPanel1.Columns.Add(new SizeInfo(DvSizeMode.Percent, 34F));
            dvTableLayoutPanel1.Columns.Add(new SizeInfo(DvSizeMode.Percent, 33F));
            dvTableLayoutPanel1.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20F));
            dvTableLayoutPanel1.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20F));
            dvTableLayoutPanel1.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20F));
            dvTableLayoutPanel1.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20F));
            dvTableLayoutPanel1.Rows.Add(new SizeInfo(DvSizeMode.Percent, 20F));
            dvTableLayoutPanel1.Name = "dvTableLayoutPanel1";
            dvTableLayoutPanel1.X = 10F;
            dvTableLayoutPanel1.Y = 60F;
            dvTableLayoutPanel1.Width = 780F;
            dvTableLayoutPanel1.Height = 410F;
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
            dvButton1.Width = 251.40002F;
            dvButton1.Height = 158F;
            dvButton1.Visible = true;
            dvButton1.Enabled = true;
            dvButton1.Fill = false;
            dvButton1.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region dvButton2
            dvButton2.IconString = "";
            dvButton2.IconSize = 12F;
            dvButton2.IconGap = 5F;
            dvButton2.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            dvButton2.Text = "dvButton2";
            dvButton2.TextPadding = new Padding(0, 0, 0, 0);
            dvButton2.FontName = "NanumGothic";
            dvButton2.FontSize = 12F;
            dvButton2.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            dvButton2.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            dvButton2.BackgroundDraw = true;
            dvButton2.ButtonColor = null;
            dvButton2.ForeColor = null;
            dvButton2.BorderColor = null;
            dvButton2.Round = null;
            dvButton2.Gradient = true;
            dvButton2.Clickable = true;
            dvButton2.Name = "dvButton2";
            dvButton2.X = 260.40002F;
            dvButton2.Y = 3F;
            dvButton2.Width = 259.2F;
            dvButton2.Height = 76F;
            dvButton2.Visible = true;
            dvButton2.Enabled = true;
            dvButton2.Fill = false;
            dvButton2.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #endregion

            #region add
            Controls.Add(dvTableLayoutPanel1);
            dvTableLayoutPanel1.Controls.Add(dvButton1, 0, 0, 1, 2);
            dvTableLayoutPanel1.Controls.Add(dvButton2, 1, 0, 1, 1);
            #endregion
        }
    }
}

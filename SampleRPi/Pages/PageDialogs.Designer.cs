using System;
using Devinno.Skia.Utils;
using Devinno.Skia.Controls;
using Devinno.Skia.Containers;
using Devinno.Skia.Design;

namespace SampleRPi.Pages
{
    partial class PageDialogs
    {
        #region declare
        private DvGridLayoutPanel dvGridLayoutPanel1;
        private DvButton dvButton1;
        private DvButton dvButton2;
        private DvButton dvButton3;
        private DvButton dvButton4;
        private DvButton dvButton5;
        private DvButton dvButton6;
        private DvButton dvButton7;
        private DvButton dvButton8;
        private DvButton dvButton9;
        private DvButton dvButton10;
        #endregion

        public void InitializeComponent()
        {
            #region base
            UseMasterPage = true;
            BackgroundDraw = false;
            BackColor = null;
            BackgroundImage = null;
            AnimationType = null;
            Text = "Dialogs";
            IconString = "";
            #endregion

            #region new
            dvGridLayoutPanel1 = new DvGridLayoutPanel { Name = nameof(dvGridLayoutPanel1) };
            dvButton1 = new DvButton { Name = nameof(dvButton1) };
            dvButton2 = new DvButton { Name = nameof(dvButton2) };
            dvButton3 = new DvButton { Name = nameof(dvButton3) };
            dvButton4 = new DvButton { Name = nameof(dvButton4) };
            dvButton5 = new DvButton { Name = nameof(dvButton5) };
            dvButton6 = new DvButton { Name = nameof(dvButton6) };
            dvButton7 = new DvButton { Name = nameof(dvButton7) };
            dvButton8 = new DvButton { Name = nameof(dvButton8) };
            dvButton9 = new DvButton { Name = nameof(dvButton9) };
            dvButton10 = new DvButton { Name = nameof(dvButton10) };
            #endregion

            #region controls
            #region dvGridLayoutPanel1
            {
                var dgr = new DvGridRow(DvSizeMode.Percent, 25F);
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25F));
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25F));
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25F));
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 25F));
                dvGridLayoutPanel1.Rows.Add(dgr);
            }
            {
                var dgr = new DvGridRow(DvSizeMode.Percent, 25F);
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 33F));
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 34F));
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 33F));
                dvGridLayoutPanel1.Rows.Add(dgr);
            }
            {
                var dgr = new DvGridRow(DvSizeMode.Percent, 25F);
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 50F));
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 50F));
                dvGridLayoutPanel1.Rows.Add(dgr);
            }
            {
                var dgr = new DvGridRow(DvSizeMode.Percent, 25F);
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 100F));
                dvGridLayoutPanel1.Rows.Add(dgr);
            }
            dvGridLayoutPanel1.Name = "dvGridLayoutPanel1";
            dvGridLayoutPanel1.X = 10F;
            dvGridLayoutPanel1.Y = 60F;
            dvGridLayoutPanel1.Width = 780F;
            dvGridLayoutPanel1.Height = 410F;
            dvGridLayoutPanel1.Visible = true;
            dvGridLayoutPanel1.Enabled = true;
            dvGridLayoutPanel1.Fill = true;
            dvGridLayoutPanel1.Margin = new Padding(10, 60, 10, 10);
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
            dvButton1.Width = 189F;
            dvButton1.Height = 96.5F;
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
            dvButton2.X = 198F;
            dvButton2.Y = 3F;
            dvButton2.Width = 189F;
            dvButton2.Height = 96.5F;
            dvButton2.Visible = true;
            dvButton2.Enabled = true;
            dvButton2.Fill = false;
            dvButton2.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region dvButton3
            dvButton3.IconString = "";
            dvButton3.IconSize = 12F;
            dvButton3.IconGap = 5F;
            dvButton3.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            dvButton3.Text = "dvButton3";
            dvButton3.TextPadding = new Padding(0, 0, 0, 0);
            dvButton3.FontName = "NanumGothic";
            dvButton3.FontSize = 12F;
            dvButton3.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            dvButton3.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            dvButton3.BackgroundDraw = true;
            dvButton3.ButtonColor = null;
            dvButton3.ForeColor = null;
            dvButton3.BorderColor = null;
            dvButton3.Round = null;
            dvButton3.Gradient = true;
            dvButton3.Clickable = true;
            dvButton3.Name = "dvButton3";
            dvButton3.X = 393F;
            dvButton3.Y = 3F;
            dvButton3.Width = 189F;
            dvButton3.Height = 96.5F;
            dvButton3.Visible = true;
            dvButton3.Enabled = true;
            dvButton3.Fill = false;
            dvButton3.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region dvButton4
            dvButton4.IconString = "";
            dvButton4.IconSize = 12F;
            dvButton4.IconGap = 5F;
            dvButton4.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            dvButton4.Text = "dvButton4";
            dvButton4.TextPadding = new Padding(0, 0, 0, 0);
            dvButton4.FontName = "NanumGothic";
            dvButton4.FontSize = 12F;
            dvButton4.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            dvButton4.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            dvButton4.BackgroundDraw = true;
            dvButton4.ButtonColor = null;
            dvButton4.ForeColor = null;
            dvButton4.BorderColor = null;
            dvButton4.Round = null;
            dvButton4.Gradient = true;
            dvButton4.Clickable = true;
            dvButton4.Name = "dvButton4";
            dvButton4.X = 588F;
            dvButton4.Y = 3F;
            dvButton4.Width = 189F;
            dvButton4.Height = 96.5F;
            dvButton4.Visible = true;
            dvButton4.Enabled = true;
            dvButton4.Fill = false;
            dvButton4.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region dvButton5
            dvButton5.IconString = "";
            dvButton5.IconSize = 12F;
            dvButton5.IconGap = 5F;
            dvButton5.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            dvButton5.Text = "dvButton5";
            dvButton5.TextPadding = new Padding(0, 0, 0, 0);
            dvButton5.FontName = "NanumGothic";
            dvButton5.FontSize = 12F;
            dvButton5.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            dvButton5.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            dvButton5.BackgroundDraw = true;
            dvButton5.ButtonColor = null;
            dvButton5.ForeColor = null;
            dvButton5.BorderColor = null;
            dvButton5.Round = null;
            dvButton5.Gradient = true;
            dvButton5.Clickable = true;
            dvButton5.Name = "dvButton5";
            dvButton5.X = 3F;
            dvButton5.Y = 105.5F;
            dvButton5.Width = 251.40002F;
            dvButton5.Height = 96.5F;
            dvButton5.Visible = true;
            dvButton5.Enabled = true;
            dvButton5.Fill = false;
            dvButton5.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region dvButton6
            dvButton6.IconString = "";
            dvButton6.IconSize = 12F;
            dvButton6.IconGap = 5F;
            dvButton6.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            dvButton6.Text = "dvButton6";
            dvButton6.TextPadding = new Padding(0, 0, 0, 0);
            dvButton6.FontName = "NanumGothic";
            dvButton6.FontSize = 12F;
            dvButton6.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            dvButton6.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            dvButton6.BackgroundDraw = true;
            dvButton6.ButtonColor = null;
            dvButton6.ForeColor = null;
            dvButton6.BorderColor = null;
            dvButton6.Round = null;
            dvButton6.Gradient = true;
            dvButton6.Clickable = true;
            dvButton6.Name = "dvButton6";
            dvButton6.X = 260.40002F;
            dvButton6.Y = 105.5F;
            dvButton6.Width = 259.2F;
            dvButton6.Height = 96.5F;
            dvButton6.Visible = true;
            dvButton6.Enabled = true;
            dvButton6.Fill = false;
            dvButton6.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region dvButton7
            dvButton7.IconString = "";
            dvButton7.IconSize = 12F;
            dvButton7.IconGap = 5F;
            dvButton7.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            dvButton7.Text = "dvButton7";
            dvButton7.TextPadding = new Padding(0, 0, 0, 0);
            dvButton7.FontName = "NanumGothic";
            dvButton7.FontSize = 12F;
            dvButton7.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            dvButton7.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            dvButton7.BackgroundDraw = true;
            dvButton7.ButtonColor = null;
            dvButton7.ForeColor = null;
            dvButton7.BorderColor = null;
            dvButton7.Round = null;
            dvButton7.Gradient = true;
            dvButton7.Clickable = true;
            dvButton7.Name = "dvButton7";
            dvButton7.X = 525.60004F;
            dvButton7.Y = 105.5F;
            dvButton7.Width = 251.40002F;
            dvButton7.Height = 96.5F;
            dvButton7.Visible = true;
            dvButton7.Enabled = true;
            dvButton7.Fill = false;
            dvButton7.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region dvButton8
            dvButton8.IconString = "";
            dvButton8.IconSize = 12F;
            dvButton8.IconGap = 5F;
            dvButton8.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            dvButton8.Text = "dvButton8";
            dvButton8.TextPadding = new Padding(0, 0, 0, 0);
            dvButton8.FontName = "NanumGothic";
            dvButton8.FontSize = 12F;
            dvButton8.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            dvButton8.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            dvButton8.BackgroundDraw = true;
            dvButton8.ButtonColor = null;
            dvButton8.ForeColor = null;
            dvButton8.BorderColor = null;
            dvButton8.Round = null;
            dvButton8.Gradient = true;
            dvButton8.Clickable = true;
            dvButton8.Name = "dvButton8";
            dvButton8.X = 3F;
            dvButton8.Y = 208F;
            dvButton8.Width = 384F;
            dvButton8.Height = 96.5F;
            dvButton8.Visible = true;
            dvButton8.Enabled = true;
            dvButton8.Fill = false;
            dvButton8.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region dvButton9
            dvButton9.IconString = "";
            dvButton9.IconSize = 12F;
            dvButton9.IconGap = 5F;
            dvButton9.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            dvButton9.Text = "dvButton9";
            dvButton9.TextPadding = new Padding(0, 0, 0, 0);
            dvButton9.FontName = "NanumGothic";
            dvButton9.FontSize = 12F;
            dvButton9.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            dvButton9.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            dvButton9.BackgroundDraw = true;
            dvButton9.ButtonColor = null;
            dvButton9.ForeColor = null;
            dvButton9.BorderColor = null;
            dvButton9.Round = null;
            dvButton9.Gradient = true;
            dvButton9.Clickable = true;
            dvButton9.Name = "dvButton9";
            dvButton9.X = 393F;
            dvButton9.Y = 208F;
            dvButton9.Width = 384F;
            dvButton9.Height = 96.5F;
            dvButton9.Visible = true;
            dvButton9.Enabled = true;
            dvButton9.Fill = false;
            dvButton9.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region dvButton10
            dvButton10.IconString = "";
            dvButton10.IconSize = 12F;
            dvButton10.IconGap = 5F;
            dvButton10.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            dvButton10.Text = "dvButton10";
            dvButton10.TextPadding = new Padding(0, 0, 0, 0);
            dvButton10.FontName = "NanumGothic";
            dvButton10.FontSize = 12F;
            dvButton10.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            dvButton10.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            dvButton10.BackgroundDraw = true;
            dvButton10.ButtonColor = null;
            dvButton10.ForeColor = null;
            dvButton10.BorderColor = null;
            dvButton10.Round = null;
            dvButton10.Gradient = true;
            dvButton10.Clickable = true;
            dvButton10.Name = "dvButton10";
            dvButton10.X = 3F;
            dvButton10.Y = 310.5F;
            dvButton10.Width = 774F;
            dvButton10.Height = 96.5F;
            dvButton10.Visible = true;
            dvButton10.Enabled = true;
            dvButton10.Fill = false;
            dvButton10.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #endregion

            #region add
            this.Controls.Add(dvGridLayoutPanel1);
            dvGridLayoutPanel1.Controls.Add(dvButton1, 0, 0);
            dvGridLayoutPanel1.Controls.Add(dvButton2, 1, 0);
            dvGridLayoutPanel1.Controls.Add(dvButton3, 2, 0);
            dvGridLayoutPanel1.Controls.Add(dvButton4, 3, 0);
            dvGridLayoutPanel1.Controls.Add(dvButton5, 0, 1);
            dvGridLayoutPanel1.Controls.Add(dvButton6, 1, 1);
            dvGridLayoutPanel1.Controls.Add(dvButton7, 2, 1);
            dvGridLayoutPanel1.Controls.Add(dvButton8, 0, 2);
            dvGridLayoutPanel1.Controls.Add(dvButton9, 1, 2);
            dvGridLayoutPanel1.Controls.Add(dvButton10, 0, 3);
            #endregion
        }
    }
}

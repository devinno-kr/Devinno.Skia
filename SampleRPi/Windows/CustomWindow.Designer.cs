using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devinno.Skia.Utils;
using Devinno.Skia.Controls;
using Devinno.Skia.Containers;
using Devinno.Skia.Design;

namespace SampleRPi.Windows
{
    partial class CustomWindow
    {
        #region declare
        private DvGridLayoutPanel grid;
        private DvButton btnOK;
        private DvButton btnCancel;
        private DvSwitch sw;
        #endregion

        public void InitializeComponent()
        {
            #region base
            Name = "CustomWindow";
            ForeColor = null;
            BackColor = null;
            TitleBarColor = null;
            IconBoxColor = null;
            WindowStateButtonColor = null;
            BackgroundImage = null;
            BackgroundDraw = true;
            UseTitleBar = true;
            ExitBox = true;
            Title = "Custom Window";
            TitleHeight = 40;
            FontName = "NanumGothic";
            FontSize = 12;
            IconString = "fa-cube";
            IconSize = 18;
            IconBoxWidth = 40;
            Width = 250;
            Height = 180;
            UseBlock = true;
            #endregion

            #region new
            grid = new DvGridLayoutPanel { Name = nameof(grid) };
            btnOK = new DvButton { Name = nameof(btnOK) };
            btnCancel = new DvButton { Name = nameof(btnCancel) };
            sw = new DvSwitch { Name = nameof(sw) };
            #endregion

            #region controls
            #region grid
            {
                var dgr = new DvGridRow(DvSizeMode.Percent, 100F);
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 100F));
                grid.Rows.Add(dgr);
            }
            {
                var dgr = new DvGridRow(DvSizeMode.Pixel, 10F);
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 100F));
                grid.Rows.Add(dgr);
            }
            {
                var dgr = new DvGridRow(DvSizeMode.Pixel, 40F);
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 20F));
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 40F));
                dgr.Columns.Add(new SizeInfo(DvSizeMode.Percent, 40F));
                grid.Rows.Add(dgr);
            }
            grid.Name = "grid";
            grid.X = 10F;
            grid.Y = 50F;
            grid.Width = 230F;
            grid.Height = 120F;
            grid.Visible = true;
            grid.Enabled = true;
            grid.Fill = true;
            grid.Margin = new Padding(10, 50, 10, 10);
            #endregion
            #region btnOK
            btnOK.IconString = "";
            btnOK.IconSize = 12F;
            btnOK.IconGap = 5F;
            btnOK.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            btnOK.Text = "확인";
            btnOK.TextPadding = new Padding(0, 0, 0, 0);
            btnOK.FontName = "NanumGothic";
            btnOK.FontSize = 12F;
            btnOK.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            btnOK.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            btnOK.BackgroundDraw = true;
            btnOK.ButtonColor = null;
            btnOK.ForeColor = null;
            btnOK.BorderColor = null;
            btnOK.Round = null;
            btnOK.Gradient = true;
            btnOK.Clickable = true;
            btnOK.Name = "btnOK";
            btnOK.X = 49F;
            btnOK.Y = 83F;
            btnOK.Width = 86F;
            btnOK.Height = 34F;
            btnOK.Visible = true;
            btnOK.Enabled = true;
            btnOK.Fill = false;
            btnOK.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region btnCancel
            btnCancel.IconString = "";
            btnCancel.IconSize = 12F;
            btnCancel.IconGap = 5F;
            btnCancel.IconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            btnCancel.Text = "취소";
            btnCancel.TextPadding = new Padding(0, 0, 0, 0);
            btnCancel.FontName = "NanumGothic";
            btnCancel.FontSize = 12F;
            btnCancel.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            btnCancel.ContentAlignment = Devinno.Skia.Design.DvContentAlignment.MiddleCenter;
            btnCancel.BackgroundDraw = true;
            btnCancel.ButtonColor = null;
            btnCancel.ForeColor = null;
            btnCancel.BorderColor = null;
            btnCancel.Round = null;
            btnCancel.Gradient = true;
            btnCancel.Clickable = true;
            btnCancel.Name = "btnCancel";
            btnCancel.X = 141F;
            btnCancel.Y = 83F;
            btnCancel.Width = 86F;
            btnCancel.Height = 34F;
            btnCancel.Visible = true;
            btnCancel.Enabled = true;
            btnCancel.Fill = false;
            btnCancel.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region sw
            sw.OnBoxColor = null;
            sw.OffBoxColor = null;
            sw.SwitchColor = null;
            sw.ForeColor = null;
            sw.OnIconString = "";
            sw.OnIconSize = 12F;
            sw.OnIconGap = 5F;
            sw.OnIconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            sw.OnText = "ON";
            sw.OnTextPadding = new Padding(0, 0, 0, 0);
            sw.OnFontName = "NanumGothic";
            sw.OnFontSize = 12F;
            sw.OnFontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            sw.OffIconString = "";
            sw.OffIconSize = 12F;
            sw.OffIconGap = 5F;
            sw.OffIconAlignment = Devinno.Skia.Design.DvTextIconAlignment.LeftRight;
            sw.OffText = "OFF";
            sw.OffTextPadding = new Padding(0, 0, 0, 0);
            sw.OffFontName = "NanumGothic";
            sw.OffFontSize = 12F;
            sw.OffFontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            sw.Corner = null;
            sw.SwitchPadding = 5F;
            sw.OnOff = false;
            sw.Name = "sw";
            sw.X = 3F;
            sw.Y = 3F;
            sw.Width = 224F;
            sw.Height = 64F;
            sw.Visible = true;
            sw.Enabled = true;
            sw.Fill = false;
            sw.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #endregion

            #region add
            Controls.Add(grid);
            grid.Controls.Add(btnOK, 1, 2);
            grid.Controls.Add(btnCancel, 2, 2);
            grid.Controls.Add(sw, 0, 0);
            #endregion
        }
    }
}

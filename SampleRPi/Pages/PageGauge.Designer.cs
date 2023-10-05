using System;
using Devinno.Skia.Utils;
using Devinno.Skia.Controls;
using Devinno.Skia.Containers;
using Devinno.Skia.Design;

namespace SampleRPi.Pages
{
    partial class PageGauge
    {
        #region declare
        private DvTableLayoutPanel tbl;
        private DvMeter meter;
        private DvGauge gauge;
        private DvKnob knob;
        private DvProgress prgsV;
        private DvSlider sldV;
        private DvRangeSlider rsldV;
        private DvProgress prgsH;
        private DvSlider sldH;
        private DvRangeSlider rsldH;
        private DvStepGauge step;
        #endregion

        public void InitializeComponent()
        {
            #region base
            UseMasterPage = true;
            BackgroundDraw = false;
            BackColor = null;
            BackgroundImage = null;
            AnimationType = null;
            Text = "Gauge";
            IconString = "";
            #endregion

            #region new
            tbl = new DvTableLayoutPanel { Name = nameof(tbl) };
            meter = new DvMeter { Name = nameof(meter) };
            gauge = new DvGauge { Name = nameof(gauge) };
            knob = new DvKnob { Name = nameof(knob) };
            prgsV = new DvProgress { Name = nameof(prgsV) };
            sldV = new DvSlider { Name = nameof(sldV) };
            rsldV = new DvRangeSlider { Name = nameof(rsldV) };
            prgsH = new DvProgress { Name = nameof(prgsH) };
            sldH = new DvSlider { Name = nameof(sldH) };
            rsldH = new DvRangeSlider { Name = nameof(rsldH) };
            step = new DvStepGauge { Name = nameof(step) };
            #endregion

            #region controls
            #region tbl
            tbl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 11F));
            tbl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 11F));
            tbl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 11F));
            tbl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 11F));
            tbl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 11F));
            tbl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 11F));
            tbl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 11F));
            tbl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 11F));
            tbl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 11F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 10F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 10F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 10F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 10F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 10F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 10F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 10F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 10F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 10F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 10F));
            tbl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 10F));
            tbl.Name = "tbl";
            tbl.X = 10F;
            tbl.Y = 60F;
            tbl.Width = 780F;
            tbl.Height = 410F;
            tbl.Visible = true;
            tbl.Enabled = true;
            tbl.Fill = true;
            tbl.Margin = new Padding(10, 60, 10, 10);
            #endregion
            #region meter
            meter.NeedleColor = null;
            meter.NeedlePointColor = null;
            meter.ForeColor = null;
            meter.Minimum = 0;
            meter.Maximum = 100;
            meter.Value = 0;
            meter.GraduationLarge = 10;
            meter.GraduationSmall = 2;
            meter.FormatString = "0";
            meter.StartAngle = 135F;
            meter.SweepAngle = 270F;
            meter.DrawText = true;
            meter.TextOffsetX = 0F;
            meter.TextOffsetY = 0F;
            meter.ValueFontName = "NanumGothic";
            meter.ValueFontSize = 18F;
            meter.ValueFontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            meter.RemarkFontName = "NanumGothic";
            meter.RemarkFontSize = 10F;
            meter.RemarkFontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            meter.Title = "";
            meter.TitleFontName = "NanumGothic";
            meter.TitleFontSize = 10F;
            meter.TitleFontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            meter.Gap = 5F;
            meter.Name = "meter";
            meter.X = 3F;
            meter.Y = 253F;
            meter.Width = 251.40002F;
            meter.Height = 154F;
            meter.Visible = true;
            meter.Enabled = true;
            meter.Fill = false;
            meter.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region gauge
            gauge.FillColor = null;
            gauge.EmptyColor = null;
            gauge.ForeColor = null;
            gauge.Minimum = 0;
            gauge.Maximum = 100;
            gauge.Value = 0;
            gauge.FormatString = "0";
            gauge.StartAngle = 135F;
            gauge.SweepAngle = 270F;
            gauge.ValueFontName = "NanumGothic";
            gauge.ValueFontSize = 18F;
            gauge.ValueFontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            gauge.Title = "";
            gauge.TitleFontName = "NanumGothic";
            gauge.TitleFontSize = 10F;
            gauge.TitleFontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            gauge.BarSize = 20F;
            gauge.BarPadding = 3F;
            gauge.Gap = 5F;
            gauge.Name = "gauge";
            gauge.X = 260.40002F;
            gauge.Y = 253F;
            gauge.Width = 251.39996F;
            gauge.Height = 154F;
            gauge.Visible = true;
            gauge.Enabled = true;
            gauge.Fill = false;
            gauge.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region knob
            knob.KnobColor = null;
            knob.KnobBackColor = null;
            knob.ForeColor = null;
            knob.FillColor = null;
            knob.OnLampColor = null;
            knob.OffLampColor = null;
            knob.Minimum = 0;
            knob.Maximum = 100;
            knob.Tick = null;
            knob.Value = 0;
            knob.StartAngle = 135F;
            knob.SweepAngle = 270F;
            knob.Gap = 5F;
            knob.KnobPadding = 7F;
            knob.Name = "knob";
            knob.X = 517.8F;
            knob.Y = 253F;
            knob.Width = 251.39996F;
            knob.Height = 154F;
            knob.Visible = true;
            knob.Enabled = true;
            knob.Fill = false;
            knob.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region prgsV
            prgsV.BoxColor = null;
            prgsV.BarColor = null;
            prgsV.ForeColor = null;
            prgsV.Minimum = 0;
            prgsV.Maximum = 100;
            prgsV.Value = 0;
            prgsV.Direction = Devinno.Skia.Design.DvDirectionHV.Vertical;
            prgsV.DrawText = false;
            prgsV.Reverse = false;
            prgsV.FormatString = "0";
            prgsV.FontName = "NanumGothic";
            prgsV.FontSize = 10F;
            prgsV.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            prgsV.BarPadding = 5F;
            prgsV.BarSize = 20;
            prgsV.Name = "prgsV";
            prgsV.X = 3F;
            prgsV.Y = 3F;
            prgsV.Width = 79.8F;
            prgsV.Height = 234F;
            prgsV.Visible = true;
            prgsV.Enabled = true;
            prgsV.Fill = true;
            prgsV.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region sldV
            sldV.BoxColor = null;
            sldV.BarColor = null;
            sldV.ForeColor = null;
            sldV.GraduationColor = null;
            sldV.CursorColor = null;
            sldV.Minimum = 0;
            sldV.Maximum = 100;
            sldV.Value = 0;
            sldV.Tick = null;
            sldV.Direction = Devinno.Skia.Design.DvDirectionHV.Vertical;
            sldV.Reverse = false;
            sldV.FormatString = "0";
            sldV.FontName = "NanumGothic";
            sldV.FontSize = 10F;
            sldV.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            sldV.CursorSize = 30F;
            sldV.BarSize = 20F;
            sldV.Name = "sldV";
            sldV.X = 88.8F;
            sldV.Y = 3F;
            sldV.Width = 79.8F;
            sldV.Height = 234F;
            sldV.Visible = true;
            sldV.Enabled = true;
            sldV.Fill = true;
            sldV.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region rsldV
            rsldV.BoxColor = null;
            rsldV.BarColor = null;
            rsldV.ForeColor = null;
            rsldV.GraduationColor = Util.FromArgb(255, 90, 90, 90);
            rsldV.CursorColor = null;
            rsldV.Minimum = 0;
            rsldV.Maximum = 100;
            rsldV.RangeStart = 0;
            rsldV.RangeEnd = 0;
            rsldV.Tick = 10;
            rsldV.Direction = Devinno.Skia.Design.DvDirectionHV.Vertical;
            rsldV.Reverse = false;
            rsldV.FormatString = "0";
            rsldV.FontName = "NanumGothic";
            rsldV.FontSize = 10F;
            rsldV.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            rsldV.CursorSize = 30F;
            rsldV.BarSize = 20F;
            rsldV.Name = "rsldV";
            rsldV.X = 174.6F;
            rsldV.Y = 3F;
            rsldV.Width = 79.80002F;
            rsldV.Height = 234F;
            rsldV.Visible = true;
            rsldV.Enabled = true;
            rsldV.Fill = false;
            rsldV.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region prgsH
            prgsH.BoxColor = null;
            prgsH.BarColor = null;
            prgsH.ForeColor = null;
            prgsH.Minimum = 0;
            prgsH.Maximum = 100;
            prgsH.Value = 0;
            prgsH.Direction = Devinno.Skia.Design.DvDirectionHV.Horizon;
            prgsH.DrawText = false;
            prgsH.Reverse = false;
            prgsH.FormatString = "0";
            prgsH.FontName = "NanumGothic";
            prgsH.FontSize = 10F;
            prgsH.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            prgsH.BarPadding = 5F;
            prgsH.BarSize = 20;
            prgsH.Name = "prgsH";
            prgsH.X = 260.40002F;
            prgsH.Y = 3F;
            prgsH.Width = 508.79993F;
            prgsH.Height = 34F;
            prgsH.Visible = true;
            prgsH.Enabled = true;
            prgsH.Fill = true;
            prgsH.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region sldH
            sldH.BoxColor = null;
            sldH.BarColor = null;
            sldH.ForeColor = null;
            sldH.GraduationColor = null;
            sldH.CursorColor = null;
            sldH.Minimum = 0;
            sldH.Maximum = 100;
            sldH.Value = 0;
            sldH.Tick = null;
            sldH.Direction = Devinno.Skia.Design.DvDirectionHV.Horizon;
            sldH.Reverse = false;
            sldH.FormatString = "0";
            sldH.FontName = "NanumGothic";
            sldH.FontSize = 10F;
            sldH.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            sldH.CursorSize = 30F;
            sldH.BarSize = 20F;
            sldH.Name = "sldH";
            sldH.X = 260.40002F;
            sldH.Y = 43F;
            sldH.Width = 508.79993F;
            sldH.Height = 74F;
            sldH.Visible = true;
            sldH.Enabled = true;
            sldH.Fill = false;
            sldH.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region rsldH
            rsldH.BoxColor = null;
            rsldH.BarColor = null;
            rsldH.ForeColor = null;
            rsldH.GraduationColor = Util.FromArgb(255, 90, 90, 90);
            rsldH.CursorColor = null;
            rsldH.Minimum = 0;
            rsldH.Maximum = 100;
            rsldH.RangeStart = 0;
            rsldH.RangeEnd = 0;
            rsldH.Tick = 10;
            rsldH.Direction = Devinno.Skia.Design.DvDirectionHV.Horizon;
            rsldH.Reverse = false;
            rsldH.FormatString = "0";
            rsldH.FontName = "NanumGothic";
            rsldH.FontSize = 10F;
            rsldH.FontStyle = Devinno.Skia.Design.DvFontStyle.Normal;
            rsldH.CursorSize = 30F;
            rsldH.BarSize = 20F;
            rsldH.Name = "rsldH";
            rsldH.X = 260.40002F;
            rsldH.Y = 123F;
            rsldH.Width = 508.79993F;
            rsldH.Height = 74F;
            rsldH.Visible = true;
            rsldH.Enabled = true;
            rsldH.Fill = true;
            rsldH.Margin = new Padding(3, 3, 3, 3);
            #endregion
            #region step
            step.OnColor = null;
            step.OffColor = null;
            step.ForeColor = null;
            step.ButtonColor = null;
            step.LeftIconString = "fa-chevron-left";
            step.LeftIconSize = 20F;
            step.RightIconString = "fa-chevron-right";
            step.RightIconSize = 20F;
            step.StepCount = 10;
            step.Step = 0;
            step.Gap = 10;
            step.UseButton = true;
            step.DrawButton = false;
            step.StepPadding = 3F;
            step.Name = "step";
            step.X = 260.40002F;
            step.Y = 207F;
            step.Width = 508.79993F;
            step.Height = 30F;
            step.Visible = true;
            step.Enabled = true;
            step.Fill = false;
            step.Margin = new Padding(3, 7, 3, 3);
            #endregion
            #endregion

            #region add
            Controls.Add(tbl);
            tbl.Controls.Add(meter, 0, 7, 3, 4);
            tbl.Controls.Add(gauge, 3, 7, 3, 4);
            tbl.Controls.Add(knob, 6, 7, 3, 4);
            tbl.Controls.Add(prgsV, 0, 0, 1, 6);
            tbl.Controls.Add(sldV, 1, 0, 1, 6);
            tbl.Controls.Add(rsldV, 2, 0, 1, 6);
            tbl.Controls.Add(prgsH, 3, 0, 6, 1);
            tbl.Controls.Add(sldH, 3, 1, 6, 2);
            tbl.Controls.Add(rsldH, 3, 3, 6, 2);
            tbl.Controls.Add(step, 3, 5, 6, 1);
            #endregion
        }
    }
}

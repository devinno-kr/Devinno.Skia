using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace SampleRPi.Pages
{
    public class  PageGauge : DvPage
    {
        #region Member Variable
        DvGauge gauge;
        DvMeter meter;
        DvKnob knob;

        DvProgressH progHr, progHg, progHb, progHn;
        DvProgressV progVr, progVg, progVb, progVn;
        DvSliderH sldH1, sldH2;
        DvSliderV sldV1, sldV2;
        DvStepGauge step;
        DvGridLayoutPanel gpnl;
        DvTableLayoutPanel tpnl;

        DateTime prev = DateTime.Now;
        #endregion

        #region Constructor
        public PageGauge()
        {
            UseMasterPage = true;

            #region ProgressV
            progVr = new DvProgressV() { Name = nameof(progVr), BarColor = SKColors.Red };
            progVg = new DvProgressV() { Name = nameof(progVg), BarColor = SKColors.Green };
            progVb = new DvProgressV() { Name = nameof(progVb), BarColor = SKColors.DodgerBlue };
            progVn = new DvProgressV() { Name = nameof(progVn), Reverse = true, DrawText = false };
            #endregion
            #region ProgressH
            progHr = new DvProgressH() { Name = nameof(progHr), BarColor = SKColors.Red };
            progHg = new DvProgressH() { Name = nameof(progHg), BarColor = SKColors.Green };
            progHb = new DvProgressH() { Name = nameof(progHb), BarColor = SKColors.DodgerBlue };
            progHn = new DvProgressH() { Name = nameof(progHn), Reverse = true, DrawText = false };
            #endregion
            #region SliderV
            sldV1 = new DvSliderV() { Name = nameof(sldV1) };
            sldV2 = new DvSliderV() { Name = nameof(sldV2), Tick = 10, DrawText = false, Reverse = true };
            #endregion
            #region SliderH
            sldH1 = new DvSliderH() { Name = nameof(sldH1) };
            sldH2 = new DvSliderH() { Name = nameof(sldH2), Tick = 10, DrawText = false, Reverse = true };
            #endregion
            #region StepGauge
            step = new DvStepGauge() { Name = nameof(step), StepCount = 7,};
            #endregion
            #region Gauge
            gauge = new DvGauge() { Name = nameof(gauge), Gradient = true, Unit = "PERCENT" };
            #endregion
            #region Meter
            meter = new DvMeter() { Name = nameof(meter), Unit = "PERCENT" };
            meter.Bars.Add(new MeterBar(0, 70, SKColors.Green));
            meter.Bars.Add(new MeterBar(70, 90, SKColors.Orange));
            meter.Bars.Add(new MeterBar(90, 100, SKColors.Red));
            #endregion
            #region Knob
            knob = new DvKnob() { Name = nameof(knob), Gradient = true, Unit = "PERCENT" };
            #endregion
            #region phpnl
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Fill = true };

            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 8.3F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 8.3F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 8.3F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 8.3F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 8.3F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 8.3F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 8.3F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 8.3F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 8.3F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 8.3F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 8.3F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 8.3F });

            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 5F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 5F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 5F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 5F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Pixel, Size = 10F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 20F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Pixel, Size = 10F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 6F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 6F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Pixel, Size = 10F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 34F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 14F });

            #endregion
            #region gpnl
            gpnl = new DvGridLayoutPanel() { Name = nameof(gpnl), Fill = true, Margin = MainWindow.BaseMargin };
            gpnl.Rows.Add(new DvGridRow() { Mode = SizeMode.Percent, Size = 40 });
            gpnl.Rows.Add(new DvGridRow() { Mode = SizeMode.Pixel, Size = 10 });
            gpnl.Rows.Add(new DvGridRow() { Mode = SizeMode.Percent, Size = 60 });

            gpnl.Rows[0].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 100F });

            gpnl.Rows[2].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 32F });
            gpnl.Rows[2].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 2F });
            gpnl.Rows[2].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 32F });
            gpnl.Rows[2].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 2F });
            gpnl.Rows[2].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 32F });
            #endregion

            gauge.Unit = "%";

            #region Controls.Add
            gpnl.Controls.Add(tpnl, 0, 0);
            gpnl.Controls.Add(gauge, 0, 2);
            gpnl.Controls.Add(meter, 2, 2);
            gpnl.Controls.Add(knob, 4, 2);

            tpnl.Controls.Add(progVr, 0, 0, 1, 12);
            tpnl.Controls.Add(progVg, 1, 0, 1, 12);
            tpnl.Controls.Add(progVb, 2, 0, 1, 12);
            tpnl.Controls.Add(progVn, 3, 0, 1, 12);
            tpnl.Controls.Add(progHr, 5, 0, 1, 3);
            tpnl.Controls.Add(progHg, 5, 3, 1, 3);
            tpnl.Controls.Add(progHb, 5, 6, 1, 3);
            tpnl.Controls.Add(progHn, 5, 9, 1, 3);
            tpnl.Controls.Add(sldV1, 7, 0, 1, 12);
            tpnl.Controls.Add(sldV2, 8, 0, 1, 12);
            tpnl.Controls.Add(sldH1, 10, 0, 1, 4);
            tpnl.Controls.Add(sldH2, 10, 4, 1, 4);
            tpnl.Controls.Add(step, 10, 8, 2, 4);

            Controls.Add(gpnl);
            #endregion
        }
        #endregion

        #region Method
        #region Tick
        public void Tick()
        {
            var v1 = Math.Abs(((DateTime.Now - prev).TotalMilliseconds / 5) % 200 - 100);
            var v2 = Math.Abs(((DateTime.Now - prev).TotalMilliseconds / 5 + 33) % 200 - 100);
            var v3 = Math.Abs(((DateTime.Now - prev).TotalMilliseconds / 5 + 66) % 200 - 100);

            progHr.Value = progVr.Value = v1;
            progHg.Value = progVg.Value = v1;
            progHb.Value = progVb.Value = v1;
            progHn.Value = progVn.Value = v1;
            meter.Value = gauge.Value = v1;
        }
        #endregion
        #endregion
    }
}

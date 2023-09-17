using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Dialogs;
using Devinno.Skia.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleRPi.Pages
{
    public class PageControl : DvPage
    {

        public PageControl()
        {
            UseMasterPage = true;

            Controls.Add(new DvDataGrid { Name = "dg", Bounds = Util.FromRect(20, 100, 400, 300), });

            Controls.Add(new DvButtons { Name = "btns", Bounds = Util.FromRect(420, 100, 150, 30) });
        }

        /*
        DvAnimate
        DvBarGraph
        DvButton
        DvButtons
        DvCalendar
        DvCheckBox
        DvCircleButton
        DvCircleGraph
        DvColorPicker
        DvComboBox
        DvDataGrid
        DvDataGrid.Designer
        DvDateTimePicker
        DvGauge
        DvInput
        DvKnob
        DvLabel
        DvLamp
        DvLampButton
        DvLineGraph
        DvListBox
        DvMeter
        DvNavBar
        DvNumberBox
        DvOnOff
        DvPictureBox
        DvProgress
        DvRadioBox
        DvRadioButton
        DvRangeSlider
        DvSelector
        DvSlider
        DvStepGauge
        DvSwitch
        DvTimeGraph
        DvToggleButton
        DvToolBox
        DvTreeView
        DvTrendGraph
        DvTriangleButton
        DvValueInput
        DvValueLabel
        DvWheelPicker
        */
    }
}

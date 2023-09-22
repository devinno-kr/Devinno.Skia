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

            var tv = new DvTreeView { Name = "tree", Bounds = Util.FromRect(20, 100, 400, 300), };
            Controls.Add(tv);

            var nd1 = new TreeViewNode { Text = "T1" };
            var nd2 = new TreeViewNode { Text = "T2" };
            var nd3 = new TreeViewNode { Text = "T3" };

            nd1.Nodes.Add(new TreeViewNode { Text = "T1.1" });
            nd1.Nodes.Add(new TreeViewNode { Text = "T1.2" });
            nd1.Nodes.Add(new TreeViewNode { Text = "T1.3" });

            nd2.Nodes.Add(new TreeViewNode { Text = "T2.1" });
            nd2.Nodes.Add(new TreeViewNode { Text = "T2.2" });

            nd3.Nodes.Add(new TreeViewNode { Text = "T3.1" });

            nd1.Nodes[0].Nodes.Add(new TreeViewNode { Text = "T1.1.1" });
            nd1.Nodes[0].Nodes.Add(new TreeViewNode { Text = "T1.1.2" });

            tv.Nodes.Add(nd1);
            tv.Nodes.Add(nd2);
            tv.Nodes.Add(nd3);

            tv.Nodes.Reset();

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

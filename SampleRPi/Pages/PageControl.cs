using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleRPi.Pages
{
    public class PageControl : DvPage
    {
        #region Member Variable
        private DvButton btnFlat, btnGrad, btnIcon;
        private DvLabel lblFCV, lblFCC, lblCC, lblCV;
        private DvTriangleButton tbtnUp, tbtnDown, tbtnLeft, tbtnRight;
        private DvCircleButton cbtnPrev, cbtnNext;
        private DvCheckBox chk1, chk2, chk3;
        private DvRadioBox rad1, rad2, rad3;
        private DvToggleButton tgl1, tgl2, tgl3;
        private DvRadioButton rbtn1, rbtn2, rbtn3;
        private DvSwitch sw;
        private DvOnOff onoff;
        private DvLamp lmpR, lmpG, lmpB;
        private DvNumberBox num1, num2, num3;
        private DvSelector selector;
        private DvValueLabel vlbl;
        private DvValueLabelButton vblbl;
        
        private DvTableLayoutPanel tpnl;
        private DvGridLayoutPanel lmppnl;
        private DvTableLayoutPanel trbpnl;
        #endregion

        #region Constructor
        public PageControl() 
        {
            UseMasterPage = true;

            #region Button
            btnFlat = new DvButton() { Name = nameof(btnFlat), Text = "FLAT", Gradient = false };
            btnGrad = new DvButton() { Name = nameof(btnGrad), Text = "GRADIENT" };
            btnIcon = new DvButton() { Name = nameof(btnIcon), Text = "ICON", IconString = "fa-cube", IconAlignment = DvTextIconAlignment.TopBottom };
            #endregion
            #region Label
            lblFCV = new DvLabel() { Name = nameof(lblFCV), Text = "FLAT CONVEX", Style = DvLabelStyle.FlatConvex };
            lblFCC = new DvLabel() { Name = nameof(lblFCC), Text = "FLAT CONCAVE", Style = DvLabelStyle.FlatConcave };
            lblCV = new DvLabel() { Name = nameof(lblCV), Text = "CONVEX", Style = DvLabelStyle.Convex };
            lblCC = new DvLabel() { Name = nameof(lblCC), Text = "CONCAVE", Style = DvLabelStyle.Concave };
            #endregion
            #region TriagleButton
            tbtnUp = new DvTriangleButton() { Name = nameof(tbtnUp), Direction = DvDirection.Up };
            tbtnDown = new DvTriangleButton() { Name = nameof(tbtnDown), Direction = DvDirection.Down };
            tbtnLeft = new DvTriangleButton() { Name = nameof(tbtnLeft), Direction = DvDirection.Left };
            tbtnRight = new DvTriangleButton() { Name = nameof(tbtnRight), Direction = DvDirection.Right };
            #endregion
            #region CircleButton
            cbtnPrev = new DvCircleButton() { Name = nameof(cbtnPrev), IconString = "fa-chevron-left", IconSize = 24, Text = "" };
            cbtnNext = new DvCircleButton() { Name = nameof(cbtnNext), IconString = "fa-chevron-right", IconSize = 24, Text = "" };
            #endregion
            #region CheckBox
            chk1 = new DvCheckBox() { Name = nameof(chk1), Text = "CHECK 1" };
            chk2 = new DvCheckBox() { Name = nameof(chk2), Text = "CHECK 2" };
            chk3 = new DvCheckBox() { Name = nameof(chk3), Text = "CHECK 3" };
            #endregion
            #region RadioBox
            rad1 = new DvRadioBox() { Name = nameof(rad1), Text = "RADIO 1" };
            rad2 = new DvRadioBox() { Name = nameof(rad2), Text = "RADIO 2" };
            rad3 = new DvRadioBox() { Name = nameof(rad3), Text = "RADIO 3" };
            #endregion
            #region ToggleButton
            tgl1 = new DvToggleButton() { Name = nameof(tgl1), Text = "TOGGLE 1" };
            tgl2 = new DvToggleButton() { Name = nameof(tgl2), Text = "TOGGLE 2" };
            tgl3 = new DvToggleButton() { Name = nameof(tgl3), Text = "TOGGLE 3" };
            #endregion
            #region RadioButton
            rbtn1 = new DvRadioButton() { Name = nameof(rbtn1), Text = "RADIO 1" };
            rbtn2 = new DvRadioButton() { Name = nameof(rbtn2), Text = "RADIO 2" };
            rbtn3 = new DvRadioButton() { Name = nameof(rbtn3), Text = "RADIO 3" };
            #endregion
            #region Switch
            sw = new DvSwitch() { Name = nameof(sw) };
            #endregion
            #region OnOff
            onoff = new DvOnOff() { Name = nameof(onoff) };
            #endregion
            #region Lamp
            lmpR = new DvLamp() { Name = nameof(lmpR), LampSize = 28, OnLampColor = SKColors.Red, Text = "RED", ContentAlignment = DvContentAlignment.MiddleLeft };
            lmpG = new DvLamp() { Name = nameof(lmpG), LampSize = 28, OnLampColor = SKColors.LimeGreen, Text = "GREEN", ContentAlignment = DvContentAlignment.MiddleLeft };
            lmpB = new DvLamp() { Name = nameof(lmpB), LampSize = 28, OnLampColor = SKColors.DodgerBlue, Text = "BLUE", ContentAlignment = DvContentAlignment.MiddleLeft };
            #endregion
            #region NumberBox
            num1 = new DvNumberBox() { Name = nameof(num1), Style = DvNumberBoxStyle.LeftRight, ButtonWidth = 30 };
            num2 = new DvNumberBox() { Name = nameof(num2), Style = DvNumberBoxStyle.Right, ButtonWidth = 30 };
            num3 = new DvNumberBox() { Name = nameof(num3), Style = DvNumberBoxStyle.UpDown, ButtonWidth = 30 };
            #endregion
            #region Selector
            selector = new DvSelector() { Name = nameof(selector), ButtonWidth=30 };
            for (int i = 1; i <= 6; i++) selector.Items.Add(new TextIconItem() { Text = "ITEM " + i, IconString = "fa-tag" });
            selector.SelectedIndex = 0;
            #endregion
            #region ValueLabel
            vlbl = new DvValueLabel() { Name = nameof(vlbl), Title = "온도", Value = "36.5", Unit = "℃" };
            #endregion
            #region ValueLabelButton
            vblbl = new DvValueLabelButton() { Name = nameof(vblbl), Title = "거리", Value = "1000", Unit = "㎜", ButtonText = "SET", ButtonWidth = 60 };
            #endregion
            #region tpnl
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Margin = MainWindow.BaseMargin, Fill = true };

            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Pixel, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Pixel, Size = 10F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 15F });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 15F });

            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 16F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 16F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 2F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 16F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 16F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 2F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 16F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 16F });

            Controls.Add(tpnl);
            #endregion
            #region trpnl
            trbpnl = new DvTableLayoutPanel() { Name = nameof(trbpnl) };

            trbpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 25F });
            trbpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 25F });
            trbpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 25F });
            trbpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 25F });

            trbpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 33.33F });
            trbpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 33.34F });
            trbpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 33.33F });
            #endregion
            #region lmppnl
            lmppnl = new DvGridLayoutPanel() { Name = nameof(lmppnl) };
            lmppnl.Rows.Add(new DvGridRow() { Mode = SizeMode.Percent, Size = 100 });
            lmppnl.Rows[0].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 30 });
            lmppnl.Rows[0].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 40 });
            lmppnl.Rows[0].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 30 });
            #endregion

            #region Controls.Add
            trbpnl.Controls.Add(tbtnUp, 1, 0, 1, 2);
            trbpnl.Controls.Add(tbtnDown, 1, 2, 1, 2);
            trbpnl.Controls.Add(tbtnLeft, 0, 1, 1, 2);
            trbpnl.Controls.Add(tbtnRight, 2, 1, 1, 2);

            lmppnl.Controls.Add(lmpR, 0, 0);
            lmppnl.Controls.Add(lmpG, 1, 0);
            lmppnl.Controls.Add(lmpB, 2, 0);

            tpnl.Controls.Add(btnFlat, 0, 0);
            tpnl.Controls.Add(btnGrad, 0, 1);
            tpnl.Controls.Add(btnIcon, 0, 2, 1, 2);
            tpnl.Controls.Add(lblFCV, 1, 0);
            tpnl.Controls.Add(lblFCC, 1, 1);
            tpnl.Controls.Add(lblCV, 1, 2);
            tpnl.Controls.Add(lblCC, 1, 3);
            tpnl.Controls.Add(trbpnl, 6, 0, 2, 4);
            tpnl.Controls.Add(chk1, 0, 5);
            tpnl.Controls.Add(chk2, 0, 6);
            tpnl.Controls.Add(chk3, 0, 7);
            tpnl.Controls.Add(rad1, 1, 5);
            tpnl.Controls.Add(rad2, 1, 6);
            tpnl.Controls.Add(rad3, 1, 7);
            tpnl.Controls.Add(sw, 3, 0, 2, 2);
            tpnl.Controls.Add(onoff, 3, 2, 2, 2);
            tpnl.Controls.Add(tgl1, 3, 5);
            tpnl.Controls.Add(tgl2, 3, 6);
            tpnl.Controls.Add(tgl3, 3, 7);
            tpnl.Controls.Add(rbtn1, 4, 5);
            tpnl.Controls.Add(rbtn2, 4, 6);
            tpnl.Controls.Add(rbtn3, 4, 7);
            tpnl.Controls.Add(cbtnPrev, 6, 5, 1, 3);
            tpnl.Controls.Add(cbtnNext, 7, 5, 1, 3);
            tpnl.Controls.Add(lmppnl, 0, 9, 2, 1);
            tpnl.Controls.Add(selector, 0, 10, 2, 1);
            tpnl.Controls.Add(num1, 3, 9);
            tpnl.Controls.Add(num2, 3, 10);
            tpnl.Controls.Add(num3, 4, 9, 1, 2);
            tpnl.Controls.Add(vlbl, 6, 9, 2, 1);
            tpnl.Controls.Add(vblbl, 6, 10, 2, 1);
            #endregion

            #region Event
            lmpR.MouseClick += (o, s) => lmpR.OnOff = !lmpR.OnOff;
            lmpG.MouseClick += (o, s) => lmpG.OnOff = !lmpG.OnOff;
            lmpB.MouseClick += (o, s) => lmpB.OnOff = !lmpB.OnOff;
            #endregion

            lblFCV.Text = "한글입력기        ";

        }
        #endregion

    }
}

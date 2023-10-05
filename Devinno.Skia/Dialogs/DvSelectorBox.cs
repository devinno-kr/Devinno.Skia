using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Dialogs
{
    public class DvSelectorBox : DvWindow
    {
        #region Properties
        public int ColumnCount { get; set; } = 1;
        public int ItemWidth { get; set; } = 150;
        public int ItemHeight { get; set; } = 40;
        public int ItemViewCount { get; set; } = 5;

        public int? DialogWidth { get; set; } = null;
        public int? DialogHeight { get; set; } = null;
        #endregion

        #region Member Variable
        AutoResetEvent ev = new AutoResetEvent(false);

        DvButton btnOK, btnCancel;
        DvGridLayoutPanel gpnl;
        DvTableLayoutPanel tpnl;
        DvSelector sels;
        DvWheelPicker wheel;

        bool bOK = false;
        #endregion

        #region Constructor
        public DvSelectorBox()
        {
            IconString = "fa-check";
            Width = 300;
            Height = 300;

            #region New
            #region gpnl
            gpnl = new DvGridLayoutPanel { Name = nameof(gpnl), Fill = true, Margin = new Padding(10, TitleHeight + 10, 10, 10) };
            gpnl.Rows.Add(new DvGridRow(DvSizeMode.Percent, 100));
            gpnl.Rows.Add(new DvGridRow(DvSizeMode.Pixel, 46));
            gpnl.Rows[0].Columns.Add(new SizeInfo(DvSizeMode.Percent, 100));
            gpnl.Rows[1].Columns.Add(new SizeInfo(DvSizeMode.Percent, 50));
            gpnl.Rows[1].Columns.Add(new SizeInfo(DvSizeMode.Percent, 50));
            #endregion
            #region tpnl
            tpnl = new DvTableLayoutPanel { Name = nameof(tpnl), Fill = true, Margin = new Padding(0) };
            #endregion
            #region Control
            sels = new DvSelector { Name = nameof(sels), Fill = true, BackgroundDraw = false };
            wheel = new DvWheelPicker { Name = nameof(wheel), Fill = true };
            btnOK = new DvButton { Name = nameof(btnOK), Fill = true, Text = "확인", Gradient = true };
            btnCancel = new DvButton { Name = nameof(btnCancel), Fill = true, Text = "취소", Gradient = true };
            #endregion
            #endregion

            #region Controls.Add
            gpnl.Controls.Add(tpnl, 0, 0);
            gpnl.Controls.Add(btnOK, 0, 1);
            gpnl.Controls.Add(btnCancel, 1, 1);

            Controls.Add(gpnl);
            #endregion

            #region Event
            btnOK.MouseClick += (o, s) => { bOK = true; Hide(); };
            btnCancel.MouseClick += (o, s) => { bOK = false; Hide(); };
            #endregion
        }
        #endregion

        #region Override
        #region OnExitButtonClick
        protected override void OnExitButtonClick()
        {
            bOK = false;
        }
        #endregion

        #region Show
        public override void Show()
        {
            base.Show();

        }
        #endregion
        #region Hide
        public override void Hide()
        {
            base.Hide();

            ev.Set();
        }
        #endregion
        #endregion

        #region Method
        #region show
        void show(string Title, Action actLayout, Action actOK)
        {
            this.Title = Title;

            actLayout();

            Task.Run(() => {

                this.Show();
                ev.WaitOne();
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    Thread.Sleep(DvDesign.HIDE_TIME);
                    actOK();
                });

            });
        }
        #endregion
        #region ShowSelector
        public void ShowSelector(string Title, SelectorItem SelectedItem, List<SelectorItem> Items, Action<SelectorItem> result)
        {
            show(Title, () =>
            {
                #region Size
                this.Width = DialogWidth ?? Math.Max(200, 10 + 50 + ItemWidth + 50 + 10);
                this.Height = DialogHeight ?? TitleHeight + 20 + (ItemHeight + 6) + 46 + 20;
                #endregion
                #region Layout
                tpnl.Rows.Clear();
                tpnl.Columns.Clear();
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 10));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 100));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 10));
                tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 100));

                tpnl.Controls.Clear();
                tpnl.Controls.Add(sels, 0, 1);
                #endregion
                #region Selector
                var SelectedIndex = Items.IndexOf(SelectedItem);
                sels.Items.Clear();
                sels.Items.AddRange(Items);
                sels.SelectedIndex = SelectedIndex == -1 ? 0 : SelectedIndex;
                #endregion

            }, () =>
            {
                if (bOK) result(Items[sels.SelectedIndex]);
                else result(null);
            });
        }
        #endregion
        #region ShowWheel
        public void ShowWheel(string Title, DvTextIcon SelectedItem, List<DvTextIcon> Items, Action<DvTextIcon> result)
        {
            show(Title, () =>
            {
                #region Size
                this.Width = DialogWidth ?? Math.Max(200, 10 + 50 + ItemWidth + 50 + 10);
                this.Height = DialogHeight ?? TitleHeight + 20 + ((ItemHeight * Math.Min(ItemViewCount, Items.Count)) + 6) + 46 + 20;
                #endregion
                #region Layout
                tpnl.Rows.Clear();
                tpnl.Columns.Clear();
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 10));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 100));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 10));
                tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 100));

                tpnl.Controls.Clear();
                tpnl.Controls.Add(wheel, 0, 1);
                #endregion
                #region Selector
                var SelectedIndex = Items.IndexOf(SelectedItem);
                wheel.Items.Clear();
                wheel.Items.AddRange(Items);
                wheel.SelectedIndex = SelectedIndex == -1 ? 0 : SelectedIndex;
                #endregion

            }, () =>
            {
                if (bOK) result(Items[wheel.SelectedIndex]);
                else result(null);
            });
        }
        #endregion
        #region ShowRadio
        public void ShowRadio(string Title, DvTextIcon SelectedItem, List<DvTextIcon> Items, Action<DvTextIcon> result)
        {
            show(Title, () =>
            {
                #region Size
                var RowCount = Convert.ToInt32(Math.Ceiling((double)Items.Count / (double)ColumnCount));
                var csz = 100F / ColumnCount;
                var rsz = 100F / RowCount;

                this.Width = DialogWidth ?? Math.Max(200, 10 + 50 + (ItemWidth * ColumnCount) + 50 + 10);
                this.Height = DialogHeight ?? TitleHeight + 20 + (ItemHeight * RowCount) + 46 + 20;
                #endregion
                #region Layout
                tpnl.Rows.Clear();
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 10));
                for (int i = 0; i < RowCount; i++) tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, rsz));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 10));

                tpnl.Columns.Clear();
                for (int i = 0; i < RowCount; i++) tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, csz));

                tpnl.Controls.Clear();
                for (int row = 0, i = 0; row < RowCount; row++)
                {
                    for (int col = 0; col < ColumnCount; col++, i++)
                    {
                        if (i < Items.Count)
                        {
                            var c = new DvRadioBox()
                            {
                                Name = "rad" + col + "_" + row,
                                Fill = true,
                                Checked = Items[i] == SelectedItem,
                                Text = Items[i].Text,
                                Tag = Items[i],
                                ContentAlignment= DvContentAlignment.MiddleCenter
                            };
                            tpnl.Controls.Add(c, col, row + 1);
                        }
                    }
                }
                #endregion

            }, () =>
            {
                if (bOK)
                {
                    var  v = tpnl.Controls.Values.Where(x => x is DvRadioBox && ((DvRadioBox)x).Checked).FirstOrDefault() as DvRadioBox;
                    if (v != null)
                    {
                        var val = v.Tag as DvTextIcon;
                        result(val);
                    }
                    else result(null);
                }
                else result(null);
            });
        }
        #endregion
        #region ShowCheck
        public void ShowCheck(string Title, List<DvTextIcon> SelectedItems, List<DvTextIcon> Items, Action<List<DvTextIcon>> result)
        {
            show(Title, () =>
            {
                #region Size
                var RowCount = Convert.ToInt32(Math.Ceiling((double)Items.Count / (double)ColumnCount));
                var csz = 100F / ColumnCount;
                var rsz = 100F / RowCount;

                this.Width = DialogWidth ?? Math.Max(200, 10 + 50 + (ItemWidth * ColumnCount) + 50 + 10);
                this.Height = DialogHeight ?? TitleHeight + 20 + (ItemHeight * RowCount) + 46 + 20;
                #endregion
                #region Layout
                tpnl.Rows.Clear();
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 10));
                for (int i = 0; i < RowCount; i++) tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, rsz));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 10));

                tpnl.Columns.Clear();
                for (int i = 0; i < RowCount; i++) tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, csz));

                tpnl.Controls.Clear();
                for (int row = 0, i = 0; row < RowCount; row++)
                {
                    for (int col = 0; col < ColumnCount; col++, i++)
                    {
                        if (i < Items.Count)
                        {
                            var c = new DvCheckBox()
                            {
                                Name = "rad" + col + "_" + row,
                                Fill = true,
                                Checked = SelectedItems.Contains(Items[i]),
                                Text = Items[i].Text,
                                Tag = Items[i],
                                ContentAlignment = DvContentAlignment.MiddleCenter
                            };
                            tpnl.Controls.Add(c, col, row + 1);
                        }
                    }
                }
                #endregion

            }, () =>
            {
                if (bOK)
                {
                    var vls = tpnl.Controls.Values.Where(x => x is DvCheckBox && ((DvCheckBox)x).Checked).Select(x => x as DvCheckBox).ToList();
                    if (vls != null)
                    {
                        result(vls.Select(x => x.Tag as DvTextIcon).ToList());
                    }
                    else result(null);
                }
                else result(null);
            });
        }
        #endregion
        #endregion
    }
}

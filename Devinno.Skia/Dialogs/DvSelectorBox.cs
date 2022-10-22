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
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Dialogs
{
    public class DvSelectorBox : DvWindow
    {
        #region Properties
        public int ButtonHeight { get; set; } = 30;
        public int ItemViewCount { get; set; } = 5;
        public int ItemHeight
        {
            get => sels.ItemHeight;
            set => sels.ItemHeight = value;
        }

        public int MinWidth { get; set; } = 200;
        public int MinHeight { get; set; } = 140;
        #endregion

        #region Member Variable
        AutoResetEvent ev = new AutoResetEvent(false);

        DvButton btnOK, btnCancel;
        DvGridLayoutPanel gpnl;
        DvWheelPicker sels;

        bool bOK = false;
        #endregion

        #region Constructor
        public DvSelectorBox()
        {
            IconString = "fa-check";
            Width = 300;
            Height = 300;

            gpnl = new DvGridLayoutPanel() { Name = nameof(gpnl), Fill = true, Margin = new Padding(10) };
            gpnl.Rows.Add(new DvGridRow() { Mode = SizeMode.Percent, Size = 100 });
            gpnl.Rows.Add(new DvGridRow() { Mode = SizeMode.Pixel, Size = 36 });
            gpnl.Rows[0].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 100 });
            gpnl.Rows[1].Columns.Add(new DvGridColumn() { Mode = SizeMode.Percent, Size = 100 });
            gpnl.Rows[1].Columns.Add(new DvGridColumn() { Mode = SizeMode.Pixel, Size = 80 });
            gpnl.Rows[1].Columns.Add(new DvGridColumn() { Mode = SizeMode.Pixel, Size = 80 });
            Controls.Add(gpnl);

            btnOK = new DvButton { Name = nameof(btnOK), Text = "확인" };
            btnCancel = new DvButton { Name = nameof(btnCancel), Text = "취소" };
            sels = new DvWheelPicker { Name = nameof(sels) };

            gpnl.Controls.Add(sels, 0, 0);
            gpnl.Controls.Add(btnOK, 1, 1);
            gpnl.Controls.Add(btnCancel, 2, 1);

            btnOK.MouseClick += (o, s) => { bOK = true; Hide(); };
            btnCancel.MouseClick += (o, s) => { bOK = false; Hide(); };
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
        void show(string Title, Action act1, Action act2)
        {
            gpnl.Rows[1].Size = ButtonHeight + 6;

            this.Title = Title;

            act1();

            Task.Run(() => {

                this.Show();
                ev.WaitOne();
                ThreadPool.QueueUserWorkItem((o) => { Thread.Sleep(DvDesign.HIDE_TIME); act2(); });

            });
        }
        #endregion
        #region ShowSelector
        public void ShowSelector(string Title, int SelectedIndex, List<TextIconItem> Items, Action<int?> result)
        {
            show(Title, () =>
            {
                #region Size
                int w = 0;
                using (var p = new SKPaint())
                {
                    var v = sels.Items.OrderByDescending(x => x.Text.Length).FirstOrDefault();

                    if (v != null) w = Convert.ToInt32(Util.MeasureTextIcon(v.Text, sels.FontName, sels.FontSize, DvFontStyle.Normal, sels.IconGap, v.IconString, sels.IconSize, sels.IconAlignment).Width);
                }

                this.Width = Math.Max(MinWidth, w + 20 + 40);
                this.Height = Math.Max(MinHeight, TitleHeight + 20 + 36 + (sels.ItemHeight * ItemViewCount) + 6 + 2);
                #endregion

                sels.Items.Clear();
                sels.Items.AddRange(Items);
                sels.SelectedIndex = SelectedIndex == -1 ? 0 : SelectedIndex;

            }, () =>
            {
                if (bOK) result(sels.SelectedIndex);
                else result(null);
            });
        }
        #endregion
        #endregion
    }
}

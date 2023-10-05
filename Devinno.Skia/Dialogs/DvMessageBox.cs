using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Dialogs
{
    public class DvMessageBox : DvWindow
    {
        #region Properties
        public int? DialogWidth { get; set; } = null;
        public int? DialogHeight { get; set; } = null;

        public int ButtonHeight { get; set; } = 40;
        public int ButtonTextSize { get; set; } = 12;
        public int MessageTextSize { get; set; } = 12;
        #endregion

        #region Member Variable
        AutoResetEvent ev = new AutoResetEvent(false);

        DvButton btnOK;
        DvButton btnCancel;
        DvButton btnYes;
        DvButton btnNo;
        DvLabel lbl;

        DvTableLayoutPanel tpnl;

        DialogResult result = DialogResult.Cancel;
        #endregion

        #region Constructor
        public DvMessageBox()
        {
            IconString = "fa-comment-dots";
            #region tpnl
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Margin = new Padding(10, TitleHeight + 10, 10, 10), Fill = true };

            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 100));
            tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 40));

            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 8F));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 14F));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 14F));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 14F));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 14F));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 14F));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 14F));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 8F));

            Controls.Add(tpnl);
            #endregion

            lbl = new DvLabel() { Name = nameof(lbl), BackgroundDraw = false, Margin = new Padding(6), ContentAlignment = DvContentAlignment.MiddleCenter };
            btnOK = new DvButton { Name = nameof(btnOK), Text = "확인", Margin = new Padding(6), Gradient = true };
            btnCancel = new DvButton { Name = nameof(btnCancel), Text = "취소", Margin = new Padding(6), Gradient = true };
            btnYes = new DvButton { Name = nameof(btnYes), Text = "예", Margin = new Padding(6), Gradient = true };
            btnNo = new DvButton { Name = nameof(btnNo), Text = "아니요", Margin = new Padding(6), Gradient = true };

            btnOK.MouseClick += (o, s) => { result = DialogResult.OK; Hide(); };
            btnCancel.MouseClick += (o, s) => { result = DialogResult.Cancel; Hide(); };
            btnYes.MouseClick += (o, s) => { result = DialogResult.Yes; Hide(); };
            btnNo.MouseClick += (o, s) => { result = DialogResult.No; Hide(); };
        }
        #endregion

        #region Override
        #region OnExitButtonClick
        protected override void OnExitButtonClick()
        {
            result = DialogResult.Cancel;
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
        void show(string Title, string Message, int btncnt, Action actLayout, Action<DialogResult> actOK)
        {
            #region Size
            var GAP = 10;

            var sz = Util.MeasureText(Message, lbl.FontName, lbl.FontSize, DvFontStyle.Normal);
            var btnSZ = Convert.ToInt32(ButtonHeight) + (btnOK.Margin.Top + btnOK.Margin.Bottom);
            var gapW = tpnl.Margin.Left + tpnl.Margin.Right + lbl.Margin.Left + lbl.Margin.Right;
            var gapH = tpnl.Margin.Top + tpnl.Margin.Bottom + lbl.Margin.Top + lbl.Margin.Bottom;
            var w = gapW + Convert.ToInt32(sz.Width) + (GAP * 2);
            var w2 = (btncnt * 80) + ((btncnt - 1) * 6) + gapW;
            var h = TitleHeight + gapH + btnSZ + Convert.ToInt32(sz.Height) + (GAP * 2);

            tpnl.Rows[1].Size = btnSZ;
            Width = DialogWidth ?? Math.Max(Math.Max(w, w2), 200);
            Height = DialogHeight ?? h;
            #endregion
            #region Control Set
            lbl.FontSize = MessageTextSize;
            btnOK.FontSize = btnCancel.FontSize = btnYes.FontSize = btnNo.FontSize = ButtonTextSize;

            this.Title = Title;
            lbl.Text = Message;
            #endregion

            actLayout();

            Task.Run(() => {

                this.Show();
                ev.WaitOne();
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    Thread.Sleep(DvDesign.HIDE_TIME);
                    actOK(this.result);
                });

            });
        }
        #endregion
        #region ShowMessageBoxOk
        public void ShowMessageBoxOk(string Title, string Message, Action<DialogResult> result)
        {
            show(Title, Message, 1, () =>
            {
                tpnl.Controls.Clear();
                tpnl.Controls.Add(lbl, 0, 0, 8, 1);
                tpnl.Controls.Add(btnOK, 1, 1, 6, 1);
            }, result);
        }
        #endregion
        #region ShowMessageBoxYesNo
        public void ShowMessageBoxYesNo(string Title, string Message, Action<DialogResult> result)
        {
            show(Title, Message, 2, () =>
             {
                 tpnl.Controls.Clear();
                 tpnl.Controls.Add(lbl, 0, 0, 8, 1);
                 tpnl.Controls.Add(btnYes, 1, 1, 3, 1);
                 tpnl.Controls.Add(btnNo, 4, 1, 3, 1);
             }, result);
        }
        #endregion
        #region ShowMessageBoxOkCancel
        public void ShowMessageBoxOkCancel(string Title, string Message, Action<DialogResult> result)
        {
            show(Title, Message, 2, () =>
            {
                tpnl.Controls.Clear();
                tpnl.Controls.Add(lbl, 0, 0, 8, 1);
                tpnl.Controls.Add(btnOK, 1, 1, 3, 1);
                tpnl.Controls.Add(btnCancel, 4, 1, 3, 1);
            }, result);
        }
        #endregion
        #region ShowMessageBoxYesNoCancel
        public void ShowMessageBoxYesNoCancel(string Title, string Message, Action<DialogResult> result)
        {
            show(Title, Message, 3, () =>
            {
                tpnl.Controls.Clear();
                tpnl.Controls.Add(lbl, 0, 0, 8, 1);
                tpnl.Controls.Add(btnYes, 1, 1, 2, 1);
                tpnl.Controls.Add(btnNo, 3, 1, 2, 1);
                tpnl.Controls.Add(btnCancel, 5, 1, 2, 1);
            }, result);
        }
        #endregion
        #endregion
    }

    #region enum : DialogResult
    public enum DialogResult { OK, Cancel, Yes, No }
    #endregion
}

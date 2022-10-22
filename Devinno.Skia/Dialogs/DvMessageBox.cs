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
    public class DvMessageBox : DvWindow
    {
        #region Properties
        public int MinWidth { get; set; } = 240;
        public int MinHeight { get; set; } = 120;
        public int ButtonHeight { get; set; } = 30;
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
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Margin = new Padding(10), Fill = true };

            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Percent, Size = 100 });
            tpnl.Rows.Add(new DvTableRow() { Mode = SizeMode.Pixel, Size = 40 });
        
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 14F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 12F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 12F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 12F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 12F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 12F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 12F });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 14F });

            Controls.Add(tpnl);
            #endregion

            lbl = new DvLabel() { Name = nameof(lbl), BackgroundDraw = false, Margin = new Padding(10), ContentAlignment = Utils.DvContentAlignment.MiddleCenter };
            btnOK = new DvButton { Name = nameof(btnOK), Text = "확인" };
            btnCancel = new DvButton { Name = nameof(btnCancel), Text = "취소" };
            btnYes = new DvButton { Name = nameof(btnYes), Text = "예" };
            btnNo = new DvButton { Name = nameof(btnNo), Text = "아니요" };

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
        void show(string Title, string Message, Action<DialogResult> result, Action act)
        {
            lbl.FontSize = MessageTextSize;
            btnOK.FontSize = btnCancel.FontSize = btnYes.FontSize = btnNo.FontSize = ButtonTextSize;

            var sz = Util.MeasureText(Message, lbl.FontName, lbl.FontSize, DvFontStyle.Normal);
            var btnSZ = Convert.ToInt32(ButtonHeight + 6);
            var gapW = tpnl.Margin.Left + tpnl.Margin.Right + lbl.Margin.Left + lbl.Margin.Right;
            var gapH = tpnl.Margin.Top + tpnl.Margin.Bottom + lbl.Margin.Top + lbl.Margin.Bottom;
            var mW = MinWidth;
            var mH = MinHeight;

            tpnl.Rows[1].Size = btnSZ;
            Width = Math.Max(gapW + Convert.ToInt32(sz.Width), mW);
            Height = Math.Max(TitleHeight + gapH + btnSZ + Convert.ToInt32(sz.Height), mH);

            this.Title = Title;
            lbl.Text = Message;

            act();

            Task.Run(() => {

                this.Show();
                ev.WaitOne();
                ThreadPool.QueueUserWorkItem((o) => { Thread.Sleep(DvDesign.HIDE_TIME); result(this.result); });
            });
        }
        #endregion
        #region ShowMessageBoxOk
        public void ShowMessageBoxOk(string Title, string Message, Action<DialogResult> result)
        {
            show(Title, Message, result, () =>
            {
                tpnl.Controls.Clear();
                tpnl.Controls.Add(lbl, 0, 0, 8, 1);
                tpnl.Controls.Add(btnOK, 2, 1, 4, 1);
            });
        }
        #endregion
        #region ShowMessageBoxYesNo
        public void ShowMessageBoxYesNo(string Title, string Message, Action<DialogResult> result)
        {
            show(Title, Message, result, () =>
            {
                tpnl.Controls.Clear();
                tpnl.Controls.Add(lbl, 0, 0, 8, 1);
                tpnl.Controls.Add(btnYes, 1, 1, 3, 1);
                tpnl.Controls.Add(btnNo, 4, 1, 3, 1);
            });
        }
        #endregion
        #region ShowMessageBoxOkCancel
        public void ShowMessageBoxOkCancel(string Title, string Message, Action<DialogResult> result)
        {
            show(Title, Message, result, () =>
            {
                tpnl.Controls.Clear();
                tpnl.Controls.Add(lbl, 0, 0, 8, 1);
                tpnl.Controls.Add(btnOK, 1, 1, 3, 1);
                tpnl.Controls.Add(btnCancel, 4, 1, 3, 1);
            });
        }
        #endregion
        #region ShowMessageBoxYesNoCancel
        public void ShowMessageBoxYesNoCancel(string Title, string Message, Action<DialogResult> result)
        {
            show(Title, Message, result, () =>
            {
                tpnl.Controls.Clear();
                tpnl.Controls.Add(lbl, 0, 0, 8, 1);
                tpnl.Controls.Add(btnYes, 1, 1, 2, 1);
                tpnl.Controls.Add(btnNo, 3, 1, 2, 1);
                tpnl.Controls.Add(btnCancel, 5, 1, 2, 1);
            });
        }
        #endregion
        #endregion
    }

    public enum DialogResult { OK, Cancel, Yes, No }
}

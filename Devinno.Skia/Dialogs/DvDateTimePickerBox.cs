using Devinno.Skia.Containers;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Dialogs
{
    public class DvDateTimePickerBox : DvWindow
    {
        #region Properties
        public DateTime SelectedValue
        {
            get
            {
                DateTime ret = new DateTime();

                switch(pickerType )
                {
                    case DateTimePickerType.DateTime:
                        {
                            var dt = calendar.SelectedDays.Count > 0 ? calendar.SelectedDays.First() : DateTime.Now.Date;
                            ret = new DateTime(dt.Year, dt.Month, dt.Day, inHour.Value, inMin.Value, inSec.Value);

                        }
                        break;
                    case DateTimePickerType.Date:
                        {
                            var dt = calendar.SelectedDays.Count > 0 ? calendar.SelectedDays.First() : DateTime.Now.Date;
                            ret = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
                        }
                        break;
                    case DateTimePickerType.Time:
                        {
                            var dt = DateTime.Now.Date;
                            ret = new DateTime(dt.Year, dt.Month, dt.Day, inHour.Value, inMin.Value, inSec.Value);
                        }
                        break;
                }
                
                return ret;
            }
        }
        #endregion

        #region Member Variable
        AutoResetEvent ev = new AutoResetEvent(false);

        DvTableLayoutPanel tpnl;
        DvButton btnOK;
        DvButton btnCancel;
        DvCalendar calendar;
        DvNumberInput<byte> inHour, inMin, inSec;

        DateTimePickerType pickerType = DateTimePickerType.DateTime;

        bool bOK = false;
        #endregion

        #region Constructor
        public DvDateTimePickerBox()
        {
            IconString = "fa-calendar-check";
            Width = 300;
            Height = 400;

            #region tpnl
            tpnl = new DvTableLayoutPanel() { Name = nameof(tpnl), Margin = new Padding(10), Fill = true };

            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 32 });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 2 });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 32 });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 2 });
            tpnl.Columns.Add(new DvTableColumn() { Mode = SizeMode.Percent, Size = 32 });

            Controls.Add(tpnl);
            #endregion
            #region New
            calendar = new DvCalendar { Name = nameof(calendar) };
            inHour = new DvNumberInput<byte> { Name = nameof(inHour), Unit = "시", Minimum = 0, Maximum = 23 };
            inMin = new DvNumberInput<byte> { Name = nameof(inMin), Unit = "분", Minimum = 0, Maximum = 59 };
            inSec = new DvNumberInput<byte> { Name = nameof(inSec), Unit = "초", Minimum = 0, Maximum = 59 };

            btnOK = new DvButton { Name = nameof(btnOK), Text = "확인" };
            btnCancel = new DvButton { Name = nameof(btnCancel), Text = "취소" };
            #endregion
            #region Controls.Add
            #endregion
            #region Event
            #region btn[OK/Cancel].MouseClick
            btnCancel.MouseClick += (o, s) => { bOK = false; Hide(); };
            btnOK.MouseClick += (o, s) => { bOK = true; Hide(); };
            #endregion
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
        void show(string Title, Action act1, Action act2)
        {
            this.Width = 10 + (256 + 6) + 10 + 20 + 10 + 100 + 10;
            this.Height = TitleHeight + 10 + (256 + 6) + 10;

            this.Title = Title;

            act1();

            Task.Run(() => {

                this.Show();
                ev.WaitOne();
                ThreadPool.QueueUserWorkItem((o) => { Thread.Sleep(DvDesign.HIDE_TIME); act2(); });
            });
        }
        #endregion
        #region ShowDateTimePicker 
        public void ShowDateTimePicker(string Title, DateTime? value, Action<DateTime?> result)
        {
            pickerType = DateTimePickerType.DateTime;

            show(Title, () =>
            {
                this.Width = 300;
                this.Height = TitleHeight + 10 + 246 + 4 + 36 + 4 + 36 + 10;
                this.IconString = "fa-calendar-check";

                #region Set
                if (value.HasValue)
                {
                    calendar.SelectedDays.Add(value.Value);
                    inHour.Value = Convert.ToByte(value.Value.Hour);
                    inMin.Value = Convert.ToByte(value.Value.Minute);
                    inSec.Value = Convert.ToByte(value.Value.Second);
                }
                else
                {
                    calendar.SelectedDays.Add(DateTime.Now.Date);
                    inHour.Value = 0;
                    inMin.Value = 0;
                    inSec.Value = 0;
                }
                #endregion
                #region Layout
                tpnl.Rows.Clear();
                tpnl.Controls.Clear();

                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Percent, Size = 100 });
                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Pixel, Size = 4 });
                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Pixel, Size = 36 });
                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Pixel, Size = 4 });
                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Pixel, Size = 36 });

                tpnl.Controls.Add(calendar, 0, 0, 5, 1);

                tpnl.Controls.Add(inHour, 0, 2);
                tpnl.Controls.Add(inMin, 2, 2);
                tpnl.Controls.Add(inSec, 4, 2);

                tpnl.Controls.Add(btnOK, 2, 4);
                tpnl.Controls.Add(btnCancel, 4, 4);
                #endregion

            }, () =>
            {
                if (bOK) result(SelectedValue);
                else result(null);
            });
        }
        #endregion
        #region ShowDatePicker
        public void ShowDatePicker(string Title, DateTime? value, Action<DateTime?> result)
        {
            pickerType = DateTimePickerType.Date;

            show(Title, () =>
            {
                this.Width = 300;
                this.Height = TitleHeight + 10 + 246 + 4 + 36 + 10;
                this.IconString = "fa-calendar-check";

                #region Set
                if (value.HasValue)
                {
                    calendar.SelectedDays.Add(value.Value);
                }
                else
                {
                    calendar.SelectedDays.Add(DateTime.Now.Date);
                }
                #endregion
                #region Layout
                tpnl.Rows.Clear();
                tpnl.Controls.Clear();

                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Percent, Size = 100 });
                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Pixel, Size = 4 });
                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Pixel, Size = 36 });

                tpnl.Controls.Add(calendar, 0, 0, 5, 1);
                tpnl.Controls.Add(btnOK, 2, 2);
                tpnl.Controls.Add(btnCancel, 4, 2);
                #endregion

            }, () =>
            {
                if (bOK) result(SelectedValue);
                else result(null);
            });
        }
        #endregion
        #region ShowTimePicker
        public void ShowTimePicker(string Title, DateTime? value, Action<DateTime?> result)
        {
            pickerType = DateTimePickerType.Time;

            show(Title, () =>
            {
                this.Width = 300;
                this.Height = TitleHeight + 10 + 36 + 4 + 36 + 10;
                this.IconString = "fa-clock";

                #region Set
                if (value.HasValue)
                {
                    inHour.Value = Convert.ToByte(value.Value.Hour);
                    inMin.Value = Convert.ToByte(value.Value.Minute);
                    inSec.Value = Convert.ToByte(value.Value.Second);
                }
                else
                {
                    inHour.Value = 0;
                    inMin.Value = 0;
                    inSec.Value = 0;
                }
                #endregion
                #region Layout
                tpnl.Rows.Clear();
                tpnl.Controls.Clear();

                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Pixel, Size = 36 });
                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Pixel, Size = 4 });
                tpnl.Rows.Add(new DvTableRow { Mode = SizeMode.Pixel, Size = 36 });

                tpnl.Controls.Add(inHour, 0, 0);
                tpnl.Controls.Add(inMin, 2, 0);
                tpnl.Controls.Add(inSec, 4, 0);
                tpnl.Controls.Add(btnOK, 2, 2);
                tpnl.Controls.Add(btnCancel, 4, 2);
                #endregion

            }, () =>
            {
                if (bOK) result(SelectedValue);
                else result(null);
            });
        }
        #endregion
        #endregion
    }
}

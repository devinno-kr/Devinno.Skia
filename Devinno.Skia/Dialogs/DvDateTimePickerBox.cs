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
        #region SelectedValue
        public DateTime SelectedValue
        {
            get
            {
                DateTime ret = new DateTime();

                switch (pickerType)
                {
                    case DateTimePickerMode.DateTime:
                        {
                            var dt = calendar.SelectedDays.Count > 0 ? calendar.SelectedDays.First() : DateTime.Now.Date;
                            ret = new DateTime(dt.Year, dt.Month, dt.Day, inHour.Value, inMin.Value, inSec.Value);
                        }
                        break;
                    case DateTimePickerMode.Date:
                        {
                            var dt = calendar.SelectedDays.Count > 0 ? calendar.SelectedDays.First() : DateTime.Now.Date;
                            ret = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0);
                        }
                        break;
                    case DateTimePickerMode.Time:
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
        #endregion

        #region Member Variable
        AutoResetEvent ev = new AutoResetEvent(false);

        DvTableLayoutPanel tpnl;
        DvButton btnOK;
        DvButton btnCancel;
        DvCalendar calendar;
        DvInputNumber<byte> inHour, inMin, inSec;

        DateTimePickerMode pickerType = DateTimePickerMode.DateTime;

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

            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 32));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 2));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 32));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 2));
            tpnl.Columns.Add(new SizeInfo(DvSizeMode.Percent, 32));

            Controls.Add(tpnl);
            #endregion
            #region New
            calendar = new DvCalendar { Name = nameof(calendar), BackgroundDraw = false };
            inHour = new DvInputNumber<byte> { Name = nameof(inHour), FormatString = "0 시", Minimum = 0, Maximum = 23 };
            inMin = new DvInputNumber<byte> { Name = nameof(inMin), FormatString = "0 분", Minimum = 0, Maximum = 59 };
            inSec = new DvInputNumber<byte> { Name = nameof(inSec), FormatString = "0 초", Minimum = 0, Maximum = 59 };

            btnOK = new DvButton { Name = nameof(btnOK), Text = "확인", Gradient = true };
            btnCancel = new DvButton { Name = nameof(btnCancel), Text = "취소", Gradient = true };
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
        void show(string Title, Action actLayout, Action actOK)
        {
            this.Title = Title;

            actLayout();

            Task.Run(() =>
            {
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
        #region ShowDateTimePicker 
        public void ShowDateTimePicker(string Title, DateTime? value, Action<DateTime?> result)
        {
            pickerType = DateTimePickerMode.DateTime;

            show(Title, () =>
            {
                #region var
                this.Width = 300;
                this.Height = TitleHeight + 10 + 246 + 4 + 36 + 4 + 36 + 10;
                this.IconString = "fa-calendar-check";
                #endregion
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

                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 100));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 4));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 36));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 4));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 36));

                tpnl.Controls.Add(calendar, 0, 0, 5, 1);

                tpnl.Controls.Add(inHour, 0, 2);
                tpnl.Controls.Add(inMin, 2, 2);
                tpnl.Controls.Add(inSec, 4, 2);

                tpnl.Controls.Add(btnOK, 2, 4);
                tpnl.Controls.Add(btnCancel, 4, 4);
                #endregion
            },
            () =>
            {
                #region result
                if (bOK) result(SelectedValue);
                else result(null);
                #endregion
            });
        }
        #endregion
        #region ShowDatePicker
        public void ShowDatePicker(string Title, DateTime? value, Action<DateTime?> result)
        {
            pickerType = DateTimePickerMode.Date;

            show(Title, () =>
            {
                #region var
                this.Width = 300;
                this.Height = TitleHeight + 10 + 246 + 4 + 36 + 10;
                this.IconString = "fa-calendar-check";
                #endregion
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

                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Percent, 100));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 4));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 36));

                tpnl.Controls.Add(calendar, 0, 0, 5, 1);
                tpnl.Controls.Add(btnOK, 2, 2);
                tpnl.Controls.Add(btnCancel, 4, 2);
                #endregion
            }, 
            () =>
            {
                #region result
                if (bOK) result(SelectedValue);
                else result(null);
                #endregion
            });
        }
        #endregion
        #region ShowTimePicker
        public void ShowTimePicker(string Title, DateTime? value, Action<DateTime?> result)
        {
            pickerType = DateTimePickerMode.Time;

            show(Title, () =>
            {
                #region var
                this.Width = 300;
                this.Height = TitleHeight + 10 + 36 + 4 + 36 + 10;
                this.IconString = "fa-clock";
                #endregion
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

                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 36));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 4));
                tpnl.Rows.Add(new SizeInfo(DvSizeMode.Pixel, 36));

                tpnl.Controls.Add(inHour, 0, 0);
                tpnl.Controls.Add(inMin, 2, 0);
                tpnl.Controls.Add(inSec, 4, 0);
                tpnl.Controls.Add(btnOK, 2, 2);
                tpnl.Controls.Add(btnCancel, 4, 2);
                #endregion
            },
            () =>
            {
                #region result
                if (bOK) result(SelectedValue);
                else result(null);
                #endregion
            });
        }
        #endregion
        #endregion
    }
}

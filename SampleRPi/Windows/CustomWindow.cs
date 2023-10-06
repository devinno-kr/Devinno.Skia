using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Devinno.Skia.Design;

namespace SampleRPi.Windows
{
    public partial class CustomWindow : DvWindow
    {
        #region Member Variable
        AutoResetEvent ev = new AutoResetEvent(false);
        bool bOK = false;
        #endregion

        public CustomWindow()
        {
            InitializeComponent();

            #region btn[OK/Cancel].MouseClick
            btnCancel.ButtonClick += (o, s) => { bOK = false; Hide(); };
            btnOK.ButtonClick += (o, s) => { bOK = true; Hide(); };
            #endregion

        }

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
        void show(Action actSet, Action actResult)
        {
            actSet();

            Task.Run(() => {

                this.Show();
                ev.WaitOne();
                ThreadPool.QueueUserWorkItem((o) => { Thread.Sleep(200); actResult(); });
            });
        }
        #endregion
        #region ShowCustomWindow
        public void ShowCustomWindow(bool? value, Action<bool?> result)
        {
            var vc = value ?? false;

            show(() =>
            {
                sw.OnOff = vc;

            }, () =>
            {
                if (bOK) result(sw.OnOff);
                else result(null);
            });
        }
        #endregion
        #endregion
    }
}

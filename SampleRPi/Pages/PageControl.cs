using System;
using Devinno.Skia.Design;
using Devinno.Skia.Utils;

namespace SampleRPi.Pages
{
    public partial class PageControl : DvPage
    {
        #region Member Variable
        DateTime prev = DateTime.Now;
        int n = 0;
        #endregion

        public PageControl()
        {
            InitializeComponent();

            #region Set
            ani.OffImage = Util.FromBitmap("./images/anioff.png");
            ani.LoadGIF("./images/ani.gif");
            #endregion

            #region Event
            vlblInt.ButtonClick += (o, s) => n = 0;
            ani.MouseClick += (o, s) => ani.OnOff = !ani.OnOff;
            #endregion

         
        }

        #region OnUpdate
        protected override void OnUpdate()
        {
            lmp.OnOff = chk1.Checked;
            lbtn.OnOff = tbtn1.Checked;

            var now = DateTime.Now;
            if ((now - prev).TotalMilliseconds >= 100)
            {
                vlblText.Value = now.ToString("HH:mm:ss");
                vlblInt.Value = n++;
                vlblBool.Value = now.Second / 5 % 2 == 0;
                prev = now;
            }

            base.OnUpdate();
        }
        #endregion

    }
}

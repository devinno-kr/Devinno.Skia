using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devinno.Skia.Utils;
using Devinno.Skia.Controls;
using Devinno.Skia.Containers;
using Devinno.Skia.Design;

namespace SampleRPi.Pages
{
    partial class PageTest
    {
        #region declare
        #endregion

        public void InitializeComponent()
        {
            #region base
            UseMasterPage = true;
            BackgroundDraw = false;
            BackColor = null;
            BackgroundImage = null;
            AnimationType = null;
            Text = "Test";
            IconString = "";
            #endregion

        }
    }
}

using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleRPi.Pages
{
    public class PageInformation : DvPage
    {
        #region Properties
        public string Title { get => lblTitle.Text; set => lblTitle.Text = value; }
        #endregion

        #region Member Variable
        private DvButton btnPrev;
        private DvLabel lblTitle;
        #endregion

        #region Constructor
        public PageInformation()
        {
            UseMasterPage = false;
            BackColor = Util.FromArgb(50, 50, 50);
            BackgroundDraw = true;
            AnimationType = Devinno.Skia.Utils.AnimationType.Drill;

            #region btnExit
            btnPrev = new DvButton() { Name = nameof(btnPrev), X = MainWindow.W - 20 - 80, Y = 20, Width = 80, Height = 70, Text = "이전" };
            Controls.Add(btnPrev);
            #endregion
            #region lblTitle
            lblTitle = new DvLabel() { Name = nameof(lblTitle), Fill = true, BackgroundDraw = false, ContentAlignment = DvContentAlignment.MiddleCenter, FontSize = 48 };
            Controls.Add(lblTitle);
            #endregion

            btnPrev.MouseClick += (o, s) => Design.SetPage(Program.MainWindow.pageContainer);
        }
        #endregion

        protected override void OnDraw(SKCanvas Canvas)
        {
            BackColor = Design.Theme.BackColor;
            base.OnDraw(Canvas);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devinno.Skia.Utils;
using Devinno.Skia.Controls;
using Devinno.Skia.Containers;
using Devinno.Skia.Design;
using Devinno.Skia.WinForms;
using Devinno.Skia.Dialogs;
using System.Windows.Forms;

namespace SampleForm
{
    public partial class MainWindow : DvViewControl
    {
        public MainWindow() : base(W, H)
        {
            InitializeComponent();

            Design.SetPage(PageControl);

            nav.FoldingMenuClicked += (o, s) =>
            {
                if (s.Item.Text == "Exit")
                {
                    DvDialogs.MessageBox.ShowMessageBoxYesNo("종료", "프로그램을 종료 하시겠습니다?",
                        (result) =>
                        {
                            if (result == Devinno.Skia.Dialogs.DialogResult.Yes)
                                Application.Exit();
                        });
                }
            };

            Design.Animation = true;
        }
    }
}

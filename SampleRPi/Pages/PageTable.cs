using System;
using Devinno.Skia.Design;

namespace SampleRPi.Pages
{
    public partial class PageTable : DvPage
    {
        public PageTable()
        {
            InitializeComponent();

            sldpnl.SelectedPage = spList;
            list.ItemHeight = 100;
        }
    }
}

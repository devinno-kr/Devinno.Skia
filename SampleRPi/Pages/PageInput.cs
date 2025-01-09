using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devinno.Skia.Design;

namespace SampleRPi.Pages
{
    public partial class PageInput : DvPage
    {
        #region Constructor
        public PageInput()
        {
            InitializeComponent();

            dvComboBox1.SelectedIndexChanged += (o, s) =>
            {

            };

            dvComboBox1.ItemFontSize = 10;
            dvComboBox1.FontSize = 10;

        }
        #endregion
    }
}

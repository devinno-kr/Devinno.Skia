using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Design
{
    public class DvSubPage : DvContainer
    {
        #region Properties
        [JsonIgnore]
        public DvContainer ContainerControl { get; internal set; }

        public string Text { get; set; } = "Page";
        public string IconString { get; set; } = null;
        #endregion
    }
}

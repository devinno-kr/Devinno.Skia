using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Tools
{
    internal class ResourceTool
    {
        internal static SKBitmap volumemask { get; private set; }
        internal static SKBitmap saturation { get; private set; }
        internal static SKBitmap circlegrad { get; private set; }

        static ResourceTool()
        {
            try
            {
                var vm = Properties.Resources.tmi2;
                var sa = Properties.Resources.Saturation;
                var cg = Properties.Resources.gb2;

                using (var ms = new MemoryStream(vm)) volumemask = SKBitmap.Decode(ms);
                using (var ms = new MemoryStream(sa)) saturation = SKBitmap.Decode(ms);
                using (var ms = new MemoryStream(cg)) circlegrad = SKBitmap.Decode(ms);
            }
            catch (Exception ex)
            {
                File.WriteAllText("error.log", DateTime.Now.ToString() + "\r\n" + ex.Message + "\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n\r\n");
            }
        }

    }
}

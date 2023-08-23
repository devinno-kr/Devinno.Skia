using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Tools
{
    public class AppTool
    {
        public static string CurrentPath => Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
    }
}

using Devinno.Skia.Collections;
using Devinno.Skia.Design;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Design
{
    public interface IDvContainer
    {
        DvControlCollection Controls { get; }

        DvDesign Design { get; }

        SKColor GetBackColor();
    }
}

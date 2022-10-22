using Devinno.Tools;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Extensions
{
    public static class ColorExtension
    {
        #region BrightnessTransmit
        public static SKColor BrightnessTransmit(this SKColor color, float Brightness)
        {
            float h, s, l;
            color.ToHsl(out h, out s, out l);
            var lc = MathTool.Constrain(l * Brightness, -100, 100);
            l = Convert.ToSingle(MathTool.Constrain(l + lc, 0, 100));
            return SKColor.FromHsl(h, s, l, color.Alpha);
        }
        #endregion
        #region BrightnessChange
        public static SKColor BrightnessChange(this SKColor color, float Brightness)
        {
            float h, s, l;
            color.ToHsl(out h, out s, out l);
            var lc = MathTool.Map(MathTool.Constrain(Brightness, -1, 1), -100, 100, 0, 100);
            l = Convert.ToSingle(lc);
            return SKColor.FromHsl(h, s, l, color.Alpha);
        }
        #endregion
    }
     
}

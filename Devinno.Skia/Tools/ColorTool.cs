using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Tools
{
    public class ColorTool
    {
        #region Member Variable
        static Dictionary<SKColor, List<string>> dic = new Dictionary<SKColor, List<string>>();
        #endregion

        #region Constructor
        static ColorTool()
        {
            var vals = typeof(SKColors).GetFields();
            foreach (var v in vals)
            {
                var color = (SKColor)v.GetValue(null);
                var name = v.Name;
                if (!dic.ContainsKey(color)) dic.Add(color, new List<string>());
                dic[color].Add(name);
            }
        }
        #endregion

        #region MixColorAlpha
        /// <summary>
        /// 원본 색상에 투명도가 적용된 색상을 덧씌웠을 때 색상
        /// </summary>
        /// <param name="dest">원본 색</param>
        /// <param name="src">덧씌울 색</param>
        /// <param name="srcAlpha">덧씌울 색 투명도</param>
        /// <returns>최종색</returns>
        public static SKColor MixColorAlpha(SKColor dest, SKColor src, int srcAlpha)
        {
            var r = Convert.ToByte(MathTool.Constrain(MathTool.Map(srcAlpha, 0.0, 255.0, dest.Red, src.Red), 0, 255));
            var g = Convert.ToByte(MathTool.Constrain(MathTool.Map(srcAlpha, 0.0, 255.0, dest.Green, src.Green), 0, 255));
            var b = Convert.ToByte(MathTool.Constrain(MathTool.Map(srcAlpha, 0.0, 255.0, dest.Blue, src.Blue), 0, 255));

            return new SKColor(r, g, b);
        }
        #endregion
        #region GetName
        public static string GetName(SKColor c, ColorCodeType code)
        {
            var ret = "";
            if (dic.ContainsKey(c)) ret = dic[c].First();
            else
            {
                if(code == ColorCodeType.ARGB) ret = c.Alpha.ToString( ) + "," + c.Red.ToString( ) + "," + c.Green.ToString( ) + "," + c.Blue.ToString( );
                else if (code == ColorCodeType.RGB) ret = c.Red.ToString() + "," + c.Green.ToString() + "," + c.Blue.ToString();
                else if(code == ColorCodeType.CodeARGB) ret = "#" + c.Alpha.ToString("X2") + c.Red.ToString("X2") + c.Green.ToString("X2") + c.Blue.ToString("X2");
                else if (code == ColorCodeType.CodeRGB) ret = "#" + c.Red.ToString("X2") + c.Green.ToString("X2") + c.Blue.ToString("X2");
            }
            return ret;
        }
        #endregion
    }

    public enum ColorCodeType { ARGB, RGB, CodeRGB, CodeARGB  }
}

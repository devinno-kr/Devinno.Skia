using Devinno.Skia.Design;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Tools
{
    public class FontTool
    {
        private static Dictionary<string, Dictionary<DvFontStyle, SKTypeface>> FontsDictionary { get; set; } = new Dictionary<string, Dictionary<DvFontStyle, SKTypeface>>();
        
        #region Load
        public static void Load()
        {
            var path = Path.Combine(AppTool.CurrentPath, "Fonts");

            using (var ms = new MemoryStream(Properties.Resources.NanumGothic))
            {
                var font = SKTypeface.FromStream(ms);
                var fs = FontStyle(font);
                var nm = font.FamilyName.ToLower();

                if (fs != DvFontStyle.Unknown)
                {
                    if (!FontsDictionary.ContainsKey(nm)) FontsDictionary.Add(nm, new Dictionary<DvFontStyle, SKTypeface>());
                    if (!FontsDictionary[nm].ContainsKey(fs)) FontsDictionary[nm].Add(fs, font);
                }
            }

            if (Directory.Exists(path))
            {
                foreach (var fn in Directory.GetFiles(path))
                {
                    var font = SKTypeface.FromFile(fn);
                    var fs = FontStyle(font);
                    var nm = font.FamilyName.ToLower();

                    if (fs != DvFontStyle.Unknown)
                    {
                        if (!FontsDictionary.ContainsKey(nm)) FontsDictionary.Add(nm, new Dictionary<DvFontStyle, SKTypeface>());
                        if (!FontsDictionary[nm].ContainsKey(fs)) FontsDictionary[nm].Add(fs, font);
                    }
                }
            }
        }
        #endregion
        #region FontStyle
        private static DvFontStyle FontStyle(SKTypeface f)
        {
            var ret = DvFontStyle.Unknown;
            if (f != null)
            {
                switch (f.FontWeight)
                {
                    case 100: ret = f.FontSlant == SKFontStyleSlant.Upright ? DvFontStyle.Thin : (f.FontSlant == SKFontStyleSlant.Italic ? DvFontStyle.ThinItalic : DvFontStyle.ThinOblique); break;
                    case 200: ret = f.FontSlant == SKFontStyleSlant.Upright ? DvFontStyle.ExtraLight : (f.FontSlant == SKFontStyleSlant.Italic ? DvFontStyle.ExtraLightItalic : DvFontStyle.ExtraLightOblique); break;
                    case 300: ret = f.FontSlant == SKFontStyleSlant.Upright ? DvFontStyle.Light : (f.FontSlant == SKFontStyleSlant.Italic ? DvFontStyle.LightItalic : DvFontStyle.LightOblique); break;
                    case 400: ret = f.FontSlant == SKFontStyleSlant.Upright ? DvFontStyle.Normal : (f.FontSlant == SKFontStyleSlant.Italic ? DvFontStyle.Italic : DvFontStyle.Oblique); break;
                    case 500: ret = f.FontSlant == SKFontStyleSlant.Upright ? DvFontStyle.Medium : (f.FontSlant == SKFontStyleSlant.Italic ? DvFontStyle.MediumItalic : DvFontStyle.MediumOblique); break;
                    case 600: ret = f.FontSlant == SKFontStyleSlant.Upright ? DvFontStyle.Demi : (f.FontSlant == SKFontStyleSlant.Italic ? DvFontStyle.DemiItalic : DvFontStyle.DemiOblique); break;
                    case 700: ret = f.FontSlant == SKFontStyleSlant.Upright ? DvFontStyle.Bold : (f.FontSlant == SKFontStyleSlant.Italic ? DvFontStyle.BoldItalic : DvFontStyle.BoldOblique); break;
                    case 800: ret = f.FontSlant == SKFontStyleSlant.Upright ? DvFontStyle.ExtraBold : (f.FontSlant == SKFontStyleSlant.Italic ? DvFontStyle.ExtraBoldItalic : DvFontStyle.ExtraBoldOblique); break;
                    case 900: ret = f.FontSlant == SKFontStyleSlant.Upright ? DvFontStyle.Black : (f.FontSlant == SKFontStyleSlant.Italic ? DvFontStyle.BlackItalic : DvFontStyle.BlackOblique); break;
                    case 950: ret = f.FontSlant == SKFontStyleSlant.Upright ? DvFontStyle.ExtraBlack : (f.FontSlant == SKFontStyleSlant.Italic ? DvFontStyle.ExtraBlackItalic : DvFontStyle.ExtraBlackOblique); break;
                }
            }
            return ret;
        }
        #endregion
        #region GetFont
        public static SKTypeface GetFont(string FontName, DvFontStyle Style)
        {
            SKTypeface ret = null;
            var nm = FontName.ToLower();

            if (FontsDictionary.ContainsKey(nm))
            {
                if (FontsDictionary[nm].ContainsKey(Style)) ret = FontsDictionary[nm][Style];
                else ret = FontsDictionary[nm].Values.FirstOrDefault();
            }
            else
            {
                ret = FontsDictionary["nanumgothic"][DvFontStyle.Normal];
            }
            return ret;
        }
        #endregion
        #region GetFontNames
        public static string[] GetFontNames() => FontsDictionary.Values.Select(x => x.FirstOrDefault()).Select(x => x.Value.FamilyName).ToArray();
        #endregion
    }

}

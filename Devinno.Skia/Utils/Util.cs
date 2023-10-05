using Devinno.Skia.Design;
using Devinno.Skia.Icon;
using Devinno.Skia.Theme;
using Devinno.Skia.Tools;
using Devinno.Tools;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathTool = Devinno.Skia.Tools.MathTool;

namespace Devinno.Skia.Utils
{
    public class Util
    {
        #region FromBitmap
        public static SKBitmap FromBitmap(string path)
        {
            SKBitmap ret = null;
            try
            {
                if (File.Exists(path))
                {
                    using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        ret = SKBitmap.Decode(fs);
                    }
                }
            }
            catch { }
            return ret;
        }
        #endregion
        #region FromBitmap64
        public static SKBitmap FromBitmap64(string base64)
        {
            SKBitmap ret = null;
            try
            {
                var ba = Convert.FromBase64String(base64);
                using (var ms = new MemoryStream(ba)) ret = SKBitmap.Decode(ba);
            }
            catch { }
            return ret;
        }
        #endregion
        #region FromAssemblyBitmap
        public static SKBitmap FromAssemblyBitmap(Assembly asm, string name)
        {
            using (var ms = asm.GetManifestResourceStream(name))
            {
                var ret = SKBitmap.Decode(ms);
                return ret;
            }
        }
        #endregion
        #region FromArgb
        public static SKColor FromArgb(Color c) => new SKColor(c.R, c.G, c.B, c.A);
        public static SKColor FromArgb(int c) => new SKColor(Convert.ToByte(((uint)c & 0xFF0000) >> 16), Convert.ToByte(((uint)c & 0xFF00) >> 8), Convert.ToByte(((uint)c & 0xFF)), Convert.ToByte(((uint)c & 0xFF000000)>>24));

        public static SKColor FromArgb(byte a, byte r, byte g, byte b) => new SKColor(r, g, b, a);
        //public static SKColor FromArgb(byte a, Color c) => new SKColor(c.R, c.G, c.B, a);
        public static SKColor FromArgb(byte a, SKColor c) => new SKColor(c.Red, c.Green, c.Blue, a);

        public static SKColor FromArgb(byte r, byte g, byte b) => new SKColor(r, g, b);
        #endregion
        #region FromRect
        public static SKRect FromRect(DvControl v) => FromRect(v.X, v.Y, v.Width, v.Height);
        public static SKRect FromRect(SKRect rt) => new SKRect(rt.Left, rt.Top, rt.Right, rt.Bottom);
        public static SKRect FromRect(Rectangle rt) => new SKRect(rt.Left, rt.Top, rt.Right, rt.Bottom);
        public static SKRect FromRect(RectangleF rt) => new SKRect(rt.Left, rt.Top, rt.Right, rt.Bottom);
        public static SKRect FromRect(int x, int y, int width, int height) => FromRect(new Rectangle(x, y, width, height));
        public static SKRect FromRect(float x, float y, float width, float height) => FromRect(new RectangleF(x, y, width, height));
        public static SKRect FromRect(SKRect rt, Padding pad) => new SKRect(rt.Left + pad.Left, rt.Top + pad.Top, rt.Right - pad.Right, rt.Bottom - pad.Bottom);
        #endregion

        #region MeasureText
        public static SKSize MeasureText(string text, string font, float size, DvFontStyle style)
        {
            SKSize ret = new SKSize();
            if (!string.IsNullOrWhiteSpace(text))
            {
                using (var paint = new SKPaint())
                {
                    #region paint set
                    paint.Typeface = FontTool.GetFont(font, style);
                    paint.TextSize = size;
                    #endregion
                    #region var
                    var ls = new List<TI>();
                    foreach (var v in text.Replace("\r\n", "\n").Split('\n'))
                    {
                        var rtv = new SKRect();
                        paint.MeasureText(v, ref rtv);
                        //rtv.Right = rtv.Left + paint.MeasureText(v);
                        ls.Add(new TI() { Text = v, Bounds = rtv });
                    }


                    var TK = paint.FontSpacing / 4F;
                    var TH = (ls.Count * paint.FontSpacing) + (TK * (ls.Count - 1));
                    var TW = ls.Max(x => x.Bounds.Width);
                    #endregion

                    ret.Width = TW;
                    ret.Height = TH;
                }
            }
            return ret;
        }
        #endregion
        #region MeasureTextIcon
        static SKSize MeasureTextIcon(SKRect rtmTX, SKRect rtmFA, DvTextIconAlignment align, float gap)
        {
            SKSize ret = new SKSize(0F, 0F);

            if (align == DvTextIconAlignment.LeftRight)
            {
                ret = new SKSize(rtmFA.Width + gap + rtmTX.Width, Math.Max(rtmTX.Height, rtmFA.Height));
            }
            else
            {
                ret = new SKSize(Math.Max(rtmTX.Width, rtmFA.Width), rtmFA.Height + gap + rtmTX.Height);
            }

            return ret;
        }

        public static SKSize MeasureTextIcon(string text, string font, float textsize, DvFontStyle style, float gap, string iconString, float iconSize, DvTextIconAlignment talign)
        {
            SKSize ret = new SKSize(0F, 0F);

            using (var paint = new SKPaint())
            {
                if (string.IsNullOrEmpty(text))
                {
                    ret = new SKSize(iconSize + 2, iconSize + 2);
                }
                else
                {
                    paint.Typeface = FontTool.GetFont(font, style);
                    paint.TextSize = textsize;

                    #region var
                    var ls = new List<TI>();
                    foreach (var v in text.Replace("\r\n", "\n").Split('\n'))
                    {
                        var rtv = new SKRect();
                        paint.MeasureText(v, ref rtv);
                        rtv.Right = rtv.Left + paint.MeasureText(v);
                        ls.Add(new TI() { Text = v, Bounds = rtv });
                    }
                    #endregion
                    #region ret
                    if (FA.Contains(iconString))
                    {
                        var TK = paint.FontSpacing / 4F;
                        var TH = (ls.Count * paint.FontSpacing) + (TK * (ls.Count - 1));
                        var TW = ls.Max(x => x.Bounds.Width);
                        var IWH = iconSize + 2;
                        var vgap = string.IsNullOrWhiteSpace(text) ? 0 : gap;

                        if (talign == DvTextIconAlignment.LeftRight)
                        {
                            ret.Width = TW + vgap + IWH;
                            ret.Height = Math.Max(TH, IWH);
                        }
                        else
                        {
                            ret.Width = Math.Max(TW, IWH);
                            ret.Height = TH + vgap + IWH;
                        }
                    }
                    else
                    {
                        var TK = paint.FontSpacing / 4F;
                        var TH = (ls.Count * paint.FontSpacing) + (TK * (ls.Count - 1));
                        var TW = ls.Max(x => x.Bounds.Width);

                        ret.Width = TW;
                        ret.Height = TH;
                    }
                    #endregion
                }
            }

            return ret;
        }

        public static SKSize MeasureTextIcon(DvTextIcon texticon)
        {
            return MeasureTextIcon(texticon.Text, texticon.FontName, texticon.FontSize, texticon.FontStyle, texticon.IconGap,
                texticon.IconString, texticon.IconSize, texticon.IconAlignment);
        }
        #endregion

        #region DrawText
        public static void DrawText(SKCanvas canvas, string text, string fontName, float fontSize, DvFontStyle fontStyle, SKColor color, SKRect rect, DvContentAlignment align = DvContentAlignment.MiddleCenter)
        {
            var ft = FontTool.GetFont(fontName, fontStyle);
            if (!string.IsNullOrWhiteSpace(text) && ft != null)
            {
                using (var paint = new SKPaint { IsAntialias = DvDesign.AA, SubpixelText = true })
                {
                    #region set
                    paint.Typeface = ft;
                    paint.TextSize = fontSize;
                    paint.Color = color;
                    #endregion
                    #region var
                    var ls = new List<TI>();
                    foreach (var v in text.Replace("\r\n", "\n").Split('\n'))
                    {
                        var rtv = new SKRect();
                        paint.MeasureText(v, ref rtv);
                        //rtv.Right = rtv.Left + paint.MeasureText(v);
                        ls.Add(new TI() { Text = v, Bounds = rtv });
                    }
                   
                    var TK = paint.FontSpacing / 4F;
                    var TH = (ls.Count * paint.FontSpacing) + (TK * (ls.Count - 1));
                    var TW = ls.Max(x => x.Bounds.Width);
                    var rtBox = MakeRectangleAlign(rect, new SKSize(TW, TH), align);

                    var idx = 0;
                    #endregion
                    #region draw
                    foreach (var v in ls)
                    {
                        var rtv = Util.FromRect(rtBox.Left, rtBox.Top + (idx * (paint.FontSpacing + TK)), rtBox.Width, paint.FontSpacing);
                        var rtText = v.Bounds;
                        var s = v.Text;

                        var xText = rtv.Left;
                        var yText = rtv.Top;

                        var ox = 0.5F;
                        var oy = 0F;

                        switch (align)
                        {
                            case DvContentAlignment.TopLeft: xText = rtv.Left - rtText.Left + ox; yText = rtv.Top - rtText.Top + oy; break;
                            case DvContentAlignment.TopCenter: xText = rtv.MidX - rtText.MidX + ox; yText = rtv.Top - rtText.Top + oy; break;
                            case DvContentAlignment.TopRight: xText = rtv.Right - rtText.Right + ox; yText = rtv.Top - rtText.Top + oy; break;

                            case DvContentAlignment.MiddleLeft: xText = rtv.Left - rtText.Left + ox; yText = rtv.MidY - rtText.MidY + oy; break;
                            case DvContentAlignment.MiddleCenter: xText = rtv.MidX - rtText.MidX + ox; yText = rtv.MidY - rtText.MidY + oy; break;
                            case DvContentAlignment.MiddleRight: xText = rtv.Right - rtText.Right + ox; yText = rtv.MidY - rtText.MidY + oy; break;

                            case DvContentAlignment.BottomLeft: xText = rtv.Left - rtText.Left + ox; yText = rtv.Bottom - rtText.Bottom + oy; break;
                            case DvContentAlignment.BottomCenter: xText = rtv.MidX - rtText.MidX + ox; yText = rtv.Bottom - rtText.Bottom + oy; break;
                            case DvContentAlignment.BottomRight: xText = rtv.Right - rtText.Right + ox; yText = rtv.Bottom - rtText.Bottom + oy; break;
                        }

                        canvas.DrawText(s, xText, yText, paint);

                        idx++;
                    }
                    #endregion
                }
            }
        }
        #endregion
        #region DrawIcon
        public static void DrawIcon(SKCanvas canvas, DvIcon icon, SKColor color, SKRect rect, DvContentAlignment align = DvContentAlignment.MiddleCenter)
        {
            if(icon != null)
            {
                if (icon.IconImage != null)
                {
                    using (var paint = new SKPaint { IsAntialias = DvDesign.AA, SubpixelText = true })
                    {
                        var rt = Util.MakeRectangle(rect, new SKSize(icon.IconSize + 2, icon.IconSize + 2));
                        canvas.DrawBitmap(icon.IconImage, rt, paint);
                    }
                }
                else DrawIcon(canvas, icon.IconString, icon.IconSize, color, rect, align);
            }
        }
        
        public static void DrawIcon(SKCanvas canvas, string iconString, float iconSize, SKColor color, SKRect rect, DvContentAlignment align = DvContentAlignment.MiddleCenter)
        {
            var fai = FA.GetFAI(iconString);

            if (fai != null && iconSize > 0)
            {
                using (var paint = new SKPaint { IsAntialias = DvDesign.AA, SubpixelText = true })
                {
                    #region set
                    paint.TextSize = iconSize;
                    paint.Typeface = fai.FontFamily;
                    paint.Color = color;
                    #endregion
                    #region var
                    var rtText = new SKRect();
                    paint.MeasureText(fai.IconText, ref rtText);
                    //rtText.Right = rtText.Left + paint.MeasureText(fai.IconText);
                    var xText = rect.Left;
                    var yText = rect.Top;
                    #endregion
                    #region draw
                    switch (align)
                    {
                        case DvContentAlignment.TopLeft: xText = rect.Left - rtText.Left; yText = rect.Top - rtText.Top; break;
                        case DvContentAlignment.TopCenter: xText = rect.MidX - rtText.MidX; yText = rect.Top - rtText.Top; break;
                        case DvContentAlignment.TopRight: xText = rect.Right - rtText.Right; yText = rect.Top - rtText.Top; break;

                        case DvContentAlignment.MiddleLeft: xText = rect.Left - rtText.Left; yText = rect.MidY - rtText.MidY; break;
                        case DvContentAlignment.MiddleCenter: xText = rect.MidX - rtText.MidX; yText = rect.MidY - rtText.MidY; break;
                        case DvContentAlignment.MiddleRight: xText = rect.Right - rtText.Right; yText = rect.MidY - rtText.MidY; break;

                        case DvContentAlignment.BottomLeft: xText = rect.Left - rtText.Left; yText = rect.Bottom - rtText.Bottom; break;
                        case DvContentAlignment.BottomCenter: xText = rect.MidX - rtText.MidX; yText = rect.Bottom - rtText.Bottom; break;
                        case DvContentAlignment.BottomRight: xText = rect.Right - rtText.Right; yText = rect.Bottom - rtText.Bottom; break;
                    }

                    canvas.DrawText(fai.IconText, xText, yText, paint);
                    #endregion
                }
            }
        }
        #endregion
        #region DrawTextIcon
        public static void DrawTextIcon(SKCanvas canvas,
            string IconString, float IconSize, DvTextIconAlignment IconAlignment, float IconGap,
            string Text, string FontName, float FontSize, DvFontStyle FontStyle, Padding TextPadding,
            SKColor colortext, SKColor coloricon, SKRect rt, DvContentAlignment align = DvContentAlignment.MiddleCenter)
        {
            var vrt = Util.FromRect(rt, TextPadding);

            var fai = FA.GetFAI(IconString);
            if (fai != null)
            {
                var vrtFA = Util.FromRect(0, 0, IconSize + 2, IconSize + 2);

                TextIconBounds(Text, FontName, FontSize, FontStyle, IconGap, vrt, vrtFA, IconAlignment, align, (rtFA, rtTX) =>
                {
                    DrawIcon(canvas, IconString, IconSize, colortext, rtFA, DvContentAlignment.MiddleCenter);
                    DrawText(canvas, Text, FontName, FontSize, FontStyle, coloricon, rtTX, DvContentAlignment.MiddleCenter);
                });
            }
            else
            {
                DrawText(canvas, Text, FontName, FontSize, FontStyle, colortext, vrt, align);
            }
        }

        public static void DrawTextIcon(SKCanvas canvas,
            SKBitmap IconImage, float IconSize, DvTextIconAlignment IconAlignment, float IconGap,
            string Text, string FontName, float FontSize, DvFontStyle FontStyle, Padding TextPadding,
            SKColor colortext, SKColor coloricon, SKRect rt, DvContentAlignment align = DvContentAlignment.MiddleCenter)
        {
            var vrt = Util.FromRect(rt, TextPadding);

            if (IconImage != null)
            {
                var vrtFA = Util.FromRect(0, 0, IconSize + 2, IconSize + 2);

                TextIconBounds(Text, FontName, FontSize, FontStyle, IconGap, vrt, vrtFA, IconAlignment, align, (rtFA, rtTX) =>
                {
                    using (var paint = new SKPaint { IsAntialias = DvDesign.AA, SubpixelText = true })
                    {
                        canvas.DrawBitmap(IconImage, rtFA, paint);
                    }
                    DrawText(canvas, Text, FontName, FontSize, FontStyle, coloricon, rtTX, DvContentAlignment.MiddleCenter);
                });
            }
            else
            {
                DrawText(canvas, Text, FontName, FontSize, FontStyle, colortext, vrt, align);
            }
        }

        public static void DrawTextIcon(SKCanvas canvas, DvTextIcon texticon, SKColor colortext, SKColor coloricon, SKRect rt, DvContentAlignment align = DvContentAlignment.MiddleCenter)
        {
            if (texticon.IconImage != null)
            {
                DrawTextIcon(canvas, texticon.IconImage, texticon.IconSize, texticon.IconAlignment, texticon.IconGap,
                     texticon.Text, texticon.FontName, texticon.FontSize, texticon.FontStyle, texticon.TextPadding,
                     colortext, coloricon, rt, align);
            }
            else
            {
                DrawTextIcon(canvas, texticon.IconString, texticon.IconSize, texticon.IconAlignment, texticon.IconGap,
                      texticon.Text, texticon.FontName, texticon.FontSize, texticon.FontStyle, texticon.TextPadding,
                      colortext, coloricon, rt, align);
            }
        }
        public static void DrawTextIcon(SKCanvas canvas, DvTextIcon texticon, SKColor color, SKRect rt, DvContentAlignment align = DvContentAlignment.MiddleCenter)
            => DrawTextIcon(canvas, texticon, color, color, rt, align);
        #endregion

        #region TextIconBounds
        public static void TextIconBounds(DvTextIcon ti, SKRect rt, SKRect rtIcon, DvContentAlignment align, Action<SKRect, SKRect> act) => TextIconBounds(ti.Text, ti.FontName, ti.FontSize, ti.FontStyle, ti.IconGap, rt, rtIcon, ti.IconAlignment, align, act);
        public static void TextIconBounds(string text, string font, float textsize, DvFontStyle style, float gap, SKRect rt, SKRect rtIcon, DvTextIconAlignment talign, DvContentAlignment align, Action<SKRect, SKRect> act)
        {
            using (var paint = new SKPaint())
            {
                if (string.IsNullOrEmpty(text))
                {
                    act(Util.MakeRectangleAlign(rt, new SKSize(Convert.ToInt32(rtIcon.Width), Convert.ToInt32(rtIcon.Height)), align), Util.FromRect(0, 0, 0, 0));
                }
                else
                {
                    paint.Typeface = FontTool.GetFont(font, style);
                    paint.TextSize = textsize;

                    #region var ls;
                    var ls = new List<TI>();
                    foreach (var v in text.Replace("\r\n", "\n").Split('\n'))
                    {
                        var rtv = new SKRect();
                        paint.MeasureText(v, ref rtv);
                        rtv.Right = rtv.Left + paint.MeasureText(v);
                        ls.Add(new TI() { Text = v, Bounds = rtv });
                    }
                    #endregion

                    var TK = paint.FontSpacing / 4F;
                    var TH = (ls.Count * paint.FontSpacing) + (TK * (ls.Count - 1));
                    var TW = ls.Max(x => x.Bounds.Width);
                    var rtmTX = MakeRectangleAlign(rt, new SKSize(TW, TH), align);

                    var rtmFA = rtIcon;

                    var vgap = string.IsNullOrWhiteSpace(text) ? 0 : gap;
                    var szv = MeasureTextIcon(rtmTX, rtIcon, talign, vgap);
                    var rtb = MakeRectangleAlign(rt, szv, align);

                    paint.IsStroke = true;

                    if (talign == DvTextIconAlignment.LeftRight)
                    {
                        var rtFA = FromRect(rtb.Left, rtb.MidY - rtmFA.Height / 2, rtmFA.Width, rtmFA.Height);
                        var rtTX = FromRect(rtb.Right - rtmTX.Width, rtb.MidY - rtmTX.Height / 2, rtmTX.Width, rtmTX.Height);

                        act(rtFA, rtTX);
                    }
                    else
                    {
                        var rtFA = FromRect(rtb.MidX - rtmFA.Width / 2, rtb.Top, rtmFA.Width, rtmFA.Height);
                        var rtTX = FromRect(rtb.MidX - rtmTX.Width / 2, rtb.Bottom - rtmTX.Height, rtmTX.Width, rtmTX.Height);

                        act(rtFA, rtTX);
                    }

                }
            }
        }
        #endregion

        #region MakeRectangleAlign
        public static SKRect MakeRectangleAlign(SKRect rect, SKSize sz, DvContentAlignment dv)
        {
            var xText = rect.Left;
            var yText = rect.Top;

            switch (dv)
            {
                case DvContentAlignment.TopLeft: xText = rect.Left; yText = rect.Top; break;
                case DvContentAlignment.TopCenter: xText = rect.MidX - sz.Width / 2F; yText = rect.Top; break;
                case DvContentAlignment.TopRight: xText = rect.Right - sz.Width; yText = rect.Top; break;

                case DvContentAlignment.MiddleLeft: xText = rect.Left; yText = rect.MidY - sz.Height / 2F; break;
                case DvContentAlignment.MiddleCenter: xText = rect.MidX - sz.Width / 2F; yText = rect.MidY - sz.Height / 2F; break;
                case DvContentAlignment.MiddleRight: xText = rect.Right - sz.Width; yText = rect.MidY - sz.Height / 2F; break;

                case DvContentAlignment.BottomLeft: xText = rect.Left; yText = rect.Bottom - sz.Height; break;
                case DvContentAlignment.BottomCenter: xText = rect.MidX - sz.Width / 2F; yText = rect.Bottom - sz.Height; break;
                case DvContentAlignment.BottomRight: xText = rect.Right - sz.Width; yText = rect.Bottom - sz.Height; break;
            }
            return Util.FromRect(xText, yText, sz.Width, sz.Height);
        }
        #endregion
        #region MakeRectangle
        public static SKRect MakeRectangle(SKRect rect, SKSize size) => FromRect(rect.Left + (rect.Width / 2) - (size.Width / 2), rect.Top + (rect.Height / 2) - (size.Height / 2), size.Width, size.Height);
        public static SKRect MakeRectangle(SKPoint cp, float width, float height) => FromRect(cp.X - (width / 2F), cp.Y - (height / 2), width, height);
        #endregion

        #region INT
        public static int INT(float v) => (int)v;
        public static SKPoint INT(SKPoint v) => new SKPoint((int)v.X, (int)v.Y);
        public static SKSize INT(SKSize v) => new SKSize((int)v.Width, (int)v.Height);
        public static SKRect INT(SKRect v) => new SKRect((int)v.Left, (int)v.Top, (int)v.Right, (int)v.Bottom);
        #endregion

        #region CenterPoint
        public static SKPoint CenterPoint(List<SKPoint> vertices)
        {
            SKPoint centroid = new SKPoint() { X = 0.0F, Y = 0.0F };
            float signedArea = 0.0F;
            float x0 = 0.0F; // Current vertex X
            float y0 = 0.0F; // Current vertex Y
            float x1 = 0.0F; // Next vertex X
            float y1 = 0.0F; // Next vertex Y
            float a = 0.0F;  // Partial signed area

            // For all vertices except last
            int i = 0;
            for (i = 0; i < vertices.Count - 1; ++i)
            {
                x0 = vertices[i].X;
                y0 = vertices[i].Y;
                x1 = vertices[i + 1].X;
                y1 = vertices[i + 1].Y;
                a = x0 * y1 - x1 * y0;
                signedArea += a;
                centroid.X += (x0 + x1) * a;
                centroid.Y += (y0 + y1) * a;
            }

            // Do last vertex
            x0 = vertices[i].X;
            y0 = vertices[i].Y;
            x1 = vertices[0].X;
            y1 = vertices[0].Y;
            a = x0 * y1 - x1 * y0;
            signedArea += a;
            centroid.X += (x0 + x1) * a;
            centroid.Y += (y0 + y1) * a;

            signedArea *= 0.5F;
            centroid.X /= (6 * signedArea);
            centroid.Y /= (6 * signedArea);

            return centroid;
        }
        #endregion
        #region RoundCorners
        public static SKPath RoundCorners(SKRect r, float radius) { return RoundCorners(new SKPoint[] { new SKPoint(r.Left, r.Top), new SKPoint(r.Right, r.Top), new SKPoint(r.Right, r.Bottom), new SKPoint(r.Left, r.Bottom) }, radius); }
        public static SKPath RoundCorners(SKPoint[] points, float radius)
        {
            SKPath retval = new SKPath();
            if (points.Length < 3) throw new ArgumentException();

            var rects = new SKRect[points.Length];
            SKPoint pt1, pt2;
            Vector v1, v2, n1 = new Vector(), n2 = new Vector();
            SizeF size = new SizeF(2 * radius, 2 * radius);
            SKPoint center = new SKPoint();

            for (int i = 0; i < points.Length; i++)
            {
                pt1 = points[i]; 
                pt2 = points[i == points.Length - 1 ? 0 : i + 1]; 
                v1 = new Vector(pt2.X, pt2.Y) - new Vector(pt1.X, pt1.Y); 
                pt2 = points[i == 0 ? points.Length - 1 : i - 1]; 
                v2 = new Vector(pt2.X, pt2.Y) - new Vector(pt1.X, pt1.Y); 
                
                float sweepangle = (float)Vector.AngleBetween(v1, v2);
                if (sweepangle < 0)
                {
                    n1 = new Vector(v1.Y, -v1.X);
                    n2 = new Vector(-v2.Y, v2.X);
                }
                else
                {
                    n1 = new Vector(-v1.Y, v1.X);
                    n2 = new Vector(v2.Y, -v2.X);
                }

                n1.Normalize(); n2.Normalize();
                n1 *= radius; n2 *= radius;
                
                SKPoint pt = points[i];
                pt1 = new SKPoint((float)(pt.X + n1.X), (float)(pt.Y + n1.Y));
                pt2 = new SKPoint((float)(pt.X + n2.X), (float)(pt.Y + n2.Y));

                double m1 = v1.Y / v1.X, m2 = v2.Y / v2.X;
                if (v1.X == 0)
                {
                    center.X = pt1.X;
                    center.Y = (float)(m2 * (pt1.X - pt2.X) + pt2.Y);
                }
                else if (v1.Y == 0)
                {
                    center.X = (float)((pt1.Y - pt2.Y) / m2 + pt2.X);
                    center.Y = pt1.Y;
                }
                else if (v2.X == 0)
                {
                    center.X = pt2.X;
                    center.Y = (float)(m1 * (pt2.X - pt1.X) + pt1.Y);
                }
                else if (v2.Y == 0)
                {
                    center.X = (float)((pt2.Y - pt1.Y) / m1 + pt1.X);
                    center.Y = pt2.Y;
                }
                else
                {
                    center.X = (float)((pt2.Y - pt1.Y + m1 * pt1.X - m2 * pt2.X) / (m1 - m2));
                    center.Y = (float)(pt1.Y + m1 * (center.X - pt1.X));
                }

                rects[i] = Util.FromRect(center.X - 2, center.Y - 2, 4, 4);
                n1.Negate(); n2.Negate();
                pt1 = new SKPoint((float)(center.X + n1.X), (float)(center.Y + n1.Y));
                pt2 = new SKPoint((float)(center.X + n2.X), (float)(center.Y + n2.Y));
              
                var  rect = Util.FromRect(center.X - radius, center.Y - radius, size.Width, size.Height);
                sweepangle = (float)Vector.AngleBetween(n2, n1);
                if (i == 0) retval.AddArc(rect, (float)Vector.AngleBetween(new Vector(1, 0), n2), sweepangle);
                else retval.ArcTo(rect, (float)Vector.AngleBetween(new Vector(1, 0), n2), sweepangle, false);
            }
            retval.Close();
            return retval;
        }

        public SKPath RoundedCorner(SKPoint angularPoint, SKPoint p1, SKPoint p2, float radius)
        {
            double dx1 = angularPoint.X - p1.X;
            double dy1 = angularPoint.Y - p1.Y;

            double dx2 = angularPoint.X - p2.X;
            double dy2 = angularPoint.Y - p2.Y;

            double angle = (Math.Atan2(dy1, dx1) - Math.Atan2(dy2, dx2)) / 2;

            double tan = Math.Abs(Math.Tan(angle));
            double segment = radius / tan;

            double length1 = GetLength(dx1, dy1);
            double length2 = GetLength(dx2, dy2);

            double length = Math.Min(length1, length2);

            if (segment > length)
            {
                segment = length;
                radius = (float)(length * tan);
            }

            var p1Cross = GetProportionPoint(angularPoint, segment, length1, dx1, dy1);
            var p2Cross = GetProportionPoint(angularPoint, segment, length2, dx2, dy2);

            double dx = angularPoint.X * 2 - p1Cross.X - p2Cross.X;
            double dy = angularPoint.Y * 2 - p1Cross.Y - p2Cross.Y;

            double L = GetLength(dx, dy);
            double d = GetLength(segment, radius);

            var circlePoint = GetProportionPoint(angularPoint, d, L, dx, dy);

            var startAngle = Math.Atan2(p1Cross.Y - circlePoint.Y, p1Cross.X - circlePoint.X);
            var endAngle = Math.Atan2(p2Cross.Y - circlePoint.Y, p2Cross.X - circlePoint.X);

            var sweepAngle = endAngle - startAngle;

            if (sweepAngle < 0)
            {
                startAngle = endAngle;
                sweepAngle = -sweepAngle;
            }

            if (sweepAngle > Math.PI)
                sweepAngle = Math.PI - sweepAngle;

            SKPath pth = new SKPath();
            pth.MoveTo(p1);  pth.LineTo(p1Cross);
            pth.LineTo(p2); pth.LineTo(p2Cross);

            var left = circlePoint.X - radius;
            var top = circlePoint.Y - radius;
            var diameter = 2 * radius;
            var degreeFactor = 180 / Math.PI;
            pth.ArcTo(Util.FromRect(left, top, diameter, diameter), (float)(startAngle * degreeFactor), (float)(sweepAngle * degreeFactor), false);

            return pth;
        }

        private double GetLength(double dx, double dy)
        {
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private SKPoint GetProportionPoint(SKPoint point, double segment,
                                          double length, double dx, double dy)
        {
            double factor = segment / length;

            return new SKPoint((float)(point.X - dx * factor), (float)(point.Y - dy * factor));
        }
        #endregion

        #region DevideSize
        public static SKRect[] DevideSizeH(SKRect bounds, List<SizeInfo> cols)
        {
            var ret = new SKRect[cols.Count];
            var tw = bounds.Width;
            var cw = tw - cols.Where(x => x.Mode == DvSizeMode.Pixel).Sum(x => x.Size);

            float x = bounds.Left;
            for (int i = 0; i < cols.Count; i++)
            {
                var v = cols[i];
                var w = v.Mode == DvSizeMode.Pixel ? (v.Size) : (cw * (v.Size / 100F));

                ret[i] = Util.FromRect(x, bounds.Top, w, bounds.Height);
                x += w;
            }
            return ret;
        }

        public static SKRect[] DevideSizeV(SKRect bounds, List<SizeInfo> rows)
        {
            var ret = new SKRect[rows.Count];
            var th = bounds.Height;
            var ch = th - rows.Where(x => x.Mode == DvSizeMode.Pixel).Sum(x => x.Size);

            float y = bounds.Top;
            for (int i = 0; i < rows.Count; i++)
            {
                var v = rows[i];
                var h = v.Mode == DvSizeMode.Pixel ? (v.Size) : (ch * (v.Size / 100F));

                ret[i] = Util.FromRect(bounds.Left, y, bounds.Width, h);
                y += h;
            }
            return ret;
        }

        public static SKRect[,] DevideSizeVH(SKRect bounds, List<SizeInfo> rows, List<SizeInfo> cols)
        {
            var ret = new SKRect[rows.Count, cols.Count];
            var th = bounds.Height;
            var tw = bounds.Width;
            var cw = tw - cols.Where(x => x.Mode == DvSizeMode.Pixel).Sum(x => x.Size);
            var ch = th - rows.Where(x => x.Mode == DvSizeMode.Pixel).Sum(x => x.Size);

            float y = bounds.Top;
            for (int ir = 0; ir < rows.Count; ir++)
            {
                var vr = rows[ir];
                var h = vr.Mode == DvSizeMode.Pixel ? (vr.Size) : (ch * (vr.Size / 100F));

                float x = bounds.Left;
                for (int ic = 0; ic < cols.Count; ic++)
                {
                    var vc = cols[ic];
                    var w = vc.Mode == DvSizeMode.Pixel ? (vc.Size) : (cw * (vc.Size / 100F));

                    ret[ir, ic] = Util.FromRect(x, y, w, h);
                    x += w;
                }

                y += h;
            }
            return ret;
        }
        #endregion
        #region MergeBounds
        public static SKRect MergeBounds(SKRect[,] rts, int col, int row, int colspan, int rowspan)
        {
            var rtLT = Util.FromRect(rts[row, col]);
            var rtRB = rts[row + rowspan - 1, col + colspan - 1];
            rtLT.Right = rtRB.Right;
            rtLT.Bottom = rtRB.Bottom;

            return rtLT;
        }

        public static SKRect MergeBoundsH(SKRect[] rts, int col, int colspan)
        {
            var rtL = Util.FromRect(rts[col]);
            var rtR = rts[col + colspan - 1];
            rtL.Right = rtR.Right;

            return rtL;
        }

        public static SKRect MergeBoundsV(SKRect[] rts, int row, int rowspan)
        {
            var rtT = Util.FromRect(rts[row]);
            var rtB = rts[row + rowspan - 1];
            rtT.Bottom = rtB.Bottom;

            return rtT;
        }

        public static SKRect MergeBounds(SKRect rt1, SKRect rt2)
        {
            var L = Math.Min(rt1.Left, rt2.Left);
            var R = Math.Max(rt1.Right, rt2.Right);
            var T = Math.Min(rt1.Top, rt2.Top);
            var B = Math.Max(rt1.Bottom, rt2.Bottom);

            return new SKRect(L, T, R, B);
        }
        #endregion

        #region Rounds
        public static DvRoundType[] Rounds(DvDirectionHV dir, DvRoundType round, int count)
        {
            var ret = new DvRoundType[count];

            for (int i = 0; i < count; i++) ret[i] = DvRoundType.Rect;

            if (ret.Length == 1)
            {
                ret[0] = round;
            }
            else if (ret.Length > 1)
            {
                var si = 0;
                var ei = count - 1;

                if (dir == DvDirectionHV.Horizon)
                {
                    switch (round)
                    {
                        #region L / T / R / B
                        case DvRoundType.L: ret[si] = DvRoundType.L; ret[ei] = DvRoundType.Rect; break;
                        case DvRoundType.R: ret[si] = DvRoundType.Rect; ret[ei] = DvRoundType.R; break;
                        case DvRoundType.T: ret[si] = DvRoundType.LT; ret[ei] = DvRoundType.RT; break;
                        case DvRoundType.B: ret[si] = DvRoundType.LB; ret[ei] = DvRoundType.RB; break;
                        #endregion

                        #region LT / RT / LB / RB
                        case DvRoundType.LT: ret[si] = DvRoundType.LT; ret[ei] = DvRoundType.Rect; break;
                        case DvRoundType.RT: ret[si] = DvRoundType.Rect; ret[ei] = DvRoundType.RT; break;
                        case DvRoundType.LB: ret[si] = DvRoundType.LB; ret[ei] = DvRoundType.Rect; break;
                        case DvRoundType.RB: ret[si] = DvRoundType.Rect; ret[ei] = DvRoundType.RB; break;
                        #endregion

                        #region All
                        case DvRoundType.All: ret[si] = DvRoundType.L; ret[ei] = DvRoundType.R; break;
                            #endregion
                    }
                }
                else if (dir == DvDirectionHV.Vertical)
                {
                    switch (round)
                    {
                        #region L / T / R / B
                        case DvRoundType.L: ret[si] = DvRoundType.LT; ret[ei] = DvRoundType.LB; break;
                        case DvRoundType.R: ret[si] = DvRoundType.RT; ret[ei] = DvRoundType.RB; break;
                        case DvRoundType.T: ret[si] = DvRoundType.T; ret[ei] = DvRoundType.Rect; break;
                        case DvRoundType.B: ret[si] = DvRoundType.Rect; ret[ei] = DvRoundType.B; break;
                        #endregion

                        #region LT / RT / LB / RB
                        case DvRoundType.LT: ret[si] = DvRoundType.LT; ret[ei] = DvRoundType.Rect; break;
                        case DvRoundType.RT: ret[si] = DvRoundType.RT; ret[ei] = DvRoundType.Rect; break;
                        case DvRoundType.LB: ret[si] = DvRoundType.Rect; ret[ei] = DvRoundType.LB; break;
                        case DvRoundType.RB: ret[si] = DvRoundType.Rect; ret[ei] = DvRoundType.RB; break;
                        #endregion

                        #region All
                        case DvRoundType.All: ret[si] = DvRoundType.T; ret[ei] = DvRoundType.B; break;
                            #endregion
                    }
                }
            }
            return ret;
        }
        #endregion

        #region GetPolygon
        public static SKPoint[] GetPolygon(DvDirection Direction, SKRect rt)
        {
            var a = Convert.ToSingle(Math.Min(rt.Width, rt.Height));
            var wh = Convert.ToSingle((Math.Sqrt(3.0) / 2.0 / 3.0) * a * 2.0);

            var ls = new List<SKPoint>();
            {
                var cp = MathTool.CenterPoint(rt);
                switch (Direction)
                {
                    case DvDirection.Up:
                        {
                            var sang = -90;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            var pts = new SKPoint[] { p1, p2, p3 };
                            var cy = MathTool.Center(pts.Min(x => x.Y), pts.Max(x => x.Y));
                            var gy = cp.Y - cy;

                            p1.Y += gy;
                            p2.Y += gy;
                            p3.Y += gy;

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;

                    case DvDirection.Down:
                        {
                            var sang = 90;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            var pts = new SKPoint[] { p1, p2, p3 };
                            var cy = MathTool.Center(pts.Min(x => x.Y), pts.Max(x => x.Y));
                            var gy = cp.Y - cy;

                            p1.Y += gy;
                            p2.Y += gy;
                            p3.Y += gy;

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;

                    case DvDirection.Left:
                        {
                            var sang = 180;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            var pts = new SKPoint[] { p1, p2, p3 };
                            var cx = MathTool.Center(pts.Min(x => x.X), pts.Max(x => x.X));
                            var gx = cp.X - cx;

                            p1.X += gx;
                            p2.X += gx;
                            p3.X += gx;

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;

                    case DvDirection.Right:
                        {
                            var sang = 0;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            var pts = new SKPoint[] { p1, p2, p3 };
                            var cx = MathTool.Center(pts.Min(x => x.X), pts.Max(x => x.X));
                            var gx = cp.X - cx;

                            p1.X += gx;
                            p2.X += gx;
                            p3.X += gx;

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;
                }
            }
            return ls.Count == 3 ? ls.ToArray() : null;
        }
        #endregion
        #region GetPolygonBtn
        public static SKPoint[] GetPolygonBtn(DvDirection Direction, SKPoint cp, SKRect rt,  float ng)
        {
            var a = Convert.ToSingle(Math.Min(rt.Width, rt.Height));
            var wh = Convert.ToSingle((Math.Sqrt(3.0) / 2.0 / 3.0) * a * 2.0) - (ng * 2F);

            var ls = new List<SKPoint>();
            {
                switch (Direction)
                {
                    case DvDirection.Up:
                        {
                            var sang = -90;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;

                    case DvDirection.Down:
                        {
                            var sang = 90;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;

                    case DvDirection.Left:
                        {
                            var sang = 180;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;

                    case DvDirection.Right:
                        {
                            var sang = 0;
                            var p1 = MathTool.GetPointWithAngle(cp, sang + 0, wh);
                            var p2 = MathTool.GetPointWithAngle(cp, sang + 120, wh);
                            var p3 = MathTool.GetPointWithAngle(cp, sang + 240, wh);

                            ls.Add(p1);
                            ls.Add(p2);
                            ls.Add(p3);
                        }
                        break;
                }
            }
            return ls.Count == 3 ? ls.ToArray() : null;
        }
        #endregion

        #region SetRound
        public static void SetRound(SKRoundRect rt, DvRoundType round, float corner)
        {
            switch (round)
            {
                case DvRoundType.Rect: rt.SetNinePatch(rt.Rect, 0, 0, 0, 0); break;
                case DvRoundType.All: rt.SetNinePatch(rt.Rect, corner, corner, corner, corner); break;
                case DvRoundType.L: rt.SetNinePatch(rt.Rect, corner, corner, 0, corner); break;
                case DvRoundType.R: rt.SetNinePatch(rt.Rect, 0, corner, corner, corner); break;
                case DvRoundType.T: rt.SetNinePatch(rt.Rect, corner, corner, corner, 0); break;
                case DvRoundType.B: rt.SetNinePatch(rt.Rect, corner, 0, corner, corner); break;
                case DvRoundType.LT: rt.SetNinePatch(rt.Rect, corner, corner, 0, 0); break;
                case DvRoundType.RT: rt.SetNinePatch(rt.Rect, 0, corner, corner, 0); break;
                case DvRoundType.LB: rt.SetNinePatch(rt.Rect, corner, 0, 0, corner); break;
                case DvRoundType.RB: rt.SetNinePatch(rt.Rect, 0, 0, corner, corner); break;
                case DvRoundType.Ellipse: rt.SetOval(rt.Rect); break;
            }
        }
        #endregion

    }

    #region struct : Vector
    internal struct Vector
    {
        #region Properties
        #region X
        internal double _x;
        public double X
        {
            get
            {
                return _x;
            }

            set
            {
                _x = value;
            }

        }
        #endregion
        #region Y
        internal double _y;
        public double Y
        {
            get
            {
                return _y;
            }

            set
            {
                _y = value;
            }

        }
        #endregion
        #region Length
        public double Length
        {
            get
            {
                return Math.Sqrt(_x * _x + _y * _y);
            }
        }
        #endregion
        #region LengthSquared
        public double LengthSquared
        {
            get
            {
                return _x * _x + _y * _y;
            }
        }
        #endregion
        #endregion

        #region Constructor
        public Vector(double x, double y)
        {
            _x = x;
            _y = y;
        }
        #endregion

        #region Method
        #region Equals
        public override bool Equals(object o)
        {
            if ((null == o) || !(o is Vector))
            {
                return false;
            }

            Vector value = (Vector)o;
            return Vector.Equals(this, value);
        }

        public bool Equals(Vector value)
        {
            return Vector.Equals(this, value);
        }
        #endregion
        #region GetHashCode
        public override int GetHashCode()
        {
            // Perform field-by-field XOR of HashCodes
            return X.GetHashCode() ^
                   Y.GetHashCode();
        }
        #endregion
        #region Normalize
        public void Normalize()
        {
            this /= Math.Max(Math.Abs(_x), Math.Abs(_y));
            this /= Length;
        }
        #endregion
        #region Negate
        public void Negate()
        {
            _x = -_x;
            _y = -_y;
        }
        #endregion
        #endregion

        #region Static Method
        #region Equals
        public static bool Equals(Vector vector1, Vector vector2)
        {
            return vector1.X.Equals(vector2.X) &&
                   vector1.Y.Equals(vector2.Y);
        }
        #endregion
        #region CrossProduct
        public static double CrossProduct(Vector vector1, Vector vector2)
        {
            return vector1._x * vector2._y - vector1._y * vector2._x;
        }
        #endregion
        #region AngleBetween
        public static double AngleBetween(Vector vector1, Vector vector2)
        {
            double sin = vector1._x * vector2._y - vector2._x * vector1._y;
            double cos = vector1._x * vector2._x + vector1._y * vector2._y;

            return Math.Atan2(sin, cos) * (180 / Math.PI);
        }
        #endregion
        #region Operator
        public static bool operator ==(Vector vector1, Vector vector2)
        {
            return vector1.X == vector2.X &&
                   vector1.Y == vector2.Y;
        }

        public static bool operator !=(Vector vector1, Vector vector2)
        {
            return !(vector1 == vector2);
        }

        public static Vector operator -(Vector vector)
        {
            return new Vector(-vector._x, -vector._y);
        }

        public static Vector operator +(Vector vector1, Vector vector2)
        {
            return new Vector(vector1._x + vector2._x,
                              vector1._y + vector2._y);
        }

        public static Vector Add(Vector vector1, Vector vector2)
        {
            return new Vector(vector1._x + vector2._x,
                              vector1._y + vector2._y);
        }

        public static Vector operator -(Vector vector1, Vector vector2)
        {
            return new Vector(vector1._x - vector2._x,
                              vector1._y - vector2._y);
        }

        public static Vector Subtract(Vector vector1, Vector vector2)
        {
            return new Vector(vector1._x - vector2._x,
                              vector1._y - vector2._y);
        }

        public static PointF operator +(Vector vector, Point point)
        {
            return new PointF(Convert.ToSingle(point.X + vector.X), Convert.ToSingle(point.Y + vector.Y));
        }

        public static PointF Add(Vector vector, Point point)
        {
            return new PointF(Convert.ToSingle(point.X + vector.X), Convert.ToSingle(point.Y + vector.Y));
        }

        public static Vector operator *(Vector vector, double scalar)
        {
            return new Vector(vector._x * scalar,
                              vector._y * scalar);
        }

        public static Vector Multiply(Vector vector, double scalar)
        {
            return new Vector(vector._x * scalar,
                              vector._y * scalar);
        }

        public static Vector operator *(double scalar, Vector vector)
        {
            return new Vector(vector._x * scalar,
                              vector._y * scalar);
        }

        public static Vector Multiply(double scalar, Vector vector)
        {
            return new Vector(vector._x * scalar,
                              vector._y * scalar);
        }

        public static Vector operator /(Vector vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }

        public static Vector Divide(Vector vector, double scalar)
        {
            return vector * (1.0 / scalar);
        }

        /*
        public static Vector operator *(Vector vector, Matrix matrix)
        {
            return matrix.Transform(vector);
        }

        public static Vector Multiply(Vector vector, Matrix matrix)
        {
            return matrix.Transform(vector);
        }
        */
        public static double operator *(Vector vector1, Vector vector2)
        {
            return vector1._x * vector2._x + vector1._y * vector2._y;
        }

        public static double Multiply(Vector vector1, Vector vector2)
        {
            return vector1._x * vector2._x + vector1._y * vector2._y;
        }

        public static double Determinant(Vector vector1, Vector vector2)
        {
            return vector1._x * vector2._y - vector1._y * vector2._x;
        }
        /*
        public static explicit operator Size(Vector vector)
        {
            return new Size(Math.Abs(vector._x), Math.Abs(vector._y));
        }

        public static explicit operator Point(Vector vector)
        {
            return new Point(vector._x, vector._y);
        }
        */
        #endregion
        #endregion
    }
    #endregion
    #region class : TI
    class TI
    {
        public string Text { get; set; }
        public SKRect Bounds { get; set; }
    }
    #endregion

}

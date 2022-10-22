using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
using Devinno.Skia.Theme;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvPictureBox : DvControl
    {
        #region Properties
        public SKBitmap Image { get; set; }
        public PictureScaleMode ScaleMode { get; set; } = PictureScaleMode.Strech;
        public SKColor? BoxColor { get; set; } = null;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var BoxColor = this.BoxColor ?? thm.ConcaveBoxColor;
                    var BorderColor = thm.GetBorderColor(BoxColor, ParentContainer.GetBackColor());
                    var Corner = thm.Corner;

                    var rt = new SKRoundRect(rtContent, Corner);

                    var sp = Canvas.Save();
                    Canvas.ClipRoundRect(rt);
                    thm.DrawBox(Canvas, rtContent, BoxColor, BorderColor, RoundType.Round, BoxStyle.Fill | BoxStyle.OutShadow);
                    if (Image != null)
                    {
                        #region Image
                        var cx = rtContent.MidX;
                        var cy = rtContent.MidY;
                        switch (ScaleMode)
                        {
                            case PictureScaleMode.Real:
                                Canvas.DrawBitmap(Image, Util.FromRect(rtContent.Left, rtContent.Top, Image.Width, Image.Height));
                                break;
                            case PictureScaleMode.CenterImage:
                                Canvas.DrawBitmap(Image, Util.FromRect(cx - (Image.Width / 2), cy - (Image.Height / 2), Image.Width, Image.Height));
                                break;
                            case PictureScaleMode.Strech:
                                Canvas.DrawBitmap(Image, rtContent);
                                break;
                            case PictureScaleMode.Zoom:
                                double imgratio = 1D;
                                if ((Image.Width - rtContent.Width) > (Image.Height - rtContent.Height)) imgratio = (double)rtContent.Width / (double)Image.Width;
                                else imgratio = (double)rtContent.Height / (double)Image.Height;

                                int szw = Convert.ToInt32((double)Image.Width * imgratio);
                                int szh = Convert.ToInt32((double)Image.Height * imgratio);

                                Canvas.DrawBitmap(Image, Util.FromRect(rtContent.Left + (rtContent.Width / 2) - (szw / 2), rtContent.Top + (rtContent.Height / 2) - (szh / 2), szw, szh));
                                break;
                        }
                        #endregion
                    }
                    else
                    {
                        var c = BoxColor.BrightnessTransmit(1F);
                        Util.DrawText(Canvas, "No Image", "NanumGothic", 12, DvFontStyle.Normal, c, rtContent);
                    }
                    Canvas.RestoreToCount(sp);
                    
                    thm.DrawBox(Canvas, rtContent, BoxColor, BorderColor, RoundType.Round, BoxStyle.Border);

                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);

            act(rtContent);
        }
        #endregion
        #endregion
    }

    #region enum : PictureScaleMode
    public enum PictureScaleMode { Real, CenterImage, Strech, Zoom }
    #endregion
}

using Devinno.Skia.Design;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvPictureBox : Design.DvControl
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
            Areas((rtContent) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var BoxColor = this.BoxColor ?? thm.ControlBackColor;
                    var BackColor = ParentContainer.GetBackColor();

                    thm.DrawPictureBox(Canvas, rtContent, BoxColor, BackColor, ScaleMode, Image);
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);

            act(rtContent);
        }
        #endregion
        #endregion
    }
}

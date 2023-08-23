using Devinno.Skia.Design;
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
    public class DvLamp : Design.DvControl
    {
        #region Properties
        #region Text
        public string Text { get; set; }
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        #endregion
        #region ContentAlignment
        public DvContentAlignment ContentAlignment { get; set; } = DvContentAlignment.MiddleCenter;
        #endregion
        #region Color
        public SKColor? OnLampColor { get; set; } = null;
        public SKColor? OffLampColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        #endregion
        #region Lamp
        public int LampSize { get; set; } = 24;
        public DvTextIconAlignment LampAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        #endregion

        #region OnOff
        private bool bOnOff = false;
        public bool OnOff
        {
            get { return bOnOff; }
            set
            {
                if (bOnOff != value)
                {
                    bOnOff = value;

                    if (Animation)
                    {
                        ani.Stop();
                        ani.Start(DvDesign.ANI, OnOff ? "On" : "Off");
                    }
                }
            }
        }
        #endregion

        public bool Animation => Design != null ? Design.Animation : false;
        #endregion

        #region Member Variable
        private Animation ani = new Animation();
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
                    var cOn = this.OnLampColor ?? thm.PointColor;
                    var cOff = this.OffLampColor ?? thm.ButtonColor;
                    var cFore = this.ForeColor ?? thm.ForeColor;
                    var cBack = ParentContainer.GetBackColor();
                    var cBorder = thm.GetBorderColor(cBack, cBack);

                    var Corner = thm.Corner;

                    thm.DrawLamp(Canvas, rtContent,
                                cOn, cOff, cFore, cBack, cBorder,
                                Text, FontName, FontSize, FontStyle,
                                LampSize, LampAlignment,
                                ContentAlignment, OnOff, Animation, ani);
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
            var rtContent = Util.FromRect(0, 0, this.Width, this.Height);
            act(rtContent);
        }
        #endregion
        #endregion
    }
}

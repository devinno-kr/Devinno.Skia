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
    public class DvCheckBox : Design.DvControl
    {
        #region Properties
        #region Text
        public string Text { get; set; }
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        #endregion
        #region ContentAlignment
        public DvContentAlignment ContentAlignment { get; set; } = DvContentAlignment.MiddleLeft;
        #endregion
        #region Color
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? CheckColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        #endregion
        #region BoxSize
        public int BoxSize { get; set; } = 20;
        #endregion

        #region Checked
        private bool bChecked = false;
        public bool Checked
        {
            get { return bChecked; }
            set
            {
                if (bChecked != value)
                {
                    bChecked = value;
                    CheckedChanged?.Invoke(this, null);
                }
            }
        }
        #endregion
        #endregion

        #region Event
        public event EventHandler CheckedChanged;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtArea) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var BoxColor = this.BoxColor ?? thm.InputColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var CheckColor = this.CheckColor ?? thm.ForeColor;
                    var BorderColor = thm.GetBorderColor(BoxColor, ParentContainer.GetBackColor());

                    var Corner = thm.Corner;

                    thm.DrawCheckBox(Canvas, rtContent, BoxSize, BoxColor, BorderColor, CheckColor, ForeColor, Text, FontName, FontSize, FontStyle, ContentAlignment, Checked);
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion

        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtArea) =>
            {
                if (CollisionTool.Check(rtArea, x, y))
                {
                    Checked = !Checked;
                    Design?.Input(this);
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect> act)
        {
            var sz = Util.MeasureText(Text, FontName, FontSize, FontStyle);

            var rtContent = Util.FromRect(0, 0, this.Width, this.Height);
            var rtArea = Util.MakeRectangleAlign(rtContent, new SKSize(sz.Width + (!string.IsNullOrWhiteSpace(Text) ? 8 : 0) + BoxSize, Math.Max(sz.Height, BoxSize)), ContentAlignment);
            rtArea.Inflate(5, 5);
            act(rtContent, rtArea);
        }
        #endregion
        #endregion
    }
}

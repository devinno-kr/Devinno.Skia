using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
using Devinno.Skia.Theme;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvCheckBox : DvControl
    {
        #region Properties
        private SKColor? BoxColor { get; set; } = null;
        public SKColor? CheckColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public int BoxSize { get; set; } = 20;
        #region Text
        public string Text { get; set; } = "Text";
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
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
            bounds((rtContent, rtBox, rtCheck, rtText) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var BoxColor = this.BoxColor ?? thm.CheckBoxColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var CheckColor = this.CheckColor ?? thm.ForeColor;
                    var BorderColor = thm.GetBorderColor(BoxColor, ParentContainer.GetBackColor());

                    var Corner = thm.Corner;

                    thm.DrawBox(Canvas, rtBox, BoxColor, BorderColor, RoundType.Rect, BoxStyle.Fill | BoxStyle.Border | BoxStyle.InShadow | BoxStyle.OutBevel);
                    Util.DrawText(Canvas, Text, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtText, DvContentAlignment.MiddleLeft, true);

                    #region Check
                    if (Checked)
                    {
                        using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                        {
                            p.StrokeCap = SKStrokeCap.Round;
                            p.StrokeJoin = SKStrokeJoin.Round;
                            p.StrokeWidth = 4F;
                            p.Color = CheckColor;
                            p.IsStroke = true;

                            var points = new SKPoint[] { new SKPoint(rtCheck.Left, rtCheck.MidY),
                                                             new SKPoint(rtCheck.MidX, rtCheck.Bottom),
                                                             new SKPoint(rtCheck.Right, rtCheck.Top)};

                            var pth = new SKPath();
                            pth.MoveTo(points[0]);
                            pth.LineTo(points[1]);
                            pth.LineTo(points[2]);

                            Canvas.DrawPath(pth, p);
                        }
                    }
                    #endregion
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            bounds((rtContent, rtBox, rtCheck, rtText) =>
            {
                if (CollisionTool.Check(rtBox, x, y) || CollisionTool.Check(rtText, x, y))
                    Checked = !Checked;
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect> act)
        {
            var INF = BoxSize / 4;
            var GAP = 8;

            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
            var rtBox = Util.INT(Util.MakeRectangleAlign(rtContent, new SKSize(BoxSize, BoxSize), DvContentAlignment.MiddleLeft)); rtBox.Offset(0, 0);
            var rtText = Util.FromRect(rtBox.Right + GAP, rtContent.Top, rtContent.Width - (GAP + rtBox.Width), rtContent.Height - 1);
            var rtCheck = Util.FromRect(rtBox.Left, rtBox.Top, rtBox.Width, rtBox.Height); rtCheck.Inflate(-INF, -INF);

            act(rtContent, rtBox, rtCheck, rtText);
        }
        #endregion
        #endregion
    }
}

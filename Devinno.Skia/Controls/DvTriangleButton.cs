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
    public class DvTriangleButton : Design.DvControl
    {
        #region Properties
        #region Color
        public SKColor? ButtonColor { get; set; } = null;
        public SKColor? ButtonBackColor { get; set; } = null;
        #endregion
        #region Direction
        public DvDirection Direction { get; set; } = DvDirection.Left;
        #endregion

        #region Corner
        public int? Corner { get; set; } = null;
        #endregion
        #region ButtonPadding
        public float ButtonPadding { get; set; } = 5;
        #endregion
        #region ButtonBackPadding
        public float ButtonBackPadding { get; set; } = 0;
        #endregion

        #region Gradient
        public bool Gradient { get; set; } = true;
        #endregion
        #region Clickable
        public bool Clickable { get; set; } = true;
        #endregion
        #region ButtonDownState
        public bool ButtonDownState { get; private set; } = false;
        #endregion
        #endregion

        #region Event
        public event EventHandler ButtonClick;
        public event EventHandler ButtonDown;
        public event EventHandler ButtonUp;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            var ds = Design;
            var thm = Design?.Theme;
            if (ds != null && thm != null)
            {
                Areas((rtContent, rtBox) =>
                {
                    #region var
                    var cButton = ButtonColor ?? thm.ButtonColor;
                    var cButtonBack = ButtonBackColor ?? thm.ControlBackColor;
                    var corner = Corner ?? thm.Corner;
                    #endregion

                    thm.DrawTriangleButton(Canvas, 
                                            rtContent, rtBox,
                                            cButton, cButtonBack,
                                            Direction, Gradient,
                                            ButtonPadding, corner,  
                                            ButtonDownState);
                });
            }
            base.OnDraw(Canvas);
        }
        #endregion

        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            var ds = Design;
            var thm = Design?.Theme;
            if (ds != null && thm != null)
            {
                Areas((rtContent, rtBox) =>
                {
                    var pts = Util.GetPolygon(Direction, rtContent);
                    var ptsBtn = Util.GetPolygonBtn(Direction, MathTool.CenterPoint(pts.ToList()), rtContent, ButtonPadding);
                    if (Clickable && CollisionTool.CheckPolygon(ptsBtn, new SKPoint[] { new SKPoint(x, y), new SKPoint(x, y), new SKPoint(x, y) }))
                    {
                        ButtonDownState = true;
                        ButtonDown?.Invoke(this, null);
                        Design?.Input(this);
                    }
                });
            }
            base.OnMouseDown(x, y);
        }
        #endregion

        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            if (ButtonDownState)
            {
                ButtonDownState = false;
                ButtonUp?.Invoke(this, null);

                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    Areas((rtContent, rtBox) =>
                    {
                        var p = new SKPoint(x, y);
                        var pts = Util.GetPolygon(Direction, rtContent);
                        var ptsBtn = Util.GetPolygonBtn(Direction, MathTool.CenterPoint(pts.ToList()), rtContent, ButtonPadding);
                        if (Clickable && CollisionTool.CheckPolygon(ptsBtn, new SKPoint[] { p, p, p })) ButtonClick?.Invoke(this, null);
                    });
                }
            }
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);
            var cp = MathTool.CenterPoint(rtContent);
            var rtBox = MathTool.MakeRectangle(cp, Math.Min(rtContent.Width, rtContent.Height) - ButtonBackPadding);
            act(rtContent, rtBox);
        }
        #endregion
        #endregion
    }
}

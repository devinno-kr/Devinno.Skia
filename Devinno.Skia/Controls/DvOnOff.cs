using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvOnOff : Design.DvControl
    {
        #region Properties
        #region Color
        public SKColor? CursorColor { get; set; } = null;
        public SKColor? OnBoxColor { get; set; } = null;
        public SKColor? OffBoxColor { get; set; } = null;
        public SKColor? OnTextColor { get; set; } = null;
        public SKColor? OffTextColor { get; set; } = null;
        #endregion

        #region Text
        public string OnText { get; set; } = "ON";
        public string OffText { get; set; } = "OFF";
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        #endregion

        #region CursorPadding
        public float CursorPadding { get; set; } = 5;
        #endregion

        #region OnOff
        private bool bOnOff = false;
        public bool OnOff
        {
            get => bOnOff;
            set
            {
                if (bOnOff != value)
                {
                    bOnOff = value;
                    OnOffChanged?.Invoke(this, null);

                    if (Animation && !ptDown.HasValue)
                    {
                        ani.Stop();
                        ani.Start(200, bOnOff ? "ON" : "OFF");
                    }
                }
            }
        }
        #endregion

        #region Animation
        public bool Animation => Design != null ? Design.Animation : false;
        #endregion
        #endregion

        #region Event 
        public event EventHandler OnOffChanged;
        #endregion

        #region Member Variable
        private Animation ani = new Animation();
        private bool downOnOff = false;
        private SKPoint? ptDown = null;
        private SKPoint? ptMove = null;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtArea, rtCursor, rtOn, rtOff, cw, ow) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region Color
                    var CursorColor = this.CursorColor ?? thm.ButtonColor;
                    var OnBoxColor = this.OnBoxColor ?? thm.PointColor;
                    var OffBoxColor = this.OffBoxColor ?? thm.ControlBackColor;
                    var OnTextColor = this.OffBoxColor ?? thm.ForeColor;
                    var OffTextColor = this.OnTextColor ?? Util.FromArgb(128, OnTextColor);
                    var BackColor = ParentContainer.GetBackColor();
                    #endregion

                    thm.DrawOnOff(Canvas,
                                  rtContent, rtArea, rtCursor, rtOn, rtOff, cw, ow,
                                  CursorColor, OnBoxColor, OffBoxColor, OnTextColor, OffTextColor, BackColor,
                                  OnText, OffText, FontName, FontSize, FontStyle,
                                  OnOff, ptDown.HasValue);
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion

        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtArea, rtCursor, rtOn, rtOff, cw, ow) =>
            {
                if (CollisionTool.Check(rtCursor, x, y))
                {
                    downOnOff = OnOff;
                    ptDown = ptMove = new SKPoint(x, y);
                }
                else if (CollisionTool.Check(rtOn, x, y)) OnOff = false;
                else if (CollisionTool.Check(rtOff, x, y)) OnOff = true;

                Design?.Input(this);
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtArea, rtCursor, rtOn, rtOff, cw, ow) =>
            {
                if (ptDown.HasValue)
                {
                    ptDown = ptMove = null;
                    OnOff = x > rtArea.MidX;
                }
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(float x, float y)
        {
            Areas((rtContent, rtArea, rtCursor, rtOn, rtOff, cw, ow) =>
            {
                if (ptDown.HasValue)
                {
                    ptMove = new SKPoint(x, y);
                    OnOff = x > rtArea.MidX;
                }
            });
            base.OnMouseMove(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect, SKRect, float, float> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width, this.Height);
            var rtArea = Util.FromRect(rtContent); rtArea.Inflate(-CursorPadding, -CursorPadding);

            var cw = rtArea.Width * 0.5F;
            var ow = rtArea.Width * 0.5F;

            var sx = 0F;
            if (ptDown.HasValue && ptMove.HasValue)
            {
                var sv = downOnOff ? -ow : 0;
                var ev = downOnOff ? 0 : ow;
                sx = (downOnOff ? rtArea.Left : rtArea.Left - ow) + MathTool.Constrain(ptMove.Value.X - ptDown.Value.X, sv, ev);
            }
            else
            {
                if (Animation && ani.IsPlaying)
                {
                    var sv = ani.Variable == "ON" ? -ow : 0;
                    var ev = ani.Variable == "ON" ? 0 : -ow;

                    sx = rtArea.Left + ani.Value(AnimationAccel.Linear, sv, ev);
                }
                else sx = OnOff ? rtArea.Left : rtArea.Left - ow;
            }

            var rtOn = Util.FromRect(sx, rtArea.Top, ow, rtArea.Height);
            var rtCursor = Util.FromRect(rtOn.Right, rtArea.Top, cw, rtArea.Height);
            var rtOff = Util.FromRect(rtCursor.Right, rtArea.Top, ow, rtArea.Height);

            act(rtContent, rtArea, rtCursor, rtOn, rtOff, cw, ow);
        }
        #endregion
        #endregion

        
    }
}

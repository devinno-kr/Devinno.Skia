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
    public class DvStepGauge : DvControl
    {
        #region Properties
        public SKColor? ButtonColor { get; set; } = null;
        public SKColor? OnColor { get; set; } = null;
        public SKColor? OffColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;

        public DvStepButtonStyle ButtonStyle { get; set; } = DvStepButtonStyle.LeftRight;

        public bool UseButton { get; set; } = true;
        public int StepCount { get; set; } = 7;
        #region Step
        private int nStep = 0;
        public int Step
        {
            get => nStep;
            set
            {
                var v = Convert.ToInt32(MathTool.Constrain(value, 0, StepCount - 1));
                if(nStep != v)
                {
                    nStep = v;
                    StepChagend?.Invoke(this, null);
                }
            }
        }
        #endregion
        #endregion

        #region Member Variable
        bool bLeft = false;
        bool bRight = false;
        #endregion

        #region Event 
        public event EventHandler StepChagend;
        #endregion

        #region Constructor
        public DvStepGauge()
        {
            if (DvDesign.ActiveDesign != null) OnColor = DvDesign.ActiveDesign.Theme.PointColor;
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtLeft, rtRight, lsGauges, GP) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region Color
                    var ButtonColor = this.ButtonColor ?? thm.ButtonColor;
                    var OnColor = this.OnColor ?? thm.StepOnColor;
                    var OffColor = this.OffColor ?? thm.StepOffColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var Corner = thm.Corner;
                    var ButtonBorderColor = thm.GetBorderColor(ButtonColor, ParentContainer.GetBackColor());
                    #endregion

                    #region LeftButton
                    if (rtLeft.HasValue)
                    {
                        var rt = rtLeft.Value;
                        var cF = bLeft ? ButtonColor.BrightnessTransmit(thm.DownBrightness) : ButtonColor;
                        var cB = bLeft ? ButtonBorderColor.BrightnessTransmit(thm.DownBrightness) : ButtonBorderColor;
                        var cT = bLeft ? ForeColor.BrightnessTransmit(thm.DownBrightness) : ForeColor;

                        if (!bLeft) thm.DrawBox(Canvas, rt, cF, cB, RoundType.Round, BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow | BoxStyle.Border);
                        else thm.DrawBox(Canvas, rt, cF, cB, RoundType.Round, BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel | BoxStyle.Border);

                        var s = ButtonStyle == DvStepButtonStyle.LeftRight ? "fa-chevron-left" : "fa-minus";

                        if (bLeft) rt.Offset(0, 1);
                        Util.DrawIconFA(Canvas, s, 12, cT, rt, DvContentAlignment.MiddleCenter, true);
                    }
                    #endregion
                    #region RightButton
                    if (rtRight.HasValue)
                    {
                        var rt = rtRight.Value;
                        var cF = bRight ? ButtonColor.BrightnessTransmit(thm.DownBrightness) : ButtonColor;
                        var cB = bRight ? ButtonBorderColor.BrightnessTransmit(thm.DownBrightness) : ButtonBorderColor;
                        var cT = bRight ? ForeColor.BrightnessTransmit(thm.DownBrightness) : ForeColor;

                        if (!bRight) thm.DrawBox(Canvas, rt, cF, cB, RoundType.Round, BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow | BoxStyle.Border);
                        else thm.DrawBox(Canvas, rt, cF, cB, RoundType.Round, BoxStyle.Fill | BoxStyle.InShadow | BoxStyle.OutBevel | BoxStyle.Border);

                        var s = ButtonStyle == DvStepButtonStyle.LeftRight ? "fa-chevron-right" : "fa-plus";

                        if (bRight) rt.Offset(0, 1);
                        Util.DrawIconFA(Canvas, s, 12, cT, rt, DvContentAlignment.MiddleCenter, true);
                    }
                    #endregion
                    #region Gauge
                    for (int i = 0; i < lsGauges.Count; i++)
                    {
                        var rt = lsGauges[i];
                        var vc = (i != Step ? OffColor : OnColor);
                        var GaugeBorderColor = thm.GetBorderColor(vc, vc);
                        thm.DrawBox(Canvas, rt, vc, GaugeBorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel | BoxStyle.OutShadow);
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
            bounds((rtContent, rtLeft, rtRight, lsGauges, GP) =>
            {
                if (rtLeft.HasValue && CollisionTool.Check(rtLeft.Value, x, y)) bLeft = true;
                if (rtRight.HasValue && CollisionTool.Check(rtRight.Value, x, y)) bRight = true;
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            bounds((rtContent, rtLeft, rtRight, lsGauges, GP) =>
            {
                if (bLeft)
                {
                    if (rtLeft.HasValue && CollisionTool.Check(rtLeft.Value, x, y))
                        Step = Convert.ToInt32(MathTool.Constrain(Step - 1, 0, StepCount - 1));

                    bLeft = false;
                }
                if (bRight)
                {
                    if (rtRight.HasValue && CollisionTool.Check(rtRight.Value, x, y))
                        Step = Convert.ToInt32(MathTool.Constrain(Step + 1, 0, StepCount - 1));

                    bRight = false;
                }
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect?, SKRect?, List<SKRect>, float> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
            var rtLeft = (SKRect?)null;
            var rtRight = (SKRect?)null;
            var rtGauge = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, rtContent.Height);
            var lsGauges = new List<SKRect>();
            var GP = DvTheme.GP2;

            if (UseButton)
            {
                rtGauge.Inflate(-(rtContent.Height + GP), 0);
                rtLeft = Util.INT(Util.MakeRectangleAlign(rtContent, new SKSize(rtContent.Height, rtContent.Height), DvContentAlignment.MiddleLeft));
                rtRight = Util.INT(Util.MakeRectangleAlign(rtContent, new SKSize(rtContent.Height, rtContent.Height), DvContentAlignment.MiddleRight));
            }

            var w = ((float)rtGauge.Width - ((float)GP * ((float)StepCount - 1F))) / (float)StepCount;
            for (int i = 0; i < StepCount; i++)
            {
                var x = (w + GP) * i;
                var rt = Util.FromRect(rtGauge.Left + Convert.ToInt32((w + GP) * i), rtGauge.Top, Convert.ToInt32(w), rtGauge.Height);
                lsGauges.Add(Util.INT(rt));
            }

            act(rtContent, rtLeft, rtRight, lsGauges, GP);
        }
        #endregion
        #endregion
    }


    #region enum : DvStepButtonStyle
    public enum DvStepButtonStyle { PlusMinus, LeftRight }
    #endregion
}

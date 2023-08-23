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
    public class DvStepGauge : Design.DvControl
    {
        #region Properties
        #region Color
        public SKColor? OnColor { get; set; } = null;
        public SKColor? OffColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? ButtonColor { get; set; } = null;
        #endregion

        #region Text / Icon
        public DvIcon LeftIcon { get; } = new DvIcon { IconString = "fa-chevron-left" };
        public DvIcon RightIcon { get; } = new DvIcon { IconString = "fa-chevron-right" };

        public string LeftIconString { get => LeftIcon.IconString; set => LeftIcon.IconString = value; }
        public float LeftIconSize { get => LeftIcon.IconSize; set => LeftIcon.IconSize = value; }

        public string RightIconString { get => RightIcon.IconString; set => RightIcon.IconString = value; }
        public float RightIconSize { get => RightIcon.IconSize; set => RightIcon.IconSize = value; }
        #endregion

        #region Value
        public int StepCount { get; set; } = 7;
        #region  public int Step { get; set; }
        private int nStep = 0;
        public int Step
        {
            get => nStep;
            set
            {
                var v = Convert.ToInt32(MathTool.Constrain(value, 0, StepCount - 1));
                if (nStep != v)
                {
                    nStep = v;
                    StepChagend?.Invoke(this, null);
                }
            }
        }
        #endregion
        public int Gap { get; set; } = 10;
        #endregion

        public bool UseButton { get; set; } = true;
        public bool DrawButton { get; set; } = false;
        public float StepPadding { get; set; } = 3;
        #endregion

        #region Member Variable
        bool bLeftDown = false, bRightDown = false;
        #endregion

        #region Event 
        public event EventHandler StepChagend;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtBtnL, rtBtns, rtBtnR) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var OnColor = this.OnColor ?? thm.PointColor;
                    var OffColor = this.OffColor ?? thm.ControlBackColor;
                    var ButtonColor = this.ButtonColor ?? thm.ButtonColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();

                    thm.DrawStepGauge(Canvas,
                        rtContent, rtBtnL, rtBtns, rtBtnR,
                        OnColor, OffColor, ButtonColor, ForeColor, BackColor,
                        StepCount, Step, UseButton, DrawButton, StepPadding,
                        LeftIcon, RightIcon, bLeftDown, bRightDown);
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtBtnL, rtBtns, rtBtnR) => {

                if (UseButton)
                {
                    if (rtBtnL.HasValue && CollisionTool.Check(rtBtnL.Value, x, y)) bLeftDown = true;
                    if (rtBtnR.HasValue && CollisionTool.Check(rtBtnR.Value, x, y)) bRightDown = true;

                    if(bLeftDown || bRightDown) Design?.Input(this);
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtBtnL, rtBtns, rtBtnR) =>
            {
                if (bLeftDown)
                {
                    if (UseButton && rtBtnL.HasValue && CollisionTool.Check(rtBtnL.Value, x, y))
                        Step = Convert.ToInt32(MathTool.Constrain(Step - 1, 0, StepCount - 1));

                    bLeftDown = false;
                }

                if (bRightDown)
                {
                    if (UseButton && rtBtnR.HasValue && CollisionTool.Check(rtBtnR.Value, x, y))
                        Step = Convert.ToInt32(MathTool.Constrain(Step + 1, 0, StepCount - 1));

                    bRightDown = false;
                }
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion


        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect?, SKRect[], SKRect?> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);
            var w = 100.0F / StepCount;

            #region ls
            var ls = new List<SizeInfo>();
            if (UseButton)
            {
                ls.Add(new SizeInfo(DvSizeMode.Pixel, this.Height));
                ls.Add(new SizeInfo(DvSizeMode.Pixel, this.Gap));
            }

            if (StepCount > 0)
            {
                for (int i = 0; i < StepCount - 1; i++)
                {
                    ls.Add(new SizeInfo(DvSizeMode.Percent, w));
                    ls.Add(new SizeInfo(DvSizeMode.Pixel, this.Gap));
                }
                ls.Add(new SizeInfo(DvSizeMode.Percent, w));
            }
            else ls.Add(new SizeInfo(DvSizeMode.Percent, 100F));

            if (UseButton)
            {
                ls.Add(new SizeInfo(DvSizeMode.Pixel, this.Gap));
                ls.Add(new SizeInfo(DvSizeMode.Pixel, this.Height));
            }
            #endregion
            #region rect
            var rts = Util.DevideSizeH(rtContent, ls);
            var lsbtns = new List<SKRect>();
            for (int i = 0; i < StepCount; i++)
            {
                var idx = (UseButton ? 2 : 0) + (i * 2);
                if (idx < rts.Length) lsbtns.Add(rts[idx]);
            }
            #endregion

            if (UseButton)
            {
                var rtL = rts.FirstOrDefault();
                var rtR = rts.LastOrDefault();
                var rtbtns = lsbtns.ToArray();

                act(rtContent, rtL, rtbtns, rtR);
            }
            else
            {
                var rtbtns = lsbtns.ToArray();

                act(rtContent, null, rtbtns, null);
            }
        }
        #endregion
        #endregion
    }
}

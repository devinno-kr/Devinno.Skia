using Devinno.Collections;
using Devinno.Skia.Collections;
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
    public class DvSelector : DvControl
    {
        #region Properties
        #region Color
        public SKColor? SelectorColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        #endregion

        #region Items
        public List<SelectorItem> Items { get; private set; } = new List<SelectorItem>();
        #endregion

        #region Shape
        public bool BackgroundDraw { get; set; } = true;
        public int? ButtonSize { get; set; } = null;
        public DvDirectionHV Direction { get; set; } = DvDirectionHV.Horizon;
        public DvRoundType? Round { get; set; } = null;
        #endregion

        #region Animation
        public bool Animation => Design != null ? Design.Animation : false;
        #endregion

        #region Text / Icon
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;

        public float IconSize { get; set; } = 12;
        public float IconGap { get; set; } = 5;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        #endregion

        #region SelectedIndex
        private int nSelectedIndex = -1;
        public int SelectedIndex
        {
            get => nSelectedIndex;
            set
            {
                if (nSelectedIndex != value)
                {
                    nSelectedIndex = value;
                    SelectedIndexChanged?.Invoke(this, null);
                }
            }
        }
        #endregion
        #endregion

        #region Constructor
        public DvSelector()
        {

        }
        #endregion

        #region Member Variable
        private bool bPrev = false, bNext = false;

        private Animation ani = new Animation();
        #endregion

        #region Event
        public event EventHandler SelectedIndexChanged;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtPrev, rtValue, rtNext, rtItems) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var SelectorColor = this.SelectorColor ?? thm.ButtonColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();

                    thm.DrawSelector(Canvas,
                            rtContent, rtPrev, rtValue, rtNext, rtItems,
                            SelectorColor, ForeColor, BackColor,
                            FontName, FontSize, FontStyle, 
                            IconSize, IconGap, IconAlignment,
                            Items, BackgroundDraw, Direction, Round, 
                            SelectedIndex,
                            bPrev, bNext, Animation, ani);
                }

            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtPrev, rtValue, rtNext, rtItems) =>
            {
                if (CollisionTool.Check(rtPrev, x, y)) bPrev = true;
                if (CollisionTool.Check(rtNext, x, y)) bNext = true;

                if (bPrev || bNext) Design?.Input(this);
            });

            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtPrev, rtValue, rtNext, rtItems) =>
            {
                #region PrevButton
                if (bPrev)
                {
                    if (CollisionTool.Check(rtPrev, x, y))
                    {
                        if (Items.Count > 0)
                        {
                            if (SelectedIndex > 0)
                            {
                                SelectedIndex--;

                                if (Animation)
                                {
                                    ani.Stop();
                                    ani.Start(DvDesign.ANI2, "Prev");
                                }
                            }
                        }
                        else SelectedIndex = -1;

                    }
                    bPrev = false;
                }
                #endregion
                #region NextButton
                if (bNext)
                {
                    if (CollisionTool.Check(rtNext, x, y))
                    {
                        if (Items.Count > 0)
                        {
                            if (SelectedIndex < Items.Count-1)
                            {
                                SelectedIndex++;

                                if (Animation)
                                {
                                    ani.Stop();
                                    ani.Start(DvDesign.ANI2, "Next");
                                }
                            }
                        }

                        
                    }
                    bNext = false;
                }
                #endregion
            });

            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect, List<SKRect>> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);
            
            if(Direction == DvDirectionHV.Horizon)
            {
                var btnSize = ButtonSize ?? Height;
                var ls = new List<SizeInfo>();
                ls.Add(new SizeInfo(DvSizeMode.Pixel, btnSize));
                ls.Add(new SizeInfo(DvSizeMode.Percent, 100F));
                ls.Add(new SizeInfo(DvSizeMode.Pixel, btnSize));
                
                var rts = Util.DevideSizeH(rtContent, ls);
                var rtPrev = rts[0];
                var rtValue = rts[1];
                var rtNext = rts[2];

                var lsv = new List<SKRect>();
                var sx = rtValue.Left;

                if (Animation && ani.IsPlaying)
                {
                    var v = Convert.ToSingle(ani.Value(AnimationAccel.DCL, rtValue.Width, 0));
                    sx = rtValue.Left + -(SelectedIndex * rtValue.Width) + (ani.Variable == "Prev" ? -v : v);
                }
                else
                {
                    sx = rtValue.Left + -(SelectedIndex * rtValue.Width);
                }


                foreach(var v in Items)
                {
                    lsv.Add(Util.FromRect(sx, rtValue.Top, rtValue.Width, rtValue.Height));
                    sx += rtValue.Width;
                }

                act(rtContent, rtPrev, rtValue, rtNext, lsv);
            }
            else if(Direction == DvDirectionHV.Vertical)
            {
                var btnSize = ButtonSize ?? Height / 3F;
                var ls = new List<SizeInfo>();
                ls.Add(new SizeInfo(DvSizeMode.Pixel, btnSize));
                ls.Add(new SizeInfo(DvSizeMode.Percent, 100F));
                ls.Add(new SizeInfo(DvSizeMode.Pixel, btnSize));

                var rts = Util.DevideSizeV(rtContent, ls);
                var rtPrev = rts[0];
                var rtValue = rts[1];
                var rtNext = rts[2];

                var lsv = new List<SKRect>();
                var sy = rtValue.Top;

                if (Animation && ani.IsPlaying)
                {
                    var v = Convert.ToSingle(ani.Value(AnimationAccel.DCL, rtValue.Height, 0));
                    sy = rtValue.Top + -(SelectedIndex * rtValue.Height) + (ani.Variable == "Prev" ? -v : v);
                }
                else
                {
                    sy = rtValue.Top + -(SelectedIndex * rtValue.Height);
                }

                foreach (var v in Items)
                {
                    lsv.Add(Util.FromRect(rtValue.Left, sy, rtValue.Width, rtValue.Height));
                    sy += rtValue.Height;
                }

                act(rtContent, rtPrev, rtValue, rtNext, lsv);
            }
        }
        #endregion
    }

    #region class : SelectorItem
    public class SelectorItem
    {
        public string Text { get; set; }
        public string IconString { get; set; }
        public object Tag { get; set; }
        public object Value { get; set; }
    }
    #endregion
}

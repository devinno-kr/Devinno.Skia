using Devinno.Skia.Design;
using Devinno.Skia.Theme;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvNumberbox : DvValueLabel
    {
        #region Const 
        const int CHKTM = 500;
        #endregion

        #region Properties
        #region Value
        #region public double Value { get; set; } = 0D;
        private double nValue = 0D;
        public double Value
        {
            get => nValue;
            set
            {
                var v = MathTool.Constrain(value, Minimum, Maximum);
                if (nValue != v)
                {
                    nValue = v;
                    ValueChanged?.Invoke(this, null);
                }
            }
        }
        #endregion
        public double Minimum { get; set; } = 0D;
        public double Maximum { get; set; } = 100D;
        public double Tick { get; set; } = 1D;
        public string FormatString { get; set; } = null;
        #endregion
        #region Font
        public string Text => ValueTool.Text<double>(Value, FormatString);
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        #endregion
        #region ButtonIconSize
        private int nButtonIconSize = 14;
        public int ButtonIconSize
        {
            get => nButtonIconSize;
            set
            {
                if (nButtonIconSize != value)
                {
                    nButtonIconSize = value;
                    foreach (var btn in Buttons) btn.IconSize = value;
                }
            }
        }
        #endregion
        #region Buttons
        private new List<ButtonInfo> Buttons => base.Buttons;
        #endregion
        #region Direction
        public override DvDirectionHV Direction
        {
            get => base.Direction;
            set
            {
                base.Direction = value;
                if (base.Direction == DvDirectionHV.Horizon)
                {
                    ButtonAreaSize = Height * 2;
                }
                else if(base.Direction == DvDirectionHV.Vertical)
                {
                    ButtonAreaSize = 40;
                }
            }
        }
        #endregion
        #endregion

        #region Member Variable
        DateTime dtPlus = DateTime.Now, dtMinus = DateTime.Now;
        int maxinterval = 250, mininterval = 10;
        #endregion

        #region Constructor
        public DvNumberbox()
        {
            Buttons.Add(new ButtonInfo("plus") { IconString = "fa-plus", IconSize = ButtonIconSize, Size = new SizeInfo(DvSizeMode.Percent, 50) });
            Buttons.Add(new ButtonInfo("minus") { IconString = "fa-minus", IconSize = 14, Size = new SizeInfo(DvSizeMode.Percent, 50) });
            
            if (Direction == DvDirectionHV.Horizon) ButtonAreaSize = Height * 2;
            else if (Direction == DvDirectionHV.Vertical) ButtonAreaSize = Height / 3;

            #region ButtonDown
            ButtonDown += (o, s) =>
            {
                if (s.Button.Name == "plus")
                {
                    dtPlus = DateTime.Now;
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        while (Buttons[0].ButtonDownState)
                        {
                            var tmp = (DateTime.Now - dtPlus).TotalMilliseconds;

                            if (Buttons[0].ButtonDownState && tmp >= CHKTM)
                            {
                                var delay = (int)MathTool.Constrain(MathTool.Map(tmp, 2000, CHKTM, mininterval, maxinterval), mininterval, maxinterval);
                                Value = MathTool.Constrain(Value + Tick, Minimum, Maximum);
                                Thread.Sleep(delay);
                            }
                            else Thread.Sleep(100);
                        }
                    });
                }
                else if(s.Button.Name == "minus")
                {
                    dtMinus = DateTime.Now;
                    ThreadPool.QueueUserWorkItem((o) =>
                    {
                        while (Buttons[1].ButtonDownState)
                        {
                            var tmm = (DateTime.Now - dtMinus).TotalMilliseconds;

                            if (Buttons[1].ButtonDownState && tmm >= CHKTM)
                            {
                                var delay = (int)MathTool.Constrain(MathTool.Map(tmm, 2000, CHKTM, mininterval, maxinterval), mininterval, maxinterval);
                                Value = MathTool.Constrain(Value - Tick, Minimum, Maximum);
                                Thread.Sleep(delay);
                            }
                            else Thread.Sleep(100);
                        }
                    });
                }
            };
            #endregion
            #region ButtonUp
            ButtonUp += (o, s) =>
            {
                if (s.Button.Name == "minus")
                {
                    if ((DateTime.Now - dtMinus).TotalMilliseconds < CHKTM) 
                        Value = MathTool.Constrain(Value - Tick, Minimum, Maximum);
                }
                if (s.Button.Name == "plus")
                {
                    if ((DateTime.Now - dtPlus).TotalMilliseconds < CHKTM) 
                        Value = MathTool.Constrain(Value + Tick, Minimum, Maximum);
                }
            };
            #endregion
        }
        #endregion

        #region Event
        public event EventHandler ValueChanged;
        #endregion

        #region DrawValue
        protected override void DrawValue(SKCanvas canvas, DvTheme thm, SKRect rtValue)
        {
            if (Direction == DvDirectionHV.Horizon) ButtonAreaSize = Height * 2;
            else if (Direction == DvDirectionHV.Vertical) ButtonAreaSize = Height / 3;

            Areas((rtContent, rtTitle, rtValue, rtButtons) =>
            {
                if (!string.IsNullOrWhiteSpace(Text))
                {
                    var cF = ForeColor ?? thm.ForeColor;
                    Util.DrawText(canvas, Text, FontName, FontSize, FontStyle, cF, rtValue, DvContentAlignment.MiddleCenter);
                }
            });
        }
        #endregion
    }
}

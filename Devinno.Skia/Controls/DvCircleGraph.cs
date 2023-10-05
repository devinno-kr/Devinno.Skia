using Devinno.Skia.Design;
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
    public class DvCircleGraph : DvControl
    {
        #region Properties
        #region Color
        public SKColor? ForeColor { get; set; } = null;
        #endregion
        #region Series
        public List<GraphSeries> Series { get; } = new List<GraphSeries>();
        #endregion
        #region Gradient
        public bool Gradient { get; set; } = true;
        #endregion
        #region FormatString
        public string FormatString { get; set; } = null;
        #endregion
        #region Font
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public float NameFontSize { get; set; } = 18;
        public float ValueFontSize { get; set; } = 15;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        #endregion
        #endregion

        #region Member Variable
        private List<GraphValue> GraphDatas = new List<GraphValue>();
        private int nSelectedIndex = -1;
        private bool bLeftSel = false;
        private bool bRightSel = false;
        private SKPoint mpt;

        private DateTime prev = DateTime.Now;
        #endregion

        #region Constructor
        public DvCircleGraph()
        {

        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtGraph, rtSelectLabel, rtSelectLeft, rtSelectRight) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region Color
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    #endregion

                    thm.DrawCircleGraph(Canvas,
                        rtContent, rtGraph, rtSelectLabel, rtSelectLeft, rtSelectRight,
                        ForeColor, BackColor,
                        Series, Gradient,
                        FontName, FontSize, NameFontSize, ValueFontSize, FontStyle,
                        GraphDatas, FormatString,
                        bLeftSel, bRightSel, mpt, nSelectedIndex, prev);
                }
            });

            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtGraph, rtSelectLabel, rtSelectLeft, rtSelectRight) =>
            {
                if (Series.Count > 1)
                {
                    if (CollisionTool.Check(rtSelectLeft, x, y))
                    {
                        bLeftSel = true;
                        if (nSelectedIndex - 1 < 0) nSelectedIndex = Series.Count - 1;
                        else nSelectedIndex--;
                    }
                    if (CollisionTool.Check(rtSelectRight, x, y))
                    {
                        bRightSel = true;
                        if (nSelectedIndex + 1 >= Series.Count) nSelectedIndex = 0;
                        else nSelectedIndex++;
                    }
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            bLeftSel = bRightSel = false;
            base.OnMouseUp(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(float x, float y)
        {
            mpt.X = x;
            mpt.Y = y;
            base.OnMouseMove(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect, SKRect> act)
        {
            using (var p = new SKPaint())
            {
                #region Font
                p.Typeface = FontTool.GetFont(FontName, FontStyle);
                p.TextSize = FontSize;
                #endregion

                #region var
                var rtCHSZ = new SKRect();
                p.MeasureText("H", ref rtCHSZ);
             
                var CH = rtCHSZ.Height;
                var GP = DvTheme.GP;
                
                var szSeries = new List<SKSize>();
                foreach(var v in Series)
                {
                    var rtv = new SKRect();
                    p.MeasureText(v.Alias, ref rtv);
                    szSeries.Add(rtv.Size);
                }

                var SelectorAreaHeight = GP + CH + GP;
                var SelectorAreaWidth = GP + (szSeries.Count == 0 ? 0 : szSeries.Max(x => x.Width)) + GP;

                #endregion

                var rtContent = Util.FromRect(0, 0, Width, Height);
                var rtSelector = Util.FromRect(rtContent.Left, rtContent.Bottom - SelectorAreaHeight, rtContent.Width, SelectorAreaHeight);
                var rtGraphA = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, rtContent.Height - SelectorAreaHeight - GP);
                var rtSelectLabel = Util.MakeRectangleAlign(rtSelector, new SKSize(SelectorAreaWidth, rtSelector.Height), DvContentAlignment.MiddleCenter);
                var rtSelectLeft = Util.FromRect(rtSelectLabel.Left - rtSelectLabel.Height, rtSelectLabel.Top, rtSelectLabel.Height, rtSelectLabel.Height);
                var rtSelectRight = Util.FromRect(rtSelectLabel.Right, rtSelectLabel.Top, rtSelectLabel.Height, rtSelectLabel.Height);

                var wh = Math.Min(rtGraphA.Width, rtGraphA.Height) - (GP * 6);
                var rtGraph = Util.MakeRectangleAlign(rtGraphA, new SKSize(wh, wh), DvContentAlignment.MiddleCenter);

                act(rtContent, rtGraph, rtSelectLabel, rtSelectLeft, rtSelectRight);
            }
        }
        #endregion
        #region SetDataSource
        public void SetDataSource<T>(IEnumerable<T> values) where T : GraphData
        {
            if (Series.Count > 0)
            {
                var pls = typeof(T).GetProperties();
                var props = typeof(T).GetProperties().Where(x => x.PropertyType == typeof(double) || x.PropertyType == typeof(float) || x.PropertyType == typeof(decimal) ||
                                                                 x.PropertyType == typeof(byte) || x.PropertyType == typeof(sbyte) ||
                                                                 x.PropertyType == typeof(short) || x.PropertyType == typeof(ushort) ||
                                                                 x.PropertyType == typeof(int) || x.PropertyType == typeof(uint) ||
                                                                 x.PropertyType == typeof(long) || x.PropertyType == typeof(ulong));
                var nmls = props.Select(x => x.Name).ToList();
                int nCnt = Series.Where(x => nmls.Contains(x.Name)).Count();
                if (nCnt == Series.Count)
                {
                    var dic = props.ToDictionary(x => x.Name);

                    GraphDatas.Clear();
                    foreach (var v in values)
                    {
                        var gv = new GraphValue() { Name = v.Name, Color = v.Color };

                        foreach (var prop in props)
                        {
                            var val = Convert.ToDouble(prop.GetValue(v));
                            gv.Values.Add(prop.Name, val);
                        }

                        GraphDatas.Add(gv);
                    }
                }
                else throw new Exception("잘못된 데이터 입니다.");
            }
            else throw new Exception("GraphSeries는 최소 1개 이상이어야 합니다.");
        }
        #endregion
        #endregion
    }
}

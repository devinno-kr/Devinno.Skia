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
    public class DvLineGraph : DvControl
    {
        #region Properties
        #region Color
        public SKColor? GraphBackColor { get; set; } = null;
        public SKColor? GridColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        #endregion
        #region Series
        public List<GraphSeries> Series { get; } = new List<GraphSeries>();
        #endregion
        #region Value
        public double Graduation { get; set; } = 10D;
        public double Minimum { get; set; } = 0D;
        public double Maximum { get; set; } = 100D;

        public string FormatString { get; set; } = null;
        public bool ValueDraw { get; set; } = true;
        #endregion
        #region Point
        public bool PointDraw { get; set; } = true;
        public int PointWidth { get; set; } = 50;

        private int DataWH => PointWidth;
        #endregion
        #region Scroll
        public bool Scrollable { get; set; } = true;
        public bool TouchMode { get => scroll.TouchMode; set => scroll.TouchMode = value; }
        #endregion
        #region Font
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 10;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        #endregion
        #endregion

        #region Member Variable
        private List<GraphValue> GraphDatas = new List<GraphValue>();
        private Scroll scroll = new Scroll();
        #endregion

        #region Constructor
        public DvLineGraph()
        {
            scroll = new Scroll() { TouchMode = true, Direction = ScrollDirection.Horizon };
            scroll.GetScrollTotal = () => GraphDatas.Count > 0 && Series.Count > 0 ? GraphDatas.Count * DataWH : 0;
            scroll.GetScrollTick = () => ((Series.Count + 2) * PointWidth);
            scroll.GetScrollView = () =>
            {
                long v = 0;
                Areas((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraph, rtScroll, szRemarks) =>
                {
                    v = Convert.ToInt64(rtGraph.Width);
                });
                return v;
            };
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraph, rtScroll, szRemarks) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region var
                    var GridColor = this.GridColor ?? thm.GridColor;
                    var GraphBackColor = this.GraphBackColor ?? SKColors.Transparent;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    #endregion
                    
                    thm.DrawLineGraph(Canvas,
                      rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraph, rtScroll, szRemarks,
                      GridColor, GraphBackColor, ForeColor, BackColor,
                      FontName, FontSize, FontStyle,
                      ValueDraw, FormatString,
                      PointDraw, DataWH,
                      Series, Graduation, Minimum, Maximum,
                      scroll, Scrollable, TouchMode,
                      GraphDatas);

                    #region Scroll
                    if (Scrollable)
                    {
                        thm.DrawScroll(Canvas, rtScroll, BackColor, scroll);
                    }
                    #endregion
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraph, rtScroll, szRemarks) =>
            {
                if (Scrollable)
                {
                    scroll.MouseDown(x, y, rtScroll);
                    if (scroll.TouchMode && CollisionTool.Check(rtGraph, x, y))
                    {
                        scroll.TouchDown(x, y);
                        if (scroll.ScrollTotal > scroll.ScrollView) Design?.Input(this);
                    }
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(float x, float y)
        {
            Areas((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraph, rtScroll, szRemarks) =>
            {
                if (Scrollable)
                {
                    scroll.MouseMove(x, y, rtScroll);
                    if (scroll.TouchMode) scroll.TouchMove(x, y);
                }
            });
            base.OnMouseMove(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraph, rtScroll, szRemarks) =>
            {
                if (Scrollable)
                {
                    scroll.MouseUp(x, y);
                    if (scroll.TouchMode) scroll.TouchUp(x, y);
                }
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, List<SKSize>> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);
            var rtRemark = new SKRect();
            var rtNameAxis = new SKRect();
            var rtValueAxis = new SKRect();
            var rtGraph = new SKRect();
            var rtScroll = new SKRect();
            var szRemarks = new List<SKSize>();

            using (var p = new SKPaint())
            {
                #region Font
                p.Typeface = FontTool.GetFont(FontName, FontStyle);
                p.TextSize = FontSize;
                #endregion
                #region var
                var rtCHSZ = new SKRect();  p.MeasureText("H", ref rtCHSZ);
                var CH = rtCHSZ.Height;
                var GP = DvTheme.GP;
                var SCWH = Scroll.SC_WH;
                #endregion
                #region Min / Max / Remark
                var Minimum = this.Minimum;
                var Maximum = this.Maximum;

                var sName = GraphDatas.OrderBy(x => x.Name.Length).LastOrDefault()?.Name ?? "";
                var sRemark = Series.OrderBy(x => x.Alias.Length).LastOrDefault()?.Name ?? "";
                var sMin = string.IsNullOrWhiteSpace(FormatString) ? Minimum.ToString() : Minimum.ToString(FormatString);
                var sMax = string.IsNullOrWhiteSpace(FormatString) ? Maximum.ToString() : Maximum.ToString(FormatString);

                var rtvName = new SKRect(); p.MeasureText(sName, ref rtvName);
                var rtvRemark = new SKRect(); p.MeasureText(sRemark, ref rtvRemark);
                var rtvMin = new SKRect(); p.MeasureText(sMin, ref rtvMin);
                var rtvMax = new SKRect(); p.MeasureText(sMax, ref rtvMax);

                foreach (var s in Series)
                {
                    var rt = new SKRect();
                    p.MeasureText(s.Alias, ref rt);
                    szRemarks.Add(rt.Size);
                }
                #endregion

                {
                    #region var
                    var rmkW = GP + szRemarks.Sum(x => 10 + 7 + x.Width + GP);
                    var rmkH = GP + CH + GP;
                    #endregion
                    #region bounds
                    var lsr = new List<SizeInfo>();
                    var lsc = new List<SizeInfo>();

                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, CH / 2F));
                    lsr.Add(new SizeInfo(DvSizeMode.Percent, 100));
                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, GP));
                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, rtvName.Height));
                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, GP));
                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, SCWH));
                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, GP));
                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, rmkH));

                    lsc.Add(new SizeInfo(DvSizeMode.Pixel, rtvName.Width));
                    lsc.Add(new SizeInfo(DvSizeMode.Pixel, GP));
                    lsc.Add(new SizeInfo(DvSizeMode.Percent, 100));

                    var rts = Util.DevideSizeVH(rtContent, lsr, lsc);
                    #endregion
                    #region set
                    rtValueAxis = rts[1, 0];
                    rtNameAxis = rts[3, 2];
                    rtGraph = rts[1, 2];
                    rtScroll = rts[5, 2];
                    rtRemark = Util.MakeRectangleAlign(rts[7, 2], new SKSize(rmkW, rts[7, 2].Height), DvContentAlignment.MiddleCenter);
                    #endregion
                }
            }

            act(rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraph, rtScroll, szRemarks);
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

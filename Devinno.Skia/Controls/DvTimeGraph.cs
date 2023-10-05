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
    public class DvTimeGraph : DvControl
    {
        #region Properties
        #region Color
        public SKColor? GraphBackColor { get; set; } = null;
        public SKColor? GridColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        #endregion
        #region Series
        public List<GraphSeries2> Series { get; } = new List<GraphSeries2>();
        #endregion
        #region Axis
        public TimeSpan XAxisGraduation { get; set; } = new TimeSpan(0, 10, 0);
        public int YAxisGraduationCount { get; set; } = 10;

        public bool XAxisGridDraw { get; set; } = false;
        public bool YAxisGridDraw { get; set; } = true;
  
        public string ValueFormatString { get; set; }
        public string TimeFormatString { get; set; }
        #endregion
        #region XScale
        public TimeSpan XScale { get; set; } = new TimeSpan(1, 0, 0);
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
        private List<TimeGraphValue> GraphDatas = new List<TimeGraphValue>();
        private Scroll scroll = new Scroll();
        #endregion

        #region Constructor
        public DvTimeGraph()
        {
            scroll = new Scroll() { TouchMode = true, Direction = ScrollDirection.Horizon };
            scroll.GetScrollTotal = () => GraphDatas.Count > 1 && Series.Count > 0 ? GraphDatas[GraphDatas.Count - 1].Time.Ticks - GraphDatas[0].Time.Ticks : 0L;
            scroll.GetScrollTick = () => XAxisGraduation.Ticks;
            scroll.GetScrollView = () => XScale.Ticks;
            scroll.GetScrollScaleFactor = () =>
            {
                long v = 0;
                Areas((rtContent, rtRemark, rtTimeAxis, rtValueTitle, rtValueAxis, rtGraph, rtScroll, szRemarks) =>
                {
                    v = Convert.ToInt64(XScale.Ticks / (double)rtGraph.Width);
                });
                return v;
            };
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtRemark, rtTimeAxis, rtValueTitle, rtValueAxis, rtGraph, rtScroll, szRemarks) =>
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

                    thm.DrawTimeGraph(Canvas,
                         rtContent, rtRemark, rtTimeAxis, rtValueTitle, rtValueAxis, rtGraph, rtScroll, szRemarks,
                         GridColor, GraphBackColor, ForeColor, BackColor,
                         FontName, FontSize, FontStyle,
                         XAxisGraduation, YAxisGraduationCount, XAxisGridDraw, YAxisGridDraw,
                         ValueFormatString, TimeFormatString,
                         XScale, Series,  
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
            Areas((rtContent, rtRemark, rtTimeAxis, rtValueTitle, rtValueAxis, rtGraph, rtScroll, szRemarks) =>
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
            Areas((rtContent, rtRemark, rtTimeAxis, rtValueTitle, rtValueAxis, rtGraph, rtScroll, szRemarks) =>
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
            Areas((rtContent, rtRemark, rtTimeAxis, rtValueTitle, rtValueAxis, rtGraph, rtScroll, szRemarks) =>
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
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, List<SKSize>> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);
            var rtRemark = new SKRect();
            var rtTimeAxis = new SKRect();
            var rtValueTitle = new SKRect();
            var rtValueAxis = new SKRect();
            var rtGraph = new SKRect();
            var rtScroll = new SKRect();

            using (var p = new SKPaint())
            {
                #region Font
                p.Typeface = FontTool.GetFont(FontName, FontStyle);
                p.TextSize = FontSize;
                #endregion
                #region var
                var rtCHSZ = new SKRect(); p.MeasureText("H", ref rtCHSZ);
                var CH = rtCHSZ.Height;
                var GP = DvTheme.GP;
                var SCWH = Scroll.SC_WH;
                #endregion
                #region Min / Max / Remark
                var dic = new Dictionary<string, _ValueAxisBounds_>();
                foreach (var x in Series)
                {
                    var vrt = new _ValueAxisBounds_();
                    
                    var sMin = string.IsNullOrWhiteSpace(ValueFormatString) ? x.Minimum.ToString() : x.Minimum.ToString(ValueFormatString);
                    var sMax = string.IsNullOrWhiteSpace(ValueFormatString) ? x.Maximum.ToString() : x.Maximum.ToString(ValueFormatString);
                    var sTxt = x.Alias;

                    p.MeasureText(sMin, ref vrt.rtMin);
                    p.MeasureText(sMax, ref vrt.rtMax);
                    p.MeasureText(sTxt, ref vrt.rtAlias);

                    dic.Add(x.Name, vrt);
                }
                #endregion

                {
                    #region var
                    var rmkW = GP + dic.Sum(x => 10 + 7 + x.Value.rtAlias.Width + GP);
                    var rmkH = GP + CH + GP;

                    var frmt = TimeFormatString ?? "yyyy.MM.dd\r\nHH:mm:ss";
                    var sval = DateTime.Now.ToString(frmt);
                    var szTime = Util.MeasureText(sval, FontName, FontSize, FontStyle);
                   
                    var nmW = dic.Select(x => Math.Max(Math.Max(x.Value.rtMin.Width, x.Value.rtMax.Width), x.Value.rtAlias.Width) + GP).Sum();
                    var tmH = szTime.Height;
                    #endregion
                    #region bounds
                    var lsr = new List<SizeInfo>();
                    var lsc = new List<SizeInfo>();

                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, CH + GP));
                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, GP/2));
                    lsr.Add(new SizeInfo(DvSizeMode.Percent, 100));
                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, GP));
                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, tmH));
                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, GP));
                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, SCWH));
                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, GP));
                    lsr.Add(new SizeInfo(DvSizeMode.Pixel, rmkH));

                    lsc.Add(new SizeInfo(DvSizeMode.Pixel, nmW));
                    lsc.Add(new SizeInfo(DvSizeMode.Pixel, GP));
                    lsc.Add(new SizeInfo(DvSizeMode.Percent, 100));
                    lsc.Add(new SizeInfo(DvSizeMode.Pixel, szTime.Width/2F));

                    var rts = Util.DevideSizeVH(rtContent, lsr, lsc);
                    #endregion
                    #region set
                    rtValueTitle = rts[0, 0];
                    rtValueAxis = rts[2, 0];
                    rtTimeAxis = rts[4, 2];
                    rtGraph = rts[2, 2];
                    rtScroll = rts[6, 2];
                    rtRemark = Util.MakeRectangleAlign(rts[8, 2], new SKSize(rmkW, rts[8, 2].Height), DvContentAlignment.MiddleCenter);
                    var szRemarks = dic.Values.Select(x => x.rtAlias.Size).ToList();
                    #endregion

                    act(rtContent, rtRemark, rtTimeAxis, rtValueTitle, rtValueAxis, rtGraph, rtScroll, szRemarks);
                }
            }
        }
        #endregion
        #region SetDataSource
        public void SetDataSource<T>(IEnumerable<T> values) where T : TimeGraphData
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
                        var tgv = new TimeGraphValue() { Time = v.Time };

                        foreach (var vk in dic.Keys) tgv.Values.Add(vk, (double)dic[vk].GetValue(v));

                        GraphDatas.Add(tgv);
                    }
                }
                else throw new Exception("잘못된 데이터 입니다.");
            }
            else throw new Exception("GraphSeries는 최소 1개 이상이어야 합니다.");
        }
        #endregion
        #endregion
    }

    #region class : _ValueAxisBounds_
    class _ValueAxisBounds_
    {
        public SKRect rtMax = new SKRect();
        public SKRect rtMin = new SKRect();
        public SKRect rtAlias = new SKRect();
    }
    #endregion
}

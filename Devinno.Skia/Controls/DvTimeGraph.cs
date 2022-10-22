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
    public class DvTimeGraph : DvControl
    {
        #region Properties
        public SKColor? GraphBackColor { get; set; } = null;
        public SKColor? GridColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? RemarkColor { get; set; } = null;
        public List<GraphSeries2> Series { get; } = new List<GraphSeries2>();

        public int YAxisGraduationCount { get; set; } = 10;
        public TimeSpan XAxisGraduation { get; set; } = new TimeSpan(0, 10, 0);

        public string ValueFormatString { get; set; }
        public string TimeFormatString { get; set; }

        public TimeSpan XScale { get; set; } = new TimeSpan(1, 0, 0);

        public bool XAxisGridDraw { get; set; } = false;
        public bool YAxisGridDraw { get; set; } = false;

        public bool TouchMode { get => scroll.TouchMode; set => scroll.TouchMode = value; }

        #region Text
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #endregion

        #region Member Variable
        private List<TGV> GraphDatas = new List<TGV>();
        private Scroll scroll = new Scroll();
        #endregion

        #region Constructor
        public DvTimeGraph()
        {
            scroll = new Scroll() { TouchMode = true, Direction = ScrollDirection.Horizon };
            scroll.GetScrollTotal = () => GraphDatas.Count > 1 && Series.Count > 0 ? (new DateTime(Convert.ToInt64(Math.Ceiling(Convert.ToDouble(GraphDatas.Last().Time.Ticks) / Convert.ToDouble(XAxisGraduation.Ticks))) * XAxisGraduation.Ticks) - new DateTime(GraphDatas.First().Time.Ticks / XAxisGraduation.Ticks * XAxisGraduation.Ticks)).Ticks : 0L;
            scroll.GetScrollTick = () => XAxisGraduation.Ticks;
            scroll.GetScrollView = () => XScale.Ticks;
            scroll.GetScrollScaleFactor = () =>
            {
                long v = 0;
                bounds((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraphAl, rtGraph, rtScroll, dicSer, GP, CH) =>
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
            bounds((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraphAl, rtGraph, rtScroll, dicSer, GP, CH) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var GridColor = this.GridColor ?? thm.GridColor;
                    var RemarkColor = this.RemarkColor ?? thm.ButtonColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var bg = GraphBackColor ?? SKColors.Transparent;
                    var RemarkBorderColor = thm.GetBorderColor(RemarkColor, ParentContainer.GetBackColor());
                    var ScrollBorderColor = thm.GetBorderColor(thm.ScrollBarColor, ParentContainer.GetBackColor());
                    var ForeColor = this.ForeColor ?? thm.ForeColor;

                    var spos = scroll.ScrollPositionWithOffset;

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        p.Typeface = FontTool.GetFont(FontName, DvFontStyle.Normal);
                        p.TextSize = FontSize;

                        #region GraphBG
                        if (bg != SKColors.Transparent)
                        {
                            p.IsStroke = false;
                            p.Color = bg;
                            Canvas.DrawRect(rtGraph, p);
                        }
                        #endregion
                        #region Remark
                        if (Series.Count > 0)
                        {
                            #region var ls;
                            var ls = new List<SKSize>();
                            foreach (var v in Series)
                            {
                                var rtv = new SKRect();
                                p.MeasureText(v.Alias, ref rtv);
                                ls.Add(rtv.Size);
                            }
                            #endregion
                            var nwbr = 10;
                            var nwgp = 5;
                            var RemarkW = (GP * 2) + ls.Sum(x => nwbr + nwgp + Convert.ToInt32(x.Width) + (GP * 2) + 2);
                            var rtRemarkBox = Util.MakeRectangleAlign(rtRemark, new SKSize(RemarkW, rtRemark.Height), DvContentAlignment.MiddleCenter);
                            thm.DrawBox(Canvas, Util.INT(rtRemarkBox), RemarkColor, RemarkBorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.OutShadow);

                            var ix = rtRemarkBox.Left + (GP * 2);
                            for (int i = 0; i < Series.Count; i++)
                            {
                                var s = Series[i];
                                var sz = ls[i];
                                var rtBR = Util.MakeRectangleAlign(Util.FromRect(ix, rtRemarkBox.Top, nwbr, rtRemarkBox.Height), new SKSize(nwbr, nwgp), DvContentAlignment.MiddleCenter);
                                ix += rtBR.Width;
                                ix += nwgp;
                                var rtTX = Util.FromRect(ix, rtRemarkBox.Top, Convert.ToInt32(ls[i].Width) + 2, rtRemarkBox.Height);
                                ix += rtTX.Width;
                                ix += GP * 2;

                                var SeriesBorderColor = thm.GetBorderColor(s.SeriesColor, ParentContainer.GetBackColor());
                                thm.DrawBox(Canvas, Util.INT(rtBR), s.SeriesColor, SeriesBorderColor, RoundType.Rect, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.OutShadow);
                                Util.DrawText(Canvas, s.Alias, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtTX, DvContentAlignment.MiddleLeft, true);
                            }
                        }
                        #endregion
                        #region Value Axis
                        if (YAxisGraduationCount > 0)
                        {
                            for (var i = 0; i <= YAxisGraduationCount; i++)
                            {
                                var y = Convert.ToInt32(MathTool.Map(i, 0, YAxisGraduationCount, rtGraph.Bottom, rtGraph.Top));

                                p.IsStroke = true;
                                var oo = 0.5f;
                                if (i == 0)
                                {
                                    p.Color = GridColor; p.StrokeWidth = 1;
                                    using (var path = new SKPath())
                                    {
                                        path.MoveTo(rtGraph.Left + oo, y + 1 + oo);
                                        path.LineTo(rtGraph.Right + oo, y + 1 + oo);
                                        Canvas.DrawPath(path, p);
                                    }
                                }
                                else if (i == YAxisGraduationCount)
                                {
                                    p.Color = GridColor; p.StrokeWidth = 1;

                                    using (var path = new SKPath())
                                    {
                                        path.MoveTo(rtGraph.Left + oo, y + oo);
                                        path.LineTo(rtGraph.Right + oo, y + oo);
                                        Canvas.DrawPath(path, p);
                                    }
                                }
                                else if (YAxisGridDraw)
                                {
                                    p.Color = GridColor; p.StrokeWidth = 1;
                                    using (var path = new SKPath())
                                    {
                                        path.MoveTo(rtGraph.Left + oo, y + oo);
                                        path.LineTo(rtGraph.Right + oo, y + oo);
                                        p.PathEffect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 0);
                                        Canvas.DrawPath(path, p);
                                        p.PathEffect = null;
                                    }
                                }

                                foreach (var ser in Series)
                                {
                                    if (dicSer.ContainsKey(ser.Name))
                                    {
                                        var vrt = dicSer[ser.Name];
                                        var val = MathTool.Map(i, 0, YAxisGraduationCount, ser.Minimum, ser.Maximum);
                                        var sval = string.IsNullOrWhiteSpace(ValueFormatString) ? val.ToString("0") : val.ToString(ValueFormatString);
                                        var rtsz = new SKRect();
                                        var sz = p.MeasureText(sval, ref rtsz);
                                        var rt = Util.FromRect(vrt.Left, y - ((rtsz.Height + 2F) / 2F), vrt.Width, (rtsz.Height) + 2F);
                                        Util.DrawText(Canvas, sval, FontName, FontSize, DvFontStyle.Normal, ser.SeriesColor, rt, DvContentAlignment.MiddleRight, true);

                                        if (i == 0)
                                        {
                                            var rtTitle = Util.FromRect(rt.Left, rtContent.Top, rt.Width, rt.Height);
                                            Util.DrawText(Canvas, ser.Alias, FontName, FontSize, DvFontStyle.Normal, ser.SeriesColor, rtTitle, DvContentAlignment.TopRight, true);
                                        }
                                    }
                                }

                            }
                        }
                        #endregion
                        #region Time Axis
                        if (GraphDatas.Count > 0)
                        {
                            p.Color = GridColor; p.StrokeWidth = 1;

                            var st = new DateTime(GraphDatas.First().Time.Ticks / XAxisGraduation.Ticks * XAxisGraduation.Ticks);
                            var ed = new DateTime(Convert.ToInt64(Math.Ceiling(Convert.ToDouble(GraphDatas.Last().Time.Ticks) / Convert.ToDouble(XAxisGraduation.Ticks))) * XAxisGraduation.Ticks);

                            for (DateTime i = st; i <= ed; i += XAxisGraduation)
                            {
                                using (var path = new SKPath())
                                {
                                    p.PathEffect = SKPathEffect.CreateDash(new float[] { 2, 2 }, 0);

                                    var x = Convert.ToSingle(MathTool.Map(Convert.ToDouble(i.Ticks + spos), st.Ticks, st.Ticks + XScale.Ticks, rtGraph.Left, rtGraph.Right));
                                    if (x >= rtGraph.Left && x <= rtGraph.Right)
                                    {
                                        if (XAxisGridDraw && x > rtGraph.Left && x < rtGraph.Right)
                                        {
                                            path.MoveTo(x, rtGraph.Top);
                                            path.LineTo(x, rtGraph.Bottom);
                                        }
                                        Canvas.DrawPath(path, p);

                                        var rtsz = new SKRect();
                                        var sval = i.ToString(string.IsNullOrWhiteSpace(TimeFormatString) ? "yyyy.MM.dd\r\nHH:mm:ss" : TimeFormatString);
                                        p.MeasureText(sval, ref rtsz);
                                        var rt = MathTool.MakeRectangle(new SKPoint(x, rtNameAxis.Top + (rtNameAxis.Height / 2)), (rtsz.Width) + 2, (rtsz.Height) + 2);
                                        Util.DrawText(Canvas, sval, FontName, FontSize, DvFontStyle.Normal, GridColor, rt, DvContentAlignment.MiddleCenter, true);
                                    }

                                    p.PathEffect = null;
                                }
                            }
                        }
                        #endregion
                        #region Data
                        if (Series.Count > 0 && GraphDatas.Count > 0)
                        {
                            var st = new DateTime(GraphDatas.First().Time.Ticks / XAxisGraduation.Ticks * XAxisGraduation.Ticks);
                            var ed = new DateTime(Convert.ToInt64(Math.Ceiling(Convert.ToDouble(GraphDatas.Last().Time.Ticks) / Convert.ToDouble(XAxisGraduation.Ticks))) * XAxisGraduation.Ticks);

                            var sp = Canvas.Save();
                            Canvas.ClipRect(rtGraph);
                            foreach (var v in Series)
                            {
                                var pts = GraphDatas.Select(x => new SKPoint(Convert.ToSingle(MathTool.Map((double)x.Time.Ticks + (double)spos, st.Ticks, st.Ticks + XScale.Ticks, rtGraph.Left, rtGraph.Right)),
                                                                           Convert.ToSingle(MathTool.Map((double)x.Values[v.Name], v.Minimum, v.Maximum, rtGraph.Bottom, rtGraph.Top)))).ToArray();

                                pts = pts.Where(x => x.X >= rtGraph.Left && x.X <= rtGraph.Right).ToArray();
                                p.StrokeWidth = 2;
                                p.Color = v.SeriesColor;

                                if (pts.Length > 1)
                                {
                                    for (int i = 0; i < pts.Length - 1; i++)
                                    {
                                        var p1 = pts[i];
                                        var p2 = pts[i + 1];
                                        Canvas.DrawLine(p1, p2, p);
                                    }
                                }
                            }
                            Canvas.RestoreToCount(sp);
                        }
                        #endregion
                        #region Scroll
                        if (scroll.ScrollTotal > scroll.ScrollView)
                        {
                            var sp = Canvas.Save();
                            thm.DrawBox(Canvas, rtScroll, thm.ScrollBarColor, ScrollBorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill);

                            Canvas.ClipRect(Util.FromRect(rtScroll.Left, rtScroll.Top + 0, rtScroll.Width, rtScroll.Height - 0));

                            var cCur = thm.ScrollCursorOffColor;
                            if (scroll.IsScrolling || scroll.IsTouchMoving) cCur = thm.ScrollCursorOnColor;

                            var rtcur = scroll.GetScrollCursorRect(rtScroll);
                            if (rtcur.HasValue) thm.DrawBox(Canvas, rtcur.Value, cCur, ScrollBorderColor, RoundType.Round, BoxStyle.Fill);

                            Canvas.RestoreToCount(sp);

                        }
                        #endregion


                        p.Typeface = null;
                    }
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            bounds((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraphAl, rtGraph, rtScroll, dicSer, GP, CH) =>
            {
                scroll.MouseDown(x, y, rtScroll);
                if (scroll.TouchMode && CollisionTool.Check(rtGraph, x, y)) scroll.TouchDown(x, y);
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(int x, int y)
        {
            bounds((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraphAl, rtGraph, rtScroll, dicSer, GP, CH) =>
            {
                scroll.MouseMove(x, y, rtScroll);
                if (scroll.TouchMode) scroll.TouchMove(x, y);
            });
            base.OnMouseMove(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            bounds((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraphAl, rtGraph, rtScroll, dicSer, GP, CH) =>
            {
                scroll.MouseUp(x, y);
                if (scroll.TouchMode) scroll.TouchUp(x, y);
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, Dictionary<string, SKRect>, float, float> act)
        {
            using (var p = new SKPaint())
            {
                {
                    p.Typeface = FontTool.GetFont(FontName, DvFontStyle.Normal);
                    p.TextSize = FontSize;


                    var rtCHSZ = new SKRect();
                    p.MeasureText("H", ref rtCHSZ);

                    var CH = rtCHSZ.Height;
                    var GP = DvTheme.GP;
                    #region var dicSer;
                    var vx = 0F;
                    var dicSer = new Dictionary<string, SKRect>();
                    foreach (var x in Series)
                    {
                        var rtMin = new SKRect();
                        var rtMax = new SKRect();
                        var rtTxt = new SKRect();

                        var sMin = string.IsNullOrWhiteSpace(ValueFormatString) ? x.Minimum.ToString("0") : x.Minimum.ToString(ValueFormatString);
                        var sMax = string.IsNullOrWhiteSpace(ValueFormatString) ? x.Maximum.ToString("0") : x.Maximum.ToString(ValueFormatString);
                        var sTxt = x.Alias;

                        p.MeasureText(sMin, ref rtMin);
                        p.MeasureText(sMax, ref rtMax);
                        p.MeasureText(sTxt, ref rtTxt);

                        var w = Math.Max(Math.Ceiling(rtMax.Width) + 1 + GP, Math.Ceiling(rtMin.Width) + 1 + GP) - GP;

                        var vw = Math.Max(Convert.ToInt32(rtTxt.Width + 2), Convert.ToInt32(w));
                        dicSer.Add(x.Name, Util.FromRect(vx, 0, vw, 1));
                        vx += vw;
                        vx += GP;
                    }
                    vx -= GP;
                    #endregion

                    var sz = Util.MeasureText(DateTime.Now.ToString(string.IsNullOrWhiteSpace(TimeFormatString) ? "yyyy.MM.dd\r\nHH:mm:ss" : TimeFormatString), FontName, FontSize, DvFontStyle.Normal);
                    var ValueAxisWidth = Series.Count > 0 ? vx : 0;
                    var NameAxisHeight = (GP + sz.Height);
                    var RemarkAreaHeight = (GP + (CH * 1.5F) + GP);
                    var gpTopMargin = (CH + 2) + GP;
                    var scwh = Scroll.SC_WH;


                    var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
                    var rtRemark = Util.FromRect(rtContent.Left + ValueAxisWidth + GP, rtContent.Bottom - RemarkAreaHeight, rtContent.Width - (ValueAxisWidth + GP) - Convert.ToInt32(sz.Width / 2), RemarkAreaHeight);
                    var rtNameAxis = Util.FromRect(rtContent.Left + ValueAxisWidth + GP, rtRemark.Top - GP - scwh - NameAxisHeight - GP, rtContent.Width - (ValueAxisWidth + GP), NameAxisHeight);
                    var rtValueAxis = Util.FromRect(rtContent.Left, rtContent.Left + gpTopMargin, ValueAxisWidth, rtNameAxis.Top - rtContent.Top - gpTopMargin - GP);
                    var rtGraphAl = Util.FromRect(rtContent.Left + ValueAxisWidth + GP, rtContent.Left + gpTopMargin, rtContent.Width - (ValueAxisWidth + GP), rtValueAxis.Height);
                    var rtGraph = Util.FromRect(rtGraphAl.Left, rtGraphAl.Top, rtGraphAl.Width - Convert.ToInt32(sz.Width / 2), rtGraphAl.Height);
                    var rtScroll = Util.FromRect(rtGraph.Left, rtRemark.Top - GP - scwh, rtGraph.Width, scwh);

                    rtValueAxis.Bottom = rtValueAxis.Top + rtGraph.Height;
                    rtGraphAl.Bottom = rtGraphAl.Top + rtGraph.Height;
                    rtNameAxis.Top += 2;

                    rtScroll = Util.INT(rtScroll);
                    rtRemark = Util.INT(rtRemark);

                    act(rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraphAl, rtGraph, rtScroll, dicSer, GP, CH);
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
                        var tgv = new TGV() { Time = v.Time };

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
}

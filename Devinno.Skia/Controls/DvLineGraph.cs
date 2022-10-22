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
    public class DvLineGraph : DvControl
    {
        #region Properties
        public SKColor? GraphBackColor { get; set; } = null;
        public SKColor? GridColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? RemarkColor { get; set; } = null;
        public List<GraphSeries> Series { get; } = new List<GraphSeries>();

        public double Graduation { get; set; } = 10D;
        public double Minimum { get; set; } = 0D;
        public double Maximum { get; set; } = 100D;

        public string FormatString { get; set; } = null;
        public bool ValueDraw { get; set; } = true;
        public bool PointDraw { get; set; } = true;

        public bool Scrollable { get; set; } = true;
        public bool TouchMode { get => scroll.TouchMode; set => scroll.TouchMode = value; } 

        public int GapWidth { get; set; } = 80;

        private int DataW => GapWidth;

        #region Text
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #endregion

        #region Member Variable
        private List<GV> GraphDatas = new List<GV>();
        private Scroll scroll = new Scroll();
        #endregion

        #region Constructor
        public DvLineGraph()
        {
            scroll = new Scroll() { TouchMode = true, Direction = ScrollDirection.Horizon };
            scroll.GetScrollTotal = () => GraphDatas.Count > 0 && Series.Count > 0 ? GraphDatas.Count * DataW : 0;
            scroll.GetScrollTick = () => ((Series.Count + 2) * GapWidth);
            scroll.GetScrollView = () =>
            {
                long v = 0;
                bounds((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraphAl, rtGraph, rtScroll, GP, CH) =>
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
            bounds((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraphAl, rtGraph, rtScroll, GP, CH) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var GridColor = this.GridColor ?? thm.GridColor;
                    var RemarkColor = this.RemarkColor ?? thm.ButtonColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var bg = GraphBackColor ?? SKColors.Transparent;
                    var RemarkBorderColor = thm.GetBorderColor(RemarkColor, ParentContainer.GetBackColor());
                    var ScrollBorderColor = thm.GetBorderColor(thm.ScrollBarColor, ParentContainer.GetBackColor());
                    
                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        p.Typeface = FontTool.GetFont(FontName, DvFontStyle.Normal);
                        p.TextSize = FontSize;

                        #region Var
                        var spos = 0;
                        var startidx = 0;
                        var endidx = GraphDatas.Count;
                        float DataW = this.DataW;
                        if (Scrollable)
                        {
                            spos = Convert.ToInt32(scroll.ScrollPositionWithOffset);

                            var sc = scroll.ScrollPosition;
                            var si = Convert.ToInt32(Math.Floor((double)(sc - scroll.TouchOffset) / (double)DataW));
                            var cnt = Convert.ToInt32(Math.Ceiling((double)(rtGraph.Width - Math.Min(0, scroll.TouchOffset)) / (double)DataW));
                            var ei = si + cnt;

                            startidx = Math.Max(0, si - 1);
                            endidx = ei + 1;
                        }
                        else DataW = rtNameAxis.Width / GraphDatas.Count;
                        #endregion
                        #region GraphBG
                        if (bg != SKColors.Transparent)
                        {
                            p.IsStroke = false;
                            p.Color = bg;
                            Canvas.DrawRect(rtGraph, p);
                        }
                        #endregion
                        #region Min/Max
                        var Minimum = this.Minimum;
                        var Maximum = this.Maximum;
                        if (GraphDatas.Count > 0)
                        {
                            var rMinimum = GraphDatas.Select(x => x.Values.Min(x2 => x2.Value)).Min();
                            var rMaximum = GraphDatas.Select(x => x.Values.Max(x2 => x2.Value)).Max();
                            Minimum = Math.Min(this.Minimum, rMinimum);
                            Maximum = Math.Max(this.Maximum, Math.Ceiling(rMaximum / Graduation) * Graduation);
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
                            var rtRemarkBox = Util.INT(Util.MakeRectangleAlign(rtRemark, new SKSize(RemarkW, rtRemark.Height), DvContentAlignment.MiddleCenter));
                            thm.DrawBox(Canvas, rtRemarkBox, RemarkColor, RemarkBorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.OutShadow);

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
                                thm.DrawBox(Canvas, rtBR, s.SeriesColor, SeriesBorderColor, RoundType.Rect, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.OutShadow);
                                Util.DrawText(Canvas, s.Alias, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtTX, DvContentAlignment.MiddleLeft, true);
                            }
                        }
                        #endregion
                        #region Value Axis
                        if (Graduation > 0)
                        {
                            for (var i = Minimum; i <= Maximum; i += Graduation)
                            {
                                var n = i;
                                var s = string.IsNullOrWhiteSpace(FormatString) ? n.ToString("0") : n.ToString(FormatString);
                                var y = Convert.ToInt32(MathTool.Map(n, Minimum, Maximum, rtGraph.Bottom, rtGraph.Top));
                                var mrt = new SKRect();
                                p.MeasureText(s, ref mrt);
                                var sz = mrt.Size;
                                var rt = MathTool.MakeRectangle(new SKPoint(0, y), Convert.ToInt32(10), Convert.ToInt32(sz.Height));
                                rt.Left = rtValueAxis.Left; rt.Right = rtValueAxis.Right;
                                Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, GridColor, rt, DvContentAlignment.MiddleRight, true);

                                p.IsStroke = true;
                                var oo = 0.5f;
                                if (n == Minimum)
                                {
                                    p.Color = GridColor; p.StrokeWidth = 1;
                                    using (var path = new SKPath())
                                    {
                                        path.MoveTo(rtGraph.Left + oo, y + 1 + oo);
                                        path.LineTo(rtGraph.Right + oo, y + 1 + oo);
                                        Canvas.DrawPath(path, p);
                                    }
                                }
                                else if (n == Maximum)
                                {
                                    p.Color = GridColor; p.StrokeWidth = 1;

                                    using (var path = new SKPath())
                                    {
                                        path.MoveTo(rtGraph.Left + oo, y + oo);
                                        path.LineTo(rtGraph.Right + oo, y + oo);
                                        Canvas.DrawPath(path, p);
                                    }
                                }
                                else
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
                            }
                        }
                        #endregion
                        #region Name Axis
                        if (GraphDatas.Count > 0)
                        {
                            var sp = Canvas.Save();
                            Canvas.ClipRect(rtNameAxis);

                            p.Color = GridColor; p.StrokeWidth = 1;

                            for (int i = 0; i < GraphDatas.Count; i++)
                            {
                                var itm = GraphDatas[i];
                                var rt = Util.FromRect(spos + rtNameAxis.Left + Convert.ToInt32(DataW * i), rtNameAxis.Top, Convert.ToInt32(DataW), rtNameAxis.Height);
                                if (CollisionTool.Check(rt, rtNameAxis))
                                    Util.DrawText(Canvas, itm.Name, FontName, FontSize, DvFontStyle.Normal, GridColor, rt, DvContentAlignment.MiddleCenter, true);
                            }
                            Canvas.RestoreToCount(sp);
                        }
                        #endregion
                        #region Data
                        if (Series.Count > 0 && GraphDatas.Count > 0)
                        {
                            var dicSer = Series.ToDictionary(x => x.Name);
                            var sp = Canvas.Save();

                            Canvas.ClipRect(Util.FromRect(rtGraph.Left, rtContent.Top, rtGraph.Width, rtGraph.Bottom - rtContent.Top + 1));
                            foreach (var vk in dicSer.Keys)
                            {
                                var ser = dicSer[vk];

                                var ls = new List<LGV>();

                                for (int i = startidx; i < endidx && i < GraphDatas.Count; i++)
                                {
                                    var itm = GraphDatas[i];
                                    var x = spos + rtGraph.Left + (DataW * i) + (DataW / 2F);
                                    var y = Convert.ToInt32(MathTool.Map(itm.Values[vk], Minimum, Maximum, rtGraph.Bottom, rtGraph.Top));
                                    ls.Add(new LGV() { Position = new SKPoint(x, y), Value = itm.Values[vk] });
                                }

                                p.StrokeWidth = 2;
                                p.Color = ser.SeriesColor;

                                if (ls.Count >= 2)
                                {
                                    for (int i = 0; i < ls.Count - 1; i++)
                                    {
                                        var p0 = ls[i].Position;
                                        var p1 = ls[i + 1].Position;
                                        Canvas.DrawLine(p0, p1, p);
                                    }
                                }

                                p.StrokeWidth = 1;
                                if (PointDraw || ValueDraw)
                                {
                                    foreach (var v in ls)
                                    {
                                        var s = string.IsNullOrWhiteSpace(FormatString) ? v.Value.ToString() : v.Value.ToString(FormatString);
                                        var mrt = new SKRect();
                                        p.MeasureText(s, ref mrt);
                                        var sz = mrt.Size;
                                        var rt = MathTool.MakeRectangle(v.Position, 11);
                                        var rtIN = Util.FromRect(rt.Left, rt.Top, rt.Width, rt.Height); rtIN.Inflate(-2, -2);
                                        var rtTxt = MathTool.MakeRectangle(v.Position, Convert.ToInt32(sz.Width)+2, Convert.ToInt32(sz.Height) + 2);
                                        rtTxt.Offset(0, -(FontSize + 2));

                                        if (PointDraw)
                                        {
                                            p.IsStroke = false;
                                            p.Color = ser.SeriesColor;
                                            Canvas.DrawCircle(MathTool.CenterPoint(rt), rt.Width / 2F, p);

                                            p.IsStroke = true;
                                            p.Color = thm.GetBorderColor(ser.SeriesColor, ParentContainer.GetBackColor());
                                            Canvas.DrawCircle(MathTool.CenterPoint(rt), rt.Width / 2F, p);

                                        }
                                        if (ValueDraw) Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, ser.SeriesColor, rtTxt, DvContentAlignment.MiddleCenter, true);
                                    }
                                }
                            }

                            Canvas.RestoreToCount(sp);
                        }
                        #endregion
                        #region Scroll
                        if (Scrollable)
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
            bounds((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraphAl, rtGraph, rtScroll, GP, CH) =>
            {
                if (Scrollable)
                {
                    scroll.MouseDown(x, y, rtScroll);
                    if (scroll.TouchMode && CollisionTool.Check(rtGraph, x, y)) scroll.TouchDown(x, y);
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(int x, int y)
        {
            bounds((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraphAl, rtGraph, rtScroll, GP, CH) =>
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
        protected override void OnMouseUp(int x, int y)
        {
            bounds((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraphAl, rtGraph, rtScroll, GP, CH) =>
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
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, float, float> act)
        {
            using (var p = new SKPaint())
            {
                {
                    p.Typeface = FontTool.GetFont(FontName, DvFontStyle.Normal);
                    p.TextSize = FontSize;

                    var sMin = string.IsNullOrWhiteSpace(FormatString) ? Minimum.ToString("0") : Minimum.ToString(FormatString);
                    var sMax = string.IsNullOrWhiteSpace(FormatString) ? Maximum.ToString("0") : Maximum.ToString(FormatString);

                    var rtCHSZ = new SKRect();
                    var rtMin = new SKRect();
                    var rtMax = new SKRect();

                    p.MeasureText("H", ref rtCHSZ);
                    p.MeasureText(sMin, ref rtMin);
                    p.MeasureText(sMax, ref rtMax);

                    var CH = rtCHSZ.Height;
                    var GP = DvTheme.GP;
                    var ValueAxisWidth = Math.Max(rtMin.Width * 1.5F, rtMax.Width * 1.5F);
                    var NameAxisHeight = (GP + (CH * 1.5F));
                    var RemarkAreaHeight = (GP + (CH * 1.5F) + GP);
                    var gpTopMargin = (CH * 2F)+1;

                    var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
                    var rtRemark = Util.FromRect(rtContent.Left + ValueAxisWidth + GP, rtContent.Bottom - RemarkAreaHeight, rtContent.Width - (ValueAxisWidth + GP), RemarkAreaHeight);
                    var rtNameAxis = Util.FromRect(rtContent.Left + ValueAxisWidth + GP, rtRemark.Top - GP - NameAxisHeight, rtContent.Width - (ValueAxisWidth + GP), NameAxisHeight);
                    var rtValueAxis = Util.FromRect(rtContent.Left, rtContent.Top + gpTopMargin, ValueAxisWidth, rtNameAxis.Top - rtContent.Top - gpTopMargin);
                    var rtGraphAl = Util.FromRect(rtContent.Left + ValueAxisWidth + GP, rtContent.Top + gpTopMargin, rtContent.Width - (ValueAxisWidth + GP), rtValueAxis.Height);
                    var rtScroll = new SKRect(0, 0, 0, 0);
                    var rtGraph = new SKRect();

                    if (!Scrollable)
                    {
                        rtGraph = rtGraphAl;
                    }
                    else
                    {
                        var scwh = Convert.ToInt32(Scroll.SC_WH);

                        rtNameAxis = Util.FromRect(rtContent.Left + ValueAxisWidth + GP, rtRemark.Top - GP - scwh - NameAxisHeight - GP, rtContent.Width - (ValueAxisWidth + GP), NameAxisHeight);
                        rtValueAxis = Util.FromRect(rtContent.Left, rtContent.Top + gpTopMargin, ValueAxisWidth, rtNameAxis.Top - rtContent.Top - gpTopMargin - GP);
                        rtGraphAl = Util.FromRect(rtContent.Left + ValueAxisWidth + GP, rtContent.Top + gpTopMargin, rtContent.Width - (ValueAxisWidth + GP), rtValueAxis.Height);

                        rtGraph = Util.FromRect(rtGraphAl.Left, rtGraphAl.Top, rtGraphAl.Width, rtGraphAl.Height);
                        rtScroll = Util.FromRect(rtGraph.Left, rtRemark.Top - GP - scwh, rtGraph.Width, scwh);
                        rtValueAxis.Bottom = rtGraphAl.Bottom = rtGraph.Bottom;
                    }

                    rtRemark = Util.INT(rtRemark);
                    rtScroll = Util.INT(rtScroll);

                    act(rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraphAl, rtGraph, rtScroll, GP, CH);
                }
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

                    var ls = new List<GV>();
                    foreach (var v in values) ls.Add(new GV() { Name = v.Name, Props = dic, Data = v });
                    GraphDatas = ls;
                }
                else throw new Exception("잘못된 데이터 입니다.");
            }
            else throw new Exception("GraphSeries는 최소 1개 이상이어야 합니다.");
        }
        #endregion
        #endregion

    }
}

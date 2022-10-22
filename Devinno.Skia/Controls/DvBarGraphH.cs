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
    public class DvBarGraphH : DvControl
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

        public BarGraphMode GraphMode { get; set; } = BarGraphMode.LIST;
        public bool Scrollable { get; set; } = true;
        public bool TouchMode { get => scroll.TouchMode; set => scroll.TouchMode = value; }

        public bool Gradient { get; set; } = true;
        public int BarSize { get; set; } = 24;
        public int BarGap { get; set; } = 8;

        private int DataH => GraphMode == BarGraphMode.LIST ? (Series.Count * BarSize) + (BarGap * 2) : BarSize + (BarGap * 2);
       
        #region Text
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #endregion

        #region Member Variable
        private List<GV> GraphDatas = new List<GV>();
        private Scroll scroll = new Scroll();
        private Dictionary<string, SKSize> lsNameSize = new Dictionary<string, SKSize>();
        private Dictionary<string, SKSize> lsSerSize = new Dictionary<string, SKSize>();
        #endregion

        #region Constructor
        public DvBarGraphH()
        {
            scroll = new Scroll() { TouchMode = true, Direction = ScrollDirection.Vertical };
            scroll.GetScrollTotal = () => GraphDatas.Count > 0 && Series.Count > 0 ? GraphDatas.Count * DataH : 0;
            scroll.GetScrollTick = () => DataH;
            scroll.GetScrollView = () =>
            {
                long v = 0;
                bounds((rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraphAl, rtGraph, rtScroll, GP, CH) =>
                {
                    v = Convert.ToInt64(rtGraph.Height);
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
                    var BackColor = ParentContainer.GetBackColor();
                    var bg = GraphBackColor ?? SKColors.Transparent;
                    var RemarkBorderColor = thm.GetBorderColor(RemarkColor, ParentContainer.GetBackColor());
                    var ScrollBorderColor = thm.GetBorderColor(thm.ScrollBarColor, ParentContainer.GetBackColor());

                    var ForeColor = this.ForeColor ?? thm.ForeColor;

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        p.Typeface = FontTool.GetFont(FontName, DvFontStyle.Normal);
                        p.TextSize = FontSize;

                        #region Var
                        var spos = 0;
                        float DataH = this.DataH;
                        if (Scrollable)
                        {
                            spos = Convert.ToInt32(scroll.ScrollPositionWithOffset);
                        }
                        else DataH = rtNameAxis.Height / GraphDatas.Count;
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
                            var rMinimum = GraphMode == BarGraphMode.LIST ? GraphDatas.Select(x => x.Values.Min(x2 => x2.Value)).Min() : GraphDatas.Select(x => x.Values.Sum(x2 => x2.Value)).Min();
                            var rMaximum = GraphMode == BarGraphMode.LIST ? GraphDatas.Select(x => x.Values.Max(x2 => x2.Value)).Max() : GraphDatas.Select(x => x.Values.Sum(x2 => x2.Value)).Max();
                            Minimum = Math.Min(this.Minimum, rMinimum);
                            Maximum = Math.Max(this.Maximum, Math.Ceiling(rMaximum / Graduation) * Graduation);
                        }
                        #endregion
                        #region Remark
                        if (Series.Count > 0)
                        {
                            var RemarkH = (CH * 1.5F);
                            var rtRemarkBox = Util.INT(Util.MakeRectangleAlign(rtRemark, new SKSize(rtRemark.Width, Convert.ToInt32(RemarkH * Series.Count) + GP + (GP / 2F * (Series.Count - 1))), DvContentAlignment.MiddleRight));
                            thm.DrawBox(Canvas, rtRemarkBox, RemarkColor, RemarkBorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.OutShadow);

                            for (int i = 0; i < Series.Count; i++)
                            {
                                var s = Series[i];
                                var rt = Util.FromRect(rtRemarkBox.Left, rtRemarkBox.Top + (RemarkH * i) + (GP / 2) + (GP / 2F * i), rtRemarkBox.Width, RemarkH);
                                var rtBR = MathTool.MakeRectangle(rt, new SKSize(Convert.ToInt32(10), Convert.ToInt32(5)));
                                rtBR.Offset(0, 1);
                                rtBR.Left = rt.Left + GP; rtBR.Right = rtBR.Left + 10;
                                var rtTX = Util.FromRect(rtBR.Right + Convert.ToInt32(5), rt.Top, rt.Width - Convert.ToInt32(5) - rtBR.Width, rt.Height);
                                var SeriasBorderColor = thm.GetBorderColor(s.SeriesColor, ParentContainer.GetBackColor());

                                thm.DrawBox(Canvas, Util.INT(rtBR), s.SeriesColor, SeriasBorderColor, RoundType.Rect, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.OutShadow);
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
                                var x = Convert.ToInt32(MathTool.Map(n, Minimum, Maximum, rtGraph.Left, rtGraph.Right));
                                var y = rtValueAxis.Top + (rtValueAxis.Height / 2);
                                var mrt = new SKRect();
                                p.MeasureText(s, ref mrt);
                                var sz = mrt.Size;
                                var rt = MathTool.MakeRectangle(new SKPoint(x, y), Convert.ToInt32(sz.Width), Convert.ToInt32(sz.Height)); rt.Inflate(2, 2);
                                Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, GridColor, rt, DvContentAlignment.MiddleCenter, true);


                                p.IsStroke = true;
                                var oo = 0.5f;
                                if (n == Minimum)
                                {
                                    p.Color = GridColor; p.StrokeWidth = 1;
                                    using (var path = new SKPath())
                                    {
                                        path.MoveTo(x - 1 + oo, rtGraph.Top + oo);
                                        path.LineTo(x - 1 + oo, rtGraph.Bottom + oo);
                                        Canvas.DrawPath(path, p);
                                    }
                                }
                                else if (n == Maximum)
                                {
                                    p.Color = GridColor; p.StrokeWidth = 1;

                                    using (var path = new SKPath())
                                    {
                                        path.MoveTo(x - 0 + oo, rtGraph.Top + oo);
                                        path.LineTo(x - 0 + oo, rtGraph.Bottom + oo);
                                        Canvas.DrawPath(path, p);
                                    }
                                }
                                else
                                {
                                    p.Color = GridColor; p.StrokeWidth = 1;
                                    using (var path = new SKPath())
                                    {
                                        path.MoveTo(x - 0 + oo, rtGraph.Top + oo);
                                        path.LineTo(x - 0 + oo, rtGraph.Bottom + oo);
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

                            for (int i = 0; i < GraphDatas.Count; i++)
                            {
                                var itm = GraphDatas[i];
                                var rt = Util.FromRect(rtNameAxis.Left, spos + rtNameAxis.Top + Convert.ToInt32(DataH * i), rtNameAxis.Width, Convert.ToInt32(DataH));
                                if (CollisionTool.Check(rt, rtNameAxis))
                                    Util.DrawText(Canvas, itm.Name, FontName, FontSize, DvFontStyle.Normal, GridColor, rt, DvContentAlignment.MiddleCenter, true);
                            }

                            Canvas.RestoreToCount(sp);
                        }
                        #endregion
                        #region Data
                        if (Series.Count > 0 && GraphDatas.Count > 0)
                        {
                            var sp = Canvas.Save();
                            Canvas.ClipRect(rtGraph);

                            var dicSer = Series.ToDictionary(x => x.Name);
                            if (GraphMode == BarGraphMode.LIST)
                            {
                                #region List
                                for (int i = 0; i < GraphDatas.Count; i++)
                                {
                                    var itm = GraphDatas[i];
                                    var rt = Util.FromRect(rtGraph.Left, spos + rtNameAxis.Top + (DataH * i), rtGraph.Width, (DataH));
                                    rt.Inflate(0, -BarGap);

                                    if (CollisionTool.Check(rt, rtGraph))
                                    {
                                        var ih = Math.Min(BarSize, (rt.Height / Series.Count));
                                        var ic = 0;
                                        var tgp = !Scrollable ? ((rt.Height) - (ih * Series.Count)) / 2 : 0;

                                        foreach (var vk in itm.Values.Keys)
                                        {
                                            if (dicSer.ContainsKey(vk))
                                            {
                                                var n = itm.Values[vk];
                                                var w = Convert.ToSingle(MathTool.Map(n, Minimum, Maximum, 0, rtGraph.Width));
                                                var rtv = Util.FromRect(rtGraph.Left, tgp + rt.Top + (ic * ih), (w), ih);
                                                var ser = dicSer[vk];
                                                var bc = ser.SeriesColor.BrightnessTransmit(thm.BorderBrightness);
                                                thm.DrawBox(Canvas, (rtv), ser.SeriesColor, bc, RoundType.Rect, BoxStyle.Border | BoxStyle.InBevel | (Gradient ? BoxStyle.GradientV : BoxStyle.Fill));
                                                if (ValueDraw)
                                                {
                                                    if (n > 0)
                                                    {
                                                        var txt = string.IsNullOrWhiteSpace(FormatString) ? n.ToString() : n.ToString(FormatString);
                                                        Util.DrawText(Canvas, txt, FontName, FontSize, DvFontStyle.Normal, ForeColor, Util.FromRect(rtv.Left, rtv.Top, rtv.Width - 5, rtv.Height), DvContentAlignment.MiddleRight, true);
                                                    }
                                                }
                                            }
                                            ic++;
                                        }
                                    }
                                }
                                #endregion
                            }
                            else if (GraphMode == BarGraphMode.STACK)
                            {
                                #region Stack
                                for (int i = 0; i < GraphDatas.Count; i++)
                                {
                                    var itm = GraphDatas[i];
                                    var rt = Util.FromRect(rtGraph.Left, spos + rtNameAxis.Top + (DataH * i), rtGraph.Width, (DataH));
                                    rt.Inflate(0, -BarGap);

                                    if (CollisionTool.Check(rt, rtGraph))
                                    {
                                        var BarSize = !Scrollable ? Math.Min(this.BarSize, rt.Height) : this.BarSize;
                                        //var ix = rt.Left;
                                        var ix = Convert.ToInt32(rt.Left);
                                        foreach (var vk in itm.Values.Keys)
                                        {
                                            if (dicSer.ContainsKey(vk))
                                            {
                                                var n = itm.Values[vk];
                                                //var w = Convert.ToSingle(MathTool.Map(n, Minimum, Maximum, 0, rtGraph.Width));
                                                var w = Convert.ToInt32(MathTool.Map(n, Minimum, Maximum, 0, rtGraph.Width));
                                                var rtv = !Scrollable ? Util.FromRect(ix, rt.Top + (rt.Height / 2) - (BarSize / 2), (w), BarSize) : Util.FromRect(ix, rt.Top, Convert.ToInt32(w), rt.Height);
                                                var ser = dicSer[vk];
                                                var bc = ser.SeriesColor.BrightnessTransmit(thm.BorderBrightness);
                                                thm.DrawBox(Canvas, (rtv), ser.SeriesColor, bc, RoundType.Rect, BoxStyle.Border | BoxStyle.InBevel | (Gradient ? BoxStyle.GradientV : BoxStyle.Fill));
                                                if (ValueDraw)
                                                {
                                                    if (n > 0)
                                                    {
                                                        var txt = string.IsNullOrWhiteSpace(FormatString) ? n.ToString() : n.ToString(FormatString);
                                                        Util.DrawText(Canvas, txt, FontName, FontSize, DvFontStyle.Normal, ForeColor, Util.FromRect(rtv.Left, rtv.Top, rtv.Width - 5, rtv.Height), DvContentAlignment.MiddleRight, true);
                                                    }
                                                }
                                                //ix = rtv.Right;
                                                ix = Convert.ToInt32(rtv.Right);
                                            }
                                        }
                                    }
                                }
                                #endregion

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
                p.Typeface = FontTool.GetFont(FontName, DvFontStyle.Normal);
                p.TextSize = FontSize;

                if (lsSerSize.Count == 0 || Series.Count != lsSerSize.Count)
                {
                    lsSerSize.Clear();
                    foreach (var v in Series)
                    {
                        var rt = new SKRect();
                        p.MeasureText(v.Alias, ref rt);
                        if (!lsSerSize.ContainsKey(v.Name)) lsSerSize.Add(v.Name, rt.Size);
                    }
                }

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

                var ValueAxisHeight = (GP + (CH * 1.5F));
                var NameAxisWidth = lsNameSize.Count > 0 ? (lsNameSize.Values.Select(x => x.Width).Max() + GP) : 0F;
                var RemarkAreaWidth = lsSerSize.Count > 0 ? (GP + (10) + (5) + lsSerSize.Values.Select(x => x.Width).Max() + GP) : 0F;
                var gpTopMargin = 0;

                var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);
                var rtRemark = Util.FromRect(rtContent.Right - RemarkAreaWidth, rtContent.Top + gpTopMargin, RemarkAreaWidth, rtContent.Height - (gpTopMargin * 2) - GP - ValueAxisHeight);
                var rtNameAxis = Util.FromRect(rtContent.Left, rtContent.Top + gpTopMargin, NameAxisWidth, rtContent.Height - (gpTopMargin * 2) - GP - ValueAxisHeight);
                var rtValueAxis = Util.FromRect(rtNameAxis.Right + GP, rtContent.Top + gpTopMargin + rtRemark.Height + GP, rtContent.Width - (GP * 2) - rtRemark.Width - rtNameAxis.Width - GP, ValueAxisHeight);
                var rtGraphAl = Util.FromRect(rtNameAxis.Right + GP, rtContent.Top + gpTopMargin, rtValueAxis.Width, rtContent.Height - (gpTopMargin * 2) - GP - ValueAxisHeight);
                var rtScroll = new SKRect(0, 0, 0, 0);
                var rtGraph = new SKRect();

                if (!Scrollable)
                {
                    rtGraph = rtGraphAl;
                }
                else
                {
                    var scwh = Convert.ToInt32(Scroll.SC_WH);

                    rtGraph = Util.FromRect(rtGraphAl.Left, rtGraphAl.Top, rtGraphAl.Width - scwh - GP * 2, rtGraphAl.Height);
                    rtScroll = Util.FromRect(rtGraph.Right + GP * 2, rtGraph.Top, scwh, rtGraph.Height);
                }

                rtRemark = Util.INT(rtRemark);
                rtScroll = Util.INT(rtScroll);

                act(rtContent, rtRemark, rtNameAxis, rtValueAxis, rtGraphAl, rtGraph, rtScroll, GP, CH);
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
                        GraphDatas.Add(new GV() { Name = v.Name, Props = dic, Data = v });


                    using (var p = new SKPaint())
                    {
                        p.Typeface = FontTool.GetFont(FontName, DvFontStyle.Normal);
                        p.TextSize = FontSize;

                        lsNameSize.Clear();
                        foreach (var v in GraphDatas)
                        {
                            var rt = new SKRect();
                            p.MeasureText(v.Name, ref rt);
                            if (!lsNameSize.ContainsKey(v.Name)) lsNameSize.Add(v.Name, rt.Size);
                        }

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

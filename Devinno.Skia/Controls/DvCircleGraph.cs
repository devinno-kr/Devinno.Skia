using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
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
        #region Const
        float GPM = 10F;
        #endregion

        #region Properties
        public SKColor? ForeColor { get; set; } = null;

        public List<GraphSeries> Series { get; } = new List<GraphSeries>();

        #region Text
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #endregion

        #region Member Variable
        private List<GV> GraphDatas = new List<GV>();
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
            bounds((rtContent, rtGraph, rtSelectLabel, rtSelectLeft, rtSelectRight, GP, CH) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var BorderColor = thm.GetBorderColor(ForeColor, ParentContainer.GetBackColor());

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        p.Typeface = FontTool.GetFont(FontName, DvFontStyle.Normal);
                        p.TextSize = FontSize;

                        using (var pf = SKImageFilter.CreateDropShadow(1, 1, 1, 1, Util.FromArgb(thm.OutShadowAlpha, SKColors.Black)))
                        {

                            if (Series.Count > 0)
                            {
                                #region Selector
                                if (Series.Count > 1)
                                {
                                    if (bLeftSel) rtSelectLeft.Offset(0, 1);
                                    if (bRightSel) rtSelectRight.Offset(0, 1);

                                    Util.DrawIconFA(Canvas, "fa-chevron-left", FontSize, ForeColor, rtSelectLeft, DvContentAlignment.MiddleCenter, true);
                                    Util.DrawIconFA(Canvas, "fa-chevron-right", FontSize, ForeColor, rtSelectRight, DvContentAlignment.MiddleCenter, true);
                                }
                                if (nSelectedIndex >= 0 && nSelectedIndex < Series.Count)
                                    Util.DrawText(Canvas, Series[nSelectedIndex].Alias, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtSelectLabel, DvContentAlignment.MiddleCenter, true);
                                #endregion

                                #region Graph
                                if (nSelectedIndex >= 0)
                                {
                                    if (GraphDatas.Count > 0)
                                    {
                                        #region Var
                                        var ls = GraphDatas.Select(x => new CGV() { Name = x.Name, Value = x.Values[Series[nSelectedIndex].Name], Color = x.Color });
                                        var cp = MathTool.CenterPoint(rtGraph);

                                        var _rtGR = Util.FromRect(rtGraph.Left, rtGraph.Top, rtGraph.Width, rtGraph.Height); _rtGR.Inflate(-(_rtGR.Width / 10), -(_rtGR.Width / 10));
                                        var _rtGI = Util.FromRect(rtGraph.Left, rtGraph.Top, rtGraph.Width, rtGraph.Height); _rtGI.Inflate(-(_rtGI.Width / 3), -(_rtGI.Height / 3));

                                        var startAngle = 315F;
                                        var sum = ls.Sum(x => x.Value);
                                        var vv = (_rtGR.Width - _rtGI.Width) / 2;
                                        var pt = Convert.ToSingle(MathTool.Constrain(vv / 6 / 1.33, 6, 14));
                                        #endregion
                                        #region Draw
                                        CGV sel = null;
                                        {
                                            foreach (var v in ls)
                                            {
                                                if (v.Value > 0)
                                                {
                                                    using (var pth = new SKPath())
                                                    {
                                                        #region Var
                                                        var aniGP = Convert.ToSingle(Math.Abs(((DateTime.Now - prev).TotalMilliseconds / 20) % (GPM * 2) - GPM));

                                                        var rtGR = Util.FromRect(_rtGR.Left, _rtGR.Top, _rtGR.Width, _rtGR.Height);
                                                        var rtGI = Util.FromRect(_rtGI.Left, _rtGI.Top, _rtGI.Width, _rtGI.Height);

                                                        var sweepAngle = Convert.ToSingle(MathTool.Map(v.Value, 0, sum, 0, 360));
                                                        var dist = Convert.ToSingle(rtGR.Width / 2F + (rtGR.Width / 12));
                                                        var mcp = MathTool.GetPointWithAngle(cp, Convert.ToSingle(MathTool.Map(5, 0, 10, startAngle, startAngle + sweepAngle)), dist);
                                                        var ang = MathTool.GetAngle(cp, mpt);
                                                        var bSel = CollisionTool.CheckCircle(_rtGR, mpt) && !CollisionTool.CheckCircle(_rtGI, mpt) && MathTool.CompareAngle(ang, startAngle, startAngle + sweepAngle);
                                                        var gpoff = bSel ? (Design.Animation ? aniGP : 15F) : 0F;
                                                        var ptOff = MathTool.GetPointWithAngle(cp, Convert.ToSingle(startAngle + sweepAngle / 2.0), gpoff);

                                                        if (bSel) sel = v;
                                                        #endregion
                                                        #region Offset
                                                        rtGI.Offset(Convert.ToInt32(ptOff.X - cp.X), Convert.ToInt32(ptOff.Y - cp.Y));
                                                        rtGR.Offset(Convert.ToInt32(ptOff.X - cp.X), Convert.ToInt32(ptOff.Y - cp.Y));
                                                        mcp.X += Convert.ToInt32(ptOff.X - cp.X);
                                                        mcp.Y += Convert.ToInt32(ptOff.Y - cp.Y);
                                                        #endregion
                                                        #region Path
                                                        pth.AddArc(rtGI, startAngle, sweepAngle);
                                                        pth.ArcTo(rtGR, startAngle + sweepAngle, -sweepAngle, false);
                                                        pth.Close();
                                                        #endregion
                                                        #region Fill
                                                        p.ImageFilter = pf;

                                                        p.IsStroke = false;
                                                        p.Color = v.Color;

                                                        Canvas.DrawPath(pth, p);

                                                        var giw = rtGI.Width / 2F;
                                                        var grw = rtGR.Width / 2F;
                                                        var va1 = giw / grw;
                                                        var vam = ((grw - giw) / 12F) / giw;
                                                        var cs = new SKColor[] { Util.FromArgb(90, SKColors.Black), Util.FromArgb(20, SKColors.Black), Util.FromArgb(50, SKColors.White), Util.FromArgb(0, SKColors.White), Util.FromArgb(30, SKColors.Black) };
                                                        var ps = new float[] { 0F + va1, 0F + va1 + vam, 0.01F + va1 + vam, 0.05F + va1 + vam, 1F };

                                                        if (bSel)
                                                        {
                                                            giw = rtGI.Width / 2F + gpoff;
                                                            grw = rtGR.Width / 2F + gpoff;
                                                            va1 = giw / grw;
                                                            vam = ((grw - giw) / 12F) / (rtGI.Width / 2F);

                                                            var va2 = gpoff / ((grw + gpoff) / 2f);
                                                            ps = new float[] { 0F + va1, 0F + va1 + vam, 0.01F + va1 + vam, 0.05F + va1 + vam, 1F };
                                                        }

                                                        using (var lg = SKShader.CreateRadialGradient(cp, rtGR.Width / 2F + gpoff, cs, ps, SKShaderTileMode.Clamp))
                                                        {
                                                            p.Shader = lg;

                                                            Canvas.DrawPath(pth, p);

                                                            p.Shader = null;
                                                        }
                                                        p.ImageFilter = null;
                                                        #endregion
                                                        #region Border
                                                        {
                                                            p.IsStroke = true;
                                                            p.Color = thm.GetBorderColor(v.Color, ParentContainer.GetBackColor());
                                                            p.StrokeWidth = 1;

                                                            Canvas.DrawPath(pth, p);
                                                            p.IsStroke = false;
                                                        }
                                                        #endregion
                                                        #region Text
                                                        {
                                                            p.ImageFilter = pf;

                                                            var sp = Canvas.Save();

                                                            var ang2 = MathTool.GetAngle(cp, mcp) + 90;

                                                            if (ang2 > 90 && ang2 < 270)
                                                            {
                                                                ang2 -= 180;
                                                            }

                                                            Canvas.Translate(mcp);
                                                            Canvas.RotateDegrees(Convert.ToSingle(ang2));

                                                            var ptVal = Convert.ToInt32(_rtGI.Height / 15F * 1.75F);

                                                            var cT = v.Color;
                                                            var rtv = new SKRect();
                                                            //var str = v.Name + "\r\n" + "" + (v.Value / sum).ToString("0.0%") + "";
                                                            var str = v.Name;
                                                            p.MeasureText(str, ref rtv);
                                                            var sz = rtv.Size;
                                                            var nsz = Math.Max(Convert.ToInt32(sz.Width), Convert.ToInt32(sz.Height));
                                                            var rtt = MathTool.MakeRectangle(new SKPoint(0, 0), nsz, nsz); rtt.Inflate(1, 1);
                                                            Util.DrawText(Canvas, str, FontName, ptVal, DvFontStyle.Normal, cT, rtt, DvContentAlignment.MiddleCenter, true);

                                                            Canvas.RestoreToCount(sp);

                                                            p.ImageFilter = null;
                                                        }
                                                        #endregion

                                                        startAngle += sweepAngle;
                                                    }
                                                }
                                            }
                                        }
                                        #endregion

                                        #region CurrentItem
                                        if (sel != null)
                                        {
                                            p.ImageFilter = pf;

                                            var ptTitle = Convert.ToInt32(_rtGI.Height / 15F * 1.75F);
                                            var ptValPer = Convert.ToInt32(_rtGI.Height / 15F * 1.25F);

                                            var th = _rtGI.Height / 2;
                                            var nh = th / 3;

                                            var rtTitle = Util.FromRect(_rtGI.Left, Convert.ToInt32(_rtGI.Top + (th - th / 2) + (nh * 0)), _rtGI.Width, Convert.ToInt32(nh));
                                            var rtValue = Util.FromRect(_rtGI.Left, Convert.ToInt32(_rtGI.Top + (th - th / 2) + (nh * 1)), _rtGI.Width, Convert.ToInt32(nh));
                                            var rtRaito = Util.FromRect(_rtGI.Left, Convert.ToInt32(_rtGI.Top + (th - th / 2) + (nh * 2)), _rtGI.Width, Convert.ToInt32(nh));

                                            Util.DrawText(Canvas, sel.Name, FontName, ptTitle, DvFontStyle.Normal, ForeColor, rtTitle, DvContentAlignment.MiddleCenter, true);
                                            Util.DrawText(Canvas, sel.Value.ToString(), FontName, ptValPer, DvFontStyle.Normal, ForeColor, rtValue, DvContentAlignment.MiddleCenter, true);
                                            Util.DrawText(Canvas, (sel.Value / sum).ToString("0.0%"), FontName, ptValPer, DvFontStyle.Normal, ForeColor, rtRaito, DvContentAlignment.MiddleCenter, true);

                                            p.ImageFilter = null;
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region NO DATA
                                        var _rtGR = Util.FromRect(rtGraph.Left, rtGraph.Top, rtGraph.Width, rtGraph.Height); _rtGR.Inflate(-(_rtGR.Width / 8), -(_rtGR.Width / 8));
                                        var _rtGI = Util.FromRect(rtGraph.Left, rtGraph.Top, rtGraph.Width, rtGraph.Height); _rtGI.Inflate(-(_rtGI.Width / 4), -(_rtGI.Height / 4));

                                        p.IsStroke = true;
                                        p.StrokeWidth = 5;
                                        var c = ForeColor.BrightnessTransmit(-0.5F);
                                        p.Color = c; Canvas.DrawCircle(MathTool.CenterPoint(_rtGR), _rtGR.Width / 2F, p);
                                        Util.DrawText(Canvas, "NO DATA", FontName, FontSize, DvFontStyle.Normal, c, _rtGR, DvContentAlignment.MiddleCenter, true);
                                        #endregion
                                    }
                                }
                                else
                                {
                                    #region NOT SELECTED
                                    var _rtGR = Util.FromRect(rtGraph.Left, rtGraph.Top, rtGraph.Width, rtGraph.Height); _rtGR.Inflate(-(_rtGR.Width / 8), -(_rtGR.Width / 8));
                                    var _rtGI = Util.FromRect(rtGraph.Left, rtGraph.Top, rtGraph.Width, rtGraph.Height); _rtGI.Inflate(-(_rtGI.Width / 4), -(_rtGI.Height / 4));

                                    p.IsStroke = true;
                                    p.StrokeWidth = 5;
                                    var c = ForeColor.BrightnessTransmit(-0.5F);
                                    p.Color = c; Canvas.DrawCircle(MathTool.CenterPoint(_rtGR), _rtGR.Width / 2F, p);
                                    Util.DrawText(Canvas, "NOT SELECTED", FontName, FontSize, DvFontStyle.Normal, c, _rtGR, DvContentAlignment.MiddleCenter, true);
                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region Empty
                                var c = ForeColor.BrightnessTransmit(-0.5F);
                                Util.DrawIconFA(Canvas, "fa-chevron-left", FontSize, c, rtSelectLeft, DvContentAlignment.MiddleCenter, true);
                                Util.DrawIconFA(Canvas, "fa-chevron-right", FontSize, c, rtSelectRight, DvContentAlignment.MiddleCenter, true);

                                var _rtGR = Util.FromRect(rtGraph.Left, rtGraph.Top, rtGraph.Width, rtGraph.Height); _rtGR.Inflate(-(_rtGR.Width / 8), -(_rtGR.Width / 8));
                                var _rtGI = Util.FromRect(rtGraph.Left, rtGraph.Top, rtGraph.Width, rtGraph.Height); _rtGI.Inflate(-(_rtGI.Width / 4), -(_rtGI.Height / 4));

                                p.IsStroke = true;
                                p.StrokeWidth = 5;
                                p.Color = c; 
                                Canvas.DrawCircle(MathTool.CenterPoint(_rtGR), _rtGR.Width / 2F, p);
                                Util.DrawText(Canvas, "EMPTY", FontName, FontSize, DvFontStyle.Normal, c, _rtGR, DvContentAlignment.MiddleCenter, true);
                                #endregion
                            }

                            p.ImageFilter = null;
                        }
                    }
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            bounds((rtContent, rtGraph, rtSelectLabel, rtSelectLeft, rtSelectRight, GP, CH) =>
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
        protected override void OnMouseUp(int x, int y)
        {
            bLeftSel = bRightSel = false;
            base.OnMouseUp(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(int x, int y)
        {
            mpt.X = x;
            mpt.Y = y;
            base.OnMouseMove(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect, float, float> act)
        {
            using (var p = new SKPaint())
            {
                p.Typeface = FontTool.GetFont(FontName, DvFontStyle.Normal);
                p.TextSize = FontSize;

                var rtCHSZ = new SKRect();
                p.MeasureText("H", ref rtCHSZ);

                var CH = rtCHSZ.Height;
                var GP = 10F;
                var SelectorAreaHeight = Convert.ToInt32(CH * 2);

                var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);
                var rtSelector = Util.FromRect(rtContent.Left, rtContent.Bottom - SelectorAreaHeight, rtContent.Width, SelectorAreaHeight);

                #region var lsSelectLabel;
                var lsSelectLabel = new List<float>();
                foreach (var ser in Series)
                {
                    var rtv = new SKRect();
                    p.MeasureText(ser.Alias, ref rtv);
                    lsSelectLabel.Add(rtv.Width);
                }
                #endregion
                var wSelectLabel = Series.Count > 0 ? Convert.ToInt32(lsSelectLabel.Max(x => x + (GP * 2))) : 0;

                var rtSelectLabel = Util.MakeRectangleAlign(rtSelector, new SKSize(wSelectLabel, rtSelector.Height), DvContentAlignment.MiddleCenter);
                var rtSelectLeft = Util.FromRect(rtSelectLabel.Left - rtSelectLabel.Height, rtSelectLabel.Top, rtSelectLabel.Height, rtSelectLabel.Height);
                var rtSelectRight = Util.FromRect(rtSelectLabel.Right, rtSelectLabel.Top, rtSelectLabel.Height, rtSelectLabel.Height);

                var rt = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, rtContent.Height - rtSelector.Height);
                var wh = Math.Min(rt.Width, rt.Height) - (GP * 2);
                var rtGraph = Util.MakeRectangleAlign(rt, new SKSize(wh, wh), DvContentAlignment.MiddleCenter);

                act(rtContent, rtGraph, rtSelectLabel, rtSelectLeft, rtSelectRight, GP, CH);
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
                        GraphDatas.Add(new GV() { Name = v.Name, Props = dic, Data = v, Color = v.Color });

                    nSelectedIndex = 0;
                }
                else throw new Exception("잘못된 데이터 입니다.");
            }
            else throw new Exception("GraphSeries는 최소 1개 이상이어야 합니다.");
        }
        #endregion
        #endregion
    }
}

using Devinno.Skia.Collections;
using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
using Devinno.Skia.Theme;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Containers
{
    public class DvTabControl : DvContainer
    {
        #region Properties
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? PageColor { get; set; } = null;
        public SKColor? TabBackColor { get; set; } = null;
        public SKColor? PointColor { get; set; } = null;
        public DvPosition TabPosition { get; set; } = DvPosition.Top;
        public int TabSize { get; set; } = 40;
        public int? Corner { get; set; } = null;

        public DvSubPageCollection TabPages { get; private set; }
        #region public DvTabPage SelectedTab { get; set; }
        [JsonIgnore]
        public DvSubPage SelectedTab
        {
            get => nowSelTab;
            set
            {
                if (TabPages.Values.Contains(value))
                {
                    if (nowSelTab != value)
                    {
                        prevSelTab = nowSelTab;
                        nowSelTab = value;

                        if (Animation)
                        {
                            var pi = prevSelTab != null ? TabPages.Values.ToList().IndexOf(prevSelTab) : -1;
                            var ni = nowSelTab != null ? TabPages.Values.ToList().IndexOf(nowSelTab) : -1;

                            bounds((rtContent, rtPage, rtNavi, dicTab) =>
                            {
                                var Width = Convert.ToInt32(rtPage.Width) + 3;
                                var Height = Convert.ToInt32(rtPage.Height) + 3;

                                if (bmCapture != null) bmCapture.Dispose();
                                bmCapture = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul);
                                using (var cv = new SKCanvas(bmCapture)) prevSelTab._Draw(cv);
                            });

                            ani.Stop();
                            ani.Start(DvDesign.ANI2, ni < pi ? "Left" : "Right");
                        }

                        SelectedTabChanged?.Invoke(this, null);
                    }
                }
            }
        }
        #endregion
        #region public string SelectedTabName { get; set; }
        [JsonIgnore]
        public string SelectedTabName
        {
            get => nowSelTab?.Name;
            set
            {
                if (TabPages.ContainsKey(value)) SelectedTab = TabPages[value];
            }
        }
        #endregion
        public override DvControlCollection Controls => SelectedTab?.Controls;
        public bool Animation => Design != null ? Design.Animation : false;

        #region Text
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        #endregion
        #region Icon
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        #endregion
        #endregion

        #region Member Variable
        private DvSubPage nowSelTab = null;
        private DvSubPage prevSelTab = null;
        private Animation ani = new Animation();

        private SKBitmap bmCapture;
        #endregion

        #region Event
        public EventHandler SelectedTabChanged;
        #endregion

        #region Constructor
        public DvTabControl()
        {
            TabPages = new DvSubPageCollection(this);
            TabPages.Changed += (o, s) => OnLayout();
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtPage, rtNavi, dicTab) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    OnLayout();

                    #region Set
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var PointColor = this.PointColor ?? thm.PointColor;
                    var TabColor = this.PageColor ?? thm.TabPageColor;
                    var TabBackColor = this.TabBackColor ?? thm.TabBackColor;
                    var TabBorderColor = thm.GetBorderColor(TabColor, ParentContainer.GetBackColor());
                    var TabBackBorderColor = thm.GetBorderColor(TabBackColor, ParentContainer.GetBackColor());
                    var PointBorderColor = thm.GetBorderColor(PointColor, ParentContainer.GetBackColor());
                    var Corner = this.Corner ?? thm.Corner;
                    #endregion

                    #region Tabs
                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        #region Background
                        {
                            RoundType e = RoundType.Rect;
                            switch (TabPosition)
                            {
                                case DvPosition.Left: e = RoundType.Round_L; break;
                                case DvPosition.Top: e = RoundType.Round_T; break;
                                case DvPosition.Right: e = RoundType.Round_R; break;
                                case DvPosition.Bottom: e = RoundType.Round_B; break;
                            }
                            thm.DrawBox(Canvas, rtNavi, TabBackColor, TabBackBorderColor, e, BoxStyle.Fill | BoxStyle.OutShadow, Corner);
                        }
                        #endregion
                        #region Border
                        {
                            p.IsStroke = true;
                            p.StrokeWidth = 1F;
                            p.Color = TabColor.BrightnessTransmit(-0.1F);

                            var x = Convert.ToInt32(rtNavi.Right) - 0.5F;
                            Canvas.DrawLine(x, rtNavi.Top, x, rtNavi.Bottom, p);
                        }
                        #endregion
                        #region Tab
                        {
                            foreach (var tab in TabPages.Values)
                            {
                                if (dicTab.ContainsKey(tab.Name))
                                {
                                    var cT = SelectedTab == tab ? ForeColor : ForeColor.WithAlpha(60);
                                    var cA = Util.FromArgb(20, SKColors.White);
                                    var rt = dicTab[tab.Name];
                                    var rtc = rtcur(rt);

                                    if (Animation && ani.IsPlaying)
                                    {
                                        if (tab == nowSelTab)
                                        {
                                            cT = ForeColor.WithAlpha(Convert.ToByte(ani.Value(AnimationAccel.Linear, 60, 255)));
                                            cA = SKColors.White.WithAlpha(Convert.ToByte(ani.Value(AnimationAccel.Linear, 0, 20)));
                                        }
                                        if (tab == prevSelTab)
                                        {
                                            cT = ForeColor.WithAlpha(Convert.ToByte(ani.Value(AnimationAccel.Linear, 255, 60)));
                                            cA = SKColors.White.WithAlpha(Convert.ToByte(ani.Value(AnimationAccel.Linear, 20, 0)));
                                        }

                                        if (nowSelTab != null && prevSelTab != null && dicTab.ContainsKey(nowSelTab.Name) && dicTab.ContainsKey(prevSelTab.Name))
                                        {
                                            var rtp = rtcur(dicTab[prevSelTab.Name]);
                                            var rtn = rtcur(dicTab[nowSelTab.Name]);
                                            var rtv = ani.Value(AnimationAccel.DCL, rtp, rtn);
                                            thm.DrawBox(Canvas, Util.INT(rtv), PointColor, PointBorderColor, RoundType.Rect, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.OutShadow);
                                        }
                                    }
                                    else
                                    {
                                        if (tab == SelectedTab)
                                        {
                                            thm.DrawBox(Canvas, Util.INT(rtc), PointColor, PointBorderColor, RoundType.Rect, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.OutShadow);
                                        }
                                    }

                                    Util.DrawTextIcon(Canvas, tab.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, tab.IconString, IconSize, cT, rttxt(rt), IconAlignment, DvContentAlignment.MiddleCenter, true, thm.OutShadowAlpha);
                                }
                            }
                        }
                        #endregion
                       
                    }
                    #endregion
                    #region Page
                    {
                        #region Background
                        RoundType e = RoundType.Rect;
                        switch (TabPosition)
                        {
                            case DvPosition.Left: e = RoundType.Round_R; break;
                            case DvPosition.Top: e = RoundType.Round_B; break;
                            case DvPosition.Right: e = RoundType.Round_L; break;
                            case DvPosition.Bottom: e = RoundType.Round_T; break;
                        }
                        thm.DrawBox(Canvas, rtPage, TabColor, TabBorderColor, e, BoxStyle.Fill | BoxStyle.OutShadow, Corner);
                        #endregion
                        #region Page
                        {
                            var sp = Canvas.Save();
                            Canvas.Translate(rtPage.Left, rtPage.Top);
                            Canvas.ClipRect(Util.FromRect(0, 0, rtPage.Width, rtPage.Height));

                            if (Animation && ani.IsPlaying)
                            {
                                var Width = Convert.ToInt32(rtPage.Width);
                                var Height = Convert.ToInt32(rtPage.Height);

                                var rt = Util.FromRect(0, 0, rtPage.Width, rtPage.Height);
                                var rtP = new SKRect();
                                var rtN = new SKRect();
                                var aP = Convert.ToByte(ani.Value(AnimationAccel.Linear, 255, 0));
                                var aN = Convert.ToByte(ani.Value(AnimationAccel.Linear, 0, 255));

                                #region Animation
                                if (TabPosition == DvPosition.Top || TabPosition == DvPosition.Bottom)
                                {
                                    rtP = ani.Value(AnimationAccel.DCL, rt, Util.FromRect(ani.Variable == "Left" ? Width : 0, 0, 0, Height));
                                    rtN = ani.Value(AnimationAccel.DCL, Util.FromRect(ani.Variable == "Left" ? 0 : Width, 0, 0, Height), rt);
                                }
                                else if (TabPosition == DvPosition.Left || TabPosition == DvPosition.Right)
                                {
                                    rtP = ani.Value(AnimationAccel.DCL, rt, Util.FromRect(0, ani.Variable == "Left" ? Height : 0, Width, 0));
                                    rtN = ani.Value(AnimationAccel.DCL, Util.FromRect(0, ani.Variable == "Left" ? 0 : Height, Width, 0), rt);
                                }
                                #endregion
                                #region prevSelTab
                                if (prevSelTab != null)
                                {
                                    /*
                                    using (var bm = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul))
                                    {
                                        using (var cv = new SKCanvas(bm)) prevSelTab._Draw(cv);

                                        using (var p = new SKPaint())
                                        {
                                            p.Color = SKColors.White.WithAlpha(aP);
                                            Canvas.DrawBitmap(bm, rtP, p);
                                        }
                                    }
                                    */

                                    if (bmCapture != null)
                                    {
                                        using (var p = new SKPaint())
                                        {
                                            p.Color = SKColors.White.WithAlpha(aP);
                                            Canvas.DrawBitmap(bmCapture, rtP, p);
                                        }
                                    }
                                }
                                #endregion
                                #region SelectedTab
                                if (SelectedTab != null)
                                {
                                    using (var bm = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul))
                                    {
                                        using (var cv = new SKCanvas(bm)) SelectedTab._Draw(cv);

                                        using (var p = new SKPaint())
                                        {
                                            p.Color = SKColors.White.WithAlpha(Convert.ToByte(aN * (!Enabled ? 90F / 255F : 1F)));
                                            Canvas.DrawBitmap(bm, rtN, p);
                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                SelectedTab._Draw(Canvas);
                            }
                            Canvas.RestoreToCount(sp);
                        }
                        #endregion
                    }
                    #endregion
                    #region Border
                    thm.DrawBox(Canvas, rtContent, TabBackColor, TabBackBorderColor, RoundType.Round, BoxStyle.Border, Corner);

                    #endregion
                }
            });
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            bounds((rtContent, rtPage, rtNavi, dicTab) =>
            {
                foreach (var tab in TabPages.Values)
                {
                    if (dicTab.ContainsKey(tab.Name))
                    {
                        var rt = dicTab[tab.Name];
                        if (CollisionTool.Check(rt, x, y)) SelectedTab = tab;
                    }
                }

                base.OnMouseDown(x - (int)rtPage.Left, y - (int)rtPage.Top);
            });

        }

        protected override void OnMouseMove(int x, int y)
        {
            bounds((rtContent, rtPage, rtNavi, dicTab) => base.OnMouseMove(x - (int)rtPage.Left, y - (int)rtPage.Top));
        }
        protected override void OnMouseUp(int x, int y)
        {
            bounds((rtContent, rtPage, rtNavi, dicTab) => base.OnMouseUp(x - (int)rtPage.Left, y - (int)rtPage.Top));
        }
        protected override void OnMouseDoubleClick(int x, int y)
        {
            bounds((rtContent, rtPage, rtNavi, dicTab) => base.OnMouseDoubleClick(x - (int)rtPage.Left, y - (int)rtPage.Top));
        }
        #endregion
        #region OnLayout
        protected override void OnLayout()
        {
            bounds((rtContent, rtPage, rtNavi, dicTab) =>
            {
                foreach (var v in TabPages)
                {
                    v.Value.X = (int)rtPage.Left;
                    v.Value.Y = (int)rtPage.Top;
                    v.Value.Width = (int)rtPage.Width;
                    v.Value.Height = (int)rtPage.Height;
                }
            });
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, Dictionary<string, SKRect>> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
            var rtPage = rtContent;
            var rtNavi = new SKRect();
            var dicTab = new Dictionary<string, SKRect>();

            switch (TabPosition)
            {
                case DvPosition.Left:
                    rtNavi = Util.FromRect(rtContent.Left, rtContent.Top, TabSize - 1, rtContent.Height);
                    rtPage = Util.FromRect(rtNavi.Right, rtContent.Top, rtContent.Width - TabSize, rtContent.Height);
                    break;
                case DvPosition.Top:
                    rtNavi = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, TabSize - 1);
                    rtPage = Util.FromRect(rtContent.Left, rtNavi.Bottom, rtContent.Width, rtContent.Height - TabSize);
                    break;
                case DvPosition.Right:
                    rtNavi = Util.FromRect(rtContent.Right - TabSize + 1, rtContent.Top, TabSize - 1, rtContent.Height);
                    rtPage = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width - TabSize, rtContent.Height);
                    break;
                case DvPosition.Bottom:
                    rtNavi = Util.FromRect(rtContent.Left, rtContent.Bottom - TabSize + 1, rtContent.Width, TabSize - 1);
                    rtPage = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, rtContent.Height - TabSize);
                    break;
            }

            var ls = TabPages.Values.ToList();
            float sum = 0F;
            for (int i = 0; i < ls.Count; i++)
            {
                var tab = ls[i];
                var sz = Util.MeasureTextIcon(tab.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, tab.IconString, IconSize, IconAlignment);

                var gp = 5;
                var gpwh = 30;
                var rt = new SKRect();

                #region Rect
                switch (TabPosition)
                {
                    case DvPosition.Left:
                        rt = Util.FromRect(rtNavi.Left, rtNavi.Top + gp + sum, rtNavi.Width, sz.Height + gpwh);
                        sum += (sz.Height + gpwh + gp);
                        break;
                    case DvPosition.Top:
                        rt = Util.FromRect(rtNavi.Left + gp + sum, rtNavi.Top, sz.Width + gpwh, rtNavi.Height);
                        sum += (sz.Width + gpwh + gp);
                        break;
                    case DvPosition.Right:
                        rt = Util.FromRect(rtNavi.Left, rtNavi.Top + gp + sum, rtNavi.Width, sz.Height + gpwh);
                        sum += (sz.Height + gpwh + gp);
                        break;
                    case DvPosition.Bottom:
                        rt = Util.FromRect(rtNavi.Left + gp + sum, rtNavi.Top, sz.Width + gpwh, rtNavi.Height);
                        sum += (sz.Width + gpwh + gp);
                        break;
                }
                #endregion

                dicTab.Add(tab.Name, rt);
            }

            act(rtContent, rtPage, rtNavi, dicTab);
        }
        #endregion
        #region rtcur
        SKRect rtcur(SKRect rt)
        {
            var ret = new SKRect();
            var nc = 5;
            switch (TabPosition)
            {
                case DvPosition.Left: ret = Util.FromRect(rt.Right - nc, rt.Top, nc, rt.Height); break;
                case DvPosition.Top: ret = Util.FromRect(rt.Left, rt.Bottom - nc, rt.Width, nc); break;
                case DvPosition.Right: ret = Util.FromRect(rt.Left, rt.Top, nc, rt.Height); break;
                case DvPosition.Bottom: ret = Util.FromRect(rt.Left, rt.Top, rt.Width, nc); break;
            }
            return ret;
        }
        #endregion
        #region rttxt
        SKRect rttxt(SKRect rt)
        {
            var ret = new SKRect();
            var nc = 3;
            switch (TabPosition)
            {
                case DvPosition.Left: ret = Util.FromRect(rt.Left, rt.Top, rt.Width - nc, rt.Height); break;
                case DvPosition.Top: ret = Util.FromRect(rt.Left, rt.Top, rt.Width, rt.Height - nc); break;
                case DvPosition.Right: ret = Util.FromRect(rt.Left + nc, rt.Top, rt.Width - nc, rt.Height); break;
                case DvPosition.Bottom: ret = Util.FromRect(rt.Left, rt.Top + nc, rt.Width, rt.Height - nc); break;
            }
            return ret;
        }
        #endregion
        #endregion
    }
}

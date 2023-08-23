using Devinno.Skia.Collections;
using Devinno.Skia.Design;
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
        #region Color
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? PageColor { get; set; } = null;
        public SKColor? TabBackColor { get; set; } = null;
        public SKColor? PointColor { get; set; } = null;
        #endregion
        #region Tab
        public DvRoundType? Round { get; set; } = null;
        public DvPosition TabPosition { get; set; } = DvPosition.Top;
        public int TabSize { get; set; } = 40;
        #endregion
        #region Control
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

                            ani.Stop();
                            ani.Start(DvDesign.ANI2, ni < pi ? "Prev" : "Next");
                        }

                        SelectedTabChanged?.Invoke(this, null);
                    }
                }
            }
        }
        #endregion
        public override DvControlCollection Controls => SelectedTab?.Controls;
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
        #endregion

        #region Member Variable
        private DvSubPage nowSelTab = null;
        private DvSubPage prevSelTab = null;
        private Animation ani = new Animation();
        #endregion

        #region Event
        public event EventHandler SelectedTabChanged;
        #endregion

        #region Constructor
        public DvTabControl()
        {
            TabPages = new DvSubPageCollection(this);
            TabPages.Changed += (o, s) => OnLayout();
        }
        #endregion

        #region Override
        #region Draw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtPage, rtNavi, dicTab) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region var
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var PointColor = this.PointColor ?? thm.PointColor;
                    var TabColor = this.PageColor ?? thm.PanelColor;
                    var TabBackColor = this.TabBackColor ?? thm.ControlBackColor;
                    var Round = this.Round ?? DvRoundType.All;
                    #endregion

                    #region Layout
                    OnLayout();
                    #endregion

                    #region Draw
                    thm.DrawTabControl(Canvas,
                        rtContent, rtPage, rtNavi, dicTab,
                        ForeColor, BackColor, PointColor, TabColor, TabBackColor,
                        FontName, FontSize, FontStyle, IconSize, IconGap, IconAlignment,
                        Round, TabPosition, TabSize,
                        TabPages, nowSelTab, prevSelTab,
                        Animation, ani);
                    #endregion

                    #region Page
                    if (SelectedTab != null)
                    {
                        if (Animation && ani.IsPlaying)
                        {
                            TabTool.AniPage(TabPages, nowSelTab, prevSelTab, TabPosition,
                                rtContent, rtPage, ani, (rtP, rtN, aP, aN) =>
                            {
                                #region Prev
                                if (prevSelTab != null)
                                {
                                    var sp = Canvas.Save();
                                    Canvas.ClipRect(rtPage);

                                    var w = Convert.ToInt32(rtPage.Width);
                                    var h = Convert.ToInt32(rtPage.Height);

                                    using (var bm = new SKBitmap(w, h, SKColorType.Rgba8888, SKAlphaType.Premul))
                                    {
                                        using (var cv = new SKCanvas(bm)) prevSelTab._Draw(cv);

                                        using (var p = new SKPaint { IsAntialias = DvDesign.AA, IsDither = DvDesign.DI, FilterQuality = DvDesign.FQ })
                                        {
                                            p.Color = SKColors.White.WithAlpha(aP);
                                            Canvas.DrawBitmap(bm, rtP, p);
                                        }
                                    }

                                    Canvas.RestoreToCount(sp);
                                }
                                #endregion
                                #region Now
                                if (nowSelTab != null)
                                {
                                    var sp = Canvas.Save();
                                    Canvas.ClipRect(rtPage);

                                    var w = Convert.ToInt32(rtPage.Width);
                                    var h = Convert.ToInt32(rtPage.Height);

                                    using (var bm = new SKBitmap(w, h, SKColorType.Rgba8888, SKAlphaType.Premul))
                                    {
                                        using (var cv = new SKCanvas(bm)) nowSelTab._Draw(cv);

                                        using (var p = new SKPaint { IsAntialias = DvDesign.AA, IsDither = DvDesign.DI, FilterQuality = DvDesign.FQ })
                                        {
                                            p.Color = SKColors.White.WithAlpha(aN);
                                            Canvas.DrawBitmap(bm, rtN, p);
                                        }
                                    }

                                    Canvas.RestoreToCount(sp);
                                }
                                #endregion
                            });
                        }
                        else
                        {
                            #region Now
                            var sp = Canvas.Save();
                            Canvas.ClipRect(rtPage);
                            Canvas.Translate(rtPage.Left, rtPage.Top);

                            nowSelTab._Draw(Canvas);

                            Canvas.RestoreToCount(sp);
                            #endregion
                        }
                    }
                    #endregion
                }
            });

            //base.OnDraw(Canvas);
        }
        #endregion

        #region Mouse
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtPage, rtNavi, dicTab) =>
            {
                base.OnMouseDown(x - rtPage.Left, y - rtPage.Top);

                foreach (var tab in TabPages.Values)
                    if (dicTab.ContainsKey(tab.Name))
                        if (CollisionTool.Check(dicTab[tab.Name], x, y)) SelectedTab = tab;
            });
        }

        protected override void OnMouseMove(float x, float y) => Areas((rtContent, rtPage, rtNavi, dicTab) => base.OnMouseMove(x - rtPage.Left, y - rtPage.Top));
        protected override void OnMouseUp(float x, float y) => Areas((rtContent, rtPage, rtNavi, dicTab) => base.OnMouseUp(x - rtPage.Left, y - rtPage.Top));
        protected override void OnMouseDoubleClick(float x, float y) => Areas((rtContent, rtPage, rtNavi, dicTab) => base.OnMouseDoubleClick(x - rtPage.Left, y - rtPage.Top));
        #endregion

        #region Layout
        protected override void OnLayout()
        {
            Areas((rtContent, rtPage, rtNavi, dicTab) =>
            {
                foreach (var page in TabPages.Values) page.Bounds = rtPage;
            });
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, Dictionary<string, SKRect>> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);
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
        #endregion
    }
}

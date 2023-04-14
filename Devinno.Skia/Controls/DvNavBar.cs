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
    public class DvNavBar : DvControl
    {
        #region Properties
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;

        public SKBitmap LogoImage { get; set; }
        public string Title { get; set; }
        public string TitleFontName { get; set; } = "NanumGothic";
        public int TitleFontSize { get; set; } = 18;

        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        public string IconString { get; set; } = null;

        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;

        public List<string> PageNames { get; } = new List<string>();
        public List<TextIconItem> FoldingMenus { get; } = new List<TextIconItem>();

        public int DropDownViewCount { get; set; } = 8;
        public int DropDownItemHeight { get; set; } = 30;
        public int? DropDownWidth { get; set; } = null;

        public bool Animation => Design != null ? Design.Animation : false;
        #endregion

        #region Member Variable
        DropDownWindow ddwnd = new DropDownWindow();
        Animation ani = new Animation();
        #endregion

        #region Event
        public event EventHandler<FoldingMenuClickedEventArgs> FoldingMenuClicked;
        #endregion

        #region Constructor
        public DvNavBar()
        {
            Margin = new Padding(0);
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtLogo, rtTitle, rtMenus, ls, rtFoldingMenus) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    ddwnd.BoxColor = this.BoxColor;
                    ddwnd.ForeColor = this.ForeColor;

                    var BoxColor = this.BoxColor ?? thm.LabelColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var ForeColorA = Util.FromArgb(75, ForeColor);
                    var BorderColor = this.BorderColor ?? Util.FromArgb(120, ForeColor);
                    var Corner = thm.Corner;

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        #region Box
                        {
                            var rt = rtContent;
                            thm.DrawBox(Canvas, rt, BoxColor, BorderColor, RoundType.Rect, BoxStyle.Fill);
                        }
                        #endregion
                        #region Logo
                        if(rtLogo.HasValue && LogoImage != null)
                        {
                            var rt = rtLogo.Value;
                            Canvas.DrawBitmap(LogoImage, rt, p);
                        }
                        #endregion
                        #region Title
                        if (rtTitle.HasValue && !string.IsNullOrWhiteSpace(Title))
                        {
                            var rt = rtTitle.Value;
                            Util.DrawText(Canvas, Title, TitleFontName, TitleFontSize, DvFontStyle.Normal, ForeColor, rt);
                        }
                        #endregion
                        #region Menus
                        if(rtMenus.HasValue && ls.Count > 0)
                        {
                            var rt = rtMenus.Value;
                            var sp = Canvas.Save();
                            Canvas.ClipRect(rt);
                            Canvas.Translate(rt.Left, rt.Top);

                            var sold = "";
                            var snew = "";
                            if (Animation && ani.IsPlaying)
                            {
                                var strs = ani.Variable.Split(':');
                                if (strs.Length == 2)
                                {
                                    sold = strs[0];
                                    snew = strs[1];
                                }
                            }

                            var first = ls.FirstOrDefault();
                            foreach (var v in ls)
                            {
                                var page = v.page;
                                var rtv = v.bounds;
                                var text = page.Text;
                                var iconstring = page.IconString;
                                if (first != v)
                                {
                                    var vh = Convert.ToInt32(FontSize / 2);
                                    var x = Convert.ToInt32(rtv.Left) + 0.5F;
                                    var y1 = Convert.ToInt32(rtv.MidY - vh) - 0.5F;
                                    var y2 = Convert.ToInt32(rtv.MidY + vh) + 0.5F;

                                    p.IsStroke = true;
                                    p.Color = ForeColorA;
                                    p.StrokeWidth = 1F;

                                    Canvas.DrawLine(x, y1, x, y2, p);
                                }

                                var c = Design.CurrentPage == page ? ForeColor : ForeColorA;

                                if (Animation && ani.IsPlaying)
                                {
                                    if (sold == page.Name) c = ani.Value(AnimationAccel.DCL, ForeColor, ForeColorA);
                                    if (snew == page.Name) c = ani.Value(AnimationAccel.DCL, ForeColorA, ForeColor);
                                }
                                Util.DrawTextIcon(Canvas, text, FontName, FontSize, DvFontStyle.Normal, IconGap, iconstring, IconSize, c, rtv, IconAlignment);
                            }

                            Canvas.RestoreToCount(sp);
                        }
                        #endregion
                        #region FoldingMenus
                        if(rtFoldingMenus.HasValue && FoldingMenus.Count > 0)
                        {
                            var rt = rtFoldingMenus.Value;
                            var c = BorderColor;
                            thm.DrawBox(Canvas, rt, BoxColor, c, RoundType.Round, BoxStyle.Border | BoxStyle.Fill);
                            Util.DrawIconFA(Canvas, "fa-bars", Convert.ToInt32(rt.Height / 2), c, rtFoldingMenus.Value);
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
            bounds((rtContent, rtLogo, rtTitle, rtMenus, ls, rtFoldingMenus) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null && !ddwnd.AnimationPlaying)
                {
                    if (rtMenus.HasValue)
                    {
                        var vx = x - rtMenus.Value.Left;
                        var vy = y - rtMenus.Value.Top;

                        foreach (var v in ls)
                        {
                            if (CollisionTool.Check(v.bounds, vx, vy))
                            {
                                if (Design.CurrentPage?.Name != v.page.Name)
                                {
                                    var sold = Design.CurrentPage != null ? Design.CurrentPage.Name : "";
                                    Design.SetPage(v.page);
                                    var snew = Design.CurrentPage != null ? Design.CurrentPage.Name : "";
                                    if(Animation) ani.Start(DvDesign.ANI2, sold + ":" + snew);
                                }
                            }
                        }
                    }
                                       
                    if(rtFoldingMenus.HasValue && FoldingMenus.Count > 0)
                    {
                        if(CollisionTool.Check(rtFoldingMenus.Value, x, y))
                        {
                            var rt = rtFoldingMenus.Value;
                            
                            var sx = ScreenX + rt.Left;
                            var sy = ScreenY + rt.Top;
                            var sw = rt.Width;
                            var sh = rt.Height;

                            var vl = FoldingMenus.OrderBy(x => x.Text.Length).LastOrDefault();
                            var sz = Util.MeasureTextIcon(vl.Text, FontName, 12, DvFontStyle.Normal, 8, vl.IconString, 12, DvTextIconAlignment.LeftRight);
                            
                            var vw = DropDownWidth ?? Math.Max(Convert.ToInt32(sz.Width + 30), sw);
                            var vh = Math.Max(Math.Min(FoldingMenus.Count, DropDownViewCount) * 30 + 12, sh);

                            var rts = Util.FromRect(sx, sy, sw, sh);
                            var rte = Util.FromRect(sx + sw - vw, sy, vw, vh);

                            ddwnd.ItemHeight = DropDownItemHeight;
                            ddwnd.BoxColor = BoxColor;
                            ddwnd.BorderColor = BorderColor;
                            ddwnd.ShowDropDown(rts, rte, FoldingMenus, (result) =>
                            {
                                FoldingMenuClicked?.Invoke(this, new FoldingMenuClickedEventArgs(result));
                            });
                        }
                    }
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect?, SKRect?, SKRect?, List<NBITEM>, SKRect?> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width, this.Height - 2);
            var rtLogo = (SKRect?)null;
            var rtTitle = (SKRect?)null;
            var rtFoldingMenus = (SKRect?)null;
            var rtMenus = (SKRect?)null;

            var ls = new List<NBITEM>();

            var H = rtContent.Height - 20;
            var x = 10F;

            if (FoldingMenus.Count > 0)
            {
                var rt = Util.FromRect(Width - 10 - H, 0, H, rtContent.Height);
                rtFoldingMenus = Util.MakeRectangle(rt, new SKSize(H, H));
            }

            if (LogoImage != null)
            {
                var ratio = H / LogoImage.Height;
                var w = Convert.ToInt32(LogoImage.Width * ratio);
                var h = Convert.ToInt32(LogoImage.Height * ratio);
                
                var rt = Util.FromRect(x, 0, H, rtContent.Height);
                rtLogo = Util.MakeRectangle(rt, new SKSize(w, h));
                x += w + 10;
            }

            if (!string.IsNullOrWhiteSpace(Title))
            {
                var sz = Util.MeasureText(Title, TitleFontName, TitleFontSize, DvFontStyle.Normal);
                
                var rt = Util.FromRect(x, 0, Convert.ToInt32(sz.Width + 10), rtContent.Height);
                rtTitle = Util.MakeRectangle(rt, new SKSize(rt.Width, H));
                x += rtTitle.Value.Width + 10;
            }

            if (x != 10F) x += 0;

            if (Design != null)
            {
                var vx = 0F;
                foreach (var pn in PageNames)
                {
                    if(Design.Pages.ContainsKey(pn))
                    {
                        var page = Design.Pages[pn];
                        var sz = Util.MeasureTextIcon(page.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, page.IconString, IconSize, IconAlignment);
                        var rt = Util.FromRect(vx, 0, Convert.ToInt32(sz.Width + 30), H);

                        ls.Add(new NBITEM() { page = page, bounds = rt });
                        vx += rt.Width;
                    }
                }

                if(ls.Count > 0)
                {
                    var rt = new SKRect(x, 0, (rtFoldingMenus.HasValue ? rtFoldingMenus.Value.Left : rtContent.Right) - 10, rtContent.Height);
                    rtMenus = Util.MakeRectangle(rt, new SKSize(rt.Width, H));
                }
            }

            act(rtContent, rtLogo, rtTitle, rtMenus, ls, rtFoldingMenus);
        }
        #endregion
        #endregion

        #region class : DropDownWindow
        class DropDownWindow : DvDropDownWindow
        {
            #region Properties
            public SKColor? BoxColor { get; set; } = null;
            public SKColor? BorderColor { get; set; } = null;
            public int ItemHeight
            {
                get => list.ItemHeight;
                set => list.ItemHeight = value;
            }
            #endregion
            #region Member Variable
            DvListBox list;
            Action<TextIconItem> result;
            #endregion
            #region Constructor
            public DropDownWindow()
            {
                list = new DvListBox
                {
                    Name = nameof(list),
                    BackgroundDraw = false,
                    Fill = true,
                    Margin = new Padding(5),
                    SelectionMode = ItemSelectionMode.NONE,
                };
                list.ItemClicked += (o, s) =>
                {
                    if (result != null) result(s.Item);
                    Hide();
                };
                Controls.Add(list);
            }
            #endregion
            #region Override
            #region OnDraw
            protected override void OnDraw(SKCanvas Canvas)
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    list.DrawScroll = !AnimationPlaying && (list.ItemHeight * list.Items.Count > list.Height);

                    var BoxColor = this.BoxColor ?? thm.LabelColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BorderColor = this.BorderColor ?? Util.FromArgb(120, ForeColor);
                    var Corner = thm.Corner;

                    var rt = Util.FromRect(X, Y, Width, Height);
                    thm.DrawBox(Canvas, rt, BoxColor, BorderColor, RoundType.Round, BoxStyle.Fill | BoxStyle.Border);
                }

                base.OnDraw(Canvas);
            }
            #endregion
            #endregion
            #region Method
            #region ShowDropDown
            public void ShowDropDown(SKRect sbounds, SKRect ebounds, List<TextIconItem> menus, Action<TextIconItem> result)
            {
                this.result = result;
                list.Items.Clear();
                list.Items.AddRange(menus);

                this.Show(sbounds, ebounds);
            }
            #endregion
            #endregion
        }
        #endregion
        #region class : NBITEM
        class NBITEM
        {
            public DvPage page { get; set; }
            public SKRect bounds { get; set; }
        }
        #endregion
    }

    #region class : FoldingMenuClickedEventArgs
    public class FoldingMenuClickedEventArgs
    {
        public TextIconItem Item { get; private set; }

        public FoldingMenuClickedEventArgs(TextIconItem item)
        {
            this.Item = item;
        }
    }
    #endregion
}

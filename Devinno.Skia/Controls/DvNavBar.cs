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
        #region Color
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BorderColor { get; set; } = null;
        #endregion
        #region Title
        public SKBitmap LogoImage { get; set; }
        public string Title { get; set; }
        public string TitleFontName { get; set; } = "NanumGothic";
        public int TitleFontSize { get; set; } = 18;
        #endregion
        #region Text/Icon
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        
        public float IconSize { get; set; } = 12;
        public int IconGap { get; set; } = 5;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        #endregion
        #region Pages
        public List<string> PageNames { get; } = new List<string>();
        public List<DvTextIcon> FoldingMenus { get; } = new List<DvTextIcon>();
        #endregion
        #region DropDown
        public int DropDownViewCount { get; set; } = 8;
        public int DropDownItemHeight { get; set; } = 30;
        public int? DropDownWidth { get; set; } = null;
        #endregion
        #region Animation
        public bool Animation => Design != null ? Design.Animation : false;
        #endregion
        #region BarHeight
        public int BarHeight { get; set; } = 50;
        #endregion
        #endregion

        #region Member Variable
        FoldingDropDownWindow ddwnd = new FoldingDropDownWindow();
        Animation ani = new Animation();
        bool bPrev = false, bNext = false;
        bool bRight = false;
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
            var ds = Design;
            var thm = Design?.Theme;
            if (ds != null && thm != null)
            {
                Fill = true;
                Margin = new Padding(0, 0, 0, ds.Height - BarHeight);

                Areas((rtContent, rtLogo, rtTitle, rtMenus, rtMenuPrev, rtMenuNext, rtFoldingMenus, mls) =>
                {
                    #region var
                    var BoxColor = this.BoxColor ?? thm.LabelColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var ForeColorA = Util.FromArgb(60, ForeColor);
                    var BorderColor = this.BorderColor ?? Util.FromArgb(120, ForeColor);
                    var Corner = thm.Corner;
                    #endregion

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        #region Box
                        p.Color = BoxColor;
                        Canvas.DrawRect(rtContent, p);
                        #endregion
                        #region Logo
                        if (rtLogo.HasValue && LogoImage != null)
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
                        if (mls.Count > 0)
                        {
                            #region var
                            var lx = mls.Last().Bounds.Right;
                            var nx = 0F;
                            if (ani.IsPlaying)
                            {
                                if (ani.Variable == "l") nx = ani.Value(AnimationAccel.DCL, lx - rtMenus.Width, 0);
                                if (ani.Variable == "r") nx = ani.Value(AnimationAccel.DCL, 0, lx - rtMenus.Width);
                            }
                            else nx = bRight ? lx - rtMenus.Width : 0F;
                            #endregion

                            #region Menus
                            var sp = Canvas.Save();
                            Canvas.ClipRect(rtMenus);
                            Canvas.Translate(rtMenus.Left - nx, 0);

                            foreach (var v in mls)
                            {
                                var bcl = v.Page == Design.CurrentPage ? ForeColor : ForeColorA;
                                thm.DrawText(Canvas, v.Bounds, v.TextIcon, bcl);

                                if (v.SepX.HasValue)
                                {
                                    p.IsAntialias = false;

                                    var vh = Convert.ToInt32(FontSize / 2);
                                    var x = v.SepX.Value;
                                    var y1 = rtMenus.MidY - vh;
                                    var y2 = rtMenus.MidY + vh;

                                    p.IsStroke = true;
                                    p.Color = ForeColorA;
                                    p.StrokeWidth = 1F;

                                    Canvas.DrawLine(x, y1, x, y2, p);
                                }
                            }

                            Canvas.RestoreToCount(sp);
                            #endregion

                            #region Prev / Next
                            if (lx > rtMenus.Width)
                            {
                                var bcl = !bRight ? ForeColorA : ForeColor;
                                var bcr = bRight ? ForeColorA : ForeColor;

                                thm.DrawText(Canvas, rtMenuPrev, "fa-chevron-left", 12, bcl, DvContentAlignment.MiddleCenter, bPrev);
                                thm.DrawText(Canvas, rtMenuNext, "fa-chevron-right", 12, bcr, DvContentAlignment.MiddleCenter, bNext);
                            }
                            #endregion
                        }
                        #endregion
                        #region FoldingMenus
                        if (rtFoldingMenus.HasValue && FoldingMenus.Count > 0)
                        {
                            var rt = rtFoldingMenus.Value;
                            var c = BorderColor;
                            thm.DrawBox(Canvas, rt, BoxColor, BorderColor, DvRoundType.All, BoxStyle.Border | BoxStyle.Fill, Corner);
                            thm.DrawText(Canvas, rt, "fa-bars", rt.Height / 2F, c);
                        }
                        #endregion
                    }

                });
            }
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtLogo, rtTitle, rtMenus, rtMenuPrev, rtMenuNext, rtFoldingMenus, mls) =>
            {
                if (CollisionTool.Check(rtMenuPrev, x, y) && bRight) bPrev = true;
                if (CollisionTool.Check(rtMenuNext, x, y) && !bRight) bNext = true;
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtLogo, rtTitle, rtMenus, rtMenuPrev, rtMenuNext, rtFoldingMenus, mls) =>
            {
                if (bPrev) bPrev = false;
                if (bNext) bNext = false;
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #region OnMouseClick
        protected override void OnMouseClick(float x, float y)
        {
            Areas((rtContent, rtLogo, rtTitle, rtMenus, rtMenuPrev, rtMenuNext, rtFoldingMenus, mls) =>
            {
                if (mls.Count > 0)
                {
                    var lx = mls.Last().Bounds.Right;
                    var nx = bRight ? lx - rtMenus.Width : 0;

                    #region Prev/Next
                    if (lx > rtMenus.Width)
                    {
                        if (CollisionTool.Check(rtMenuPrev, x, y) && bRight)
                        {
                            bRight = false;
                            if (Animation && !ani.IsPlaying) ani.Start(DvDesign.ANI2, "l");
                        }
                        if (CollisionTool.Check(rtMenuNext, x, y) && !bRight)
                        {
                            bRight = true;
                            if (Animation && !ani.IsPlaying) ani.Start(DvDesign.ANI2, "r");
                        }
                    }
                    #endregion

                    #region SetPage
                    if (CollisionTool.Check(Util.FromRect(rtMenus.Left, rtContent.Top, rtMenus.Width, rtContent.Height), x, y))
                        foreach (var v in mls)
                        {
                            var rt = Util.FromRect(v.Bounds); rt.Inflate(10, 10);
                            if (CollisionTool.Check(rt, x - rtMenus.Left + nx, y)) Design.SetPage(v.Page);
                        }
                    #endregion

                    #region FoldingMenu
                    if (rtFoldingMenus.HasValue && FoldingMenus.Count > 0 && Design != null && Design.Theme != null)
                    {
                        if (CollisionTool.Check(rtFoldingMenus.Value, x, y))
                        {
                            var rt = rtFoldingMenus.Value;

                            var sx = ScreenX + rt.Left;
                            var sy = ScreenY + rt.Top;
                            var sw = rt.Width;
                            var sh = rt.Height;

                            var vl = FoldingMenus.OrderBy(x => x.Text.Length).LastOrDefault();
                            var sz = Util.MeasureTextIcon(vl.Text, FontName, 12, DvFontStyle.Normal, 8, vl.IconString, 12, DvTextIconAlignment.LeftRight);

                            var vw = DropDownWidth ?? Math.Max(Convert.ToInt32(sz.Width + 40), sw);
                            var vh = Math.Max(Math.Min(FoldingMenus.Count, DropDownViewCount) * 30 + 20, sh);

                            var rts = Util.FromRect(sx, sy, sw, sh);
                            var rte = Util.FromRect(sx + sw - vw, sy, vw, vh);
                            var BoxColor = this.BoxColor ?? Design.Theme.LabelColor;

                            ddwnd.ShowDropDown(rts, rte, FoldingMenus, DropDownItemHeight, DropDownViewCount,
                                BoxColor, ForeColor ?? Design.Theme.ForeColor, (result) =>
                            {
                                if (result != null)
                                    FoldingMenuClicked?.Invoke(this, new FoldingMenuClickedEventArgs(result));
                            });
                        }
                    }
                    #endregion
                }
            });
            base.OnMouseClick(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect?, SKRect?, SKRect, SKRect, SKRect, SKRect?, List<NavItem>> act)
        {
            #region var
            var rtContent = Util.FromRect(0, 0, Width, Height);
            var rtLogo = (SKRect?)null;
            var rtTitle = (SKRect?)null;
            var rtMenus = new SKRect();
            var rtMenuPrev = new SKRect();
            var rtMenuNext = new SKRect();
            var rtFoldingMenus = (SKRect?)null;
            #endregion

            #region rts
            var rt = Util.FromRect(rtContent); rt.Inflate(-10, -10);
            var lsz = new List<SizeInfo>();
            #region Logo
            if (LogoImage != null)
            {
                lsz.Add(new SizeInfo(DvSizeMode.Pixel, (LogoImage.Width * (rt.Height / LogoImage.Height))));
                lsz.Add(new SizeInfo(DvSizeMode.Pixel, 10F));
            }
            #endregion
            #region Title
            if (!string.IsNullOrWhiteSpace(Title))
            {
                var sz = Util.MeasureText(Title, TitleFontName, TitleFontSize, DvFontStyle.Normal);
                lsz.Add(new SizeInfo(DvSizeMode.Pixel, sz.Width + 2F));
                lsz.Add(new SizeInfo(DvSizeMode.Pixel, 10F));
            }
            #endregion
            #region Menus
            {
                lsz.Add(new SizeInfo(DvSizeMode.Pixel, rt.Height * 1.5F));
                lsz.Add(new SizeInfo(DvSizeMode.Percent, 100F));
                lsz.Add(new SizeInfo(DvSizeMode.Pixel, rt.Height * 1.5F));
            }
            #endregion
            #region Folding
            if (FoldingMenus.Count > 0)
            {
                lsz.Add(new SizeInfo(DvSizeMode.Pixel, 10F));
                lsz.Add(new SizeInfo(DvSizeMode.Pixel, rt.Height));
            }
            #endregion
            var rts = Util.DevideSizeH(rt, lsz);
            #endregion

            #region bounds
            var idx = 0;
            if (LogoImage != null) { rtLogo = rts[idx]; idx += 2; }
            if (!string.IsNullOrWhiteSpace(Title)) { rtTitle = rts[idx]; idx += 2; }
            rtMenuPrev = rts[idx]; idx++;
            rtMenus = rts[idx]; idx++;
            rtMenuNext = rts[idx]; idx++;
            if (FoldingMenus.Count > 0)
            {
                idx++;
                rtFoldingMenus = rts[idx];
            }
            #endregion

            var mls = items(rtMenus);
            act(rtContent, rtLogo, rtTitle, rtMenus, rtMenuPrev, rtMenuNext, rtFoldingMenus, mls);
        }
        #endregion

        #region items
        List<NavItem> items(SKRect rtMenus)
        {
            var x = 0F;
            var ls = new List<NavItem>();

            foreach (var pn in PageNames)
            {
                if (Design.Pages.ContainsKey(pn))
                {
                    var page = Design.Pages[pn];
                    var ti = new DvTextIcon
                    {
                        Text = page.Text,
                        FontName = FontName,
                        FontSize = FontSize,
                        FontStyle = FontStyle,
                        IconGap = IconGap,
                        IconString = page.IconString,
                        IconSize = IconSize,
                        IconAlignment = IconAlignment,
                    };
                    var sz = Util.MeasureTextIcon(ti);
                    var rt = Util.FromRect(x, rtMenus.Top, sz.Width, rtMenus.Height);
                    x += sz.Width + 15F;
                    var sx = Design.Pages.Values.LastOrDefault()?.Name != pn ? x : (float?)null;
                    ls.Add(new NavItem(page, rt, ti, sx));
                    x += 15;
                }
            }

            return ls;
        }
        #endregion
        #endregion
    }

    #region class : NavItem
    public class NavItem
    {
        public DvPage Page { get; private set; }
        public SKRect Bounds { get; private set; }
        public DvTextIcon TextIcon { get; private set; }
        public float? SepX { get; private set; }

        public NavItem(DvPage Page, SKRect Bounds, DvTextIcon TextIcon, float? SepX)
        {
            this.Page = Page;
            this.Bounds = Bounds;
            this.TextIcon = TextIcon;
            this.SepX = SepX;
        }
    }
    #endregion
    #region class : DropDownWindow
    internal class FoldingDropDownWindow : DvDropDownWindow
    {
        #region Properties
        public bool Reverse { get; set; }
        #endregion

        #region Member Variable
        DvListBox list;
        Action<DvTextIcon> result;
        int itemHeight = 30;
        SKColor? boxColor = null;
        SKColor? foreColor = null;
        #endregion

        #region Constructor
        public FoldingDropDownWindow()
        {
            list = new DvListBox
            {
                Name = nameof(list),
                BackgroundDraw = false,
                Fill = true,
                Margin = new Padding(10),
                SelectionMode = ItemSelectionMode.NONE,
            };

            list.ItemClicked += (o, s) =>
            {
                if (result != null) result(s.Item.Tag as DvTextIcon);
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
                var BoxColor = boxColor ?? thm.LabelColor;
                var ForeColor = foreColor ?? thm.ForeColor;
                var BorderColor = Util.FromArgb(120, ForeColor);

                var rt = Util.FromRect(X, Y, Width, Height);
                thm.DrawBox(Canvas, rt, BoxColor, BorderColor, DvRoundType.All, BoxStyle.Fill | BoxStyle.Border, thm.Corner);
            }

            base.OnDraw(Canvas);
        }
        #endregion
        #endregion

        #region Method
        #region ShowDropDown
        public void ShowDropDown(SKRect sbounds, SKRect ebounds,
            List<DvTextIcon> menus, int itemHeight, int viewCount, SKColor boxColor, SKColor foreColor,
            Action<DvTextIcon> result)
        {
            this.result = result;

            this.itemHeight = itemHeight;
            this.boxColor = boxColor;
            this.foreColor = foreColor;

            list.SelectionMode = ItemSelectionMode.NONE;
            list.Items.Clear();
            list.Items.AddRange(menus.Select(x => new ListBoxItem { Text = x.Text, IconString = x.IconString, Tag = x }));
            list.Round = DvRoundType.All;
            list.BoxColor = boxColor;
            list.ItemHeight = itemHeight;
            list.DrawScroll = viewCount < menus.Count;
            
            this.Show(sbounds, ebounds);
        }
        #endregion
        #endregion
    }
    #endregion
}

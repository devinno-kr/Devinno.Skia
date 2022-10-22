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
    public class DvComboBox : DvControl
    {
        #region Properties
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? ItemColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? SelectedItemColor { get; set; } = null;

        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;

        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        public List<TextIconItem> Items { get; } = new List<TextIconItem>();

        public int ButtonWidth { get; set; } = 40;
        public int MaximumViewCount { get; set; } = 8;
        public int ItemHeight { get; set; } = 30;
        public int SelectedIndex { get; set; } = -1;

        public Padding ItemPadding { get; set; } = new Padding(0, 0, 0, 0);
        public DvContentAlignment ContentAlignment { get; set; } = DvContentAlignment.MiddleCenter;
        #endregion

        #region Drop
        bool bOpen => Design.VisibleDropDownWindow(ddwnd);
        DropDownWindow ddwnd = new DropDownWindow();
        #endregion

        #region Event
        public event EventHandler SelectedIndexChanged;
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtBox, rtIco, rtText) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    var BoxColor = this.BoxColor ?? thm.ButtonColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var ItemColor = this.ItemColor ?? thm.ButtonColor;
                    var SelectedItemColor = this.SelectedItemColor ?? thm.PointColor;
                    var BorderColor = thm.GetBorderColor(BoxColor, ParentContainer.GetBackColor());

                    var Corner = thm.Corner;

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        var rt = rtContent;

                        thm.DrawBox(Canvas, rtContent, BoxColor, BorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.Fill | BoxStyle.InBevel | BoxStyle.OutShadow);

                        if(SelectedIndex >=0 && SelectedIndex < Items.Count)
                        {
                            var v = Items[SelectedIndex];
                            Util.DrawTextIcon(Canvas, v.Text, FontName, FontSize, DvFontStyle.Normal, IconGap, v.IconString, IconSize, ForeColor, rtText, IconAlignment, ContentAlignment);
                        }

                        #region Icon
                        var nisz = Convert.ToInt32(rtIco.Height / 2);
                        Util.DrawIconFA(Canvas, "fa-chevron-down", nisz, ForeColor, rtIco);
                        #endregion
                        #region Unit Sep
                        {
                            var szh = Convert.ToInt32(rtIco.Height / 2);

                            p.StrokeWidth = 1;

                            p.Color = BoxColor.BrightnessTransmit(thm.BorderBrightness);
                            Canvas.DrawLine(rtIco.Left + 0.5F, (rtContent.Top + (rtContent.Height / 2)) - (szh / 2) + 1, rtIco.Left + 0.5F, (rtContent.Top + (rtContent.Height / 2)) + (szh / 2) + 1, p);

                            p.Color = thm.GetInBevelColor(BoxColor);
                            Canvas.DrawLine(rtIco.Left + 1.5F, (rtContent.Top + (rtContent.Height / 2)) - (szh / 2) + 1, rtIco.Left + 1.5F, (rtContent.Top + (rtContent.Height / 2)) + (szh / 2) + 1, p);
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
            bounds((rtContent, rtBox, rtIco, rtText) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    if (Items.Count > 0)
                    {
                        var rt = rtContent;

                        var sx = ScreenX;
                        var sy = ScreenY;
                        var sw = rt.Width + 3;
                        var sh = rt.Height + 3;

                        var vh = Math.Max(Math.Min(Items.Count, MaximumViewCount) * 30, sh);

                        var rts = Util.FromRect(sx, sy, sw, sh);
                        var rte = Util.FromRect(sx, sy, sw, vh);

                        if (rte.Bottom > Design.Height) rte = Util.FromRect(sx, sy + sh - vh - 3, sw, vh);

                        ddwnd.ItemHeight = ItemHeight;
                        ddwnd.RowColor = this.BoxColor ?? thm.RowColor;
                        ddwnd.ShowDropDown(rts, rte, Items, SelectedIndex, (result) => SelectedIndex = result);
                    }
                }
            });

            base.OnMouseDown(x, y);
        }
        #endregion

        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            base.OnMouseUp(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);
            var rtIco = Util.FromRect(rtContent.Right - ButtonWidth, rtContent.Top, ButtonWidth, rtContent.Height);
            var rtBox = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width - rtIco.Width, rtContent.Height);
            var rtText = Util.FromRect(rtBox.Left + ItemPadding.Left, rtBox.Top + ItemPadding.Top, rtBox.Width - (ItemPadding.Left + ItemPadding.Right), rtBox.Height - (ItemPadding.Top + ItemPadding.Bottom));

            act(rtContent, rtBox, rtIco, rtText);
        }
        #endregion
        #endregion

        #region class : DropDownWindow
        class DropDownWindow : DvDropDownWindow
        {
            #region Properties
            public SKColor? RowColor
            {
                get => list.RowColor;
                set => list.RowColor = value;
            }

            public int ItemHeight
            {
                get => list.ItemHeight;
                set => list.ItemHeight = value;
            }
            #endregion
            #region Member Variable
            DvListBox list;
            Action<int> result;
            #endregion
            #region Constructor
            public DropDownWindow()
            {
                list = new DvListBox
                {
                    Name = nameof(list),
                    BackgroundDraw = true,
                    Fill = true,
                    Margin = new Padding(0),
                    SelectionMode = ItemSelectionMode.SINGLE,
                };
                list.SelectedChanged += (o, s) =>
                {
                    var sitm = list.SelectedItems.FirstOrDefault();
                    if (result != null && sitm != null) result(list.Items.IndexOf(sitm));
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
                }
                base.OnDraw(Canvas);
            }
            #endregion
            #endregion
            #region Method
            #region ShowDropDown
            public void ShowDropDown(SKRect sbounds, SKRect ebounds, List<TextIconItem> items, int SelectedIndex, Action<int> result)
            {
                this.result = result;
                list.Items.Clear();
                list.Items.AddRange(items);

                list.SelectedItems.Clear();
                if (SelectedIndex >= 0 && SelectedIndex < items.Count) list.SelectedItems.Add(items[SelectedIndex]);


                list.ScrollPosition = (SelectedIndex) * ItemHeight;


                this.Show(sbounds, ebounds);
            }
            #endregion
            #endregion
        }
        #endregion
    }
}

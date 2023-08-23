using Devinno.Skia.Collections;
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
        #region Color
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? SelectedColor { get; set; } = null;
        #endregion
        #region Font
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;

        public float IconSize { get; set; } = 12;
        public float IconGap { get; set; } = 5;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        #endregion
        #region Items
        public List<ComboBoxItem> Items { get; } = new List<ComboBoxItem>();

        public int SelectedIndex { get; set; } = -1;
        #endregion
        #region Shape
        public int ButtonWidth { get; set; } = 40;
        public int MaximumViewCount { get; set; } = 8;
        public int ItemHeight { get; set; } = 30;

        //public DvContentAlignment ContentAlignment { get; set; } = DvContentAlignment.MiddleCenter;
        #endregion
        #endregion

        #region Member Variable
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
            Areas((rtContent, rtIco, rtText) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region var
                    var BoxColor = this.BoxColor ?? thm.ButtonColor;
                    var ForeColor = this.ForeColor ?? thm.ForeColor;
                    var BackColor = ParentContainer.GetBackColor();
                    var SelectedColor = this.SelectedColor ?? thm.PointColor;
                    var ListBackColor = thm.ListBackColor;
                    #endregion

                    thm.DrawComboBox(Canvas, 
                        rtContent, rtIco, rtText,
                        BoxColor, ForeColor,BackColor, SelectedColor, ListBackColor,
                        FontName, FontSize, FontStyle, IconSize, IconAlignment, IconGap,
                        Items, SelectedIndex,
                        bOpen, ddwnd.Reverse, true);
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion

        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            Areas((rtContent, rtIco, rtText) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    if (Items.Count > 0)
                    {
                        var rt = rtContent;

                        var sx = ScreenX;
                        var sy = ScreenY - 1;
                        var sw = rt.Width;
                        var sh = rt.Height;

                        var vh = Math.Max(Math.Min(Items.Count, MaximumViewCount) * 30, sh) + 2;

                        var rts = Util.FromRect(sx, sy + Height, sw, sh);
                        var rte = Util.FromRect(sx, sy + Height, sw, vh);

                        if (rte.Bottom > Design.Height)  rte = Util.FromRect(sx, sy + sh - vh, sw, vh);
                        
                        ddwnd.ShowDropDown(rts, rte, Items, ItemHeight, SelectedIndex, 
                                            BoxColor ?? thm.ButtonColor, rte.Bottom > Design.Height,
                                            (result) => SelectedIndex = result);
                    }
                }
            });

            base.OnMouseDown(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);
            var rtIco = Util.FromRect(rtContent.Right - ButtonWidth, rtContent.Top, ButtonWidth, rtContent.Height);
            var rtText = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width - rtIco.Width, rtContent.Height);

            act(rtContent, rtIco, rtText);
        }
        #endregion
        #endregion
    }

    #region class : DropDownWindow
    internal class DropDownWindow : DvDropDownWindow
    {
        #region Properties
        public bool Reverse { get; set; }
        #endregion

        #region Member Variable
        DvListBox list;
        Action<int> result;
        int itemHeight;
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

        #region Method
        #region ShowDropDown
        public void ShowDropDown(SKRect sbounds, SKRect ebounds,
            List<ComboBoxItem> items, int itemHeight, int selectedIndex, SKColor boxColor, bool reverse,
            Action<int> result)
        {
            this.result = result;
            this.itemHeight = itemHeight;
            this.Reverse = reverse;

            list.Items.Clear();
            list.Items.AddRange(items.Select(x => new ListBoxItem { Text = x.Text, IconString = x.IconString, Tag = x }));

            list.SelectedItems.Clear();
            if (selectedIndex >= 0 && selectedIndex < items.Count) list.SelectedItems.Add(list.Items[selectedIndex]);

            list.Round = reverse ? DvRoundType.T : DvRoundType.B;
            list.BoxColor = boxColor;
            list.ScrollPosition = selectedIndex * itemHeight;

            this.Show(sbounds, ebounds);
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : ComboBoxItem
    public class ComboBoxItem
    {
        #region Properties
        public DvComboBox ComboBox { get; internal set; }

        public float Top { get; set; }
        public float Bottom { get; set; }
        public string Text { get; set; }
        public string IconString { get; set; }
        public object Tag { get; set; }
        public object Value { get; set; }
        #endregion

        #region Method
        protected virtual void Draw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            var cb = ComboBox;
            if (cb != null)
            {
                #region Item
                #region var
                var foreColor = cb.ForeColor ?? thm.ForeColor;
                var selectedColor = cb.SelectedColor ?? thm.PointColor;
                #endregion
                using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                {
                    #region Selected
                    if (cb.SelectedIndex >= 0 && cb.SelectedIndex < cb.Items.Count && cb.Items[cb.SelectedIndex] == this)
                    {
                        var rtv = Util.FromRect(bounds); rtv.Inflate(0, 0.5F);
                        p.Color = selectedColor;
                        p.IsStroke = false;
                        canvas.DrawRect(rtv, p);
                    }
                    #endregion
                    #region Text
                    Util.DrawTextIcon(canvas, IconString, cb.IconSize, DvTextIconAlignment.LeftRight, cb.IconGap,
                                               Text, cb.FontName, cb.FontSize, cb.FontStyle,
                                               Padding.Zero, foreColor, foreColor, bounds, DvContentAlignment.MiddleCenter);
                    #endregion
                }
                #endregion
            }
        }
        protected virtual void MouseDown(SKRect bounds, float x, float y) { }
        protected virtual void MouseMove(SKRect bounds, float x, float y) { }
        protected virtual void MouseUp(SKRect bounds, float x, float y) { }


        internal void _Draw(SKCanvas canvas, DvTheme thm, SKRect bounds) => Draw(canvas, thm, bounds);
        internal void _MouseDown(SKRect bounds, float x, float y) => MouseDown(bounds, x, y);
        internal void _MouseMove(SKRect bounds, float x, float y) => MouseMove(bounds, x, y);
        internal void _MouseUp(SKRect bounds, float x, float y) => MouseUp(bounds, x, y);
        #endregion
    }
    #endregion
}

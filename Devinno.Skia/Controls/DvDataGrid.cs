using Devinno.Skia.Containers;
using Devinno.Skia.Design;
using Devinno.Skia.Dialogs;
using Devinno.Skia.Extensions;
using Devinno.Skia.Theme;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvDataGrid : DvControl
    {
        #region Const 
        internal const int SPECIAL_CELL_WIDTH = 30;
        internal const int SELECTOR_BOX_WIDTH = 18;
        #endregion

        #region Properties
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? ColumnColor { get; set; } = null;
        public SKColor? RowColor { get; set; } = null;
        public SKColor? SelectedRowColor { get; set; } = null;
        public SKColor? SummaryRowColor { get; set; } = null;

        public int RowHeight { get; set; } = 30;
        public int ColumnHeight { get; set; } = 30;

        #region public List<IDvDataGridColumn> ColumnGroups { get; }
        [Browsable(false)]
        public List<IDvDataGridColumn> ColumnGroups { get; private set; } = new List<IDvDataGridColumn>();
        #endregion
        #region public List<IDvDataGridColumn> Columns { get; }
        [Browsable(false)]
        public List<IDvDataGridColumn> Columns { get; private set; } = new List<IDvDataGridColumn>();
        #endregion
        #region public List<DvDataGridRow> Rows { get; }
        [Browsable(false)]
        public List<DvDataGridRow> Rows { get; } = new List<DvDataGridRow>();
        #endregion
        #region public List<DvDataGridSummaryRow> SummaryRows { get; }
        [Browsable(false)]
        public List<DvDataGridSummaryRow> SummaryRows { get; } = new List<DvDataGridSummaryRow>();
        #endregion

        public DvDataGridSelectionMode SelectionMode { get; set; } = DvDataGridSelectionMode.SINGLE;
        
        public ScrollMode ScrollMode { get; set; } = ScrollMode.Vertical;
        #region public bool TouchMode { get; set; }
        public bool TouchMode
        {
            get => vscroll.TouchMode;
            set => vscroll.TouchMode = hscroll.TouchMode = value;
        }
        #endregion

        public double VScrollPosition { get => vscroll.ScrollPosition; set => vscroll.ScrollPosition = value; }
        public double HScrollPosition { get => hscroll.ScrollPosition; set => hscroll.ScrollPosition = value; }

        public bool Bevel { get; set; } = true;

        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;

        internal DvInputBox InputBox { get; } = new DvInputBox() { };
        internal DvDateTimePickerBox DateTimePicker { get; } = new DvDateTimePickerBox();
        internal DvColorPickerBox ColorPicker { get; } = new DvColorPickerBox();

        internal Type DataType { get; private set; } = null;
        #endregion

        #region Member Variable
        List<DvDataGridRow> mrows = new List<DvDataGridRow>();
        Scroll vscroll = new Scroll() { Direction = ScrollDirection.Vertical, Cut = true, TouchMode = true };
        Scroll hscroll = new Scroll() { Direction = ScrollDirection.Horizon, Cut = true, TouchMode = true };

        bool bNotRaiseEvent = false;
        object objs = null;
        int PrevTotalHeight = -1;
        List<_DGSearch_> lsp = new List<_DGSearch_>();

        internal bool bModSize = false;

        double hST = 0, vST = 0;
        double hSV = 0, vSV = 0;
        #endregion

        #region Event
        public event EventHandler SelectedChanged;
        public event EventHandler SortChanged;

        public event EventHandler<ColumnMouseEventArgs> ColumnMouseDown;
        public event EventHandler<ColumnMouseEventArgs> ColumnMouseUp;
        public event EventHandler<CellMouseEventArgs> CellMouseDown;
        public event EventHandler<CellMouseEventArgs> CellMouseUp;
        public event EventHandler<CellMouseEventArgs> CellMouseDoubleClick;
        public event EventHandler<CellValueChangedEventArgs> ValueChanged;
        public event EventHandler<CellButtonClickEventArgs> CellButtonClick;

        public event EventHandler AutoSetChanged;
        #endregion

        #region Constructor
        public DvDataGrid()
        {
            hscroll.GetScrollTotal = () => hST;
            hscroll.GetScrollTick = () => 1;
            hscroll.GetScrollView = () => hSV;
         
            vscroll.GetScrollTotal = () => vST;
            vscroll.GetScrollTick = () => RowHeight;
            vscroll.GetScrollView = () => vSV;
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtColumn, rtBox, rtSummary, rtScrollContent, rtScrollV, rtScrollH, rtScrollR) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region Color
                    var ForeColor = GetForeColor(thm);
                    var BoxColor = GetBoxColor(thm);
                    var ColumnColor = GetColumnColor(thm);
                    var RowColor = GetRowColor(thm);
                    var SelectRowColor = GetSelectedRowColor(thm);
                    var SummaryRowColor = GetSummaryRowColor(thm);
                    var BorderColor = thm.GetBorderColor(BoxColor, ParentContainer.GetBackColor());
                    #endregion
                    #region Set
                    var Corner = thm.Corner;

                    var rtColumnV = Util.FromRect(rtColumn.Left, rtColumn.Top, rtScrollContent.Width, rtColumn.Height);

                    var ush = ScrollMode == ScrollMode.Both || ScrollMode == ScrollMode.Horizon;
                    var usv = ScrollMode == ScrollMode.Both || ScrollMode == ScrollMode.Vertical;
                    var useFilter = Columns.Where(x => x.UseFilter).Count() > 0;
                    var colrc = GetColumnRowCount() + (useFilter ? 1 : 0);
                    var ColWidths = GetColumnsWidths(rtScrollContent);
                    var rts = GetColumnBounds(rtColumn, rtScrollContent);
                    var spw = Convert.ToInt32(SPECIAL_CELL_WIDTH);
                    var sbw = Convert.ToInt32(SELECTOR_BOX_WIDTH);
                    var vspos = Convert.ToInt32(vscroll.ScrollPositionWithOffset);
                    var hspos = Convert.ToInt32(hscroll.ScrollPositionWithOffset);

                    hST = Columns.Where(x => !x.Fixed).Select(x => ColWidths[Columns.IndexOf(x)]).Sum();
                    hSV = rtScrollContent.Width - Columns.Where(x => x.Fixed).Select(x => ColWidths[Columns.IndexOf(x)]).Sum();
                    vSV = rtScrollContent.Height;

                    var bAllSelect = GetRows().Where(x => x.Selected).Count() > 0;
                    var bUseFilter = Columns.Where(x => x.UseFilter).Count() > 0;
                    #endregion
                    #region Draw
                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        var sp = Canvas.SaveLayer();
                        var rtc = new SKRoundRect(rtContent, Corner);
                        Canvas.ClipRoundRect(rtc);

                        #region Column Index 
                        var rtnm = "rtColumn";
                        var lsnf = Columns.Where(x => !x.Fixed).ToList(); var vsnf = lsnf.FirstOrDefault(); var venf = lsnf.LastOrDefault(); var mrtNF = (SKRect?)null;
                        int? isnf = null, ienf = null;
                        if (vsnf != null && venf != null && rts.ContainsKey(rtnm + vsnf.Name) && rts.ContainsKey(rtnm + venf.Name))
                        {
                            var rtsv = rts[rtnm + vsnf.Name];
                            var rtev = rts[rtnm + venf.Name];
                            var mrt = Util.FromRect(rtsv.Left, rtColumn.Top, Math.Min(rtColumnV.Right, rtev.Right) - rtsv.Left, rtColumn.Height);

                            var vls = lsnf.Where(x => CollisionTool.Check(mrt, (Util.FromRect(rts[rtnm + x.Name].Left + hspos, rts[rtnm + x.Name].Top, rts[rtnm + x.Name].Width, rts[rtnm + x.Name].Height)))).ToList();
                            isnf = Columns.IndexOf(vls.FirstOrDefault());
                            ienf = Columns.IndexOf(vls.LastOrDefault());
                            mrtNF = mrt;
                        }

                        var lsf = Columns.Where(x => x.Fixed).ToList(); var vsf = lsf.FirstOrDefault(); var vef = lsf.LastOrDefault(); var mrtF = (SKRect?)null;
                        int? isf = null, ief = null;
                        if (vsf != null && vef != null && rts.ContainsKey(rtnm + vsf.Name) && rts.ContainsKey(rtnm + vef.Name))
                        {
                            var rtsv = rts[rtnm + vsf.Name];
                            var rtev = rts[rtnm + vef.Name];
                            var mrt = Util.FromRect(rtsv.Left, rtColumn.Top, Math.Min(rtColumnV.Right, rtev.Right) - rtsv.Left + 1, rtColumn.Height);

                            var vls = lsf.Where(x => CollisionTool.Check(mrt, (Util.FromRect(rts[rtnm + x.Name].Left, rts[rtnm + x.Name].Top, rts[rtnm + x.Name].Width, rts[rtnm + x.Name].Height)))).ToList();
                            isf = Columns.IndexOf(vls.FirstOrDefault());
                            ief = Columns.IndexOf(vls.LastOrDefault());
                            mrtF = mrt;
                        }

                        rtnm = "rtColumnGroup";
                        var lsgnf = ColumnGroups.Where(x => !x.Fixed).ToList(); var vsgnf = lsgnf.FirstOrDefault(); var vegnf = lsgnf.LastOrDefault(); var mrtGNF = (SKRect?)null;
                        int? isgnf = null, iegnf = null;
                        if (vsgnf != null && vegnf != null && rts.ContainsKey(rtnm + vsgnf.Name) && rts.ContainsKey(rtnm + vegnf.Name))
                        {
                            var rtsv = rts[rtnm + vsgnf.Name];
                            var rtev = rts[rtnm + vegnf.Name];
                            var mrt = Util.FromRect(rtsv.Left, rtsv.Top, Math.Min(rtColumnV.Right, rtev.Right) - rtsv.Left, rtsv.Height);

                            var vls = lsgnf.Where(x => CollisionTool.Check(mrt, (Util.FromRect(rts[rtnm + x.Name].Left + hspos, rts[rtnm + x.Name].Top, rts[rtnm + x.Name].Width, rts[rtnm + x.Name].Height)))).ToList();
                            isgnf = ColumnGroups.IndexOf(vls.FirstOrDefault());
                            iegnf = ColumnGroups.IndexOf(vls.LastOrDefault());
                            mrtGNF = mrt;
                        }

                        var lsgf = ColumnGroups.Where(x => x.Fixed).ToList(); var vsgf = lsgf.FirstOrDefault(); var vegf = lsgf.LastOrDefault(); var mrtGF = (SKRect?)null;
                        int? isgf = null, iegf = null;
                        if (vsgf != null && vegf != null && rts.ContainsKey(rtnm + vsgf.Name) && rts.ContainsKey(rtnm + vegf.Name))
                        {
                            var rtsv = rts[rtnm + vsgf.Name];
                            var rtev = rts[rtnm + vegf.Name];
                            var mrt = Util.FromRect(rtsv.Left, rtsv.Top, Math.Min(rtColumnV.Right, rtev.Right) - rtsv.Left + 1, rtColumn.Height);

                            var vls = lsgf.Where(x => CollisionTool.Check(mrt, (Util.FromRect(rts[rtnm + x.Name].Left, rts[rtnm + x.Name].Top, rts[rtnm + x.Name].Width, rts[rtnm + x.Name].Height)))).ToList();
                            isgf = ColumnGroups.IndexOf(vls.FirstOrDefault());
                            iegf = ColumnGroups.IndexOf(vls.LastOrDefault());
                            mrtGF = mrt;
                        }
                        #endregion
                        #region Content
                        {
                            #region Rows
                            {
                                //var sp2 = Canvas.SaveLayer();
                                //Canvas.ClipRect(rtScrollContent);

                                Loop((i, rtROW, v) =>
                                {
                                    #region Selector
                                    if (SelectionMode == DvDataGridSelectionMode.SELECTOR)
                                    {
                                        var rt = Util.INT(Util.FromRect(rtROW.Left, rtROW.Top, spw, rtROW.Height));

                                        #region Background
                                        #region Fill
                                        var bg = BoxColor;
                                        var c = v.Selected ? SelectRowColor : RowColor;
                                        p.Color = c;
                                        p.IsStroke = false;
                                        Canvas.DrawRect(rt, p);
                                        #endregion
                                        #region Bevel
                                        if(Bevel)
                                        {
                                            var n = 0.5F;
                                            var pts = new SKPoint[] {
                                            new SKPoint(rt.Right - n, rt.Top + 1 + n),
                                            new SKPoint(rt.Left + 1 + n, rt.Top + 1 + n),
                                            new SKPoint(rt.Left + 1 + n, rt.Bottom - n)
                                            };
                                            p.Color = thm.GetInBevelColor(c);
                                            p.IsStroke = true;
                                            p.StrokeWidth = 1;
                                            Canvas.DrawLine(pts[0], pts[1], p);
                                            Canvas.DrawLine(pts[1], pts[2], p);
                                        }
                                        #endregion
                                        #region Border
                                        {
                                            p.Color = BorderColor;
                                            p.IsStroke = true;
                                            p.StrokeWidth = 1;
                                            Canvas.DrawLine(rt.Left, rt.Top + 0.5F, rt.Right, rt.Top + 0.5F, p);
                                            Canvas.DrawLine(rt.Left + 0.5F, rt.Top, rt.Left + 0.5F, rt.Bottom, p);

                                        }
                                        #endregion
                                        #endregion
                                        #region CheckBox
                                        var rtSelectorBox = MathTool.MakeRectangle(rt, new SKSize(sbw, sbw));
                                        thm.DrawBox(Canvas, rtSelectorBox, c.BrightnessTransmit(thm.DataGridCheckBoxBright), BorderColor, RoundType.Rect, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutBevel | BoxStyle.InShadow);
                                        #endregion
                                        #region Check
                                        if (v.Selected)
                                        {
                                            var INF = sbw / 4;
                                            var rtCheck = Util.FromRect(rtSelectorBox.Left, rtSelectorBox.Top - 0, rtSelectorBox.Width, rtSelectorBox.Height); rtCheck.Inflate(-INF, -INF);
                                            rtCheck.Inflate(-1, -1);

                                            using (var fOut = SKImageFilter.CreateDropShadow(1, 1, 1, 1, Util.FromArgb(thm.OutShadowAlpha, SKColors.Black)))
                                            {
                                                p.ImageFilter = fOut;
                                                p.StrokeCap = SKStrokeCap.Round;
                                                p.StrokeJoin = SKStrokeJoin.Round;
                                                p.StrokeWidth = 4F;
                                                p.Color = ForeColor;
                                                p.IsStroke = true;

                                                var points = new SKPoint[] { new SKPoint(rtCheck.Left, rtCheck.MidY),
                                                             new SKPoint(rtCheck.MidX, rtCheck.Bottom),
                                                             new SKPoint(rtCheck.Right, rtCheck.Top)};

                                                var pth = new SKPath();
                                                pth.MoveTo(points[0]);
                                                pth.LineTo(points[1]);
                                                pth.LineTo(points[2]);

                                                Canvas.DrawPath(pth, p);
                                                p.ImageFilter = null;
                                            }
                                        }
                                        #endregion
                                    }
                                    #endregion
                                    #region !Fixed
                                    if (mrtNF.HasValue && isnf.HasValue && ienf.HasValue)
                                    {
                                        var vls = v.Cells.GetRange(isnf.Value, ienf.Value - isnf.Value + 1).ToList();
                                        var mrt = Util.FromRect(mrtNF.Value.Left, rtScrollContent.Top, mrtNF.Value.Width, rtScrollContent.Height);

                                        foreach (var cell in vls)
                                        {
                                            if (cell.Visible)
                                            {
                                                var rtCol = rts["rtColumn" + cell.Column.Name];
                                                var rt = Util.FromRect(rtCol.Left, rtROW.Top, rtCol.Width, rtROW.Height); rt.Offset(hspos, 0);

                                                if (cell.ColSpan > 1 && cell.ColumnIndex + cell.ColSpan <= ColWidths.Count) rt.Right = rt.Left + (int)ColWidths.GetRange(cell.ColumnIndex, cell.ColSpan).Sum();
                                                if (cell.RowSpan > 1 && cell.RowIndex + cell.RowSpan <= Rows.Count) rt.Bottom = rt.Top + Rows.GetRange(cell.RowIndex, cell.RowSpan).Sum(x => x.RowHeight);
                                                cell.Draw(thm, Canvas, rt);
                                            }
                                        }
                                    }
                                    #endregion
                                    #region Fixed
                                    if (mrtF.HasValue && isf.HasValue && ief.HasValue)
                                    {
                                        var vls = v.Cells.GetRange(isf.Value, ief.Value - isf.Value + 1).ToList();
                                        var mrt = Util.FromRect(mrtF.Value.Left, rtScrollContent.Top, mrtF.Value.Width + 1, rtScrollContent.Height);

                                        var last = vls.Where(x => x.Visible).LastOrDefault();
                                        foreach (var cell in vls)
                                        {
                                            if (cell.Visible)
                                            {
                                                var rtCol = rts["rtColumn" + cell.Column.Name];
                                                var rt = Util.FromRect(rtCol.Left, rtROW.Top, rtCol.Width, rtROW.Height);
                                                if (cell.ColSpan > 1 && cell.ColumnIndex + cell.ColSpan <= ColWidths.Count) rt.Right = rt.Left + (int)ColWidths.GetRange(cell.ColumnIndex, cell.ColSpan).Sum();
                                                if (cell.RowSpan > 1 && cell.RowIndex + cell.RowSpan <= Rows.Count) rt.Bottom = rt.Top + Rows.GetRange(cell.RowIndex, cell.RowSpan).Sum(x => x.RowHeight);
                                                cell.Draw(thm, Canvas, rt);

                                                if (last != null && cell == last)
                                                {
                                                    p.IsStroke = true;
                                                    p.StrokeWidth = 1F;
                                                    p.Color = BorderColor;

                                                    var l = rt.Left;
                                                    var r = rt.Right;
                                                    var b = rt.Bottom;
                                                    var t = rt.Top;
                                                    Canvas.DrawLine(r + 0.5F, t, r + 0.5F, b, p);
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                });

                                //Canvas.RestoreToCount(sp2);
                            }
                            #endregion
                            #region Summary 
                            if (SummaryRows.Count > 0)
                            {
                                //var sp2 = Canvas.SaveLayer();
                                //Canvas.ClipRect(rtSummary);

                                for (int i = 0; i < SummaryRows.Count; i++)
                                {
                                    var v = SummaryRows[i];
                                    var rtROW = Util.FromRect(rtSummary.Left, rtSummary.Top + (i * RowHeight), rtSummary.Width, RowHeight);
                                    #region Selector
                                    if (SelectionMode == DvDataGridSelectionMode.SELECTOR)
                                    {
                                        var rt = Util.INT(Util.FromRect(rtROW.Left, rtROW.Top, spw, rtROW.Height));

                                        #region Background
                                        #region Fill
                                        var bg = BoxColor;
                                        var c = SummaryRowColor;
                                        p.Color = c;
                                        p.IsStroke = false;
                                        Canvas.DrawRect(rt, p);
                                        #endregion
                                        #region Bevel
                                        if(Bevel)
                                        {
                                            var n = 0.5F;
                                            var pts = new SKPoint[] {
                                                new SKPoint(rt.Right - n, rt.Top + 1 + n),
                                                new SKPoint(rt.Left + 1 + n, rt.Top + 1 + n),
                                                new SKPoint(rt.Left + 1 + n, rt.Bottom - n)
                                            };
                                            p.Color = thm.GetInBevelColor(c);
                                            p.IsStroke = true;
                                            p.StrokeWidth = 1;
                                            Canvas.DrawLine(pts[0], pts[1], p);
                                            Canvas.DrawLine(pts[1], pts[2], p);
                                        }
                                        #endregion
                                        #region Border
                                        {
                                            p.Color = BorderColor;
                                            p.IsStroke = true;
                                            p.StrokeWidth = 1;
                                            Canvas.DrawLine(rt.Left, rt.Top + 0.5F, rt.Right, rt.Top + 0.5F, p);
                                            Canvas.DrawLine(rt.Left + 0.5F, rt.Top, rt.Left + 0.5F, rt.Bottom, p);
                                        }
                                        #endregion
                                        #endregion
                                    }
                                    #endregion
                                    #region !Fixed
                                    if (mrtNF.HasValue && isnf.HasValue && ienf.HasValue)
                                    {
                                        var vls = v.Cells.GetRange(isnf.Value, ienf.Value - isnf.Value + 1).ToList();
                                        var mrt = Util.FromRect(mrtNF.Value.Left, rtSummary.Top, mrtNF.Value.Width, rtSummary.Height);

                                        foreach (var cell in vls)
                                        {
                                            if (cell.Visible)
                                            {
                                                var rtCol = rts["rtColumn" + Columns[cell.ColumnIndex].Name];
                                                var rt = Util.FromRect(rtCol.Left, rtROW.Top, rtCol.Width, rtROW.Height); rt.Offset(hspos, 0);
                                                if (cell.ColumnSpan > 1 && cell.ColumnIndex + cell.ColumnSpan <= ColWidths.Count) rt.Right = rt.Left + (int)ColWidths.GetRange(cell.ColumnIndex, cell.ColumnSpan).Sum();
                                                cell.Draw(thm, Canvas, rt);
                                            }
                                        }
                                    }
                                    #endregion
                                    #region Fixed
                                    if (mrtF.HasValue && isf.HasValue && ief.HasValue)
                                    {
                                        var vls = v.Cells.GetRange(isf.Value, ief.Value - isf.Value + 1).ToList();
                                        var mrt = Util.FromRect(mrtF.Value.Left, rtSummary.Top, mrtF.Value.Width + 1, rtSummary.Height);

                                        var last = vls.Where(x => x.Visible).LastOrDefault();
                                        foreach (var cell in vls)
                                        {
                                            if (cell.Visible)
                                            {
                                                var rtCol = rts["rtColumn" + Columns[cell.ColumnIndex].Name];
                                                var rt = Util.FromRect(rtCol.Left, rtROW.Top, rtCol.Width, rtROW.Height);
                                                if (cell.ColumnSpan > 1 && cell.ColumnIndex + cell.ColumnSpan <= ColWidths.Count) rt.Right = rt.Left + (int)ColWidths.GetRange(cell.ColumnIndex, cell.ColumnSpan).Sum();
                                                cell.Draw(thm, Canvas, rt);

                                                if (last != null && cell == last)
                                                {
                                                    p.IsStroke = true;
                                                    p.StrokeWidth = 1F;
                                                    p.Color = BorderColor;

                                                    var l = rt.Left;
                                                    var r = rt.Right;
                                                    var b = rt.Bottom;
                                                    var t = rt.Top;
                                                    Canvas.DrawLine(r + 0.5F, t, r + 0.5F, b, p);
                                                }
                                            }
                                        }
                                    }
                                    #endregion
                                }

                                //Canvas.RestoreToCount(sp2);
                            }
                            #endregion
                        }
                        #endregion
                        #region Column
                        if (Columns.Count > 0)
                        {
                            //var sp2 = Canvas.SaveLayer();
                            //Canvas.ClipRect(rtColumnV);

                            #region Column
                            {
                                #region !Fixed
                                if (mrtNF.HasValue && isnf.HasValue && ienf.HasValue)
                                {
                                    var vls = Columns.GetRange(isnf.Value, ienf.Value - isnf.Value + 1).ToList();
                                    var mrt = Util.FromRect(mrtNF.Value.Left, rtColumn.Top, mrtNF.Value.Width, rtColumn.Height);

                                    foreach (var col in vls)
                                    {
                                        #region Column
                                        {
                                            var rt = (rts["rtColumn" + col.Name]); rt.Offset(hspos, 0);
                                            thm.DrawBox(Canvas, rt, ColumnColor, BorderColor, RoundType.Rect, BoxStyle.Fill);
                                            DrawColumnBorder(Canvas, thm, rtColumn, rtScrollContent, rt);
                                            col.Draw(thm, Canvas, rt);
                                        }
                                        #endregion
                                        #region Filter
                                        if (col.UseFilter)
                                        {
                                            var rt = (rts["rtFilter" + col.Name]); rt.Offset(hspos, 0);
                                            var n2 = Convert.ToInt32(0);
                                            var rtin = Util.FromRect(rt.Left, rt.Top, rt.Width, rt.Height); rtin.Inflate(-n2, -n2);
                                            var cbox = ColumnColor.BrightnessTransmit(thm.DataGridInputBright);
                                            var cbor = ColumnColor.BrightnessTransmit(thm.BorderBrightness);
                                            thm.DrawBox(Canvas, rtin, cbox, cbor, RoundType.Rect, BoxStyle.Fill);
                                            Util.DrawText(Canvas, col.FilterText, FontName, FontSize, DvFontStyle.Normal, ForeColor, rt);
                                            DrawColumnBorder(Canvas, thm, rtColumn, rtScrollContent, rt, false);
                                        }
                                        else
                                        {
                                            var rt = (rts["rtFilter" + col.Name]); rt.Offset(hspos, 0);
                                            if (bUseFilter)
                                            {
                                                thm.DrawBox(Canvas, rt, ColumnColor, BorderColor, RoundType.Rect, BoxStyle.Fill);

                                                p.IsStroke = true;
                                                p.StrokeWidth = 1F;
                                                p.Color = BorderColor;

                                                var b = rt.Bottom;
                                                var t = rt.Top;
                                                Canvas.DrawLine(rt.Left, t + 0.5F, rt.Right, t + 0.5F, p);
                                                Canvas.DrawLine(rt.Left, b + 0.5F, rt.Right, b + 0.5F, p);
                                            }
                                        }
                                        #endregion
                                    }
                                }
                                #endregion
                                #region Fixed
                                if (mrtF.HasValue && isf.HasValue && ief.HasValue)
                                {
                                    var vls = Columns.GetRange(isf.Value, ief.Value - isf.Value + 1).ToList();
                                    var mrt = Util.FromRect(mrtF.Value.Left, rtColumn.Top, mrtF.Value.Width, rtColumn.Height);

                                    var last = vls.LastOrDefault();
                                    foreach (var col in vls)
                                    {
                                        #region Column
                                        {
                                            var rt = (rts["rtColumn" + col.Name]);
                                            thm.DrawBox(Canvas, rt, ColumnColor, BorderColor, RoundType.Rect, BoxStyle.Fill);
                                            DrawColumnBorder(Canvas, thm, rtColumn, rtScrollContent, rt);
                                            col.Draw(thm, Canvas, rt);

                                            if (last != null && col == last)
                                            {
                                                p.IsStroke = true;
                                                p.StrokeWidth = 1F;
                                                p.Color = BorderColor;

                                                var l = rt.Left;
                                                var r = rt.Right;
                                                var b = rt.Bottom;
                                                var t = rt.Top;
                                                Canvas.DrawLine(r + 0.5F, t, r + 0.5F, b, p);
                                            }
                                        }
                                        #endregion
                                        #region Filter
                                        if (col.UseFilter)
                                        {
                                            var rt = (rts["rtFilter" + col.Name]);
                                            var n2 = Convert.ToInt32(0);
                                            var rtin = Util.FromRect(rt.Left, rt.Top + 1, rt.Width, rt.Height - 1); rtin.Inflate(-n2, -n2);
                                            var cbox = ColumnColor.BrightnessTransmit(thm.DataGridInputBright);
                                            var cbor = ColumnColor.BrightnessTransmit(thm.BorderBrightness);
                                            thm.DrawBox(Canvas, rtin, cbox, cbor, RoundType.Rect, BoxStyle.Fill);
                                            Util.DrawText(Canvas, col.FilterText, FontName, FontSize, DvFontStyle.Normal, ForeColor, rt);
                                            DrawColumnBorder(Canvas, thm, rtColumn, rtScrollContent, rt, false);
                                        }
                                        else
                                        {
                                            var rt = (rts["rtFilter" + col.Name]);
                                            if (bUseFilter)
                                            {
                                                thm.DrawBox(Canvas, rt, ColumnColor, BorderColor, RoundType.Rect, BoxStyle.Fill);

                                                p.IsStroke = true;
                                                p.StrokeWidth = 1F;
                                                p.Color = BorderColor;

                                                var b = rt.Bottom;
                                                var t = rt.Top;
                                                Canvas.DrawLine(rt.Left, t + 0.5F, rt.Right, t + 0.5F, p);
                                                Canvas.DrawLine(rt.Left, b + 0.5F, rt.Right, b + 0.5F, p);
                                            }
                                        }
                                        #endregion
                                    }
                                }
                                #endregion
                            }
                            #endregion
                            #region ColumnGroup
                            {
                                #region !Fixed
                                if (mrtGNF.HasValue && isgnf.HasValue && iegnf.HasValue)
                                {
                                    var vls = ColumnGroups.GetRange(isgnf.Value, iegnf.Value - isgnf.Value + 1).ToList();
                                    var mrt = Util.FromRect(mrtGNF.Value.Left, rtColumn.Top, mrtGNF.Value.Width, rtColumn.Height);

                                    foreach (var colgroup in vls)
                                    {
                                        var rt = Util.INT(rts["rtColumnGroup" + colgroup.Name]); rt.Offset(hspos, 0);
                                        thm.DrawBox(Canvas, rt, ColumnColor, BorderColor, RoundType.Rect, BoxStyle.Fill);
                                        DrawColumnBorder(Canvas, thm, rtColumn, rtScrollContent, rt);
                                        colgroup.Draw(thm, Canvas, rt);
                                    }
                                }
                                #endregion
                                #region Fixed
                                if (mrtGF.HasValue && isgf.HasValue && iegf.HasValue)
                                {
                                    var vls = ColumnGroups.GetRange(isgf.Value, iegf.Value - isgf.Value + 1).ToList();
                                    var mrt = Util.FromRect(mrtGF.Value.Left, rtColumn.Top, mrtGF.Value.Width, rtColumn.Height);

                                    var last = vls.LastOrDefault();
                                    foreach (var colgroup in vls)
                                    {
                                        var rt = Util.INT(rts["rtColumnGroup" + colgroup.Name]);
                                        thm.DrawBox(Canvas, rt, ColumnColor, BorderColor, RoundType.Rect, BoxStyle.Fill);
                                        DrawColumnBorder(Canvas, thm, rtColumn, rtScrollContent, rt);
                                        colgroup.Draw(thm, Canvas, rt);

                                        if (last != null && colgroup == last)
                                        {
                                            p.IsStroke = true;
                                            p.StrokeWidth = 1F;
                                            p.Color = BorderColor;

                                            var l = rt.Left;
                                            var r = rt.Right;
                                            var b = rt.Bottom;
                                            var t = rt.Top;
                                            Canvas.DrawLine(r + 0.5F, t, r + 0.5F, b, p);
                                        }
                                    }
                                }
                                #endregion
                            }
                            #endregion
                            #region ColumnSelector
                            if (SelectionMode == DvDataGridSelectionMode.SELECTOR)
                            {
                                var rtSelector = Util.FromRect(rtColumn.Left, rtColumn.Top, spw, rtColumn.Height);
                                thm.DrawBox(Canvas, rtSelector, ColumnColor, BorderColor, RoundType.Rect, BoxStyle.Fill);
                                DrawColumnBorder(Canvas, thm, rtColumn, rtScrollContent, rtSelector);

                                var rtSelectorBox = MathTool.MakeRectangle(Util.INT(rtSelector), new SKSize(sbw, sbw));
                                thm.DrawBox(Canvas, rtSelectorBox, ColumnColor.BrightnessTransmit(thm.DataGridCheckBoxBright), BorderColor, RoundType.Rect, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutBevel | BoxStyle.InShadow);

                                if (bAllSelect)
                                {
                                    var INF = sbw / 4;
                                    var rtCheck = Util.FromRect(rtSelectorBox.Left, rtSelectorBox.Top - 0, rtSelectorBox.Width, rtSelectorBox.Height); rtCheck.Inflate(-INF, -INF);
                                    rtCheck.Inflate(-1, -1);

                                    using (var fOut = SKImageFilter.CreateDropShadow(1, 1, 1, 1, Util.FromArgb(thm.OutShadowAlpha, SKColors.Black)))
                                    {
                                        p.ImageFilter = fOut;
                                        p.StrokeCap = SKStrokeCap.Round;
                                        p.StrokeJoin = SKStrokeJoin.Round;
                                        p.StrokeWidth = 4F;
                                        p.Color = ForeColor;
                                        p.IsStroke = true;

                                        var points = new SKPoint[] { new SKPoint(rtCheck.Left, rtCheck.MidY),
                                                             new SKPoint(rtCheck.MidX, rtCheck.Bottom),
                                                             new SKPoint(rtCheck.Right, rtCheck.Top)};

                                        var pth = new SKPath();
                                        pth.MoveTo(points[0]);
                                        pth.LineTo(points[1]);
                                        pth.LineTo(points[2]);

                                        Canvas.DrawPath(pth, p);
                                        p.ImageFilter = null;
                                    }
                                }
                            }
                            #endregion

                            //Canvas.RestoreToCount(sp2);

                            #region Column Scroll
                            {
                                var rt = new SKRect(rtScrollV.Left, rtColumn.Top, rtScrollV.Right, rtColumn.Bottom);
                                thm.DrawBox(Canvas, rt, ColumnColor, BorderColor, RoundType.Round_RT, BoxStyle.Fill);
                            }
                            #endregion
                            #region Column Border
                            p.StrokeWidth = 1;
                            p.Color = BorderColor;
                            Canvas.DrawLine(rtColumn.Left, rtColumn.Bottom + 0.5F, rtColumn.Right, rtColumn.Bottom + 0.5F, p);

                            if (rtColumn.Right != rtScrollContent.Right)
                            {
                                p.StrokeWidth = 1;
                                p.Color = BorderColor;
                                Canvas.DrawLine(rtScrollContent.Right - 0.5F, rtColumn.Top, rtScrollContent.Right - 0.5F, rtColumn.Bottom, p);
                            }

                            if (SelectionMode == DvDataGridSelectionMode.SELECTOR)
                            {
                                p.StrokeWidth = 1;
                                p.Color = BorderColor;
                                Canvas.DrawLine(rtScrollContent.Left + spw + 0.5F, rtColumn.Top, rtScrollContent.Left + spw + 0.5F, rtColumn.Bottom, p);
                            }
                            #endregion
                        }
                        #endregion
                        #region Scroll
                        switch (ScrollMode)
                        {
                            #region Horizon
                            case ScrollMode.Horizon:
                                {
                                    thm.DrawBox(Canvas, rtScrollH, thm.ScrollBarColor, BorderColor, RoundType.Round_B, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutShadow);

                                    var cCur = thm.ScrollCursorOffColor;
                                    if (hscroll.IsScrolling || hscroll.IsTouchMoving) cCur = thm.ScrollCursorOnColor;

                                    var rtcur = hscroll.GetScrollCursorRect(rtScrollH);
                                    if (rtcur.HasValue) thm.DrawBox(Canvas, rtcur.Value, cCur, BorderColor, RoundType.Round, BoxStyle.Fill);
                                }
                                break;
                            #endregion
                            #region Vertical
                            case ScrollMode.Vertical:
                                {
                                    thm.DrawBox(Canvas, rtScrollV, thm.ScrollBarColor, BorderColor, Columns.Count > 0 ? RoundType.Round_RB : RoundType.Round_R, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutShadow);

                                    var cCur = thm.ScrollCursorOffColor;
                                    if (vscroll.IsScrolling || vscroll.IsTouchMoving) cCur = thm.ScrollCursorOnColor;

                                    var rtcur = vscroll.GetScrollCursorRect(rtScrollV);
                                    if (rtcur.HasValue) thm.DrawBox(Canvas, rtcur.Value, cCur, BorderColor, RoundType.Round, BoxStyle.Fill);
                                }
                                break;
                            #endregion
                            #region Both
                            case ScrollMode.Both:
                                {
                                    thm.DrawBox(Canvas, rtScrollH, thm.ScrollBarColor, BorderColor, RoundType.Round_LB, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutShadow);
                                    thm.DrawBox(Canvas, rtScrollV, thm.ScrollBarColor, BorderColor, RoundType.Rect, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutShadow);
                                    thm.DrawBox(Canvas, rtScrollR, thm.ScrollBarColor, BorderColor, Columns.Count > 0 ? RoundType.Round_RB : RoundType.Round_R, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutShadow);

                                    var cCurH = thm.ScrollCursorOffColor;
                                    if (hscroll.IsScrolling || hscroll.IsTouchMoving) cCurH = thm.ScrollCursorOnColor;
                                    var rtcurH = hscroll.GetScrollCursorRect(rtScrollH);
                                    if (rtcurH.HasValue) thm.DrawBox(Canvas, rtcurH.Value, cCurH, BorderColor, RoundType.Round, BoxStyle.Fill);

                                    var cCurV = thm.ScrollCursorOffColor;
                                    if (vscroll.IsScrolling || vscroll.IsTouchMoving) cCurV = thm.ScrollCursorOnColor;
                                    var rtcurV = vscroll.GetScrollCursorRect(rtScrollV);
                                    if (rtcurV.HasValue) thm.DrawBox(Canvas, rtcurV.Value, cCurV, BorderColor, RoundType.Round, BoxStyle.Fill);
                                }
                                break;
                                #endregion
                        }
                        #endregion

                        Canvas.RestoreToCount(sp);
                    }

                    thm.DrawBox(Canvas, rtContent, BoxColor, BorderColor, RoundType.Round, BoxStyle.Border);
                    #endregion
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion

        #region OnMouseDown
        protected override void OnMouseDown(int x, int y)
        {
            bounds((rtContent, rtColumn, rtBox, rtSummary, rtScrollContent, rtScrollV, rtScrollH, rtScrollR) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region Bounds
                    var rts = GetColumnBounds(rtColumn, rtScrollContent);
                    var spw = Convert.ToInt32(SPECIAL_CELL_WIDTH);
                    var sbw = Convert.ToInt32(SELECTOR_BOX_WIDTH);

                    var rtColumnV = Util.FromRect(rtColumn.Left, rtColumn.Top, rtScrollContent.Width, rtColumn.Height);
                    var ColWidths = GetColumnsWidths(rtScrollContent);
                    var vspos = Convert.ToInt32(vscroll.ScrollPositionWithOffset);
                    var hspos = Convert.ToInt32(hscroll.ScrollPositionWithOffset);
                    #endregion
                    #region Column
                    foreach (var col in Columns)
                    {
                        var rt = rts["rtColumn" + col.Name];
                        col.MouseDown(rt, x, y);
                    }
                    #endregion
                    #region SelectorAll
                    if (SelectionMode == DvDataGridSelectionMode.SELECTOR)
                    {
                        var wh = Convert.ToInt32(SELECTOR_BOX_WIDTH);
                        var rtSelector = Util.FromRect(rtColumn.Left, rtColumn.Top, spw, rtColumn.Height);
                        var rtSelectorBox = MathTool.MakeRectangle(rtSelector, new SKSize(wh, wh));

                        if (CollisionTool.Check(rtSelectorBox, x, y))
                        {
                            var bAllSelect = GetRows().Where(x => x.Selected).Count() > 0;
                            foreach (var v in GetRows()) v.Selected = !bAllSelect;
                        }
                    }
                    #endregion
                    #region Rows
                    if (CollisionTool.Check(rtScrollContent, x, y))
                    {
                        #region Column Index 

                        var rtnm = "rtColumn";
                        var lsnf = Columns.Where(x => !x.Fixed).ToList(); var vsnf = lsnf.FirstOrDefault(); var venf = lsnf.LastOrDefault(); var mrtNF = (SKRect?)null;
                        int? isnf = null, ienf = null;
                        if (vsnf != null && venf != null && rts.ContainsKey(rtnm + vsnf.Name) && rts.ContainsKey(rtnm + venf.Name))
                        {
                            var rtsv = rts[rtnm + vsnf.Name];
                            var rtev = rts[rtnm + venf.Name];
                            var mrt = Util.FromRect(rtsv.Left, rtColumn.Top, Math.Min(rtColumnV.Right, rtev.Right) - rtsv.Left, rtColumn.Height);

                            var vls = lsnf.Where(x => CollisionTool.Check(mrt, Util.INT(Util.FromRect(rts[rtnm + x.Name].Left + hspos, rts[rtnm + x.Name].Top, rts[rtnm + x.Name].Width, rts[rtnm + x.Name].Height)))).ToList();
                            isnf = Columns.IndexOf(vls.FirstOrDefault());
                            ienf = Columns.IndexOf(vls.LastOrDefault());
                            mrtNF = mrt;
                        }

                        var lsf = Columns.Where(x => x.Fixed).ToList(); var vsf = lsf.FirstOrDefault(); var vef = lsf.LastOrDefault(); var mrtF = (SKRect?)null;
                        int? isf = null, ief = null;
                        if (vsf != null && vef != null && rts.ContainsKey(rtnm + vsf.Name) && rts.ContainsKey(rtnm + vef.Name))
                        {
                            var rtsv = rts[rtnm + vsf.Name];
                            var rtev = rts[rtnm + vef.Name];
                            var mrt = Util.FromRect(rtsv.Left, rtColumn.Top, Math.Min(rtColumnV.Right, rtev.Right) - rtsv.Left + 1, rtColumn.Height);

                            var vls = lsf.Where(x => CollisionTool.Check(mrt, Util.INT(Util.FromRect(rts[rtnm + x.Name].Left, rts[rtnm + x.Name].Top, rts[rtnm + x.Name].Width, rts[rtnm + x.Name].Height)))).ToList();
                            isf = Columns.IndexOf(vls.FirstOrDefault());
                            ief = Columns.IndexOf(vls.LastOrDefault());
                            mrtF = mrt;
                        }

                        rtnm = "rtColumnGroup";
                        var lsgnf = ColumnGroups.Where(x => !x.Fixed).ToList(); var vsgnf = lsgnf.FirstOrDefault(); var vegnf = lsgnf.LastOrDefault(); var mrtGNF = (SKRect?)null;
                        int? isgnf = null, iegnf = null;
                        if (vsgnf != null && vegnf != null && rts.ContainsKey(rtnm + vsgnf.Name) && rts.ContainsKey(rtnm + vegnf.Name))
                        {
                            var rtsv = rts[rtnm + vsgnf.Name];
                            var rtev = rts[rtnm + vegnf.Name];
                            var mrt = Util.FromRect(rtsv.Left, rtsv.Top, Math.Min(rtColumnV.Right, rtev.Right) - rtsv.Left, rtsv.Height);

                            var vls = lsgnf.Where(x => CollisionTool.Check(mrt, Util.INT(Util.FromRect(rts[rtnm + x.Name].Left + hspos, rts[rtnm + x.Name].Top, rts[rtnm + x.Name].Width, rts[rtnm + x.Name].Height)))).ToList();
                            isgnf = ColumnGroups.IndexOf(vls.FirstOrDefault());
                            iegnf = ColumnGroups.IndexOf(vls.LastOrDefault());
                            mrtGNF = mrt;
                        }

                        var lsgf = ColumnGroups.Where(x => x.Fixed).ToList(); var vsgf = lsgf.FirstOrDefault(); var vegf = lsgf.LastOrDefault(); var mrtGF = (SKRect?)null;
                        int? isgf = null, iegf = null;
                        if (vsgf != null && vegf != null && rts.ContainsKey(rtnm + vsgf.Name) && rts.ContainsKey(rtnm + vegf.Name))
                        {
                            var rtsv = rts[rtnm + vsgf.Name];
                            var rtev = rts[rtnm + vegf.Name];
                            var mrt = Util.FromRect(rtsv.Left, rtsv.Top, Math.Min(rtColumnV.Right, rtev.Right) - rtsv.Left + 1, rtColumn.Height);

                            var vls = lsgf.Where(x => CollisionTool.Check(mrt, Util.INT(Util.FromRect(rts[rtnm + x.Name].Left, rts[rtnm + x.Name].Top, rts[rtnm + x.Name].Width, rts[rtnm + x.Name].Height)))).ToList();
                            isgf = ColumnGroups.IndexOf(vls.FirstOrDefault());
                            iegf = ColumnGroups.IndexOf(vls.LastOrDefault());
                            mrtGF = mrt;
                        }
                        #endregion

                        Loop((i, rtROW, v) =>
                        {
                            #region Selector
                            if (SelectionMode == DvDataGridSelectionMode.SELECTOR)
                            {
                                var rt = Util.INT(Util.FromRect(rtROW.Left, rtROW.Top, spw, rtROW.Height));
                                var rtSelectorBox = MathTool.MakeRectangle(rt, new SKSize(sbw, sbw));
                                if (CollisionTool.Check(rtSelectorBox, x, y))
                                {
                                    v.Selected = !v.Selected;
                                }
                            }
                            #endregion
                            #region !Fixed
                            if (mrtNF.HasValue && isnf.HasValue && ienf.HasValue)
                            {
                                var vls = v.Cells.GetRange(isnf.Value, ienf.Value - isnf.Value + 1).ToList();
                                var mrt = Util.FromRect(mrtNF.Value.Left, rtScrollContent.Top, mrtNF.Value.Width, rtScrollContent.Height);
                                foreach (var cell in vls)
                                {
                                    if (cell.Visible)
                                    {
                                        var rtCol = rts["rtColumn" + cell.Column.Name];
                                        var rt = Util.FromRect(rtCol.Left, rtROW.Top, rtCol.Width, rtROW.Height); rt.Offset(hspos, 0);

                                        if (cell.ColSpan > 1 && cell.ColumnIndex + cell.ColSpan <= ColWidths.Count) rt.Right = rt.Left + (int)ColWidths.GetRange(cell.ColumnIndex, cell.ColSpan).Sum();
                                        if (cell.RowSpan > 1 && cell.RowIndex + cell.RowSpan <= Rows.Count) rt.Bottom = rt.Top + Rows.GetRange(cell.RowIndex, cell.RowSpan).Sum(x => x.RowHeight);
                                        cell.MouseDown(Util.INT(rt), x, y);
                                    }
                                }
                            }
                            #endregion
                            #region Fixed
                            if (mrtF.HasValue && isf.HasValue && ief.HasValue)
                            {
                                var vls = v.Cells.GetRange(isf.Value, ief.Value - isf.Value + 1).ToList();
                                var mrt = Util.FromRect(mrtF.Value.Left, rtScrollContent.Top, mrtF.Value.Width + 1, rtScrollContent.Height);
                                foreach (var cell in vls)
                                {
                                    if (cell.Visible)
                                    {
                                        var rtCol = rts["rtColumn" + cell.Column.Name];
                                        var rt = Util.FromRect(rtCol.Left, rtROW.Top, rtCol.Width, rtROW.Height);
                                        if (cell.ColSpan > 1 && cell.ColumnIndex + cell.ColSpan <= ColWidths.Count) rt.Right = rt.Left + (int)ColWidths.GetRange(cell.ColumnIndex, cell.ColSpan).Sum();
                                        if (cell.RowSpan > 1 && cell.RowIndex + cell.RowSpan <= Rows.Count) rt.Bottom = rt.Top + Rows.GetRange(cell.RowIndex, cell.RowSpan).Sum(x => x.RowHeight);
                                        cell.MouseDown(Util.INT(rt), x, y);
                                    }
                                }
                            }
                            #endregion
                        });
                    }
                    #endregion

                    #region Scroll / Touch
                    if (ScrollMode == ScrollMode.Vertical)
                    {
                        vscroll.MouseDown(x, y, rtScrollV);
                        if (vscroll.TouchMode && CollisionTool.Check(rtContent, x, y) && !CollisionTool.Check(rtScrollV, x, y)) vscroll.TouchDown(x, y);
                    }
                    else if (ScrollMode == ScrollMode.Horizon)
                    {
                        hscroll.MouseDown(x, y, rtScrollH);
                        if (hscroll.TouchMode && CollisionTool.Check(rtContent, x, y) && !CollisionTool.Check(rtScrollH, x, y)) hscroll.TouchDown(x, y);
                    }
                    else
                    {
                        vscroll.MouseDown(x, y, rtScrollV);
                        hscroll.MouseDown(x, y, rtScrollH);
                        if (hscroll.TouchMode && CollisionTool.Check(rtContent, x, y) && !CollisionTool.Check(rtScrollH, x, y)) hscroll.TouchDown(x, y);
                        if (vscroll.TouchMode && CollisionTool.Check(rtContent, x, y) && !CollisionTool.Check(rtScrollV, x, y)) vscroll.TouchDown(x, y);
                    }
                    #endregion
                }
            });
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(int x, int y)
        {
            bounds((rtContent, rtColumn, rtBox, rtSummary, rtScrollContent, rtScrollV, rtScrollH, rtScrollR) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region Bounds
                    var rts = GetColumnBounds(rtColumn, rtScrollContent);
                    var spw = Convert.ToInt32(SPECIAL_CELL_WIDTH);

                    var rtColumnV = Util.FromRect(rtColumn.Left, rtColumn.Top, rtScrollContent.Width, rtColumn.Height);
                    var ColWidths = GetColumnsWidths(rtScrollContent);
                    var vspos = Convert.ToInt32(vscroll.ScrollPositionWithOffset);
                    var hspos = Convert.ToInt32(hscroll.ScrollPositionWithOffset);
                    #endregion
                    #region Scroll / Touch
                    if (ScrollMode == ScrollMode.Vertical)
                    {
                        vscroll.MouseMove(x, y, rtScrollV);
                        if (vscroll.TouchMode) vscroll.TouchMove(x, y);
                    }
                    else if (ScrollMode == ScrollMode.Horizon)
                    {
                        hscroll.MouseMove(x, y, rtScrollH);
                        if (hscroll.TouchMode) hscroll.TouchMove(x, y);
                    }
                    else
                    {
                        vscroll.MouseMove(x, y, rtScrollV);
                        if (vscroll.TouchMode) vscroll.TouchMove(x, y);

                        hscroll.MouseMove(x, y, rtScrollH);
                        if (hscroll.TouchMode) hscroll.TouchMove(x, y);
                    }
                    #endregion
                }
            });
            base.OnMouseMove(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(int x, int y)
        {
            bounds((rtContent, rtColumn, rtBox, rtSummary, rtScrollContent, rtScrollV, rtScrollH, rtScrollR) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region Bounds
                    var rts = GetColumnBounds(rtColumn, rtScrollContent);
                    var spw = Convert.ToInt32(SPECIAL_CELL_WIDTH);
                    var sbw = Convert.ToInt32(SELECTOR_BOX_WIDTH);

                    var rtColumnV = Util.FromRect(rtColumn.Left, rtColumn.Top, rtScrollContent.Width, rtColumn.Height);
                    var ColWidths = GetColumnsWidths(rtScrollContent);
                    var vspos = Convert.ToInt32(vscroll.ScrollPositionWithOffset);
                    var hspos = Convert.ToInt32(hscroll.ScrollPositionWithOffset);
                    #endregion
                    #region Scroll / Touch
                    if (ScrollMode == ScrollMode.Vertical)
                    {
                        vscroll.MouseUp(x, y);
                        if (vscroll.TouchMode) vscroll.TouchUp(x, y);
                    }
                    else if (ScrollMode == ScrollMode.Horizon)
                    {
                        hscroll.MouseUp(x, y);
                        if (hscroll.TouchMode) hscroll.TouchUp(x, y);
                    }
                    else
                    {
                        vscroll.MouseUp(x, y);
                        if (vscroll.TouchMode) vscroll.TouchUp(x, y);

                        hscroll.MouseUp(x, y);
                        if (hscroll.TouchMode) hscroll.TouchUp(x, y);
                    }
                    #endregion
                    #region Rows
                    #region Column Index 

                    var rtnm = "rtColumn";
                    var lsnf = Columns.Where(x => !x.Fixed).ToList(); var vsnf = lsnf.FirstOrDefault(); var venf = lsnf.LastOrDefault(); var mrtNF = (SKRect?)null;
                    int? isnf = null, ienf = null;
                    if (vsnf != null && venf != null && rts.ContainsKey(rtnm + vsnf.Name) && rts.ContainsKey(rtnm + venf.Name))
                    {
                        var rtsv = rts[rtnm + vsnf.Name];
                        var rtev = rts[rtnm + venf.Name];
                        var mrt = Util.FromRect(rtsv.Left, rtColumn.Top, Math.Min(rtColumnV.Right, rtev.Right) - rtsv.Left, rtColumn.Height);

                        var vls = lsnf.Where(x => CollisionTool.Check(mrt, Util.INT(Util.FromRect(rts[rtnm + x.Name].Left + hspos, rts[rtnm + x.Name].Top, rts[rtnm + x.Name].Width, rts[rtnm + x.Name].Height)))).ToList();
                        isnf = Columns.IndexOf(vls.FirstOrDefault());
                        ienf = Columns.IndexOf(vls.LastOrDefault());
                        mrtNF = mrt;
                    }

                    var lsf = Columns.Where(x => x.Fixed).ToList(); var vsf = lsf.FirstOrDefault(); var vef = lsf.LastOrDefault(); var mrtF = (SKRect?)null;
                    int? isf = null, ief = null;
                    if (vsf != null && vef != null && rts.ContainsKey(rtnm + vsf.Name) && rts.ContainsKey(rtnm + vef.Name))
                    {
                        var rtsv = rts[rtnm + vsf.Name];
                        var rtev = rts[rtnm + vef.Name];
                        var mrt = Util.FromRect(rtsv.Left, rtColumn.Top, Math.Min(rtColumnV.Right, rtev.Right) - rtsv.Left + 1, rtColumn.Height);

                        var vls = lsf.Where(x => CollisionTool.Check(mrt, Util.INT(Util.FromRect(rts[rtnm + x.Name].Left, rts[rtnm + x.Name].Top, rts[rtnm + x.Name].Width, rts[rtnm + x.Name].Height)))).ToList();
                        isf = Columns.IndexOf(vls.FirstOrDefault());
                        ief = Columns.IndexOf(vls.LastOrDefault());
                        mrtF = mrt;
                    }

                    rtnm = "rtColumnGroup";
                    var lsgnf = ColumnGroups.Where(x => !x.Fixed).ToList(); var vsgnf = lsgnf.FirstOrDefault(); var vegnf = lsgnf.LastOrDefault(); var mrtGNF = (SKRect?)null;
                    int? isgnf = null, iegnf = null;
                    if (vsgnf != null && vegnf != null && rts.ContainsKey(rtnm + vsgnf.Name) && rts.ContainsKey(rtnm + vegnf.Name))
                    {
                        var rtsv = rts[rtnm + vsgnf.Name];
                        var rtev = rts[rtnm + vegnf.Name];
                        var mrt = Util.FromRect(rtsv.Left, rtsv.Top, Math.Min(rtColumnV.Right, rtev.Right) - rtsv.Left, rtsv.Height);

                        var vls = lsgnf.Where(x => CollisionTool.Check(mrt, Util.INT(Util.FromRect(rts[rtnm + x.Name].Left + hspos, rts[rtnm + x.Name].Top, rts[rtnm + x.Name].Width, rts[rtnm + x.Name].Height)))).ToList();
                        isgnf = ColumnGroups.IndexOf(vls.FirstOrDefault());
                        iegnf = ColumnGroups.IndexOf(vls.LastOrDefault());
                        mrtGNF = mrt;
                    }

                    var lsgf = ColumnGroups.Where(x => x.Fixed).ToList(); var vsgf = lsgf.FirstOrDefault(); var vegf = lsgf.LastOrDefault(); var mrtGF = (SKRect?)null;
                    int? isgf = null, iegf = null;
                    if (vsgf != null && vegf != null && rts.ContainsKey(rtnm + vsgf.Name) && rts.ContainsKey(rtnm + vegf.Name))
                    {
                        var rtsv = rts[rtnm + vsgf.Name];
                        var rtev = rts[rtnm + vegf.Name];
                        var mrt = Util.FromRect(rtsv.Left, rtsv.Top, Math.Min(rtColumnV.Right, rtev.Right) - rtsv.Left + 1, rtColumn.Height);

                        var vls = lsgf.Where(x => CollisionTool.Check(mrt, Util.INT(Util.FromRect(rts[rtnm + x.Name].Left, rts[rtnm + x.Name].Top, rts[rtnm + x.Name].Width, rts[rtnm + x.Name].Height)))).ToList();
                        isgf = ColumnGroups.IndexOf(vls.FirstOrDefault());
                        iegf = ColumnGroups.IndexOf(vls.LastOrDefault());
                        mrtGF = mrt;
                    }
                    #endregion

                    var SelectedRows = Rows.Where(x => x.Selected).ToList();
                    var bSelectionChange = false;

                    Loop((i, rtROW, v) =>
                    {
                        #region !Fixed
                        if (mrtNF.HasValue && isnf.HasValue && ienf.HasValue)
                        {
                            var vls = v.Cells.GetRange(isnf.Value, ienf.Value - isnf.Value + 1).ToList();
                            var mrt = Util.FromRect(mrtNF.Value.Left, rtScrollContent.Top, mrtNF.Value.Width, rtScrollContent.Height);
                            foreach (var cell in vls)
                            {
                                if (cell.Visible)
                                {
                                    var rtCol = rts["rtColumn" + cell.Column.Name];
                                    var rt = Util.FromRect(rtCol.Left, rtROW.Top, rtCol.Width, rtROW.Height); rt.Offset(hspos, 0);

                                    if (cell.ColSpan > 1 && cell.ColumnIndex + cell.ColSpan <= ColWidths.Count) rt.Right = rt.Left + (int)ColWidths.GetRange(cell.ColumnIndex, cell.ColSpan).Sum();
                                    if (cell.RowSpan > 1 && cell.RowIndex + cell.RowSpan <= Rows.Count) rt.Bottom = rt.Top + Rows.GetRange(cell.RowIndex, cell.RowSpan).Sum(x => x.RowHeight);
                                    cell.MouseUp(Util.INT(rt), x, y);
                                }
                            }
                        }
                        #endregion
                        #region Fixed
                        if (mrtF.HasValue && isf.HasValue && ief.HasValue)
                        {
                            var vls = v.Cells.GetRange(isf.Value, ief.Value - isf.Value + 1).ToList();
                            var mrt = Util.FromRect(mrtF.Value.Left, rtScrollContent.Top, mrtF.Value.Width + 1, rtScrollContent.Height);
                            foreach (var cell in vls)
                            {
                                if (cell.Visible)
                                {
                                    var rtCol = rts["rtColumn" + cell.Column.Name];
                                    var rt = Util.FromRect(rtCol.Left, rtROW.Top, rtCol.Width, rtROW.Height);
                                    if (cell.ColSpan > 1 && cell.ColumnIndex + cell.ColSpan <= ColWidths.Count) rt.Right = rt.Left + (int)ColWidths.GetRange(cell.ColumnIndex, cell.ColSpan).Sum();
                                    if (cell.RowSpan > 1 && cell.RowIndex + cell.RowSpan <= Rows.Count) rt.Bottom = rt.Top + Rows.GetRange(cell.RowIndex, cell.RowSpan).Sum(x => x.RowHeight);
                                    cell.MouseUp(Util.INT(rt), x, y);
                                }
                            }
                        }
                        #endregion

                        #region MultiSelect
                        else if (SelectionMode == DvDataGridSelectionMode.MULTI)
                        {
                            if (CollisionTool.Check(rtROW, x, y))
                            {
                                if (SelectedRows.Contains(v))
                                {
                                    SelectedRows.Remove(v);
                                    bSelectionChange = true;
                                }
                                else
                                {
                                    SelectedRows.Add(v);
                                    bSelectionChange = true;
                                }
                            }
                        }
                        #endregion
                        #region SingleSelect
                        else if (SelectionMode == DvDataGridSelectionMode.SINGLE)
                        {
                            if (CollisionTool.Check(rtROW, x, y))
                            {
                                SelectedRows.Clear();
                                SelectedRows.Add(v);
                                bSelectionChange = true;
                            }
                        }
                        #endregion

                    });

                    foreach (var v in Rows) v.Selected = SelectedRows.Contains(v);
                    if (bSelectionChange) SelectedChanged?.Invoke(this, null);
                    #endregion
                }
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #region OnMouseClick
        protected override void OnMouseClick(int x, int y)
        {
            bounds((rtContent, rtColumn, rtBox, rtSummary, rtScrollContent, rtScrollV, rtScrollH, rtScrollR) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region Bounds
                    var rts = GetColumnBounds(rtColumn, rtScrollContent);
                    var spw = Convert.ToInt32(SPECIAL_CELL_WIDTH);
                    var sbw = Convert.ToInt32(SELECTOR_BOX_WIDTH);

                    var rtColumnV = Util.FromRect(rtColumn.Left, rtColumn.Top, rtScrollContent.Width, rtColumn.Height);
                    var ColWidths = GetColumnsWidths(rtScrollContent);
                    var vspos = Convert.ToInt32(vscroll.ScrollPositionWithOffset);
                    var hspos = Convert.ToInt32(hscroll.ScrollPositionWithOffset);
                    #endregion
                    #region Filter
                    foreach (var col in Columns)
                    {
                        var rtFilter = rts["rtFilter" + col.Name];
                        if (col.UseFilter && CollisionTool.Check(rtFilter, x, y))
                        {
                            InputBox.ShowString("필터 : " + col.HeaderText, col.FilterText, (ret) =>
                            {
                                if (ret != null)
                                {
                                    col.FilterText = ret;
                                    RefreshRows();
                                    MovingStop();
                                }
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
        #region Clear
        public void Clear()
        {
            SelectionMode = DvDataGridSelectionMode.SINGLE;
            ScrollMode = ScrollMode.Vertical;
            bNotRaiseEvent = true;
            objs = null;
            ColumnGroups.Clear();
            Columns.Clear();
            SummaryRows.Clear();
            Rows.Clear();
            bNotRaiseEvent = false;
            RefreshRows();
        }
        #endregion
        #region Draw
        void DrawColumnBorder(SKCanvas canvas, DvTheme thm, SKRect rtColumn, SKRect rtScrollContent, SKRect rt, bool bevel = true)
        {
            var BoxColor = GetBoxColor(thm);
            var ColumnColor = GetColumnColor(thm);
            var BorderColor = thm.GetBorderColor(BoxColor, ParentContainer.GetBackColor());
            var BevelColor = ColumnColor.BrightnessTransmit(thm.DataGridColumnBevelBright);
            var spw = Convert.ToInt32(SPECIAL_CELL_WIDTH);

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
            {
                p.StrokeWidth = 1;

                var l = Convert.ToInt32(rt.Left);
                var r = Convert.ToInt32(rt.Right);
                var t = Convert.ToInt32(rt.Top);
                var b = Convert.ToInt32(rt.Bottom);
                var nx = rt.Left == 1 ? 0 : 1;
                var ny = rt.Top == 1 ? 0 : 1;
                
                if (bevel && Bevel)
                {
                    p.Color = BevelColor;
                    canvas.DrawLine(r - 1, t + ny + 0.5F, l + 1, t + ny + 0.5F, p);
                    canvas.DrawLine(l + nx + 0.5F, t + 1, l + nx + 0.5F, b - 1, p);
                }

                p.Color = BorderColor;
                canvas.DrawLine(l, b + 0.5F, r, b + 0.5F, p);
                if (Math.Abs(rtScrollContent.Left - rt.Left) > 3) canvas.DrawLine(l + 0.5F, t, l + 0.5F, b, p);
                if (Math.Abs(rtScrollContent.Right - rt.Right) > 3) canvas.DrawLine(r + 0.5F, t, r + 0.5F, b, p);
            }
        }
        #endregion
        #region DataSource
        #region SetDataSource<T>
        public void SetDataSource<T>(IEnumerable<T> values)
        {
            objs = values;
            bNotRaiseEvent = true;
            DataType = typeof(T);
            var props = typeof(T).GetProperties();
            int nCnt = Columns.Where(x => props.Select(v => v.Name).Contains(x.Name) || x is DvDataGridButtonColumn).Count();
            if (nCnt == Columns.Count)
            {
                var dic = props.ToDictionary(x => x.Name);
                Rows.Clear();
                if (values != null)
                {
                    int ri = 0;
                    foreach (var src in values)
                    {
                        var row = new DvDataGridRow(this) { Source = src };

                        for (int i = 0; i < Columns.Count; i++)
                        {
                            var col = Columns[i];
                            if (col is DvDataGridButtonColumn)
                            {
                                var cell = new DvDataGridButtonCell(this, row, (DvDataGridButtonColumn)col);
                                row.Cells.Add(cell);
                            }
                            else
                            {
                                var prop = dic[col.Name];
                                var cell = Activator.CreateInstance(col.CellType, this, row, col) as IDvDataGridCell;
                                row.Cells.Add(cell);
                            }
                        }

                        Rows.Add(row);
                        ri++;
                    }
                }
                RefreshRows();
                bModSize = true;
            }
            else throw new Exception("VALID COUNT");
            bNotRaiseEvent = false;
        }
        #endregion
        #region ResetDataSource
        public void ResetDataSource<T>()
        {
            var SummaryRowColor = GetSummaryRowColor(Design.Theme);
            if (objs != null)
            {
                var values = (IEnumerable<T>)objs;
                SetDataSource<T>(values);

                foreach (var srow in SummaryRows)
                    foreach (var cell in srow.Cells)
                    {
                        cell.CellBackColor = SummaryRowColor;

                        var prop = cell.GetType().GetProperty("CellTextColor");
                        if (prop != null) prop.SetValue(cell, ForeColor);
                    }
            }
        }
        #endregion
        #endregion
        #region Color
        public SKColor GetForeColor(DvTheme Theme) => this.ForeColor ?? Theme.ForeColor;
        public SKColor GetBoxColor(DvTheme Theme) => this.BoxColor ?? Theme.ListBackColor;
        public SKColor GetColumnColor(DvTheme Theme) => this.ColumnColor ?? Theme.ColumnColor;
        public SKColor GetRowColor(DvTheme Theme) => this.RowColor ?? Theme.RowColor;
        public SKColor GetSelectedRowColor(DvTheme Theme) => this.SelectedRowColor ?? Theme.PointColor;
        public SKColor GetSummaryRowColor(DvTheme Theme) => this.SummaryRowColor ?? Theme.RowColor.BrightnessTransmit(-0.1F);
        #endregion
        #region Column
        #region GetColumnRowCount
        private int GetColumnRowCount()
        {
            int ret = 0;
            foreach (var col in Columns)
            {
                var ls = new List<IDvDataGridColumn>();
                ReentCol(col, ls);
                ret = Math.Max(ls.Count, ret);
            }
            return ret;
        }

        private void ReentCol(IDvDataGridColumn col, List<IDvDataGridColumn> ls)
        {
            ls.Add(col);

            if (col.GroupName != null)
            {
                var v = ColumnGroups.Where(x => x.Name == col.GroupName).FirstOrDefault();
                if (v != null) ReentCol(v, ls);
            }
        }
        #endregion
        #region GetColumnBounds
        Dictionary<string, SKRect> GetColumnBounds(SKRect rtColumn, SKRect rtScrollContent)
        {
            var ret = new Dictionary<string, SKRect>();
            var colrc = GetColumnRowCount();
            var spw = Convert.ToInt32(SPECIAL_CELL_WIDTH);

            #region Column
            var xws = new List<SKRect>();
            #region XWs
            var cws = GetColumnsWidths(rtScrollContent);
            if (cws.Count == Columns.Count)
            {
                var x = rtScrollContent.Left + (SelectionMode == DvDataGridSelectionMode.SELECTOR ? spw : 0);

                for (int i = 0; i < cws.Count; i++)
                {
                    var w = cws[i];
                    xws.Add(Util.FromRect(x, rtColumn.Top, w, 0));
                    x += w;
                }
            }
            else throw new Exception("Column Count Mismatch");
            #endregion
            #region Column / Filter
            if (Columns.Where(x => x.UseFilter).Count() > 0)
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    var rt = xws[i];
                    var col = Columns[i];
                    var ls = new List<IDvDataGridColumn>();
                    ReentCol(col, ls);
                    var nc = colrc - ls.Count + 1;

                    var rtFilterV = Util.FromRect(rt.Left, rtColumn.Bottom - ColumnHeight, (rt.Width), ColumnHeight);
                    var rtColumnV = Util.FromRect(rt.Left, rtFilterV.Top - (ColumnHeight * nc), (rt.Width), (ColumnHeight * nc));

                    ret.Add("rtColumn" + col.Name, rtColumnV);
                    ret.Add("rtFilter" + col.Name, rtFilterV);
                }
            }
            else
            {
                for (int i = 0; i < Columns.Count; i++)
                {
                    var rt = xws[i];
                    var col = Columns[i];
                    var ls = new List<IDvDataGridColumn>();
                    ReentCol(col, ls);
                    var nc = colrc - ls.Count + 1;

                    var rtColumnV = Util.FromRect(rt.Left, rtColumn.Bottom - (ColumnHeight * nc), (rt.Width), (ColumnHeight * nc));

                    ret.Add("rtColumn" + col.Name, rtColumnV);
                    ret.Add("rtFilter" + col.Name, Util.FromRect(rtColumnV.Left, rtColumnV.Top, rtColumnV.Width, 0));
                }
            }
            #endregion
            #region ColumnGroup
            for (int i = 0; i < ColumnGroups.Count; i++)
            {
                var rt = xws[i];
                var col = Columns[i];
                var v = ColumnGroups[i];
                var vls = new List<IDvDataGridColumn>();
                var Depth = GetColumnDepth(v);
                GetColumnChildList(v, vls);

                int imin = vls.Min(_x => Columns.IndexOf(_x));
                int imax = vls.Max(_x => Columns.IndexOf(_x));

                var x = (xws[imin].Left);
                var y = rtColumn.Top + ((colrc - Depth) * ColumnHeight);
                var w = (xws[imax].Right) - Convert.ToInt32(xws[imin].Left);
                var h = ColumnHeight;

                ret.Add("rtColumnGroup" + v.Name, Util.FromRect(x, y, w, h));
            }
            #endregion
            #endregion

            return ret;
        }
        #endregion
        #region GetColumnsWidths
        List<int> GetColumnsWidths(SKRect rtScrollContent)
        {
            var ret = new List<int>();
            var spw = Convert.ToInt32(SPECIAL_CELL_WIDTH);
            var tw = rtScrollContent.Width - (SelectionMode == DvDataGridSelectionMode.SELECTOR ? spw : 0);
            var cw = tw - Convert.ToSingle(Columns.Where(x => x.SizeMode == SizeMode.Pixel).Sum(x => x.Width));
            foreach (var v in Columns) ret.Add(v.SizeMode == SizeMode.Pixel ? Convert.ToInt32(v.Width) : Convert.ToInt32((decimal)cw * (v.Width / 100M)));
            return ret;
        }
        #endregion
        #region GetColumnDepth
        int GetColumnDepth(IDvDataGridColumn col)
        {
            if (ColumnGroups.Where(x => x.GroupName == col.Name).Count() > 0)
            {
                return ColumnGroups.Where(x => x.GroupName == col.Name).Select(x => GetColumnDepth(x) + 1).Max();
            }
            else if (Columns.Where(x => x.GroupName == col.Name).Count() > 0)
            {
                return Columns.Where(x => x.GroupName == col.Name).Select(x => GetColumnDepth(x) + 1).Max();
            }
            else return 1;
        }
        #endregion
        #region GetColumnChildList
        void GetColumnChildList(IDvDataGridColumn col, List<IDvDataGridColumn> ls)
        {
            if (col != null)
            {
                if (Columns.Contains(col)) { ls.Add(col); }
                else
                {
                    if (Columns.Count(x => x.GroupName == col.Name) > 0) ls.AddRange(Columns.Where(x => x.GroupName == col.Name));
                    else if (ColumnGroups.Count(x => x.GroupName == col.Name) > 0)
                    {
                        var vls = ColumnGroups.Where(x => x.GroupName == col.Name);
                        foreach (var v in vls) GetColumnChildList(v, ls);
                    }
                }
            }
        }
        #endregion
        #endregion
        #region Row
        #region GetRows
        internal List<DvDataGridRow> GetRows()
        {
            return mrows;
        }
        #endregion
        #region RefreshRows
        public void RefreshRows()
        {
            mrows.Clear();
            mrows.AddRange(Rows);

            #region Sort
            {
                var cols = Columns.Where(x => x.UseSort).ToList();
                var ls = cols.Where(x => x.UseSort).OrderBy(x => x.SortOrder);
                if (ls.Count() > 0)
                {
                    IOrderedEnumerable<DvDataGridRow> result = null;
                    bool bFirst = true;
                    foreach (var col in ls)
                    {
                        int i = Columns.IndexOf(col);
                        switch (col.SortState)
                        {
                            case DvDataGridColumnSortState.ASC:
                                if (bFirst) { result = mrows.OrderBy(x => x.Cells[i].Value); bFirst = false; }
                                else if (result != null)
                                {
                                    result = result.ThenBy(x => x.Cells[i].Value);
                                }
                                break;
                            case DvDataGridColumnSortState.DESC:
                                if (bFirst) { result = mrows.OrderByDescending(x => x.Cells[i].Value); bFirst = false; }
                                else if (result != null)
                                {
                                    result = result.ThenByDescending(x => x.Cells[i].Value);
                                }
                                break;
                        }
                    }

                    if (result != null)
                    {
                        var l = result.ToList();
                        mrows.Clear();
                        mrows.AddRange(l);
                    }
                }
            }
            #endregion
            #region Filter
            {
                var cols = Columns.ToList();
                for (int i = 0; i < cols.Count; i++)
                {
                    if (cols[i].UseFilter && !string.IsNullOrWhiteSpace(cols[i].FilterText))
                    {
                        mrows = mrows.Where(m => (m.Cells[i].Value != null ? m.Cells[i].Value.ToString().ToLower().IndexOf(cols[i].FilterText.ToLower()) != -1 : false)).ToList();
                    }
                }
            }
            #endregion
            #region Make DBSearch
            {
                var lsv = mrows;
                var sum = lsv.Sum(x => x.RowHeight);
                if (PrevTotalHeight != sum)
                {
                    vST = PrevTotalHeight = sum;
                    lsp.Clear();
                    var nsum = 0;
                    foreach (var v in lsv) { lsp.Add(new _DGSearch_() { Height = v.RowHeight, Sum = nsum, Row = v }); nsum += v.RowHeight; }
                }
            }
            #endregion
            #region Summary Calc
            foreach (var row in SummaryRows)
            {
                foreach (var v in row.Cells.Where(x => x is DvDataGridSummaryCell)) ((DvDataGridSummaryCell)v).Calculation();
            }
            #endregion
        }
        #endregion
        #endregion
        #region Invoke
        #region InvokeColumnMouseDown
        public void InvokeColumnMouseDown(IDvDataGridColumn column, int x, int y)
        {
            if (!bNotRaiseEvent && ColumnMouseDown != null) ColumnMouseDown.Invoke(this, new ColumnMouseEventArgs(column, x, y));
        }
        #endregion
        #region InvokeColumnMouseUp
        public void InvokeColumnMouseUp(IDvDataGridColumn column, int x, int y)
        {
            if (!bNotRaiseEvent && ColumnMouseUp != null) ColumnMouseUp.Invoke(this, new ColumnMouseEventArgs(column, x, y));
        }
        #endregion
        #region InvokeCellMouseDown
        public void InvokeCellMouseDown(IDvDataGridCell cell, int x, int y)
        {
            if (!bNotRaiseEvent && CellMouseDown != null) CellMouseDown.Invoke(this, new CellMouseEventArgs(cell, x, y));
        }
        #endregion
        #region InvokeCellMouseUp
        public void InvokeCellMouseUp(IDvDataGridCell cell, int x, int y)
        {
            if (!bNotRaiseEvent && CellMouseUp != null) CellMouseUp.Invoke(this, new CellMouseEventArgs(cell, x, y));
        }
        #endregion
        #region InvokeCellDoubleClick
        public void InvokeCellDoubleClick(IDvDataGridCell cell, int x, int y)
        {
            if (!bNotRaiseEvent && CellMouseDoubleClick != null) CellMouseDoubleClick.Invoke(this, new CellMouseEventArgs(cell, x, y));
        }
        #endregion
        #region InvokeSortChanged
        public void InvokeSortChanged()
        {
            RefreshRows();
            if (!bNotRaiseEvent && SortChanged != null) SortChanged.Invoke(this, new EventArgs());
        }
        #endregion
        #region InvokeValueChanged
        public void InvokeValueChanged(IDvDataGridCell cell, object oldValue, object newValue)
        {
            if (!bNotRaiseEvent && ValueChanged != null) ValueChanged.Invoke(this, new CellValueChangedEventArgs(cell, oldValue, newValue));
        }
        #endregion
        #region InvokeCellButtonClick
        public void InvokeCellButtonClick(DvDataGridCell cell)
        {
            if (!bNotRaiseEvent && CellButtonClick != null) CellButtonClick.Invoke(this, new CellButtonClickEventArgs(cell));
        }
        #endregion
        #endregion

        #region Loop
        private void Loop(Action<int, SKRect, DvDataGridRow> Func)
        {
            bounds((rtContent, rtColumn, rtBox, rtSummary, rtScrollContent, rtScrollV, rtScrollH, rtScrollR) =>
            {
                #region Bounds
                var rts = GetColumnBounds(rtColumn, rtScrollContent);
                var rtsc = rtScrollContent;
                var vspos = Convert.ToInt32(vscroll.ScrollPositionWithOffset);
                var hspos = Convert.ToInt32(hscroll.ScrollPositionWithOffset);
                #endregion
                #region Make DBSearch
                if (bModSize)
                {
                    var lsv = mrows;
                    var sum = lsv.Sum(x => x.RowHeight);
                    if (PrevTotalHeight != sum)
                    {
                        vST = PrevTotalHeight = sum;
                        lsp.Clear();
                        var nsum = 0;
                        foreach (var v in lsv) { lsp.Add(new _DGSearch_() { Height = v.RowHeight, Sum = nsum, Row = v }); nsum += v.RowHeight; }
                    }
                    bModSize = false;
                }
                #endregion
                #region Index Calc
                var ls = GetRows();
                var startidx = (int)MathTool.Constrain(BNSearch(lsp, 0, lsp.Count - 1, rtScrollContent.Top, vspos, rtScrollContent.Top) - 1, 0, ls.Count - 1);
                var endidx = (int)MathTool.Constrain(BNSearch(lsp, 0, lsp.Count - 1, rtScrollContent.Top, vspos, rtScrollContent.Bottom) + 1, 0, ls.Count - 1);
                #endregion

                if (ls.Count > 0)
                {
                    for (int i = startidx; i <= endidx; i++)
                    {
                        var v = ls[i];
                        var y = Convert.ToInt32(rtScrollContent.Top + lsp[i].Sum + vspos);
                        var rtITM = Util.FromRect(rtScrollContent.Left, y, rtScrollContent.Width, v.RowHeight);
                        Func(i, rtITM, v);
                    }
                }
            });
        }

        #region BNSearch
        int BNSearch(List<_DGSearch_> ls, int si, int ei, float top, int vpos, float value)
        {
            int idx = (int)MathTool.Map((double)1, 0, 2, si, ei);
            if (si != ei && idx != si && idx != ei)
            {
                if (idx >= 0 && idx < ls.Count)
                {
                    if (value > ls[idx].Top + vpos + top)
                    {
                        return BNSearch(ls, idx, ei, top, vpos, value);
                    }
                    else if (value < ls[idx].Top + vpos + top)
                    {
                        return BNSearch(ls, si, idx, top, vpos, value);
                    }
                    else return idx;
                }
                return idx;
            }
            else return idx;
        }
        #endregion
        #endregion
        
        #region MovingStop
        public void MovingStop()
        {
            vscroll.TouchStop();
            hscroll.TouchStop();
        }
        #endregion

        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);
            
            var scwh = Convert.ToInt32(Scroll.SC_WH);
            var colrc = GetColumnRowCount();
            var columnH = (colrc + (Columns.Where(x => x.UseFilter).Count() > 0 ? 1 : 0)) * ColumnHeight;
            var summaryH = SummaryRows.Count * RowHeight;

            var ush = ScrollMode == ScrollMode.Both || ScrollMode == ScrollMode.Horizon;
            var usv = ScrollMode == ScrollMode.Both || ScrollMode == ScrollMode.Vertical;

            var rtColumn = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, columnH);
            var rtBox = Util.FromRect(rtColumn.Left, rtColumn.Bottom, rtContent.Width - (usv ? scwh : 0), rtContent.Height - columnH - (ush ? scwh : 0));
            var rtSummary = Util.FromRect(rtBox.Left, rtBox.Bottom - summaryH, rtContent.Width - (usv ? scwh : 0), summaryH);
            var rtScrollContent = Util.FromRect(rtBox.Left, rtBox.Top, rtBox.Width, rtBox.Height - summaryH);
            var rtScrollV = Util.FromRect(rtBox.Right, rtBox.Top, usv ? scwh : 0, rtBox.Height);
            var rtScrollH = Util.FromRect(rtBox.Left, rtBox.Bottom, rtBox.Width, ush ? scwh : 0);
            var rtScrollR = Util.FromRect(rtScrollH.Right, rtScrollV.Bottom, usv ? scwh : 0, ush ? scwh : 0);

            act(rtContent, rtColumn, rtBox, rtSummary, rtScrollContent, rtScrollV, rtScrollH, rtScrollR);
        }
        #endregion
        #endregion
    }
}

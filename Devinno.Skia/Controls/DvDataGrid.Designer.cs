using Devinno.Skia.Collections;
using Devinno.Skia.Design;
using Devinno.Skia.Dialogs;
using Devinno.Skia.Extensions;
using Devinno.Skia.Theme;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    #region enums
    #region enum : DvDataGridSelectionMode
    public enum DvDataGridSelectionMode { NONE, SELECTOR, SINGLE, MULTI }
    #endregion
    #region enum : DvDataGridColumnSortState
    public enum DvDataGridColumnSortState { NONE, ASC, DESC };
    #endregion
    #endregion

    #region classes : base
    #region class : DvDataGridColumn
    public abstract class DvDataGridColumn
    {
        #region Properties
        public DvDataGrid Grid { get; private set; }

        public string Name { get; set; }
        public string GroupName { get; set; }
        public string HeaderText { get; set; }

        public SizeInfo Size { get; set; }

        public bool UseFilter { get; set; }
        public string FilterText { get; set; }

        public bool UseSort { get; set; }
        public DvDataGridColumnSortState SortState { get; set; }
        public int SortOrder { get; private set; }

        public SKColor? TextColor { get; set; }
        public SKColor? CellBackColor { get; set; }
        public Type CellType { get; set; }
        public bool Fixed { get; set; }
        #endregion

        #region Constructor
        public DvDataGridColumn(DvDataGrid dataGrid)
        {
            Grid = dataGrid;
        }

        protected DvDataGridColumn()
        {
        }
        #endregion

        #region Method
        #region internal
        #region Draw
        internal void Draw(SKCanvas canvas, DvTheme thm, SKRect bounds, SKRect? boundsFilter,
            SKColor ForeColor, SKColor BackColor, SKColor ColumnColor, SKColor BorderColor, SKColor InputColumnColor, SKColor InputColumnBorderColor)
        {
            if (Grid != null)
            {
                thm.DrawDataGridColumn(canvas, 
                    bounds, boundsFilter,
                    ForeColor, BackColor, ColumnColor, BorderColor, InputColumnColor, InputColumnBorderColor,
                    HeaderText, Grid.FontName, Grid.FontSize, Grid.FontStyle,
                    UseFilter, FilterText, UseSort, SortState);

                ColumnDraw(canvas, thm, bounds);
            }
        }
        #endregion
        #region MouseDown
        internal void MouseDown(SKRect Bounds, SKRect? BoundsFilter, float x, float y) => ColumnMouseDown(Bounds, x, y);
        #endregion
        #region MouseUp
        internal void MouseUp(SKRect Bounds, SKRect? BoundsFilter, float x, float y) => ColumnMouseUp(Bounds, x, y);
        #endregion
        #region MouseClick
        internal void MouseClick(SKRect Bounds, SKRect? BoundsFilter, float x, float y)
        {
            if (Grid != null)
            {
                if (BoundsFilter.HasValue && CollisionTool.Check(BoundsFilter.Value, x, y) && UseFilter)
                {
                    DvDialogs.Keyboard.ShowKeyboard(HeaderText, (result) =>
                    {
                        if(result != null)
                        {
                            FilterText = result;
                            Grid.RefreshRows();
                        }
                    });
                }

                if (UseSort)
                {
                    if(CollisionTool.Check(Bounds, x, y))
                    {
                        switch (SortState)
                        {
                            case DvDataGridColumnSortState.NONE:
                                SortState = DvDataGridColumnSortState.ASC;
                                SortOrder = (SortState != DvDataGridColumnSortState.NONE ? Grid.Columns.Where(x => x.UseSort && x.SortState != DvDataGridColumnSortState.NONE).Count() : 1000);
                                Grid.InvokeSortChanged();
                                Grid.RefreshRows();
                                break;
                            case DvDataGridColumnSortState.ASC:
                                SortState = DvDataGridColumnSortState.DESC;
                                SortOrder = (SortState != DvDataGridColumnSortState.NONE ? Grid.Columns.Where(x => x.UseSort && x.SortState != DvDataGridColumnSortState.NONE).Count() : 1000);
                                Grid.InvokeSortChanged();
                                Grid.RefreshRows();
                                break;
                            case DvDataGridColumnSortState.DESC:
                                SortState = DvDataGridColumnSortState.NONE;
                                SortOrder = (SortState != DvDataGridColumnSortState.NONE ? Grid.Columns.Where(x => x.UseSort && x.SortState != DvDataGridColumnSortState.NONE).Count() : 1000);
                                Grid.InvokeSortChanged();
                                Grid.RefreshRows();
                                break;
                        }
                    }
                }
            }

            ColumnMouseClick(Bounds, x, y);
        }
        #endregion
        #region MouseDoubleClick
        internal void MouseDoubleClick(SKRect Bounds, SKRect? BoundsFilter, float x, float y) => ColumnMouseDoubleClick(Bounds, x, y);
        #endregion
        #region MouseLongClick
        internal void MouseLongClick(SKRect Bounds, SKRect? BoundsFilter, float x, float y) => ColumnMouseLongClick(Bounds, x, y);
        #endregion
        #endregion

        #region virtual
        protected virtual void ColumnDraw(SKCanvas Canvas, DvTheme Theme, SKRect Bounds) { }
        protected virtual void ColumnMouseDown(SKRect Bounds, float x, float y) { }
        protected virtual void ColumnMouseUp(SKRect Bounds, float x, float y) { }
        protected virtual void ColumnMouseClick(SKRect Bounds, float x, float y) { }
        protected virtual void ColumnMouseDoubleClick(SKRect Bounds, float x, float y) { }
        protected virtual void ColumnMouseLongClick(SKRect Bounds, float x, float y) { }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridRow
    public class DvDataGridRow
    {
        #region Properties
        public DvDataGrid Grid { get; private set; }

        public List<DvDataGridCell> Cells { get; private set; } = new List<DvDataGridCell>();
        public float RowHeight { get; private set; }
        public bool Selected { get; set; }
        public object Source { get; set; }
        public object Tag { get; set; }
        public int RowIndex { get; internal set; }

        internal float Top { get; set; }
        internal float Bottom { get; set; }
        #endregion

        #region Constructor
        public DvDataGridRow(DvDataGrid Grid)
        {
            this.Grid = Grid;
            this.RowHeight = Grid.RowHeight;
        }
        #endregion

        #region Method
        #region internal
        #region Draw
        internal void Draw(SKCanvas canvas, DvTheme thm, SKRect bounds, 
            SKRect rtColumn, SKRect rtScrollContent,
            List<DGColumnInfo> lscol, List<DvDataGridRow> rows, List<DGColumnInfo> cols, float hspos,
            string FontName, float FontSize, DvFontStyle FontStyle,
            SKColor ForeColor, SKColor BackColor, SKColor BoxColor, SKColor BorderColor, SKColor InputBoxColor, SKColor InputBoxBorderColor)
        {
            if (Grid != null)
            {
                var bc = BoxColor;
                var sp = canvas.Save();
                var rtcp = Util.FromRect(bounds); rtcp.Inflate(0, 0.5F);
                canvas.ClipRect(rtcp);

                #region Cell
                loop(lscol, rows, cols, hspos, (cell, col, rtv) =>
                {
                    if (cell.Visible)
                    {
                        cell.Draw(canvas, thm, rtv, rows, cols, hspos,
                             ForeColor, BackColor, bc, BorderColor, InputBoxColor, InputBoxBorderColor);
                    }
                    else
                    {
                        var his = new List<DvDataGridCell>();
                        var res = new List<DvDataGridCell>();
                        re(Cells[col.ColIndex], Cells[col.ColIndex], rows, his, res);
                        if (res.Count > 0)
                        {
                            var cell2 = res.First();

                            var lsc = cols.GetRange(cell2.ColIndex, cell2.ColSpan);
                            var lsr = rows.GetRange(cell2.RowIndex, cell2.RowSpan);

                            var L = lsc.First().GetBounds(hspos).Left;
                            var R = lsc.Last().GetBounds(hspos).Right;
                            var T = lsr.First().Top;
                            var B = lsr.Last().Bottom;

                            var rtv2 = new SKRect(L, T, R, B);

                            cell2.Draw(canvas, thm, rtv2, rows, cols, hspos,
                                ForeColor, BackColor, bc, BorderColor, InputBoxColor, InputBoxBorderColor);
                        }
                    }
                });
                #endregion
                #region Selector
                if (Grid.SelectionMode == DvDataGridSelectionMode.SELECTOR)
                {
                    var cb =  (Selected ? Grid.GetSelectedColor(thm) : (Grid.SelectorCellBackColor ?? bc));
                    var bc2 = cb.BrightnessTransmit(RowIndex % 2 == 0 ? 0 : -0.1F);
                    var ibc = bc2.BrightnessTransmit(-0.25F);
                    var ibbc = thm.GetBorderColor(ibc, bc2);

                    var SPECIAL_CELL_WIDTH = DvDataGrid.SPECIAL_CELL_WIDTH;
                    var SELECTOR_BOX_WIDTH = DvDataGrid.SELECTOR_BOX_WIDTH;

                    var rtSelector = new SKRect(rtColumn.Left, Top, rtColumn.Left + SPECIAL_CELL_WIDTH, Bottom);
                    var rtSelectorBox = MathTool.MakeRectangle(Util.INT(rtSelector), new SKSize(SELECTOR_BOX_WIDTH, SELECTOR_BOX_WIDTH));

                    thm.DrawDataGridCell(canvas,
                         rtSelector,
                         ForeColor, BackColor, bc2, BorderColor, InputBoxColor, InputBoxBorderColor);

                    thm.DrawCheckBox(canvas, rtSelectorBox, SELECTOR_BOX_WIDTH,
                        ibc, ibbc, ForeColor, ForeColor,
                        "", FontName, FontSize, FontStyle, DvContentAlignment.MiddleLeft, Selected);

                }
                #endregion

                RowDraw(canvas, thm, bounds);

                canvas.RestoreToCount(sp);
            }
        }
        #endregion
        #region MouseDown
        internal void MouseDown(SKRect Bounds, float x, float y,
            List<DGColumnInfo> lscol, List<DvDataGridRow> rows, List<DGColumnInfo> cols, float hspos)
        {
            loop(lscol, rows, cols, hspos, (cell, col, rtv) =>
            {
                if (cell.Visible) cell.MouseDown(rtv, x, y);
            });

            RowMouseDown(Bounds, x, y);
        }
        #endregion
        #region MouseUp
        internal void MouseUp(SKRect Bounds, float x, float y,
            List<DGColumnInfo> lscol, List<DvDataGridRow> rows, List<DGColumnInfo> cols, float hspos)
        {
            loop(lscol, rows, cols, hspos, (cell, col, rtv) =>
            {
                if (cell.Visible) cell.MouseUp(rtv, x, y);
            });

            RowMouseUp(Bounds, x, y);
        }
        #endregion
        #region MouseClick
        internal void MouseClick(SKRect Bounds, float x, float y,
            List<DGColumnInfo> lscol, List<DvDataGridRow> rows, List<DGColumnInfo> cols, float hspos)
        {
            var SPECIAL_CELL_WIDTH = DvDataGrid.SPECIAL_CELL_WIDTH;
            var SELECTOR_BOX_WIDTH = DvDataGrid.SELECTOR_BOX_WIDTH;

            var rtSelector = new SKRect(0, Top, 0 + SPECIAL_CELL_WIDTH, Bottom);
            var rtSelectorBox = MathTool.MakeRectangle(Util.INT(rtSelector), new SKSize(SELECTOR_BOX_WIDTH, SELECTOR_BOX_WIDTH));

            if (CollisionTool.Check(rtSelector, x, y)) Selected = !Selected;

            loop(lscol, rows, cols, hspos, (cell, col, rtv) =>
            {
                if (cell.Visible) cell.MouseClick(rtv, x, y);
            });

            RowMouseClick(Bounds, x, y);
        }
        #endregion
        #region MouseDoubleClick
        internal void MouseDoubleClick(SKRect Bounds, float x, float y,
            List<DGColumnInfo> lscol, List<DvDataGridRow> rows, List<DGColumnInfo> cols, float hspos)
        {
            loop(lscol, rows, cols, hspos, (cell, col, rtv) =>
            {
                if (cell.Visible) cell.MouseDoubleClick(rtv, x, y);
            });

            RowMouseDoubleClick(Bounds, x, y);
        }
        #endregion
        #region MouseLongClick
        internal void MouseLongClick(SKRect Bounds, float x, float y,
            List<DGColumnInfo> lscol, List<DvDataGridRow> rows, List<DGColumnInfo> cols, float hspos)
        {
            loop(lscol, rows, cols, hspos, (cell, col, rtv) =>
            {
                if (cell.Visible) cell.MouseLongClick(rtv, x, y);
            });

            RowMouseLongClick(Bounds, x, y);
        }
        #endregion
        #endregion

        #region virtual
        protected virtual void RowDraw(SKCanvas Canvas, DvTheme Theme, SKRect Bounds) { }
        protected virtual void RowMouseDown(SKRect Bounds, float x, float y) { }
        protected virtual void RowMouseUp(SKRect Bounds, float x, float y) { }
        protected virtual void RowMouseClick(SKRect Bounds, float x, float y) { }
        protected virtual void RowMouseDoubleClick(SKRect Bounds, float x, float y) { }
        protected virtual void RowMouseLongClick(SKRect Bounds, float x, float y) { }
        #endregion

        #region Private
        #region re
        void re(DvDataGridCell target, DvDataGridCell v, List<DvDataGridRow> rows, List<DvDataGridCell> his, List<DvDataGridCell> res)
        {
            his.Add(v);

            if (res.Count == 0)
            {
                var l = get(v.ColIndex - 1, v.RowIndex, rows);
                var r = get(v.ColIndex + 1, v.RowIndex, rows);
                var t = get(v.ColIndex, v.RowIndex - 1, rows);
                var b = get(v.ColIndex, v.RowIndex + 1, rows);

                if (l != null && !his.Contains(l))
                {
                    if (l.Visible)
                    {
                        if (CollisionTool.Check(Util.FromRect(l.ColIndex, l.RowIndex, l.ColSpan - 1, l.RowSpan - 1),
                                                Util.FromRect(target.ColIndex, target.RowIndex, target.ColSpan - 1, target.RowSpan - 1)))
                        {
                            res.Add(l);
                        }
                    }
                    else re(target, l, rows, his, res);
                }

                if (r != null && !his.Contains(r))
                {
                    if (r.Visible)
                    {
                        if (CollisionTool.Check(Util.FromRect(r.ColIndex, r.RowIndex, r.ColSpan - 1, r.RowSpan - 1),
                                                Util.FromRect(target.ColIndex, target.RowIndex, target.ColSpan - 1, target.RowSpan - 1)))
                        {
                            res.Add(r);
                        }
                    }
                    else re(target, r, rows, his, res);
                }

                if (t != null && !his.Contains(t))
                {
                    if (t.Visible)
                    {
                        if (CollisionTool.Check(Util.FromRect(t.ColIndex, t.RowIndex, t.ColSpan - 1, t.RowSpan - 1),
                                                Util.FromRect(target.ColIndex, target.RowIndex, target.ColSpan - 1, target.RowSpan - 1)))
                        {
                            res.Add(t);
                        }
                    }
                    else re(target, t, rows, his, res);
                }

                if (b != null && !his.Contains(b))
                {
                    if (b.Visible)
                    {
                        if (CollisionTool.Check(Util.FromRect(b.ColIndex, b.RowIndex, b.ColSpan - 1, b.RowSpan - 1),
                                                Util.FromRect(target.ColIndex, target.RowIndex, target.ColSpan - 1, target.RowSpan - 1)))
                        {
                            res.Add(b);
                        }
                    }
                    else re(target, b, rows, his, res);
                }
            }
        }
        #endregion
        #region get
        DvDataGridCell get(int ic, int ir, List<DvDataGridRow> rows)
        {
            if (ir >= 0 && ir < rows.Count && ic >= 0 && ic <= rows[ir].Cells.Count)
                return rows[ir].Cells[ic];
            else 
                return null;
        }
        #endregion
        #region loop
        void loop(List<DGColumnInfo> lscol, List<DvDataGridRow> rows, List<DGColumnInfo> cols, float hspos,
            Action<DvDataGridCell, DGColumnInfo, SKRect> act)
        {
            foreach (var v in lscol.OrderBy(x => x.Column.Fixed))
            {
                var rtb = v.GetBounds(hspos);
                var rtv = new SKRect(rtb.Left, Top, rtb.Right, Bottom);

                if (v.ColIndex >= 0 && v.ColIndex < Cells.Count)
                {
                    var cell = Cells[v.ColIndex];

                    var lsc = cols.GetRange(cell.ColIndex, cell.ColSpan);
                    var lsr = rows.GetRange(cell.RowIndex, cell.RowSpan);

                    var L = lsc.First().GetBounds(hspos).Left;
                    var R = lsc.Last().GetBounds(hspos).Right;
                    var T = lsr.First().Top;
                    var B = lsr.Last().Bottom;

                    var rt = new SKRect(L, T, R, B);

                    act(cell, v, rt);
                }
            }
        }
        #endregion
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridCell
    public abstract class DvDataGridCell
    {
        #region Properties
        public string Name { get; set; }

        public int ColSpan { get; set; } = 1;
        public int RowSpan { get; set; } = 1;
        public int ColIndex { get { return Row.Cells.IndexOf(this); } }
        public int RowIndex { get { return Grid.Rows.IndexOf(Row); } }

        public bool Visible { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public object Tag { get; set; }

        public SKColor? CellBackColor { get; set; }
        public SKColor? SelectedCellBackColor { get; set; }
        public SKColor? CellTextColor { get; set; }

        public DvDataGrid Grid { get; private set; }
        public DvDataGridRow Row { get; private set; }
        public DvDataGridColumn Column { get; private set; }

        protected PropertyInfo ValueInfo { get; }

        public object Value
        {
            get
            {
                return ValueInfo?.GetValue(Row.Source);
            }
            set
            {
                if (ValueInfo != null && ValueInfo.CanWrite)
                {
                    var old = ValueInfo.GetValue(Row.Source);
                    if (old != value)
                    {
                        ValueInfo.SetValue(Row.Source, value);
                        Grid?.InvokeValueChanged(this, old, value);
                    }
                }
            }
        }
        #endregion

        #region Constructor
        public DvDataGridCell(DvDataGrid Grid, DvDataGridRow Row, DvDataGridColumn Column)
        {
            this.Grid = Grid;
            this.Row = Row;
            this.Column = Column;
            this.CellBackColor = this.Column?.CellBackColor;

            if (Grid.DataType != null && !(Column is DvDataGridButtonColumn))
                ValueInfo = Grid.DataType.GetProperty(Column.Name);
        }
        #endregion

        #region Method
        #region internal
        internal void Draw(SKCanvas canvas, DvTheme thm, SKRect bounds,
            List<DvDataGridRow> rows, List<DGColumnInfo> cols, float hspos,
            SKColor ForeColor, SKColor BackColor, SKColor BoxColor, SKColor BorderColor, SKColor InputBoxColor, SKColor InputBoxBorderColor)
        {
            if (Row.Grid != null)
            {
                var CellBackColor = GetCellBackColor(thm);
                thm.DrawDataGridCell(canvas, bounds, ForeColor, BackColor, CellBackColor, BorderColor, InputBoxColor, InputBoxBorderColor);
            }

            CellDraw(canvas, thm, bounds);
        }
        internal void MouseDown(SKRect Bounds, float x, float y) => CellMouseDown(Bounds, x, y);
        internal void MouseUp(SKRect Bounds, float x, float y) => CellMouseUp(Bounds, x, y);
        internal void MouseClick(SKRect Bounds, float x, float y) => CellMouseClick(Bounds, x, y);
        internal void MouseDoubleClick(SKRect Bounds, float x, float y) => CellMouseDoubleClick(Bounds, x, y);
        internal void MouseLongClick(SKRect Bounds, float x, float y) => CellMouseLongClick(Bounds, x, y);

        internal SKColor GetCellBackColor(DvTheme thm)
        {
            var BoxColor = Row.Grid.GetBoxColor(thm);
            return (Row.Selected ? Row.Grid.GetSelectedColor(thm) : (this.CellBackColor ?? BoxColor)).BrightnessTransmit(Row.RowIndex % 2 == 0 ? 0 : -0.1F);
        }

        internal SKColor GetInputBackColor(DvTheme thm) => GetCellBackColor(thm).BrightnessTransmit(-0.25F);
        #endregion

        #region virtual
        protected virtual void CellDraw(SKCanvas Canvas, DvTheme Theme, SKRect Bounds) { }
        protected virtual void CellMouseDown(SKRect Bounds, float x, float y) { }
        protected virtual void CellMouseUp(SKRect Bounds, float x, float y) { }
        protected virtual void CellMouseClick(SKRect Bounds, float x, float y) { }
        protected virtual void CellMouseDoubleClick(SKRect Bounds, float x, float y) { }
        protected virtual void CellMouseLongClick(SKRect Bounds, float x, float y) { }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridSummaryRow
    public class DvDataGridSummaryRow
    {
        #region Properties
        public DvDataGrid Grid { get; private set; }
        public EventList2<DvDataGridSummaryCell> Cells { get; private set; } = new EventList2<DvDataGridSummaryCell>();
        public int RowIndex { get; internal set; }
        public object Tag { get; set; }

        internal float Top { get; set; }
        internal float Bottom { get; set; }
        #endregion

        #region Constructor
        public DvDataGridSummaryRow(DvDataGrid Grid)
        {
            this.Grid = Grid;
            Cells.Appended += (o, s) =>
            {
                foreach (var v in s.Items) v.Row = this;
            };
        }
        #endregion

        #region Method
        #region internal
        internal void Draw(SKCanvas canvas, DvTheme thm, SKRect bounds,
            SKRect rtColumn, SKRect rtScrollContent,
            List<DGColumnInfo> lscol, List<DvDataGridSummaryRow> rows, List<DGColumnInfo> cols, float hspos,
            string FontName, float FontSize, DvFontStyle FontStyle,
            SKColor ForeColor, SKColor BackColor, SKColor BoxColor, SKColor BorderColor, SKColor InputBoxColor, SKColor InputBoxBorderColor)
        {
            if (Grid != null)
            {
                var bc = BoxColor;
                var sp = canvas.Save();
                var rtcp = Util.FromRect(bounds); rtcp.Inflate(0, 0.5F);
                canvas.ClipRect(rtcp);

                var ri = RowIndex;

                #region Cell
                foreach (var v in lscol.OrderBy(x => x.Column.Fixed))
                {
                    var rtb = v.GetBounds(hspos);
                    var rtv = new SKRect(rtb.Left, Top, rtb.Right, Bottom);

                    if (v.ColIndex >= 0 && v.ColIndex < Cells.Count)
                    {
                        if (Cells[v.ColIndex].Visible)
                        {
                            Cells[v.ColIndex].Draw(canvas, thm, rtv, rows, cols, hspos,
                                ForeColor, BackColor, bc, BorderColor, InputBoxColor, InputBoxBorderColor);
                        }
                    }
                }
                #endregion
                #region Selector
                if (Grid.SelectionMode == DvDataGridSelectionMode.SELECTOR)
                {
                    var SPECIAL_CELL_WIDTH = DvDataGrid.SPECIAL_CELL_WIDTH;

                    var rtSelector = new SKRect(rtColumn.Left, Top, rtColumn.Left + SPECIAL_CELL_WIDTH, Bottom);

                    thm.DrawDataGridCell(canvas,
                         rtSelector,
                         ForeColor, BackColor, bc, BorderColor, InputBoxColor, InputBoxBorderColor);

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        var L = rtSelector.Left;
                        var R = rtSelector.Right;
                        var T = rtSelector.Top;

                        #region Border
                        p.IsAntialias = false;
                        p.IsStroke = true;
                        p.StrokeWidth = 1F;
                        p.Color = BorderColor;
                        canvas.DrawLine(L, T + 0.5F, R, T + 0.5F, p);
                        #endregion
                    }
                }
                #endregion

                RowDraw(canvas, thm, bounds);

                canvas.RestoreToCount(sp);
            }
        }
        internal void MouseDown(SKRect Bounds, float x, float y) { }
        internal void MouseUp(SKRect Bounds, float x, float y) { }
        internal void MouseClick(SKRect Bounds, float x, float y) { }
        internal void MouseDoubleClick(SKRect Bounds, float x, float y) { }
        internal void MouseLongClick(SKRect Bounds, float x, float y) { }
        #endregion

        #region virtual
        protected virtual void RowDraw(SKCanvas Canvas, DvTheme Theme, SKRect Bounds) { }
        protected virtual void RowMouseDown(SKRect Bounds, float x, float y) { }
        protected virtual void RowMouseUp(SKRect Bounds, float x, float y) { }
        protected virtual void RowMouseClick(SKRect Bounds, float x, float y) { }
        protected virtual void RowMouseDoubleClick(SKRect Bounds, float x, float y) { }
        protected virtual void RowMouseLongClick(SKRect Bounds, float x, float y) { }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridSummaryCell
    public abstract class DvDataGridSummaryCell
    {
        #region Properties
        public int ColSpan { get; set; } = 1;
        public int RowSpan { get; set; } = 1;
        public int ColIndex { get { return Row.Cells.IndexOf(this); } }
        public int RowIndex { get { return Grid.SummaryRows.IndexOf(Row); } }

        public bool Visible { get; set; } = true;
        public object Tag { get; set; }

        public SKColor? CellBackColor { get; set; }
        public SKColor? CellTextColor { get; set; }

        public DvDataGrid Grid { get; private set; }
        public DvDataGridSummaryRow Row { get; internal set; }
        #endregion

        #region Constructor
        public DvDataGridSummaryCell(DvDataGrid Grid)
        {
            this.Grid = Grid;
        }
        #endregion

        #region Method
        #region internal
        internal void Draw(SKCanvas canvas, DvTheme thm, SKRect bounds,
            List<DvDataGridSummaryRow> rows, List<DGColumnInfo> cols, float hspos,
            SKColor ForeColor, SKColor BackColor, SKColor BoxColor, SKColor BorderColor, SKColor InputBoxColor, SKColor InputBoxBorderColor)
        {
            var CellBackColor = this.CellBackColor ?? BoxColor;

            var lsc = cols.GetRange(ColIndex, ColSpan);
            var lsr = rows.GetRange(RowIndex, RowSpan);

            var L = lsc.First().GetBounds(hspos).Left;
            var R = lsc.Last().GetBounds(hspos).Right;
            var T = lsr.First().Top;
            var B = lsr.Last().Bottom;

            var rtv = new SKRect(L, T, R, B);

            thm.DrawDataGridCell(canvas, rtv, ForeColor, BackColor, CellBackColor, BorderColor, InputBoxColor, InputBoxBorderColor);

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                #region Border
                p.IsAntialias = false;
                p.IsStroke = true;
                p.StrokeWidth = 1F;
                p.Color = BorderColor;
                canvas.DrawLine(L, T+0.5F, R, T+0.5F, p);
                #endregion
            }

            CellDraw(canvas, thm, rtv);
        }
        internal void MouseDown(SKRect Bounds, float x, float y) { }
        internal void MouseUp(SKRect Bounds, float x, float y) { }
        internal void MouseClick(SKRect Bounds, float x, float y) { }
        internal void MouseDoubleClick(SKRect Bounds, float x, float y) { }
        internal void MouseLongClick(SKRect Bounds, float x, float y) { }
        #endregion

        #region virtual
        protected virtual void CellDraw(SKCanvas Canvas, DvTheme Theme, SKRect Bounds) { }
        protected virtual void CellMouseDown(SKRect Bounds, float x, float y) { }
        protected virtual void CellMouseUp(SKRect Bounds, float x, float y) { }
        protected virtual void CellMouseClick(SKRect Bounds, float x, float y) { }
        protected virtual void CellMouseDoubleClick(SKRect Bounds, float x, float y) { }
        protected virtual void CellMouseLongClick(SKRect Bounds, float x, float y) { }
     
        public virtual void Calculate() { }
        #endregion
        #endregion
    }
    #endregion
    #endregion

    #region classes : Column /  Cell
    #region class : DvDataGridLabelColumn
    public class DvDataGridLabelColumn : DvDataGridColumn
    {
        #region Properties
        public Func<object, string> TextConverter { get; set; }
        public string FormatString { get; set; }
        #endregion

        #region Constructor
        public DvDataGridLabelColumn(DvDataGrid dg) : base(dg) => CellType = typeof(DvDataGridLabelCell);
        #endregion
    }
    #endregion
    #region class : DvDataGridLabelCell
    public class DvDataGridLabelCell : DvDataGridCell
    {
        #region Constructor
        public DvDataGridLabelCell(DvDataGrid dg, DvDataGridRow row, DvDataGridColumn col) : base(dg, row, col) { }
        #endregion

        #region Override
        #region CellDraw
        protected override void CellDraw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (Grid != null)
            {
                var textColor = CellTextColor ?? Grid.GetForeColor(thm);

                var s = "";
                var col = Column as DvDataGridLabelColumn;
                if (col != null && col.TextConverter != null) s = col.TextConverter(Value);
                else s = ValueTool.Text(Value, col.FormatString);
               
                thm.DrawText(canvas, bounds, Grid.FontName, Grid.FontSize, Grid.FontStyle, s, textColor);
            }
            base.CellDraw(canvas, thm, bounds);
        }
        #endregion
        #endregion
    }
    #endregion

    #region class : DvDataGridNumberColumn<T>
    public class DvDataGridNumberColumn<T> : DvDataGridColumn where T : struct
    {
        #region Properties
        public string FormatString { get; set; }
        #endregion

        #region Constructor
        public DvDataGridNumberColumn(DvDataGrid dg) : base(dg) => CellType = typeof(DvDataGridNumberCell<T>);
        #endregion
    }
    #endregion
    #region class : DvDataGridNumberCell<T>
    public class DvDataGridNumberCell<T> : DvDataGridCell where T : struct
    {
        #region Constructor
        public DvDataGridNumberCell(DvDataGrid dg, DvDataGridRow row, DvDataGridColumn col) : base(dg, row, col) { }
        #endregion

        #region Override
        #region CellDraw
        protected override void CellDraw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (Grid != null)
            {
                var format = (Column as DvDataGridNumberColumn<T>)?.FormatString;
                var textColor = CellTextColor ?? Grid.GetForeColor(thm);
                var s = ValueTool.Text<T>((T)Value, format);
                thm.DrawText(canvas, bounds, Grid.FontName, Grid.FontSize, Grid.FontStyle, s, textColor);
            }

            base.CellDraw(canvas, thm, bounds);
        }
        #endregion
        #endregion
    }
    #endregion

    #region class : DvDataGridButtonColumn
    public class DvDataGridButtonColumn : DvDataGridColumn
    {
        #region Properties
        public SKColor? ButtonColor { get; set; } = null;
        public string ButtonText { get; set; } = null;
        public string IconString { get; set; } = null;
        public bool Gradient { get; set; } = false;
        #endregion

        #region Constructor
        public DvDataGridButtonColumn(DvDataGrid dg) : base(dg) => CellType = typeof(DvDataGridButtonCell);
        #endregion
    }
    #endregion
    #region class : DvDataGridButtonCell
    public class DvDataGridButtonCell : DvDataGridCell
    {
        #region Properties
        public SKColor? ButtonColor { get; set; } = null;
        public string ButtonText { get; set; } = null;
        public string IconString { get; set; } = null;
        public DvDataGridButtonCell(DvDataGrid dg, DvDataGridRow row, DvDataGridColumn col) : base(dg, row, col) { }
        #endregion

        #region Member Variable
        private bool bDown = false;
        #endregion

        #region Override
        #region CellDraw
        protected override void CellDraw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (Row.Grid != null)
            {
                var ForeColor = Row.Grid.GetForeColor(thm);
                var BackColor = Row.Grid.GetBackColor();
                var ButtonColor = this.ButtonColor ?? ((Column as DvDataGridButtonColumn)?.ButtonColor ?? thm.ButtonColor);
                var BorderColor = thm.GetBorderColor(ButtonColor, BackColor);
                var ButtonText = this.ButtonText ?? ((Column as DvDataGridButtonColumn)?.ButtonText);
                var IconString = this.IconString ?? ((Column as DvDataGridButtonColumn)?.IconString);
                var Gradient = ((Column as DvDataGridButtonColumn)?.Gradient) ?? false;

                var ti = new DvTextIcon
                {
                    Text = ButtonText,
                    FontName = Row.Grid.FontName,
                    FontSize = Row.Grid.FontSize,
                    FontStyle = Row.Grid.FontStyle,
                    IconString = IconString,
                    IconSize = Row.Grid.FontSize,
                };

                thm.DrawButton(canvas, bounds, 
                    ButtonColor, BorderColor, ForeColor, BackColor,
                    ti, DvRoundType.Rect, Gradient, true, DvContentAlignment.MiddleCenter, bDown);
            }
        }
        #endregion
        #region CellMouseDown
        protected override void CellMouseDown(SKRect Bounds, float x, float y)
        {
            if (CollisionTool.Check(Bounds, x, y))
            {
                bDown = true;
                if (Row.Grid != null)
                {
                    Row.Grid.Design?.Input(Row.Grid);
                    Row.Grid.ScrollClear();
                }
            }
            base.CellMouseDown(Bounds, x, y);
        }
        #endregion
        #region CellMouseUp
        protected override void CellMouseUp(SKRect Bounds, float x, float y)
        {
            if (bDown)
            {
                bDown = false;
                Row.Grid?.InvokeCellButtonClick(this);
            }
            base.CellMouseUp(Bounds, x, y);
        }
        #endregion
        #endregion
    }
    #endregion

    #region class : DvDataGridLampColumn
    public class DvDataGridLampColumn : DvDataGridColumn
    {
        #region Properties
        public SKColor? OnLampColor { get; set; } = null;
        public SKColor? OffLampColor { get; set; } = null;
        public int LampSize { get; set; } = 24;
        public string Text { get; set; } = null;
        #endregion

        #region Constructor
        public DvDataGridLampColumn(DvDataGrid dg) : base(dg) => CellType = typeof(DvDataGridLampCell);
        #endregion
    }
    #endregion
    #region class : DvDataGridLampCell
    public class DvDataGridLampCell : DvDataGridCell
    {
        #region Properties
        private bool Animation => Grid != null && Grid.Design != null ? Grid.Design.Animation : false;
        #endregion

        #region Member Variable
        Animation ani = new Animation();
        #endregion

        #region Constructor
        public DvDataGridLampCell(DvDataGrid dg, DvDataGridRow row, DvDataGridColumn col) : base(dg, row, col) { }
        #endregion

        #region Override
        #region CellDraw
        protected override void CellDraw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (Row.Grid != null)
            {
                var ForeColor = Row.Grid.GetForeColor(thm);
                var BackColor = GetCellBackColor(thm);
                var OnLampColor = (Column as DvDataGridLampColumn)?.OnLampColor ?? thm.PointColor;
                var OffLampColor = (Column as DvDataGridLampColumn)?.OffLampColor ?? thm.ButtonColor;
                var BorderColor = thm.GetBorderColor(BackColor, BackColor);
                var LampText = (Column as DvDataGridLampColumn)?.Text ?? "";
                var LampSize = (Column as DvDataGridLampColumn)?.LampSize ?? 24;
                var val = (Value as bool?) ?? false;

                thm.DrawLamp(canvas, bounds,
                            OnLampColor, OffLampColor, ForeColor, BackColor, BorderColor,
                            LampText, Row.Grid.FontName, Row.Grid.FontSize, Row.Grid.FontStyle,
                            LampSize, DvTextIconAlignment.LeftRight,
                            DvContentAlignment.MiddleCenter, val, Animation, ani);
            }
        }
        #endregion
        #endregion
    }
    #endregion

    #region class : DvDataGridCheckBoxColumn
    public class DvDataGridCheckBoxColumn : DvDataGridColumn
    {
        #region Properties
        public SKColor? CheckColor { get; set; } = null;
        public string Text { get; set; } = null;
        #endregion

        #region DvDataGridCheckBoxColumn
        public DvDataGridCheckBoxColumn(DvDataGrid dg) : base(dg) => CellType = typeof(DvDataGridCheckBoxCell);
        #endregion
    }
    #endregion
    #region class : DvDataGridCheckBoxCell
    public class DvDataGridCheckBoxCell: DvDataGridCell
    {
        #region Constructor
        public DvDataGridCheckBoxCell(DvDataGrid dg, DvDataGridRow row, DvDataGridColumn col) : base(dg, row, col) { }
        #endregion

        #region Override
        #region CellDraw
        protected override void CellDraw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (Grid != null)
            {
                var BoxSize = DvDataGrid.SELECTOR_BOX_WIDTH;
                var BoxColor = Grid.GetInputBoxColor(thm);
                var BorderColor = Grid.GetInputBoxBorderColor(thm);
                var ForeColor = Grid.GetForeColor(thm);
                var InputColor = GetInputBackColor(thm);
                var CheckColor = (Column as DvDataGridCheckBoxColumn)?.CheckColor ?? ForeColor;
                var Text = (Column as DvDataGridCheckBoxColumn)?.Text ?? "";
                var val = (Value as bool?) ?? false;
                thm.DrawCheckBox(canvas, bounds, BoxSize, 
                    InputColor, BorderColor, ForeColor, ForeColor,
                    Text, Grid.FontName, Grid.FontSize, Grid.FontStyle, DvContentAlignment.MiddleCenter, val);
            }

            base.CellDraw(canvas, thm, bounds);
        }
        #endregion

        #region CellMouseClick
        protected override void CellMouseClick(SKRect Bounds, float x, float y)
        {
            if (CollisionTool.Check(Bounds, x, y))
            {
                var val = (Value as bool?) ?? false;
                Value = !val; 
            }
            base.CellMouseClick(Bounds, x, y);
        }
        #endregion'
        #endregion
    }
    #endregion

    #region class : DvDataGridEditTextColumn
    public class DvDataGridEditTextColumn : DvDataGridColumn
    {
        public KeyboardMode KeyboardMode { get; set; } = KeyboardMode.Korea;
        #region DvDataGridCheckBoxColumn
        public DvDataGridEditTextColumn(DvDataGrid dg) : base(dg) => CellType = typeof(DvDataGridEditTextCell);
        #endregion
    }
    #endregion
    #region class : DvDataGridEditTextCell
    public class DvDataGridEditTextCell : DvDataGridCell
    {
        #region Constructor
        public DvDataGridEditTextCell(DvDataGrid dg, DvDataGridRow row, DvDataGridColumn col) : base(dg, row, col) { }
        #endregion

        #region Override 
        #region CellDraw
        protected override void CellDraw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (Grid != null)
            {
                var BoxColor = Grid.GetBoxColor(thm);
                var BorderColor = Grid.GetInputBoxBorderColor(thm);
                var ForeColor = Grid.GetForeColor(thm);
                var InputColor = GetInputBackColor(thm);

                var val = Value as string;
                thm.DrawDataGridInputBox(canvas, bounds);
                thm.DrawText(canvas, bounds, Grid.FontName, Grid.FontSize, Grid.FontStyle, val, ForeColor);
            }

            base.CellDraw(canvas, thm, bounds);
        }
        #endregion

        #region CellMouseClick
        protected override void CellMouseClick(SKRect Bounds, float x, float y)
        {
            var rt = DvDataGridTool.InputBox(Bounds);
            if (CollisionTool.Check(rt, x, y))
            {
                var col = Column as DvDataGridEditTextColumn;
                if (col != null)
                {
                    DvDialogs.Keyboard.ShowKeyboard(col.HeaderText, col.KeyboardMode, Value as string, (result) =>
                    {
                        if(result != null)
                            Value = result;
                    });
                }
            }
            base.CellMouseClick(Bounds, x, y);
        }
        #endregion'
        #endregion
    }
    #endregion

    #region class : DvDataGridEditNumberColumn<T>
    public class DvDataGridEditNumberColumn<T> : DvDataGridColumn where T : struct
    {
        #region Properties
        public string FormatString { get; set; }

        public T? Minimum { get; set; } = null;
        public T? Maximum { get; set; } = null;
        #endregion

        #region Constructor
        public DvDataGridEditNumberColumn(DvDataGrid dg) : base(dg) => CellType = typeof(DvDataGridEditNumberCell<T>);
        #endregion
    }
    #endregion
    #region class : DvDataGridEditNumberCell<T>
    public class DvDataGridEditNumberCell<T> : DvDataGridCell where T : struct
    {
        #region Constructor
        public DvDataGridEditNumberCell(DvDataGrid dg, DvDataGridRow row, DvDataGridColumn col) : base(dg, row, col) { }
        #endregion

        #region Override
        #region CellDraw
        protected override void CellDraw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (Grid != null)
            {
                var BoxColor = Grid.GetBoxColor(thm);
                var BorderColor = Grid.GetInputBoxBorderColor(thm);
                var ForeColor = Grid.GetForeColor(thm);
                var InputColor = GetInputBackColor(thm);

                var col = Column as DvDataGridEditNumberColumn<T>;
                var val = ValueTool.Text<T>((T)Value, col?.FormatString);
                thm.DrawDataGridInputBox(canvas, bounds);
                thm.DrawText(canvas, bounds, Grid.FontName, Grid.FontSize, Grid.FontStyle, val, ForeColor);
            }

            base.CellDraw(canvas, thm, bounds);
        }
        #endregion

        #region CellMouseClick
        protected override void CellMouseClick(SKRect Bounds, float x, float y)
        {
            var rt = DvDataGridTool.InputBox(Bounds);
            var col = Column as DvDataGridEditNumberColumn<T>;
            if (col != null)
            {
                if (CollisionTool.Check(rt, x, y))
                {
                    DvDialogs.Keypad.ShowKeypad<T>(Column.HeaderText, Value as T?, col.Minimum, col.Maximum, (result) =>
                    {
                        if (result.HasValue)
                        {
                            Value = result.Value;
                        }
                    });
                }
            }
            base.CellMouseClick(Bounds, x, y);
        }
        #endregion'
        #endregion
    }
    #endregion

    #region class : DvDataGridEditBoolColumn
    public class DvDataGridEditBoolColumn : DvDataGridColumn 
    {
        #region Properties
        public string OnText { get; set; } = "ON";
        public string OffText { get; set; } = "OFF";
        #endregion

        #region Constructor
        public DvDataGridEditBoolColumn(DvDataGrid dg) : base(dg) => CellType = typeof(DvDataGridEditBoolCell);
        #endregion
    }
    #endregion
    #region class : DvDataGridEditBoolCell
    public class DvDataGridEditBoolCell : DvDataGridCell
    {
        #region Properties
        private bool Animation => Grid != null && Grid.Design != null ? Grid.Design.Animation : false;
        #endregion

        #region Member Variable
        Animation ani = new Animation();
        #endregion

        #region Constructor
        public DvDataGridEditBoolCell(DvDataGrid dg, DvDataGridRow row, DvDataGridColumn col) : base(dg, row, col) { }
        #endregion

        #region Override
        #region CellDraw
        protected override void CellDraw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (Grid != null)
            {
                var BoxColor = Grid.GetBoxColor(thm);
                var BorderColor = Grid.GetInputBoxBorderColor(thm);
                var ForeColor = Grid.GetForeColor(thm);
                var InputColor = GetInputBackColor(thm);

                var col = Column as DvDataGridEditBoolColumn;
                thm.DrawDataGridInputBox(canvas, bounds);

                var val = Value as bool? ?? false;
                if (col != null)
                    Areas2(bounds, (rtOn, rtOff) =>
                    {
                        thm.DrawValueOnOff(canvas,
                         bounds, rtOn, rtOff,
                         ForeColor,
                         col.OnText, col.OffText, Grid.FontName, Grid.FontSize, Grid.FontStyle,
                         val,
                         Animation, ani);
                    });
            }

            base.CellDraw(canvas, thm, bounds);
        }
        #endregion

        #region CellMouseClick
        protected override void CellMouseClick(SKRect Bounds, float x, float y)
        {
            var rt = DvDataGridTool.InputBox(Bounds);
            var col = Column as DvDataGridEditBoolColumn;
            if (col != null)
            {
                var val = Value as bool? ?? false;
                Areas2(rt, (rtOn, rtOff) =>
                {
                    if (CollisionTool.Check(rtOn, x, y) && !val)
                    {
                        Value = true;
                        if (Animation)
                        {
                            ani.Stop();
                            ani.Start(DvDesign.ANI, "On" );
                        }
                    }
                    else if (CollisionTool.Check(rtOff, x, y) && val)
                    {
                        Value = false;
                        if (Animation)
                        {
                            ani.Stop();
                            ani.Start(DvDesign.ANI, "Off");
                        }
                    }
                });
            }
            base.CellMouseClick(Bounds, x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas2
        void Areas2(SKRect rtValue, Action<SKRect, SKRect> act)
        {
            var w = rtValue.Width / 2F;
            var rtOn = Util.FromRect(rtValue.Left, rtValue.Top, w, rtValue.Height);
            var rtOff = Util.FromRect(rtValue.Left + w, rtValue.Top, w, rtValue.Height);
            act(rtOn, rtOff);
        }
        #endregion
        #endregion
    }
    #endregion

    #region class : DvDataGridEditTimeColumn
    public class DvDataGridEditTimeColumn : DvDataGridColumn
    {
        #region Properties
        public DateTimePickerMode Type { get; set; } = DateTimePickerMode.DateTime;
        #endregion

        #region Constructor
        public DvDataGridEditTimeColumn(DvDataGrid dg) : base(dg) => CellType = typeof(DvDataGridEditTimeCell);
        #endregion
    }
    #endregion
    #region class : DvDataGridEditTimeCell
    public class DvDataGridEditTimeCell : DvDataGridCell
    {
        #region Constructor
        public DvDataGridEditTimeCell(DvDataGrid dg, DvDataGridRow row, DvDataGridColumn col) : base(dg, row, col) { }
        #endregion

        #region Override
        #region CellDraw
        protected override void CellDraw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (Grid != null)
            {
                var BoxColor = Grid.GetBoxColor(thm);
                var BorderColor = Grid.GetInputBoxBorderColor(thm);
                var ForeColor = Grid.GetForeColor(thm);
                var InputColor = GetInputBackColor(thm);

                var col = Column as DvDataGridEditTimeColumn;

                var val = (Value as DateTime?) ?? DateTime.Now;
                var s = "";
                if (col.Type == DateTimePickerMode.Date) s = val.ToString("yyyy-MM-dd");
                else if (col.Type == DateTimePickerMode.Time) s = val.ToString("HH:mm:ss");
                else if (col.Type == DateTimePickerMode.DateTime) s = val.ToString("yyyy-MM-dd HH:mm:ss");

                thm.DrawDataGridInputBox(canvas, bounds);
                thm.DrawText(canvas, bounds, Grid.FontName, Grid.FontSize, Grid.FontStyle, s, ForeColor);
            }

            base.CellDraw(canvas, thm, bounds);
        }
        #endregion

        #region CellMouseClick
        protected override void CellMouseClick(SKRect Bounds, float x, float y)
        {
            var rt = DvDataGridTool.InputBox(Bounds);
            var col = Column as DvDataGridEditTimeColumn;
            if (col != null)
            {
                if (CollisionTool.Check(Bounds, x, y))
                {
                    var val = (Value as DateTime?) ?? DateTime.Now;
                    if (col.Type == DateTimePickerMode.DateTime) DvDialogs.DateTimePickerBox.ShowDateTimePicker(col.HeaderText, val, (datetime) => { if (datetime.HasValue) Value = datetime.Value; });
                    if (col.Type == DateTimePickerMode.Date) DvDialogs.DateTimePickerBox.ShowDatePicker(col.HeaderText, val, (datetime) => { if (datetime.HasValue) Value = datetime.Value.Date; });
                    if (col.Type == DateTimePickerMode.Time) DvDialogs.DateTimePickerBox.ShowTimePicker(col.HeaderText, val, (datetime) => { if (datetime.HasValue) Value = datetime.Value; });
                }
            }
            base.CellMouseClick(Bounds, x, y);
        }
        #endregion
        #endregion
    }
    #endregion

    #region class : DvDataGridEditColorColumn
    public class DvDataGridEditColorColumn : DvDataGridColumn
    {
        #region Properties
        public ColorCodeType Code { get; set; } = ColorCodeType.CodeRGB;
        #endregion

        #region Constructor
        public DvDataGridEditColorColumn(DvDataGrid dg) : base(dg) => CellType = typeof(DvDataGridEditColorCell);
        #endregion
    }
    #endregion
    #region class : DvDataGridEditColorCell
    public class DvDataGridEditColorCell : DvDataGridCell
    {
        #region Constructor
        public DvDataGridEditColorCell(DvDataGrid dg, DvDataGridRow row, DvDataGridColumn col) : base(dg, row, col) { }
        #endregion

        #region Override
        #region CellDraw
        protected override void CellDraw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (Grid != null)
            {
                var BoxColor = Grid.GetBoxColor(thm);
                var BorderColor = Grid.GetInputBoxBorderColor(thm);
                var ForeColor = Grid.GetForeColor(thm);
                var InputColor = GetInputBackColor(thm);

                var col = Column as DvDataGridEditColorColumn;

                var val = (Value as SKColor?) ?? SKColors.White;
                var s = ColorTool.GetName(val, col.Code);
                var sz = 14;
                thm.DrawDataGridInputBox(canvas, bounds);

                Util.TextIconBounds(s, Grid.FontName, Grid.FontSize, Grid.FontStyle, 10, bounds,
                   Util.FromRect(0, 0, sz, sz), DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleCenter,
                   (rtIcon, rtText) =>
                   {
                       rtIcon.Offset(0, 1);
                       thm.DrawBox(canvas, rtIcon, val, SKColors.Black, DvRoundType.Rect, BoxStyle.Fill | BoxStyle.Border, thm.Corner);
                       thm.DrawText(canvas, rtText, Grid.FontName, Grid.FontSize, Grid.FontStyle, s, ForeColor);
                   });
            }

            base.CellDraw(canvas, thm, bounds);
        }
        #endregion

        #region CellMouseClick
        protected override void CellMouseClick(SKRect Bounds, float x, float y)
        {
            var rt = DvDataGridTool.InputBox(Bounds);
            var col = Column as DvDataGridEditColorColumn;
            if (col != null)
            {
                var val = (Value as SKColor?) ?? SKColors.White;
                if (CollisionTool.Check(Bounds, x, y))
                {
                    DvDialogs.ColorPickerBox.ShowColorPicker(col.HeaderText, val,
                       (color) =>
                       {
                           if (color.HasValue)
                               Value = color.Value;
                       });
                }
            }
            base.CellMouseClick(Bounds, x, y);
        }
        #endregion
        #endregion
    }
    #endregion

    #region class : DvDataGridEditSelectorColumn
    public class DvDataGridEditSelectorColumn : DvDataGridColumn
    {
        #region Properties
        public List<SelectorItem> Items { get; set; } = new List<SelectorItem>();
        #endregion

        #region Constructor
        public DvDataGridEditSelectorColumn(DvDataGrid dg) : base(dg) => CellType = typeof(DvDataGridEditSelectorCell);
        #endregion
    }
    #endregion
    #region class : DvDataGridEditSelectorCell
    public class DvDataGridEditSelectorCell : DvDataGridCell
    {
        #region Properties
        private bool Animation => Grid != null && Grid.Design != null ? Grid.Design.Animation : false;
        #endregion

        #region Member Variable
        private bool bPrev = false, bNext = false;

        private Animation ani = new Animation();
        #endregion

        #region Constructor
        public DvDataGridEditSelectorCell(DvDataGrid dg, DvDataGridRow row, DvDataGridColumn col) : base(dg, row, col) { }
        #endregion

        #region Override
        #region CellDraw
        protected override void CellDraw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (Grid != null)
            {
                var BoxColor = Grid.GetBoxColor(thm);
                var BorderColor = Grid.GetInputBoxBorderColor(thm);
                var ForeColor = Grid.GetForeColor(thm);
                var BackColor = Grid.GetBackColor();
                var InputColor = GetInputBackColor(thm);

                var col = Column as DvDataGridEditSelectorColumn;

                var val = (Value as SKColor?) ?? SKColors.White;
                thm.DrawDataGridInputBox(canvas, bounds);

                Areas2(bounds, (rtPrev, rtValue2, rtNext, rtItems) =>
                {
                    var sel = col.Items.Where(x => x.Value?.Equals(Value) ?? false).FirstOrDefault();
                    var si = col.Items.IndexOf(sel);
                    thm.DrawSelector(canvas,
                           bounds, rtPrev, rtValue2, rtNext, rtItems,
                           ForeColor, ForeColor, BackColor,
                           Grid.FontName, Grid.FontSize, Grid.FontStyle,
                           12, 5, DvTextIconAlignment.LeftRight,
                           col.Items, false,  DvDirectionHV.Horizon,  DvRoundType.Rect,
                           si,
                           bPrev, bNext, Animation, ani);
                });
            }

            base.CellDraw(canvas, thm, bounds);
        }
        #endregion
        
        #region CellMouseDown
        protected override void CellMouseDown(SKRect Bounds, float x, float y)
        {
            Areas2(Bounds, (rtPrev, rtValue, rtNext, lsv) =>
            {
                if (CollisionTool.Check(rtPrev, x, y)) bPrev = true;
                if (CollisionTool.Check(rtNext, x, y)) bNext = true;
            });
           
            base.CellMouseDown(Bounds, x, y);
        }
        #endregion

        #region CellMouseUp
        protected override void CellMouseUp(SKRect Bounds, float x, float y)
        {
            var col = Column as DvDataGridEditSelectorColumn;

            Areas2(Bounds, (rtPrev, rtValue, rtNext, lsv) =>
            {
                var sel = col.Items.Where(x => x.Value?.Equals(Value) ?? false).FirstOrDefault();
                var si = col.Items.IndexOf(sel);

                #region PrevButton
                if (bPrev)
                {
                    if (CollisionTool.Check(rtPrev, x, y))
                    {
                        if (col.Items.Count > 0)
                        {
                            if (si > 0)
                            {
                                si--;
                                Value = col.Items[si].Value;

                                if (Animation)
                                {
                                    ani.Stop();
                                    ani.Start(DvDesign.ANI2, "Prev");
                                }
                            }
                        }
                        else si = -1;

                    }
                    bPrev = false;
                }
                #endregion
                #region NextButton
                if (bNext)
                {
                    if (CollisionTool.Check(rtNext, x, y))
                    {
                        if (col.Items.Count > 0)
                        {
                            if (si < col.Items.Count - 1)
                            {
                                si++;
                                Value = col.Items[si].Value;

                                if (Animation)
                                {
                                    ani.Stop();
                                    ani.Start(DvDesign.ANI2, "Next");
                                }
                            }
                        }


                    }
                    bNext = false;
                }
                #endregion
            });

            base.CellMouseDown(Bounds, x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas2
        public void Areas2(SKRect rtContent, Action<SKRect, SKRect, SKRect, List<SKRect>> act)
        {
            var btnSize = rtContent.Height;
            var ls = new List<SizeInfo>();
            ls.Add(new SizeInfo(DvSizeMode.Pixel, btnSize));
            ls.Add(new SizeInfo(DvSizeMode.Percent, 100F));
            ls.Add(new SizeInfo(DvSizeMode.Pixel, btnSize));

            var col = Column as DvDataGridEditSelectorColumn;
            var sel = col.Items.Where(x => x.Value?.Equals(Value) ?? false).FirstOrDefault();
            var si = col.Items.IndexOf(sel);

            var rts = Util.DevideSizeH(rtContent, ls);
            var rtPrev = rts[0];
            var rtValue = rts[1];
            var rtNext = rts[2];

            var lsv = new List<SKRect>();
            var sx = rtValue.Left;

            if (Animation && ani.IsPlaying)
            {
                var v = Convert.ToSingle(ani.Value(AnimationAccel.DCL, rtValue.Width, 0));
                sx = rtValue.Left + -(si * rtValue.Width) + (ani.Variable == "Prev" ? -v : v);
            }
            else
            {
                sx = rtValue.Left + -(si * rtValue.Width);
            }

            foreach (var v in col.Items)
            {
                lsv.Add(Util.FromRect(sx, rtValue.Top, rtValue.Width, rtValue.Height));
                sx += rtValue.Width;
            }

            act(rtPrev, rtValue, rtNext, lsv);
        }
        #endregion
        #endregion
    }
    #endregion
    #endregion

    #region classes : SummaryCell
    #region class : DvDataGridSummaryLabelCell
    public class DvDataGridSummaryLabelCell : DvDataGridSummaryCell
    {
        public string Text { get; set; }
        public DvDataGridSummaryLabelCell(DvDataGrid dg) : base(dg) { }

        #region CellDraw
        protected override void CellDraw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (Grid != null)
            {
                var textColor = CellTextColor ?? Grid.GetForeColor(thm);
                var s = Text ?? "";
                thm.DrawText(canvas, bounds, Grid.FontName, Grid.FontSize, Grid.FontStyle, s, textColor);
            }

            base.CellDraw(canvas, thm, bounds);
        }
        #endregion
    }
    #endregion
    #region class : DvDataGridSummarySumCell
    public class DvDataGridSummarySumCell : DvDataGridSummaryCell
    {
        public string FormatString { get; set; }
        public decimal Value { get; set; }
        public DvDataGridSummarySumCell(DvDataGrid dg) : base(dg) { }

        #region CellDraw
        protected override void CellDraw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (Grid != null)
            {
                var Format = FormatString;
                var textColor = CellTextColor ?? Grid.GetForeColor(thm);
                var s = ValueTool.Text<decimal>(Value, Format);
                thm.DrawText(canvas, bounds, Grid.FontName, Grid.FontSize, Grid.FontStyle, s, textColor);
            }

            base.CellDraw(canvas, thm, bounds);
        }
        #endregion
        #region Calculate
        public override void Calculate()
        {
            var ls = Grid.GetRows();
            Value = ls.Count == 0 ? 0 : ls.Select(x => Convert.ToDecimal(x.Cells[ColIndex].Value)).Sum();
            base.Calculate();
        }
        #endregion
    }
    #endregion
    #region class : DvDataGridSummaryAverageCell
    public class DvDataGridSummaryAverageCell : DvDataGridSummaryCell
    {
        public string FormatString { get; set; }
        public decimal Value { get; set; }
        public DvDataGridSummaryAverageCell(DvDataGrid dg) : base(dg) { }

        #region CellDraw
        protected override void CellDraw(SKCanvas canvas, DvTheme thm, SKRect bounds)
        {
            if (Grid != null)
            {
                var Format = FormatString;
                var textColor = CellTextColor ?? Grid.GetForeColor(thm);
                var s = ValueTool.Text<decimal>(Value, Format);
                thm.DrawText(canvas, bounds, Grid.FontName, Grid.FontSize, Grid.FontStyle, s, textColor);
            }

            base.CellDraw(canvas, thm, bounds);
        }
        #endregion
        #region Calculate
        public override void Calculate()
        {
            var ls = Grid.GetRows();
            Value = ls.Count == 0 ? 0 : ls.Select(x => Convert.ToDecimal(x.Cells[ColIndex].Value)).Average();
            base.Calculate();
        }
        #endregion
    }
    #endregion
    #endregion

    #region classes : EventArgs
    #region class : CellButtonClickEventArgs
    public class CellButtonClickEventArgs : EventArgs
    {
        public DvDataGridCell Cell { get; private set; }
        public CellButtonClickEventArgs(DvDataGridCell Cell)
        {
            this.Cell = Cell;
        }
    }
    #endregion
    #region class : ColumnMouseEventArgs
    public class ColumnMouseEventArgs : EventArgs
    {
        public DvDataGridColumn Column { get; private set; }
        public float MouseX { get; private set; }
        public float MouseY { get; private set; }

        public ColumnMouseEventArgs(DvDataGridColumn column, float x, float y)
        {
            this.Column = column;
            this.MouseX = x;
            this.MouseY = y;
        }
    }
    #endregion
    #region class : CellMouseEventArgs
    public class CellMouseEventArgs : EventArgs
    {
        public DvDataGridCell Cell { get; private set; }
        public float MouseX { get; private set; }
        public float MouseY { get; private set; }
        public CellMouseEventArgs(DvDataGridCell Cell, float x, float y)
        {
            this.Cell = Cell;
            this.MouseX = x;
            this.MouseY = y;
        }
    }
    #endregion
    #region class : CellValueChangedEventArgs
    public class CellValueChangedEventArgs : EventArgs
    {
        public DvDataGridCell Cell { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }
        public CellValueChangedEventArgs(DvDataGridCell Cell, object OldValue, object NewValue)
        {
            this.Cell = Cell;
            this.OldValue = OldValue;
            this.NewValue = NewValue;
        }
    }
    #endregion
    #region class : RowDoubleClickEventArgs
    public class RowDoubleClickEventArgs : EventArgs
    {
        public DvDataGridRow Row { get; private set; }
        public float MouseX { get; private set; }
        public float MouseY { get; private set; }
        public RowDoubleClickEventArgs(DvDataGridRow Row, float x, float y)
        {
            this.Row = Row;
            this.MouseX = x;
            this.MouseY = y;
        }
    }
    #endregion
    #endregion

    #region class : Tool
    public class DvDataGridTool
    {
        public static SKRect FilterBox(SKRect rt)
        {
            var rtv = Util.FromRect(rt);
            rtv.Inflate(-3F, -3F);
            rtv.Right--;
            rtv.Bottom--;
            return rtv;
        }

        public static SKRect InputBox(SKRect rt)
        {
            var rtv = Util.FromRect(rt);
            rtv.Inflate(-2F, -1F);
            //rtv.Left++;
            rtv.Top++;
            return rtv;
        }
    }
    #endregion
}

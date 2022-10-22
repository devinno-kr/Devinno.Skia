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
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    #region enum : DvDataGridSelectionMode
    public enum DvDataGridSelectionMode { NONE, SELECTOR, SINGLE, MULTI }
    #endregion
    #region enum : DvDataGridColumnSortState
    public enum DvDataGridColumnSortState { NONE, ASC, DESC };
    #endregion

    #region interface : IDvDataGridColumn
    public interface IDvDataGridColumn
    {
        string Name { get; set; }
        string GroupName { get; set; }
        string HeaderText { get; set; }

        SizeMode SizeMode { get; set; }
        decimal Width { get; set; }

        bool UseFilter { get; set; }
        string FilterText { get; set; }

        bool UseSort { get; set; }
        DvDataGridColumnSortState SortState { get; set; }
        int SortOrder { get; }

        SKColor? TextColor { get; set; }
        Type CellType { get; set; }
        bool Fixed { get; set; }

        void Draw(DvTheme Theme, SKCanvas g, SKRect ColumnBounds);
        void MouseDown(SKRect ColumnBounds, int x, int y);
        void MouseUp(SKRect ColumnBounds, int x, int y);
    }
    #endregion
    #region interface : IDvDataGridCell
    public interface IDvDataGridCell
    {
        string Name { get; set; }

        int ColSpan { get; set; }
        int RowSpan { get; set; }
        int ColumnIndex { get; }
        int RowIndex { get; }

        bool Visible { get; set; }
        bool Enabled { get; set; }
        object Value { get; set; }
        object Tag { get; set; }

        SKColor? CellBackColor { get; set; }
        SKColor? SelectedCellBackColor { get; set; }

        DvDataGrid Grid { get; }
        DvDataGridRow Row { get; }
        IDvDataGridColumn Column { get; }

        void Draw(DvTheme Theme, SKCanvas g, SKRect ColumnBounds);
        void MouseDown(SKRect ColumnBounds, int x, int y);
        void MouseUp(SKRect ColumnBounds, int x, int y);
        void MouseDoubleClick(SKRect ColumnBounds, int x, int y);
    }
    #endregion
    
    #region class : _DGSearch_
    class _DGSearch_
    {
        public int Height { get; set; }
        public int Sum { get; set; }
        public DvDataGridRow Row { get; set; }

        public int Top => Sum;
        public int Bottom => Top + Height;
    }
    #endregion
    #region classes : EventArgs
    #region class : CellButtonClickEventArgs
    public class CellButtonClickEventArgs : EventArgs
    {
        public IDvDataGridCell Cell { get; private set; }
        public CellButtonClickEventArgs(IDvDataGridCell Cell)
        {
            this.Cell = Cell;
        }
    }
    #endregion
    #region class : ColumnMouseEventArgs
    public class ColumnMouseEventArgs : EventArgs
    {
        public IDvDataGridColumn Column { get; private set; }
        public int MouseX { get; private set; }
        public int MouseY { get; private set; }

        public ColumnMouseEventArgs(IDvDataGridColumn column, int x, int y)
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
        public IDvDataGridCell Cell { get; private set; }
        public int MouseX { get; private set; }
        public int MouseY { get; private set; }
        public CellMouseEventArgs(IDvDataGridCell Cell, int x, int y)
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
        public IDvDataGridCell Cell { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }
        public CellValueChangedEventArgs(IDvDataGridCell Cell, object OldValue, object NewValue)
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
        public int MouseX { get; private set; }
        public int MouseY { get; private set; }
        public RowDoubleClickEventArgs(DvDataGridRow Row, int x, int y)
        {
            this.Row = Row;
            this.MouseX = x;
            this.MouseY = y;
        }
    }
    #endregion
    #endregion
    #region class : DvDataGridTool
    internal class DvDataGridTool
    {
        #region GetText
        public static string GetText(object Value)
        {
            var s = (string)null;
            if (Value is string) s = (string)Value;
            else if (Value != null) s = Value.ToString();
            return s;
        }
        #endregion
    }
    #endregion

    #region class : DvDataGridColumn
    public class DvDataGridColumn : IDvDataGridColumn
    {
        #region Properties
        public DvDataGrid Grid { get; private set; }

        public string Name { get; set; }
        public string GroupName { get; set; }
        public string HeaderText { get; set; }

        public SizeMode SizeMode { get; set; }
        public decimal Width { get; set; }

        public bool UseFilter { get; set; }
        public string FilterText { get; set; }

        public bool UseSort { get; set; }
        public DvDataGridColumnSortState SortState { get; set; }
        public int SortOrder { get; private set; }

        public SKColor? TextColor { get; set; }
        public Type CellType { get; set; }
        public bool Fixed { get; set; }
        #endregion

        #region Constructor
        public DvDataGridColumn(DvDataGrid dataGrid)
        {
            Grid = dataGrid;
            CellType = typeof(DvDataGridLabelCell);
        }
        #endregion

        #region Draw
        public virtual void Draw(DvTheme Theme, SKCanvas Canvas, SKRect Bounds)
        {
            if (Grid != null)
            {
                #region Color
                var nc = Grid;
                var TextColor = this.TextColor ?? Theme.ForeColor;
                var BoxColor = nc.GetBoxColor(Theme);
                var ColumnColor = nc.GetColumnColor(Theme);
                var RowColor = nc.GetRowColor(Theme);
                var SelectRowColor = nc.GetSelectedRowColor(Theme);
                var SummaryRowColor = nc.GetSummaryRowColor(Theme);

                var FontName = nc.FontName;
                var FontSize = nc.FontSize;
                #endregion

                var rt = Bounds;
                Util.DrawText(Canvas, HeaderText, FontName, FontSize, DvFontStyle.Normal, TextColor, rt);

                if (UseSort)
                {
                    var sizewh = Convert.ToInt32(12);
                    var rtSort = Util.FromRect(Bounds.Right - Bounds.Height, Bounds.Top, Bounds.Height, Bounds.Height);
                    Util.DrawIconFA(Canvas, "fa-sort", FontSize, TextColor.BrightnessTransmit(-0.7F), rtSort);

                    switch (SortState)
                    {
                        case DvDataGridColumnSortState.ASC: Util.DrawIconFA(Canvas, "fa-sort-up", FontSize, TextColor, rtSort); break;
                        case DvDataGridColumnSortState.DESC: Util.DrawIconFA(Canvas, "fa-sort-down", FontSize, TextColor, rtSort); break;
                    }
                }
            }
        }
        #endregion
        #region MouseDown
        public virtual void MouseDown(SKRect Bounds, int x, int y)
        {
            bool b = false;
            if (UseSort)
            {
                var sizewh = Convert.ToInt32(12);
                var rtSort = Util.FromRect(Bounds.Right - Bounds.Height, Bounds.Top, Bounds.Height, Bounds.Height);

                if (CollisionTool.Check(rtSort, x, y))
                {
                    switch (SortState)
                    {
                        case DvDataGridColumnSortState.NONE:
                            SortState = DvDataGridColumnSortState.ASC;
                            SortOrder = (SortState != DvDataGridColumnSortState.NONE ? Grid.Columns.Where(x => x.UseSort && x.SortState != DvDataGridColumnSortState.NONE).Count() : 1000);
                            Grid.InvokeSortChanged();
                            break;
                        case DvDataGridColumnSortState.ASC:
                            SortState = DvDataGridColumnSortState.DESC;
                            SortOrder = (SortState != DvDataGridColumnSortState.NONE ? Grid.Columns.Where(x => x.UseSort && x.SortState != DvDataGridColumnSortState.NONE).Count() : 1000);
                            Grid.InvokeSortChanged();
                            break;
                        case DvDataGridColumnSortState.DESC:
                            SortState = DvDataGridColumnSortState.NONE;
                            SortOrder = (SortState != DvDataGridColumnSortState.NONE ? Grid.Columns.Where(x => x.UseSort && x.SortState != DvDataGridColumnSortState.NONE).Count() : 1000);
                            Grid.InvokeSortChanged();
                            break;
                    }
                    b = true;
                }
            }
        }
        #endregion
        #region MouseUp
        public virtual void MouseUp(SKRect Bounds, int x, int y)
        {
        }
        #endregion
    }
    #endregion
    #region class : DvDataGridRow
    public class DvDataGridRow
    {
        public DvDataGrid Grid { get; private set; }

        int nRowHeight;
        public int RowHeight
        {
            get => nRowHeight;
            set
            {
                if (nRowHeight != value)
                {
                    nRowHeight = value;
                }
            }
        }

        public List<IDvDataGridCell> Cells { get; private set; } = new List<IDvDataGridCell>();
        public object Source { get; set; }
        public object Tag { get; set; }
        public bool Selected { get; set; }

        public DvDataGridRow(DvDataGrid Grid)
        {
            this.Grid = Grid;
            this.nRowHeight = Grid.RowHeight;
        }
    }
    #endregion
    #region class : DvDataGridCell
    public abstract class DvDataGridCell : IDvDataGridCell
    {
        #region Properties
        public string Name { get; set; }

        public int ColSpan { get; set; } = 1;
        public int RowSpan { get; set; } = 1;
        public int ColumnIndex { get { return Row.Cells.IndexOf(this); } }
        public int RowIndex { get { return Grid.Rows.IndexOf(Row); } }

        public bool Visible { get; set; } = true;
        public bool Enabled { get; set; } = true;
        public object Tag { get; set; }

        public SKColor? CellBackColor { get; set; }
        public SKColor? SelectedCellBackColor { get; set; }
        public SKColor? CellTextColor { get; set; }

        public DvDataGrid Grid { get; private set; }
        public DvDataGridRow Row { get; private set; }
        public IDvDataGridColumn Column { get; private set; }

        protected PropertyInfo ValueInfo { get; }
        public object Value
        {
            get => ValueInfo?.GetValue(Row.Source);
            set
            {
                if (ValueInfo != null && ValueInfo.CanWrite)
                {
                    ValueInfo.SetValue(Row.Source, value);
                }
            }
        }
        #endregion

        #region Constructor
        public DvDataGridCell(DvDataGrid Grid, DvDataGridRow Row, IDvDataGridColumn Column)
        {
            this.Grid = Grid;
            this.Row = Row;
            this.Column = Column;

            if (Grid.DataType != null && !(Column is DvDataGridButtonColumn)) 
                ValueInfo = Grid.DataType.GetProperty(Column.Name);
        }
        #endregion

        #region Virtual Method
        #region Draw
        public virtual void Draw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds)
        {
            if (Grid != null)
            {
                var CellTextColor = this.CellTextColor ?? Grid.GetForeColor(Theme);
                var CellBackColor = this.CellBackColor ?? Grid.GetRowColor(Theme);
                var SelectedCellBackColor = this.SelectedCellBackColor ?? Grid.GetSelectedRowColor(Theme);
                var BorderColor = Theme.GetBorderColor(CellBackColor, Grid.ParentContainer.GetBackColor());

                var rt = CellBounds;
                var bg = Grid.GetBoxColor(Theme);
                var c = Row.Selected ? SelectedCellBackColor : CellBackColor;
                var border = Theme.GetBorderColor(c, Grid.ParentContainer.GetBackColor());
                Theme.DrawBox(Canvas, rt, c, border, RoundType.Rect, BoxStyle.Fill);

                var inf = new DvDataGridCellDrawInfo();
                CellDraw(Theme, Canvas, CellBounds, inf);

                using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                {
                    #region Bevel
                    if(Grid.Bevel && inf.Bevel)
                    {
                        var n = 0.5F;
                        var pts = new SKPoint[] {
                                            new SKPoint(rt.Right - n, rt.Top + 1 + n),
                                            new SKPoint(rt.Left + 1 + n, rt.Top + 1 + n),
                                            new SKPoint(rt.Left + 1 + n, rt.Bottom - n)
                                        };
                        p.Color = c.BrightnessTransmit(Theme.DataGridRowBevelBright);
                        p.IsStroke = true;
                        p.StrokeWidth = 1;
                        Canvas.DrawLine(pts[0], pts[1], p);
                        Canvas.DrawLine(pts[1], pts[2], p);
                    }
                    #endregion
                    #region Border
                    if(inf.Border)
                    {
                        p.Color = BorderColor;
                        p.IsStroke = true;
                        p.StrokeWidth = 1;
                        Canvas.DrawLine(rt.Left, rt.Top + 0.5F, rt.Right, rt.Top + 0.5F, p);
                        Canvas.DrawLine(rt.Left + 0.5F, rt.Top, rt.Left + 0.5F, rt.Bottom, p);
                    }
                    #endregion
                }
            }
        }
        #endregion
        #region CellDraw
        public virtual void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds, DvDataGridCellDrawInfo Info) { }
        #endregion
        #region MouseDown / MouseUp
        public virtual void MouseDown(SKRect CellBounds, int x, int y)
        {
            if (Enabled && CollisionTool.Check(CellBounds, x, y))
                CellMouseDown(CellBounds, x, y);
        }
        public virtual void MouseUp(SKRect CellBounds, int x, int y)
        {
            if (Enabled)
                CellMouseUp(CellBounds, x, y);
        }
        public virtual void MouseDoubleClick(SKRect CellBounds, int x, int y)
        {
            if (Enabled && CollisionTool.Check(CellBounds, x, y))
                CellMouseDoubleClick(CellBounds, x, y);
        }
        #endregion
        #region Cell MouseDown / MouseUp
        public virtual void CellMouseDown(SKRect CellBounds, int x, int y) { }
        public virtual void CellMouseUp(SKRect CellBounds, int x, int y) { }
        public virtual void CellMouseDoubleClick(SKRect CellBounds, int x, int y) { }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridSummaryRow
    public class DvDataGridSummaryRow
    {
        public DvDataGrid Grid { get; private set; }
        public List<DvDataGridSummaryCell> Cells { get; private set; } = new List<DvDataGridSummaryCell>();
        public object Tag { get; set; }

        public DvDataGridSummaryRow(DvDataGrid Grid)
        {
            this.Grid = Grid;
        }
    }
    #endregion
    #region class : DvDataGridSummaryCell
    public class DvDataGridSummaryCell
    {
        #region Properties
        public int RowIndex { get { return Grid.SummaryRows.IndexOf(Row); } }
        public object Tag { get; set; }

        public int ColumnIndex { get; set; } = 0;
        public int ColumnSpan { get; set; } = 1;

        public bool Visible { get; set; } = true;

        public SKColor? CellBackColor { get; set; }
        public SKColor? CellTextColor { get; set; }

        public DvDataGrid Grid { get; private set; }
        public DvDataGridSummaryRow Row { get; private set; }
        #endregion

        #region Constructor
        public DvDataGridSummaryCell(DvDataGrid Grid, DvDataGridSummaryRow Row)
        {
            this.Grid = Grid;
            this.Row = Row;
        }
        #endregion

        #region Virtual Method
        #region Draw
        public virtual void Draw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds)
        {
            var rt = CellBounds;
            var CellTextColor = this.CellTextColor ?? Grid.GetForeColor(Theme);
            var CellBackColor = this.CellBackColor ?? Grid.GetSummaryRowColor(Theme);
            var BorderColor = Theme.GetBorderColor(CellBackColor, Grid.ParentContainer.GetBackColor());

            Theme.DrawBox(Canvas, rt, CellBackColor, BorderColor, RoundType.Rect, BoxStyle.Fill);

            using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
            {
                #region Bevel
                if(Grid.Bevel)
                {
                    var n = 0.5F;
                    var pts = new SKPoint[] {
                                            new SKPoint(rt.Right - n, rt.Top + 1 + n),
                                            new SKPoint(rt.Left + 1 + n, rt.Top + 1 + n),
                                            new SKPoint(rt.Left + 1 + n, rt.Bottom - n)
                                        };
                    p.Color = Theme.GetInBevelColor(CellBackColor);
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
            }

            CellDraw(Theme, Canvas, CellBounds);
        }
        #endregion
        #region CellDraw
        public virtual void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds) { }
        #endregion
        #region Calculation
        public virtual void Calculation() { }
        #endregion
        #endregion
    }
    #endregion
  
    #region class : DvDataGridTextFormatColumn
    public class DvDataGridTextFormatColumn : DvDataGridColumn
    {
        #region Properties
        public string Format { get; set; }
        #endregion
        #region Constructor
        public DvDataGridTextFormatColumn(DvDataGrid dataGrid) : base(dataGrid)
        {
            CellType = typeof(DvDataGridLabelCell);
        }
        #endregion
        #region Method
        #region GetText
        public string GetText(object Value)
        {
            var s = (string)null;
            if (string.IsNullOrWhiteSpace(Format)) s = (Value != null) ? Value.ToString() : null;
            else
            {
                if (Value is string) s = (string)Value;
                else if (Value is byte) s = (!string.IsNullOrWhiteSpace(Format) ? ((byte)Value).ToString(Format) : ((byte)Value).ToString());
                else if (Value is short) s = (!string.IsNullOrWhiteSpace(Format) ? ((short)Value).ToString(Format) : ((short)Value).ToString());
                else if (Value is ushort) s = (!string.IsNullOrWhiteSpace(Format) ? ((ushort)Value).ToString(Format) : ((ushort)Value).ToString());
                else if (Value is int) s = (!string.IsNullOrWhiteSpace(Format) ? ((int)Value).ToString(Format) : ((int)Value).ToString());
                else if (Value is uint) s = (!string.IsNullOrWhiteSpace(Format) ? ((uint)Value).ToString(Format) : ((uint)Value).ToString());
                else if (Value is long) s = (!string.IsNullOrWhiteSpace(Format) ? ((long)Value).ToString(Format) : ((long)Value).ToString());
                else if (Value is ulong) s = (!string.IsNullOrWhiteSpace(Format) ? ((ulong)Value).ToString(Format) : ((ulong)Value).ToString());
                else if (Value is float) s = (!string.IsNullOrWhiteSpace(Format) ? ((float)Value).ToString(Format) : ((float)Value).ToString());
                else if (Value is double) s = (!string.IsNullOrWhiteSpace(Format) ? ((double)Value).ToString(Format) : ((double)Value).ToString());
                else if (Value is decimal) s = (!string.IsNullOrWhiteSpace(Format) ? ((decimal)Value).ToString(Format) : ((decimal)Value).ToString());
                else if (Value is DateTime) s = (!string.IsNullOrWhiteSpace(Format) ? ((DateTime)Value).ToString(Format) : ((DateTime)Value).ToString());
                else if (Value is Enum) s = Value.ToString();
            }
            return s;
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridTextConverterColumn
    public class DvDataGridTextConverterColumn : DvDataGridColumn
    {
        #region Properties
        public Func<object, string> Converter { get; set; }
        #endregion
        #region Constructor
        public DvDataGridTextConverterColumn(DvDataGrid dataGrid) : base(dataGrid)
        {
            CellType = typeof(DvDataGridLabelCell);
        }
        #endregion
        #region Method
        #region GetText
        public string GetText(object Value)
        {
            var s = (string)null;
            if (Converter != null) s = Converter(Value);
            else if (Value != null) s = Value.ToString();
            return s;
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridLampColumn
    public class DvDataGridLampColumn : DvDataGridColumn
    {
        #region Properties
        public SKColor? OnColor { get; set; } = null;
        public SKColor? OffColor { get; set; } = null;
        #endregion
        #region Constructor
        public DvDataGridLampColumn(DvDataGrid dataGrid) : base(dataGrid)
        {
            CellType = typeof(DvDataGridLampCell);
        }
        #endregion
    }
    #endregion
    #region class : DvDataGridButtonColumn
    public class DvDataGridButtonColumn : DvDataGridColumn
    {
        #region Properties
        public string Text { get; set; }
        public string IconString { get; set; } = null;
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        #endregion
        #region Constructor
        public DvDataGridButtonColumn(DvDataGrid dataGrid) : base(dataGrid)
        {
            CellType = typeof(DvDataGridButtonCell);
        }
        #endregion
    }
    #endregion
    #region class : DvDataGridImageColumn
    public class DvDataGridImageColumn : DvDataGridColumn
    {
        #region Properties
        public PictureScaleMode ScaleMode { get; set; }
        #endregion
        #region Constructor
        public DvDataGridImageColumn(DvDataGrid dataGrid) : base(dataGrid)
        {
            ScaleMode = PictureScaleMode.Strech;
            CellType = typeof(DvDataGridImageCell);
        }
        #endregion
    }
    #endregion
    #region class : DvDataGridCheckBoxColumn
    public class DvDataGridCheckBoxColumn : DvDataGridColumn
    {
        #region Properties
        public SKColor? BoxColor { get; set; }
        public SKColor? CheckColor { get; set; }
        #endregion
        #region Constructor
        public DvDataGridCheckBoxColumn(DvDataGrid dataGrid) : base(dataGrid)
        {
            CellType = typeof(DvDataGridCheckBoxCell);
        }
        #endregion
    }
    #endregion
    #region class : DvDataGridComboBoxColumn
    public class DvDataGridComboBoxColumn : DvDataGridColumn
    {
        #region Properties
        public int ButtonWidth { get; set; } = 40;
        public int MaximumViewCount { get; set; } = 8;
        public int ItemHeight { get; set; } = 30;
        public int IconSize { get; set; } = 15;
        public int IconGap { get; set; } = 8;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;
        public List<TextIconItem> Items { get; set; }
        #endregion
        #region Constructor
        public DvDataGridComboBoxColumn(DvDataGrid dataGrid) : base(dataGrid)
        {
            Items = new List<TextIconItem>();
            CellType = typeof(DvDataGridComboBoxCell);
            ItemHeight = Grid.RowHeight;
        }
        #endregion
    }
    #endregion
    #region class : DvDataGridEditTextColumn
    public class DvDataGridEditTextColumn : DvDataGridColumn 
    {
        #region Properties
        public KeyboardMode Mode { get; set; } =  KeyboardMode.Korea;
        #endregion
        #region Constructor
        public DvDataGridEditTextColumn(DvDataGrid dataGrid) : base(dataGrid)
        {
            CellType = typeof(DvDataGridEditTextCell);
        }
        #endregion
    }
    #endregion
    #region class : DvDataGridEditNumberColumn
    public class DvDataGridEditNumberColumn<T> : DvDataGridColumn where T : struct
    {
        #region Properties
        public T? Maximum { get; set; } = null;
        public T? Minimum { get; set; } = null;
        public string Format { get; set; } = null;
        #endregion
        #region Constructor
        public DvDataGridEditNumberColumn(DvDataGrid dataGrid) : base(dataGrid)
        {
            CellType = typeof(DvDataGridEditNumberCell<T>);
        }
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
        public DvDataGridEditBoolColumn(DvDataGrid dataGrid) : base(dataGrid)
        {
            CellType = typeof(DvDataGridEditBoolCell);
        }
        #endregion
    }
    #endregion
    #region class : DvDataGridDateTimePickerColumn
    public class DvDataGridDateTimePickerColumn : DvDataGridColumn
    {
        #region Properties
        public string Format { get; set; }
        public DateTimePickerType PickerMode { get; set; }
        #endregion
        #region Constructor
        public DvDataGridDateTimePickerColumn(DvDataGrid dataGrid) : base(dataGrid)
        {
            CellType = typeof(DvDataGridEditDateTimeCell);
            PickerMode = DateTimePickerType.DateTime;
        }
        #endregion
    }
    #endregion
    #region class : DvDataGridColorPickerColumn
    public class DvDataGridColorPickerColumn : DvDataGridColumn
    {
        #region Properties
        public ColorCodeType CodeType { get; set; } = ColorCodeType.CodeRGB;
        #endregion
        #region Constructor
        public DvDataGridColorPickerColumn(DvDataGrid dataGrid) : base(dataGrid)
        {
            CellType = typeof(DvDataGridEditColorCell);
        }
        #endregion
    }
    #endregion

    #region class : DvDataGridLabelCell
    public class DvDataGridLabelCell : DvDataGridCell
    {
        #region Constructor
        public DvDataGridLabelCell(DvDataGrid Grid, DvDataGridRow Row, IDvDataGridColumn Column) : base(Grid, Row, Column)
        {
        }
        #endregion

        #region Override
        #region CellDraw
        public override void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds, DvDataGridCellDrawInfo Info)
        {
            var CellTextColor = this.CellTextColor ?? Grid.GetForeColor(Theme);
            var CellBackColor = this.CellBackColor ?? Grid.GetRowColor(Theme);
            var SelectedCellBackColor = this.SelectedCellBackColor ?? Grid.GetSelectedRowColor(Theme);
            var BorderColor = Theme.GetBorderColor(CellTextColor, Grid.ParentContainer.GetBackColor());

            var nc = Grid;
            var s = "";
            var FontName = nc.FontName;
            var FontSize = nc.FontSize;
            var val = Value;

            if (Column is DvDataGridTextFormatColumn) s = ((DvDataGridTextFormatColumn)Column).GetText(val);
            else if (Column is DvDataGridTextConverterColumn) s = ((DvDataGridTextConverterColumn)Column).GetText(val);
            else s = DvDataGridTool.GetText(val);

            if (!string.IsNullOrWhiteSpace(s))
                Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, CellTextColor, CellBounds);

            base.CellDraw(Theme, Canvas, CellBounds, Info);
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridLampCell
    public class DvDataGridLampCell : DvDataGridCell
    {
        #region Properties
        public SKColor? OnColor { get; set; }
        public SKColor? OffColor { get; set; }
        #endregion

        #region Constructor
        public DvDataGridLampCell(DvDataGrid Grid, DvDataGridRow Row, IDvDataGridColumn Column) : base(Grid, Row, Column)
        {
            if (Column is DvDataGridLampColumn)
            {
                var c = Column as DvDataGridLampColumn;
                this.OnColor = c.OnColor;
                this.OffColor = c.OffColor;
            }
        }
        #endregion

        #region Override
        #region CellDraw
        public override void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds, DvDataGridCellDrawInfo Info)
        {
            var ov = Value;
            var v = ov is bool ? (bool)ov : false;

            #region Color
            var OnColor = this.OnColor ?? Theme.LampOnColor;
            var OffColor = this.OffColor ?? Theme.LampOffColor;
            var vc = v ? OnColor : OffColor;
            var CellTextColor = this.CellTextColor ?? Grid.GetForeColor(Theme);
            var CellBackColor = this.CellBackColor ?? Grid.GetRowColor(Theme);
            var SelectedCellBackColor = this.SelectedCellBackColor ?? Grid.GetSelectedRowColor(Theme);
            var BorderColor = Theme.GetBorderColor(vc, vc);
            #endregion
            #region Out 
            {
                var c = Row.Selected ? SelectedCellBackColor : CellBackColor;
                var rt = Util.INT(CellBounds);
                rt.Inflate(-3, -3);
                Theme.DrawBox(Canvas, rt, c, BorderColor, RoundType.Round, BoxStyle.GradientV_R);
            }
            #endregion
            #region Lamp 
            {
                var c = v ? OnColor : OffColor;
                var rt = Util.INT(CellBounds);
                rt.Inflate(-5, -5);
                Theme.DrawBox(Canvas, rt, c, BorderColor, RoundType.Round, BoxStyle.Border | BoxStyle.GradientV | BoxStyle.InBevel);

                using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                {
                    using (var mf = SKImageFilter.CreateBlur(3, 3))
                    {
                        p.ImageFilter = mf;

                        var nh = 3;
                        var nw = 3;
                        var rtv = new SKRoundRect(new SKRect(rt.Left + nw, rt.Top + nh, rt.Right - nw, rt.Top + (nh + 3)), Theme.Corner);

                        float h, s, b;
                        c.ToHsv(out h, out s, out b);
                        
                        var a = Convert.ToByte(MathTool.Constrain(b * 2.55, 0, 255));
                        if(v)  a = Convert.ToByte(MathTool.Constrain(a * 1.2, 0, 255));
                        else a= Convert.ToByte(MathTool.Constrain(a * 0.5, 0, 255));
                     
                        p.IsStroke = false;
                        p.Color = Util.FromArgb(a, SKColors.White);
                        Canvas.DrawRoundRect(rtv, p);

                        p.ImageFilter = null;
                    }
                }
            }
            #endregion
            base.CellDraw(Theme, Canvas, CellBounds, Info);
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridButtonCell
    public class DvDataGridButtonCell : DvDataGridCell
    {
        #region Properties
        #endregion
        #region Member Variable
        private bool bDown = false;
        #endregion
        #region Constructor
        public DvDataGridButtonCell(DvDataGrid Grid, DvDataGridRow Row, DvDataGridColumn Column) : base(Grid, Row, Column)
        {
        }
        #endregion
        #region Override
        #region CellDraw
        public override void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds, DvDataGridCellDrawInfo Info)
        {
            #region Set
            var CellTextColor = this.CellTextColor ?? Grid.GetForeColor(Theme);
            var CellBackColor = this.CellBackColor ?? Grid.GetRowColor(Theme);
            var SelectedCellBackColor = this.SelectedCellBackColor ?? Grid.GetSelectedRowColor(Theme);
            var BorderColor = Theme.GetBorderColor(CellTextColor, Grid.ParentContainer.GetBackColor());
            var ButtonColor = Row.Selected ? SelectedCellBackColor : CellBackColor;

            var nc = Grid;
            var FontName = nc.FontName;
            var FontSize = nc.FontSize;
            #endregion
            #region Draw
            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
            {
                var rt = CellBounds;
                var rtText = CellBounds;

                var cF = bDown ? ButtonColor.BrightnessTransmit(Theme.DownBrightness) : ButtonColor;
                var cB = bDown ? BorderColor.BrightnessTransmit(Theme.DownBrightness) : BorderColor;
                var cT = bDown ? CellTextColor.BrightnessTransmit(Theme.DownBrightness) : CellTextColor;
                var cL = Util.FromArgb(Theme.OutBevelAlpha, SKColors.White);

                if (!bDown) Theme.DrawBox(Canvas, rt, cF, cB, RoundType.Rect, BoxStyle.GradientV);
                else Theme.DrawBox(Canvas, rt, cF, cB, RoundType.Rect, BoxStyle.Fill | BoxStyle.InShadow);

                if (Column is DvDataGridButtonColumn)
                {
                    var col = Column as DvDataGridButtonColumn;
                    var Text = col.Text;
                    var IconGap = col.IconGap;
                    var IconString = col.IconString;
                    var IconSize = col.IconSize;
                    var IconAlignment = col.IconAlignment;

                    if (bDown) rtText.Offset(0, 1);
                    Util.DrawTextIcon(Canvas, Text, FontName, FontSize, DvFontStyle.Normal, IconGap, IconString, IconSize, cT, rtText, IconAlignment);
                }
            }
            #endregion
            base.CellDraw(Theme, Canvas, CellBounds, Info);
        }
        #endregion
        #region CellMouseDown
        public override void CellMouseDown(SKRect CellBounds, int x, int y)
        {
            if (CollisionTool.Check(CellBounds, x, y))
            {
                bDown = true;
            }
            base.CellMouseDown(CellBounds, x, y);
        }
        #endregion
        #region CellMouseUp
        public override void CellMouseUp(SKRect CellBounds, int x, int y)
        {
            if (bDown)
            {
                bDown = false;
                if (CollisionTool.Check(CellBounds, x, y)) Grid.InvokeCellButtonClick(this);
            }
            base.CellMouseUp(CellBounds, x, y);
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridImageCell
    public class DvDataGridImageCell : DvDataGridCell
    {
        #region Properties
        public PictureScaleMode ScaleMode { get; set; } = PictureScaleMode.Strech;
        #endregion
        #region Constructor
        public DvDataGridImageCell(DvDataGrid Grid, DvDataGridRow Row, IDvDataGridColumn Column) : base(Grid, Row, Column)
        {
            if (Column is DvDataGridImageColumn)
            {
                this.ScaleMode = ((DvDataGridImageColumn)Column).ScaleMode;
            }
        }
        #endregion
        #region Override
        #region CellDraw
        public override void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds, DvDataGridCellDrawInfo Info)
        {
            #region Draw
            if (Value is SKBitmap)
            {
                var rtv = CellBounds;

                var Image = (SKBitmap)Value;
                var rtContent = CellBounds;
                #region Image
                var cx = rtContent.MidX;
                var cy = rtContent.MidY;
                switch (ScaleMode)
                {
                    case PictureScaleMode.Real:
                        Canvas.DrawBitmap(Image, Util.FromRect(rtContent.Left, rtContent.Top, Image.Width, Image.Height));
                        break;
                    case PictureScaleMode.CenterImage:
                        Canvas.DrawBitmap(Image, Util.FromRect(cx - (Image.Width / 2), cy - (Image.Height / 2), Image.Width, Image.Height));
                        break;
                    case PictureScaleMode.Strech:
                        Canvas.DrawBitmap(Image, rtContent);
                        break;
                    case PictureScaleMode.Zoom:
                        double imgratio = 1D;
                        if ((Image.Width - rtContent.Width) > (Image.Height - rtContent.Height)) imgratio = (double)rtContent.Width / (double)Image.Width;
                        else imgratio = (double)rtContent.Height / (double)Image.Height;

                        int szw = Convert.ToInt32((double)Image.Width * imgratio);
                        int szh = Convert.ToInt32((double)Image.Height * imgratio);

                        Canvas.DrawBitmap(Image, Util.FromRect(rtContent.Left + (rtContent.Width / 2) - (szw / 2), rtContent.Top + (rtContent.Height / 2) - (szh / 2), szw, szh));
                        break;
                }
                #endregion

            }
            #endregion

            Info.Bevel = false;
            base.CellDraw(Theme, Canvas, CellBounds, Info);
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridCheckBoxCell
    public class DvDataGridCheckBoxCell : DvDataGridCell
    {
        #region Properties
        public SKColor? CheckBoxColor { get; set; }
        public SKColor? CheckColor { get; set; }
        #endregion
        #region Constructor
        public DvDataGridCheckBoxCell(DvDataGrid Grid, DvDataGridRow Row, IDvDataGridColumn Column) : base(Grid, Row, Column)
        {
            if (Column is DvDataGridCheckBoxColumn)
            {
                CheckBoxColor = ((DvDataGridCheckBoxColumn)Column).BoxColor;
                CheckColor = ((DvDataGridCheckBoxColumn)Column).CheckColor;
            }
        }
        #endregion
        #region Override
        #region CellDraw
        public override void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds, DvDataGridCellDrawInfo Info)
        {
            if (Grid != null)
            {
                #region Var
                var sbw = Convert.ToInt32(DvDataGrid.SELECTOR_BOX_WIDTH);
                var BoxBright = Theme.DataGridCheckBoxBright;
                var RowColor = Grid.GetRowColor(Theme);
                var SelectedRowColor = Grid.GetSelectedRowColor(Theme);

                var Value = this.Value is bool ? (bool)this.Value : false;
                var cRow = Row.Selected ? SelectedRowColor : RowColor;
                var CheckBoxColor = this.CheckBoxColor ?? cRow.BrightnessTransmit(BoxBright);
                var CheckColor = this.CheckColor ?? Grid.GetForeColor(Theme);
                var BorderColor = Theme.GetBorderColor(cRow, Grid.ParentContainer.GetBackColor());
                #endregion

                #region Draw
                {
                    var Checked = Value;
                    var rtSelectorBox = MathTool.MakeRectangle(Util.INT(CellBounds), new SKSize(sbw, sbw));
                    Theme.DrawBox(Canvas, rtSelectorBox, CheckBoxColor, BorderColor, RoundType.Rect, BoxStyle.Border | BoxStyle.Fill | BoxStyle.OutBevel | BoxStyle.InShadow);
                    if (Checked)
                    {
                        var INF = sbw / 4;
                        var rtCheck = Util.FromRect(rtSelectorBox.Left, rtSelectorBox.Top - 0, rtSelectorBox.Width, rtSelectorBox.Height); rtCheck.Inflate(-INF, -INF);
                        rtCheck.Inflate(-1, -1);

                        using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                        {
                            using (var fOut = SKImageFilter.CreateDropShadow(1, 1, 1, 1, Util.FromArgb(Theme.OutShadowAlpha, SKColors.Black)))
                            {
                                p.ImageFilter = fOut;
                                p.StrokeCap = SKStrokeCap.Round;
                                p.StrokeJoin = SKStrokeJoin.Round;
                                p.StrokeWidth = 4F;
                                p.Color = CheckColor;
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
                }
                #endregion
            }
            base.CellDraw(Theme, Canvas, CellBounds, Info);
        }
        #endregion
        #region CellMouseDown
        public override void CellMouseDown(SKRect CellBounds, int x, int y)
        {
            if (CollisionTool.Check(CellBounds, x, y))
            {
                var Value = this.Value is bool ? (bool)this.Value : false;
                var v = !((bool)Value);
                if (v != (bool)Value)
                {
                    #region Value Set
                    var old = Value;
                    this.Value = v;
                    Grid.InvokeValueChanged(this, old, v);
                    #endregion
                }
            }
            base.CellMouseDown(CellBounds, x, y);
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridComboBoxCell
    public class DvDataGridComboBoxCell : DvDataGridCell
    {
        #region Properties
        public int ButtonWidth { get; set; } = 40;
        public int MaximumViewCount { get; set; } = 8;
        public int ItemHeight { get; set; } = 30;
        public List<TextIconItem> Items { get; } = new List<TextIconItem>();

        public Padding ItemPadding { get; set; } = new Padding(0, 0, 0, 0);
        public DvContentAlignment ContentAlignment { get; set; } = DvContentAlignment.MiddleCenter;
        #endregion

        #region Drop
        bool bOpen => Grid != null && Grid.Design != null ? Grid.Design.VisibleDropDownWindow(ddwnd) : false;
        DropDownWindow ddwnd = new DropDownWindow();
        #endregion

        #region Constructor
        public DvDataGridComboBoxCell(DvDataGrid Grid, DvDataGridRow Row, IDvDataGridColumn Column) : base(Grid, Row, Column)
        {
            if (Column is DvDataGridComboBoxColumn)
            {
                var col = ((DvDataGridComboBoxColumn)Column);
                Items.AddRange(col.Items);
                ItemHeight = col.ItemHeight;
                ButtonWidth = col.ButtonWidth;
                MaximumViewCount = col.MaximumViewCount;
            }
        }
        #endregion

        #region Override
        #region CellDraw
        public override void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds, DvDataGridCellDrawInfo Info)
        {
            if (Grid != null && Grid.Design != null)
            {
                #region Var
                var Design = Grid.Design;
                var thm = Design.Theme;

                var rtContent = CellBounds;
                var rtIco = Util.FromRect(rtContent.Right - ButtonWidth, rtContent.Top, ButtonWidth, rtContent.Height);
                var rtBox = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width - rtIco.Width, rtContent.Height);
                var rtText = Util.FromRect(rtBox.Left + ItemPadding.Left, rtBox.Top + ItemPadding.Top, rtBox.Width - (ItemPadding.Left + ItemPadding.Right), rtBox.Height - (ItemPadding.Top + ItemPadding.Bottom));

                var BoxColor = Row.Selected ? Grid.GetSelectedRowColor(thm) : Grid.GetRowColor(thm);
                var ForeColor = Grid.GetForeColor(thm);
                var ItemColor = Grid.GetRowColor(thm);
                var SelectedItemColor = Grid.GetSelectedRowColor(thm);
                var BorderColor = thm.GetBorderColor(BoxColor, Grid.ParentContainer.GetBackColor());

                var FontName = Grid.FontName;
                var FontSize = Grid.FontSize;

                var col = Column as DvDataGridComboBoxColumn;
                var IconGap = col != null ? col.IconGap : 8;
                var IconSize = col != null ? col.IconSize : 15; 
                var IconAlignment = col != null ? col.IconAlignment : DvTextIconAlignment.LeftRight;
                #endregion

                var vsel = Items.Where(x => x.Value.Equals(Value)).FirstOrDefault();
                var SelectedIndex = vsel != null ? Items.IndexOf(vsel) : -1;

                using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                {
                    var rt = rtContent;

                    thm.DrawBox(Canvas, rtContent, BoxColor, BorderColor, RoundType.Rect, BoxStyle.Fill);

                    if (SelectedIndex >= 0 && SelectedIndex < Items.Count)
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

            base.CellDraw(Theme, Canvas, CellBounds, Info);
        }
        #endregion
        #region CellMouseDown
        DateTime downTime;
        SKPoint downPoint;
        public override void CellMouseDown(SKRect CellBounds, int x, int y)
        {
            if (CollisionTool.Check(CellBounds, x, y))
            {
                downPoint = new SKPoint(x, y);
                downTime = DateTime.Now;
            }
            base.CellMouseDown(CellBounds, x, y);
        }
        #endregion
        #region CellMouseUp
        public override void CellMouseUp(SKRect CellBounds, int x, int y)
        {
            if (CollisionTool.Check(CellBounds, x, y) && MathTool.GetDistance(downPoint, new SKPoint(x, y)) < 10)
            {
                if (Items != null && Items.Count > 0)
                {
                    if (Grid != null && Grid.Design != null)
                    {
                        var Design = Grid.Design;
                        var thm = Design.Theme;

                        var rt = CellBounds;

                        var sx = Grid.ScreenX + CellBounds.Left;
                        var sy = Grid.ScreenY + CellBounds.Top;
                        var sw = rt.Width + 3;
                        var sh = rt.Height + 3;

                        var vh = Math.Max(Math.Min(Items.Count, MaximumViewCount) * 30, sh);

                        var rts = Util.FromRect(sx, sy, sw, sh);
                        var rte = Util.FromRect(sx, sy, sw, vh);

                        if (rte.Bottom > Grid.Design.Height) rte = Util.FromRect(sx, sy + sh - vh-3, sw, vh);

                        rts = Util.INT(rts);
                        rte = Util.INT(rte);
                        rts.Inflate(1, 1);
                        rte.Inflate(1, 1);

                        ddwnd.ItemHeight = ItemHeight;
                        ddwnd.RowColor = Row.Selected ? Grid.GetSelectedRowColor(thm) : Grid.GetRowColor(thm);
                        ddwnd.ShowDropDown(rts, rte, Items, Value, (result) => Value = result);
                    }
                }
            }
            base.CellMouseUp(CellBounds, x, y);
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
            Action<object> result;
            object val;
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
                    Corner = 0,
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
            public void ShowDropDown(SKRect sbounds, SKRect ebounds, List<TextIconItem> Items, object Value, Action<object> result)
            {
                this.result = result;
                this.val = Value;
                list.Items.Clear();
                list.Items.AddRange(Items);

                var vsel = Items.Where(x => x.Value.Equals(Value)).FirstOrDefault();
                var SelectedIndex = vsel != null ? Items.IndexOf(vsel) : -1;
                list.SelectedItems.Clear();
                if (vsel != null) list.SelectedItems.Add(vsel);

                list.ScrollPosition = (SelectedIndex) * ItemHeight;

                this.Show(sbounds, ebounds);
            }
            #endregion
            #endregion
        }
        #endregion
    }
    #endregion
    #region class : DvDataGridEditTextCell 
    public class DvDataGridEditTextCell : DvDataGridCell
    {
        #region Properties
        public bool ReadOnly { get; set; }
        #endregion
        #region Constructor
        public DvDataGridEditTextCell(DvDataGrid Grid, DvDataGridRow Row, DvDataGridColumn Column) : base(Grid, Row, Column)
        {
        }
        #endregion
        #region Override
        #region CellDraw
        public override void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds, DvDataGridCellDrawInfo Info)
        {
            var CellTextColor = this.CellTextColor ?? Grid.GetForeColor(Theme);
            var CellBackColor = this.CellBackColor ?? Grid.GetRowColor(Theme);
            var SelectedCellBackColor = this.SelectedCellBackColor ?? Grid.GetSelectedRowColor(Theme);
            var BoxColor = (Row.Selected ? SelectedCellBackColor : CellBackColor).BrightnessTransmit(Theme.DataGridInputBright);
           
            var nc = Grid;
            var s = "";
            var FontName = nc.FontName;
            var FontSize = nc.FontSize;
            var val = Value;
            Theme.DrawBox(Canvas, CellBounds, BoxColor, BoxColor.BrightnessTransmit(Theme.BorderBrightness), RoundType.Rect, BoxStyle.Fill);
            if (Value != null)
            {
                if (Value is string) s = (string)Value;
                else s = Value.ToString();

                if (!string.IsNullOrWhiteSpace(s))
                {
                    Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, CellTextColor, CellBounds);
                }
            }
            Info.Bevel = false;
            base.CellDraw(Theme, Canvas, CellBounds, Info);
        }
        #endregion
        #region CellMouseDown
        DateTime downTime;
        SKPoint downPoint;
        public override void CellMouseDown(SKRect CellBounds, int x, int y)
        {
            if (CollisionTool.Check(CellBounds, x, y) && !ReadOnly)
            {
                downPoint = new SKPoint(x, y);
                downTime = DateTime.Now;
            }
            base.CellMouseDown(CellBounds, x, y);
        }
        #endregion
        #region CellMouseUp
        public override void CellMouseUp(SKRect CellBounds, int x, int y)
        {
            if (CollisionTool.Check(CellBounds, x, y) && !ReadOnly && MathTool.GetDistance(downPoint, new SKPoint(x, y)) < 10)
            {
                #region Click
                var mode = Column is DvDataGridEditTextColumn ? ((DvDataGridEditTextColumn)Column).Mode : KeyboardMode.Korea;

                Grid.Design.Keyboard.ShowKeyboard(Column.HeaderText, mode, Value as string, (result) =>
                {
                    var v = result;
                    if (v != null)
                    {
                        var old = Value;
                        Value = v;
                        Grid.InvokeValueChanged(this, old, v);
                    }
                });
                #endregion
            }

            base.CellMouseUp(CellBounds, x, y);
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridEditNumberCell 
    public class DvDataGridEditNumberCell<T> : DvDataGridCell where T : struct
    {
        #region Properties
        public T? Maximum { get; set; } = null;
        public T? Minimum { get; set; } = null;
        public string Format { get; set; }
        public bool ReadOnly { get; set; }
        #endregion
        #region Constructor
        public DvDataGridEditNumberCell(DvDataGrid Grid, DvDataGridRow Row, DvDataGridColumn Column) : base(Grid, Row, Column)
        {
            if (Column is DvDataGridTextFormatColumn)
            {
                this.Format = ((DvDataGridTextFormatColumn)Column).Format;
            }
            else if (Column is DvDataGridEditNumberColumn<T>)
            {
                this.Minimum = ((DvDataGridEditNumberColumn<T>)Column).Minimum;
                this.Maximum = ((DvDataGridEditNumberColumn<T>)Column).Maximum;
                this.Format = ((DvDataGridEditNumberColumn<T>)Column).Format;
            }
        }
        #endregion
        #region Override
        #region CellDraw
        public override void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds, DvDataGridCellDrawInfo Info)
        {
            var CellTextColor = this.CellTextColor ?? Grid.GetForeColor(Theme);
            var CellBackColor = this.CellBackColor ?? Grid.GetRowColor(Theme);
            var SelectedCellBackColor = this.SelectedCellBackColor ?? Grid.GetSelectedRowColor(Theme);
            var BoxColor = (Row.Selected ? SelectedCellBackColor : CellBackColor).BrightnessTransmit(Theme.DataGridInputBright);

            var nc = Grid;
            var s = "";
            var FontName = nc.FontName;
            var FontSize = nc.FontSize;
            var val = Value;

            Theme.DrawBox(Canvas, CellBounds, BoxColor, CellBackColor, RoundType.Rect, BoxStyle.Fill);
            if (Value != null)
            {
                if (typeof(T) == typeof(byte)) s = (!string.IsNullOrWhiteSpace(Format) ? ((byte)Value).ToString(Format) : ((byte)Value).ToString());
                else if (typeof(T) == typeof(sbyte)) s = (!string.IsNullOrWhiteSpace(Format) ? ((sbyte)Value).ToString(Format) : ((sbyte)Value).ToString());
                else if (typeof(T) == typeof(short)) s = (!string.IsNullOrWhiteSpace(Format) ? ((short)Value).ToString(Format) : ((short)Value).ToString());
                else if (typeof(T) == typeof(ushort)) s = (!string.IsNullOrWhiteSpace(Format) ? ((ushort)Value).ToString(Format) : ((ushort)Value).ToString());
                else if (typeof(T) == typeof(int)) s = (!string.IsNullOrWhiteSpace(Format) ? ((int)Value).ToString(Format) : ((int)Value).ToString());
                else if (typeof(T) == typeof(uint)) s = (!string.IsNullOrWhiteSpace(Format) ? ((uint)Value).ToString(Format) : ((uint)Value).ToString());
                else if (typeof(T) == typeof(long)) s = (!string.IsNullOrWhiteSpace(Format) ? ((long)Value).ToString(Format) : ((long)Value).ToString());
                else if (typeof(T) == typeof(ulong)) s = (!string.IsNullOrWhiteSpace(Format) ? ((ulong)Value).ToString(Format) : ((ulong)Value).ToString());
                else if (typeof(T) == typeof(float)) s = (!string.IsNullOrWhiteSpace(Format) ? ((float)Value).ToString(Format) : ((float)Value).ToString());
                else if (typeof(T) == typeof(double)) s = (!string.IsNullOrWhiteSpace(Format) ? ((double)Value).ToString(Format) : ((double)Value).ToString());
                else if (typeof(T) == typeof(decimal)) s = (!string.IsNullOrWhiteSpace(Format) ? ((decimal)Value).ToString(Format) : ((decimal)Value).ToString());

                if (!string.IsNullOrWhiteSpace(s))
                {
                    Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, CellTextColor, CellBounds);
                }
            }
            Info.Bevel = false;
            base.CellDraw(Theme, Canvas, CellBounds, Info);
        }
        #endregion
        #region CellMouseDown
        DateTime downTime;
        SKPoint downPoint;
        public override void CellMouseDown(SKRect CellBounds, int x, int y)
        {
            if (CollisionTool.Check(CellBounds, x, y) && !ReadOnly)
            {
                downPoint = new SKPoint(x, y);
                downTime = DateTime.Now;
            }
            base.CellMouseDown(CellBounds, x, y);
        }
        #endregion
        #region CellMouseUp
        public override void CellMouseUp(SKRect CellBounds, int x, int y)
        {
            if (CollisionTool.Check(CellBounds, x, y) && !ReadOnly && MathTool.GetDistance(downPoint, new SKPoint(x, y)) < 10)
            {
                #region Click
                var Design = Grid.Design;
                Design.Keypad.ShowKeypad<T>(Column.HeaderText, (T?)Value, Minimum, Maximum, (result) =>
                {
                    if (result.HasValue && !((object)result.Value).Equals(Value))
                    {
                        var val = result.Value;
                        var old = Value;

                        if (Minimum.HasValue && Maximum.HasValue)
                        {
                            var min = (object)Minimum.Value;
                            var max = (object)Maximum.Value;

                            if (typeof(T) == typeof(byte)) val = (T)(object)MathTool.Constrain((byte)(object)val, (byte)min, (byte)max);
                            else if (typeof(T) == typeof(sbyte)) val = (T)(object)MathTool.Constrain((sbyte)(object)val, (sbyte)min, (sbyte)max);
                            else if (typeof(T) == typeof(short)) val = (T)(object)MathTool.Constrain((short)(object)val, (short)min, (short)max);
                            else if (typeof(T) == typeof(ushort)) val = (T)(object)MathTool.Constrain((ushort)(object)val, (ushort)min, (ushort)max);
                            else if (typeof(T) == typeof(int)) val = (T)(object)MathTool.Constrain((int)(object)val, (int)min, (int)max);
                            else if (typeof(T) == typeof(uint)) val = (T)(object)MathTool.Constrain((uint)(object)val, (uint)min, (uint)max);
                            else if (typeof(T) == typeof(long)) val = (T)(object)MathTool.Constrain((long)(object)val, (long)min, (long)max);
                            else if (typeof(T) == typeof(ulong)) val = (T)(object)MathTool.Constrain((ulong)(object)val, (ulong)min, (ulong)max);
                            else if (typeof(T) == typeof(float)) val = (T)(object)MathTool.Constrain((float)(object)val, (float)min, (float)max);
                            else if (typeof(T) == typeof(double)) val = (T)(object)MathTool.Constrain((double)(object)val, (double)min, (double)max);
                            else if (typeof(T) == typeof(decimal)) val = (T)(object)MathTool.Constrain((decimal)(object)val, (decimal)min, (decimal)max);
                        }

                        Value = val; 
                        Grid.InvokeValueChanged(this, old, (object)(result.Value));
                    }
                });
                #endregion
            }
            base.CellMouseUp(CellBounds, x, y);
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridEditBoolCell 
    public class DvDataGridEditBoolCell : DvDataGridCell
    {
        #region Properties
        public bool ReadOnly { get; set; }

        public string OnText { get; set; } = "ON";
        public string OffText { get; set; } = "OFF";
        #endregion
        #region Member Variable
        Animation ani = new Animation();
        #endregion
        #region Constructor
        public DvDataGridEditBoolCell(DvDataGrid Grid, DvDataGridRow Row, DvDataGridColumn Column) : base(Grid, Row, Column)
        {
            if (Column is DvDataGridEditBoolColumn)
            {
                var col = ((DvDataGridEditBoolColumn)Column);
                OnText = col.OnText;
                OffText = col.OffText;
            }
        }
        #endregion
        #region Override
        #region CellDraw
        public override void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds, DvDataGridCellDrawInfo Info)
        {
            var CellTextColor = this.CellTextColor ?? Grid.GetForeColor(Theme);
            var CellBackColor = this.CellBackColor ?? Grid.GetRowColor(Theme);
            var SelectedCellBackColor = this.SelectedCellBackColor ?? Grid.GetSelectedRowColor(Theme);
            var BoxColor = (Row.Selected ? SelectedCellBackColor : CellBackColor).BrightnessTransmit(Theme.DataGridInputBright);

            var nc = Grid;
            var s = "";
            var FontName = nc.FontName;
            var FontSize = nc.FontSize;
            var val = Value;
            Theme.DrawBox(Canvas, CellBounds, BoxColor, BoxColor.BrightnessTransmit(Theme.BorderBrightness), RoundType.Rect, BoxStyle.Fill);
            if (Value != null && val is bool)
            {
                bounds(CellBounds, (rtOn, rtOff) => {
                    using (var p = new SKPaint { IsAntialias = true })
                    {
                        var v = (bool)val; 
                        var thm = Theme;
                        var cL = CellTextColor;
                        var cD = Util.FromArgb(75, cL);
                        var cOn = v ? cL : cD;
                        var cOff = v ? cD : cL;

                        var isOn = v ? 12 : 0;
                        var isOff = v ? 0 : 12;
                        var igOn = v ? 5 : 0;
                        var igOff = v ? 0 : 5;

                        if (Grid.Design.Animation && ani.IsPlaying)
                        {
                            if (v)
                            {
                                cOn = ani.Value(AnimationAccel.DCL, cD, cL);
                                cOff = ani.Value(AnimationAccel.DCL, cL, cD);
                                isOn = ani.Value(AnimationAccel.DCL, 0, 12);
                                isOff = ani.Value(AnimationAccel.DCL, 12, 0);
                                igOn = ani.Value(AnimationAccel.DCL, 0, 5);
                                igOff = ani.Value(AnimationAccel.DCL, 5, 0);
                            }
                            else
                            {
                                cOn = ani.Value(AnimationAccel.DCL, cL, cD);
                                cOff = ani.Value(AnimationAccel.DCL, cD, cL);
                                isOff = ani.Value(AnimationAccel.DCL, 0, 12);
                                isOn = ani.Value(AnimationAccel.DCL, 12, 0);
                                igOff = ani.Value(AnimationAccel.DCL, 0, 5);
                                igOn = ani.Value(AnimationAccel.DCL, 5, 0);
                            }
                        }

                        Util.DrawTextIcon(Canvas, OnText, FontName, FontSize, DvFontStyle.Normal, igOn, "fa-check", isOn, cOn, rtOn, DvTextIconAlignment.LeftRight);
                        Util.DrawTextIcon(Canvas, OffText, FontName, FontSize, DvFontStyle.Normal, igOff, "fa-check", isOff, cOff, rtOff, DvTextIconAlignment.LeftRight);

                        #region Unit Sep
                        {
                            float h, s, b;
                            BoxColor.ToHsv(out h, out s, out b);

                            var szh = Convert.ToInt32(CellBounds.Height / 2);
                            var x = Convert.ToInt32(CellBounds.MidX);

                            p.StrokeWidth = 1;

                            p.Color = b < 50 ? thm.GetInBevelColor(BoxColor) : BoxColor.BrightnessTransmit(thm.BorderBrightness);
                            Canvas.DrawLine(x + 0.5F, (CellBounds.Top + (CellBounds.Height / 2)) - (szh / 2) + 1, x + 0.5F, (CellBounds.Top + (CellBounds.Height / 2)) + (szh / 2) + 1, p);

                            p.Color = b < 50 ? BoxColor.BrightnessTransmit(thm.BorderBrightness) : thm.GetInBevelColor(BoxColor);
                            Canvas.DrawLine(x + 1.5F, (CellBounds.Top + (CellBounds.Height / 2)) - (szh / 2) + 1, x + 1.5F, (CellBounds.Top + (CellBounds.Height / 2)) + (szh / 2) + 1, p);
                        }
                        #endregion
                    }
                });
            }
            Info.Bevel = false;
            base.CellDraw(Theme, Canvas, CellBounds, Info);
        }
        #endregion
        #region CellMouseDown
        DateTime downTime;
        SKPoint downPoint;
        public override void CellMouseDown(SKRect CellBounds, int x, int y)
        {
            if (CollisionTool.Check(CellBounds, x, y) && !ReadOnly)
            {
                downPoint = new SKPoint(x, y);
                downTime = DateTime.Now;
            }
            base.CellMouseDown(CellBounds, x, y);
        }
        #endregion
        #region CellMouseUp
        public override void CellMouseUp(SKRect CellBounds, int x, int y)
        {
            if (CollisionTool.Check(CellBounds, x, y) && !ReadOnly && MathTool.GetDistance(downPoint, new SKPoint(x, y)) < 10)
            {
                #region Click
                bounds(CellBounds, (rtOn, rtOff) =>
                {
                    var val = Value;
                    if (val != null && val is bool)
                    {
                        var v = (bool)val;
                        if (CollisionTool.Check(rtOn, x, y) && !v) { Value = true; if (Grid.Design.Animation) { ani.Stop(); ani.Start(DvDesign.ANI); } }
                        if (CollisionTool.Check(rtOff, x, y) && v) { Value = false; if (Grid.Design.Animation) { ani.Stop(); ani.Start(DvDesign.ANI); } }
                    }
                });
                #endregion
            }

            base.CellMouseUp(CellBounds, x, y);
        }
        #endregion
        #endregion
        #region Method
        #region bounds
        void bounds(SKRect rtValue, Action<SKRect, SKRect> act)
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
    #region class : DvDataGridEditDateTimeCell 
    public class DvDataGridEditDateTimeCell : DvDataGridCell
    {
        #region Properties
        public bool ReadOnly { get; set; }
        public string Format { get; set; }
        public DateTimePickerType PickerMode { get; set; }
        #endregion
        #region Constructor
        public DvDataGridEditDateTimeCell(DvDataGrid Grid, DvDataGridRow Row, DvDataGridColumn Column) : base(Grid, Row, Column)
        {
            if (Column is DvDataGridDateTimePickerColumn)
            {
                this.Format = ((DvDataGridDateTimePickerColumn)Column).Format;
                this.PickerMode = ((DvDataGridDateTimePickerColumn)Column).PickerMode;
            }
        }
        #endregion
        #region Override
        #region CellDraw
        public override void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds, DvDataGridCellDrawInfo Info)
        {
            var CellTextColor = this.CellTextColor ?? Grid.GetForeColor(Theme);
            var CellBackColor = this.CellBackColor ?? Grid.GetRowColor(Theme);
            var SelectedCellBackColor = this.SelectedCellBackColor ?? Grid.GetSelectedRowColor(Theme);
            var BoxColor = (Row.Selected ? SelectedCellBackColor : CellBackColor).BrightnessTransmit(Theme.DataGridInputBright);

            var nc = Grid;
            var s = "";
            var FontName = nc.FontName;
            var FontSize = nc.FontSize;
            var val = Value;

            Theme.DrawBox(Canvas, CellBounds, BoxColor, CellBackColor, RoundType.Rect, BoxStyle.Fill);
            if (Value != null)
            {
                if (Value is DateTime)
                {
                    if (Format == null)
                    {
                        switch (PickerMode)
                        {
                            case DateTimePickerType.DateTime: s = ((DateTime)Value).ToString("yyyy-MM-dd HH:mm:ss"); break;
                            case DateTimePickerType.Date: s = ((DateTime)Value).ToString("yyyy-MM-dd"); break;
                            case DateTimePickerType.Time: s = ((DateTime)Value).ToString("HH:mm:ss"); break;
                        }
                    }
                    else s = ((DateTime)Value).ToString(Format);
                }
                else s = Value.ToString();

                if (!string.IsNullOrWhiteSpace(s))
                {
                    Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, CellTextColor, CellBounds);
                }
            }
            Info.Bevel = false;
            base.CellDraw(Theme, Canvas, CellBounds, Info);
        }
        #endregion
        #region CellMouseDown
        DateTime downTime;
        SKPoint downPoint;
        public override void CellMouseDown(SKRect CellBounds, int x, int y)
        {
            if (CollisionTool.Check(CellBounds, x, y) && !ReadOnly)
            {
                downPoint = new SKPoint(x, y);
                downTime = DateTime.Now;
            }
            base.CellMouseDown(CellBounds, x, y);
        }
        #endregion
        #region CellMouseUp
        public override void CellMouseUp(SKRect CellBounds, int x, int y)
        {
            if (CollisionTool.Check(CellBounds, x, y) && !ReadOnly && MathTool.GetDistance(downPoint, new SKPoint(x, y)) < 10)
            {
                #region Click
                var ds = Grid.Design;
                if (PickerMode == DateTimePickerType.DateTime) ds.DateTimePickerBox.ShowDateTimePicker(Column.HeaderText, (DateTime?)Value, (time) =>
                {
                    if (time.HasValue)
                    {
                        if (time.Value != (DateTime?)Value)
                        {
                            var old = Value;
                            Value = time.Value;
                            Grid.InvokeValueChanged(this, old, time.Value);
                        }
                    }
                });
                else if (PickerMode == DateTimePickerType.Date) ds.DateTimePickerBox.ShowDatePicker(Column.HeaderText, (DateTime?)Value, (time) =>
                {
                    if (time.HasValue)
                    {
                        if (time.Value != (DateTime?)Value)
                        {
                            var old = Value;
                            Value = time.Value;
                            Grid.InvokeValueChanged(this, old, time.Value);
                        }
                    }
                });
                else if (PickerMode == DateTimePickerType.Time) ds.DateTimePickerBox.ShowTimePicker(Column.HeaderText, (DateTime?)Value, (time) =>
                {
                    if (time.HasValue)
                    {
                        if (time.Value != (DateTime?)Value)
                        {
                            var old = Value;
                            Value = time.Value;
                            Grid.InvokeValueChanged(this, old, time.Value);
                        }
                    }
                });
                #endregion
            }
            base.CellMouseUp(CellBounds, x, y);
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridEditColorCell 
    public class DvDataGridEditColorCell : DvDataGridCell
    {
        #region Properties
        public bool ReadOnly { get; set; }
        #endregion
        #region Constructor
        public DvDataGridEditColorCell(DvDataGrid Grid, DvDataGridRow Row, DvDataGridColumn Column) : base(Grid, Row, Column)
        {
            if (Column is DvDataGridColorPickerColumn)
            {
            }
        }
        #endregion
        #region Override
        #region CellDraw
        public override void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds, DvDataGridCellDrawInfo Info)
        {
            var CellTextColor = this.CellTextColor ?? Grid.GetForeColor(Theme);
            var CellBackColor = this.CellBackColor ?? Grid.GetRowColor(Theme);
            var SelectedCellBackColor = this.SelectedCellBackColor ?? Grid.GetSelectedRowColor(Theme);
            var BoxColor = (Row.Selected ? SelectedCellBackColor : CellBackColor).BrightnessTransmit(Theme.DataGridInputBright);

            var nc = Grid;
            var s = "";
            var FontName = nc.FontName;
            var FontSize = nc.FontSize;
            var val = Value;

            Theme.DrawBox(Canvas, CellBounds, BoxColor, CellBackColor, RoundType.Rect, BoxStyle.Fill);
            if (Value != null && Value is SKColor)
            {
                var thm = Grid.Design.Theme;
                var ForeColor = Grid.GetForeColor(thm);
                var BorderColor = thm.GetBorderColor(BoxColor, Grid.ParentContainer.GetBackColor());
                var Corner = thm.Corner;
                var CodeType = (Column is DvDataGridColorPickerColumn) ? ((DvDataGridColorPickerColumn)Column).CodeType : ColorCodeType.CodeRGB;
           
                using (var p = new SKPaint() { IsAntialias = DvDesign.AA })
                {
                    var vc = (SKColor)Value;
                    var sz = Math.Max(12, CellBounds.Height / 3);
                    s = ColorTool.GetName(vc, CodeType);

                    Util.TextIconBounds(s, FontName, FontSize, DvFontStyle.Normal, 8, CellBounds, Util.FromRect(0, 0, sz, sz), DvTextIconAlignment.LeftRight, DvContentAlignment.MiddleCenter, (rtIcon, rtText) =>
                    {
                        rtIcon.Offset(0, 0);
                        thm.DrawBox(Canvas, rtIcon, vc, SKColors.Black, RoundType.Rect, BoxStyle.Fill | BoxStyle.Border | BoxStyle.OutShadow);
                        Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtText, DvContentAlignment.MiddleCenter, true);
                    });
                }
            }
            Info.Bevel = false;
            base.CellDraw(Theme, Canvas, CellBounds, Info);
        }
        #endregion
        #region CellMouseDown
        DateTime downTime;
        SKPoint downPoint;
        public override void CellMouseDown(SKRect CellBounds, int x, int y)
        {
            if (CollisionTool.Check(CellBounds, x, y) && !ReadOnly)
            {
                downPoint = new SKPoint(x, y);
                downTime = DateTime.Now;
            }
            base.CellMouseDown(CellBounds, x, y);
        }
        #endregion
        #region CellMouseUp
        public override void CellMouseUp(SKRect CellBounds, int x, int y)
        {
            if (CollisionTool.Check(CellBounds, x, y) && !ReadOnly && MathTool.GetDistance(downPoint, new SKPoint(x, y)) < 10)
            {
                #region Click
                Grid.Design.ColorPickerBox.ShowColorPicker(Column.HeaderText, (SKColor?)Value, (ret) =>
                {
                    if (ret.HasValue)
                    {
                        if (ret.Value != (SKColor?)Value)
                        {
                            var old = Value;
                            Value = ret.Value;
                            Grid.InvokeValueChanged(this, old, ret.Value);
                        }
                    }
                });
                #endregion
            }
            base.CellMouseUp(CellBounds, x, y);
        }
        #endregion
        #endregion
    }
    #endregion

    #region class : DvDataGridSummaryLabelCell
    public class DvDataGridSummaryLabelCell : DvDataGridSummaryCell
    {
        #region Properties
        public string Text { get; set; }
        #endregion
        #region Constructor
        public DvDataGridSummaryLabelCell(DvDataGrid Grid, DvDataGridSummaryRow Row) : base(Grid, Row)
        {

        }
        #endregion
        #region Override
        #region CellDraw
        public override void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds)
        {
            var CellTextColor = this.CellTextColor ?? Grid.GetForeColor(Theme);
            var CellBackColor = this.CellBackColor ?? Grid.GetRowColor(Theme);
            var BorderColor = Theme.GetBorderColor(CellBackColor, Grid.ParentContainer.GetBackColor());

            var nc = Grid;
            var FontName = nc.FontName;
            var FontSize = nc.FontSize;

            var rt = Util.INT(CellBounds);
            var s = !string.IsNullOrWhiteSpace(Text) ? Text : "";
            Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, CellTextColor, rt); ;

            base.CellDraw(Theme, Canvas, CellBounds);
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridSummarySumCell
    public class DvDataGridSummarySumCell : DvDataGridSummaryCell
    {
        #region Properties
        public string Format { get; set; }
        public decimal Value { get; private set; }
        #endregion
        #region Constructor
        public DvDataGridSummarySumCell(DvDataGrid Grid, DvDataGridSummaryRow Row) : base(Grid, Row)
        {

        }
        #endregion
        #region Virtual Method
        #region CellPaint
        public override void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds)
        {
            var CellTextColor = this.CellTextColor ?? Grid.GetForeColor(Theme);
            var CellBackColor = this.CellBackColor ?? Grid.GetRowColor(Theme);
            var BorderColor = Theme.GetBorderColor(CellBackColor, Grid.ParentContainer.GetBackColor());

            var nc = Grid;
            var FontName = nc.FontName;
            var FontSize = nc.FontSize;

            var rt = Util.INT(CellBounds);
            var s = string.IsNullOrWhiteSpace(Format) ? Value.ToString() : Value.ToString(Format);
            Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, CellTextColor, rt); ;

            base.CellDraw(Theme, Canvas, CellBounds);
        }
        #endregion
        #region Calc
        public override void Calculation()
        {
            Value = Grid.GetRows().Select(x => Convert.ToDecimal(x.Cells[ColumnIndex].Value)).Sum();
            base.Calculation();
        }
        #endregion
        #endregion
    }
    #endregion
    #region class : DvDataGridSummaryAverageCell
    public class DvDataGridSummaryAverageCell : DvDataGridSummaryCell
    {
        #region Properties
        public string Format { get; set; }
        public decimal Value { get; private set; }
        #endregion
        #region Constructor
        public DvDataGridSummaryAverageCell(DvDataGrid Grid, DvDataGridSummaryRow Row) : base(Grid, Row)
        {

        }
        #endregion
        #region Virtual Method
        #region CellPaint
        public override void CellDraw(DvTheme Theme, SKCanvas Canvas, SKRect CellBounds)
        {
            var CellTextColor = this.CellTextColor ?? Grid.GetForeColor(Theme);
            var CellBackColor = this.CellBackColor ?? Grid.GetRowColor(Theme);
            var BorderColor = Theme.GetBorderColor(CellBackColor, Grid.ParentContainer.GetBackColor());

            var nc = Grid;
            var FontName = nc.FontName;
            var FontSize = nc.FontSize;

            var rt = Util.INT(CellBounds);
            var s = string.IsNullOrWhiteSpace(Format) ? Value.ToString() : Value.ToString(Format);
            Util.DrawText(Canvas, s, FontName, FontSize, DvFontStyle.Normal, CellTextColor, rt); ;

            base.CellDraw(Theme, Canvas, CellBounds);
        }
        #endregion
        #region Calc
        public override void Calculation()
        {
            Value = Grid.GetRows().Select(x => Convert.ToDecimal(x.Cells[ColumnIndex].Value)).Average();
            base.Calculation();
        }
        #endregion
        #endregion
    }
    #endregion

    #region class : DvDataGridCellDrawInfo
    public class DvDataGridCellDrawInfo
    {
        public bool Bevel { get; set; } = true;
        public bool Border { get; set; } = true;
    }
    #endregion
}

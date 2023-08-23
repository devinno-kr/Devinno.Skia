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
    public partial class DvDataGrid : DvControl
    {
        #region Const 
        internal const float SPECIAL_CELL_WIDTH = 40;
        internal const float SELECTOR_BOX_WIDTH = 20;
        #endregion

        #region Properties
        #region BackgroundDraw
        //public bool BackgroundDraw { get; set; } = true;
        #endregion
        #region Color
        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BoxColor { get; set; } = null;
        public SKColor? ColumnColor { get; set; } = null;
        public SKColor? SelectedColor { get; set; } = null;
        public SKColor? SummaryColor { get; set; } = null;

        public SKColor? SelectorCellBackColor { get; set; } = null;
        #endregion
        #region RowHeight
        public float RowHeight { get; set; } = 30F;
        public float ColumnHeight { get; set; } = 40F;
        #endregion
        #region Font
        public string FontName { get; set; } = "NanumGothic";
        public int FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;
        #endregion
        #region Columns/Rows
        public EventList2<DvDataGridColumn> ColumnGroups { get; private set; } = new EventList2<DvDataGridColumn>();
        public EventList2<DvDataGridColumn> Columns { get; private set; } = new EventList2<DvDataGridColumn>();
        public List<DvDataGridRow> Rows { get; } = new List<DvDataGridRow>();
        public List<DvDataGridSummaryRow> SummaryRows { get; } = new List<DvDataGridSummaryRow>();
        #endregion
        #region Select
        public DvDataGridSelectionMode SelectionMode { get; set; } = DvDataGridSelectionMode.SINGLE;
        #endregion
        #region Round
        public DvRoundType Round { get; set; } = DvRoundType.All;
        #endregion
        #region Scroll
        public ScrollMode ScrollMode { get; set; } = ScrollMode.Vertical;

        public double VScrollPosition { get => vscroll.ScrollPosition; set => vscroll.ScrollPosition = value; }
        public double HScrollPosition { get => hscroll.ScrollPosition; set => hscroll.ScrollPosition = value; }

        public bool TouchMode
        {
            get => vscroll.TouchMode;
            set => vscroll.TouchMode = hscroll.TouchMode = value;
        }
        #endregion
        #region Internal
        internal Type DataType { get; private set; } = null;

        internal float SPWO_V => Convert.ToSingle(vscroll.ScrollPositionWithOffset);
        internal float SPWO_H => Convert.ToSingle(hscroll.ScrollPositionWithOffset);
        #endregion
        #endregion

        #region Event
        public event EventHandler SelectedChanged;
        public event EventHandler SortChanged;

        public event EventHandler<ColumnMouseEventArgs> ColumnMouseClick;
        public event EventHandler<CellMouseEventArgs> CellMouseClick;
        public event EventHandler<CellMouseEventArgs> CellMouseDoubleClick;
        public event EventHandler<CellButtonClickEventArgs> CellButtonClick;
        public event EventHandler<CellValueChangedEventArgs> ValueChanged;
        #endregion

        #region Member Variable
        List<DvDataGridRow> mrows = new List<DvDataGridRow>();
        List<DGColumnInfo> mcols = new List<DGColumnInfo>();
        List<DGColumnInfo> mcols_col = new List<DGColumnInfo>();
        List<DGColumnInfo> mcols_grp = new List<DGColumnInfo>();

        Scroll vscroll = new Scroll() { Direction = ScrollDirection.Vertical, Cut = true, TouchMode = true };
        Scroll hscroll = new Scroll() { Direction = ScrollDirection.Horizon, Cut = true, TouchMode = true };

        object objs = null;
        //float PrevTotalHeight = -1;
        int colrc = 1;
        bool bRefreshColumn = true;

        double hST = 0, vST = 0;
        double hSV = 0, vSV = 0;

        float? dx = null, dy = null;
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

            Columns.Changed += (o, s) => bRefreshColumn = true;
            ColumnGroups.Changed += (o, s) => bRefreshColumn = true;
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            #region refresh
            if (bRefreshColumn)
            {
                MakeColumnInfo();
                bRefreshColumn = true;
            }
            #endregion

            Areas((rtContent, rtColumn, rtScrollContent, rtSummary, rtScrollV, rtScrollH, rtScrollR) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    #region color
                    var ForeColor = GetForeColor(thm);
                    var BackColor = GetBackColor();
                    var ColumnColor = GetColumnColor(thm);
                    var SelectedColor = GetSelectedColor(thm);
                    var SummaryColor = GetSummaryColor(thm);
                    var BoxColor = GetBoxColor(thm);
                    var BorderColor = GetBorderColor(thm);
                    var InputBoxColor = GetInputBoxColor(thm);
                    var InputBoxBorderColor = GetInputBoxBorderColor(thm);
                    var InputColumnColor = GetInputColumnColor(thm);
                    var InputColumnBorderColor = GetInputColumnBorderColor(thm);
                    #endregion
                    #region rect
                    var rt = new SKRoundRect(rtContent, thm.Corner);
                    Util.SetRound(rt, Round, thm.Corner);
                    #endregion
                    #region var
                    var vspos = Convert.ToSingle(vscroll.ScrollPositionWithOffset);
                    var hspos = Convert.ToSingle(hscroll.ScrollPositionWithOffset);
                    var bAllSelect = mrows.Where(x => x.Selected).Count() > 0;
                    var bUseFilter = Columns.Where(x => x.UseFilter).Count() > 0;

                    var lsc = mcols_col;
                    #endregion

                    using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                    {
                        var sp = Canvas.Save();
                        Canvas.ClipRoundRect(rt);

                        ColIndex(rtScrollContent, (csi, cei) =>
                        {
                            RowIndex(rtScrollContent, (rsi, rei) =>
                            {
                                #region var
                                var lscol = mcols.Where(x => CollisionTool.CheckHorizon(rtScrollContent.Left, rtScrollContent.Right,
                                                            x.Bounds.Left + (!x.Column.Fixed ? hspos : 0), x.Bounds.Right + (!x.Column.Fixed ? hspos : 0))).ToList();
                                #endregion

                                #region Column
                                #region Column
                                {
                                    var sp2 = Canvas.Save();
                                    Canvas.Translate(0, rtColumn.Top);

                                    foreach (var v in lscol.OrderBy(x => x.Column.Fixed))
                                    {
                                        var rtb = v.GetBounds(hspos);
                                        var rtbf = v.GetBoundsFilter(hspos);

                                        v.Column.Draw(Canvas, thm, rtb, rtbf, ForeColor, BackColor, ColumnColor, BorderColor, InputColumnColor, InputColumnBorderColor);
                                    }

                                    Canvas.RestoreToCount(sp2);
                                }
                                #endregion
                                #region ColumnSelector
                                if (SelectionMode == DvDataGridSelectionMode.SELECTOR)
                                {
                                    var rtSelector = Util.FromRect(rtColumn.Left, rtColumn.Top, SPECIAL_CELL_WIDTH, rtColumn.Height);
                                    var rtSelectorBox = MathTool.MakeRectangle(Util.INT(rtSelector), new SKSize(SELECTOR_BOX_WIDTH, SELECTOR_BOX_WIDTH));

                                    thm.DrawDataGridColumn(Canvas, rtSelector, null,
                                        ForeColor, BackColor, ColumnColor, BorderColor, InputColumnColor, InputColumnBorderColor,
                                        "", FontName, FontSize, FontStyle,
                                        false, "",
                                        false, DvDataGridColumnSortState.NONE);

                                    thm.DrawCheckBox(Canvas, rtSelectorBox, SELECTOR_BOX_WIDTH,
                                        InputColumnColor, InputColumnBorderColor, ForeColor, ForeColor,
                                        "", FontName, FontSize, FontStyle, DvContentAlignment.MiddleLeft, bAllSelect);

                                }
                                #endregion
                                #region ColumnScroll
                                if (rtScrollV.HasValue)
                                {
                                    var rtv = Util.FromRect(rtScrollV.Value.Left, rtColumn.Top, rtScrollV.Value.Width, rtColumn.Height);
                                    var c = ColumnColor.BrightnessTransmit(-0.05F);

                                    thm.DrawDataGridColumn(Canvas, rtv, null,
                                    ForeColor, BackColor, c, BorderColor, InputColumnColor, InputColumnBorderColor,
                                    "", FontName, FontSize, FontStyle,
                                    false, "",
                                    false, DvDataGridColumnSortState.NONE, false);
                                }
                                #endregion
                                #endregion

                                #region Row
                                {
                                    var sp2 = Canvas.Save();
                                    Canvas.ClipRect(rtScrollContent);
                                    Canvas.Translate(0, rtScrollContent.Top + vspos);

                                    try
                                    {
                                        for (int i = rsi; i <= rei && i < mrows.Count; i++)
                                        {
                                            var v = mrows[i];
                                            var rt = new SKRect(rtScrollContent.Left, v.Top, rtScrollContent.Right, v.Bottom);
                                            v.Draw(Canvas, thm, rt,
                                                rtColumn, rtScrollContent,
                                                lscol.Where(x => Columns.Contains(x.Column)).ToList(), Rows, mcols_col,
                                                hspos,
                                                FontName, FontSize, FontStyle,
                                                ForeColor, BackColor, BoxColor, BorderColor, InputBoxColor, InputBoxBorderColor);
                                        }
                                    }
                                    catch { }

                                    Canvas.RestoreToCount(sp2);
                                }
                                #endregion

                                #region Summary
                                if (rtSummary.HasValue)
                                {
                                    var sp2 = Canvas.Save();
                                    Canvas.ClipRect(rtSummary.Value);
                                    Canvas.Translate(0, rtSummary.Value.Top);

                                    for (int i = 0; i < SummaryRows.Count; i++)
                                    {
                                        var v = SummaryRows[i];
                                        var rt = new SKRect(rtSummary.Value.Left, v.Top, rtSummary.Value.Right, v.Bottom);
                                        v.Draw(Canvas, thm, rt,
                                            rtColumn, rtScrollContent,
                                            lscol.Where(x => Columns.Contains(x.Column)).ToList(), SummaryRows, mcols_col,
                                            hspos,
                                            FontName, FontSize, FontStyle,
                                            ForeColor, BackColor, SummaryColor, BorderColor, InputBoxColor, InputBoxBorderColor);
                                    }

                                    Canvas.RestoreToCount(sp2);
                                }
                                #endregion

                                #region Scroll
                                if (ScrollMode == ScrollMode.Both && rtScrollV.HasValue && rtScrollH.HasValue && rtScrollR.HasValue)
                                {
                                    thm.DrawScroll(Canvas, rtScrollH.Value, BackColor, hscroll, DvRoundType.Rect);
                                    thm.DrawScroll(Canvas, rtScrollV.Value, BackColor, vscroll, DvRoundType.Rect);
                                    thm.DrawBox(Canvas, rtScrollR.Value, thm.ScrollBarColor, thm.ScrollBarColor, DvRoundType.RB, BoxStyle.Fill, thm.Corner);
                                }
                                else if (ScrollMode == ScrollMode.Vertical && rtScrollV.HasValue)
                                {
                                    thm.DrawScroll(Canvas, rtScrollV.Value, BackColor, vscroll, DvRoundType.Rect);
                                }
                                else if (ScrollMode == ScrollMode.Horizon && rtScrollH.HasValue)
                                {
                                    thm.DrawScroll(Canvas, rtScrollH.Value, BackColor, hscroll, DvRoundType.Rect);
                                }
                                #endregion
                            });
                        });

                        Canvas.RestoreToCount(sp);

                        #region Border
                        p.IsStroke = true;
                        p.StrokeWidth = 1F;
                        p.Color = BorderColor;
                        rt.Inflate(-0.5F, -0.5F);

                        Canvas.DrawRoundRect(rt, p);
                        #endregion
                    }
                }
            });

            base.OnDraw(Canvas);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(float x, float y)
        {
            dx = x;
            dy = y;

            Areas((rtContent, rtColumn, rtScrollContent, rtSummary, rtScrollV, rtScrollH, rtScrollR) =>
            {
                #region Scroll / Touch
                if (ScrollMode == ScrollMode.Vertical && rtScrollV.HasValue)
                {
                    vscroll.MouseDown(x, y, rtScrollV.Value);
                    if (vscroll.TouchMode && CollisionTool.Check(rtScrollContent, x, y)) vscroll.TouchDown(x, y);
                }
                else if (ScrollMode == ScrollMode.Horizon && rtScrollH.HasValue)
                {
                    hscroll.MouseDown(x, y, rtScrollH.Value);
                    if (hscroll.TouchMode && CollisionTool.Check(rtScrollContent, x, y)) hscroll.TouchDown(x, y);
                }
                else if (ScrollMode == ScrollMode.Both && rtScrollV.HasValue && rtScrollH.HasValue)
                {
                    vscroll.MouseDown(x, y, rtScrollV.Value);
                    hscroll.MouseDown(x, y, rtScrollH.Value);
                    if (hscroll.TouchMode && CollisionTool.Check(rtScrollContent, x, y)) hscroll.TouchDown(x, y);
                    if (vscroll.TouchMode && CollisionTool.Check(rtScrollContent, x, y)) vscroll.TouchDown(x, y);
                }
                #endregion

                #region Column / Rows
                {
                    #region var
                    var vspos = Convert.ToSingle(vscroll.ScrollPositionWithOffset);
                    var hspos = Convert.ToSingle(hscroll.ScrollPositionWithOffset);
                    var bAllSelect = mrows.Where(x => x.Selected).Count() > 0;
                    var bUseFilter = Columns.Where(x => x.UseFilter).Count() > 0;

                    var lsc = mcols_col;
                    #endregion

                    ColIndex(rtScrollContent, (csi, cei) =>
                    {
                        RowIndex(rtScrollContent, (rsi, rei) =>
                        {
                            #region var
                            var lscol = mcols.Where(x => CollisionTool.CheckHorizon(rtScrollContent.Left, rtScrollContent.Right,
                                                            x.Bounds.Left + (!x.Column.Fixed ? hspos : 0), x.Bounds.Right + (!x.Column.Fixed ? hspos : 0))).ToList();
                            #endregion
                            #region Column
                            foreach (var v in lscol.OrderBy(x => x.Column.Fixed))
                            {
                                var rtb = v.GetBounds(hspos);
                                var rtbf = v.GetBoundsFilter(hspos);

                                v.Column.MouseDown(rtb, rtbf, x, y - rtColumn.Top);
                            }
                            #endregion
                            #region Row
                            {
                                for (int i = rsi; i <= rei; i++)
                                {
                                    var v = mrows[i];
                                    var rt = new SKRect(rtScrollContent.Left, v.Top, rtScrollContent.Right, v.Bottom);
                                     
                                    v.MouseDown(rt, x, y - (rtScrollContent.Top + vspos),
                                        lscol.Where(x => Columns.Contains(x.Column)).ToList(), Rows, mcols_col,
                                        hspos);
                                }
                            }
                            #endregion
                            #region Summary
                            if (rtSummary.HasValue)
                            {
                                for (int i = 0; i < SummaryRows.Count; i++)
                                {
                                    var v = SummaryRows[i];
                                    var rt = new SKRect(rtSummary.Value.Left, v.Top, rtSummary.Value.Right, v.Bottom);
                                    v.MouseDown(rt, x, y - (rtSummary.Value.Top));
                                }
                            }
                            #endregion
                        });
                    });
                }
                #endregion
            });
         
            base.OnMouseDown(x, y);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(float x, float y)
        {
            Areas((rtContent, rtColumn, rtScrollContent, rtSummary, rtScrollV, rtScrollH, rtScrollR) =>
            {
                #region Scroll / Touch
                if (ScrollMode == ScrollMode.Vertical && rtScrollV.HasValue)
                {
                    vscroll.MouseUp(x, y);
                    if (vscroll.TouchMode) vscroll.TouchUp(x, y);
                }
                else if (ScrollMode == ScrollMode.Horizon && rtScrollH.HasValue)
                {
                    hscroll.MouseUp(x, y);
                    if (hscroll.TouchMode) hscroll.TouchUp(x, y);
                }
                else if (ScrollMode == ScrollMode.Both && rtScrollV.HasValue && rtScrollH.HasValue)
                {
                    vscroll.MouseUp(x, y);
                    hscroll.MouseUp(x, y);
                    if (hscroll.TouchMode) hscroll.TouchUp(x, y);
                    if (vscroll.TouchMode) vscroll.TouchUp(x, y);
                }
                #endregion

                #region Column / Rows
                {
                    #region var
                    var vspos = Convert.ToSingle(vscroll.ScrollPositionWithOffset);
                    var hspos = Convert.ToSingle(hscroll.ScrollPositionWithOffset);
                    var bAllSelect = mrows.Where(x => x.Selected).Count() > 0;
                    var bUseFilter = Columns.Where(x => x.UseFilter).Count() > 0;

                    var lsc = mcols_col;
                    #endregion

                    ColIndex(rtScrollContent, (csi, cei) =>
                    {
                        RowIndex(rtScrollContent, (rsi, rei) =>
                        {
                            #region var
                            var lscol = mcols.Where(x => CollisionTool.CheckHorizon(rtScrollContent.Left, rtScrollContent.Right,
                                                            x.Bounds.Left + (!x.Column.Fixed ? hspos : 0), x.Bounds.Right + (!x.Column.Fixed ? hspos : 0))).ToList();
                            #endregion
                            #region Column
                            foreach (var v in lscol.OrderBy(x => x.Column.Fixed))
                            {
                                var rtb = v.GetBounds(hspos);
                                var rtbf = v.GetBoundsFilter(hspos);

                                v.Column.MouseUp(rtb, rtbf, x, y - rtColumn.Top);
                            }
                            #endregion
                            #region Row
                            {
                                for (int i = rsi; i <= rei; i++)
                                {
                                    var v = mrows[i];
                                    var rt = new SKRect(rtScrollContent.Left, v.Top, rtScrollContent.Right, v.Bottom);

                                    v.MouseUp(rt, x, y - (rtScrollContent.Top + vspos),
                                        lscol.Where(x => Columns.Contains(x.Column)).ToList(), Rows, mcols_col,
                                        hspos);
                                }
                            }
                            #endregion
                            #region Summary
                            if (rtSummary.HasValue)
                            {
                                for (int i = 0; i < SummaryRows.Count; i++)
                                {
                                    var v = SummaryRows[i];
                                    var rt = new SKRect(rtSummary.Value.Left, v.Top, rtSummary.Value.Right, v.Bottom);
                                    v.MouseUp(rt, x, y - (rtSummary.Value.Top));
                                }
                            }
                            #endregion
                        });
                    });
                }
                #endregion
            });
            base.OnMouseUp(x, y);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(float x, float y)
        {
            Areas((rtContent, rtColumn, rtScrollContent, rtSummary, rtScrollV, rtScrollH, rtScrollR) =>
            {
                #region Scroll / Touch
                if (ScrollMode == ScrollMode.Vertical && rtScrollV.HasValue)
                {
                    vscroll.MouseMove(x, y, rtScrollV.Value);
                    if (vscroll.TouchMode) vscroll.TouchMove(x, y);
                }
                else if (ScrollMode == ScrollMode.Horizon && rtScrollH.HasValue)
                {
                    hscroll.MouseMove(x, y, rtScrollH.Value);
                    if (hscroll.TouchMode) hscroll.TouchMove(x, y);
                }
                else if (ScrollMode == ScrollMode.Both && rtScrollV.HasValue && rtScrollH.HasValue)
                {
                    vscroll.MouseMove(x, y, rtScrollV.Value);
                    hscroll.MouseMove(x, y, rtScrollH.Value);
                    if (hscroll.TouchMode) hscroll.TouchMove(x, y);
                    if (vscroll.TouchMode) vscroll.TouchMove(x, y);
                }
                #endregion
                 
            });
            base.OnMouseMove(x, y);
        }
        #endregion
        #region OnMouseClick
        protected override void OnMouseClick(float x, float y)
        {
            Areas((rtContent, rtColumn, rtScrollContent, rtSummary, rtScrollV, rtScrollH, rtScrollR) =>
            {
                #region Column / Rows
                if (dx.HasValue && dy.HasValue && MathTool.GetDistance(new SKPoint(dx.Value, dy.Value), new SKPoint(x, y)) < 5)
                {
                    #region var
                    var vspos = Convert.ToSingle(vscroll.ScrollPositionWithOffset);
                    var hspos = Convert.ToSingle(hscroll.ScrollPositionWithOffset);
                    var bAllSelect = mrows.Where(x => x.Selected).Count() > 0;
                    var bUseFilter = Columns.Where(x => x.UseFilter).Count() > 0;

                    var lsc = mcols_col;
                    #endregion

                    ColIndex(rtScrollContent, (csi, cei) =>
                    {
                        RowIndex(rtScrollContent, (rsi, rei) =>
                        {
                            #region var
                            var lscol = mcols.Where(x => CollisionTool.CheckHorizon(rtScrollContent.Left, rtScrollContent.Right,
                                                            x.Bounds.Left + (!x.Column.Fixed ? hspos : 0), x.Bounds.Right + (!x.Column.Fixed ? hspos : 0))).ToList();
                            #endregion
                            #region Column
                            foreach (var v in lscol.OrderBy(x => x.Column.Fixed))
                            {
                                var rtb = v.GetBounds(hspos);
                                var rtbf = v.GetBoundsFilter(hspos);

                                v.Column.MouseClick(rtb, rtbf, x, y - rtColumn.Top);
                            }

                            if (SelectionMode == DvDataGridSelectionMode.SELECTOR)
                            {
                                var rtSelector = Util.FromRect(rtColumn.Left, rtColumn.Top, SPECIAL_CELL_WIDTH, rtColumn.Height);
                                var rtSelectorBox = MathTool.MakeRectangle(Util.INT(rtSelector), new SKSize(SELECTOR_BOX_WIDTH, SELECTOR_BOX_WIDTH));

                                if (CollisionTool.Check(rtSelectorBox, x, y - rtColumn.Top))
                                {
                                    foreach (var v in GetRows()) v.Selected = !bAllSelect;
                                }
                            }
                            #endregion
                            #region Row
                            {
                                for (int i = rsi; i <= rei; i++)
                                {
                                    var v = mrows[i];
                                    var rt = new SKRect(rtScrollContent.Left, v.Top, rtScrollContent.Right, v.Bottom);

                                    v.MouseClick(rt, x, y - (rtScrollContent.Top + vspos),
                                        lscol.Where(x => Columns.Contains(x.Column)).ToList(), Rows, mcols_col,
                                        hspos);
                                }
                            }
                            #endregion
                            #region Summary
                            if (rtSummary.HasValue)
                            {
                                for (int i = 0; i < SummaryRows.Count; i++)
                                {
                                    var v = SummaryRows[i];
                                    var rt = new SKRect(rtSummary.Value.Left, v.Top, rtSummary.Value.Right, v.Bottom);
                                    v.MouseClick(rt, x, y - (rtSummary.Value.Top));
                                }
                            }
                            #endregion
                        });
                    });
                }
                #endregion
            });
            base.OnMouseClick(x, y);
        }
        #endregion
        #region OnMouseLongClick
        protected override void OnMouseLongClick(float x, float y)
        {
            Areas((rtContent, rtColumn, rtScrollContent, rtSummary, rtScrollV, rtScrollH, rtScrollR) =>
            {
                #region Column / Rows
                if (dx.HasValue && dy.HasValue && MathTool.GetDistance(new SKPoint(dx.Value, dy.Value), new SKPoint(x, y)) < 5)
                {
                    #region var
                    var vspos = Convert.ToSingle(vscroll.ScrollPositionWithOffset);
                    var hspos = Convert.ToSingle(hscroll.ScrollPositionWithOffset);
                    var bAllSelect = mrows.Where(x => x.Selected).Count() > 0;
                    var bUseFilter = Columns.Where(x => x.UseFilter).Count() > 0;

                    var lsc = mcols_col;
                    #endregion

                    ColIndex(rtScrollContent, (csi, cei) =>
                    {
                        RowIndex(rtScrollContent, (rsi, rei) =>
                        {
                            #region var
                            var lscol = mcols.Where(x => CollisionTool.CheckHorizon(rtScrollContent.Left, rtScrollContent.Right,
                                                            x.Bounds.Left + (!x.Column.Fixed ? hspos : 0), x.Bounds.Right + (!x.Column.Fixed ? hspos : 0))).ToList();
                            #endregion
                            #region Column
                            foreach (var v in lscol.OrderBy(x => x.Column.Fixed))
                            {
                                var rtb = v.GetBounds(hspos);
                                var rtbf = v.GetBoundsFilter(hspos);

                                v.Column.MouseLongClick(rtb, rtbf, x, y - rtColumn.Top);
                            }
                            #endregion
                            #region Row
                            {
                                for (int i = rsi; i <= rei; i++)
                                {
                                    var v = mrows[i];
                                    var rt = new SKRect(rtScrollContent.Left, v.Top, rtScrollContent.Right, v.Bottom);

                                    v.MouseLongClick(rt, x, y - (rtScrollContent.Top + vspos),
                                        lscol.Where(x => Columns.Contains(x.Column)).ToList(), Rows, mcols_col,
                                        hspos);
                                }
                            }
                            #endregion
                            #region Summary
                            if (rtSummary.HasValue)
                            {
                                for (int i = 0; i < SummaryRows.Count; i++)
                                {
                                    var v = SummaryRows[i];
                                    var rt = new SKRect(rtSummary.Value.Left, v.Top, rtSummary.Value.Right, v.Bottom);
                                    v.MouseLongClick(rt, x, y - (rtSummary.Value.Top));
                                }
                            }
                            #endregion
                        });
                    });
                }
                #endregion
            });
            base.OnMouseLongClick(x, y);
        }
        #endregion
        #region OnMouseDoubleClick
        protected override void OnMouseDoubleClick(float x, float y)
        {
            Areas((rtContent, rtColumn, rtScrollContent, rtSummary, rtScrollV, rtScrollH, rtScrollR) =>
            {
                #region Column / Rows
                if (dx.HasValue && dy.HasValue && MathTool.GetDistance(new SKPoint(dx.Value, dy.Value), new SKPoint(x, y)) < 5)
                {
                    #region var
                    var vspos = Convert.ToSingle(vscroll.ScrollPositionWithOffset);
                    var hspos = Convert.ToSingle(hscroll.ScrollPositionWithOffset);
                    var bAllSelect = mrows.Where(x => x.Selected).Count() > 0;
                    var bUseFilter = Columns.Where(x => x.UseFilter).Count() > 0;

                    var lsc = mcols_col;
                    #endregion

                    ColIndex(rtScrollContent, (csi, cei) =>
                    {
                        RowIndex(rtScrollContent, (rsi, rei) =>
                        {
                            #region var
                            var lscol = mcols.Where(x => CollisionTool.CheckHorizon(rtScrollContent.Left, rtScrollContent.Right,
                                                            x.Bounds.Left + (!x.Column.Fixed ? hspos : 0), x.Bounds.Right + (!x.Column.Fixed ? hspos : 0))).ToList();
                            #endregion
                            #region Column
                            foreach (var v in lscol.OrderBy(x => x.Column.Fixed))
                            {
                                var rtb = v.GetBounds(hspos);
                                var rtbf = v.GetBoundsFilter(hspos);

                                v.Column.MouseDoubleClick(rtb, rtbf, x, y - rtColumn.Top);
                            }
                            #endregion
                            #region Row
                            {
                                for (int i = rsi; i <= rei; i++)
                                {
                                    var v = mrows[i];
                                    var rt = new SKRect(rtScrollContent.Left, v.Top, rtScrollContent.Right, v.Bottom);

                                    v.MouseDoubleClick(rt, x, y - (rtScrollContent.Top + vspos),
                                        lscol.Where(x => Columns.Contains(x.Column)).ToList(), Rows, mcols_col,
                                        hspos);
                                }
                            }
                            #endregion
                            #region Summary
                            if (rtSummary.HasValue)
                            {
                                for (int i = 0; i < SummaryRows.Count; i++)
                                {
                                    var v = SummaryRows[i];
                                    var rt = new SKRect(rtSummary.Value.Left, v.Top, rtSummary.Value.Right, v.Bottom);
                                    v.MouseDoubleClick(rt, x, y - (rtSummary.Value.Top));
                                }
                            }
                            #endregion
                        });
                    });
                }
                #endregion
            });
            base.OnMouseClick(x, y);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect, SKRect, SKRect?, SKRect?, SKRect?, SKRect?> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);

            var scwh = Convert.ToInt32(Scroll.SC_WH);
            var columnH = (colrc + (Columns.Where(x => x.UseFilter).Count() > 0 ? 1 : 0)) * ColumnHeight;
            var summaryH = SummaryRows.Count * RowHeight;

            var rtColumn = Util.FromRect(rtContent.Left, rtContent.Top, rtContent.Width, columnH);
            if (ScrollMode == ScrollMode.Both)
            {
                var rtBox = Util.FromRect(rtColumn.Left, rtColumn.Bottom, rtContent.Width - scwh, rtContent.Height - columnH - scwh);
                var rtSummary = Util.FromRect(rtBox.Left, rtBox.Bottom - summaryH, rtContent.Width - scwh, summaryH);
                var rtScrollContent = Util.FromRect(rtBox.Left, rtBox.Top, rtBox.Width, rtBox.Height - summaryH);
                var rtScrollV = Util.FromRect(rtBox.Right, rtBox.Top, scwh, rtBox.Height);
                var rtScrollH = Util.FromRect(rtBox.Left, rtBox.Bottom, rtBox.Width, scwh);
                var rtScrollR = Util.FromRect(rtScrollH.Right, rtScrollV.Bottom, scwh, scwh);

                act(rtContent, rtColumn, rtScrollContent, summaryH > 0 ? rtSummary : null, rtScrollV, rtScrollH, rtScrollR);
            }
            else if(ScrollMode == ScrollMode.Vertical)
            {
                var rtBox = Util.FromRect(rtColumn.Left, rtColumn.Bottom, rtContent.Width - scwh, rtContent.Height - columnH);
                var rtSummary = Util.FromRect(rtBox.Left, rtBox.Bottom - summaryH, rtContent.Width - scwh, summaryH);
                var rtScrollContent = Util.FromRect(rtBox.Left, rtBox.Top, rtBox.Width, rtBox.Height - summaryH);
                var rtScrollV = Util.FromRect(rtBox.Right, rtBox.Top, scwh, rtBox.Height);

                act(rtContent, rtColumn, rtScrollContent, summaryH > 0 ? rtSummary : null, rtScrollV, null, null);
            }
            else if(ScrollMode == ScrollMode.Horizon)
            {
                var rtBox = Util.FromRect(rtColumn.Left, rtColumn.Bottom, rtContent.Width, rtContent.Height - columnH - scwh);
                var rtSummary = Util.FromRect(rtBox.Left, rtBox.Bottom - summaryH, rtContent.Width, summaryH);
                var rtScrollContent = Util.FromRect(rtBox.Left, rtBox.Top, rtBox.Width, rtBox.Height - summaryH);
                var rtScrollH = Util.FromRect(rtBox.Left, rtBox.Bottom, rtBox.Width, scwh);

                act(rtContent, rtColumn, rtScrollContent, summaryH > 0 ? rtSummary : null, null, rtScrollH, null);
            }
            
        }
        #endregion

        #region Invoke
        internal void InvokeSelectedChanged() => SelectedChanged?.Invoke(this, null);
        internal void InvokeSortChanged() => SortChanged?.Invoke(this, null);

        internal void InvokeColumnMouseClick(DvDataGridColumn col, float x, float y) => ColumnMouseClick?.Invoke(this, new ColumnMouseEventArgs(col, x, y));
        internal void InvokeCellMouseClick(DvDataGridCell cell, float x, float y) => CellMouseClick?.Invoke(this, new CellMouseEventArgs(cell, x, y));
        internal void InvokeCellMouseDoubleClick(DvDataGridCell cell, float x, float y) => CellMouseDoubleClick?.Invoke(this, new CellMouseEventArgs(cell, x, y));
        internal void InvokeCellButtonClick(DvDataGridCell cell) => CellButtonClick?.Invoke(this, new CellButtonClickEventArgs(cell));
        internal void InvokeValueChanged(DvDataGridCell cell, object oldVal, object newVal) => ValueChanged?.Invoke(this, new CellValueChangedEventArgs(cell, oldVal, newVal));
        #endregion

        #region DataSource
        #region SetDataSource<T>
        public void SetDataSource<T>(IEnumerable<T> values)
        {
            objs = values;
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
                                var cell = Activator.CreateInstance(col.CellType, this, row, col) as DvDataGridCell;
                                row.Cells.Add(cell);
                            }
                        }

                        Rows.Add(row);
                        ri++;
                    }
                }
                
                RefreshRows();
            }
            else throw new Exception("VALID COUNT");
        }
        #endregion
        #endregion

        #region Column
        #region MakeColumnInfo
        #region MakeColumnTree
        void MakeColumnTree(List<DGColumnInfo> lsg, List<DGColumnInfo> lsc, DGColumnInfo col, List<DGColumnInfo> ls)
        {
            ls.Add(col);

            if (col.Column.GroupName != null)
            {
                var v = lsg.Where(x => x.Column.Name == col.Column.GroupName).FirstOrDefault();
                if (v != null) MakeColumnTree(lsg, lsc, v, ls);
            }
        }
        #endregion
        #region MakeColumnChilds
        void MakeColumnChilds(List<DGColumnInfo> lsg, List<DGColumnInfo> lsc, DGColumnInfo col, List<DGColumnInfo> ls)
        {
            if (col != null)
            {
                if (lsc.Contains(col)) { ls.Add(col); }
                else
                {
                    if (lsc.Count(x => x.Column.GroupName == col.Column.Name) > 0) ls.AddRange(lsc.Where(x => x.Column.GroupName == col.Column.Name));
                    else if (lsg.Count(x => x.Column.GroupName == col.Column.Name) > 0)
                    {
                        var vls = lsg.Where(x => x.Column.GroupName == col.Column.Name);
                        foreach (var v in vls) MakeColumnChilds(lsg, lsc, v, ls);
                    }
                }
            }
        }
        #endregion
        #region MakeColumnInfo 
        void MakeColumnInfo()
        {
            #region MakeList
            mcols.Clear();

            mcols.AddRange(Columns.Select(x => new DGColumnInfo { Column = x }));
            mcols.AddRange(ColumnGroups.Select(x => new DGColumnInfo { Column = x }));
            
            var lsc = mcols.Where(x => Columns.Contains(x.Column)).ToList();
            var lsg = mcols.Where(x => ColumnGroups.Contains(x.Column)).ToList();

            foreach (var v in mcols)
            {
                v.Parent = mcols.Where(x => x.Column.Name == v.Column.GroupName).FirstOrDefault();
                v.Childs.AddRange(mcols.Where(x => x.Column.GroupName == v.Column.Name));

                MakeColumnChilds(lsg, lsc, v, v.ColumnChilds);
                MakeColumnTree(lsg, lsc, v, v.ColumnTree);
            }

            colrc = mcols.Max(x => x.ColumnTree.Count);

            foreach (var v in mcols.OrderByDescending(x=>x.ColumnTree.Count))
            {
                if (Columns.Contains(v.Column))
                {
                    v.ColIndex = Columns.IndexOf(v.Column);
                    v.RowIndex = colrc - 1;
                }
                else if (ColumnGroups.Contains(v.Column))
                {
                    v.ColIndex = v.ColumnChilds.Count > 0 ? v.ColumnChilds.First().ColIndex : 0;
                    v.Colspan = v.ColumnChilds.Count > 1 ? v.ColumnChilds.Count : 1;

                    if (v.Parent == null)
                    {
                        v.RowIndex = 0;
                        var vs = v.ColumnChilds.FirstOrDefault();
                        if (vs != null) v.Rowspan = colrc - vs.ColumnTree.Count + 1;
                    }
                    else
                    {
                        var vs = v.Childs.FirstOrDefault();
                        if (vs != null) v.RowIndex = vs.RowIndex - 1;
                    }
                }
            }
            #endregion

            Areas((rtContent, rtColumn, rtScrollContent, rtSummary, rtScrollV, rtScrollH, rtScrollR) =>
            {
                #region rts
                var ixo = SelectionMode == DvDataGridSelectionMode.SELECTOR ? 1 : 0;
                var lsz = Columns.Select(x => x.Size).ToList();
                if (SelectionMode == DvDataGridSelectionMode.SELECTOR) lsz.Insert(0, new SizeInfo(DvSizeMode.Pixel, SPECIAL_CELL_WIDTH));
                var rts = Util.DevideSizeH(rtScrollContent, lsz);
                #endregion
                #region Column
                var useFilter = Columns.Where(x => x.UseFilter).Count() > 0;
                foreach (var v in lsc)
                {
                    var rt = rts[v.ColIndex + ixo];
                    v.Bounds = Util.FromRect(rt.Left, rtColumn.Top + (v.RowIndex * ColumnHeight), rt.Width, ColumnHeight);
                    if (useFilter) v.BoundsFilter = Util.FromRect(rt.Left, rtColumn.Top + ((v.RowIndex + 1) * ColumnHeight), rt.Width, ColumnHeight);
                }
                #endregion
                #region ColumnGroup
                foreach (var v in lsg)
                {
                    int imin = v.ColumnChilds.Min(x => x.ColIndex) + ixo;
                    int imax = v.ColumnChilds.Max(x => x.ColIndex) + ixo;

                    var x = rts[imin].Left;
                    var y = rtColumn.Top + (v.RowIndex * ColumnHeight);
                    var w = rts[imax].Right - rts[imin].Left;
                    var h = ColumnHeight * v.Rowspan;

                    v.Bounds = Util.FromRect(x, y, w, h);
                }
                #endregion

                mcols_col = mcols.Where(x => Columns.Contains(x.Column)).OrderBy(x=>x.ColIndex).ToList();
                mcols_grp = mcols.Where(x => ColumnGroups.Contains(x.Column)).ToList();

                hST = Columns.Where(x => !x.Fixed).Select(x => rts[Columns.IndexOf(x) + ixo].Width).Sum();
                hSV = rtScrollContent.Width - (Columns.Where(x => x.Fixed).Select(x => rts[Columns.IndexOf(x) + ixo].Width).Sum() + (SelectionMode == DvDataGridSelectionMode.SELECTOR ? ColumnHeight : 0));
                vSV = rtScrollContent.Height;

            });
        }
        #endregion
        #endregion

        #region ColIndex
        void ColIndex(SKRect rtScrollContent, Action<int, int> act)
        {
            var si = searchH(0, mcols_col.Count - 1, rtScrollContent.Left);
            var ei = searchH(0, mcols_col.Count - 1, rtScrollContent.Right);

            act(Math.Max(si, 0), Math.Min(ei + 1, mcols_col.Count - 1));
        }
        #endregion
        #endregion

        #region Row
        #region GetRows
        public List<DvDataGridRow> GetRows() => mrows;
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
            #region Rows
            {
                var lsv = mrows;
                var sum = lsv.Sum(x => x.RowHeight);
                //if (PrevTotalHeight != sum)
                {
                    //vST = PrevTotalHeight = sum;
                    vST = sum;

                    var y = 0F;
                    var ri = 1;
                    foreach (var v in lsv)
                    {
                        v.RowIndex = ri;
                        v.Top = y;
                        v.Bottom = y + v.RowHeight;

                        y += v.RowHeight;
                        ri++;
                    }
                }
            }
            #endregion
            #region Summary
            {
                var y = 0F;
                var ri = 1;
                foreach (var row in SummaryRows)
                {
                    row.RowIndex = ri; 
                    row.Top = y;
                    row.Bottom = y + RowHeight;

                    foreach (var v in row.Cells.Select(x => x as DvDataGridSummaryCell))
                        if (v != null)
                            v.Calculate();

                    y += RowHeight;
                    ri++;
                }
            }
            #endregion
        }
        #endregion

        #region RowIndex
        void RowIndex(SKRect rtScrollContent, Action<int, int> act)
        {
            var spos = Convert.ToSingle(vscroll.ScrollPositionWithOffset);

            var si = searchV(0, mrows.Count - 1,  0);
            var ei = searchV(0, mrows.Count - 1, rtScrollContent.Height);

            act(Math.Max(si, 0), Math.Min(ei + 1, mrows.Count - 1));
        }
        #endregion
        #endregion

        #region Color
        public SKColor GetForeColor(DvTheme Theme) => this.ForeColor ?? Theme.ForeColor;
        public SKColor GetBackColor() => ParentContainer.GetBackColor();
        public SKColor GetColumnColor(DvTheme Theme) => this.ColumnColor ?? Theme.ColumnColor;
        public SKColor GetSelectedColor(DvTheme Theme) => this.SelectedColor ?? Theme.PointColor;
        public SKColor GetSummaryColor(DvTheme Theme) => this.SummaryColor ?? Theme.SummaryColor;
        public SKColor GetBoxColor(DvTheme Theme) => this.BoxColor ?? Theme.ListBackColor;
        public SKColor GetBorderColor(DvTheme Theme) => Theme.GetBorderColor(GetColumnColor(Theme), GetBackColor());
        public SKColor GetInputBoxColor(DvTheme Theme) => GetBoxColor(Theme).BrightnessTransmit(-0.25F);
        public SKColor GetInputBoxBorderColor(DvTheme Theme) => Theme.GetBorderColor(GetInputBoxColor(Theme), GetBoxColor(Theme));
        public SKColor GetInputColumnColor(DvTheme Theme) => GetColumnColor(Theme).BrightnessTransmit(-0.25F);
        public SKColor GetInputColumnBorderColor(DvTheme Theme) => Theme.GetBorderColor(GetInputColumnColor(Theme), GetColumnColor(Theme));
        #endregion

        #region Private
        #region searchH
        private int searchH(int si, int ei, float pos)
        {
            var spos = Convert.ToSingle(hscroll.ScrollPositionWithOffset);
            int idx = MathTool.Center(si, ei);
            if (si != ei && idx != si && idx != ei)
            {
                if (idx >= 0 && idx < mcols_col.Count)
                {
                    if (pos > mcols_col[idx].Bounds.Right + spos)
                    {
                        return searchH(idx, ei, pos);
                    }
                    else if (pos < mcols_col[idx].Bounds.Left + spos)
                    {
                        return searchH(si, idx, pos);
                    }
                    else return idx;
                }
                return idx;
            }
            else return idx;
        }
        #endregion
        #region searchV
        private int searchV(int si, int ei, float pos)
        {
            var spos = Convert.ToSingle(vscroll.ScrollPositionWithOffset);
            int idx = MathTool.Center(si, ei);
            if (si != ei && idx != si && idx != ei)
            {
                if (idx >= 0 && idx < mrows.Count)
                {
                    if (pos > mrows[idx].Bottom + spos)
                    {
                        return searchV(idx, ei, pos);
                    }
                    else if (pos < mrows[idx].Top + spos)
                    {
                        return searchV(si, idx, pos);
                    }
                    else return idx;
                }
                return idx;
            }
            else return idx;
        }
        #endregion
        #endregion

        #region Internal
        internal void ScrollClear()
        {
            vscroll.Clear();
            hscroll.Clear();
        }
        #endregion
        #endregion
    }

    #region class : DGColumnInfo
    class DGColumnInfo
    {
        public DGColumnInfo Parent { get; set; }
        public List<DGColumnInfo> Childs { get; } = new List<DGColumnInfo>();

        public DvDataGridColumn Column { get; set; }
        public List<DGColumnInfo> ColumnTree { get; } = new List<DGColumnInfo>();
        public List<DGColumnInfo> ColumnChilds { get; } = new List<DGColumnInfo>();

        public SKRect Bounds { get; set; }
        public SKRect? BoundsFilter { get; set; } = null;

        public int ColIndex { get; set; }
        public int RowIndex { get; set; }
        public int Colspan { get; set; } = 1;
        public int Rowspan { get; set; } = 1;

        public override string ToString() => Column?.Name;

        public SKRect GetBounds(float hspos) => Util.FromRect(Bounds.Left + (Column?.Fixed ?? false ? 0 : hspos), Bounds.Top, Bounds.Width, Bounds.Height);
        public SKRect? GetBoundsFilter(float hspos) => (BoundsFilter.HasValue ? Util.FromRect(BoundsFilter.Value.Left + (Column?.Fixed ?? false ? 0 : hspos), BoundsFilter.Value.Top , BoundsFilter.Value.Width, BoundsFilter.Value.Height) : null);
    }
    #endregion

}

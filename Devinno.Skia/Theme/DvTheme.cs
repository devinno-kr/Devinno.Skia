using Devinno.Collections;
using Devinno.Skia.Collections;
using Devinno.Skia.Controls;
using Devinno.Skia.Design;
using Devinno.Skia.Extensions;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Theme
{
    public abstract class DvTheme
    {
        #region Const
        public const float GP = 10F;
        #endregion

        #region Properties
        public abstract float Corner { get; }
        public abstract SKColor ForeColor { get; }
        public abstract SKColor BackColor { get; }
        public abstract SKColor ButtonColor { get; }
        public abstract SKColor LabelColor { get; }
        public abstract SKColor InputColor { get; }
        public abstract SKColor TitleColor { get; }
        public abstract SKColor WindowTitleColor { get; }
        public abstract SKColor PointColor { get; }
        public abstract SKColor ControlBackColor { get; }
        public abstract SKColor PanelColor { get; }
        public abstract SKColor GroupBoxColor { get; }
        public abstract SKColor ScrollBarColor { get; }
        public abstract SKColor ScrollCursorOffColor { get; }
        public abstract SKColor ScrollCursorOnColor { get; }
        public abstract SKColor NeedelColor { get; }
        public abstract SKColor GridColor { get; }
        public abstract SKColor ListBackColor { get; }
        public abstract SKColor ColumnColor { get; }
        public abstract SKColor SummaryColor { get; }

        public abstract byte OutShadowAlpha { get; }
        public abstract byte OutBevelAlpha { get; }
        public abstract byte InShadowAlpha { get; }
        public abstract float GradientLight { get; }
        public abstract float GradientDark { get; }
        public abstract float DownBrightness { get; }
        public abstract float BorderBrightness { get; }
        #endregion

        #region Method : Control Draw
        #region DrawText
        public abstract void DrawText(SKCanvas canvas, SKRect rect, string fontName, float fontSize, DvFontStyle fontStyle, string text, SKColor foreColor, DvContentAlignment contentAlignment = DvContentAlignment.MiddleCenter, bool down = false);
        public abstract void DrawText(SKCanvas canvas, SKRect rect, string iconString, float iconSize, SKColor foreColor, DvContentAlignment contentAlignment = DvContentAlignment.MiddleCenter, bool down = false);
        public abstract void DrawText(SKCanvas canvas, SKRect rect, DvTextIcon texticon, SKColor foreColor, DvContentAlignment contentAlignment = DvContentAlignment.MiddleCenter, bool down = false);
        #endregion
        #region DrawButton
        public abstract void DrawButton(SKCanvas canvas,
            SKRect rect,
            SKColor buttonColor, SKColor borderColor, SKColor foreColor, SKColor backColor,
            DvTextIcon texticon,
            DvRoundType round, bool gradient, bool backgroundDraw,
            DvContentAlignment align,
            bool down);
        #endregion
        #region DrawLabel
        public abstract void DrawLabel(SKCanvas canvas,
            SKRect rect,
            SKColor labelColor, SKColor borderColor, SKColor foreColor, SKColor backColor,
            DvTextIcon texticon,
            DvRoundType round, bool backgroundDraw,
            DvContentAlignment align, bool drawBorder = true);

        #endregion
        #region DrawTitle
        public abstract void DrawTitle(SKCanvas canvas,
            SKRect rect,
            SKColor labelColor, SKColor borderColor, SKColor foreColor, SKColor backColor,
            DvTextIcon texticon,
            DvRoundType round, bool backgroundDraw);
        #endregion
        #region DrawValueOnOff
        public abstract void DrawValueOnOff(SKCanvas canvas,
            SKRect rect,
            SKRect rtOn, SKRect rtOff,
            SKColor foreColor,
            string onText, string offText, string fontName, float fontSize, DvFontStyle fontStyle,
            bool onOff,
            bool useAnimation, Animation ani);
         
        #endregion
        #region DrawTriangleButton
        public abstract void DrawTriangleButton(SKCanvas canvas,
            SKRect rect, SKRect rtBox,
            SKColor buttonColor, SKColor buttonBackColor,
            DvDirection dir, bool gradient, float buttonPadding, float corner,
            bool down);

        #endregion
        #region DrawCircleButton
        public abstract void DrawCircleButton(SKCanvas canvas,
            SKRect rect, SKRect rtCircleBack, SKRect rtCircle, SKRect rtText,
            SKColor buttonColor, SKColor buttonBackColor,
            bool gradient, bool down);
        #endregion
        #region DrawCheckBox
        public abstract void DrawCheckBox(SKCanvas canvas,
            SKRect rect, float BoxSize,
            SKColor boxColor, SKColor borderColor, SKColor checkColor, SKColor foreColor,
            string text, string fontName, float fontSize, DvFontStyle fontStyle, DvContentAlignment align,
            bool check);

        #endregion
        #region DrawRadioBox
        public abstract void DrawRadioBox(SKCanvas canvas,
            SKRect rect, float BoxSize,
            SKColor boxColor, SKColor borderColor, SKColor checkColor, SKColor foreColor,
            string text, string fontName, float fontSize, DvFontStyle fontStyle, DvContentAlignment align,
            bool check);
        #endregion
        #region DrawLamp
        public abstract void DrawLamp(SKCanvas canvas,
            SKRect rect,
            SKColor onLampColor, SKColor offLampColor, SKColor foreColor, SKColor backColor, SKColor borderColor,
            string text, string fontName, float fontSize, DvFontStyle fontStyle,
            int lampSize, DvTextIconAlignment lampAlignment, DvContentAlignment contentAlignment, bool onOff,
            bool useAnimation, Animation ani);

        #endregion
        #region DrawOnOff
        public abstract void DrawOnOff(SKCanvas canvas,
            SKRect rect, SKRect rtArea, SKRect rtCursor, SKRect rtOn, SKRect rtOff, float cursorWidth, float onoffWidth,
            SKColor CursorColor, SKColor OnBoxColor, SKColor OffBoxColor, SKColor OnTextColor, SKColor OffTextColor, SKColor BackColor,
            string onText, string offText, string fontName, float fontSize, DvFontStyle fontStyle,
            bool OnOff, bool bCurDown);
        #endregion
        #region DrawSwitch
        public abstract void DrawSwitch(SKCanvas canvas,
           SKRect rtContent, SKRect rtSwitch, SKRect rtOn, SKRect rtOff, SKRect rtOnText, SKRect rtOffText,
           SKColor switchColor, SKColor onBoxColor, SKColor offBoxColor, SKColor foreColor,
           DvTextIcon OnTextIcon, DvTextIcon OffTextIcon,
           float Corner, bool OnOff, bool useAnimation, Animation ani);
        #endregion
        #region DrawProgress
        public abstract void DrawProgress(SKCanvas canvas,
            SKRect rtContent, SKRect rtEmpty, SKRect rtFill,
            SKColor boxColor, SKColor barColor, SKColor foreColor, SKColor backColor,
            DvDirectionHV direction, bool drawText, bool reverse,
            string text, string fontName, float fontSize, DvFontStyle fontStyle);
        #endregion
        #region DrawSlider
        public abstract void DrawSlider(SKCanvas canvas,
            SKRect rtContent, SKRect rtEmpty, SKRect rtFill, SKRect rtCur,
            SKColor boxColor, SKColor barColor, SKColor cursorColor, SKColor graduationColor, SKColor foreColor, SKColor backColor,
            DvDirectionHV direction, bool reverse,
            double min, double max, double? tick, float barSize, float cursorSize,
            string text, string fontName, float fontSize, DvFontStyle fontStyle,
            bool bCurDown);
        #endregion
        #region DrawRangeSlider
        public abstract void DrawRangeSlider(SKCanvas canvas,
            SKRect rtContent, SKRect rtEmpty, SKRect rtFill, SKRect rtCurStart, SKRect rtCurEnd,
            SKColor boxColor, SKColor barColor, SKColor cursorColor, SKColor graduationColor, SKColor foreColor, SKColor backColor,
            DvDirectionHV direction, bool reverse,
            double min, double max, double? tick, float barSize, float cursorSize,
            string textS, string textE, string fontName, float fontSize, DvFontStyle fontStyle,
            bool bRangeStartCurDown, bool bRangeEndCurDown);
        #endregion
        #region DrawStepGauge
        public abstract void DrawStepGauge(SKCanvas canvas,
            SKRect rtContent, SKRect? rtBtnL, SKRect[] rtBtns, SKRect? rtBtnR,
            SKColor onColor, SKColor offColor, SKColor buttonColor, SKColor foreColor, SKColor backColor,
            int stepCount, int step, bool UseButton, bool DrawButton, float StepPadding,
            DvIcon leftIcon, DvIcon rightIcon, bool bLeftDown, bool bRightDown);
        #endregion
        #region DrawGauge
        public abstract void DrawGauge(SKCanvas canvas,
            SKRect rtContent, SKRect rtGauge, SKRect rtValue, SKRect rtTitle,
            SKColor fillColor, SKColor emptyColor, SKColor foreColor, SKColor backColor,
            double minimum, double maximum, double value,
            string text, string valueFontName, float valueFontSize, DvFontStyle valueFontStyle,
            string title, string titleFontName, float titleFontSize, DvFontStyle titleFontStyle,
            float startAngle, float sweepAngle, float barSize, float barPadding);
        #endregion
        #region DrawMeter
        public abstract void DrawMeter(SKCanvas canvas,
            SKRect rtContent, SKRect rtGauge, SKRect rtValue, SKRect rtTitle,
            SKColor needleColor, SKColor needlePointColor, SKColor foreColor, SKColor backColor,
            double minimum, double maximum, double value, double graduationLarge, double graduationSmall,
            bool drawText,
            string text, string valueFontName, float valueFontSize, DvFontStyle valueFontStyle,
            string title, string titleFontName, float titleFontSize, DvFontStyle titleFontStyle,
            string remarkFontName, float remarkFontSize, DvFontStyle remarkFontStyle,
            float startAngle, float sweepAngle, List<MeterBar> bars);

        #endregion
        #region DrawKnob
        public abstract void DrawKnob(SKCanvas canvas,
            SKRect rtContent, SKRect rtKnobBack, SKRect rtKnob, SKRect rtCursor,
            SKColor knobColor, SKColor knobBackColor, SKColor fillColor, SKColor onLampColor, SKColor offLampColor, SKColor foreColor, SKColor backColor,
            double minimum, double maximum, double value, double? tick,
            float startAngle, float sweepAngle, bool cursorDownState, float knobPadding);

        #endregion
        #region DrawPictureBox
        public abstract void DrawPictureBox(SKCanvas canvas,
            SKRect rtContent, SKColor boxColor, SKColor backColor, PictureScaleMode scaleMode, SKBitmap image);
        #endregion
        #region DrawSelector
        public abstract void DrawSelector(SKCanvas canvas,
            SKRect rtContent, SKRect rtPrev, SKRect rtValue, SKRect rtNext, List<SKRect> rtItems,
            SKColor selectorColor, SKColor foreColor, SKColor backColor,
            string fontName, float fontSize, DvFontStyle fontStyle,
            float iconSize, float iconGap, DvTextIconAlignment iconAlignment,
            List<SelectorItem> items, bool backgroundDraw, DvDirectionHV direction, DvRoundType? Round,
            int selectedIndex,
            bool bPrev, bool bNext, bool useAnimation, Animation ani);
        #endregion
        #region DrawCalendar
        public abstract void DrawCalendar(SKCanvas canvas,
            SKRect rtContent, SKRect rtMonthly, SKRect rtWeekly, SKRect rtDays, SKRect rtMonthPrev, SKRect rtMonthNext, SKRect rtMonthText,
            Dictionary<string, SKRect> dicBoxes, Dictionary<string, SKRect> dicWeeks,
            SKColor boxColor, SKColor selectColor, SKColor foreColor, SKColor backColor,
            string monthText, string fontName, float fontSize, DvFontStyle fontStyle,
            int currentYear, int currentMonth, List<DateTime> selectedDays,
            bool backgroundDraw,
            bool bMonthPrev, bool bMonthNext);
        #endregion
        #region DrawInput
        public abstract void DrawInput(SKCanvas canvas, 
            SKRect rtContent, 
            SKColor inputColor, SKColor borderColor, SKColor foreColor, SKColor backColor, 
            DvRoundType round, bool backgroundDraw, 
            bool drawBorder = true);

        #endregion
        #region DrawBarGraph
        public abstract void DrawBarGraph(SKCanvas canvas,
            SKRect rtContent, SKRect rtRemark, SKRect rtNameAxis, SKRect rtValueAxis, SKRect rtGraph, SKRect rtScroll, List<SKSize> szRemarks,
            SKColor gridColor, SKColor graphBackColor, SKColor foreColor, SKColor backColor,
            string fontName, float fontSize, DvFontStyle fontStyle,
            BarGraphMode graphMode, DvDirectionHV direction, bool valueDraw, string formatString,
            bool gradient, int barSize, int barGap, int dataWH,
            List<GraphSeries> series, double graduation, double minimum, double maximum,
            Scroll scroll, bool scrollable, bool touchMode,
            List<GraphValue> graphDatas);
        #endregion
        #region DrawLineGraph
        public abstract void DrawLineGraph(SKCanvas canvas, 
            SKRect rtContent, SKRect rtRemark, SKRect rtNameAxis, SKRect rtValueAxis, SKRect rtGraph, SKRect rtScroll, List<SKSize> szRemarks, 
            SKColor gridColor, SKColor graphBackColor, SKColor foreColor, SKColor backColor, 
            string fontName, float fontSize, DvFontStyle fontStyle, 
            bool valueDraw, string formatString,
            bool pointDraw, int dataWH,
            List<GraphSeries> series, double graduation, double minimum, double maximum, 
            Scroll scroll, bool scrollable, bool touchMode, 
            List<GraphValue> graphDatas);
        #endregion
        #region DrawCircleGraph
        public abstract void DrawCircleGraph(SKCanvas canvas,
            SKRect rtContent, SKRect rtGraph, SKRect rtSelectLabel, SKRect rtSelectLeft, SKRect rtSelectRight,
            SKColor foreColor, SKColor backColor,
            List<GraphSeries> series, bool gradient,
            string fontName, float fontSize, float nameFontSize, float valueFontSize, DvFontStyle fontStyle,
            List<GraphValue> graphDatas, string formatString, 
            bool bLeftSel, bool bRightSel, SKPoint mpt, int nSelectedIndex, DateTime prev);
        #endregion
        #region DrawTimeGraph
        public abstract void DrawTimeGraph(SKCanvas canvas,
           SKRect rtContent, SKRect rtRemark, SKRect rtNameAxis, SKRect rtValueTitle, SKRect rtValueAxis, SKRect rtGraph, SKRect rtScroll, List<SKSize> szRemarks,
           SKColor gridColor, SKColor graphBackColor, SKColor foreColor, SKColor backColor,
           string fontName, float fontSize, DvFontStyle fontStyle,
           TimeSpan xAxisGraduation, int yAxisGraduationCount,
           bool xAxisGridDraw, bool yAxisGridDraw, string valueFormatString, string timeFormatString,
           TimeSpan xScale, List<GraphSeries2> series,
           Scroll scroll, bool scrollable, bool touchMode,
           List<TimeGraphValue> graphDatas);
        #endregion
        #region DrawTrendGraph
        public abstract void DrawTrendGraph(SKCanvas canvas, 
            SKRect rtContent, SKRect rtRemark, SKRect rtNameAxis, SKRect rtValueTitle, SKRect rtValueAxis, SKRect rtGraph, SKRect rtScroll, List<SKSize> szRemarks, 
            SKColor gridColor, SKColor graphBackColor, SKColor foreColor, SKColor backColor, 
            string fontName, float fontSize, DvFontStyle fontStyle, 
            TimeSpan xAxisGraduation, int yAxisGraduationCount, 
            bool xAxisGridDraw, bool yAxisGridDraw, string valueFormatString, string timeFormatString, 
            TimeSpan xScale, List<GraphSeries2> series, 
            Scroll scroll, bool scrollable, bool touchMode, 
            List<TimeGraphValue> graphDatas, DateTime firstAppendTime);
        #endregion
        #region DrawListBox
        public abstract void DrawListBox(SKCanvas canvas,
            SKRect rtContent, SKRect rtBox, SKRect rtScroll,
            SKColor boxColor, SKColor selectedColor, SKColor foreColor, SKColor backColor,
            bool backgroundDraw, DvContentAlignment itemAlignment, DvRoundType round,
            Scroll scroll, bool drawScroll,
            Action<SKRect, Action<SKRect, ListBoxItem>> loop);
        #endregion
        #region DrawToolBox
        public abstract void DrawToolBox(SKCanvas canvas,
            SKRect rtContent, SKRect rtBox, SKRect rtScroll,
            SKColor boxColor, SKColor categoryColor, SKColor foreColor, SKColor backColor,
            bool backgroundDraw, float iconSize, int itemHeight, DvRoundType round, 
            string fontName, float fontSize, DvFontStyle fontStyle,
            Scroll scroll,
            Action<SKRect, Action<SKRect, ToolCategoryItem>> loop);


        public abstract void DrawToolBoxCategory(SKCanvas canvas,
            SKRect rtRow, SKRect rtt, SKRect rti,
            SKColor boxColor, SKColor categoryColor, SKColor foreColor, SKColor backColor,
            string text, string fontName, float fontSize, DvFontStyle fontStyle,
            string iconString, float iconSize, float iconGap, DvTextIconAlignment iconAlignment,
            bool expands, float itemHeight, 
            bool animation, Animation ani);


        public abstract void DrawToolBoxItem(SKCanvas canvas,
            SKRect rtv, SKRect rti, SKRect rtt,
            SKColor boxColor, SKColor foreColor,
            DvTextIcon v,
            float mx, float my, bool isDrag);

        #endregion
        #region DrawTreeView
        public abstract void DrawTreeView(SKCanvas canvas,
            SKRect rtContent, SKRect rtBox, SKRect rtScroll,
            SKColor boxColor, SKColor selectedColor, SKColor radioColor, SKColor foreColor, SKColor backColor,
            bool backgroundDraw, float iconSize, int itemHeight, DvRoundType round,
            string fontName, float fontSize, DvFontStyle fontStyle,
            Scroll scroll, Action<SKRect, Action<SKRect, TreeViewNode>> loop);


        public abstract void DrawTreeViewNode(SKCanvas canvas,
            SKRect rtRow, SKRect rti, SKRect rtt,
            SKColor boxColor, SKColor selectedColor, SKColor radioColor, SKColor radioBackColor, SKColor foreColor, SKColor backColor,
            DvTextIcon v, bool sel, TreeViewNodeCollection nodes, bool expands,
            bool animation, Animation ani);
        #endregion
        #region DrawComboBox
        public abstract void DrawComboBox(SKCanvas canvas,
            SKRect rtContent, SKRect rtIco, SKRect rtText,
            SKColor boxColor, SKColor foreColor, SKColor backColor, SKColor selectedColor, SKColor listBackColor,
            string fontName, float fontSize, DvFontStyle fontStyle, DvRoundType rnd,
            float iconSize, DvTextIconAlignment iconAlignment, float iconGap,
            List<ComboBoxItem> items, int selectedIndex,
            bool bOpen, bool reverse, bool backgroundDraw);
        #endregion
        #region DrawDataGridColumn
        public abstract void DrawDataGridColumn(SKCanvas canvas,
            SKRect bounds, SKRect? boundsFilter,
            SKColor foreColor, SKColor backColor, SKColor boxColor, SKColor borderColor, SKColor inputBoxColor, SKColor inputBorderColor,
            string text, string fontName, float fontSize, DvFontStyle fontStyle, 
            bool useFilter, string filterText, bool useSort, DvDataGridColumnSortState sortState, bool dash = true);
        #endregion
        #region DrawDataGridCell
        public abstract void DrawDataGridCell(SKCanvas canvas,
            SKRect bounds,
            SKColor foreColor, SKColor backColor, SKColor boxColor, SKColor borderColor, SKColor inputBoxColor, SKColor inputBoxBorderColor,
            bool dash = true);
        #endregion
        #region DrawDataGridInputBox
        public abstract void DrawDataGridInputBox(SKCanvas canvas, SKRect bounds);
        #endregion

        #region DrawPanel
        public abstract void DrawPanel(SKCanvas canvas,
            SKRect rtContent, SKRect rtPanel, SKRect rtTitle, SKRect rtText,
            SKColor panelColor, SKColor foreColor, SKColor backColor,
            DvTextIcon textIcon,
            bool drawTitle);
        #endregion
        #region DrawBoxPanel
        public abstract void DrawBoxPanel(SKCanvas canvas,
            SKRect rtContent, SKRect rtPanel, SKRect rtText,
            SKColor panelColor, SKColor foreColor, SKColor backColor,
            DvTextIcon textIcon,
            float corner, DvRoundType round);
        #endregion
        #region DrawBorderPanel
        public abstract void DrawBorderPanel(SKCanvas canvas,
            SKRect rtContent, SKRect rtPanel, SKRect rtTitle, SKRect rtText,
            SKColor borderPanelColor, SKColor foreColor, SKColor backColor,
            DvTextIcon textIcon,
            bool drawTitle, float borderWidth);
        #endregion
        #region DrawGroupBox
        public abstract void DrawGroupBox(SKCanvas canvas,
            SKRect rtContent, SKRect rtPanel, SKRect rtText,
            SKColor borderColor, SKColor foreColor, SKColor backColor,
            DvTextIcon textIcon,
            float borderWidth);
        #endregion
        #region DrawTabControl
        public abstract void DrawTabControl(SKCanvas canvas,
            SKRect rtContent, SKRect rtPage, SKRect rtNavi, Dictionary<string, SKRect> dicTab,
            SKColor foreColor, SKColor backColor, SKColor pointColor, SKColor tabColor, SKColor tabBackColor,
            string fontName, float fontSize, DvFontStyle fontStyle, float iconSize, float iconGap, DvTextIconAlignment iconAlignment,
            DvRoundType round, DvPosition tabPosition, int tabSize,
            DvSubPageCollection tabPages, DvSubPage nowSelTab, DvSubPage prevSelTab,
            bool useAnimation, Animation ani);
        #endregion

        #region DrawScroll
        public abstract void DrawScroll(SKCanvas canvas, SKRect rtScroll, SKColor backColor, Scroll scroll, DvRoundType round = DvRoundType.All);
        public abstract void DrawScrollR(SKCanvas canvas, SKRect rtScroll, SKColor backColor, Scroll scroll, DvRoundType round = DvRoundType.All);
        #endregion
        #endregion

        #region Method
        public abstract void DrawBox(SKCanvas canvas, SKRect rect, SKColor boxColor, SKColor borderColor, DvRoundType round, BoxStyle style, float corner, float inWidth = 1, float outWidth = 1, float borderWidth = 1);
        public abstract void DrawPath(SKCanvas canvas, SKPath path, SKColor boxColor, SKColor borderColor, BoxStyle style, float inWidth = 1, float outWidth = 1, float borderWidth = 1);
        public abstract SKColor GetBorderColor(SKColor fillColor, SKColor backColor);
        public abstract byte GetBevelAlpha(SKColor BaseColor);
        #endregion
    }

    #region enum : BoxStyle
    public enum BoxStyle : int
    {
        None = 0,
        Fill = 1,
        GradientV = 2, GradientV_R = 4, GradientH = 8, GradientH_R = 16,
        Border = 512,
        OutShadow = 1024, InShadow = 2048,
        InBevelLT = 4096, InBevel = 8192, OutBevel = 16384,
    }
    #endregion

}

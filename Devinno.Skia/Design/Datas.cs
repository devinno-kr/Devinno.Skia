using Devinno.Collections;
using Devinno.Skia;
using Devinno.Skia.Controls;
using Devinno.Skia.Icon;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Design
{
    #region Padding
    public struct Padding
    {
        public static readonly Padding Zero = new Padding(0);

        public int Left { get; set; }
        public int Right { get; set; }
        public int Top { get; set; }
        public int Bottom { get; set; }
 
        public Padding(int All)
        {
            Left = Right = Top = Bottom = All;
        }

        public Padding(int Left, int Top, int Right, int Bottom)
        {
            this.Left = Left;
            this.Top = Top;
            this.Right = Right;
            this.Bottom = Bottom;
        }
    }
    #endregion

    #region classes
    #region EventArgs
    #region class : MouseEventArgs
    public class MouseEventArgs: EventArgs
    {
        public float X { get; private set; }
        public float Y { get; private set; }

        public MouseEventArgs(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
    }
    #endregion
    #region class : ButtonsClickedventArgs
    public class ButtonsClickedEventArgs : EventArgs
    {
        public ButtonInfo Button { get; private set; }

        public ButtonsClickedEventArgs(ButtonInfo Button)
        {
            this.Button = Button;
        }
    }
    #endregion
    #region class : ButtonsSelectedventArgs
    public class ButtonsSelectedventArgs : EventArgs
    {
        public ButtonInfo Button { get; private set; }

        public ButtonsSelectedventArgs(ButtonInfo Button)
        {
            this.Button = Button;
        }
    }
    #endregion
    #region class : DragDropEventArgs 
    public class DragDropEventArgs : EventArgs
    {
        public object DragItem { get; }

        public DragDropEventArgs(object DragItem)
        {
            this.DragItem = DragItem;
        }
    }
    #endregion
    #region class : ListBoxItemClickedEventArgs
    public class ListBoxItemClickedEventArgs : EventArgs
    {
        public ListBoxItem Item { get; private set; }

        public ListBoxItemClickedEventArgs(ListBoxItem Item) => this.Item = Item; 
    }
    #endregion
    #region class : ToolItemClickedEventArgs
    public class ToolItemClickedEventArgs : EventArgs
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public ToolItem Item { get; private set; }

        public ToolItemClickedEventArgs(float X, float Y, ToolItem Item)
        {
            this.X = X;
            this.Y = Y;
            this.Item = Item;
        }
    }
    #endregion
    #region class : ToolItemDragEventArgs
    public class ToolItemDragEventArgs : EventArgs
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public ToolItem Item { get; private set; }
        public bool Drag { get; set; }

        public ToolItemDragEventArgs(float X, float Y, ToolItem Item)
        {
            this.X = X;
            this.Y = Y;
            this.Item = Item;
        }
    }
    #endregion
    #region class : TreeViewNodeClickedEventArgs
    public class TreeViewNodeClickedEventArgs : EventArgs
    {
        public float X { get; private set; }
        public float Y { get; private set; }
        public TreeViewNode Node { get; private set; }

        public TreeViewNodeClickedEventArgs(float X, float Y, TreeViewNode Node)
        {
            this.X = X;
            this.Y = Y;
            this.Node = Node;
        }
    }
    #endregion
    #region class : FoldingMenuClickedEventArgs
    public class FoldingMenuClickedEventArgs
    {
        public DvTextIcon Item { get; private set; }

        public FoldingMenuClickedEventArgs(DvTextIcon item)
        {
            this.Item = item;
        }
    }
    #endregion
    #endregion

    #region TextIcon
    #region class : DvTextIcon
    public class DvTextIcon
    {
        #region Static
        public static DvTextIcon Empty = new DvTextIcon();
        #endregion

        #region Member Variable
        private DvIcon ico = new DvIcon();
        #endregion

        #region Properteis
        public DvIcon Icon => ico;
        public SKBitmap IconImage { get => ico.IconImage; set => ico.IconImage = value; }
        public string IconString { get => ico.IconString; set => ico.IconString = value; }
        public float IconSize { get => ico.IconSize; set => ico.IconSize = value; }
        public DvTextIconAlignment IconAlignment { get => ico.IconAlignment; set => ico.IconAlignment = value; }
        public float IconGap { get; set; } = 5;

        public string Text { get; set; }
        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;
        public DvFontStyle FontStyle { get; set; } = DvFontStyle.Normal;

        public Padding TextPadding { get; set; } = new Padding(0);
        
        public object Tag { get; set; }
        public object Value { get; set; }
        #endregion
    }
    #endregion
    #region class : DvIcon
    public class DvIcon
    {
        public SKBitmap IconImage { get; set; } = null;
        public string IconString { get; set; } = null;
        public float IconSize { get; set; } = 12;
        public DvTextIconAlignment IconAlignment { get; set; } = DvTextIconAlignment.LeftRight;

        public DvIcon() { }
        public DvIcon(SKBitmap ico) => this.IconImage = ico;
        public DvIcon(string iconString) => this.IconString = iconString;
        public DvIcon(string iconString, float Size) : this(iconString) => this.IconSize = Size;
        public DvIcon(string iconString, float Size, DvTextIconAlignment align) : this(iconString, Size) => this.IconAlignment = align;
    }
    #endregion
    #endregion

    #region Graph
    #region class : MeterBar
    public class MeterBar
    {
        public double Minimum { get; private set; }
        public double Maximum { get; private set; }
        public SKColor Color { get; private set; }

        public MeterBar(double Minimum, double Maximum, SKColor Color)
        {
            this.Minimum = Minimum;
            this.Maximum = Maximum;
            this.Color = Color;
        }
    }
    #endregion
    #region class : GraphSeries
    public class GraphSeries
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public SKColor SeriesColor { get; set; }
    }
    #endregion
    #region class : GraphSeries2
    public class GraphSeries2 : GraphSeries
    {
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public bool Visible { get; set; } = true;
    }
    #endregion

    #region abstract class : GraphData
    public abstract class GraphData
    {
        public abstract string Name { get; set; }
        public SKColor Color { get; set; }
    }
    #endregion
    #region abstract class : TimeGraphData
    public abstract class TimeGraphData
    {
        public abstract DateTime Time { get; set; }
    }
    #endregion

    #region class : GraphValue
    public class GraphValue
    {
        public string Name { get; set; }
        public SKColor Color { get; set; }
        public Dictionary<string, double> Values { get; } = new Dictionary<string, double>();
    }
    #endregion
    #region class : TimeGraphValue
    public class TimeGraphValue
    {
        public DateTime Time { get; set; }
        public Dictionary<string, double> Values { get; } = new Dictionary<string, double>();
    }
    #endregion
    #region class : LGV
    class LGV
    {
        public SKPoint Position { get; set; }
        public double Value { get; set; }
    }
    #endregion
    #region class : CGV
    class CGV
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public SKColor Color { get; set; }
    }
    #endregion
    #endregion

    #region Info
    #region class : ButtonInfo
    public class ButtonInfo : DvTextIcon
    {
        #region Properties
        #region Name
        public string Name { get; set; }
        #endregion
        #region Size
        public SizeInfo Size { get; set; } = new SizeInfo(DvSizeMode.Percent, 100);
        #endregion
        #region Checked
        public bool Checked { get; set; }
        #endregion
        #region ButtonDownState
        public bool ButtonDownState { get; set; }
        #endregion
        #endregion

        public ButtonInfo(string Name)
        {
            this.Name = Name;
        }
    }
    #endregion
    #region class : SizeInfo
    public class SizeInfo
    {
        public DvSizeMode Mode { get; set; }
        public float Size { get; set; }

        public SizeInfo() { }
        public SizeInfo(DvSizeMode mode, float size) { this.Mode = mode; this.Size = size; }
    }
    #endregion
    #endregion
    #endregion

    #region enums
    #region enum : DvContentAlignment
    public enum DvContentAlignment
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, MiddleCenter, MiddleRight,
        BottomLeft, BottomCenter, BottomRight
    }
    #endregion
    #region enum : DvTextIconAlignment
    public enum DvTextIconAlignment
    {
        LeftRight, TopBottom
    }
    #endregion
    #region enum : DvPosition
    public enum DvPosition { Left, Top, Right, Bottom }
    #endregion
    #region enum : DvDirection
    public enum DvDirection { Left, Right, Up, Down }
    public enum DvDirectionHV { Horizon, Vertical }
    #endregion
    #region enum : DvSizeMode
    public enum DvSizeMode { Pixel, Percent }
    #endregion
    #region enum : DvFontStyle
    public enum DvFontStyle
    {
        Thin, ThinItalic, ThinOblique,
        ExtraLight, ExtraLightItalic, ExtraLightOblique,
        Light, LightItalic, LightOblique,
        Normal, Italic, Oblique,
        Medium, MediumItalic, MediumOblique,
        Demi, DemiItalic, DemiOblique,
        Bold, BoldItalic, BoldOblique,
        ExtraBold, ExtraBoldItalic, ExtraBoldOblique,
        Black, BlackItalic, BlackOblique,
        ExtraBlack, ExtraBlackItalic, ExtraBlackOblique,

        Unknown
    }
    #endregion
    #region enum : DvRoundType
    public enum DvRoundType
    {
        Rect,
        All,
        L, R, T, B,
        LT, RT, LB, RB,
        Ellipse,
    }
    #endregion
    
    #region enum : ItemSelectionMode 
    public enum ItemSelectionMode { NONE, SINGLE, MULTI }
    #endregion
    #region enum : BarGraphMode 
    public enum BarGraphMode { STACK, LIST }
    #endregion
    #region enum : PictureScaleMode
    public enum PictureScaleMode { Real, CenterImage, Strech, Zoom }
    #endregion
    #region enum : DateTimePickerMode
    public enum DateTimePickerMode { Date, Time, DateTime }
    #endregion
    #endregion
}

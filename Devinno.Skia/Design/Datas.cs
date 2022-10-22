using Devinno.Skia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Design
{
    #region struct : Padding
    public struct Padding
    {
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

    #region class : MouseEventArgs
    public class MouseEventArgs: EventArgs
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public MouseEventArgs(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
    #endregion
    #region class : ButtonsClickventArgs
    public class ButtonsClickventArgs : EventArgs
    {
        public ButtonInfo Button { get; private set; }

        public ButtonsClickventArgs(ButtonInfo Button)
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

    #region enum : DvDirection
    public enum DvPosition { Left, Top, Right, Bottom }
    public enum DvDirection { Left, Right, Up, Down}
    public enum DvDirectionHV { Horizon, Vertical }
    #endregion
 
}

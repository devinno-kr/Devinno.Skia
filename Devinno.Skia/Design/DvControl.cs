using Devinno.Skia.Design;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Design
{
    public class DvControl
    {
        #region Properties
        public string Name { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; } = 70;
        public int Height { get; set; } = 30;

        public int ScreenX => ParentContainer != null && ParentContainer is DvControl ? ((DvControl)ParentContainer).ScreenX + X : X;
        public int ScreenY => ParentContainer != null && ParentContainer is DvControl ? ((DvControl)ParentContainer).ScreenY + Y : Y;

        public int Left => X;
        public int Top => Y;
        public int Right => X + Width;
        public int Bottom => Y + Height;

        public SKRect Bounds => Util.FromRect(X, Y, Width, Height);

        public Point Location { get => new Point(X, Y); set { X = value.X; Y = value.Y; } }
        public Size Size { get => new Size(Width, Height); set { Width = value.Width; Height = value.Height; } }

        public bool Visible { get; set; } = true;
        public bool Enabled { get; set; } = true;

        public bool Fill { get; set; } = false;
        public Padding Margin { get; set; } = new Padding(3);

        [JsonIgnore]
        public object Tag { get; set; }
        [JsonIgnore]
        public IDvContainer ParentContainer { get; set; }

        [JsonIgnore]
        public DvDesign Design => ParentContainer != null ? ParentContainer.Design : null;

        public bool BufferDraw { get; set; } = true;
        #endregion

        #region Member Variable
        internal bool _bMouseDown_ = false;
        bool bDown = false;
        #endregion

        #region Event
        public event EventHandler<MouseEventArgs> MouseClick;
        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseDoubleClick;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<SKCanvas> Drawn;

        public event EventHandler<DragDropEventArgs> DragDrop;
        #endregion

        #region Constructor
        public DvControl()
        {
        }
        #endregion

        #region Method
        #region Virtual
        protected virtual void OnDraw(SKCanvas Canvas) { }

        protected virtual void OnMouseDown(int x, int y) { }
        protected virtual void OnMouseUp(int x, int y) { }
        protected virtual void OnMouseMove(int x, int y) { }
        protected virtual void OnMouseClick(int x, int y) { }
        protected virtual void OnMouseDoubleClick(int x, int y) { }
        protected virtual void OnDragDrop(object dragItem) { }

        public virtual void OnUnload() { }
        #endregion

        #region Internal
        internal virtual void _Draw(SKCanvas Canvas)
        {
            if (BufferDraw)
            {
                using (var bm = new SKBitmap(Width + 3, Height + 3, SKColorType.Rgba8888, SKAlphaType.Premul))
                {
                    using (var cv = new SKCanvas(bm))
                    {
                        OnDraw(cv);
                        Drawn?.Invoke(this, cv);
                    }

                    using (var p = new SKPaint())
                    {
                        if (!Enabled) p.Color = SKColors.White.WithAlpha(90);
                        Canvas.DrawBitmap(bm, Util.FromRect(0, 0, Width + 3, Height + 3), p);
                    }
                }
            }
            else
            {
                if (Enabled)
                {
                    OnDraw(Canvas);
                    Drawn?.Invoke(this, Canvas);
                }
                else
                {
                    using (var bm = new SKBitmap(Width + 3, Height + 3, SKColorType.Rgba8888, SKAlphaType.Premul))
                    {
                        using (var cv = new SKCanvas(bm))
                        {
                            OnDraw(cv);
                            Drawn?.Invoke(this, cv);
                        }

                        using (var p = new SKPaint())
                        {
                            p.Color = SKColors.White.WithAlpha(90);
                            Canvas.DrawBitmap(bm, Util.FromRect(0, 0, Width + 3, Height + 3), p);
                        }
                    }
                }
            }
        }

        internal virtual void _Unload()
        {
            OnUnload();
        }

        internal virtual void _MouseDown(int x, int y)
        {
            if (CollisionTool.Check(Util.FromRect(0, 0, Width, Height), x, y))
            {
                bDown = true;
                MouseDown?.Invoke(this, new MouseEventArgs(x, y));
                OnMouseDown(x, y);
            }
        }

        internal virtual void _MouseUp(int x, int y)
        {
            if (CollisionTool.Check(Util.FromRect(0, 0, Width, Height), x, y) || bDown)
            {
                if(Design != null && Design.IsDrag)
                {
                    var itm = Design.GetDragItem();
                    DragDrop?.Invoke(this, new DragDropEventArgs(itm));
                    OnDragDrop(itm);
                }

                MouseUp?.Invoke(this, new MouseEventArgs(x, y));

                if (bDown)
                {
                    bDown = false;

                    if (CollisionTool.Check(Util.FromRect(0, 0, Width, Height), x, y))
                    {
                        MouseClick?.Invoke(this, new MouseEventArgs(x, y));
                        OnMouseClick(x, y);
                    }
                }

                OnMouseUp(x, y);
            }
        }

        internal virtual void _MouseMove(int x, int y)
        {
            if (CollisionTool.Check(Util.FromRect(0, 0, Width, Height), x, y) || bDown)
            {
                MouseMove?.Invoke(this, new MouseEventArgs(x, y));
                OnMouseMove(x, y);
            }
        }

        internal virtual void _MouseDoubleClick(int x, int y)
        {
            if (CollisionTool.Check(Util.FromRect(0, 0, Width, Height), x, y))
            {
                MouseDoubleClick?.Invoke(this, new MouseEventArgs(x, y));
                OnMouseDoubleClick(x, y);
            }
        }
        #endregion
        #endregion
    }

}

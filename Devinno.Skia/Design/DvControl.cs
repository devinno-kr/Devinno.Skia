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
        #region Const
        const int LONG_CLICK_TIME = 200;
        #endregion

        #region Properties
        #region Name
        public string Name { get; set; }
        #endregion

        #region Position
        private SKRect bounds = Util.FromRect(0, 0, 70, 30);

        public SKRect Bounds { get => bounds; set => bounds = value; }
        public SKRect ScreenBounds
        {
            get
            {
                if (ParentContainer != null && ParentContainer is DvControl)
                {
                    var x = ((DvControl)ParentContainer).ScreenX + X;
                    var y = ((DvControl)ParentContainer).ScreenY + Y;
                    return Util.FromRect(x, y, Width, Height);
                }
                else return bounds;
            }
        }

        public float Left { get => bounds.Left; set => bounds.Left = value; }
        public float Top { get => bounds.Top; set => bounds.Top = value; }
        public float Right { get => bounds.Right; set => bounds.Right = value; }
        public float Bottom { get => bounds.Bottom; set => bounds.Bottom = value; }

        public float X { get => bounds.Left; set => bounds.Left = value; }
        public float Y { get => bounds.Top; set => bounds.Top = value; }
        public float Width { get => bounds.Right - bounds.Left; set => bounds.Right = value + bounds.Left; }
        public float Height { get => bounds.Bottom - bounds.Top; set => bounds.Bottom = value + bounds.Top; }

        public float ScreenX => ParentContainer != null && ParentContainer is DvControl ? ((DvControl)ParentContainer).ScreenX + X : X;
        public float ScreenY => ParentContainer != null && ParentContainer is DvControl ? ((DvControl)ParentContainer).ScreenY + Y : Y;

        public SKPoint Location { get => new SKPoint(X, Y); set { X = value.X; Y = value.Y; } }
        public SKSize Size { get => new SKSize(Width, Height); set { Width = value.Width; Height = value.Height; } }
        #endregion

        #region Visible / Enabled
        public bool Visible { get; set; } = true;
        public bool Enabled { get; set; } = true;
        #endregion

        #region Fill
        public bool Fill { get; set; } = false;
        #endregion
        #region Margin
        public Padding Margin { get; set; } = new Padding(3);
        #endregion
        #region Tag
        [JsonIgnore]
        public object Tag { get; set; }
        #endregion
        #region ParentContainer
        [JsonIgnore]
        public IDvContainer ParentContainer { get; set; }
        #endregion
        #region Design
        [JsonIgnore]
        public DvDesign Design => ParentContainer != null ? ParentContainer.Design : null;
        #endregion
        #endregion

        #region Member Variable
        internal bool _bMouseDown_ = false;
        private bool bDown = false;
        private DateTime downTime;
        float dx, dy;
        #endregion

        #region Event
        public event EventHandler<MouseEventArgs> MouseClick;
        public event EventHandler<MouseEventArgs> MouseLongClick;
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

        protected virtual void OnMouseDown(float x, float y) { }
        protected virtual void OnMouseUp(float x, float y) { }
        protected virtual void OnMouseMove(float x, float y) { }
        protected virtual void OnMouseClick(float x, float y) { }
        protected virtual void OnMouseDoubleClick(float x, float y) { }
        protected virtual void OnMouseLongClick(float x, float y) { }
        protected virtual void OnDragDrop(object dragItem) { }

        public virtual void OnUnload() { }
        #endregion

        #region Internal
        #region _Draw
        internal virtual void _Draw(SKCanvas Canvas)
        {
            if (Enabled)
            {
                OnDraw(Canvas);
                Drawn?.Invoke(this, Canvas);
            }
            else
            {
                using (var bm = new SKBitmap(Convert.ToInt32(Width + 3), Convert.ToInt32(Height + 3), SKColorType.Rgba8888, SKAlphaType.Premul))
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
        #endregion

        #region _MouseDown
        internal virtual void _MouseDown(float x, float y)
        {
            var rt = Util.FromRect(0, 0, Width, Height);
            if (CollisionTool.Check(rt, x, y))
            {
                dx = x;
                dy = y;
                bDown = true;
                downTime = DateTime.Now;
                MouseDown?.Invoke(this, new MouseEventArgs(x, y));
                OnMouseDown(x, y);

                ThreadPool.QueueUserWorkItem((o) =>
                {
                    while (bDown && (DateTime.Now - downTime).TotalMilliseconds < LONG_CLICK_TIME) Thread.Sleep(50);

                    if((DateTime.Now - downTime).TotalMilliseconds >= LONG_CLICK_TIME)
                    {
                        if (CollisionTool.Check(rt, x, y))
                        {
                            MouseLongClick?.Invoke(this, new MouseEventArgs(x, y));
                            OnMouseLongClick(x, y);
                        }
                    }

                });
            }
        }
        #endregion
        #region _MouseUp
        internal virtual void _MouseUp(float x, float y)
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
                OnMouseUp(x, y);
             
                if (bDown)
                {
                    bDown = false;

                    var dist = Math.Abs(MathTool.GetDistance(new SKPoint(dx, dy), new SKPoint(x, y)));
                    if (CollisionTool.Check(Util.FromRect(0, 0, Width, Height), x, y) && dist < 3)
                    {
                        MouseClick?.Invoke(this, new MouseEventArgs(x, y));
                        OnMouseClick(x, y);
                    }
                }
            }
        }
        #endregion
        #region _MouseMove
        internal virtual void _MouseMove(float x, float y)
        {
            if (CollisionTool.Check(Util.FromRect(0, 0, Width, Height), x, y) || bDown)
            {
                MouseMove?.Invoke(this, new MouseEventArgs(x, y));
                OnMouseMove(x, y);
            }
        }
        #endregion
        #region _MouseDoubleClick
        internal virtual void _MouseDoubleClick(float x, float y)
        {
            if (CollisionTool.Check(Util.FromRect(0, 0, Width, Height), x, y))
            {
                MouseDoubleClick?.Invoke(this, new MouseEventArgs(x, y));
                OnMouseDoubleClick(x, y);
            }
        }
        #endregion

        #region _Unload
        internal virtual void _Unload()
        {
            OnUnload();
        }
        #endregion
        #endregion
        #endregion
    }

}

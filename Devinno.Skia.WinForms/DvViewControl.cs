using Devinno.Skia.Design;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.WinForms
{
    public class DvViewControl : SKGLControl
    {
        #region Properties
        public DvDesign Design { get; private set; }

        public int Interval { get; set; } = 10;
        #endregion

        #region Member Variable
        private System.Threading.Thread th;
        private bool bLoaded = false;
        private bool bFirstDraw = false;
        private DateTime dcTime;
        #endregion

        #region Constructor
        public DvViewControl(int width, int height)
        {
            Design = new DvDesign() { Width = width, Height = height };

            th = new System.Threading.Thread(() =>
            {
                while (true)
                {
                    if (!this.IsDisposed && bLoaded && bFirstDraw)
                    {
                        Design?.Update();
                        this.Invoke(new Action(() => Invalidate()));
                    }
                    System.Threading.Thread.Sleep(Interval);
                }
            })
            { IsBackground = true };
            th.Start();
        }
        #endregion

        #region Override
        #region OnLoad
        protected override void OnLoad(EventArgs e)
        {
            bLoaded = true;
            base.OnLoad(e);
        }
        #endregion
        
        #region OnPaintSurface
        protected override void OnPaintSurface(SKPaintGLSurfaceEventArgs e)
        {
            bFirstDraw = true;
            Design?.Draw(e.Surface.Canvas);
            base.OnPaintSurface(e);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            Design?.MouseDown(e.X, e.Y);
            base.OnMouseDown(e);
        }
        #endregion
        #region OnMouseUp
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            Design?.MouseUp(e.X, e.Y);

            if ((DateTime.Now - dcTime).TotalMilliseconds < 300)
            {
                Design?.MouseDoubleClick(e.X, e.Y);
            }

            dcTime = DateTime.Now;
            base.OnMouseUp(e);
        }
        #endregion
        #region OnMouseDown
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            Design?.MouseMove(e.X, e.Y);
            base.OnMouseMove(e);
        }
        #endregion

        #region OnHandleDestroyed
        protected override void OnHandleDestroyed(EventArgs e)
        {
            Design?.Unload();
            base.OnHandleDestroyed(e);
        }
        #endregion
        #endregion

    }
}

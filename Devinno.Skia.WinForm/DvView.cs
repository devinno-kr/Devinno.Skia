using Devinno.Skia.Design;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Devinno.Skia.WinForm
{
    public class DvView : SKControl
    {
		#region Properties
		public DvDesign Design { get; private set; }
		public int RefreshInterval
		{
			get => tmr.Interval;
			set
			{
				tmr.Interval = value;
			}
		}

		public bool AutoRefresh { get => tmr.Enabled; set => tmr.Enabled = value; }
        #endregion

        #region Event
        public event EventHandler<DrawnEventArgs> Drawn;
		#endregion

		#region Member Variable
		Timer tmr = new Timer();
		#endregion

		#region Constructor
		public DvView()
		{
			Design = new DvDesign();
			
			if (!DesignMode)
			{
				tmr.Interval = 10;
				tmr.Tick += (o, s) => Invalidate();
			}
		}
		#endregion

		#region Override
		#region OnPaint
		protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
		{
			OnDrawn(e.Surface.Canvas, Width, Height);
			base.OnPaintSurface(e);
		}
        #endregion
        #region OnClientSizeChanged
        protected override void OnClientSizeChanged(EventArgs e)
		{
			base.OnClientSizeChanged(e);
			Invalidate();
		}
		#endregion

		#region OnDrawn
		protected virtual void OnDrawn(SKCanvas canvas, int width, int height)
		{
			if (Design != null)
			{
				Design.Width = width;
				Design.Height = height;
				Design.Draw(canvas);
			}

			Drawn?.Invoke(this, new DrawnEventArgs(canvas));
		}
		#endregion
		#region OnMouseUp
		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (Design != null) Design.MouseUp(e.X, e.Y);
		
			Invalidate();
			base.OnMouseUp(e);
		}
        #endregion
        #region OnMouseDown
        protected override void OnMouseDown(MouseEventArgs e)
        {
			Design?.MouseDown(e.X, e.Y);
			Invalidate();
            base.OnMouseDown(e);
        }
        #endregion
        #region OnMouseMove
        protected override void OnMouseMove(MouseEventArgs e)
        {
			Design?.MouseMove(e.X, e.Y);
			Invalidate();
            base.OnMouseMove(e);
        }
        #endregion
        #region OnMouseDoubleClick
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
			Design?.MouseDoubleClick(e.X, e.Y);
			Invalidate();
			base.OnMouseDoubleClick(e);
        }
        #endregion
        #endregion
    }

    #region class : DrawnEventArgs 
    public class DrawnEventArgs : EventArgs
	{
		public SKCanvas Canvas { get; private set; }
	
		public DrawnEventArgs(SKCanvas Canvas)
		{
			this.Canvas = Canvas;
		}
	}
    #endregion
}

using Devinno.Skia.Collections;
using Devinno.Skia.Design;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Design
{
    public class DvContainer : DvControl, IDvContainer
    {
        #region Properties
        private DvControlCollection lsControls;
        public virtual DvControlCollection Controls { get => lsControls; }
        #endregion

        #region Constructor
        public DvContainer()
        {
            lsControls = new DvControlCollection(this);
            lsControls.Changed += (o, s) => OnLayout();
        }
        #endregion

        #region Method
        #region GetBackColor
        public virtual SKColor GetBackColor() => (ParentContainer != null ? ParentContainer.GetBackColor() : Design.Theme.BackColor);
        #endregion

        #region Internal
        protected override void OnDraw(SKCanvas Canvas)
        {
            #region Controls
            if (Controls != null)
            {
                OnLayout();

                foreach (var v in Controls.Values)
                {
                    if (v.Visible)
                    {
                        var sp = Canvas.Save();
                        Canvas.Translate(v.X, v.Y);
                        Canvas.ClipRect(Util.FromRect(0, 0, v.Width + 3, v.Height + 3));

                        v._Draw(Canvas);
                        Canvas.RestoreToCount(sp);
                    }
                }
            }
            #endregion

            base.OnDraw(Canvas);
        }

        protected override void OnMouseDown(int x, int y)
        {
            #region Controls
            bool bcl = false;
            if (Controls != null)
            {
                foreach (var v in Controls.Values)
                {
                    var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y);
                    if (bCheck)
                    {
                        v._bMouseDown_ = true;
                        if (v.Visible && v.Enabled && this.Enabled) v._MouseDown(x - v.X, y - v.Y);
                        bcl = bCheck;
                    }
                }
            }
            #endregion

            if (!bcl) base.OnMouseDown(x, y);
        }

        protected override void OnMouseUp(int x, int y)
        {
            #region Controls
            bool bcl = false;
            if (Controls != null)
            {
                foreach (var v in Controls.Values)
                {
                    var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y);
                    if (bCheck || v._bMouseDown_)
                    {
                        if (v.Visible && v.Enabled && this.Enabled) v._MouseUp(x - v.X, y - v.Y);
                        bcl = bCheck;
                    }
                    v._bMouseDown_ = false;
                }
            }
            #endregion

            if (!bcl) base.OnMouseUp(x, y);
        }

        protected override void OnMouseMove(int x, int y)
        {
            #region Controls
            bool bcl = false;
            if (Controls != null)
            {
                foreach (var v in Controls.Values)
                {
                    var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y);
                    if (bCheck || v._bMouseDown_)
                    {
                        if (v.Visible && v.Enabled && this.Enabled) v._MouseMove(x - v.X, y - v.Y);
                        bcl = bCheck;
                    }
                }
            }
            #endregion

            if (!bcl) base.OnMouseMove(x, y);
        }

        protected override void OnMouseDoubleClick(int x, int y)
        {
            #region Controls
            bool bcl = false;
            if (Controls != null)
            {
                foreach (var v in Controls.Values)
                {
                    var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y);
                    if (bCheck)
                    {
                        if (v.Visible && v.Enabled && this.Enabled) v._MouseDoubleClick(x - v.X, y - v.Y);
                        bcl = bCheck;
                    }
                }
            }
            #endregion

            if (!bcl) base.OnMouseDown(x, y);
        }

        protected virtual void OnLayout()
        {
            foreach (var v in Controls.Values)
            {
                if (v.Fill)
                {
                    v.X = v.Margin.Left;
                    v.Y = v.Margin.Top;
                    v.Width = Width - (v.Margin.Left + v.Margin.Right);
                    v.Height = Height - (v.Margin.Top + v.Margin.Bottom);
                }
            }
        }

        public override void OnUnload()
        {
            foreach (var c in Controls.Values) c._Unload();
            base.OnUnload();
        }
        #endregion
        #endregion
    }
}

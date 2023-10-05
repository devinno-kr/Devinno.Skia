using Devinno.Skia.Collections;
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
using System.Threading.Tasks;

namespace Devinno.Skia.Design
{
    public class DvPage : IDvContainer
    {
        #region Properties
        public string Name { get; set; }

        public bool UseMasterPage { get; set; } = true;
        public bool BackgroundDraw { get; set; } = false;

        public SKColor? BackColor { get; set; } = null;
        public SKBitmap BackgroundImage { get; set; } = null;
        public AnimationType? AnimationType { get; set; } = null;
        public DvControlCollection Controls { get; private set; }

        [JsonIgnore]
        public DvDesign Design { get; internal set; }

        public string Text { get; set; }
        public string IconString { get; set; }
        #endregion

        #region Member Variable
        #endregion

        #region Event
        public event EventHandler Update;
        #endregion

        #region Constructor
        public DvPage()
        {
            Controls = new DvControlCollection(this);
            Controls.Changed += (o, s) => OnLayout();
        }
        #endregion

        #region Method
        #region Virtual
        protected virtual void OnDraw(SKCanvas Canvas) {  }
        protected virtual void OnUpdate() { }

        protected virtual void OnMouseDown(int x, int y) { }
        protected virtual void OnMouseUp(int x, int y) { }
        protected virtual void OnMouseMove(int x, int y) { }
        protected virtual void OnMouseDoubleClick(int x, int y) { }

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }

        public virtual void OnUnload() { }

        protected virtual void OnLayout()
        {
            if (Design != null)
            {
                foreach (var v in Controls.Values)
                {
                    if (v.Fill)
                    {
                        v.X = v.Margin.Left;
                        v.Y = v.Margin.Top;
                        v.Width = Design.Width - (v.Margin.Left + v.Margin.Right);
                        v.Height = Design.Height - (v.Margin.Top + v.Margin.Bottom);
                    }
                }
            }
        }
        #endregion

        #region Internal
        internal void _Draw(SKCanvas Canvas)
        {
            var thm = Design.Theme;

            if (Design != null && thm != null)
            {
                var BackColor = this.BackColor ?? thm.BackColor;
                var sp = Canvas.Save();

                OnLayout();
                
                #region Background
                if (BackgroundDraw && !UseMasterPage)
                {
                    if (BackgroundImage != null)
                        Canvas.DrawBitmap(BackgroundImage, Util.FromRect(0, 0, Design.Width, Design.Height));
                    else
                        Canvas.Clear(BackColor);
                }
                #endregion
                #region Controls
                foreach (var v in Controls.Values)
                {
                    if (v.Visible)
                    {
                        var sp2 = Canvas.Save();
                        Canvas.Translate(v.X, v.Y);
                        Canvas.ClipRect(Util.FromRect(0, 0, v.Width + 3, v.Height + 3));

                        v._Draw(Canvas);

                        Canvas.RestoreToCount(sp2);
                    }
                }
                #endregion
                
                Canvas.RestoreToCount(sp);

                OnDraw(Canvas);
            }
        }

        internal void _Update()
        {
            OnUpdate();
            Update?.Invoke(this, null);
        }

        internal void _MouseDown(int x, int y)
        {
            if (Design != null)
            {
                #region Controls
                bool bcl = false;
                foreach (var v in Controls.Values)
                {
                    var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y);
                    if (bCheck)
                    {
                        v._bMouseDown_ = true;
                        if (v.Visible && v.Enabled) v._MouseDown(x - v.X, y - v.Y);
                        bcl = bCheck;
                    }
                }
                #endregion

                if (!bcl) OnMouseDown(x, y);
            }
        }

        internal void _MouseUp(int x, int y)
        {
            if (Design != null)
            {
                #region Controls
                bool bcl = false;
                foreach (var v in Controls.Values)
                {
                    var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y);
                    if (bCheck || v._bMouseDown_)
                    {
                        if (v.Visible && v.Enabled) v._MouseUp(x - v.X, y - v.Y);
                        bcl = bCheck;
                    }
                    v._bMouseDown_ = false;
                }
                #endregion

                if (!bcl) OnMouseUp(x, y);
            }
        }

        internal void _MouseMove(int x, int y)
        {
            if (Design != null)
            {
                #region Controls
                bool bcl = false;
                foreach (var v in Controls.Values)
                {
                    var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y);
                    if (bCheck || v._bMouseDown_)
                    {
                        if (v.Visible && v.Enabled) v._MouseMove(x - v.X, y - v.Y);
                        bcl = bCheck;
                    }
                }
                #endregion

                if (!bcl) OnMouseMove(x, y);
            }
        }

        internal void _MouseDoubleClick(int x, int y)
        {
            if (Design != null)
            {
                #region Controls
                bool bcl = false;
                foreach (var v in Controls.Values)
                {
                    var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y);
                    if (bCheck)
                    {
                        if (v.Visible && v.Enabled) v._MouseDoubleClick(x - v.X, y - v.Y);
                        bcl = bCheck;
                    }
                }
                #endregion

                if (!bcl) OnMouseMove(x, y);
            }
        }

        internal void _Show()
        {
            if (Design != null)
            {
                _Update();
                OnShow();
            }
        }

        internal void _Hide()
        {
            if (Design != null)
            {
                OnHide();
            }
        }

        internal void _Unload()
        {
            foreach (var c in Controls.Values) c._Unload();

            OnUnload();
        }
        #endregion

        #region GetBackColor
        public SKColor GetBackColor() => BackColor ?? Design.Theme.BackColor;
        #endregion
        #endregion
    }
}

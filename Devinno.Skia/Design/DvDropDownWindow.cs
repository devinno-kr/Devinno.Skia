using Devinno.Skia.Collections;
using Devinno.Skia.Design;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Design
{
    public class DvDropDownWindow : IDvContainer
    {
        #region Properties
        public string Name { get; set; }

        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BackColor { get; set; } = null;

        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;

        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Width { get; set; } = 400;
        public int Height { get; set; } = 300;

        public DvControlCollection Controls { get; private set; }

        public bool Animation => Design != null ? Design.Animation : false;
        internal bool AnimationPlaying => (Animation && ani.IsPlaying);

        [JsonIgnore]
        public DvDesign Design => DvDesign.ActiveDesign;
        #endregion

        #region Member Variable
        internal bool _bMouseDown_ = false;

        private Animation ani = new Animation();
        private SKRect sbounds, ebounds;
        #endregion

        #region Constructor
        public DvDropDownWindow()
        {
            this.Controls = new DvControlCollection(this);
        }
        #endregion

        #region Method
        #region GetBackColor
        public SKColor GetBackColor() => BackColor ?? Design.Theme.BackColor;
        #endregion

        #region Virtual
        protected virtual void OnDraw(SKCanvas Canvas) { }

        protected virtual void OnMouseDown(int x, int y) { }
        protected virtual void OnMouseUp(int x, int y) { }
        protected virtual void OnMouseMove(int x, int y) { }
        protected virtual void OnMouseDoubleClick(int x, int y) { }

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
                        v.Width = Width - (v.Margin.Left + v.Margin.Right);
                        v.Height = Height - (v.Margin.Top + v.Margin.Bottom);
                    }
                }
            }
        }

        protected virtual void OnExitButtonClick() { }
        public void Unload()
        {
            foreach (var c in Controls.Values) c._Unload();
        }
        #endregion

        #region Internal
        internal void _Draw(SKCanvas Canvas)
        {
            if (Design != null && Design.Theme != null)
            {
                var thm = Design.Theme;

                if (Animation)
                {
                    if (ani.IsPlaying)
                    {
                        using (var p = new SKPaint())
                        {
                            if (ani.Variable == "Show")
                            {
                                var rt = ani.Value(AnimationAccel.DCL, sbounds, ebounds);
                                this.X = Convert.ToInt32(rt.Left);
                                this.Y = Convert.ToInt32(rt.Top);
                                this.Width = Convert.ToInt32(rt.Width);
                                this.Height = Convert.ToInt32(rt.Height);
                            }
                            else if (ani.Variable == "Hide")
                            {
                                var rt = ani.Value(AnimationAccel.DCL, ebounds, sbounds);
                                this.X = Convert.ToInt32(rt.Left);
                                this.Y = Convert.ToInt32(rt.Top);
                                this.Width = Convert.ToInt32(rt.Width);
                                this.Height = Convert.ToInt32(rt.Height);
                            }
                        }
                    }
                    else
                    {
                        if (ani.Variable == "Show")
                        {
                            this.X = Convert.ToInt32(ebounds.Left);
                            this.Y = Convert.ToInt32(ebounds.Top);
                            this.Width = Convert.ToInt32(ebounds.Width);
                            this.Height = Convert.ToInt32(ebounds.Height);
                        }
                        else if (ani.Variable == "Hide")
                        {
                            this.X = Convert.ToInt32(sbounds.Left);
                            this.Y = Convert.ToInt32(sbounds.Top);
                            this.Width = Convert.ToInt32(sbounds.Width);
                            this.Height = Convert.ToInt32(sbounds.Height);
                        }
                    }
                }

                OnLayout();

                OnDraw(Canvas);

                #region Controls
                {
                    var sp = Canvas.Save();

                    Canvas.Translate(X, Y);
                    Canvas.ClipRect(Util.FromRect(0, 0, Width, Height));
                  
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
                }
                #endregion
            }
        }

        internal void _MouseDown(int x, int y)
        {
            if (Design != null && !AnimationPlaying)
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
            if (Design != null && !AnimationPlaying)
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
            if (Design != null && !AnimationPlaying)
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
            if (Design != null && !AnimationPlaying)
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

                if (!bcl) OnMouseDown(x, y);
            }
        }
        #endregion

        #region Show / Hide
        public virtual void Show(SKRect sbounds, SKRect ebounds)
        {
            if (Design != null)
            {
                var rt = ebounds;
                
                Design.ShowDropDownWindow(this);

                this.sbounds = sbounds;
                this.ebounds = ebounds;

                if (Animation)
                {
                    this.X = Convert.ToInt32(sbounds.Left);
                    this.Y = Convert.ToInt32(sbounds.Top);
                    this.Width = Convert.ToInt32(sbounds.Width);
                    this.Height = Convert.ToInt32(sbounds.Height);

                    ani.Stop();
                    ani.Start(DvDesign.ANI, "Show");
                }
                else
                {
                    this.X = Convert.ToInt32(ebounds.Left);
                    this.Y = Convert.ToInt32(ebounds.Top);
                    this.Width = Convert.ToInt32(ebounds.Width);
                    this.Height = Convert.ToInt32(ebounds.Height);
                }
            }
        }

        public virtual void Hide()
        {
            if (Design != null)
            {
                if (Animation && !ani.IsPlaying)
                {
                    ani.Stop();
                    ani.Start(DvDesign.ANI, "Hide", () => Design.HideDropDownWindow(this));
                }
                else Design.HideDropDownWindow(this);
            }
        }
        #endregion

        #region bounds
        void bounds(Action<SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);

            act(rtContent);
        }
        #endregion
        #endregion
    }
}

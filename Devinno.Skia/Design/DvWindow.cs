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
    public class DvWindow : IDvContainer
    {
        #region Properties
        public string Name { get; set; }

        public SKColor? ForeColor { get; set; } = null;
        public SKColor? BackColor { get; set; } = null;
        public SKColor? TitleBarColor { get; set; } = null;
        public SKColor? IconBoxColor { get; set; } = null;
        public SKColor? WindowStateButtonColor { get; set; } = null;

        public SKBitmap BackgroundImage { get; set; } = null;
        public bool BackgroundDraw { get; set; } = true;
        public bool UseTitleBar { get; set; } = true;
        public bool ExitBox { get; set; } = true;

        public string Title { get; set; } = "Title";
        public int TitleHeight { get; set; } = 40;

        public string FontName { get; set; } = "NanumGothic";
        public float FontSize { get; set; } = 12;

        public string IconString { get; set; } = "fa-cube";
        public float IconSize { get; set; } = 18;
        public float IconBoxWidth { get; set; } = 40;

        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Width { get; set; } = 400;
        public int Height { get; set; } = 300;

        public DvControlCollection Controls { get; private set; }

        public bool Animation => Design != null ? Design.Animation : false;
        internal bool AnimationPlaying => (Animation && ani.IsPlaying);

        public bool UseBlock { get; set; } = true;

        [JsonIgnore]
        public DvDesign Design => DvDesign.ActiveDesign;
        #endregion

        #region Member Variable
        bool bExitDown = false;
        internal bool _bMouseDown_ = false;

        private Animation ani = new Animation();
        private SKPoint sp;
        #endregion

        #region Event
        public event EventHandler<ClosingEventArgs> Closing;
        #endregion

        #region Constructor
        public DvWindow()
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
                        v.Height = Height - (UseTitleBar ? TitleHeight : 0) - (v.Margin.Top + v.Margin.Bottom);
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

        #region SampleDraw
        public void SampleDraw(SKCanvas Canvas, Theme.DvTheme thm)
        {
            OnLayout();

            using (var bm = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul))
            {
                using (var cv = new SKCanvas(bm))
                {
                    var sp = cv.Save();

                    cv.ClipRect(Util.FromRect(0, 0, Width, Height));

                    #region Window
                    bounds((rtExit, rtTitle, rtTitleBar, rtContent, rtIcon) =>
                    {
                        var ForeColor = this.ForeColor ?? thm.ForeColor;
                        var BackColor = this.BackColor ?? thm.BackColor;
                        var TitleBarColor = this.TitleBarColor ?? thm.WindowTitleColor;
                        var WindowStateButtonColor = this.ForeColor ?? thm.ForeColor;
                        var IconBoxColor = this.IconBoxColor ?? thm.PointColor;

                        using (var p = new SKPaint())
                        {

                            if (BackgroundDraw)
                            {
                                if (BackgroundImage != null)
                                {
                                    cv.DrawBitmap(BackgroundImage, Util.FromRect(0, 0, Width, Height));
                                }
                                else
                                {
                                    cv.Clear(BackColor);
                                }
                            }

                            if (UseTitleBar)
                            {
                                p.Color = TitleBarColor;
                                cv.DrawRect(rtTitleBar, p);

                                p.Color = IconBoxColor;
                                cv.DrawRect(rtIcon, p);

                                p.Color = ForeColor;
                                Util.DrawText(cv, Title, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtTitle, DvContentAlignment.MiddleLeft);
                                Util.DrawIcon(cv, IconString, IconSize, ForeColor, rtIcon, DvContentAlignment.MiddleCenter);

                                if (ExitBox)
                                {
                                    var cn = 4;
                                    var rt = Util.MakeRectangle(rtExit, new SKSize(TitleHeight / cn, TitleHeight / cn));
                                    p.Color = bExitDown ? Util.FromArgb(255, 0, 0) : WindowStateButtonColor;
                                    p.StrokeWidth = 1;

                                    cv.DrawLine(rt.Left, rt.Top, rt.Right, rt.Bottom, p);
                                    cv.DrawLine(rt.Right, rt.Top, rt.Left, rt.Bottom, p);
                                }

                                OnDraw(cv);
                            }
                        }
                    });
                    #endregion

                    #region Controls
                    foreach (var v in Controls.Values)
                    {
                        if (v.Visible)
                        {
                            var sp2 = cv.Save();
                            cv.Translate(v.X, v.Y + (BackgroundDraw ? TitleHeight : 0));
                            cv.ClipRect(Util.FromRect(0, 0, v.Width + 3, v.Height + 3));

                            v._Draw(cv);

                            cv.RestoreToCount(sp2);
                        }
                    }
                    #endregion

                    cv.RestoreToCount(sp);
                }

                var rt = Util.FromRect(X, Y, Width, Height);
                var cp = MathTool.CenterPoint(rt);
                Canvas.DrawBitmap(bm, rt);
            }
        }
        #endregion

        #region Internal
        internal void _Draw(SKCanvas Canvas)
        {
            if (Design != null && Design.Theme != null)
            {
                var thm = Design.Theme;
               
                #region Block
                if (UseBlock)
                {
                    using (var paint = new SKPaint())
                    {
                        var fadeAni = 0;
                        if (ani.Variable == "Show") fadeAni = Convert.ToByte(ani.Value(AnimationAccel.Linear, 0F, 255F));
                        else if (ani.Variable == "Hide") fadeAni = Convert.ToByte(ani.Value(AnimationAccel.Linear, 255F, 0F));
                        var alpha = Animation ? Convert.ToByte(120.0 * (fadeAni / 255.0)) : (byte)120;
                        paint.Color = Util.FromArgb(alpha, 0, 0, 0);
                        paint.IsStroke = false;
                        Canvas.DrawRect(Util.FromRect(0, 0, Design.Width, Design.Height), paint);
                    }
                }
                #endregion

                OnLayout();

                using (var bm = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul))
                {
                    using (var cv = new SKCanvas(bm))
                    {
                        var sp = cv.Save();

                        cv.ClipRect(Util.FromRect(0, 0, Width, Height));

                        #region Window
                        bounds((rtExit, rtTitle, rtTitleBar, rtContent, rtIcon) =>
                        {
                            var ForeColor = this.ForeColor ?? thm.ForeColor;
                            var BackColor = this.BackColor ?? thm.BackColor;
                            var TitleBarColor = this.TitleBarColor ?? thm.WindowTitleColor;
                            var WindowStateButtonColor = this.ForeColor ?? thm.ForeColor;
                            var IconBoxColor = this.IconBoxColor ?? thm.PointColor;

                            using (var p = new SKPaint())
                            {

                                if (BackgroundDraw)
                                {
                                    if (BackgroundImage != null)
                                    {
                                        cv.DrawBitmap(BackgroundImage, Util.FromRect(0, 0, Width, Height));
                                    }
                                    else
                                    {
                                        cv.Clear(BackColor);
                                    }
                                }

                                if (UseTitleBar)
                                {
                                    p.Color = TitleBarColor;
                                    cv.DrawRect(rtTitleBar, p);

                                    p.Color = IconBoxColor;
                                    cv.DrawRect(rtIcon, p);

                                    p.Color = ForeColor;
                                    Util.DrawText(cv, Title, FontName, FontSize, DvFontStyle.Normal, ForeColor, rtTitle, DvContentAlignment.MiddleLeft);
                                    Util.DrawIcon(cv, IconString, IconSize, ForeColor, rtIcon, DvContentAlignment.MiddleCenter);

                                    if (ExitBox)
                                    {
                                        var cn = 4;
                                        var rt = Util.MakeRectangle(rtExit, new SKSize(TitleHeight / cn, TitleHeight / cn));
                                        p.Color = bExitDown ? Util.FromArgb(255, 0, 0) : WindowStateButtonColor;
                                        p.StrokeWidth = 1;

                                        cv.DrawLine(rt.Left, rt.Top, rt.Right, rt.Bottom, p);
                                        cv.DrawLine(rt.Right, rt.Top, rt.Left, rt.Bottom, p);
                                    }

                                    OnDraw(cv);
                                }
                            }
                        });
                        #endregion

                        #region Controls
                        foreach (var v in Controls.Values)
                        {
                            if (v.Visible)
                            {
                                var sp2 = cv.Save();
                                cv.Translate(v.X, v.Y + (BackgroundDraw ? TitleHeight : 0));
                                cv.ClipRect(Util.FromRect(0, 0, v.Width + 3, v.Height + 3));

                                v._Draw(cv);

                                cv.RestoreToCount(sp2);
                            }
                        }
                        #endregion

                        cv.RestoreToCount(sp);
                    }

                    var rt = Util.FromRect(X, Y, Width, Height);
                    var cp = MathTool.CenterPoint(rt);
                    if (Animation)
                    {
                        if (ani.IsPlaying)
                        {
                            using (var p = new SKPaint())
                            {
                                if (ani.Variable == "Show")
                                {
                                    p.Color = SKColors.White.WithAlpha(Convert.ToByte(ani.Value(AnimationAccel.Linear, 0F, 255F)));
                                    Canvas.DrawBitmap(bm, ani.Value(AnimationAccel.DCL, sp, rt), p);
                                }
                                else if (ani.Variable == "Hide")
                                {
                                    p.Color = SKColors.White.WithAlpha(Convert.ToByte(ani.Value(AnimationAccel.Linear, 255F, 0F)));
                                    Canvas.DrawBitmap(bm, ani.Value(AnimationAccel.ACL, rt, cp), p);
                                }
                            }
                        }
                        else if(ani.Variable == "Show") Canvas.DrawBitmap(bm, rt);
                    }
                    else Canvas.DrawBitmap(bm, rt);
                }
            }
        }

        internal void _MouseDown(int x, int y)
        {
            if (Design != null && !AnimationPlaying)
            {
                #region Window
                bounds((rtExit, rtTitle, rtTitleBar, rtContent, rtIcon) =>
                {
                    if (CollisionTool.Check(rtExit, x, y)) bExitDown = true;
                });
                #endregion

                #region Controls
                bool bcl = false;
                foreach (var v in Controls.Values)
                {
                    var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y - (BackgroundDraw ? TitleHeight : 0));
                    if (bCheck)
                    {
                        v._bMouseDown_ = true;
                        if (v.Visible && v.Enabled) v._MouseDown(x - v.X, y - (BackgroundDraw ? TitleHeight : 0) - v.Y);
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
                #region Window
                bounds((rtExit, rtTitle, rtTitleBar, rtContent, rtIcon) =>
                {
                    if (bExitDown)
                    {
                        bExitDown = false;

                        OnExitButtonClick();

                        Hide();
                    }
                });
                #endregion

                #region Controls
                bool bcl = false;
                foreach (var v in Controls.Values)
                {
                    var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y - (BackgroundDraw ? TitleHeight : 0));
                    if (bCheck || v._bMouseDown_)
                    {
                        if (v.Visible && v.Enabled) v._MouseUp(x - v.X, y - (BackgroundDraw ? TitleHeight : 0) - v.Y);
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
                    var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y - (BackgroundDraw ? TitleHeight : 0));
                    if (bCheck || v._bMouseDown_)
                    {
                        if (v.Visible && v.Enabled) v._MouseMove(x - v.X, y - (BackgroundDraw ? TitleHeight : 0) - v.Y);
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
                    var bCheck = CollisionTool.Check(Util.FromRect(v.X, v.Y, v.Width, v.Height), x, y - (BackgroundDraw ? TitleHeight : 0));
                    if (bCheck)
                    {
                        if (v.Visible && v.Enabled) v._MouseDoubleClick(x - v.X, y - (BackgroundDraw ? TitleHeight : 0) - v.Y);
                        bcl = bCheck;
                    }
                }
                #endregion

                if (!bcl) OnMouseDown(x, y);
            }
        }
        #endregion

        #region Show / Hide
        public virtual void Show()
        {
            if (Design != null)
            {
                var rt = Util.MakeRectangle(Util.FromRect(0, 0, Design.Width, Design.Height), new SKSize(Width, Height));
                this.sp = new SKPoint(Design.MouseDownX, Design.MouseDownY);

                this.X = Convert.ToInt32(rt.Left);
                this.Y = Convert.ToInt32(rt.Top);

                Design.ShowWindow(this);

                if (Animation)
                {
                    ani.Stop();
                    ani.Start(DvDesign.ANI, "Show");
                }
            }
        }

        public virtual void Hide()
        {
            bool Cancel = false;
            var args = new ClosingEventArgs();
            Closing?.Invoke(this, args);
            Cancel = args.Cancel;

            if (!Cancel)
            {
                if (Design != null)
                {
                    if (Animation)
                    {
                        ani.Stop();
                        ani.Start(DvDesign.ANI, "Hide", () => Design.HideWindow(this));
                    }
                    else Design.HideWindow(this);
                }
            }
        }
        #endregion

        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect, SKRect, SKRect> act)
        {
            int szwh = TitleHeight / 4;
            int wh = TitleHeight;
            var rtExit = Util.FromRect(Width - wh, 0, wh, wh);

            var rtTitle = Util.FromRect(TitleHeight + 15, 0, Width - (TitleHeight * 2) - 15, TitleHeight);
            var rtTitleBar = Util.FromRect(0, 0, Width, TitleHeight);
            var rtContent = Util.FromRect(0, TitleHeight, Width, Height - TitleHeight);
            var rtIcon = Util.FromRect(0, 0, IconBoxWidth, TitleHeight);

            act(rtExit, rtTitle, rtTitleBar, rtContent, rtIcon);
        }
        #endregion
        #endregion
    }

    #region class : ClosingEventArgs
    public class ClosingEventArgs
    {
        public bool Cancel { get; set; } = false;
    }
    #endregion
}

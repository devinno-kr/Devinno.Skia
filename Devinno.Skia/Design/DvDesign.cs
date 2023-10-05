using Devinno.Skia.Collections;
using Devinno.Skia.Icon;
using Devinno.Skia.Theme;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Design
{
    public class DvDesign
    {
        #region Const
        internal const int HIDE_TIME = 200;
        internal const int ANI = 150;
        internal const int ANI2 = 250;

        internal const SKFilterQuality FQ = SKFilterQuality.High;
        internal const bool AA = true;
        internal const bool DI = true;
        #endregion

        #region Static
        public static DvDesign ActiveDesign { get; private set; }
        #endregion

        #region Properties
        #region Size
        public int Width { get; set; } = 800;
        public int Height { get; set; } = 480;
        #endregion
        #region Theme
        public DvTheme Theme { get; private set; }
        #endregion
        #region Animation
        public bool Animation { get; set; } = false;
        public AnimationType AnimationType { get; set; } = AnimationType.SlideH;
        #endregion
        #region Mouse
        [JsonIgnore]
        public int MouseX { get; private set; }

        [JsonIgnore]
        public int MouseY { get; private set; }

        [JsonIgnore]
        public int MouseDownX { get; private set; }

        [JsonIgnore]
        public int MouseDownY { get; private set; }
        #endregion
        #region Page
        public DvPageCollection Pages { get; private set; }
        public DvPage MasterPage { get; private set; }

        [JsonIgnore]
        public DvPage CurrentPage
        {
            get => nowSelPage;
            private set
            {
                if (Pages.Values.Contains(value))
                {
                    if (nowSelPage != value)
                    {
                        prevSelPage = nowSelPage;
                        nowSelPage = value;

                        if (Animation)
                        {
                            var pi = prevSelPage != null ? Pages.Values.ToList().IndexOf(prevSelPage) : -1;
                            var ni = nowSelPage != null ? Pages.Values.ToList().IndexOf(nowSelPage) : -1;

                            #region Capture
                            {
                                if (bmCapture != null) { bmCapture.Dispose(); bmCapture = null; }
                                bmCapture = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul);
                                using (var cv = new SKCanvas(bmCapture))
                                    if (prevSelPage != null) prevSelPage._Draw(cv);
                            }
                            #endregion

                            ani.Stop();
                            ani.Start(DvDesign.ANI2, ni < pi ? "Left" : "Right");
                        }
                    }
                }
            }
        }
        #endregion
        #region IsDrag
        public bool IsDrag => dragItem != null;
        #endregion
        #region InputControl
        internal DvControl InputControl { get; private set; } = null;
        #endregion
        #endregion

        #region Member Variable
        private Stack<DvWindow> stkWindow = new Stack<DvWindow>();
        private DvDropDownWindow dropdownWindow;

        private DvPage nowSelPage = null;
        private DvPage prevSelPage = null;
        private Animation ani = new Animation();

        private SKBitmap bmCapture;

        private object dragItem = null;
        #endregion

        #region Constructor
        public DvDesign()
        {
            Pages = new DvPageCollection(this);
            MasterPage = new DvPage() { Name = "_MASTER_", Design = this, BackgroundDraw = true, UseMasterPage = false };
            Theme = new BlackTheme();

            var v1 = ResourceTool.saturation;
            var v2 = ColorTool.GetName(SKColors.White, ColorCodeType.ARGB);
            FA.GetFAI("fa-box");
            FontTool.Load();


            ActiveDesign = this;
        }
        #endregion

        #region Method
        #region Unload
        public void Unload()
        {
            MasterPage._Unload();
            foreach (var page in Pages.Values) page._Unload();
        }
        #endregion

        #region Draw
        public void Draw(SKCanvas Canvas)
        {
            #region Page
            if (CurrentPage != null)
            {
                if (CurrentPage.UseMasterPage) MasterPage._Draw(Canvas);

                if (Animation && ani.IsPlaying)
                {
                    var rt = Util.FromRect(0, 0, Width, Height);
                    var rtP = new SKRect();
                    var rtN = new SKRect();
                    var aP = Convert.ToByte(ani.Value(AnimationAccel.DCL, 255, 0));
                    var aN = Convert.ToByte(ani.Value(AnimationAccel.DCL, 0, 255));

                    #region Animation
                    var pat = (prevSelPage != null && prevSelPage.AnimationType.HasValue ? prevSelPage.AnimationType.Value : (AnimationType?)null);
                    var nat = (nowSelPage != null && nowSelPage.AnimationType.HasValue ? nowSelPage.AnimationType.Value : (AnimationType?)null);

                    var AnimationType = pat.HasValue ? pat.Value : (nat.HasValue ? nat.Value : this.AnimationType);

                    if (AnimationType == AnimationType.SlideH)
                    {
                        rtP = ani.Value(AnimationAccel.DCL, rt, Util.FromRect(ani.Variable == "Left" ? Width : -Width, 0, Width, Height));
                        rtN = ani.Value(AnimationAccel.DCL, Util.FromRect(ani.Variable == "Left" ? -Width : Width, 0, Width, Height), rt);
                        aP = Convert.ToByte(ani.Value(AnimationAccel.DCL, 255, 0));
                        aN = Convert.ToByte(ani.Value(AnimationAccel.DCL, 0, 255));

                    }
                    else if (AnimationType == AnimationType.SlideV)
                    {
                        rtP = ani.Value(AnimationAccel.DCL, rt, Util.FromRect(0, ani.Variable == "Left" ? Height : -Height, Width, Height));
                        rtN = ani.Value(AnimationAccel.DCL, Util.FromRect(0, ani.Variable == "Left" ? -Height : Height, Width, Height), rt);
                        aP = Convert.ToByte(ani.Value(AnimationAccel.DCL, 255, 0));
                        aN = Convert.ToByte(ani.Value(AnimationAccel.DCL, 0, 255));

                    }
                    else if (AnimationType == AnimationType.Fade)
                    {
                        rtP = rt;
                        rtN = rt;
                        aP = Convert.ToByte(ani.Value(AnimationAccel.DCL, 255, 0));
                        aN = Convert.ToByte(ani.Value(AnimationAccel.DCL, 0, 255));
                    }
                    else if (AnimationType == AnimationType.Drill)
                    {
                        var vv = ani.Variable == "Left" ? -1 : 1;
                        var _rtP = rt; _rtP.Inflate(rt.Width / 10 * vv, rt.Height / 10 * vv);
                        var _rtN = rt; _rtN.Inflate(-rt.Width / 10 * vv, -rt.Height / 10 * vv);
                        rtP = ani.Value(AnimationAccel.DCL, rt, _rtP);
                        rtN = ani.Value(AnimationAccel.DCL, _rtN, rt);
                        aP = Convert.ToByte(ani.Value(AnimationAccel.DCL, 255, 0));
                        aN = Convert.ToByte(ani.Value(AnimationAccel.DCL, 0, 255));
                    }
                    else if (AnimationType == AnimationType.None)
                    {
                        rtP = rt;
                        rtN = rt;
                        aP = 0;
                        aN = 255;
                    }
                    #endregion

                    #region prevSelPage
                    if (prevSelPage != null && AnimationType != AnimationType.None)
                    {
                        /*
                        using (var bm = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul))
                        {
                            using (var cv = new SKCanvas(bm)) prevSelPage._Draw(cv);
                            
                            using (var p = new SKPaint())
                            {
                                p.Color = SKColors.White.WithAlpha(aP);
                                Canvas.DrawBitmap(bm, rtP, p);
                            }
                        }
                        */

                        if (bmCapture != null)
                        {
                            using (var p = new SKPaint())
                            {
                                p.Color = SKColors.White.WithAlpha(aP);
                                Canvas.DrawBitmap(bmCapture, rtP, p);
                            }
                        }
                    }
                    #endregion
                    #region nowSelPage
                    if (CurrentPage != null)
                    {
                        using (var bm = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul))
                        {
                            using (var cv = new SKCanvas(bm))
                            {
                                CurrentPage._Draw(cv);
                            }
                            using (var p = new SKPaint())
                            {
                                p.Color = SKColors.White.WithAlpha(aN);
                                Canvas.DrawBitmap(bm, rtN, p);
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    CurrentPage._Draw(Canvas);
                }
            }
            else
            {
                MasterPage._Draw(Canvas);
            }
            #endregion

            #region Window
            if (stkWindow.Count > 0)
            {
                var ls = stkWindow.Reverse().ToList();

                foreach (var w in ls)
                {
                    #region Window
                    var sp = Canvas.Save();

                    w._Draw(Canvas);

                    Canvas.RestoreToCount(sp);
                    #endregion
                }
            }
            #endregion

            #region DropDownWindow
            if (dropdownWindow != null)
            {
                dropdownWindow._Draw(Canvas);
            }
            #endregion

            #region DragDrop
            if(dragItem != null)
            {
                var rt = Util.FromRect(MouseX - 12, MouseY - 12, 24, 24);
                rt.Offset(5, 10);
                Util.DrawIcon(Canvas, "fa-hand-pointer", 24, Theme.ForeColor, rt);
            }
            #endregion
        }
        #endregion

        #region Update
        public void Update()
        {
            MasterPage?._Update();
            CurrentPage?._Update();

            #region Window
            if (stkWindow.Count > 0)
            {
                var ls = stkWindow.Reverse().ToList();

                foreach (var w in ls)
                {
                    w._Update();
                }
            }
            #endregion
        }
        #endregion

        #region Mouse
        #region MouseDown
        public void MouseDown(int x, int y)
        {
            ActiveDesign = this;

            MouseDownX = MouseX = x;
            MouseDownY = MouseY = y;

            if (dropdownWindow != null)
            {
                var w = dropdownWindow;
                if (CollisionTool.Check(Util.FromRect(w.X, w.Y, w.Width, w.Height), x, y))
                {
                    w._bMouseDown_ = true;
                    w._MouseDown(x - w.X, y - w.Y);
                }
                else dropdownWindow.Hide();
            }
            else if (stkWindow.Count > 0)
            {
                var w = stkWindow.Peek();
                if (CollisionTool.Check(Util.FromRect(w.X, w.Y, w.Width, w.Height), x, y))
                {
                    w._bMouseDown_ = true;
                    w._MouseDown(x - w.X, y - w.Y);
                }
            }
            else
            {
                if (CurrentPage != null)
                {
                    if (CurrentPage.UseMasterPage) MasterPage._MouseDown(x, y);
                    CurrentPage._MouseDown(x, y);
                }
                else MasterPage._MouseDown(x, y);
            }
        }
        #endregion
        #region MouseUp
        public void MouseUp(int x, int y)
        {
            MouseX = x;
            MouseY = y;
            
            if (dropdownWindow != null)
            {
                var w = dropdownWindow;
                if (CollisionTool.Check(Util.FromRect(w.X, w.Y, w.Width, w.Height), x, y) || w._bMouseDown_)
                    w._MouseUp(x - w.X, y - w.Y);
                w._bMouseDown_ = false;
            }
            else if (stkWindow.Count > 0)
            {
                var w = stkWindow.Peek();
                if (CollisionTool.Check(Util.FromRect(w.X, w.Y, w.Width, w.Height), x, y) || w._bMouseDown_)
                    w._MouseUp(x - w.X, y - w.Y);
                w._bMouseDown_ = false;
            }
            else
            {
                if (CurrentPage != null)
                {
                    if (CurrentPage.UseMasterPage) MasterPage._MouseUp(x, y);
                    CurrentPage._MouseUp(x, y);
                }
                else MasterPage._MouseUp(x, y);
            }

            if (dragItem != null)
            {
                dragItem = null;
            }

            InputControl = null;
        }
        #endregion
        #region MouseMove
        public void MouseMove(int x, int y)
        {
            MouseX = x;
            MouseY = y;

            if (dropdownWindow != null)
            {
                var w = dropdownWindow;
                if (CollisionTool.Check(Util.FromRect(w.X, w.Y, w.Width, w.Height), x, y) || w._bMouseDown_)
                    w._MouseMove(x - w.X, y - w.Y);
            }
            else if (stkWindow.Count > 0)
            {
                var w = stkWindow.Peek();
                if (CollisionTool.Check(Util.FromRect(w.X, w.Y, w.Width, w.Height), x, y) || w._bMouseDown_)
                    w._MouseMove(x - w.X, y - w.Y);
            }
            else
            {
                if (CurrentPage != null)
                {
                    if (CurrentPage.UseMasterPage) MasterPage._MouseMove(x, y);
                    CurrentPage._MouseMove(x, y);
                }
                else MasterPage._MouseMove(x, y);
            }
        }
        #endregion
        #region MouseDoubleClick
        public void MouseDoubleClick(int x, int y)
        {
            if (dropdownWindow != null)
            {
                var w = dropdownWindow;
                if (CollisionTool.Check(Util.FromRect(w.X, w.Y, w.Width, w.Height), x, y))
                    w._MouseDoubleClick(x - w.X, y - w.Y);
            }
            else if (stkWindow.Count > 0)
            {
                var w = stkWindow.Peek();
                if (CollisionTool.Check(Util.FromRect(w.X, w.Y, w.Width, w.Height), x, y))
                    w._MouseDoubleClick(x - w.X, y - w.Y);
            }
            else
            {
                if (CurrentPage != null)
                {
                    if (CurrentPage.UseMasterPage) MasterPage._MouseDoubleClick(x, y);
                    CurrentPage._MouseDoubleClick(x, y);
                }
                else MasterPage._MouseDoubleClick(x, y);
            }
        }
        #endregion
        #endregion

        #region SetPage
        public void SetPage(DvPage page)
        {
            if (Pages.ContainsKey(page.Name) && Pages.Values.Contains(page) && page.Design == this)
            {
                CurrentPage?._Hide();

                CurrentPage = page;

                CurrentPage?._Show();
            }
        }

        public void SetPage(string pageName)
        {
            if (Pages.ContainsKey(pageName))
            {
                var page = Pages[pageName];
                if (Pages.Values.Contains(page) && page.Design == this)
                {
                    CurrentPage?._Hide();

                    CurrentPage = page;

                    CurrentPage?._Show();
                }
            }
        }
        #endregion

        #region Window
        #region ShowWindow
        internal void ShowWindow(DvWindow window)
        {
            stkWindow.Push(window);
        }
        #endregion
        #region HideWindow
        internal void HideWindow(DvWindow window)
        {
            if (stkWindow.Count > 0)
            {
                var w = stkWindow.Peek();
                if (window == w) stkWindow.Pop();
            }
        }
        #endregion
        #region VisibleWindow
        public bool VisibleWindow(DvWindow window)
        {
            return stkWindow.Contains(window);
        }
        #endregion
        #endregion

        #region DropDownWindow
        #region ShowDropWindow
        internal void ShowDropDownWindow(DvDropDownWindow window)
        {
            dropdownWindow = window;
        }
        #endregion
        #region HideDropDownWindow
        internal void HideDropDownWindow(DvDropDownWindow window)
        {
            if (dropdownWindow == window)
                dropdownWindow = null;
        }
        #endregion
        #region VisibleDropDownWindow
        public bool VisibleDropDownWindow(DvDropDownWindow window)
        {
            return dropdownWindow == window;
        }
        #endregion
        #endregion

        #region DragDrop
        public void Drag(object o) => dragItem = o;
        public object GetDragItem() => dragItem;
        #endregion

        #region SetTheme
        public void SetTheme(DvTheme Theme) => this.Theme = Theme;
        #endregion

        #region Input
        internal void Input(DvControl c)
        {
            if (InputControl == null)
                InputControl = c;
        }
        #endregion
        #endregion

    }
}

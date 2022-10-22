using Devinno.Skia.Collections;
using Devinno.Skia.Design;
using Devinno.Skia.Design;
using Devinno.Skia.Utils;
using Newtonsoft.Json;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Containers
{
    public class DvSwitchPanel : DvContainer
    {
        #region Properties
        public DvSubPageCollection Pages { get; private set; }
        #region public DvSubPage SelectedPage { get; set; }
        [JsonIgnore]
        public DvSubPage SelectedPage
        {
            get => nowSelPage;
            set
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

                            bounds((rtContent, rtPage, rtNavi) =>
                            {
                                var Width = Convert.ToInt32(rtPage.Width) + 3;
                                var Height = Convert.ToInt32(rtPage.Height) + 3;

                                if (bmCapture != null) bmCapture.Dispose();
                                bmCapture = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul);
                                using (var cv = new SKCanvas(bmCapture)) prevSelPage._Draw(cv);
                            });

                            ani.Stop();
                            ani.Start(DvDesign.ANI2, ni < pi ? "Left" : "Right");
                        }

                        SelectedPageChanged?.Invoke(this, null);
                    }
                }
            }
        }
        #endregion
        #region public string SelectedPageName { get; set; }
        [JsonIgnore]
        public string SelectedPageName
        {
            get => nowSelPage?.Name;
            set
            {
                if (Pages.ContainsKey(value)) SelectedPage = Pages[value];
            }
        }
        #endregion
        public override DvControlCollection Controls => SelectedPage?.Controls;
        public bool Animation => Design != null ? Design.Animation : false;
        public AnimationType AnimationType { get; set; } = AnimationType.SlideH;

        #endregion

        #region Member Variable
        private DvSubPage nowSelPage = null;
        private DvSubPage prevSelPage = null;
        private Animation ani = new Animation();

        private SKBitmap bmCapture;
        #endregion

        #region Event
        public EventHandler SelectedPageChanged;
        #endregion

        #region Constructor
        public DvSwitchPanel()
        {
            Pages = new DvSubPageCollection(this);
            Pages.Changed += (o, s) => OnLayout();
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent, rtPage, rtNavi) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    OnLayout();

                    var Width = Convert.ToInt32(rtPage.Width) + 3;
                    var Height = Convert.ToInt32(rtPage.Height) + 3;
                    var rt = Util.FromRect(0, 0, Width, Height);

                    if (Animation && ani.IsPlaying)
                    {
                        var rtP = new SKRect();
                        var rtN = new SKRect();
                        var aP = Convert.ToByte(ani.Value(AnimationAccel.DCL, 255, 0));
                        var aN = Convert.ToByte(ani.Value(AnimationAccel.DCL, 0, 255));

                        #region Animation
                        if (AnimationType == AnimationType.SlideH)
                        {
                            rtP = ani.Value(AnimationAccel.DCL, rt, Util.FromRect(ani.Variable == "Left" ? Width : 0, 0, 0, Height));
                            rtN = ani.Value(AnimationAccel.DCL, Util.FromRect(ani.Variable == "Left" ? 0 : Width, 0, 0, Height), rt);
                        }
                        else if (AnimationType == AnimationType.SlideV)
                        {
                            rtP = ani.Value(AnimationAccel.DCL, rt, Util.FromRect(0, ani.Variable == "Left" ? Height : 0, Width, 0));
                            rtN = ani.Value(AnimationAccel.DCL, Util.FromRect(0, ani.Variable == "Left" ? 0 : Height, Width, 0), rt);
                        }
                        else if (AnimationType == AnimationType.Fade)
                        {
                            rtP = rt;
                            rtN = rt;
                        }
                        else if (AnimationType == AnimationType.Drill)
                        {
                            var vv = ani.Variable == "Left" ? -1 : 1;
                            var _rtP = rt; _rtP.Inflate(rt.Width / 10 * vv, rt.Height / 10 * vv);
                            var _rtN = rt; _rtN.Inflate(-rt.Width / 10 * vv, -rt.Height / 10 * vv);
                            rtP = ani.Value(AnimationAccel.DCL, rt, _rtP);
                            rtN = ani.Value(AnimationAccel.DCL, _rtN, rt);
                        }
                        else if (AnimationType == AnimationType.None)
                        {
                            rtP = rt;
                            rtN = rt;
                            aP = 0;
                            aN = 255;
                        }
                        #endregion

                        #region prevSelTab
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
                        #region SelectedTab
                        if (SelectedPage != null)
                        {
                            using (var bm = new SKBitmap(Width, Height, SKColorType.Rgba8888, SKAlphaType.Premul))
                            {
                                using (var cv = new SKCanvas(bm)) SelectedPage._Draw(cv);

                                using (var p = new SKPaint())
                                {
                                    p.Color = SKColors.White.WithAlpha(Convert.ToByte(aN * (!Enabled ? 90F / 255F : 1F)));
                                    Canvas.DrawBitmap(bm, rtN, p);
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        SelectedPage._Draw(Canvas); //base.OnDraw(cvs);
                    }
                }
            });
        }
        #endregion
        #region OnMouse[Down/Move/Up/DoubleClick]
        protected override void OnMouseDown(int x, int y) => bounds((rtContent, rtPage, rtNavi) => base.OnMouseDown(x - (int)rtPage.Left, y - (int)rtPage.Top));
        protected override void OnMouseMove(int x, int y) => bounds((rtContent, rtPage, rtNavi) => base.OnMouseMove(x - (int)rtPage.Left, y - (int)rtPage.Top));
        protected override void OnMouseUp(int x, int y) => bounds((rtContent, rtPage, rtNavi) => base.OnMouseUp(x - (int)rtPage.Left, y - (int)rtPage.Top));
        protected override void OnMouseDoubleClick(int x, int y) => bounds((rtContent, rtPage, rtNavi) => base.OnMouseDoubleClick(x - (int)rtPage.Left, y - (int)rtPage.Top));
        #endregion
        #region OnLayout
        protected override void OnLayout()
        {
            bounds((rtContent, rtPage, rtNavi) =>
            {
                foreach (var v in Pages)
                {
                    v.Value.X = (int)rtPage.Left;
                    v.Value.Y = (int)rtPage.Top;
                    v.Value.Width = (int)rtPage.Width;
                    v.Value.Height = (int)rtPage.Height;
                }
            });
        }
        #endregion
        #endregion

        #region Method
        #region bounds
        void bounds(Action<SKRect, SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(1, 1, this.Width-2, this.Height-2);
            var rtPage = rtContent;
            var rtNavi = new SKRect();
            var dicTab = new Dictionary<string, SKRect>();

            act(rtContent, rtPage, rtNavi);
        }
        #endregion
        #endregion
    }

    public class DvSubPage : DvContainer
    {
        #region Properties
        [JsonIgnore]
        public DvSwitchPanel TabControl { get; internal set; }
        
        public string Text { get; set; } = "Page";
        public string IconString { get; set; } = null;
        #endregion
    }

}

using Devinno.Skia.Collections;
using Devinno.Skia.Design;
using Devinno.Skia.Theme;
using Devinno.Skia.Tools;
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
                Areas((rtContent, rtPage) =>
                {
                    if (Pages.Values.Contains(value))
                    {
                        if (nowSelPage != value)
                        {
                            prevSelPage = nowSelPage;
                            nowSelPage = value;

                            if (Animation)
                            {
                                ani.Stop();
                                ani.Start(DvDesign.ANI2, "Next");
                            }

                            SelectedPageChanged?.Invoke(this, null);
                        }
                    }
                });
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

        private SKBitmap bmCapture = null;
        #endregion

        #region Event
        public event EventHandler SelectedPageChanged;
        #endregion

        #region Constructor
        public DvSwitchPanel()
        {
            Pages = new DvSubPageCollection(this);
            Pages.Changed += (o, s) => OnLayout();
        }
        #endregion

        #region Override
        #region Draw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent, rtPage) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    OnLayout();

                    #region Page
                    if (SelectedPage != null)
                    {
                        if (Animation && ani.IsPlaying && AnimationType != AnimationType.None)
                        {
                            Ani(rtContent, rtPage, (rtP, rtN, aP, aN) =>
                            {
                                #region Prev
                                if (prevSelPage != null)
                                {
                                    var sp = Canvas.Save();
                                    Canvas.ClipRect(rtPage);

                                    var w = Convert.ToInt32(rtPage.Width);
                                    var h = Convert.ToInt32(rtPage.Height);

                                    using (var bm = new SKBitmap(w, h, SKColorType.Rgba8888, SKAlphaType.Premul))
                                    {
                                        using (var cv = new SKCanvas(bm)) prevSelPage._Draw(cv);

                                        using (var p = new SKPaint { IsAntialias = DvDesign.AA, IsDither = DvDesign.DI, FilterQuality = DvDesign.FQ })
                                        {
                                            p.Color = SKColors.White.WithAlpha(aP);
                                            Canvas.DrawBitmap(bm, rtP, p);
                                        }
                                    }

                                    Canvas.RestoreToCount(sp);
                                }
                                #endregion
                                #region Now
                                if (nowSelPage != null)
                                {
                                    var sp = Canvas.Save();
                                    Canvas.ClipRect(rtPage);

                                    var w = Convert.ToInt32(rtPage.Width);
                                    var h = Convert.ToInt32(rtPage.Height);

                                    using (var bm = new SKBitmap(w, h, SKColorType.Rgba8888, SKAlphaType.Premul))
                                    {
                                        using (var cv = new SKCanvas(bm)) nowSelPage._Draw(cv);

                                        using (var p = new SKPaint { IsAntialias = DvDesign.AA, IsDither = DvDesign.DI, FilterQuality = DvDesign.FQ })
                                        {
                                            p.Color = SKColors.White.WithAlpha(aN);
                                            Canvas.DrawBitmap(bm, rtN, p);
                                        }
                                    }

                                    Canvas.RestoreToCount(sp);
                                }
                                #endregion
                            });
                        }
                        else
                        {
                            #region Now
                            var sp = Canvas.Save();
                            Canvas.ClipRect(rtPage);
                            Canvas.Translate(rtPage.Left, rtPage.Top);

                            nowSelPage._Draw(Canvas);

                            Canvas.RestoreToCount(sp);
                            #endregion
                        }
                    }
                    #endregion
                }
            });
        }
        #endregion

        #region Mouse
        protected override void OnMouseDown(float x, float y) => Areas((rtContent, rtPage) => base.OnMouseDown(x - rtPage.Left, y - rtPage.Top));
        protected override void OnMouseMove(float x, float y) => Areas((rtContent, rtPage) => base.OnMouseMove(x - rtPage.Left, y - rtPage.Top));
        protected override void OnMouseUp(float x, float y) => Areas((rtContent, rtPage) => base.OnMouseUp(x - rtPage.Left, y - rtPage.Top));
        protected override void OnMouseDoubleClick(float x, float y) => Areas((rtContent, rtPage) => base.OnMouseDoubleClick(x - rtPage.Left, y - rtPage.Top));
        #endregion

        #region Layout
        protected override void OnLayout()
        {
            Areas((rtContent, rtPage) =>
            {
                foreach (var page in Pages.Values) page.Bounds = rtPage;
            });
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect, SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, Width, Height);
            var rtPage = rtContent;

            act(rtContent, rtPage);
        }
        #endregion
        #region Ani 
        void Ani(SKRect rtContent, SKRect rtPage, Action<SKRect, SKRect, byte, byte> act)
        {
            var si = Pages.Values.ToList().IndexOf(nowSelPage);
            if (si != -1)
            {
                #region var
                var rtP = new SKRect();
                var rtN = new SKRect();
                var aP = Convert.ToByte(ani.Value(AnimationAccel.DCL, 255, 0));
                var aN = Convert.ToByte(ani.Value(AnimationAccel.DCL, 0, 255));
                #endregion

                #region Animation
                if (AnimationType == AnimationType.SlideH)
                {
                    #region SlideH
                    var _rtP = Util.FromRect(rtPage);
                    var _rtN = Util.FromRect(rtPage);

                    if (ani.Variable == "Prev")
                    {
                        _rtP.Offset(rtPage.Width, 0);
                        _rtN.Offset(-rtPage.Width, 0);
                    }
                    else if (ani.Variable == "Next")
                    {
                        _rtP.Offset(-rtPage.Width, 0);
                        _rtN.Offset(rtPage.Width, 0);
                    }

                    rtP = ani.Value(AnimationAccel.DCL, rtPage, _rtP);
                    rtN = ani.Value(AnimationAccel.DCL, _rtN, rtPage);
                    #endregion
                }
                else if (AnimationType == AnimationType.SlideV)
                {
                    #region SlideV
                    var _rtP = Util.FromRect(rtPage);
                    var _rtN = Util.FromRect(rtPage);

                    if (ani.Variable == "Prev")
                    {
                        _rtP.Offset(0, rtPage.Height);
                        _rtN.Offset(0, -rtPage.Height);
                    }
                    else if (ani.Variable == "Next")
                    {
                        _rtP.Offset(0, -rtPage.Height);
                        _rtN.Offset(0, rtPage.Height);
                    }

                    rtP = ani.Value(AnimationAccel.DCL, rtPage, _rtP);
                    rtN = ani.Value(AnimationAccel.DCL, _rtN, rtPage);
                    #endregion
                }
                else if (AnimationType == AnimationType.Fade)
                {
                    #region Fade
                    rtP = rtPage;
                    rtN = rtPage;
                    #endregion
                }
                else if (AnimationType == AnimationType.Drill)
                {
                    #region Drill
                    var vv = ani.Variable == "Prev" ? -10F : 10F;
                    var _rtP = rtPage; _rtP.Inflate(rtPage.Width / vv, rtPage.Height / vv);
                    var _rtN = rtPage; _rtN.Inflate(-rtPage.Width / vv, -rtPage.Height / vv);
                    rtP = ani.Value(AnimationAccel.DCL, rtPage, _rtP);
                    rtN = ani.Value(AnimationAccel.DCL, _rtN, rtPage);
                    #endregion
                }
                else if (AnimationType == AnimationType.None)
                {
                    #region None
                    rtP = rtPage;
                    rtN = rtPage;
                    aP = 0;
                    aN = 255;
                    #endregion
                }
                #endregion

                act(rtP, rtN, aP, aN);
            }
        }
        #endregion
        #endregion
    }
}

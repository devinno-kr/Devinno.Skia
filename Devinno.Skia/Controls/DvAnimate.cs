using Devinno.Skia.Design;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvAnimate : Design.DvControl
    {
        #region Properties
        public bool IsStart { get; private set; }
        public bool IsLoaded { get; private set; }
        public int FrameCount { get; private set; }
        public int CurrentIndex { get; private set; }
        public int Time { get; set; } = 30;
        public SKBitmap OffImage { get; set; }
        public bool OnOff
        {
            get => IsStart;
            set
            {
                if (value) Start();
                else Stop();
            }
        }
        #endregion

        #region Member Variable
        List<SKBitmap> bmps = new List<SKBitmap>();
        DateTime prev;
        #endregion

        #region Constructor
        public DvAnimate()
        {
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            Areas((rtContent) =>
            {
                var ds = Design;
                var thm = Design?.Theme;
                if (ds != null && thm != null)
                {
                    if (IsStart)
                    {
                        Canvas.DrawBitmap(bmps[CurrentIndex], rtContent);
                        if ((DateTime.Now - prev).TotalMilliseconds >= Time)
                        {
                            CurrentIndex++;
                            if (CurrentIndex >= FrameCount) CurrentIndex = 0;
                            prev = DateTime.Now;
                        }
                    }
                    else
                    {
                        if (OffImage != null) Canvas.DrawBitmap(OffImage, rtContent);
                        else
                        {
                            using (var p = new SKPaint() { IsAntialias = DvDesign.AA, FilterQuality = DvDesign.FQ, IsDither = DvDesign.DI })
                            {
                                p.IsStroke = false;
                                p.Color = SKColors.Black;
                                Canvas.DrawRect(rtContent, p);
                            }
                        }
                    }
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #endregion

        #region Method
        #region Areas
        public void Areas(Action<SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width, this.Height);

            act(rtContent);
        }
        #endregion
        #region LoadGIF
        public void LoadGIF(string ImagePath)
        {
            if (ImagePath != null && File.Exists(ImagePath) && !IsStart)
            {
                IsLoaded = false;

                try
                {
                    var ls = ImageExtractor.ProcessImageFromMemory(File.ReadAllBytes(ImagePath));
                    FrameCount = ls.Count;
                    bmps.Clear();
                    bmps.AddRange(ls);
                    CurrentIndex = 0;
                    IsStart = false;
                }
                catch { }

                IsLoaded = true;
            }
        }
        #endregion
        #region Load
        public void Load(params string[] ImagePaths)
        {
            //ThreadPool.QueueUserWorkItem((o) =>
            //{
            IsLoaded = false;
            foreach (var ImagePath in ImagePaths)
            {
                if (ImagePath != null && File.Exists(ImagePath) && !IsStart) 
                    bmps.Add(Util.FromBitmap(ImagePath));
            }

            CurrentIndex = 0;
            FrameCount = bmps.Count;
            IsStart = false;

            IsLoaded = true;
            //});
        }
        #endregion
        #region Unload
        public void Unload()
        {
            var olds = bmps.ToList();
            bmps.Clear();
            foreach (var v in olds) v.Dispose();
        }
        #endregion
        #region Start
        public void Start()
        {
            if (!IsStart && IsLoaded && bmps.Count > 0)
            {
                CurrentIndex = 0;
                IsStart = true;
            }
        }
        #endregion
        #region Stop
        public void Stop()
        {
            if (IsStart)
            {
                IsStart = false;
            }
        }
        #endregion
        #endregion
    }
}

using Devinno.Skia.Design;
using Devinno.Skia.Movie;
using Devinno.Skia.Tools;
using Devinno.Skia.Utils;
using FFmpeg.AutoGen;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Devinno.Skia.Controls
{
    public class DvMovie : DvControl
    {
        #region Properteis
        public string Url { get; set; } = null;
        public bool IsStart { get; private set; } = false;
        public bool Repeat { get; set; }
        public int? Delay { get; set; } = null;
        #endregion

        #region Event
        public event EventHandler PlayComplete;
        #endregion

        #region Member Variable
        private SKBitmap bmp;
        private Thread th;
        private object olock = new object();
        #endregion

        #region Constructor
        public DvMovie()
        {
            FFMpegHelper.Register();
        }
        #endregion

        #region Override
        #region OnDraw
        protected override void OnDraw(SKCanvas Canvas)
        {
            bounds((rtContent) =>
            {
                if (IsStart && bmp != null)
                {
                    lock (olock)
                        Canvas.DrawBitmap(bmp, rtContent);
                }
                else
                {
                    using (var p = new SKPaint())
                    {
                        p.IsStroke = false;
                        p.Color = SKColors.Black;
                        Canvas.DrawRect(rtContent, p);
                    }
                }
            });
            base.OnDraw(Canvas);
        }
        #endregion
        #region OnUnload
        public override void OnUnload()
        {
            IsStart = false;
            base.OnUnload();
        }
        #endregion
        #endregion

        #region Method
        #region Start
        public void Start()
        {
            if (!this.IsStart)
            {
                this.th = new Thread(Process) { IsBackground = false };
                this.th.Start();
            }
        }
        #endregion
        #region Stop
        public void Stop()
        {
            if(this.IsStart)
            {
                this.IsStart = false;

                var old = bmp;
                bmp = null;
                if (old != null) old.Dispose();
            }
        }
        #endregion
        #region Process
        unsafe void Process()
        {
            IsStart = true;

            var w = this.Width - 2;
            var h = this.Height - 2;
            
            using (var decoder = new VideoStreamDecoder(Url) { Repeat = this.Repeat })
            {
                var contextInfoDictionary = decoder.GetContextInfoDictionary();
                var sourceSize = decoder.FrameSize;
                var sourcePixelFormat = decoder.PixelFormat;
                var targetSize = sourceSize;
                var targetPixelFormat = AVPixelFormat.AV_PIX_FMT_RGB565LE;
                var startTime = ffmpeg.av_gettime();
                using (var converter = new VideoFrameConverter(sourceSize, sourcePixelFormat, targetSize, targetPixelFormat) )
                {
                    bool rp;
                    while (decoder.TryDecodeNextFrame(out AVFrame sourceFrame, out rp) && IsStart)
                    {
                        if(rp) startTime = ffmpeg.av_gettime();
                        AVFrame targetFrame = converter.Convert(sourceFrame);

                        var ct = SKColorType.Rgb565;
                        lock (olock)
                        {
                            if (bmp == null) bmp = new SKBitmap(targetFrame.width, targetFrame.height, ct, SKAlphaType.Premul);
                            bmp.InstallPixels(new SKImageInfo()
                            {
                                Width = targetFrame.width,
                                Height = targetFrame.height,
                                ColorType = ct,
                                AlphaType = SKAlphaType.Premul
                            }, (IntPtr)targetFrame.data[0], targetFrame.linesize[0]);
                        }

                        if (Delay.HasValue) System.Threading.Thread.Sleep(Delay.Value);
                        else
                        {
                            var streamIndex = decoder.GetStreamIndex();
                            var formatContext = decoder.GetFormatContext();

                            AVRational time_base = formatContext->streams[streamIndex]->time_base;
                            AVRational time_base_q = new AVRational { num = 1, den = ffmpeg.AV_TIME_BASE };
                            long pts_time = ffmpeg.av_rescale_q(sourceFrame.pkt_dts, time_base, time_base_q);
                            long now_time = ffmpeg.av_gettime() - startTime;
                            if (pts_time > now_time)
                                ffmpeg.av_usleep(Convert.ToUInt32(pts_time - now_time));
                        }
                    }
                }

                var old = bmp;
                bmp = null;
                if (old != null) old.Dispose();

                if (IsStart) PlayComplete?.Invoke(this, null);
            }

            IsStart = false;
        }
        #endregion

        #region bounds
        void bounds(Action<SKRect> act)
        {
            var rtContent = Util.FromRect(0, 0, this.Width - 0, this.Height - 0);

            act(rtContent);
        }
        #endregion
        #endregion
    }
}

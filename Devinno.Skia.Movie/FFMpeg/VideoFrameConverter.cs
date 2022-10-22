using System;
using System.Runtime.InteropServices;
using System.Windows;

using FFmpeg.AutoGen;
using SkiaSharp;

namespace Devinno.Skia.Movie
{
    internal sealed unsafe class VideoFrameConverter : IDisposable
    {
        #region Field
        private readonly SKSizeI targetSize;
        private readonly SwsContext* context;
        private readonly IntPtr buferHandle;
        private readonly byte_ptrArray4 temporaryFrameData;
        private readonly int_array4 temporaryFrameLineSize;
        #endregion

        #region 생성자 - VideoFrameConverter(sourceSize, sourcePixelFormat, targetSize, targetPixelFormat)
        public VideoFrameConverter(SKSizeI sourceSize, AVPixelFormat sourcePixelFormat, SKSizeI targetSize, AVPixelFormat targetPixelFormat)
        {
            this.targetSize = targetSize;

            this.context = ffmpeg.sws_getContext
            (
                (int)sourceSize.Width,
                (int)sourceSize.Height,
                sourcePixelFormat,
                (int)targetSize.Width,
                (int)targetSize.Height,
                targetPixelFormat,
                ffmpeg.SWS_FAST_BILINEAR,
                null,
                null,
                null
            );

            if(this.context == null)
            {
                throw new ApplicationException("Could not initialize the conversion context.");
            }

            int bufferSize = ffmpeg.av_image_get_buffer_size(targetPixelFormat, (int)targetSize.Width, (int)targetSize.Height, 1);

            this.buferHandle = Marshal.AllocHGlobal(bufferSize);

            this.temporaryFrameData = new byte_ptrArray4();

            this.temporaryFrameLineSize = new int_array4();

            ffmpeg.av_image_fill_arrays
            (
                ref this.temporaryFrameData,
                ref this.temporaryFrameLineSize,
                (byte*)this.buferHandle,
                targetPixelFormat,
                (int)targetSize.Width,
                (int)targetSize.Height,
                1
            );
        }

        #endregion

        #region 변환하기 - Convert(sourceFrame)
        public AVFrame Convert(AVFrame sourceFrame)
        {
            ffmpeg.sws_scale
            (
                this.context,
                sourceFrame.data,
                sourceFrame.linesize,
                0,
                sourceFrame.height,
                this.temporaryFrameData,
                this.temporaryFrameLineSize
            );

            byte_ptrArray8 targetFrameData = new byte_ptrArray8();

            targetFrameData.UpdateFrom(this.temporaryFrameData);

            int_array8 targetFrameLineSize = new int_array8();

            targetFrameLineSize.UpdateFrom(this.temporaryFrameLineSize);

            return new AVFrame
            {
                data     = targetFrameData,
                linesize = targetFrameLineSize,
                width    = (int)this.targetSize.Width,
                height   = (int)this.targetSize.Height
            };
        }
        #endregion

        #region 리소스 해제하기 - Dispose()
        public void Dispose()
        {
            Marshal.FreeHGlobal(this.buferHandle);

            ffmpeg.sws_freeContext(this.context);
        }
        #endregion
    }
}
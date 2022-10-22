using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using Devinno.Skia.Tools;
using FFmpeg.AutoGen;
using SkiaSharp;

namespace Devinno.Skia.Movie
{
    internal sealed unsafe class VideoStreamDecoder : IDisposable
    {
        #region Properteis
        public bool Repeat { get; set; }
        #endregion
        #region Field
        private readonly AVFormatContext* formatContext;
        private readonly int streamIndex;
        private readonly AVCodecContext* codecContext;
        private readonly AVPacket* packet;
        private readonly AVFrame* frame;
        #endregion

        #region 코덱명 - CodecName
        public string CodecName { get; }
        #endregion
        #region 프레임 크기 - FrameSize
        public SKSizeI FrameSize { get; }
        #endregion
        #region 픽셀 포맷 - PixelFormat
        public AVPixelFormat PixelFormat { get; }
        #endregion

        #region 생성자 - VideoStreamDecoder(url)
        public VideoStreamDecoder(string url)
        {
             
            this.formatContext = ffmpeg.avformat_alloc_context();

            AVFormatContext* formatContext = this.formatContext;

            ffmpeg.avformat_open_input(&formatContext, url, null, null).ThrowExceptionIfError();
            ffmpeg.avformat_find_stream_info(this.formatContext, null).ThrowExceptionIfError();

            AVStream* stream = null;

            for (var i = 0; i < this.formatContext->nb_streams; i++)
            {
                if (this.formatContext->streams[i]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    stream = this.formatContext->streams[i];
                }
            }

            if (stream == null) throw new InvalidOperationException("Could not found video stream.");

            this.streamIndex = stream->index;
            this.codecContext = stream->codec;

            AVCodecID codecID = this.codecContext->codec_id;
            AVCodec* codec = ffmpeg.avcodec_find_decoder(codecID);

            if (codec == null) throw new InvalidOperationException("Unsupported codec.");

            ffmpeg.avcodec_open2(this.codecContext, codec, null).ThrowExceptionIfError();

            CodecName = ffmpeg.avcodec_get_name(codecID);
            FrameSize = new SKSizeI(this.codecContext->width, this.codecContext->height);
            PixelFormat = this.codecContext->pix_fmt;

            this.packet = ffmpeg.av_packet_alloc();
            this.frame = ffmpeg.av_frame_alloc();
            
        }
        #endregion

        #region 컨텍스트 정보 딕셔너리 구하기 - GetContextInfoDictionary()
        public IReadOnlyDictionary<string, string> GetContextInfoDictionary()
        {
            AVDictionaryEntry* dictionaryEntry = null;

            Dictionary<string, string> resultDictionary = new Dictionary<string, string>();

            while((dictionaryEntry = ffmpeg.av_dict_get(this.formatContext->metadata, "", dictionaryEntry, ffmpeg.AV_DICT_IGNORE_SUFFIX)) != null)
            {
                string key   = Marshal.PtrToStringAnsi((IntPtr)dictionaryEntry->key  );
                string value = Marshal.PtrToStringAnsi((IntPtr)dictionaryEntry->value);

                resultDictionary.Add(key, value);
            }

            return resultDictionary;
        }
        #endregion
        #region 다음 프레임 디코드 시도하기 - TryDecodeNextFrame(frame)
        public bool TryDecodeNextFrame(out AVFrame frame, out bool rp)
        {
            ffmpeg.av_frame_unref(this.frame);
            
            int errorCode;
            var ps = formatContext->streams[streamIndex];
            var start_time = ffmpeg.av_gettime();
            rp = false;

            do
            {
                try
                {
                    do
                    {
                        errorCode = ffmpeg.av_read_frame(this.formatContext, this.packet);
                        
                        if(errorCode == ffmpeg.AVERROR_EOF)
                        {
                            frame = *this.frame;
                            if (Repeat) { Seek(0); rp = true; return true; }
                            else return false;
                        }

                        errorCode.ThrowExceptionIfError();
                    }
                    while(this.packet->stream_index != this.streamIndex);
                  
                    ffmpeg.avcodec_send_packet(this.codecContext, this.packet).ThrowExceptionIfError();
                }
                finally
                {
                    ffmpeg.av_packet_unref(this.packet);
                }

                errorCode = ffmpeg.avcodec_receive_frame(this.codecContext, this.frame);
            }
            while(errorCode == ffmpeg.AVERROR(ffmpeg.EAGAIN));

            errorCode.ThrowExceptionIfError();

            frame = *this.frame;

            return true;
        }
        #endregion
        #region 리소스 해제하기 - Dispose()
        public void Dispose()
        {
            ffmpeg.av_frame_unref(this.frame);
            ffmpeg.av_free(this.frame);
            ffmpeg.av_packet_unref(this.packet);
            ffmpeg.av_free(this.packet);
            ffmpeg.avcodec_close(this.codecContext);
            AVFormatContext* formatContext = this.formatContext;
            ffmpeg.avformat_close_input(&formatContext);
        }
        #endregion
        #region 탐색 - Seek
        internal void Seek(long seek)
        {
            ffmpeg.av_seek_frame(formatContext, streamIndex, seek, ffmpeg.AVSEEK_FLAG_ANY);
        }
        #endregion

        public AVFormatContext* GetFormatContext() => formatContext;
        public int GetStreamIndex() => streamIndex;
    }
}
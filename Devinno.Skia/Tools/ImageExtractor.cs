using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Devinno.Skia.Tools
{
    public class ImageExtractor
    {
        public static List<SKBitmap> ProcessImageFromMemory(byte[] imageData)
        {
            var frames = new List<SKBitmap>();

            try
            {
                using (var stream = new SKMemoryStream(imageData))
                using (var codec = SKCodec.Create(stream))
                {
                    if (codec == null) throw new Exception("제공된 이미지 데이터에서 코덱을 생성할 수 없습니다.");

                    var info = codec.Info;
                    var frameCount = codec.FrameCount;

                    if (frameCount > 1) // GIF or animated image
                    {
                        for (int i = 0; i < frameCount; i++)
                        {
                            using (var bitmap = new SKBitmap(info))
                            {
                                if (codec.GetFrameInfo(i, out var frameInfo))
                                {
                                    var options = new SKCodecOptions(i);
                                    var result = codec.GetPixels(info, bitmap.GetPixels(), options);
                                    if (result == SKCodecResult.Success)
                                    {
                                        frames.Add(bitmap.Copy());
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        var bitmap = new SKBitmap(info);
                        if (codec.GetPixels(info, bitmap.GetPixels()) == SKCodecResult.Success)
                        {
                            frames.Add(bitmap);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return frames;
        }
    }
}

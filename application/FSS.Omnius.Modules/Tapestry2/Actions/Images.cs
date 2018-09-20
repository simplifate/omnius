using FSS.Omnius.Modules.CORE;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;

namespace FSS.Omnius.Modules.Tapestry2.Actions
{
    public class Images : ActionManager
    {
        [Action(7000, "Image: Download", "Result", "Error")]
        public static (byte[], bool) Download(COREobject core, string Url)
        {
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);

                httpWebRequest.Method = "GET";

                var response = httpWebRequest.GetResponse();
                Stream responseStream = response.GetResponseStream();

                if (response.ContentType.StartsWith("image"))
                {
                    byte[] rawData;
                    using (var ms = new MemoryStream())
                    {
                        responseStream.CopyTo(ms);
                        rawData = ms.ToArray();
                    }

                    return (rawData, false);
                }
                else
                {
                    Watchtower.OmniusInfo.Log($"Requested file \"{Url}\" is not image.");
                    return (new byte[] { }, true);
                }
            }
            catch (Exception e)
            {
                Watchtower.OmniusInfo.Log(e.Message);
                return (new byte[] { }, true);
            }
        }

        [Action(7001, "Image: Resize", "Result")]
        public static byte[] Resize(COREobject core, byte[] ImageData, bool KeepAspectRatio, int Width, int Height = 0)
        {
            MemoryStream outStreem = new MemoryStream();

            using (Stream stream = new MemoryStream(ImageData))
            {
                Image original = Image.FromStream(stream);
                int originalWidth = original.Width;
                int originalHeight = original.Height;

                if (KeepAspectRatio || Height == 0)
                {
                    double aspect = (double)originalWidth / (double)originalHeight;
                    int targetWidth = Width;
                    int targetHeight = (int)System.Math.Floor(Width / aspect);

                    if (Height > 0 && targetHeight > Height)
                    {
                        targetWidth = (int)System.Math.Floor(Height * aspect);
                        targetHeight = Height;
                    }
                    Width = targetWidth;
                    Height = targetHeight;
                }

                Rectangle destRect = new Rectangle(0, 0, Width, Height);
                Bitmap destImage = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);

                destImage.SetResolution(original.HorizontalResolution, original.VerticalResolution);
                using (var g = Graphics.FromImage(destImage))
                {
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    using (var wrapMode = new ImageAttributes())
                    {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        g.DrawImage(original, destRect, 0, 0, originalWidth, originalHeight, GraphicsUnit.Pixel, wrapMode);
                    }
                }

                ImageCodecInfo jpgEncoder = ImageCodecInfo.GetImageDecoders().FirstOrDefault(dec => dec.FormatID == ImageFormat.Jpeg.Guid);
                Encoder myEncoder = Encoder.Quality;
                EncoderParameters myEncoderParams = new EncoderParameters(1);
                EncoderParameter myEncoderParam = new EncoderParameter(myEncoder, 90L);

                myEncoderParams.Param[0] = myEncoderParam;

                destImage.Save(outStreem, jpgEncoder, myEncoderParams);
            }

            return outStreem.ToArray();
        }
    }
}

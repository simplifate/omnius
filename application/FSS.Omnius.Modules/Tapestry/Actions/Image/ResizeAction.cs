using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using FSS.Omnius.Modules.CORE;

namespace FSS.Omnius.Modules.Tapestry.Actions.Entitron
{
    [ImageRepository]
    class ResizeAction : Action
    {
        public override int Id
        {
            get
            {
                return 7001;
            }
        }

        public override string[] InputVar
        {
            get
            {
                return new string[] { "v$ImageData", "i$Width", "?i$Height", "b$KeepAspectRatio" };
            }
        }

        public override string Name
        {
            get
            {
                return "Image: Resize";
            }
        }

        public override string[] OutputVar
        {
            get
            {
                return new string[]
                {
                    "Result"
                };
            }
        }

        public override int? ReverseActionId
        {
            get
            {
                return null;
            }
        }

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            byte[] imageData = (byte[])vars["ImageData"];
            int width = (int)vars["Width"];
            int height = vars.ContainsKey("Height") ? (int)vars["Height"] : 0;
            bool keepAspectRatio = (bool)vars["KeepAspectRatio"];

            MemoryStream outStreem = new MemoryStream();

            using (Stream stream = new MemoryStream(imageData)) {
                Image original = Image.FromStream(stream);
                int originalWidth = original.Width;
                int originalHeight = original.Height;

                if(keepAspectRatio || height == 0) {
                    double aspect = (double)originalWidth / (double)originalHeight;
                    int targetWidth = width;
                    int targetHeight = (int)System.Math.Floor(width / aspect);
                    
                    if(height > 0 && targetHeight > height) {
                        targetWidth = (int)System.Math.Floor(height * aspect);
                        targetHeight = height;
                    }
                    width = targetWidth;
                    height = targetHeight;
                }

                Rectangle destRect = new Rectangle(0, 0, width, height);
                Bitmap destImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                destImage.SetResolution(original.HorizontalResolution, original.VerticalResolution);
                using (var g = Graphics.FromImage(destImage)) {
                    g.CompositingMode = CompositingMode.SourceCopy;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    
                    using(var wrapMode = new ImageAttributes()) {
                        wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                        g.DrawImage(original, destRect, 0, 0, originalWidth, originalHeight, GraphicsUnit.Pixel, wrapMode);
                    }
                }
                
                ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                Encoder myEncoder = Encoder.Quality;
                EncoderParameters myEncoderParams = new EncoderParameters(1);
                EncoderParameter myEncoderParam = new EncoderParameter(myEncoder, 90L);

                myEncoderParams.Param[0] = myEncoderParam;
                
                destImage.Save(outStreem, jpgEncoder, myEncoderParams);
            }

            outputVars["Result"] = outStreem.ToArray();
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs) {
                if (codec.FormatID == format.Guid) {
                    return codec;
                }
            }
            return null;
        }
    }
}

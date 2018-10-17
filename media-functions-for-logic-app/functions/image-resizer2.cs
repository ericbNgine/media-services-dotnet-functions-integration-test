using System;
using System.IO;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using System.Drawing;

using System.Collections.Generic;
using ImageResizer;

namespace media_functions_for_logic_app.functions
{
    public static class image_resizer2
    {
        //private static string logicAppUri = @"https://prod-05.westus.logic.azure.com:443/.........";
        [FunctionName("image_resizer2")]
        public static void Run(
  [BlobTrigger("sample-images/{name}")] Stream image,
  [Blob("sample-images-sm/{name}", FileAccess.Write)] Stream imageSmall,
  [Blob("sample-images-md/{name}", FileAccess.Write)] Stream imageMedium, string name, TraceWriter log)
        {
            log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Size: { image.Length} Bytes");
            ///// 
            using (System.Drawing.Image imageTemp = System.Drawing.Image.FromStream(image))
            {
                int h = imageTemp.Height;
                int w = imageTemp.Width;
                log.Info($"C# Blob trigger function Processed blob\n Name:{name} \n Image Height : {h}  \n Image Width : {w} ");
                int targetedHeight;
                if (h > w)
                {
                    targetedHeight = 720; //portrait
                    log.Info($"C# Blob trigger function Processed blob\n Image is Portrait ");
                }
                else
                {
                    log.Info($"C# Blob trigger function Processed blob\n Image is Landscape ");
                }
            }
            
            log.Info($"C# Blob trigger function Processed  \n Size after getting Width and Height: { image.Length} Bytes");
            ////
            var imageBuilder = ImageResizer.ImageBuilder.Current;
            var size = imageDimensionsTable[ImageSize.Small];

            imageBuilder.Build(image, imageSmall,
                new ResizeSettings(size.Item1, size.Item2, FitMode.Max, null), false);

            image.Position = 0;
            size = imageDimensionsTable[ImageSize.Medium];

            imageBuilder.Build(image, imageMedium,
                new ResizeSettings(size.Item1, size.Item2, FitMode.Max, null), false);
        }

        public enum ImageSize { ExtraSmall, Small, Medium }

        private static Dictionary<ImageSize, (int, int)> imageDimensionsTable = new Dictionary<ImageSize, (int, int)>() {
    { ImageSize.ExtraSmall, (320, 200) },
    { ImageSize.Small,      (640, 400) },
    { ImageSize.Medium,     (800, 600) }
    };
    }

}

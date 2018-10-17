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
        [FunctionName("image_resizer2")]
        public static void Run(
          [BlobTrigger("sample-images/{name}")] Stream image,
          [Blob("sample-images-sm/{name}", FileAccess.Write)] Stream imageSmall,
          [Blob("sample-images-md/{name}", FileAccess.Write)] Stream imageMedium)
        {
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

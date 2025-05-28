using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace RenderMentor.Utility.Helper
{
    public class ImageProcessing
    {
        public static void CropImage(string imagePath, string thumbPath, int width, int height)
        {
            using (Image imageProcessor = Image.Load(imagePath))
            {
                //imageProcessor.Mutate(x => x.Resize(170, 100));
                var processedThumb = ProcessCropping(imageProcessor, new Size(width, height));

                processedThumb.Save(thumbPath);
            }
        }
        public static Image ProcessCropping(Image sourceImage, Size size)
        {
            return sourceImage.Clone(
                ctx => ctx.Resize(
                    new ResizeOptions
                    {
                        Size = size,
                        Mode = ResizeMode.Crop
                    }));
        }
    }
}

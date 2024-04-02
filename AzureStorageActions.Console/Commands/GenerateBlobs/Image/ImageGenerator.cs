using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageActions.Console.Commands.GenerateBlobs.Image
{
    internal static class ImageGenerator
    {
        public static byte[] Generate()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            int width = random.Next(100, 1000);
            int height = random.Next(100, 1000);

            var bitmap = new SKBitmap(width, height);

            using (var canvas = new SKCanvas(bitmap))
            {
                // Fill the image with random colors
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        var color = new SKColor(
                            (byte)random.Next(256), // Red
                            (byte)random.Next(256), // Green
                            (byte)random.Next(256)  // Blue
                        );
                        canvas.DrawPoint(x, y, new SKPaint { Color = color });
                    }
                }
            }

            return bitmap.Encode(SKEncodedImageFormat.Jpeg,100).ToArray();
        }
    }
}

using ISRMUL.Recognition.KMeansPlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ISRMUL.Utils
{
    class ImageConverter
    {
        public static CachedBitmap ByteToImage(byte[] buffer, int width, int height, PixelFormat format)
        {
            //var stride = ((width * format.BitsPerPixel + 31) / 32) * 4;
            var stride = (width) * 4;
            CachedBitmap image = (CachedBitmap)BitmapImage.Create(width, height, 96d, 96d, format, null, buffer, stride);

            return image;
        }
        public static List<ISRMUL.Recognition.MeanShift.Point> getPointFromImage(BitmapSource img)
        {
            return getPointFromImage(img, 0, 0, img.PixelWidth, img.PixelHeight);
        }
        public static List<ISRMUL.Recognition.MeanShift.Point> getPointFromImage(BitmapSource img, int startX, int startY, int width, int height)
        {
            List<ISRMUL.Recognition.MeanShift.Point> points = new List<Recognition.MeanShift.Point>();

            int endX = startX + width;
            int endY = startY + height;

            int stride = img.PixelWidth * 4;
            int size = img.PixelHeight * stride;
            byte[] pixels = new byte[size];
            img.CopyPixels(pixels, stride, 0);

            for (int y = startY; y < endY; y++)
            {
                for (int x = startX; x < endX; x++)
                {
                    int index = y * stride + 4 * x;
                    byte red = pixels[index];
                    byte green = pixels[index + 1];
                    byte blue = pixels[index + 2];
                    byte alpha = pixels[index + 3];

                    // if (!isBackground(red, green, blue, backThresh))
                    if (alpha != 0)
                        points.Add(new ISRMUL.Recognition.MeanShift.Point(new double[] { x, y }) { R = red, G = green, B = blue });
                }
            }

            return points;
        }

        public static bool isBackground(byte r, byte g, byte b, int backThresh)
        {
            return (r + g + b) > backThresh;
        }

        public static CachedBitmap pointToImage(List<ISRMUL.Recognition.MeanShift.Point> points, int startX, int startY, int width, int height)
        {
            int stride = (width) * 4;
            int size = (height) * stride;
            int endY = startY + height;
            int endX = startX + width;

            byte[] pixels = new byte[size];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + 4 * x;

                    
                        pixels[index] = 255;
                        pixels[index + 1] = 255;
                        pixels[index + 2] = 255;
                        pixels[index + 3] = 0; 
                }
            }

            foreach (ISRMUL.Recognition.MeanShift.Point point in points)
            {
                int index = ((int)point.Original[1]-startY) * stride + 4 * ((int)point.Original[0]-startX);
                pixels[index] = (byte)point.R;
                pixels[index + 1] = (byte)point.G;
                pixels[index + 2] = (byte)point.B;
                pixels[index + 3] = 255;

            }

            return ByteToImage(pixels, endX - startX, endY - startY, PixelFormats.Bgra32);
        }

        public static CachedBitmap cutBackground(BitmapSource image)
        {
            var point = getPointFromImage(image);

            var originalPoint = new HashSet<ISRMUL.Recognition.MeanShift.Point>(point);

            KMeans means = new KMeans(2, originalPoint.Select(x => new Vector(3,new double[] { x.R, x.G, x.B }) { X=(int)x.Original[0],Y=(int)x.Original[1] }).ToList(), new Euclidean());
            means.RGBClustersInitialize();
            means.Proccess(1000);

            double minR = means.Clusters[0].Vectors.Min(x => x.Original[0]);
            double minG = means.Clusters[0].Vectors.Min(x => x.Original[1]);
            double minB = means.Clusters[0].Vectors.Min(x => x.Original[2]);

            double maxR = means.Clusters[0].Vectors.Max(x => x.Original[0]);
            double maxG = means.Clusters[0].Vectors.Max(x => x.Original[1]);
            double maxB = means.Clusters[0].Vectors.Max(x => x.Original[2]);

            var cutted = point.Where(x =>
                (x.R >= minR)&&(x.R <= maxR)&&(x.G >= minG)&&(x.G <= maxG)&&(x.B >= minB)&&(x.B <= maxB)
                ).ToList();

            return pointToImage(cutted, 0, 0, image.PixelWidth, image.PixelHeight);
        }

        public static BitmapSource UniformResizeImage(BitmapSource bitmapSource, double newWidth, double newHeight)
        {
            double dx = newWidth / bitmapSource.PixelWidth;
            double dy = newHeight / bitmapSource.PixelHeight;
            if (bitmapSource.PixelWidth < newWidth)
                dx = 1;
            if (bitmapSource.PixelHeight < newHeight)
                dy = 1;
            var bitmap = new TransformedBitmap(bitmapSource, new ScaleTransform(dx,dy));


            return bitmap;
        }

        public static double[,] bitmapSourceToArray(BitmapSource img)
        {
            int startX = 0;
            int startY = 0;
            int endX = img.PixelWidth;
            int endY = img.PixelHeight;

            int stride = img.PixelWidth * 4;
            int size = img.PixelHeight * stride;
            byte[] pixels = new byte[size];
            img.CopyPixels(pixels, stride, 0);

            double[,] data = new double[endY,endX];

            for (int y = startY; y < endY; y++)
            { 
                for (int x = startX; x < endX; x++)
                {
                    int index = y * stride + 4 * x;
                    byte red = pixels[index];
                    byte green = pixels[index + 1];
                    byte blue = pixels[index + 2];
                    byte alpha = pixels[index + 3];

                    if (alpha!=0)
                    data[y,x] = (255 - (red + green + blue) / 3.0 )/ 255.0;

                }
            }
            return data;
        }

        public static double[,] printToArrayUniform(double[,] source, int top, int left, int width, int height)
        {
            double[,] result = new double[height, width];
            for (int i = 0; i < source.GetLength(0); i++)
            {
                for (int j = 0; j < source.GetLength(1); j++)
                {
                    result[i + top, j + left] = source[i, j];
                }
            }

            return result;
        }
    }
}

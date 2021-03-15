using System;
using System.Drawing.Imaging;

namespace BMP
{
    class Program
    {
        static void Main(string[] args)
        {
            ImageAlgorithms imageAlgorithms = new ImageAlgorithms();
            imageAlgorithms.PrintOriginalImageSize();
            imageAlgorithms.CompressWithStatistics(ImageFormat.Bmp, (long)EncoderValue.CompressionRle);
            imageAlgorithms.CompressWithStatistics(ImageFormat.Tiff, (long)EncoderValue.CompressionLZW);
            imageAlgorithms.CompressWithStatistics(ImageFormat.Jpeg, (long)EncoderValue.CompressionNone);

            imageAlgorithms.GetAllDifferenceBetweenBmpAndJpeg();
            Console.ReadKey();
        }
    }
}

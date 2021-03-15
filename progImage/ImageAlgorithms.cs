using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using ByteSizeLib;

namespace BMP
{
    public class ImageAlgorithms
    {
        private string image { get; set; } = "1";
        private string type { get; set; } = ".bmp";
        private string path { get; set; } = @"C:\Desktop\1";
        private string justPath { get; set; } = @"C:\Desktop\";
        private const int Padding = 23;

        public void PrintOriginalImageSize()
        {
            Console.WriteLine($"\n\tSize of original bmp image: {ByteSize.FromBytes(new FileInfo(path + type).Length)}");
        }

        public void CompressWithStatistics(ImageFormat imageFormat, long compressionAlgorithm)
        {
            string outputType = imageFormat.ToString().ToLower();
            string algorithmName = outputType == "bmp" ? "RLE" : (outputType == "tiff" ? "LZW" : "Standard Encoding");
            const int pad = 23;
            Console.WriteLine($"\n\n\tCompression using {outputType} using {algorithmName}");

            ImageCodecInfo info = ImageCodecInfo.GetImageEncoders().FirstOrDefault(e => e.FormatID == imageFormat.Guid);

            EncoderParameters parameters = new EncoderParameters
            {
                Param = new []
                {
                    new EncoderParameter(Encoder.Quality, compressionAlgorithm) 
                }
            };

            Image image = Image.FromFile(path + type);
            Stopwatch timer = new Stopwatch();
            timer.Start();
            image.Save(path + outputType + $".{outputType}", info, parameters);
            timer.Stop();
            Console.WriteLine("Writing time:".PadLeft(Padding) + $" {timer.ElapsedMilliseconds}");

            Image mainImage = Image.FromFile(path + type);

            timer.Restart();
            image = Image.FromFile(path + type);
            timer.Stop();
            Console.WriteLine("Reading time:".PadLeft(Padding) + $" {timer.Elapsed.Milliseconds}");

            timer.Restart();
            image = new Bitmap(path + type);
            timer.Stop();
            Console.WriteLine("Decoding time:".PadLeft(Padding) + $" {timer.Elapsed.TotalMilliseconds}");

            timer.Restart();
            image = (Bitmap)mainImage;
            timer.Stop();
            Console.WriteLine("Encoding time:".PadLeft(Padding) + $" {timer.Elapsed.TotalMilliseconds}");

            Console.WriteLine("Size after:".PadLeft(Padding) + $" {ByteSize.FromBytes(new FileInfo(path + outputType + $".{outputType}").Length)}");
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void GetDifference(Bitmap first, Bitmap second, string pathToSave, string typeToSave)
        {
            Bitmap diff = new Bitmap(first.Width, first.Height);
            int r = 0, g = 0, b = 0;
            for (int i = 0; i < first.Width; ++i)
            {
                for (int j = 0; j < first.Height; ++j)
                {
                    r = Math.Abs(second.GetPixel(i, j).R - first.GetPixel(i, j).R);
                    g = Math.Abs(second.GetPixel(i, j).G - first.GetPixel(i, j).G);
                    b = Math.Abs(second.GetPixel(i, j).B - first.GetPixel(i, j).B);

                    r = (r > 255) ? 255 : r;
                    g = (g > 255) ? 255 : g;
                    b = (b > 255) ? 255 : b;

                    diff.SetPixel(i, j, Color.FromArgb(byte.MaxValue, r, g, b));
                }
            }

            diff.Save(pathToSave + "Difference" + typeToSave, GetEncoder(ImageFormat.Bmp), null);
        }

        private BigInteger GetDifferenceR(Bitmap first, Bitmap second, string pathToSave, string typeToSave)
        {
            Bitmap diff = new Bitmap(first.Width, first.Height);
            BigInteger difference = 0;
            int r = 0;
            for (int i = 0; i < first.Width; ++i)
            {
                for (int j = 0; j < first.Height; ++j)
                {
                    r = Math.Abs(second.GetPixel(i, j).R - first.GetPixel(i, j).R);
                    r = (r > 255) ? 255 : r;
                    difference += r;

                    diff.SetPixel(i, j, Color.FromArgb(byte.MaxValue, r, 0, 0));
                }
            }

            diff.Save(pathToSave + "DifferenceR" + typeToSave, GetEncoder(ImageFormat.Bmp), null);
            return difference;
        }

        private BigInteger GetDifferenceG(Bitmap first, Bitmap second, string pathToSave, string typeToSave)
        {
            Bitmap diff = new Bitmap(first.Width, first.Height);
            BigInteger difference = 0;
            int g = 0;
            for (int i = 0; i < first.Width; ++i)
            {
                for (int j = 0; j < first.Height; ++j)
                {
                    g = Math.Abs(second.GetPixel(i, j).G - first.GetPixel(i, j).G);
                    g = (g > 255) ? 255 : g;
                    difference += g;

                    diff.SetPixel(i, j, Color.FromArgb(byte.MaxValue, 0, g, 0));
                }
            }

            diff.Save(pathToSave + "DifferenceG" + typeToSave, GetEncoder(ImageFormat.Bmp), null);
            return difference;
        }

        private BigInteger GetDifferenceB(Bitmap first, Bitmap second, string pathToSave, string typeToSave)
        {
            Bitmap diff = new Bitmap(first.Width, first.Height);
            BigInteger difference = 0;
            int b = 0;
            for (int i = 0; i < first.Width; ++i)
            {
                for (int j = 0; j < first.Height; ++j)
                {
                    b = Math.Abs(second.GetPixel(i, j).B - first.GetPixel(i, j).B);
                    b = (b > 255) ? 255 : b;
                    difference += b;

                    diff.SetPixel(i, j, Color.FromArgb(byte.MaxValue, 0, 0, b));
                }
            }

            diff.Save(pathToSave + "DifferenceB" + typeToSave, GetEncoder(ImageFormat.Bmp), null);
            return difference;
        }

        public void GetAllDifference()
        {
            Task[] tasks = new Task[]{
                //new Task(() => GetDifference(new Bitmap(justPath + image + ".bmp"), new Bitmap(justPath + image + "bmp.bmp"), justPath + image + "bmpbmp", ".bmp")),
                new Task(() => GetDifference(new Bitmap(justPath + image + "bmp.bmp"), new Bitmap(justPath + image + "jpeg.jpeg"), justPath + image + "bmpjpeg", ".bmp")),
                //new Task(() => GetDifference(new Bitmap(justPath + image + "bmp.bmp"), new Bitmap(justPath + image + "tiff.tiff"), justPath + image + "bmptiff", ".bmp")),
                //new Task(() => GetDifference(new Bitmap(justPath + image + "jpeg.jpeg"), new Bitmap(justPath + image + "tiff.tiff"), justPath + image + "jpegtiff", ".bmp"))
            };

            foreach (var task in tasks)
                task.Start();
            Task.WaitAll(tasks);

            Task[] tasksR = new Task[]{
                //new Task(() => GetDifferenceR(new Bitmap(justPath + image + ".bmp"), new Bitmap(justPath + image + "bmp.bmp"), justPath + image + "bmpbmp", ".bmp")),
                new Task(() => GetDifferenceR(new Bitmap(justPath + image + "bmp.bmp"), new Bitmap(justPath + image + "jpeg.jpeg"), justPath + image + "bmpjpeg", ".bmp")),
                //new Task(() => GetDifferenceR(new Bitmap(justPath + image + "bmp.bmp"), new Bitmap(justPath + image + "tiff.tiff"), justPath + image + "bmptiff", ".bmp")),
                //new Task(() => GetDifferenceR(new Bitmap(justPath + image + "jpeg.jpeg"), new Bitmap(justPath + image + "tiff.tiff"), justPath + image + "jpegtiff", ".bmp"))
            };

            foreach (var task in tasksR)
                task.Start();
            Task.WaitAll(tasksR);

            Task[] tasksG = new Task[]{
                //new Task(() => GetDifferenceG(new Bitmap(justPath + image + ".bmp"), new Bitmap(justPath + image + "bmp.bmp"), justPath + image + "bmpbmp", ".bmp")),
                new Task(() => GetDifferenceG(new Bitmap(justPath + image + "bmp.bmp"), new Bitmap(justPath + image + "jpeg.jpeg"), justPath + image + "bmpjpeg", ".bmp")),
                //new Task(() => GetDifferenceG(new Bitmap(justPath + image + "bmp.bmp"), new Bitmap(justPath + image + "tiff.tiff"), justPath + image + "bmptiff", ".bmp")),
                //new Task(() => GetDifferenceG(new Bitmap(justPath + image + "jpeg.jpeg"), new Bitmap(justPath + image + "tiff.tiff"), justPath + image + "jpegtiff", ".bmp"))
            };

            foreach (var task in tasksG)
                task.Start();
            Task.WaitAll(tasksG);

            Task[] tasksB = new Task[]{
                //new Task(() => GetDifferenceB(new Bitmap(justPath + image + "bmp.bmp"), new Bitmap(justPath + image + "bmp.bmp"), justPath + image + "bmpbmp", ".bmp")),
                new Task(() => GetDifferenceB(new Bitmap(justPath + image + "bmp.bmp"), new Bitmap(justPath + image + "jpeg.jpeg"), justPath + image + "bmpjpeg", ".bmp")),
                //new Task(() => GetDifferenceB(new Bitmap(justPath + image + "bmp.bmp"), new Bitmap(justPath + image + "tiff.tiff"), justPath + image + "bmptiff", ".bmp")),
                //new Task(() => GetDifferenceB(new Bitmap(justPath + image + "jpeg.jpeg"), new Bitmap(justPath + image + "tiff.tiff"), justPath + image + "jpegtiff", ".bmp"))
            };

            foreach (var task in tasksB)
                task.Start();

            Task.WaitAll(tasksB);
        }

        public void GetAllDifferenceBetweenBmpAndJpeg()
        {
            GetDifference(new Bitmap(justPath + image + "bmp.bmp"), new Bitmap(justPath + image + "jpeg.jpeg"),
                justPath + image + "bmpjpeg", ".bmp");
            BigInteger diffByR = GetDifferenceR(new Bitmap(justPath + image + "bmp.bmp"), new Bitmap(justPath + image + "jpeg.jpeg"),
                justPath + image + "bmpjpeg", ".bmp");
            BigInteger diffByG = GetDifferenceG(new Bitmap(justPath + image + "bmp.bmp"), new Bitmap(justPath + image + "jpeg.jpeg"),
                justPath + image + "bmpjpeg", ".bmp");
            BigInteger diffByB = GetDifferenceB(new Bitmap(justPath + image + "bmp.bmp"), new Bitmap(justPath + image + "jpeg.jpeg"),
                justPath + image + "bmpjpeg", ".bmp");

            Console.WriteLine("\n\n\tDifference between BMP and JPEG");
            Console.WriteLine($"by R:".PadLeft(Padding) + $" {diffByR}");
            Console.WriteLine($"by G:".PadLeft(Padding) + $" {diffByG}");
            Console.WriteLine($"by B:".PadLeft(Padding) + $" {diffByB}");
        }
    }
}

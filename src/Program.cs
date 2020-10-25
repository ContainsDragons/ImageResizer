using System;
using System.IO;
using System.Configuration;
using System.Linq;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace ImageResizer
{
    class Program
    {

        private static string[] images;
        private static int newWidth;
        private static int newHeight;
        private static string outputDir;
        private static string directory;
        //private static string imageType;
        private static int qualityType;

        private static bool useDefaults = Convert.ToBoolean(ConfigurationManager.AppSettings["useDefaults"]);
        private static int defWidth = Convert.ToInt32(ConfigurationManager.AppSettings["width"]);
        private static int defHeight = Convert.ToInt32(ConfigurationManager.AppSettings["height"]);
        private static bool searchAllDirectories = Convert.ToBoolean(ConfigurationManager.AppSettings["allDirs"]);
        //private static string defFileType = ConfigurationManager.AppSettings["imageType"];
        private static int defQuality = Convert.ToInt32(ConfigurationManager.AppSettings["quality"]);

        private static string configPath = $"{Directory.GetCurrentDirectory()}\\ImageResizer.exe.config"; 

        static void Main(string[] args)
        {
            StartUp();
            Console.WindowHeight = 45;
            Console.WindowWidth = 143;
            Console.Title = "Image Resizer...";
        }

        private static void StartUp()
        {

            if (!File.Exists(configPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("CONFIG FILE MISSING!!!");
                Console.Title = "Oops...";
                Console.ReadKey();
                return;
            }

            if (!useDefaults)
            {
                Console.Title = "Image Resizer, not using defaults!";
                Console.WriteLine("Input An Input Directory...");
                directory = Console.ReadLine();
                Console.WriteLine("Input An Output Directory...");
                outputDir = Console.ReadLine();
                Console.WriteLine("Input A New Width...");
                newWidth = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("Input A New Height...");
                newHeight = Convert.ToInt32(Console.ReadLine());
                //.WriteLine("Image Type...");
                //imageType = Console.ReadLine();
                Console.WriteLine("Input a number for quality 0: Default, 1: HighSpeed, 2: High Quality, 3: Gamma Corrected, 4: Assume Linear. Recommened: 2");
                qualityType = Convert.ToInt32(Console.ReadLine());
                images = GetFiles();
                Resize();
                Console.Read();
            }
            else
            {
                Console.Title = "Image Resizer, using defaults!";
                Console.WriteLine($"Using defaults! width: {defWidth} height: {defHeight} quality: {defQuality}");
                newWidth = defWidth;
                newHeight = defHeight;
                //imageType = defFileType;
                qualityType = defQuality;
                Console.WriteLine("Input An Input Directory...");
                directory = Console.ReadLine();
                Console.WriteLine("Input An Output Directory...");
                outputDir = Console.ReadLine();
                images = GetFiles();
                Resize();
                Console.Read();
            }
        }

        private static string[] GetFiles()
        {
            Console.Title = "Image Resizer, getting files...";
            string[] _images;
            if (searchAllDirectories)
            {
                
                _images = Directory.GetFiles(directory, $"*.png", SearchOption.AllDirectories);
                return _images;
            }
            else
            {
                _images = Directory.GetFiles(directory, $"*.png", SearchOption.TopDirectoryOnly);
                return _images;
            }
        }
        
        private static void Resize()
        {
            Console.Title = "Image Resizer, resizing!";
            foreach (var image in images)
            {
                Image _image = Image.FromFile(image);
                ResiseImage(_image, newWidth, newHeight, image);
                Console.WriteLine($"Resized resized_{image.Split('\\').Last()} and saved it as {outputDir}{image.Split('\\').Last()}");
            }

        }

        /// 
        /// 
        /// THE CODE BELOW WAS CREATED BY https://stackoverflow.com/a/24199315
        /// 
        /// SLIGHTLY TWEAKED BY ME
        /// 
        private static void ResiseImage(Image _image, int _width, int _height, string path)
        {
            // https://stackoverflow.com/a/24199315's code:
            var destRect = new Rectangle(0, 0, _width, _height);
            var destImage = new Bitmap(_width, _height);

            destImage.SetResolution(_image.HorizontalResolution, _image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = (CompositingQuality)qualityType; //a bit me
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(_image, destRect, 0, 0, _image.Width, _image.Height, GraphicsUnit.Pixel, wrapMode);
                }

            }

            //my code:
            if (!Directory.Exists(outputDir))
            {
                Directory.CreateDirectory(outputDir);
            }
            destImage.Save($"{outputDir}\\resized_{path.Split('\\').Last()}", ImageFormat.Png);

        }

    }
}

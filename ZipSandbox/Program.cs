using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ZipLibrary;

namespace ZipSandbox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the ZipSandbox");
            const string destinationFile = @"C:\Users\QVP6WXL\Documents\Test\ZipSandboxTest.zip";
            ExecuteTextFilesExperiment(destinationFile);
            ExecuteImagesExperiment(destinationFile);
        }

        private static void ExecuteTextFilesExperiment(string destinationFile)
        {
            var folder = @"C:\TEMP\ZipSandboxTest";
            Console.WriteLine($"This experiment zips text files in folder {folder} to archive {destinationFile}");
            Console.WriteLine("The archive preserves the original folder structure");
            Console.WriteLine("Let us start with creating some files");
            CreateDirectory(folder);
            var secondFolder = Path.Combine(folder, "second");
            CreateDirectory(secondFolder);
            var thirdFolder = Path.Combine(folder, "third");
            CreateDirectory(thirdFolder);
            CreateTextFile(Path.Combine(folder, "first.txt"), "This is the text in the first file");
            CreateTextFile(Path.Combine(secondFolder, "second.txt"), "This is the text in the second file");
            CreateTextFile(Path.Combine(thirdFolder, "third.txt"), "This is the text in the third file");
            CreateTextFile(Path.Combine(folder, "fourth.txt"), "This is the text in the fourth file");
            var paths = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
            var files = paths.Select(file => new FileInfo(file)).ToList();
            var zipHelper = new ZipHelper();
            zipHelper.ZipFiles(files, folder, destinationFile);
            Console.WriteLine($"Zip result in {destinationFile}");
            Console.ReadLine();
        }

        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        private static void CreateTextFile(string path, string content)
        {
            using (var stream = new FileStream(path, FileMode.Create))
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    streamWriter.WriteLine(content);
                }
            }
        }

        private static void ExecuteImagesExperiment(string destinationFile)
        {
            const string sourceFolder = @"C:\TEMP\ZipSandboxImages";
            var images = ReadImagesFromFolder(sourceFolder);
            var byteArrays = ConvertImagesToByteArray(images);
            var pdfByteArrays = byteArrays
                .Keys
                .ToDictionary(path => path, path => ConvertImageToPdf(byteArrays[path]));
            var zipHelper = new ZipHelper();
            zipHelper.ZipByteArrays(pdfByteArrays, destinationFile, ZipArchiveMode.Update);
        }

        private static Dictionary<string, Image> ReadImagesFromFolder(string folder)
        {
            var result = new Dictionary<string, Image>();
            string[] allowedExtensions = {".bmp", ".jpg", ".png"};
            var paths = Directory
                .GetFiles(folder, "*.*", SearchOption.AllDirectories)
                .Where(file => allowedExtensions.Any(file.ToLower().EndsWith));
            foreach (var path in paths)
            {
                var fileInfo = new FileInfo(path);
                var image = Image.FromFile(path);
                result.Add(fileInfo.Name, image);
            }
            return result;
        }

        private static Dictionary<string, byte[]> ConvertImagesToByteArray(Dictionary<string, Image> images)
        {
            var result = new Dictionary<string, byte[]>();
            foreach (var image in images)
            {
                using (var stream = new MemoryStream())
                {
                    image.Value.Save(stream, image.Value.RawFormat);
                    result.Add(image.Key, stream.ToArray());
                }
            }
            return result;
        }

        private static byte[] ConvertImageToPdf(byte[] byteArray)
        {
            using (var pdfStream = new MemoryStream())
            {
                using (var stream = new MemoryStream(byteArray))
                {
                    using (var document = new PdfDocument())
                    {
                        var page = document.AddPage();
                        using (var graphics = XGraphics.FromPdfPage(page))
                        {
                            using (var image = XImage.FromStream(stream))
                            {
                                page.Width = image.PointWidth;
                                page.Height = image.PointHeight;
                                graphics.DrawImage(image, 0, 0);
                                document.Save(pdfStream);
                            }
                        }
                        document.Close();
                    }
                }
                return pdfStream.ToArray();
            }
        }
    }
}

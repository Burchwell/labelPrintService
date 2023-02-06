using LabelService3.Models;
using Spire.Pdf;
using System;
using System.IO;
using System.Net;

namespace LabelService3
{
    static class Locals
    {
        public static string FileRoot = "output";
    }

    class PDF
    {
        public static void SaveLabelFromUri(string Uri, string FileName = "")
        {
            PdfDocument pdf = new PdfDocument();
            string path = _Download(Uri, "tmp_" + FileName);
            pdf.LoadFromFile(path);
            _Save(pdf, FileName);
        }
        // This method sends the PDF to the printer
        public static String PrintLabelFromUri(string Uri, bool saveToFile = false)
        {
            PdfDocument pdf = new PdfDocument();
            string path = _Download(Uri);
            pdf.LoadFromFile(path);
            _Print(pdf);
            Byte[] bytes = File.ReadAllBytes(path);
            String file = Convert.ToBase64String(bytes);
            return file;
        }

        public static void SaveLabelBase64(string base64, string FileName)
        {
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(base64));
            PdfDocument pdf = new PdfDocument();

            pdf.LoadFromStream(stream);
            _Save(pdf);
        }
        // This method sends the PDF to the printer
        public static void PrintLabelBase64Pdf(string base64, bool saveToFile = false)
        {
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(base64));
            PdfDocument pdf = new PdfDocument();

            pdf.LoadFromStream(stream);

            _Print(pdf);
        }

        private static MemoryStream Base64ToStream(string base64)
        {
            MemoryStream stream = new MemoryStream(Convert.FromBase64String(base64));
            PdfDocument pdf = new PdfDocument();
            return stream;
        }

        private static void _Print(PdfDocument pdf)
        {
            pdf.PrintSettings.PrinterName = Globals.Printer;
            pdf.Print();

        }

        private static void _Save(PdfDocument pdf, string FileName = "")
        {
            string fn = !FileName.Contains("") ? FileName : String.Format("skar-audio-label-{0}", DateTime.Now.ToString("yyyy-MM-ddTHHmmss")) + ".pdf";
            Directory.CreateDirectory(Locals.FileRoot);
            pdf.SaveToFile(String.Format("{0}\\{1}", Locals.FileRoot, fn));
        }

        private static string _Download(string Source, string FileName = "")
        {
            Directory.CreateDirectory("temp");
            using (WebClient client = new WebClient())
            {
                Directory.CreateDirectory(Locals.FileRoot);
                byte[] arr = client.DownloadData(Source);
                string fn = !FileName.Contains("") ? FileName : String.Format("skar-audio-label-{0}", DateTime.Now.ToString("yyyy-MM-ddTHHmmss")) + ".pdf";
                string path = String.Format("temp\\{0}", fn);
                File.WriteAllBytes(path, arr);
                return path;
            }
        }
    }
}

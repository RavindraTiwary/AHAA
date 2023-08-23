using Azure.Storage.Blobs;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace ResumeMetadataGenerator.utils
{
    internal static class DocumentHelper
    {
        public static async Task<(byte[], Uri)> GetBlobFile(string fileName)
        {
            string blobConnString = Environment.GetEnvironmentVariable("ConnectionStrings:BlobConnectionString");
            BlobServiceClient blobServiceClient = new BlobServiceClient(@blobConnString);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("resumes");

            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            var result = await blobClient.DownloadContentAsync();
            Uri resumeUri = blobClient.GenerateSasUri(Azure.Storage.Sas.BlobSasPermissions.Read, DateTimeOffset.Now.AddDays(7));
            return (result.Value.Content.ToArray(), resumeUri);
        }

        /* 
        public static string ToText(this byte[] bytes)
        {
            MemoryStream memory = new MemoryStream(bytes);
            BinaryReader BRreader = new BinaryReader(memory);
            StringBuilder text = new StringBuilder();

            PdfReader iTextReader = new PdfReader(memory);
            PdfDocument pdfDoc = new PdfDocument(iTextReader);

            int numberofpages = pdfDoc.GetNumberOfPages();
            for (int page = 1; page <= numberofpages; page++)
            {
                iText.Kernel.Pdf.Canvas.Parser.Listener.ITextExtractionStrategy strategy = new iText.Kernel.Pdf.Canvas.Parser.Listener.SimpleTextExtractionStrategy();
                string currentText = PdfTextExtractor.GetTextFromPage(pdfDoc.GetPage(page), strategy);
                currentText = Encoding.UTF8.GetString(ASCIIEncoding.Convert(
                    Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(currentText)));
                text.Append(currentText);
            }

            return EscapeData(text.ToString());
        }
        */

        public static string ToText(this byte[] bytes)
        {
            StringBuilder text = new StringBuilder();
            using (UglyToad.PdfPig.PdfDocument doc = UglyToad.PdfPig.PdfDocument.Open(bytes))
            {
                foreach(var page in doc.GetPages())
                {
                    var content = ContentOrderTextExtractor.GetText(page);
                    text.Append(content);
                }
                
                return EscapeData(text.ToString());
            }
                
        }

        public static string ExtractEmail(this string text)
        {
            const string emailPattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            Regex rx = new Regex(emailPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            MatchCollection matches = rx.Matches(text);
            int noOfMatches = matches.Count;
            string result = string.Empty;
            foreach (Match match in matches)
            {
                result = match.Value.ToString();
                break;
            }

            return result;
        }

        private static string EscapeData(string info)
        {
            if (string.IsNullOrEmpty(info)) return info;
            return info.Replace(Environment.NewLine, " ");
        }
    }
}

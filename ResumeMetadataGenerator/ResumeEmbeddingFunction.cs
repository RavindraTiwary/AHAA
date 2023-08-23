using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using Azure.Storage.Blobs;
using iText.IO.Source;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.StyledXmlParser.Css.Resolve.Shorthand.Impl;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Azure.KeyVault.Core;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using ResumeMetadataGenerator.utils;

namespace ResumeMetadataGenerator
{
    public class ResumeEmbeddingFunction
    {
        [FunctionName("ResumeEmbedding")]
        public async Task Run([BlobTrigger("resumes/{name}", Connection = "BlobConnectionString")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation("Getting PDF");
            var pdfInfo = await DocumentHelper.GetBlobFile(name);
            log.LogInformation("Parsing PDF to text");
            string content = pdfInfo.Item1.ToText();
            string email = content.ExtractEmail();
            log.LogInformation("Creating index and storing the attachment.");
            var openAIClient = OpenAPIHelper.GetOpenAIClient();
            List<SearchDocument> searchDocs = new List<SearchDocument>();
            Dictionary<string, object> resumeMetaData = new Dictionary<string, object>
            {
                { "id", DateTime.Now.Ticks.ToString() },

                { "fileName", name },
                { "name", "dummy" }, //TODO get Interviewee name
                { "resume", pdfInfo.Item2.ToString() },
                { "email", email },
                { "category", "Software" },
                { "contentVector", (await VectorEmbeddingHelper.GenerateEmbeddings(content, openAIClient)).ToArray() }
            };
            searchDocs.Add(new SearchDocument(resumeMetaData));
            var searchClient = CognitiveSearchHelper.GetSearchClient(true);
            var indexResult = await searchClient.IndexDocumentsAsync(IndexDocumentsBatch.Upload(searchDocs));
            if (indexResult.Value.Results == null) throw new Exception("Error while indexing");
            log.LogInformation("Resume indexed successfully.");
        }

    }
}

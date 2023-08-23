using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ResumeMetadataGenerator.utils;
using Azure.Search.Documents;
using System.Linq;
using Azure.Search.Documents.Models;
using System.Collections.Generic;

namespace ResumeMetadataGenerator
{
    public static class ResumeSearchFunction
    {
        [FunctionName("ResumeSearchFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string query = req.Query["query"];
            string resultCount = req.Query["count"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            query = query ?? data?.query;
            resultCount = resultCount ?? data?.count;

            var queryEmbeddings = await VectorEmbeddingHelper.GenerateEmbeddings(query, OpenAPIHelper.GetOpenAIClient());

            // Perform the vector similarity search  
            var searchOptions = new SearchOptions
            {
                Vectors = { new() { Value = queryEmbeddings.ToArray(), KNearestNeighborsCount = 3, Fields = { "contentVector" } } },
                Size = Int16.Parse(resultCount),
                Select = { "name", "resume", "email" },
            };

            SearchResults<SearchDocument> searchResults = await CognitiveSearchHelper.GetSearchClient(false).SearchAsync<SearchDocument>(null, searchOptions);
            List<(string, string, double?)> response = new List<(string, string, double?)>();
            await foreach(var result in searchResults.GetResultsAsync())
            {
                response.Add((result.Document["name"].ToString(), result.Document["resume"].ToString(), result.Score));
            }
            return new OkObjectResult(response);
        }
    }
}

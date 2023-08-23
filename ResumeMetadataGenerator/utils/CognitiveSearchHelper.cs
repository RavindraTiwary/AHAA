using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents;
using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeMetadataGenerator.utils
{
    internal static class CognitiveSearchHelper
    {
        public static SearchClient GetSearchClient(bool createIndex)
        {
            var serviceEndpoint = Environment.GetEnvironmentVariable("AZURE_SEARCH_SERVICE_ENDPOINT") ?? string.Empty;
            var indexName = Environment.GetEnvironmentVariable("AZURE_SEARCH_INDEX_NAME") ?? string.Empty;
            var key = Environment.GetEnvironmentVariable("AZURE_SEARCH_ADMIN_KEY") ?? string.Empty;

            // Initialize Azure Cognitive Search clients  
            var searchCredential = new AzureKeyCredential(key);
            var indexClient = new SearchIndexClient(new Uri(serviceEndpoint), searchCredential);
            var searchClient = indexClient.GetSearchClient(indexName);
            if (createIndex)
            {
                indexClient.CreateOrUpdateIndex(GetIndex(indexName));
            }
            return searchClient;
        }

        private static SearchIndex GetIndex(string name)
        {
            string vectorSearchConfigName = Environment.GetEnvironmentVariable("VECTOR_SEARCH_CONFIG");
            int modelDimension = Convert.ToInt16(Environment.GetEnvironmentVariable("MODEL_DIMENSION"));
            string semanticConfig = Environment.GetEnvironmentVariable("SEMANTIC_CONFIG");

            SearchIndex searchIndex = new(name)
            {
                VectorSearch = new()
                {
                    AlgorithmConfigurations =
                {
                    new HnswVectorSearchAlgorithmConfiguration(vectorSearchConfigName)
                }
                },
                SemanticSettings = new()
                {

                    Configurations =
                    {
                       new SemanticConfiguration(semanticConfig, new()
                       {
                           TitleField = new(){ FieldName = "fileName" },
                           ContentFields =
                           {
                               new() { FieldName = "resume" }
                           },
                           KeywordFields =
                           {
                               new() { FieldName = "category" }
                           }

                       })

                },
                },
                Fields =
            {
                new SimpleField("id", SearchFieldDataType.String) { IsKey = true, IsFilterable = true, IsSortable = true, IsFacetable = true },
                new SearchableField("fileName") { IsFilterable = true, IsSortable = true },
                new SearchableField("resume") { IsFacetable = true },
                new SearchableField("email") { IsFacetable = true },
                new SearchableField("name") { IsFacetable = true },
                new SearchField("fileVector", SearchFieldDataType.Collection(SearchFieldDataType.Single))
                {
                    IsSearchable = true,
                    VectorSearchDimensions = modelDimension,
                    VectorSearchConfiguration = vectorSearchConfigName
                },
                new SearchField("contentVector", SearchFieldDataType.Collection(SearchFieldDataType.Single))
                {
                    IsSearchable = true,
                    VectorSearchDimensions = modelDimension,
                    VectorSearchConfiguration = vectorSearchConfigName
                },
                new SearchableField("category") { IsFilterable = true, IsSortable = true, IsFacetable = true }
            }
            };


            return searchIndex;
        }
    }
}

using Azure.AI.OpenAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumeMetadataGenerator.utils
{
    internal static class VectorEmbeddingHelper
    {
        public static async Task<IReadOnlyList<float>> GenerateEmbeddings(string text, OpenAIClient openAIClient)
        {
            string deployedModel = Environment.GetEnvironmentVariable("AZURE_OPENAI_EMBEDDING_DEPLOYED_MODEL") ?? string.Empty;
            var response = await openAIClient.GetEmbeddingsAsync(deployedModel, new EmbeddingsOptions(text));
            return response.Value.Data[0].Embedding;
        }
    }
}

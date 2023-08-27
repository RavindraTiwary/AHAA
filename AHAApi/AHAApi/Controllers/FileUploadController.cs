using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs.Specialized;
using Azure;
using SharpCompress.Common;
using System.Text;
using System.IO;
namespace AHAApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly IConfiguration _configuration;
        public FileUploadController(IConfiguration configuration)
        {
            _configuration = configuration;
            _blobServiceClient = new BlobServiceClient(configuration.GetConnectionString("AzureBlobStorage"));
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] IDictionary<string, string> metadata)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var containerName = string.Empty;
                    var type = metadata.ContainsKey("type") ? metadata["type"] : "resume";
                    if (type == "resume")
                    {
                        containerName = _configuration.GetSection("AzureBlobStorageSettings")["ResumeContainerName"];
                    }
                    else if (type == "job")
                    {
                        containerName = _configuration.GetSection("AzureBlobStorageSettings")["JobPostingsContainerName"];
                    }
                    var blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                    BlobClient blobClient = containerClient.GetBlobClient(blobName);
                    metadata.Remove("type");
                    using (Stream stream = file.OpenReadStream())
                    {

                        BlobUploadOptions options = new BlobUploadOptions
                        {
                            Metadata = metadata, // Set metadata for this specific blob
                        };
                        //Metadata = metadata, // Set metadata for this specific blob

                        // Upload the blob with metadata specific to this blob
                        await blobClient.UploadAsync(stream, options);
                    }

                    var blobUrl = $"{containerClient.Uri}/{blobName}";

                    return Ok(new { UploadedUrl = blobUrl });
                }

                return BadRequest("No file provided for upload.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("GetJobs")]
        public async Task<IActionResult> GetJobList()
        {
            try
            {
                List<UrlWithMetadata> data = new();
                var listOfJobs = new List<string>();
                var containerName = string.Empty;
                containerName = _configuration.GetSection("AzureBlobStorageSettings")["ResumeContainerName"];
                var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    listOfJobs.Add($"{containerClient.Uri}/{blobItem.Name}");
                }

                foreach (var jobUrl in listOfJobs)
                {

                    // Create a BlobClient from the blob URL
                    BlobClient blobClient = new BlobClient(new Uri(jobUrl));

                    // Retrieve the blob's properties, which include metadata
                    BlobProperties blobProperties = await blobClient.GetPropertiesAsync();

                    // Get the metadata from the blob properties
                    IDictionary<string, string> metadata = blobProperties.Metadata;
                    data.Add(new UrlWithMetadata() { bloburl = jobUrl, metadata = metadata });
                }

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading file: {ex.Message}");
            }
        }
    }

}
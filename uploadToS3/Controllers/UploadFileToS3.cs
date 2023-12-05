using Amazon.S3;
using Microsoft.AspNetCore.Mvc;
using uploadToS3.Controllers;

namespace UploadToS3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UploadFileToS3 : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;

        public UploadFileToS3(ILogger<WeatherForecastController> logger,
            IAmazonS3 s3Client
            )
        {
            _s3Client = s3Client;
        }
        [HttpPost(template: "upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest(error: "Invalid File");
            }

            string bucketName = "itrackfiles";
            string fileKey = "uploads/" + file.FileName;

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);

                var putRequest = new Amazon.S3.Model.PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = fileKey,
                    InputStream = memoryStream
                };

                var response = await _s3Client.PutObjectAsync(putRequest);

                if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Ok(value: "File uploaded to S3 successfully");
                }
                else
                {
                    return StatusCode(statusCode: 500, value: "Error uploading file to S3");
                }
            }
        }
    }
}

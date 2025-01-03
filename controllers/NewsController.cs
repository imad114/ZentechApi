using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zentech.Models;
using Zentech.Services;
using ZentechAPI.Dto;
using ZentechAPI.Models;

namespace Zentech.Controllers
{
    /// <summary>
    /// Controller for managing news.
    /// Allows retrieving, adding, updating, and deleting news.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly NewsService _newsService;

        /// <summary>
        /// Constructor to inject the news management service.
        /// </summary>
        /// <param name="newsService">Service for managing news.</param>
        public NewsController(NewsService newsService)
        {
            _newsService = newsService;
        }
        /// <summary>
        /// Retrieve all news.
        /// Accessible only to users with the "Admin" role.
        /// </summary>
        /// <returns>List of all news.</returns>

        [HttpGet]
        public IActionResult GetAllNews()
        {
            var news = _newsService.GetAllNews();
            return Ok(news);
        }

        /// <summary>
        /// Retrieve a specific news item by ID.
        /// Accessible only to users with the "Admin" role.
        /// </summary>
        /// <param name="id">The ID of the news item to retrieve.</param>
        /// <returns>The requested news or NotFound if not found.</returns>

        [HttpGet("{id}")]
        public IActionResult GetNewsById(int id)
        {
            var news = _newsService.GetNewsById(id);
            if (news == null)
            {
                return NotFound(new { Message = "News not found." });
            }
            return Ok(news);
        }

        /// <summary>
        /// Add a new news item.
        /// Accessible only to users with the "Admin" role.
        /// </summary>
        /// <param name="news">The news data to add.</param>
        /// <returns>The created news item.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AddNews([FromBody] NewsDto news)
        {
            if (news == null || string.IsNullOrEmpty(news.Title) || string.IsNullOrEmpty(news.Content))
            {
                return BadRequest(new { Message = "Invalid news data. Title and content are required." });
            }

            var createdNews = _newsService.AddNews(news);
            return CreatedAtAction(nameof(GetNewsById), new { id = createdNews.NewsID }, createdNews);
        }

        /// <summary>
        /// Update an existing news item.
        /// Accessible only to users with the "Admin" role.
        /// </summary>
        /// <param name="id">The ID of the news item to update.</param>
        /// <param name="news">The updated news data.</param>
        /// <returns>The updated news item.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateNews(int id, [FromBody] NewsDto news)
        {
            if (news == null || id != news.NewsID)
            {
                return BadRequest(new { Message = "Invalid news data or ID mismatch." });
            }

            var updated = _newsService.UpdateNews(news);
            if (!updated)
            {
                return NotFound(new { Message = "News not found." });
            }

            return Ok(news);
        }

        /// <summary>
        /// Delete a news item.
        /// Accessible only to users with the "Admin" role.
        /// </summary>
        /// <param name="id">The ID of the news item to delete.</param>
        /// <returns>NoContent response if the deletion is successful.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteNews(int id)
        {
            _newsService.DeleteNews(id);
            return NoContent();
        }



        [HttpPost("{newsId}/upload-photo1")]
        [SwaggerOperation(Summary = "Upload a photo for a news", Description = "Allows uploading a photo for a specific news.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Photo uploaded successfully", typeof(object))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid file upload", typeof(object))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while uploading the photo", typeof(object))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddPhotoToNews(int newsId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { Message = "Invalid file upload." });
                }

                // Validation du type de fichier
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { Message = "Invalid file type. Only JPG and PNG files are allowed." });
                }

                // Validation de la taille du fichier (10 Mo max)
                if (file.Length > 10 * 1024 * 1024)
                {
                    return BadRequest(new { Message = "File size exceeds the maximum limit of 10MB." });
                }

                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/News");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileName = $"{Guid.NewGuid()}_{file.FileName}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var photoUrl = $"/uploads/{fileName}";
                _newsService.AddPhotoToNews(newsId, photoUrl);

                return CreatedAtAction("AddPhotoToNews", new { newsId, photoUrl }, new { Message = "Photo uploaded successfully.", Url = photoUrl });
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logger here)
                return StatusCode(500, new { Message = "An error occurred while uploading the photo.", Error = ex.Message });
            }
        }





        /// <summary>
        /// Delete a photo from a news item.
        /// Accessible only to users with the "Admin" role.
        /// </summary>
        /// <param name="photoUrl">The URL of the photo to delete.</param>
        /// <returns>OK response if the photo was deleted successfully.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("photos")]
        public IActionResult DeletePhotoFromNews([FromBody] string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
            {
                return BadRequest(new { Message = "Invalid photo URL." });
            }

            _newsService.DeletePhotoFromNews(photoUrl);
            return Ok(new { Message = "Photo deleted successfully." });
        }
    }
}

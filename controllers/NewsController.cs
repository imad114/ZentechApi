using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zentech.Models;
using Zentech.Services;
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        public IActionResult AddNews([FromBody] News news)
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
        public IActionResult UpdateNews(int id, [FromBody] News news)
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

        /// <summary>
        /// Add a photo to a news item.
        /// Accessible only to users with the "Admin" role.
        /// </summary>
        /// <param name="newsId">The ID of the news item.</param>
        /// <param name="photoUrl">The URL of the photo to add.</param>
        /// <returns>OK response if the photo was added successfully.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("{newsId}/photos")]
        public IActionResult AddPhotoToNews(int newsId, [FromBody] string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
            {
                return BadRequest(new { Message = "Invalid photo URL." });
            }

            _newsService.AddPhotoToNews(newsId, photoUrl);
            return Ok(new { Message = "Photo added successfully." });
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

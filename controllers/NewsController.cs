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


        [HttpGet("newsByCategory/{category_id}")]
        public IActionResult GetNewsByCategoryId(int category_id)
        {
            var news = _newsService.GetNewsByCategoryId(category_id);
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
            var createdBy = User.Identity?.Name;
            var createdNews = _newsService.AddNews(news, createdBy);
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
            var updatedBy = User.Identity?.Name;
            var updated = _newsService.UpdateNews(news, updatedBy);
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



        [HttpPost("{newsId}/upload-photoNews")]
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

                var photoUrl = $"/uploads/News/{fileName}";
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

            try
            {
                // Delete the photo from the database
                _newsService.DeletePhotoFromNews(photoUrl);

                // Ensure the photoUrl is a relative path starting with "/uploads/"
                var relativePath = photoUrl.StartsWith("/uploads/News/") ? photoUrl : $"/uploads/News/{Path.GetFileName(photoUrl)}";

                // Get the physical path of the photo
                var photoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                // Check if the file exists and delete it
                if (System.IO.File.Exists(photoPath))
                {
                    System.IO.File.Delete(photoPath);
                }

                return Ok(new { Message = "Photo deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error deleting photo: {ex.Message}" });
            }
        }



        #region Categories methods

        // Get all categories
        /// <summary>
        /// Retrieve all categories.
        /// </summary>
        /// <returns>Returns the complete list of categories.</returns>
        [HttpGet("CategoriesNews")]
        [SwaggerOperation(Summary = "Get all categories", Description = "Returns the complete list of categories.")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _newsService.GetNewsCategoriesAsync();
            return Ok(categories);
        }

        // Add a new category
        /// <summary>
        /// Add a new category.
        /// </summary>
        /// <param name="category">Object containing the details of the category to add.</param>
        /// <returns>The created category with its identifier.</returns>
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Add a category", Description = "Adds a new category to the system.")]
        [HttpPost("Categories")]
        public async Task<IActionResult> AddNewsCategory([FromBody] Other_Category category)
        {
            if (category == null)
            {
                return BadRequest("Category is null.");
            }

            try
            {
                string createdBy = User.Identity?.Name;
                category.CreatedBy = createdBy ?? "null";
                var createdCategory = await _newsService.AddNewsCategoryAsync(category);
                return CreatedAtAction(nameof(GetAllCategories), new { id = createdCategory.CategoryID }, createdCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Update a category
        /// <summary>
        /// Update an existing category.
        /// </summary>
        /// <param name="category">Object containing the updated details of the category.</param>
        /// <returns>The updated category.</returns>
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Update a category", Description = "Updates an existing category in the system.")]
        [HttpPut("Categories")]
        public async Task<IActionResult> UpdateNewsCategory([FromBody] Other_Category category)
        {
            if (category == null)
            {
                return BadRequest("Category is null.");
            }

            try
            {
                string updatedBy = User.Identity?.Name;
                category.UpdatedBy = updatedBy ?? "null";
                var updatedCategory = await _newsService.UpdateNewsCategoryAsync(category);
                if (updatedCategory == null)
                {
                    return NotFound($"Category with ID {category.CategoryID} not found.");
                }
                return Ok(updatedCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // Delete a category
        /// <summary>
        /// Delete a category.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Delete a category", Description = "Deletes an existing category from the system.")]
        [HttpDelete("Categories/{id}")]
        public async Task<IActionResult> DeleteNewsCategory([FromRoute] int id)
        {
            try
            {
                int result = await _newsService.DeleteNewsCategoryAsync(id);
                if (result > 0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound($"Category with ID {id} not found or is referenced by another entity.");
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }
        #endregion

    }
}

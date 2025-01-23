using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Zentech.Services;
using ZentechAPI.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ZentechAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SlidesController : ControllerBase
    {
        private readonly SlidesService _slideService;
        private readonly ILogger<SlidesController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlidesController"/> class.
        /// </summary>
        /// <param name="slideService">The service used for managing slides.</param>
        /// <param name="logger">Logger for debugging and error tracking.</param>
        public SlidesController(SlidesService slideService, ILogger<SlidesController> logger)
        {
            _slideService = slideService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all slides.
        /// </summary>
        /// <returns>A list of all slides.</returns>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Slides retrieved successfully", typeof(Slide[]))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public IActionResult GetAllSlides()
        {
            try
            {
                var slides = _slideService.GetAllSlides();
                return Ok(slides);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving slides.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Retrieves a slide by its ID.
        /// </summary>
        /// <param name="id">The ID of the slide.</param>
        /// <returns>The requested slide.</returns>
        [HttpGet("{id}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Slide retrieved successfully", typeof(Slide))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Slide not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public IActionResult GetSlideById(int id)
        {
            try
            {
                var slide = _slideService.GetSlideById(id);
                if (slide == null)
                {
                    return NotFound("Slide not found.");
                }
                return Ok(slide);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving slide with ID {id}.");
                return StatusCode(500, "Internal server error.");
            }
        }
        // update 16h16 22/01/2025
        /// <summary>
        /// Adds a new slide.
        /// </summary>
        /// <param name="slide">The slide data.</param>
        /// <returns>The created slide.</returns>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status201Created, "Slide created successfully", typeof(Slide))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid slide data")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> AddSlide([FromForm] Slide slide)
        {
            try
            {
                if (slide == null)
                {
                    return BadRequest("Slide data is null.");
                }

                slide.CreatedBy = "admin"; // Replace with dynamic user context
                var slideId = _slideService.AddSlide(slide, slide.CreatedBy);
                slide.SlideID = slideId;

                if (slide.Picture != null && slide.Picture.Length > 0)
                {
                    await UploadSlidePhoto(slide.SlideID, slide.Picture);
                }

                return CreatedAtAction(nameof(GetSlideById), new { id = slideId }, slide);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new slide.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // update by imad 23/01/2025 8:10 replace UpdateSlide with old UpdateSlide
        /// <summary>
        /// Updates an existing slide.
        /// </summary>
        /// <param name="id">The ID of the slide to update.</param>
        /// <param name="slide">The updated slide data.</param>
        [HttpPut("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Slide updated successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid slide data")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Slide not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> UpdateSlide(int id, [FromBody] Slide slide)
        {
            try
            {
                if (slide == null || id != slide.SlideID)
                {
                    return BadRequest("Invalid slide data or ID mismatch.");
                }

                slide.UpdatedBy = "admin"; // Replace with dynamic user context
                var success = _slideService.UpdateSlide(slide, slide.UpdatedBy);

                if (!success)
                {
                    return NotFound("Slide not found.");
                }

                if (slide.Picture != null && slide.Picture.Length > 0)
                {
                    await UploadSlidePhoto(slide.SlideID, slide.Picture);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating slide with ID {id}.");
                return StatusCode(500, "Internal server error.");
            }
        }

        // update by imad 23/01/2025 8:10 replace UploadSlidePhoto with old UploadSlidePhoto
        /// <summary>
        /// Uploads a photo for a slide.
        /// </summary>
        /// <param name="slideID">The ID of the slide.</param>
        /// <param name="file">The photo file.</param>
        [HttpPost("{slideID}/upload-photo")]
        //[Authorize(Roles = "Admin")]
        [SwaggerResponse(StatusCodes.Status201Created, "Photo uploaded successfully")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid file upload")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public async Task<IActionResult> UploadSlidePhoto(int slideID, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Invalid file upload.");
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Invalid file type. Only JPG and PNG files are allowed.");
                }

                if (file.Length > 10 * 1024 * 1024)
                {
                    return BadRequest("File size exceeds the maximum limit of 10MB.");
                }

                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Slides");
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var photoUrl = $"/uploads/Slides/{fileName}";
                _slideService.UpdateSlidePicture(slideID, photoUrl);

                return CreatedAtAction(nameof(UploadSlidePhoto), new { slideID, photoUrl }, new { Message = "Photo uploaded successfully.", Url = photoUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading photo for slide ID {slideID}.");
                return StatusCode(500, "Internal server error.");
            }
        }
        /// <summary>
        /// Deletes a slide by its ID.
        /// </summary>
        /// <param name="id">The ID of the slide to delete.</param>
        [HttpDelete("{id}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Slide deleted successfully")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Slide not found")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal server error")]
        public IActionResult DeleteSlide(int id)
        {
            try
            {
                var success = _slideService.DeleteSlide(id);
                if (!success)
                {
                    return NotFound("Slide not found.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting slide with ID {id}.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}

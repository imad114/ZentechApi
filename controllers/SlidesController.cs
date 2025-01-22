﻿using Microsoft.AspNetCore.Authorization;
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


                    slide.PicturePath =  await UploadSlidePhoto(slide.SlideID, slide.Picture);
                    _slideService.UpdateSlide(slide,slide.UpdatedBy);
                }

                return CreatedAtAction(nameof(GetSlideById), new { id = slideId }, slide);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding new slide.");
                return StatusCode(500, "Internal server error.");
            }
        }

        /// <summary>
        /// Updates an existing slide.
        /// </summary>
        /// <param name="slide">The updated slide data.</param>

        [HttpPut]
        public async Task<IActionResult> UpdateSlide([FromForm] Slide slide)
        {
            try
            {

                var updatedBy = "admin";
                slide.UpdatedBy = updatedBy;
                slide.PicturePath = await UploadSlidePhoto(slide.SlideID,slide.Picture);
                _slideService.UpdateSlide(slide,slide.UpdatedBy);

                if (string.IsNullOrEmpty(slide.PicturePath))
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
                return StatusCode(500, "Internal server error.");
            }
        }



        [HttpPost("{productId}/upload-photoSlide")]
        [SwaggerResponse(StatusCodes.Status201Created, "Photo uploaded successfully", typeof(object))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid file upload", typeof(object))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while uploading the photo", typeof(object))]
        [Authorize(Roles = "Admin")]
        public async Task<string> UploadSlidePhoto(int slideID, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {

                    throw new Exception("File is empty");
                }


                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {

                    throw new Exception("Invalid file type. Only JPG and PNG files are allowed.");
                }

                if (file.Length > 10 * 1024 * 1024)
                {
                    throw new Exception("File size exceeds the maximum limit of 10MB.");

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

                return photoUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading photo for slide ID {slideID}.");
                throw new Exception("File not uploaded !"+ex.Message);

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

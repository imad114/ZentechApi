using Microsoft.AspNetCore.Mvc;
using ZentechAPI.Dto;
using ZentechAPI.Models;
using Zentech.Services;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ZentechAPI.Controllers
{
    /// <summary>
    /// API controller for managing pages.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly PageService _pageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageController"/> class.
        /// </summary>
        /// <param name="pageService">The service used for managing pages.</param>
        public PageController(PageService pageService)
        {
            _pageService = pageService;
        }

        /// <summary>
        /// Retrieves all pages.
        /// </summary>
        /// <returns>A list of all pages.</returns>
        /// <response code="200">Returns the list of pages.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet]
        public IActionResult GetAllPages()
        {
            try
            {
                var pages = _pageService.GetAllPages();
                return Ok(pages);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a page by its ID.
        /// </summary>
        /// <param name="id">The ID of the page.</param>
        /// <returns>The requested page.</returns>
        /// <response code="200">Returns the requested page.</response>
        /// <response code="404">If the page is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("{id}")]
        public IActionResult GetPageById(int id)
        {
            try
            {
                var page = _pageService.GetPageById(id);
                if (page == null)
                {
                    return NotFound("Page not found.");
                }
                return Ok(page);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a page by its slug.
        /// </summary>
        /// <param name="slug">The slug of the page.</param>
        /// <returns>The requested page.</returns>
        /// <response code="200">Returns the requested page.</response>
        /// <response code="404">If the page is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpGet("slug/{slug}")]
        public IActionResult GetPageBySlug(string slug)
        {
            try
            {
                var page = _pageService.GetPageBySlug(slug);
                if (page == null)
                {
                    return NotFound("Page not found.");
                }
                return Ok(page);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Adds a new page.
        /// </summary>
        /// <param name="pageDto">The page data.</param>
        /// <returns>The created page.</returns>
        /// <response code="201">If the page is successfully created.</response>
        /// <response code="400">If the page data is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost]
        public IActionResult AddPage([FromBody] PageDto pageDto)
        {
            try
            {
                if (pageDto == null)
                {
                    return BadRequest("Page data is null.");
                }

                var createdBy = "admin"; // In a real application, you'd fetch this from the authenticated user
                var pageId = _pageService.AddPage(pageDto, createdBy);

                return CreatedAtAction(nameof(GetPageById), new { id = pageId }, pageDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing page.
        /// </summary>
        /// <param name="id">The ID of the page to update.</param>
        /// <param name="pageDto">The updated page data.</param>
        /// <response code="204">If the page is successfully updated.</response>
        /// <response code="400">If the page data is invalid.</response>
        /// <response code="404">If the page is not found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPut("{id}")]
        public IActionResult UpdatePage(int id, [FromBody] PageDto pageDto)
        {
            try
            {
                if (pageDto == null)
                {
                    return BadRequest("Page data is null.");
                }

                if (id != pageDto.Id)
                {
                    return BadRequest("Page ID mismatch.");
                }

                var updatedBy = "admin"; // In a real application, you'd fetch this from the authenticated user
                var success = _pageService.UpdatePage(pageDto, updatedBy);

                if (!success)
                {
                    return NotFound("Page not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Increments the visitor count for a specified page.
        /// </summary>
        /// <param name="id">The ID of the page whose visitor count is to be incremented.</param>
        /// <returns>A success message if the operation is successful.</returns>
        /// <response code="200">If the visitor count is incremented successfully.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpPost("/api/pages/{id}/increment-visitor-count")]
        public IActionResult IncrementVisitorCount(int id)
        {
            try
            {
                _pageService.IncrementVisitorCount(id);
                return Ok(new { message = "Visitor count incremented successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while incrementing visitor count.", error = ex.Message });
            }
        }


        /// <summary>
        /// Deletes a page by its ID.
        /// </summary>
        /// <param name="id">The ID of the page to delete.</param>
        /// <response code="204">If the page is successfully deleted.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [HttpDelete("{id}")]
        public IActionResult DeletePage(int id)
        {
            try
            {
                _pageService.DeletePage(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

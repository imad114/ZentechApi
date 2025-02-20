using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zentech.Services;
using ZentechAPI.Models;

namespace ZentechAPI.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TechnicalDocController : ControllerBase
    {
        private readonly TechnicalDocService _technicalDocService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TechnicalDocController"/> class.
        /// </summary>
        /// <param name="technicalDocService">The service used for managing technical documents.</param>
        public TechnicalDocController(TechnicalDocService technicalDocService)
        {
            _technicalDocService = technicalDocService;
        }

        /// <summary>
        /// Adds a new technical document.
        /// </summary>
        /// <param name="technicalDoc">The technical document data.</param>
        /// <returns>The created technical document.</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddTechnicalDoc([FromForm] TechincalDoc technicalDoc)
        {
            try
            {
                if (technicalDoc == null)
                {
                    return BadRequest("Technical document data is null.");
                }

                technicalDoc.CreatedBy = "admin"; // Assigning a default creator
               

                if (technicalDoc.file != null && technicalDoc.file.Length > 0)
                {
                    var filePath = await SaveFile(technicalDoc.file);
                    technicalDoc.filePath = filePath;
                }

                var tdId = _technicalDocService.AddTechnicalDoc(technicalDoc, technicalDoc.CreatedBy);
                technicalDoc.TD_ID = tdId;

                return CreatedAtAction(nameof(GetTechnicalDocById), new { id = tdId }, technicalDoc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates an existing technical document.
        /// </summary>
        /// <param name="id">The ID of the technical document to update.</param>
        /// <param name="technicalDoc">The updated technical document data.</param>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTechnicalDoc([FromForm] TechincalDoc technicalDoc)
        {
            try
            {
                if (technicalDoc == null)
                {
                    return BadRequest("Technical document data is null.");
                }

                technicalDoc.UpdatedBy = "admin";

                // Save file if uploaded
                if (technicalDoc.file != null && technicalDoc.file.Length > 0)
                {
                    var filePath = await SaveFile(technicalDoc.file);
                    technicalDoc.filePath = filePath;
                }

                var success = _technicalDocService.UpdateTechnicalDoc(technicalDoc, technicalDoc.UpdatedBy);

                if (!success)
                {
                    return NotFound("Technical document not found.");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves a technical document by its ID.
        /// </summary>
        /// <param name="id">The ID of the technical document.</param>
        /// <returns>The requested technical document.</returns>
        [HttpGet("{id}")]
        public IActionResult GetTechnicalDocById(string id)
        {
            try
            {
                var technicalDoc = _technicalDocService.GetTechnicalDocByID(id);
                if (technicalDoc == null)
                {
                    return NotFound("Technical document not found.");
                }
                return Ok(technicalDoc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("All/{limit}")]
        public IActionResult GetAllTechnicalDocs(int limit)
        {
            try
            {
                var technicalDoc = _technicalDocService.GetAllTechnicalDocs(limit);
                if (technicalDoc == null)
                {
                    return NotFound("Technical document not found.");
                }
                return Ok(technicalDoc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("WithCategories/{limit}")]
        public IActionResult GetAllTechnicalDocsWithCategories(int limit)
        {
            try
            {
                var technicalDoc = _technicalDocService.GetAllTechnicalDocsWithCategories(limit);
                if (technicalDoc == null)
                {
                    return NotFound("Technical document not found.");
                }
                return Ok(technicalDoc);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Helper method to save uploaded files.
        /// </summary>
        /// <param name="file">The file to save.</param>
        /// <returns>The file path.</returns>
        private async Task<string> SaveFile(IFormFile file)
        {
            try
            {
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new Exception("Invalid file type. Only PDF, DOC, and DOCX files are allowed.");
                }

                if (file.Length > 10 * 1024 * 1024)
                {
                    throw new Exception("File size exceeds the maximum limit of 10MB.");
                }

                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/TD");
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

                return $"/uploads/TD/{fileName}";
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving file: {ex.Message}");
            }
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteSlide(int id)
        {
            try
            {
                _technicalDocService.DeleteTechnicalDoc(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        #region Categories methods

        // Get all categories
        /// <summary>
        /// Retrieve all categories.
        /// </summary>
        /// <returns>Returns the complete list of categories.</returns>
        [HttpGet("CategoriesDoc")]
        [SwaggerOperation(Summary = "Get all categories", Description = "Returns the complete list of categories.")]
        public async Task<IActionResult> GetAllCategories()
        {
           
                var categories = await _technicalDocService.GetAllCategories();
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
        public async Task<IActionResult> AddTechnicalDocCategory([FromBody] Other_Category category)
        {
            if (category == null)
            {
                return BadRequest("Category is null.");
            }

            try
            {
                string createdBy = User.Identity?.Name;
                category.CreatedBy = createdBy ?? "null";
                var createdCategory = await _technicalDocService.AddTechnicalDocCategory(category);
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
        public async Task<IActionResult> UpdateTechnicalDocCategory([FromBody] Other_Category category)
        {
            if (category == null)
            {
                return BadRequest("Category is null.");
            }

            try
            {
                string updatedBy = User.Identity?.Name;
                category.UpdatedBy = updatedBy ?? "null";
                var updatedCategory = await _technicalDocService.UpdateTechnicalDocCategory(category);
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
        public async Task<IActionResult> DeleteTechnicalDocCategory([FromRoute] int id)
        {
            try
            {
                int result = await _technicalDocService.DeleteTechnicalDocCategory(id);
                if (result > 0)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound($"Category with ID {id} not found or The category used by another entity. ");
                }
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion
    }
}

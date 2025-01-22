using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> AddTechnicalDoc([FromForm] TechincalDoc technicalDoc)
        {
            try
            {
                if (technicalDoc == null)
                {
                    return BadRequest("Technical document data is null.");
                }

                technicalDoc.CreatedBy = "admin"; // Assigning a default creator
                technicalDoc.CreateDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

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
        public async Task<IActionResult> UpdateTechnicalDoc([FromForm] TechincalDoc technicalDoc)
        {
            try
            {
                if (technicalDoc == null)
                {
                    return BadRequest("Technical document data is null.");
                }


                technicalDoc.UpdatedDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                // Save file if uploaded
                if (technicalDoc.file != null && technicalDoc.file.Length > 0)
                {
                    var filePath = await SaveFile(technicalDoc.file);
                    technicalDoc.filePath = filePath;
                }

                var success = _technicalDocService.UpdateTechnicalDoc(technicalDoc);

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

        [HttpGet("Categories")]
        public IActionResult GetAllCategories()
        {
            try
            {
                var categories = _technicalDocService.GetAllCategories();
                if (categories == null || !categories.Any())
                {
                    return NotFound("No categories found.");
                }
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("Categories")]
        public IActionResult AddTechnicalDocCategory([FromForm] Other_Category category)
        {
            if (category == null)
            {
                return BadRequest("Category is null.");
            }

            try
            {
                string createdBy = User.Identity?.Name;
                category.CreatedBy = createdBy ?? "null";
                var createdCategory = _technicalDocService.AddTechnicalDocCategory(category);
                return CreatedAtAction(nameof(GetAllCategories), new { id = createdCategory.CategoryID }, createdCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("Categories")]
        public IActionResult UpdateTechnicalDocCategory([FromForm] Other_Category category)
        {

            try
            {
                var updatedCategory = _technicalDocService.UpdateTechnicalDocCategory(category);
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

        [HttpDelete("Categories/{id}")]
        public IActionResult DeleteTechnicalDocCategory([FromRoute] int id)
        {
            try
            {
                _technicalDocService.DeleteTechnicalDocCategory(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Category with ID {id} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion
    }
}

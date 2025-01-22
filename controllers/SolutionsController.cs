using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Zentech.Services;
using ZentechAPI.Dto;
using ZentechAPI.Models;

namespace Zentech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SolutionsController : ControllerBase
    {
        private readonly SolutionService _solutionService;

        public SolutionsController(SolutionService solutionService)
        {
            _solutionService = solutionService;
        }

        /// <summary>
        /// Retrieve all solutions.
        /// </summary>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all solutions", Description = "Returns a list of all solutions.")]
        public async Task<IActionResult> GetAllSolutions()
        {
            var solutions = await _solutionService.GetAllSolutionsAsync();
            return Ok(solutions);
        }

        [HttpGet("GetSolutionById/{id}")]
        public IActionResult GetSolutionById(int id)
        {
            try
            {
                var solution = _solutionService.GetSolutionById(id);
                if (solution == null)
                    return NotFound("Solution not found.");

                return Ok(solution);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("AddSolution")]
        [SwaggerOperation(Summary = "Add a Solution", Description = "Adds a new Solution to the system.")]
        public IActionResult AddSolution([FromBody] SolutionDto solutionDto)
        {
            try
            {
                var createdBy = User.Identity?.Name;
                var createdSolution = _solutionService.AddSolution(solutionDto, createdBy); // Assuming User.Identity.Name gives the creator's username.
                return CreatedAtAction(nameof(GetSolutionById), new { id = createdSolution.SolutionID }, createdSolution);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }


        [HttpPost("AddProductsToSolution")]
        public IActionResult AddProductsToSolution([FromBody] AddProductsToSolutionDto dto)
        {
            if (dto == null || dto.ProductIDs == null || !dto.ProductIDs.Any())
            {
                return BadRequest("Invalid data: 'dto' or 'ProductIDs' is missing or empty.");
            }

            try
            {
                _solutionService.AddProductsToSolution(dto.SolutionID, dto.ProductIDs);
                return Ok("Products added successfully to the solution.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }



        /// <summary>
        /// Upload a photo for a specific solution.
        /// </summary>
        /// <param name="solutionId">The ID of the solution to associate the photo with.</param>
        /// <param name="file">The photo file to upload.</param>
        [HttpPost("{solutionId}/upload-photoSolution")]
        [SwaggerOperation(Summary = "Upload a photo for  solution", Description = "Allows uploading a photo for a specific solution.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Photo uploaded successfully", typeof(object))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid file upload", typeof(object))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while uploading the photo", typeof(object))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadPhotoToSolution(int solutionId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { Message = "Invalid file upload." });
                }


                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { Message = "Invalid file type. Only JPG and PNG files are allowed." });
                }

               
                if (file.Length > 10 * 1024 * 1024)
                {
                    return BadRequest(new { Message = "File size exceeds the maximum limit of 10MB." });
                }

                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/Solutions");
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

                var photoUrl = $"/uploads/Solutions/{fileName}";

                _solutionService.AddPhotoToSolution(solutionId, photoUrl);

                return CreatedAtAction("UploadPhotoToSolution", new { solutionId, photoUrl }, new { Message = "Photo uploaded successfully.", Url = photoUrl });
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logger here)
                return StatusCode(500, new { Message = "An error occurred while uploading the photo.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Updates an existing solution.
        /// </summary>
        /// <param name="solutionDto">The solution data to update.</param>
        /// <returns>HTTP response indicating success or failure.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a solution", Description = "Updates the details of an existing solution.")]
        public IActionResult UpdateSolution(int id, [FromBody] SolutionDto solutionDto)
        {
            if (solutionDto == null || id != solutionDto.SolutionID)
            {
                return BadRequest(new { Message = "Invalid solution data or mismatched ID." });
            }

            try
            {
                var updatedBy = User.Identity.Name ?? "System"; // Use the logged-in username or fallback to 'System'
                _solutionService.UpdateSolution(solutionDto, updatedBy);
                return Ok(new { Message = "Solution updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error updating solution: {ex.Message}" });
            }
        }


        /// <summary>
        /// Delete a photo from a product using its URL.
        /// </summary>
        /// <param name="photoUrl">The URL of the photo to delete.</param>
        [Authorize(Roles = "Admin")]
        [HttpDelete("photos")]
        [SwaggerOperation(Summary = "Delete a photo from a solution", Description = "Deletes a specific photo by its URL.")]
        public IActionResult DeletePhotoFromProduct([FromBody] string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
            {
                return BadRequest(new { Message = "Invalid photo URL." });
            }

            try
            {
                // Delete the photo from the database
                _solutionService.DeletePhotoFromSolution(photoUrl);

                // Ensure the photoUrl is a relative path starting with "/uploads/"
                var relativePath = photoUrl.StartsWith("/uploads/Solutions/") ? photoUrl : $"/uploads/Solutions/{Path.GetFileName(photoUrl)}";

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



        // added by imad 20/01/25 13h30

        /// <summary>
        /// Delete a product from a solution.
        /// </summary>
        /// <param name="solutionId">The ID of the solution.</param>
        /// <param name="productId">The ID of the product.</param>
        /// <returns>HTTP 200 OK if successful, or an error message.</returns>
        [HttpDelete("{solutionId}/products/{productId}")]
        public IActionResult DeleteProductFromSolution(int solutionId, int productId)
        {
            if (solutionId <= 0 || productId <= 0)
                return BadRequest("Invalid solution or product ID.");

            try
            {
                _solutionService.DeleteProductFromSolution(solutionId, productId);
                return Ok(new { message = "Product successfully removed from the solution." });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while processing your request.", details = ex.Message });
            }
        }



        /// <summary>
        /// Delete a solution by its ID.
        /// </summary>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a solution", Description = "Deletes a solution from the system.")]
        public async Task<IActionResult> DeleteSolution(int id)
        {
            await _solutionService.DeleteSolutionAsync(id);
            return NoContent();
        }
    }
}

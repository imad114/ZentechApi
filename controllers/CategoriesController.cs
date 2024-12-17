using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using Zentech.Services;
using ZentechAPI.Models;

namespace Zentech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoryController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // Get all categories
        /// <summary>
        /// Retrieve all categories.
        /// </summary>
        /// <returns>The complete list of categories.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all categories", Description = "Returns the complete list of categories.")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // Get a category by ID
        /// <summary>
        /// Retrieve a category by its ID.
        /// </summary>
        /// <param name="id">Unique identifier of the category.</param>
        /// <returns>The details of the corresponding category.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a category by ID", Description = "Returns the details of a specific category.")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        // Add a new category
        /// <summary>
        /// Add a new category.
        /// </summary>
        /// <param name="category">Object containing the details of the category to add.</param>
        /// <returns>The created category with its identifier.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [SwaggerOperation(Summary = "Add a category", Description = "Adds a new category to the system.")]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdBy = User.Identity.Name; // Récupère le nom de l'utilisateur connecté
            var createdCategory = await _categoryService.AddCategoryAsync(category, createdBy);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.CategoryID }, createdCategory);
        }

<<<<<<< HEAD
        // Update an existing category
        /// <summary>
        /// Update an existing category.
=======
        /// <summary>
        /// Update existing category.
>>>>>>> 49e709d9939d5a8fc9c3035e096dd3097f1d532f
        /// </summary>
        /// <param name="id">ID of the category to update.</param>
        /// <param name="category">Object containing the updated information of the category.</param>
        /// <returns>The updated category.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a category", Description = "Updates the information of an existing category.")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.CategoryID)
                return BadRequest("ID de la catégorie non valide.");

            var updatedBy = User.Identity.Name;  // Get the name of the logged-in user
            var isUpdated = await _categoryService.UpdateCategoryAsync(category, updatedBy);
            if (!isUpdated)
                return NotFound();

            return Ok(category);
        }

        // Delete a category
        /// <summary>
        /// Delete a category.
        /// </summary>
        /// <param name="id">ID of the category to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a category", Description = "Deletes a category from the system.")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var isDeleted = await _categoryService.DeleteCategoryAsync(id);
            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
    }
}

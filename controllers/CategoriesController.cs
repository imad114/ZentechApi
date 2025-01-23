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



        #region Sub Category 
        // Get all categories
        /// <summary>
        /// Retrieve all categories.
        /// </summary>
        /// <returns>Returns the complete list of categories.</returns>
        [HttpGet]
        [Route("sub")]
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
        [HttpGet("sub/{id}")]
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
        [HttpPost("sub")]
        [SwaggerOperation(Summary = "Add a category", Description = "Adds a new category to the system.")]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Validate the FK (MainCategoryID)
            var mainCategory = await _categoryService.GetMainProdCategoryByIdAsync(category.MainCategoryID);
            if (mainCategory == null)
                return BadRequest($"MainCategoryID {category.MainCategoryID} does not exist.");

            var createdBy = User.Identity?.Name ?? "System"; // Récupère le nom de l'utilisateur connecté
            var createdCategory = await _categoryService.AddCategoryAsync(category, createdBy);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.CategoryID }, createdCategory);
        }

        // Update an existing category
        /// <summary>
        /// Update an existing category.
        /// </summary>
        /// <param name="id">ID of the category to update.</param>
        /// <param name="category">Object containing the updated information of the category.</param>
        /// <returns>The updated category.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("sub/{id}")]
        [SwaggerOperation(Summary = "Update a category", Description = "Updates the information of an existing category.")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.CategoryID)
                return BadRequest("The provided category ID does not match the ID in the request.");

            // Validate the FK (MainCategoryID)
            var mainCategory = await _categoryService.GetMainProdCategoryByIdAsync(category.MainCategoryID);
            if (mainCategory == null)
                return BadRequest($"MainCategoryID {category.MainCategoryID} does not exist.");


            var updatedBy = User.Identity?.Name ?? "System"; // Get the name of the logged-in user
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
        [HttpDelete("sub/{id}")]
        [SwaggerOperation(Summary = "Delete a category", Description = "Deletes a category from the system.")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var isDeleted = await _categoryService.DeleteCategoryAsync(id);
            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
        #endregion

        #region Main Category
        // Get all main categories
        /// <summary>
        /// Retrieve all main categories.
        /// </summary>
        /// <returns>Returns the complete list of main categories.</returns>
        [HttpGet]
        [Route("main")]
        [SwaggerOperation(Summary = "Get all main categories", Description = "Returns the complete list of main categories.")]
        public async Task<IActionResult> GetAllMainProdCategories()
        {
            var categories = await _categoryService.GetAllMainProdCategoriesAsync();
            return Ok(categories);
        }

        // Get a main category by ID
        /// <summary>
        /// Retrieve a main category by its ID.
        /// </summary>
        /// <param name="id">Unique identifier of the main category.</param>
        /// <returns>The details of the corresponding main category.</returns>
        [HttpGet("main/{id}")]
        [SwaggerOperation(Summary = "Get a main category by ID", Description = "Returns the details of a specific main category.")]
        public async Task<IActionResult> GetMainProdCategoryById(int id)
        {
            var category = await _categoryService.GetMainProdCategoryByIdAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        // Add a new main category
        /// <summary>
        /// Add a new main category.
        /// </summary>
        /// <param name="category">Object containing the details of the main category to add.</param>
        /// <returns>The created main category with its identifier.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("main")]
        [SwaggerOperation(Summary = "Add a main category", Description = "Adds a new main category to the system.")]
        public async Task<IActionResult> AddMainProdCategory([FromBody] MainProdCategory category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdBy = User.Identity.Name; // Retrieve the name of the logged-in user
            var createdCategory = await _categoryService.AddMainProdCategoryAsync(category, createdBy);
            return CreatedAtAction(nameof(GetMainProdCategoryById), new { id = createdCategory.CategoryID }, createdCategory);
        }

        // Update an existing main category
        /// <summary>
        /// Update an existing main category.
        /// </summary>
        /// <param name="id">ID of the main category to update.</param>
        /// <param name="category">Object containing the updated information of the main category.</param>
        /// <returns>The updated main category.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("main/{id}")]
        [SwaggerOperation(Summary = "Update a main category", Description = "Updates the information of an existing main category.")]
        public async Task<IActionResult> UpdateMainProdCategory(int id, [FromBody] MainProdCategory category)
        {
            if (id != category.CategoryID)
                return BadRequest("Invalid category ID.");

            var updatedBy = User.Identity.Name; // Retrieve the name of the logged-in user
            var isUpdated = await _categoryService.UpdateMainProdCategoryAsync(category, updatedBy);
            if (!isUpdated)
                return NotFound();

            return Ok(category);
        }

        // Delete a main category
        /// <summary>
        /// Delete a main category.
        /// </summary>
        /// <param name="id">ID of the main category to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("main/{id}")]
        [SwaggerOperation(Summary = "Delete a main category", Description = "Deletes a main category from the system.")]
        public async Task<IActionResult> DeleteMainProdCategory(int id)
        {
            var isDeleted = await _categoryService.DeleteMainProdCategoryAsync(id);
            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
        #endregion
    }
}

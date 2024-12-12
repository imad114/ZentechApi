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

        // Obtenir toutes les catégories
        /// <summary>
        /// Récupérer toutes les catégories.
        /// </summary>
        /// <returns>La liste complète des catégories.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Obtenir toutes les catégories", Description = "Retourne la liste complète des catégories.")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        // Obtenir une catégorie par ID
        /// <summary>
        /// Récupérer une catégorie par son ID.
        /// </summary>
        /// <param name="id">Identifiant unique de la catégorie.</param>
        /// <returns>Les détails de la catégorie correspondante.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtenir une catégorie par ID", Description = "Retourne les détails d'une catégorie spécifique.")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        // Ajouter une nouvelle catégorie
        /// <summary>
        /// Ajouter une nouvelle catégorie.
        /// </summary>
        /// <param name="category">Objet contenant les détails de la catégorie à ajouter.</param>
        /// <returns>La catégorie créée avec son identifiant.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [SwaggerOperation(Summary = "Ajouter une catégorie", Description = "Ajoute une nouvelle catégorie au système.")]
        public async Task<IActionResult> AddCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdBy = User.Identity.Name; // Récupère le nom de l'utilisateur connecté
            var createdCategory = await _categoryService.AddCategoryAsync(category, createdBy);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.CategoryID }, createdCategory);
        }

        /// <summary>
        /// Update existing category.
        /// </summary>
        /// <param name="id">ID de la catégorie à mettre à jour.</param>
        /// <param name="category">Objet contenant les informations mises à jour de la catégorie.</param>
        /// <returns>La catégorie mise à jour.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Mettre à jour une catégorie", Description = "Met à jour les informations d'une catégorie existante.")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.CategoryID)
                return BadRequest("ID de la catégorie non valide.");

            var updatedBy = User.Identity.Name;  // Récupère le nom de l'utilisateur connecté
            var isUpdated = await _categoryService.UpdateCategoryAsync(category, updatedBy);
            if (!isUpdated)
                return NotFound();

            return Ok(category);
        }

        // Supprimer une catégorie
        /// <summary>
        /// Supprimer une catégorie.
        /// </summary>
        /// <param name="id">ID de la catégorie à supprimer.</param>
        /// <returns>Confirmation de la suppression.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Supprimer une catégorie", Description = "Supprime une catégorie du système.")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var isDeleted = await _categoryService.DeleteCategoryAsync(id);
            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
    }
}

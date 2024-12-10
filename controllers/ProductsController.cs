using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using System.Collections.Generic;
using Zentech.Services;
using ZentechAPI.Models;

namespace Zentech.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // Get all products
        /// <summary>
        /// Get all products.
        /// </summary>
        /// <remarks>
        /// This endpoint returns the complete list of available products.
        /// Accessible without authentication.
        /// </remarks>
        /// <returns>A list of all products.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all products", Description = "Returns the complete list of products.")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // Get a specific product by ID
        /// <summary>
        /// Get a product by ID.
        /// </summary>
        /// <param name="id">Unique product identifier.</param>
        /// <returns>The details of the corresponding product.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a product by ID", Description = "Returns the details of a specific product.")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            return Ok(product);
        }

        // Add a new product
        /// <summary>
        /// Add a new product.
        /// </summary>
        /// <param name="product">Object containing the details of the product to be added.</param>
        /// <returns>The created product with its identifier.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [SwaggerOperation(Summary = "Add a product", Description = "Adds a new product to the system.")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var createdBy = User.Identity.Name; // // Retrieves the name of the logged-in user
            var createdProduct = await _productService.AddProductAsync(product, createdBy);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.ProductID }, createdProduct);
        }

        // Update an existing product
        /// <summary>
        /// Update an existing product.
        /// </summary>
        /// <param name="id">ID of the product to update.</param>
        /// <param name="product">Object containing the updated product information.</param>
        /// <returns>The updated product.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a product", Description = "Updates the information of an existing product.")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.ProductID)
                return BadRequest("Product ID mismatch.");

            var createdBy = User.Identity.Name;  // Retrieves the name of the logged-in user

            var isUpdated = await _productService.UpdateProductAsync(product, createdBy);
            if (!isUpdated)
                return NotFound();

            return Ok(product);
        }

        // Delete a product
        /// <summary>
        /// Delete a product.
        /// </summary>
        /// <param name="id">ID of the product to delete.</param>
        /// <returns>Confirmation of deletion.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a product", Description = "Deletes a product from the system.")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var isDeleted = await _productService.DeleteProductAsync(id);
            if (!isDeleted)
                return NotFound();

            return NoContent();
        }
        /// <summary>
        /// Add a photo to a product.
        /// Accessible only to users with the "Admin" role.
        /// </summary>
        /// <param name="productId">The product ID.</param>
        /// <param name="photoUrl">The URL of the photo to add.</param>
        /// <returns>OK response if the photo is successfully added.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost("{productId}/photos")]
        public IActionResult AddPhotoToProduct(int productId, [FromBody] string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
            {
                return BadRequest(new { Message = "Invalid photo URL." });
            }

            _productService.AddPhotoToProduct(productId, photoUrl);
            return Ok(new { Message = "Photo added successfully." });
        }

        /// <summary>
        /// Delete a photo from a product.
        /// Accessible only to users with the "Admin" role.
        /// </summary>
        /// <param name="photoUrl">The URL of the photo to delete.</param>
        /// <returns>OK response if the photo is successfully deleted.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("photos")]
        public IActionResult DeletePhotoFromProduct([FromBody] string photoUrl)
        {
            if (string.IsNullOrEmpty(photoUrl))
            {
                return BadRequest(new { Message = "Invalid photo URL." });
            }

            _productService.DeletePhotoFromProduct(photoUrl);
            return Ok(new { Message = "Photo deleted successfully." });
        }
    }
}

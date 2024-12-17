using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Swashbuckle.AspNetCore.Annotations;
using System.IO;
using System.Threading.Tasks;
using Zentech.Services;
using ZentechAPI.Models;
using System;
using ZentechAPI.Dto;

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
        [HttpGet]
        [SwaggerOperation(Summary = "Get all products", Description = "Returns the complete list of products.")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        // Get a specific product by ID
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
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [SwaggerOperation(Summary = "Add a product", Description = "Adds a new product to the system.")]
        public async Task<IActionResult> AddProduct([FromBody] ProductDto product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdBy = User.Identity?.Name;
            var ProductID = await _productService.AddProductAsync(product, createdBy);
            return CreatedAtAction(nameof(GetProductById), new { id = ProductID }, ProductID);
        }

        // Update an existing product
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update a product", Description = "Updates the information of an existing product.")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDto product)
        {
            if (id != product.ProductID)
                return BadRequest("Product ID mismatch.");

            var createdBy = User.Identity?.Name;
            var isUpdated = await _productService.UpdateProductAsync(product, createdBy);

            if (!isUpdated)
                return NotFound();

            return Ok(product);
        }

        // Delete a product
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
        [HttpPost("{productId}/upload-photo")]
        [SwaggerOperation(Summary = "Upload a photo for a product", Description = "Allows uploading a photo for a specific product.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Photo uploaded successfully", typeof(object))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid file upload", typeof(object))]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "An error occurred while uploading the photo", typeof(object))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UploadPhotoToProduct(int productId, IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { Message = "Invalid file upload." });
                }

                // Validation du type de fichier
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { Message = "Invalid file type. Only JPG and PNG files are allowed." });
                }

                // Validation de la taille du fichier (10 Mo max)
                if (file.Length > 10 * 1024 * 1024)
                {
                    return BadRequest(new { Message = "File size exceeds the maximum limit of 10MB." });
                }

                var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
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

                var photoUrl = $"/uploads/{fileName}";

                _productService.AddPhotoToProduct(productId, photoUrl);

                return CreatedAtAction("UploadPhotoToProduct", new { productId, photoUrl }, new { Message = "Photo uploaded successfully.", Url = photoUrl });
            }
            catch (Exception ex)
            {
                // Log the exception (you can use a logger here)
                return StatusCode(500, new { Message = "An error occurred while uploading the photo.", Error = ex.Message });
            }
        }



        // Delete a photo from a product
        [Authorize(Roles = "Admin")]
        [HttpDelete("photos")]
        [SwaggerOperation(Summary = "Delete a photo from a product", Description = "Deletes a specific photo by its URL.")]
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

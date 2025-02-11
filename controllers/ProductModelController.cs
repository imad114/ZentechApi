using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using ZentechAPI.Models;

namespace ZentechAPI.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductModelController : ControllerBase
    {
        private readonly ProductModelService _productModelService;

        public ProductModelController(ProductModelService productModelService)
        {
            _productModelService = productModelService;
        }

        /// <summary>
        /// Get a paginated list of product models.
        /// </summary>
        /// <param name="offset">The starting index for pagination (default: 0).</param>
        /// <param name="limit">The maximum number of models to return (default: 10).</param>
        /// <returns>A list of product models.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all product models", Description = "Returns a paginated list of product models.")]
        [SwaggerResponse(StatusCodes.Status200OK, "List of product models retrieved successfully.")]
        public async Task<IActionResult> GetAllModels(int offset = 0, int limit = 10)
        {
            var models = await _productModelService.GetAllModels(offset, limit);
            return Ok(models);
        }

        /// <summary>
        /// Get a product model by ID.
        /// </summary>
        /// <param name="id">The ID of the product model.</param>
        /// <returns>The requested product model.</returns>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get a product model by ID", Description = "Returns a specific product model based on its ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "Product model retrieved successfully.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product model not found.")]
        public async Task<IActionResult> GetModelById(int id)
        {
            var model = await _productModelService.GetModelById(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        /// <summary>
        /// Get all product models associated with a specific product ID.
        /// </summary>
        /// <param name="prodId">The product ID.</param>
        /// <param name="offset">The starting index for pagination (default: 0).</param>
        /// <param name="limit">The maximum number of models to return (default: 10).</param>
        /// <returns>A list of product models associated with the given product ID.</returns>
        [HttpGet("product/{prodId}")]
        [SwaggerOperation(Summary = "Get product models by product ID", Description = "Returns all product models associated with a specific product ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, "List of product models retrieved successfully.")]
        public async Task<IActionResult> GetModelsByProductId(int prodId, int offset = 0, int limit = 10)
        {
            var models = await _productModelService.GetModelsByProductId(prodId, offset, limit);
            return Ok(models);
        }


        [HttpGet("productModelsBySpecification/{prodId}")]
        public async Task<IActionResult> GetModelsBySpecificationFiltered(int prodId, int offset = 0, int limit = 10,
          string specification = "", string model = "", string displacement = "",
          string coolingType = "", string motorType = "",
          string volFreq = "", string coolingCapW = "", string coolingCapBTU = "", string coolingCapKcal="",
          string copWW = "", string copBTUWh = "")
        {
            // Calling the service method with the parameters  
            var (models, count) = await _productModelService.GetModelsBySpecificationFiltered(prodId,
                specification, model, displacement, coolingType, motorType,
                volFreq, coolingCapW, coolingCapBTU, coolingCapKcal, copWW, copBTUWh, limit, offset);

            // Checking if any models were found  
            if (models == null || models.Count == 0)
            {
                return NotFound("No models found for the specified product ID.");
            }

            // Returning the list of models and optionally the count  
            return Ok(new { models, count });
        }

        [HttpGet("SpecificationFilterOptions/{prodId}")]
        public async Task<IActionResult> GetSpecificationFilterOptions(int prodId,string specification)
        {
            // Calling the service method with the parameters  
            var result = await _productModelService.GetSpecificationFilterOptions(prodId,
                specification);

            // Checking if any models were found  
            if (result == null)
            {
                return NotFound("No filters found for this specification.");
            }

            // Returning the list of models and optionally the count  
            return Ok(result);
        }

        /// <summary>
        /// Add a new product model.
        /// </summary>
        /// <param name="model">The product model to add.</param>
        /// <returns>The ID of the newly created product model.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [SwaggerOperation(Summary = "Add a new product model", Description = "Creates a new product model and returns its ID.")]
        [SwaggerResponse(StatusCodes.Status201Created, "Product model created successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data.")]
        public async Task<IActionResult> AddModel([FromBody] ProductModel model)
        {
            if (model == null)
            {
                return BadRequest("Model cannot be null.");
            }
            var createdBy = User.Identity?.Name ?? "";
            var newModelId = await _productModelService.AddModel(model, createdBy);
            return CreatedAtAction(nameof(GetModelById), new { id = newModelId }, newModelId);
        }

        /// <summary>
        /// Update an existing product model.
        /// </summary>
        /// <param name="model">The product model with updated data.</param>
        /// <returns>No content.</returns>
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [SwaggerOperation(Summary = "Update an existing product model", Description = "Updates the details of an existing product model.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Product model updated successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product model not found.")]
        public async Task<IActionResult> UpdateModel([FromBody] ProductModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Something's wrong with this request.");

            }
            if (model == null)
            {
                return BadRequest("Model cannot be null.");
            }
            var UpdatedBy = User.Identity?.Name ?? "";
            var updated = await _productModelService.UpdateModel(model, UpdatedBy);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Remove a product model by ID.
        /// </summary>
        /// <param name="id">The ID of the product model to remove.</param>
        /// <returns>No content.</returns>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a product model by ID", Description = "Deletes a specific product model based on its ID.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "Product model deleted successfully.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Product model not found.")]
        public async Task<IActionResult> RemoveModel(int id)
        {
            var removed = await _productModelService.RemoveModel(id);
            if (!removed)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Uploads an Excel file and saves data into the database.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="file">Excel file to be processed.</param>
        /// <returns>Number of records successfully inserted.</returns>
        [HttpPost("upload/{productId}")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Import Excel File", Description = "Uploads an Excel file and saves data into the database.")]
        [SwaggerResponse(StatusCodes.Status200OK, "File processed successfully.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid file format.")]
        public async Task<IActionResult> ImportExcel(int productId ,IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            if (!file.FileName.EndsWith(".xlsx"))
            {
                return BadRequest("Invalid file format. Only .xlsx files are allowed.");
            }
            var createdBy = User.Identity?.Name;
            var result = await _productModelService.ProcessExcelFile(file, createdBy, productId);
            return Ok(new { message = $"{result} records inserted successfully." });
        }
    }



}

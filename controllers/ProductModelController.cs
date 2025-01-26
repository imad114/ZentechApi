using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        // GET: api/productmodel
        [HttpGet]
        public async Task<IActionResult> GetAllModels(int offset = 0, int limit = 10)
        {
            var models = await _productModelService.GetAllModels(offset, limit);
            return Ok(models);
        }

        // GET: api/productmodel/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetModelById(int id)
        {
            var model = await _productModelService.GetModelById(id);
            if (model == null)
            {
                return NotFound();
            }
            return Ok(model);
        }

        // GET: api/productmodel/product/{prodId}
        [HttpGet("product/{prodId}")]
        public async Task<IActionResult> GetModelsByProductId(int prodId, int offset = 0, int limit = 10)
        {
            var models = await _productModelService.GetModelsByProductId(prodId, offset, limit);
            return Ok(models);
        }

        // POST: api/productmodel
        [HttpPost]
        public async Task<IActionResult> AddModel([FromBody] ProductModel model)
        {
            if (model == null)
            {
                return BadRequest("Model cannot be null.");
            }

            var newModelId = await _productModelService.AddModel(model, model.CreatedBy);
            return CreatedAtAction(nameof(GetModelById), new { id = newModelId }, newModelId);
        }

        // PUT: api/productmodel
        [HttpPut]
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

            var updated = await _productModelService.UpdateModel(model, model.UpdatedBy);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // DELETE: api/productmodel/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveModel(int id)
        {
            var removed = await _productModelService.RemoveModel(id);
            if (!removed)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

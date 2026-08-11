using Microsoft.AspNetCore.Mvc;
using RuleWay.Application.Services;
using RuleWay.Domain.Entities;
using RuleWay.Application.DTOs;

namespace RuleWay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Product
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();

            return Ok(products);
        }

        // GET: api/Product/filter
        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
            [FromQuery] string? keyword,
            [FromQuery] int? minStock,
            [FromQuery] int? maxStock)
        {
            try
            {
                var products = await _productService.FilterAsync(
                    keyword,
                    minStock,
                    maxStock);

                return Ok(products);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: api/Product
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductDto dto)
        {
            var product = new Product
            {
                Title = dto.Title,
                Description = dto.Description,
                StockQuantity = dto.StockQuantity,
                CategoryId = dto.CategoryId
            };

            try
            {
                var createdProduct = await _productService.CreateAsync(product);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdProduct.Id },
                    createdProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // PUT: api/Product/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
    int id,
    UpdateProductDto dto)
        {
            var existingProduct =
                await _productService.GetByIdAsync(id);

            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Title = dto.Title;
            existingProduct.Description = dto.Description;
            existingProduct.StockQuantity = dto.StockQuantity;
            existingProduct.CategoryId = dto.CategoryId;

            try
            {
                await _productService.UpdateAsync(existingProduct);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
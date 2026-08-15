using Microsoft.AspNetCore.Mvc;
using RuleWay.Application.Common;
using RuleWay.Application.DTOs;
using RuleWay.Application.Services;
using RuleWay.Domain.Entities;

namespace RuleWay.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;
        private readonly IWebHostEnvironment _environment;

        public ProductController(
            ProductService productService,
            IWebHostEnvironment environment)
        {
            _productService = productService;
            _environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _productService.GetAllAsync(
                page,
                pageSize,
                cancellationToken);

            var response = new PagedResult<ProductResponseDto>
            {
                Items = result.Items.Select(ToDto).ToList(),
                TotalCount = result.TotalCount,
                Page = result.Page,
                PageSize = result.PageSize
            };

            return Ok(response);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
            [FromQuery] string? keyword,
            [FromQuery] int? minStock,
            [FromQuery] int? maxStock,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _productService.FilterAsync(
                    keyword,
                    minStock,
                    maxStock,
                    page,
                    pageSize,
                    cancellationToken);

                var response = new PagedResult<ProductResponseDto>
                {
                    Items = result.Items.Select(ToDto).ToList(),
                    TotalCount = result.TotalCount,
                    Page = result.Page,
                    PageSize = result.PageSize
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetByIdAsync(
                id,
                cancellationToken);

            if (product == null)
                return NotFound();

            return Ok(ToDto(product));
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateProductDto dto,
            CancellationToken cancellationToken = default)
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
                var createdProduct = await _productService.CreateAsync(
                    product,
                    cancellationToken);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdProduct.Id },
                    ToDto(createdProduct));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            int id,
            UpdateProductDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingProduct = await _productService.GetByIdAsync(
                id,
                cancellationToken);

            if (existingProduct == null)
                return NotFound();

            existingProduct.Title = dto.Title;
            existingProduct.Description = dto.Description;
            existingProduct.StockQuantity = dto.StockQuantity;
            existingProduct.CategoryId = dto.CategoryId;

            try
            {
                await _productService.UpdateAsync(
                    existingProduct,
                    cancellationToken);

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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            int id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _productService.DeleteAsync(
                    id,
                    cancellationToken);

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

        [HttpPost("{id}/image")]
        public async Task<IActionResult> UploadImage(
            int id,
            IFormFile file,
            CancellationToken cancellationToken = default)
        {
            var product = await _productService.GetByIdAsync(
                id,
                cancellationToken);

            if (product == null)
                return NotFound();

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Dosya seçilmedi." });

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = "Desteklenmeyen dosya formatı. (jpg, jpeg, png, webp)" });

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "Dosya boyutu 5 MB'den büyük olamaz." });

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                var oldFilePath = Path.Combine(_environment.WebRootPath, product.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                    System.IO.File.Delete(oldFilePath);
            }

            product.ImageUrl = $"/uploads/products/{fileName}";

            await _productService.UpdateAsync(product, cancellationToken);

            return Ok(new { imageUrl = product.ImageUrl });
        }

        private static ProductResponseDto ToDto(Product product)
        {
            return new ProductResponseDto
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                StockQuantity = product.StockQuantity,
                IsLive = product.IsLive,
                ImageUrl = product.ImageUrl,
                CategoryId = product.CategoryId
            };
        }
    }
}
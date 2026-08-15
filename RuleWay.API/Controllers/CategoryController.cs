using Microsoft.AspNetCore.Mvc;
using RuleWay.Application.DTOs;
using RuleWay.Application.Services;
using RuleWay.Domain.Entities;

namespace RuleWay.API.Controllers
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

        [HttpGet]
        public async Task<IActionResult> GetAll(
            CancellationToken cancellationToken = default)
        {
            var categories = await _categoryService.GetAllAsync(
                cancellationToken);

            var response = categories.Select(ToDto).ToList();

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(
            int id,
            CancellationToken cancellationToken = default)
        {
            var category = await _categoryService.GetByIdAsync(
                id,
                cancellationToken);

            if (category == null)
                return NotFound();

            return Ok(ToDto(category));
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            CreateCategoryDto dto,
            CancellationToken cancellationToken = default)
        {
            var category = new Category
            {
                Name = dto.Name,
                MinimumStockQuantity = dto.MinimumStockQuantity
            };

            try
            {
                var createdCategory = await _categoryService.CreateAsync(
                    category,
                    cancellationToken);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdCategory.Id },
                    ToDto(createdCategory));
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
            UpdateCategoryDto dto,
            CancellationToken cancellationToken = default)
        {
            var existingCategory = await _categoryService.GetByIdAsync(
                id,
                cancellationToken);

            if (existingCategory == null)
                return NotFound();

            existingCategory.Name = dto.Name;
            existingCategory.MinimumStockQuantity = dto.MinimumStockQuantity;

            try
            {
                await _categoryService.UpdateAsync(
                    existingCategory,
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
                await _categoryService.DeleteAsync(
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
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    message = ex.Message
                });
            }
        }

        private static CategoryResponseDto ToDto(Category category)
        {
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                MinimumStockQuantity = category.MinimumStockQuantity
            };
        }
    }
}
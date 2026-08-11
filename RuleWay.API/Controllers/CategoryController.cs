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

        // GET: api/Category
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();

            return Ok(categories);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                MinimumStockQuantity = dto.MinimumStockQuantity
            };

            try
            {
                var createdCategory = await _categoryService.CreateAsync(category);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdCategory.Id },
                    createdCategory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
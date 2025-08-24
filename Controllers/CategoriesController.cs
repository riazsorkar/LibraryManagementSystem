using AutoMapper;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LibraryManagementSystem.DTOs.CategoryDTOs;

namespace LibraryManagementSystem.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        // GET: api/Categories
        [AllowAnonymous] // Public access
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDTO>>> GetCategories()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return Ok(_mapper.Map<IEnumerable<CategoryResponseDTO>>(categories));
        }

        // GET: api/Categories/5
        [AllowAnonymous] // Public access
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDTO>> GetCategory(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null) return NotFound();

            return Ok(_mapper.Map<CategoryResponseDTO>(category));
        }

        // POST: api/Categories
        [Authorize(Roles = UserRoles.Admin)] // Only Admin can add categories
        [HttpPost]
        public async Task<ActionResult<CategoryResponseDTO>> PostCategory(CategoryCreateDTO categoryCreateDTO)
        {
            var category = _mapper.Map<Category>(categoryCreateDTO);
            await _categoryRepository.AddCategoryAsync(category);

            var categoryResponse = _mapper.Map<CategoryResponseDTO>(category);
            return CreatedAtAction(nameof(GetCategory), new { id = categoryResponse.CategoryId }, categoryResponse);
        }

        // PUT: api/Categories/5
        [Authorize(Roles = UserRoles.Admin)] // Only Admin can update categories
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, CategoryCreateDTO categoryUpdateDTO)
        {
            if (!await _categoryRepository.CategoryExistsAsync(id))
                return NotFound();

            var category = _mapper.Map<Category>(categoryUpdateDTO);
            category.CategoryId = id; // Ensure ID is set

            await _categoryRepository.UpdateCategoryAsync(category);
            return NoContent();
        }

        // DELETE: api/Categories/5
        [Authorize(Roles = UserRoles.Admin)] // Only Admin can delete categories
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            if (!await _categoryRepository.CategoryExistsAsync(id))
                return NotFound();

            await _categoryRepository.DeleteCategoryAsync(id);
            return NoContent();
        }
    }
}

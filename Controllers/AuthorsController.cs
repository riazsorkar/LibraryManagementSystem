using AutoMapper;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LibraryManagementSystem.DTOs.AuthorDTOs;

namespace LibraryManagementSystem.Controllers
{
    // Controllers/AuthorsController.cs
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        // GET: api/Authors
        [AllowAnonymous] // Public access
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorResponseDTO>>> GetAuthors()
        {
            var authors = await _authorRepository.GetAllAuthorsAsync();
            return Ok(_mapper.Map<IEnumerable<AuthorResponseDTO>>(authors));
        }

        // GET: api/Authors/5
        [AllowAnonymous] // Public access
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorResponseDTO>> GetAuthor(int id)
        {
            var author = await _authorRepository.GetAuthorByIdAsync(id);
            if (author == null) return NotFound();

            return Ok(_mapper.Map<AuthorResponseDTO>(author));
        }

        // POST: api/Authors
        [Authorize(Roles = UserRoles.Admin)] // Only Admin can add authors
        [HttpPost]
        public async Task<ActionResult<AuthorResponseDTO>> PostAuthor(AuthorCreateDTO authorCreateDTO)
        {
            var author = _mapper.Map<Author>(authorCreateDTO);
            await _authorRepository.AddAuthorAsync(author);

            var authorResponse = _mapper.Map<AuthorResponseDTO>(author);
            return CreatedAtAction(nameof(GetAuthor), new { id = authorResponse.AuthorId }, authorResponse);
        }

        // PUT: api/Authors/5
        [Authorize(Roles = UserRoles.Admin)] // Only Admin can update authors
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAuthor(int id, AuthorCreateDTO authorUpdateDTO)
        {
            if (!await _authorRepository.AuthorExistsAsync(id))
                return NotFound();

            var author = _mapper.Map<Author>(authorUpdateDTO);
            author.AuthorId = id; // Ensure ID is set

            await _authorRepository.UpdateAuthorAsync(author);
            return NoContent();
        }

        // DELETE: api/Authors/5
        [Authorize(Roles = UserRoles.Admin)] // Only Admin can delete authors
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            if (!await _authorRepository.AuthorExistsAsync(id))
                return NotFound();

            await _authorRepository.DeleteAuthorAsync(id);
            return NoContent();
        }
    }
}

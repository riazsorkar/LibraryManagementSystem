using AutoMapper;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static LibraryManagementSystem.DTOs.BookDTOs;



[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;

    public BooksController(IBookRepository bookRepository, IMapper mapper)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
    }

    // GET: api/Books
    [AllowAnonymous] // Public access
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookResponseDTO>>> GetBooks()
    {
        var books = await _bookRepository.GetAllBooksAsync();
        return Ok(_mapper.Map<IEnumerable<BookResponseDTO>>(books));
    }

    // GET: api/Books/5
    [AllowAnonymous] // Public access
    [HttpGet("{id}")]
    public async Task<ActionResult<BookResponseDTO>> GetBook(int id)
    {
        var book = await _bookRepository.GetBookByIdAsync(id);
        if (book == null) return NotFound();

        return Ok(_mapper.Map<BookResponseDTO>(book));
    }

    // POST: api/Books
    [Authorize(Roles = UserRoles.Admin)] // Only Admin can add books
    [HttpPost]
    public async Task<ActionResult<BookResponseDTO>> PostBook(BookCreateDTO bookCreateDTO)
    {
        var book = _mapper.Map<Book>(bookCreateDTO);
        await _bookRepository.AddBookAsync(book);

        var bookResponse = _mapper.Map<BookResponseDTO>(book);
        return CreatedAtAction(nameof(GetBook), new { id = bookResponse.BookId }, bookResponse);
    }

    // PUT: api/Books/5
    [Authorize(Roles = UserRoles.Admin)] // Only Admin can update books
    [HttpPut("{id}")]
    public async Task<IActionResult> PutBook(int id, BookCreateDTO bookUpdateDTO)
    {
        if (!await _bookRepository.BookExistsAsync(id))
            return NotFound();

        var book = _mapper.Map<Book>(bookUpdateDTO);
        book.BookId = id; // Ensure ID is set

        await _bookRepository.UpdateBookAsync(book);
        return NoContent();
    }

    // DELETE: api/Books/5
    [Authorize(Roles = UserRoles.Admin)] // Only Admin can delete books
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        if (!await _bookRepository.BookExistsAsync(id))
            return NotFound();

        await _bookRepository.DeleteBookAsync(id);
        return NoContent();
    }
}
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/search")]
    [ApiController]
    [AllowAnonymous] // Public access
    public class SearchController : ControllerBase
    {
        private readonly IBookSearchRepository _searchRepository;

        public SearchController(IBookSearchRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        [HttpPost("books")]
        public async Task<ActionResult<BookSearchResultDTO>> SearchBooks([FromBody] BookSearchRequestDTO searchRequest)
        {
            if (searchRequest.Page < 1) searchRequest.Page = 1;
            if (searchRequest.PageSize < 1 || searchRequest.PageSize > 50)
                searchRequest.PageSize = 10;

            var result = await _searchRepository.SearchBooksAsync(searchRequest);
            return Ok(result);
        }

        [HttpGet("suggestions")]
        public async Task<ActionResult<List<string>>> GetSearchSuggestions([FromQuery] string query)
        {
            var suggestions = await _searchRepository.GetSearchSuggestionsAsync(query);
            return Ok(suggestions);
        }

        [HttpGet("quick")]
        public async Task<ActionResult<BookSearchResultDTO>> QuickSearch(
            [FromQuery] string q,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var searchRequest = new BookSearchRequestDTO
            {
                Query = q,
                Page = page,
                PageSize = pageSize
            };

            var result = await _searchRepository.SearchBooksAsync(searchRequest);
            return Ok(result);
        }
    }
}
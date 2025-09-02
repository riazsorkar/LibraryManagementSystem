using LibraryManagementSystem.DTOs;

namespace LibraryManagementSystem.Repositories
{
    public interface IBookSearchRepository
    {
        Task<BookSearchResultDTO> SearchBooksAsync(BookSearchRequestDTO searchRequest);
        Task<List<string>> GetSearchSuggestionsAsync(string query);
    }
}
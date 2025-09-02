using AutoMapper;
using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using static LibraryManagementSystem.DTOs.BookDTOs;

namespace LibraryManagementSystem.Repositories
{
    public class BookSearchRepository : IBookSearchRepository
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;

        public BookSearchRepository(LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BookSearchResultDTO> SearchBooksAsync(BookSearchRequestDTO searchRequest)
        {
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .AsQueryable();

            // Apply search filters
            if (!string.IsNullOrEmpty(searchRequest.Query))
            {
                query = query.Where(b =>
                    b.Title.Contains(searchRequest.Query) ||
                    b.Summary.Contains(searchRequest.Query) ||
                    b.Author.Name.Contains(searchRequest.Query) ||
                    b.Category.Name.Contains(searchRequest.Query));
            }

            if (!string.IsNullOrEmpty(searchRequest.Author))
            {
                query = query.Where(b => b.Author.Name.Contains(searchRequest.Author));
            }

            if (!string.IsNullOrEmpty(searchRequest.Category))
            {
                query = query.Where(b => b.Category.Name.Contains(searchRequest.Category));
            }

            if (searchRequest.AvailableOnly == true)
            {
                query = query.Where(b => b.AvailableCopies > 0);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting
            query = ApplySorting(query, searchRequest.SortBy, searchRequest.SortOrder);

            // Apply pagination
            var books = await query
                .Skip((searchRequest.Page - 1) * searchRequest.PageSize)
                .Take(searchRequest.PageSize)
                .ToListAsync();

            return new BookSearchResultDTO
            {
                Books = _mapper.Map<List<BookResponseDTO>>(books),
                TotalCount = totalCount,
                Page = searchRequest.Page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)searchRequest.PageSize),
                HasNextPage = searchRequest.Page < (int)Math.Ceiling(totalCount / (double)searchRequest.PageSize),
                HasPreviousPage = searchRequest.Page > 1
            };
        }

        public async Task<List<string>> GetSearchSuggestionsAsync(string query)
        {
            if (string.IsNullOrEmpty(query) || query.Length < 2)
                return new List<string>();

            var suggestions = new List<string>();

            // Title suggestions
            var titleSuggestions = await _context.Books
                .Where(b => b.Title.Contains(query))
                .Select(b => b.Title)
                .Distinct()
                .Take(5)
                .ToListAsync();

            suggestions.AddRange(titleSuggestions);

            // Author suggestions
            var authorSuggestions = await _context.Books
                .Include(b => b.Author)
                .Where(b => b.Author.Name.Contains(query))
                .Select(b => b.Author.Name)
                .Distinct()
                .Take(5)
                .ToListAsync();

            suggestions.AddRange(authorSuggestions);

            // Category suggestions
            var categorySuggestions = await _context.Books
                .Include(b => b.Category)
                .Where(b => b.Category.Name.Contains(query))
                .Select(b => b.Category.Name)
                .Distinct()
                .Take(5)
                .ToListAsync();

            suggestions.AddRange(categorySuggestions);

            return suggestions.Distinct().Take(10).ToList();
        }

        private IQueryable<Book> ApplySorting(IQueryable<Book> query, string sortBy, string sortOrder)
        {
            var descending = sortOrder?.ToLower() == "desc";

            return sortBy?.ToLower() switch
            {
                "title" => descending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
                "author" => descending ? query.OrderByDescending(b => b.Author.Name) : query.OrderBy(b => b.Author.Name),
                "category" => descending ? query.OrderByDescending(b => b.Category.Name) : query.OrderBy(b => b.Category.Name),
                "newest" => descending ? query.OrderByDescending(b => b.CreatedAt) : query.OrderBy(b => b.CreatedAt),
                "popular" => descending ? query.OrderByDescending(b => b.BookBorrows.Count) : query.OrderBy(b => b.BookBorrows.Count),
                _ => descending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
            };
        }
    }
}

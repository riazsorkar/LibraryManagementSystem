using static LibraryManagementSystem.DTOs.BookDTOs;

namespace LibraryManagementSystem.DTOs
{
    public class BookSearchRequestDTO
    {
        public string Query { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool? AvailableOnly { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "title"; // title, author, newest, popular
        public string SortOrder { get; set; } = "asc"; // asc, desc
    }

    public class BookSearchResultDTO
    {
        public List<BookResponseDTO> Books { get; set; } = new List<BookResponseDTO>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }
}
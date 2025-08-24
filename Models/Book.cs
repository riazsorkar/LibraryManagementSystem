namespace LibraryManagementSystem.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string? Summary { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public int? AuthorId { get; set; }
        public Author? Author { get; set; }

        public string? CoverImagePath { get; set; }
        public bool HardCopyAvailable { get; set; }
        public string? SoftCopyAvailable { get; set; }
        public string? AudioFileAvailable { get; set; }
        public int TotalCopies { get; set; }

        public int AvailableCopies { get; set; }

        //public bool IsFeatured { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



        public ICollection<BookBorrow> BookBorrows { get; set; }
        public ICollection<BookRating> BookRatings { get; set; }
    }
}

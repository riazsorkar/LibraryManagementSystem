using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class BookBorrow
    {
        public int BorrowId { get; set; }

        [Required]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public DateTime BorrowDate { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime DueDate { get; set; }  // Must be set when borrowing

        public DateTime? ReturnDate { get; set; }  // Nullable until returned

        [Required]
        public string Status { get; set; } = "Pending";

        public int ExtensionCount { get; set; } = 0;
        public DateTime? ApprovalDate { get; set; }
    }
}

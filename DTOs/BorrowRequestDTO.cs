using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs
{
    // DTOs/BorrowRequestDTO.cs
    public class BorrowRequestDTO
    {
        [Required(ErrorMessage = "BookId is required")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "DueDate is required")]
        public DateTime DueDate { get; set; }

        // Internal use only - set by controller from JWT token
        public int UserId { get; set; }
    }

    public class BorrowDetailsDTO
    {
        public int BorrowId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookCoverImage { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; }
        public int ExtensionsUsed { get; set; }
        public bool CanBeExtended { get; set; }

        public DateTime? ApprovalDate { get; set; }
    }
}
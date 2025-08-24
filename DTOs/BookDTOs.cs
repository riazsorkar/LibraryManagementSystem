using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs
{
    public class BookDTOs
    {
        public class BookResponseDTO
        {
            public int BookId { get; set; }
            public string Title { get; set; } = string.Empty;
            public string? Summary { get; set; }  // Nullable
            public string? CoverImagePath { get; set; }  // Nullable
            public bool HardCopyAvailable { get; set; }
            public string? SoftCopyAvailable { get; set; }
            public string? AudioFileAvailable { get; set; }
            public int TotalCopies { get; set; }
            public int AvailableCopies { get; set; }

            // Related data (flattened)
            public string? AuthorName { get; set; }  // Nullable
            public string? CategoryName { get; set; }  // Nullable
        }

        public class BookCreateDTO
        {
            [Required]
            public string Title { get; set; }

            public string Summary { get; set; }
            public int? CategoryId { get; set; }
            public int? AuthorId { get; set; }

            public string CoverImagePath { get; set; }
            public bool HardCopyAvailable { get; set; }
            public string SoftCopyAvailable { get; set; }
            public string AudioFileAvailable { get; set; }

            [Range(1, int.MaxValue, ErrorMessage = "Must have at least 1 copy")]
            public int TotalCopies { get; set; }
            public int AvailableCopies { get; set; }
        }

        public class ExtensionRequestDTO
        {
            [Required]
            public int BorrowId { get; set; }

            [Required]
            [FutureDate(ErrorMessage = "New due date must be in the future")]
            public DateTime NewDueDate { get; set; }
        }

        // Custom validation attribute
        public class FutureDateAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                return value is DateTime date && date > DateTime.Now;
            }
        }

        public class BookRecommendationDTO : BookResponseDTO
        {
            public string RecommendationReason { get; set; }
        }
    }
}

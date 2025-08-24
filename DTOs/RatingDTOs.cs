using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs
{
    public class RatingDTOs
    {
        public class CreateRatingDTO
        {
            [Required]
            [Range(1, 5)]
            public int Rating { get; set; }

            [MaxLength(1000)]
            public string? Review { get; set; }

            [Required]
            public int BookId { get; set; }
        }

        public class RatingResponseDTO
        {
            public int RatingId { get; set; }
            public int Rating { get; set; }
            public string? Review { get; set; }
            public DateTime CreatedAt { get; set; }
            public int BookId { get; set; }
            public string BookTitle { get; set; }
            public int UserId { get; set; }
            public string Username { get; set; }
        }

        public class BookRatingSummaryDTO
        {
            public int BookId { get; set; }
            public string BookTitle { get; set; }
            public double AverageRating { get; set; }
            public int TotalRatings { get; set; }
            public List<RatingResponseDTO> RecentReviews { get; set; }
        }
    }
}

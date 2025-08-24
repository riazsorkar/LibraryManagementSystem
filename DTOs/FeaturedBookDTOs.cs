// DTOs/FeaturedBookDTOs.cs
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs
{
    public class FeaturedBookDTO
    {
        public int FeaturedBookId { get; set; }
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string CoverImagePath { get; set; }
        public bool HardCopyAvailable { get; set; }
        public string SoftCopyAvailable { get; set; }
        public string AudioFileAvailable { get; set; }
        public int TotalCopies { get; set; }
        public int AvailableCopies { get; set; }
        public string AuthorName { get; set; }
        public string CategoryName { get; set; }
        public DateTime FeaturedDate { get; set; }
    }

    public class SetFeaturedBookDTO
    {
        [Required]
        public int BookId { get; set; }
    }
}
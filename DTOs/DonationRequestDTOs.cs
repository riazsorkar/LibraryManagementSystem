// DTOs/DonationRequestDTOs.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.DTOs
{
    public class DonationRequestDTO
    {
        public int DonationRequestId { get; set; }
        public string BookTitle { get; set; }
        public string AuthorName { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string AdminComments { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string BrainStationId { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }

    public class CreateDonationRequestDTO
    {
        [Required(ErrorMessage = "Book title is required")]
        [StringLength(255, ErrorMessage = "Book title cannot exceed 255 characters")]
        public string BookTitle { get; set; }

        [StringLength(100, ErrorMessage = "Author name cannot exceed 100 characters")]
        public string AuthorName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string Reason { get; set; } = string.Empty;

        [Required(ErrorMessage = "BrainStation ID is required")]
        [StringLength(50, ErrorMessage = "BrainStation ID cannot exceed 50 characters")]
        public string BrainStationId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;
    }

    public class UpdateDonationStatusDTO
    {
        [Required(ErrorMessage = "New status is required")]
        [RegularExpression("^(Approved|Rejected)$", ErrorMessage = "Status must be either Approved or Rejected")]
        public string NewStatus { get; set; }

        [StringLength(200, ErrorMessage = "Admin comments cannot exceed 200 characters")]
        public string AdminComments { get; set; } = string.Empty;
    }
}
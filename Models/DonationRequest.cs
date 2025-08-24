using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models
{
    public class DonationRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequestId { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        [StringLength(255)]
        public string BookTitle { get; set; } = string.Empty;

        [StringLength(100)]
        public string AuthorName { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "datetime")]
        public DateTime? ProcessedDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? RequestDate { get; set; } = DateTime.UtcNow;

        [StringLength(200)]
        public string AdminComments { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string BrainStationId { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;
    }
}
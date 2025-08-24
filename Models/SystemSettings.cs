using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class SystemSettings
    {
        [Key]
        public int SettingId { get; set; }
        public int MaxBorrowDuration { get; set; } = 14;
        public int MaxBorrowLimit { get; set; } = 5;
        public int MaxExtensionLimit { get; set; } = 1;
        public int MaxBookingDuration { get; set; } = 7;
        public int MaxBookingLimit { get; set; } = 3;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}

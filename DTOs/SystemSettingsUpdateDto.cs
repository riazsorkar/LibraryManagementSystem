using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models
{
    public class SystemSettingsUpdateDto
    {
        [Range(1, 365, ErrorMessage = "Max borrow duration must be between 1 and 365 days")]
        public int MaxBorrowDuration { get; set; }

        [Range(1, 20, ErrorMessage = "Max borrow limit must be between 1 and 20")]
        public int MaxBorrowLimit { get; set; }

        [Range(0, 10, ErrorMessage = "Max extension limit must be between 0 and 10")]
        public int MaxExtensionLimit { get; set; }

        [Range(1, 30, ErrorMessage = "Max booking duration must be between 1 and 30 days")]
        public int MaxBookingDuration { get; set; }

        [Range(1, 10, ErrorMessage = "Max booking limit must be between 1 and 10")]
        public int MaxBookingLimit { get; set; }
    }
}
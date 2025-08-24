namespace LibraryManagementSystem.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; } = UserRoles.User;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<BookBorrow> BookBorrows { get; set; }
        public ICollection<BookRating> BookRatings { get; set; }
        public ICollection<DonationRequest> DonationRequests { get; set; }
    }

    
}

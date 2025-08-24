namespace LibraryManagementSystem.DTOs
{
    public class UserProfileDTO
    {
        public string username { get; set; }
        public int TotalBorrowedBooks { get; set; }
        public int CurrentlyBorrowedBooks { get; set; }
        public int ReturnedBooks { get; set; }
        public int OverdueBooks { get; set; }
        public List<UserBookActivityDTO> RecentActivity { get; set; }
    }

    public class UserBookActivityDTO
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; } // "Borrowed", "Returned", "Overdue"
    }
}
namespace LibraryManagementSystem.DTOs
{
    public class AdminDashboardDTO
    {
        public int TotalBooks { get; set; }
        public int AvailableBooks { get; set; }
        public int TotalBorrowedBooks { get; set; }
        public int OverdueBooks { get; set; }
        public int NewMembersThisMonth { get; set; }
        public int PendingDonationRequests { get; set; }
        public List<RecentActivityDTO> RecentBorrows { get; set; }
    }

    public class RecentActivityDTO
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string UserName { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
    }
}
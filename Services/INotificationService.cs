namespace LibraryManagementSystem.Services
{
    public interface INotificationService
    {
        Task NotifyUserAboutApproval(int userId, string bookTitle);
        Task NotifyUserAboutRejection(int userId, string bookTitle, string reason);
        Task NotifyUsersAboutApproval(List<int> userIds, List<string> bookTitles);
        Task NotifyUsersAboutRejection(List<int> userIds, List<string> bookTitles, string reason);
    }

}

// Services/IUserContextService.cs
namespace LibraryManagementSystem.Services
{
    public interface IUserContextService
    {
        int GetUserId();
        string GetUserEmail();
        bool IsAuthenticated();
    }
}
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Models.Auth;

namespace LibraryManagementSystem.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(LoginModel model);
        Task<AuthResponse> Register(RegisterModel model);
        string GenerateJwtToken(User user);
    }
}
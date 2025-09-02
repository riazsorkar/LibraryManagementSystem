using LibraryManagementSystem.Models.Auth;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LibraryManagementSystem.DTOs.BookDTOs;
using BCrypt.Net;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ITokenBlacklistService _tokenBlacklistService;

        public AuthController(AuthService authService, ITokenBlacklistService tokenBlacklistService)
        {
            _authService = authService;
            _tokenBlacklistService = tokenBlacklistService;
        }

        //public AuthController(AuthService authService)
        //{
        //    _authService = authService;
        //}

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                var token = await _authService.Register(model.Username, model.Email, model.Password);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        {
            try
            {
                var token = await _authService.Register(model.Username, model.Email, model.Password, "Admin");
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var token = await _authService.Login(model.Username, model.Password);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var token = GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return BadRequest("No token provided");

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var expiry = jwtToken.ValidTo;

            await _tokenBlacklistService.BlacklistTokenAsync(token, expiry);

            return Ok(new
            {
                Message = "Successfully logged out",
                LogoutTime = DateTime.UtcNow
            });
        }

        [HttpPost("validate-token")]
        [Authorize]
        public async Task<IActionResult> ValidateToken()
        {
            var token = GetTokenFromHeader();
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            var isBlacklisted = await _tokenBlacklistService.IsTokenBlacklistedAsync(token);
            if (isBlacklisted)
                return Unauthorized("Token has been invalidated");

            return Ok(new
            {
                Valid = true,
                UserId = GetUserIdFromToken(),
                Username = User.FindFirst(ClaimTypes.Name)?.Value
            });
        }

        private string GetTokenFromHeader()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            return authHeader.StartsWith("Bearer ") ? authHeader.Substring(7) : null;
        }

        private int GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null ? int.Parse(userIdClaim.Value) : 0;
        }
    }


    public class RegisterModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}

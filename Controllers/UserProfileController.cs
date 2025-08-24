using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/user/profile")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileRepository _profileRepository;

        public UserProfileController(IUserProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        [HttpGet]
        public async Task<ActionResult<UserProfileDTO>> GetUserProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var profile = await _profileRepository.GetUserProfileAsync(userId);
            return Ok(profile);
        }
    }
}
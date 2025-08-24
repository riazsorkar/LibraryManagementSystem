using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/admin/dashboard")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IAdminDashboardRepository _dashboardRepository;

        public AdminDashboardController(IAdminDashboardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        [HttpGet]
        public async Task<ActionResult<AdminDashboardDTO>> GetDashboardData()
        {
            var dashboardData = await _dashboardRepository.GetAdminDashboardDataAsync();
            return Ok(dashboardData);
        }
    }
}
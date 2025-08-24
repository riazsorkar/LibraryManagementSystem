using LibraryManagementSystem.DTOs;
using LibraryManagementSystem.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DonationsController : ControllerBase
    {
        private readonly IDonationRepository _donationRepository;

        public DonationsController(IDonationRepository donationRepository)
        {
            _donationRepository = donationRepository;
        }

        // POST: api/donations
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<DonationRequestDTO>> CreateDonationRequest([FromBody] CreateDonationRequestDTO request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var donation = await _donationRepository.CreateDonationRequestAsync(request, userId);
                return CreatedAtAction(nameof(GetDonationById), new { id = donation.DonationRequestId }, donation);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/donations/user
        [HttpGet("user")]
        public async Task<ActionResult<List<DonationRequestDTO>>> GetUserDonations()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var donations = await _donationRepository.GetUserDonationsAsync(userId);
                return Ok(donations);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/donations/user/status/{status}
        [HttpGet("user/status/{status}")]
        public async Task<ActionResult<List<DonationRequestDTO>>> GetUserDonationsByStatus(string status)
        {
            try
            {
                if (status != "Pending" && status != "Approved" && status != "Rejected")
                    return BadRequest("Invalid status. Must be Pending, Approved, or Rejected");

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var allDonations = await _donationRepository.GetUserDonationsAsync(userId);
                var filteredDonations = allDonations.Where(d => d.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();

                return Ok(filteredDonations);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/donations/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<DonationRequestDTO>> GetDonationById(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var donations = await _donationRepository.GetUserDonationsAsync(userId);
                var donation = donations.FirstOrDefault(d => d.DonationRequestId == id);

                if (donation == null)
                    return NotFound($"Donation request with ID {id} not found");

                return Ok(donation);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/donations (Admin only)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DonationRequestDTO>>> GetAllDonations()
        {
            try
            {
                var donations = await _donationRepository.GetAllDonationsAsync();
                return Ok(donations);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/donations/status/{status} (Admin only)
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<DonationRequestDTO>>> GetDonationsByStatus(string status)
        {
            try
            {
                if (status != "Pending" && status != "Approved" && status != "Rejected")
                    return BadRequest("Invalid status. Must be Pending, Approved, or Rejected");

                var donations = await _donationRepository.GetDonationsByStatusAsync(status);
                return Ok(donations);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/donations/{id}/status (Admin only)
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateDonationStatus(int id, [FromBody] UpdateDonationStatusDTO updateDto)
        {
            try
            {
                if (updateDto.NewStatus != "Approved" && updateDto.NewStatus != "Rejected")
                    return BadRequest("NewStatus must be either 'Approved' or 'Rejected'");

                var success = await _donationRepository.UpdateDonationStatusAsync(id, updateDto);
                if (!success) return NotFound($"Donation request with ID {id} not found");

                return NoContent();
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
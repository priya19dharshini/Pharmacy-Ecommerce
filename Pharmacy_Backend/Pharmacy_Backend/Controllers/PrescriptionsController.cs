
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy_Backend.DTOs.Prescription;
using Pharmacy_Backend.Services.Interfaces;

namespace Pharmacy_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _service;

        public PrescriptionsController(IPrescriptionService service)
        {
            _service = service;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User ID not found.");
        }

        // Customer uploads prescription
        [HttpPost("upload")]
        [Authorize(Roles = "Customer")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload(
            [FromForm] UploadPrescriptionDto dto)
        {
            try
            {
                var result = await _service.UploadAsync(
                    GetUserId(),
                    dto.File,
                    dto.OrderId
                );

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = result.Id },
                    result
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Customer views their own prescriptions
        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyPrescriptions()
        {
            var result =
                await _service.GetMyPrescriptionsAsync(GetUserId());

            return Ok(result);
        }

        // Admin and Pharmacist view all prescriptions
        [HttpGet]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // Customer can view their own prescription.
        // Admin and Pharmacist can view any prescription.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
                return NotFound(new { message = "Prescription not found." });

            bool isStaff =
                User.IsInRole("Admin") ||
                User.IsInRole("Pharmacist");

            if (!isStaff && result.UserId != GetUserId())
                return Forbid();

            return Ok(result);
        }

        // Admin and Pharmacist approve or reject
        [HttpPut("{id:int}/review")]
        [Authorize(Roles = "Admin,Pharmacist")]
        public async Task<IActionResult> Review(
            int id,
            [FromBody] ReviewPrescriptionDto dto)
        {
            try
            {
                bool updated = await _service.ReviewAsync(
                    id,
                    GetUserId(),
                    dto
                );

                if (!updated)
                    return NotFound(new
                    {
                        message = "Prescription not found."
                    });

                return Ok(new
                {
                    message = "Prescription reviewed successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Secure file viewing/downloading
        [HttpGet("{id:int}/file")]
        public async Task<IActionResult> GetFile(int id)
        {
            var prescription = await _service.GetByIdAsync(id);

            if (prescription == null)
                return NotFound();

            bool isStaff =
                User.IsInRole("Admin") ||
                User.IsInRole("Pharmacist");

            if (!isStaff && prescription.UserId != GetUserId())
                return Forbid();

            var file = await _service.GetFileAsync(id);

            if (file == null)
                return NotFound(new
                {
                    message = "Prescription file not found."
                });

            return PhysicalFile(
                file.Value.FilePath,
                file.Value.ContentType
            );
        }
    }
}

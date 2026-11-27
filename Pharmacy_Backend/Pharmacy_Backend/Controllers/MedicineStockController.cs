using Microsoft.AspNetCore.Mvc;
using Pharmacy_Backend.DTOs.Medicine;
using Pharmacy_Backend.Services.Interfaces;

namespace Pharmacy_Backend.Controllers;

[ApiController]
[Route("api/medicines")]
public class MedicineStockController : ControllerBase
{
    private readonly IMedicineService _service;

    public MedicineStockController(IMedicineService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get low-stock medicines below or equal to the specified threshold.
    /// Default threshold: 10
    /// </summary>
    /// <param name="threshold">Maximum stock quantity to consider as low stock. Default: 10</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of low-stock medicines with their status</returns>
    [HttpGet("stock/low")]
    public async Task<IActionResult> GetLowStock(
        [FromQuery] int threshold = 10,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _service.GetLowStockAsync(
                threshold, cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all out-of-stock medicines (stock quantity = 0).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of out-of-stock medicines</returns>
    [HttpGet("stock/out-of-stock")]
    public async Task<IActionResult> GetOutOfStock(
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetOutOfStockAsync(
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Update medicine stock with specified operation (Increase, Decrease, or Set).
    /// </summary>
    /// <param name="id">Medicine ID</param>
    /// <param name="dto">Stock update request with operation and quantity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated stock information with status</returns>
    [HttpPatch("{id:int}/stock")]
    public async Task<IActionResult> UpdateStock(
        int id,
        UpdateStockDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.UpdateStockAsync(
                id, dto, cancellationToken);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Medicine not found."
                });
            }

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}


using Microsoft.AspNetCore.Mvc;
using Pharmacy_Backend.DTOs.Medicine;
using Pharmacy_Backend.Services.Interfaces;

namespace Pharmacy_Backend.Controllers;

[ApiController]
[Route("api/medicines")]
public class MedicineController : ControllerBase
{
    private readonly IMedicineService _service;

    public MedicineController(IMedicineService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        var medicines = await _service.GetAllAsync(
            cancellationToken);

        return Ok(medicines);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var medicine = await _service.GetByIdAsync(
            id, cancellationToken);

        if (medicine == null)
        {
            return NotFound(new
            {
                message = "Medicine not found."
            });
        }

        return Ok(medicine);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateMedicineDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var medicine = await _service.CreateAsync(
                dto, cancellationToken);

            return CreatedAtAction(
                nameof(GetById),
                new { id = medicine.Id },
                medicine);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateMedicineDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var medicine = await _service.UpdateAsync(
                id, dto, cancellationToken);

            if (medicine == null)
            {
                return NotFound(new
                {
                    message = "Medicine not found."
                });
            }

            return Ok(medicine);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAsync(
            id, cancellationToken);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "Medicine not found."
            });
        }

        return Ok(new
        {
            message = "Medicine deleted successfully."
        });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] MedicineSearchDto dto,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _service.SearchAsync(
                dto, cancellationToken);

            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

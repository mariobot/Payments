using Hyip_Payments.Command.RecurringInvoiceCommand;
using Hyip_Payments.Models;
using Hyip_Payments.Query.RecurringInvoiceQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.RecurringInvoice;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RecurringInvoiceController : ControllerBase
{
    private readonly IMediator _mediator;

    public RecurringInvoiceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all recurring invoices
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<RecurringInvoiceModel>>> GetAll(
        [FromQuery] int? customerId,
        [FromQuery] bool? isActive,
        [FromQuery] string? frequency)
    {
        var query = new GetRecurringInvoiceListQuery
        {
            CustomerId = customerId,
            IsActive = isActive,
            Frequency = frequency
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get recurring invoice by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<RecurringInvoiceModel>> GetById(int id)
    {
        var query = new GetRecurringInvoiceByIdQuery { Id = id };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound(new { message = $"Recurring invoice with ID {id} not found" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Get recurring invoices that are due for generation
    /// </summary>
    [HttpGet("due")]
    public async Task<ActionResult<List<RecurringInvoiceModel>>> GetDue([FromQuery] DateTime? asOfDate)
    {
        var query = new GetRecurringInvoicesDueQuery { AsOfDate = asOfDate };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Create a new recurring invoice template
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<RecurringInvoiceModel>> Create([FromBody] CreateRecurringInvoiceDto dto)
    {
        try
        {         
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;

            var command = new AddRecurringInvoiceCommand
            {
                RecurringInvoice = dto,
                UserId = userId
            };

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating recurring invoice", error = ex.Message });
        }
    }

    /// <summary>
    /// Update an existing recurring invoice template
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<RecurringInvoiceModel>> Update(int id, [FromBody] CreateRecurringInvoiceDto dto)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;

            var command = new UpdateRecurringInvoiceCommand
            {
                Id = id,
                RecurringInvoice = dto,
                UserId = userId
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating recurring invoice", error = ex.Message });
        }
    }

    /// <summary>
    /// Delete a recurring invoice template
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteRecurringInvoiceCommand { Id = id };
        var success = await _mediator.Send(command);

        if (!success)
        {
            return NotFound(new { message = $"Recurring invoice with ID {id} not found" });
        }

        return NoContent();
    }

    /// <summary>
    /// Toggle active status of recurring invoice
    /// </summary>
    [HttpPatch("{id}/toggle-active")]
    public async Task<ActionResult> ToggleActive(int id, [FromBody] bool isActive)
    {
        var command = new ToggleRecurringInvoiceActiveCommand { Id = id, IsActive = isActive };
        var success = await _mediator.Send(command);

        if (!success)
        {
            return NotFound(new { message = $"Recurring invoice with ID {id} not found" });
        }

        return Ok(new { message = $"Recurring invoice {(isActive ? "activated" : "deactivated")} successfully" });
    }

    /// <summary>
    /// Manually generate an invoice from a recurring template
    /// </summary>
    [HttpPost("{id}/generate")]
    public async Task<ActionResult> GenerateInvoice(int id)
    {
        try
        {
            var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("userId")?.Value;

            var command = new ManualGenerateInvoiceCommand
            {
                RecurringInvoiceId = id,
                UserId = userId
            };

            var result = await _mediator.Send(command);
            return Ok(new
            {
                message = "Invoice generated successfully",
                invoiceId = result.InvoiceId,
                invoiceNumber = result.InvoiceNumber,
                totalAmount = result.TotalAmount
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error generating invoice", error = ex.Message });
        }
    }
}

using Hyip_Payments.Command.CustomerCommand;
using Hyip_Payments.Models;
using Hyip_Payments.Query.CustomerQuery;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Customer;

/// <summary>
/// API Controller for Customer management
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CustomerController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all customers
    /// </summary>
    /// <param name="isActive">Filter by active status (null = all)</param>
    /// <param name="searchTerm">Search term</param>
    /// <returns>List of customers</returns>
    [HttpGet]
    public async Task<ActionResult<List<CustomerModel>>> GetAll(
        [FromQuery] bool? isActive = null,
        [FromQuery] string? searchTerm = null)
    {
        var query = new GetCustomerListQuery
        {
            IsActive = isActive,
            SearchTerm = searchTerm,
            OrderBy = "CustomerNumber",
            OrderAscending = true
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get customer by ID
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="includeInvoices">Include invoices in response</param>
    /// <returns>Customer details</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerModel>> GetById(
        int id,
        [FromQuery] bool includeInvoices = false)
    {
        var query = new GetCustomerByIdQuery
        {
            Id = id,
            IncludeInvoices = includeInvoices,
            IncludeCountry = true
        };

        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound(new { message = $"Customer with ID {id} not found" });

        return Ok(result);
    }

    /// <summary>
    /// Create a new customer
    /// </summary>
    /// <param name="command">Customer creation data</param>
    /// <returns>Created customer ID</returns>
    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] AddCustomerCommand command)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var customerId = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { id = customerId }, new { id = customerId });
    }

    /// <summary>
    /// Update an existing customer
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="command">Customer update data</param>
    /// <returns>No content on success</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UpdateCustomerCommand command)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        command.Id = id;
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound(new { message = $"Customer with ID {id} not found" });

        return NoContent();
    }

    /// <summary>
    /// Delete a customer (or deactivate if has invoices)
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var command = new DeleteCustomerCommand { Id = id };
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound(new { message = $"Customer with ID {id} not found" });

        return NoContent();
    }

    /// <summary>
    /// Toggle customer active/inactive status
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id}/toggle-active")]
    public async Task<ActionResult> ToggleActive(int id)
    {
        var command = new ToggleCustomerActiveCommand { Id = id };
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound(new { message = $"Customer with ID {id} not found" });

        return NoContent();
    }

    /// <summary>
    /// Get all invoices for a customer
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <param name="status">Filter by invoice status</param>
    /// <returns>List of customer invoices</returns>
    [HttpGet("{id}/invoices")]
    public async Task<ActionResult<List<InvoiceModel>>> GetCustomerInvoices(
        int id,
        [FromQuery] string? status = null)
    {
        var query = new GetCustomerInvoicesQuery
        {
            CustomerId = id,
            StatusFilter = status
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get customer statistics (invoices, revenue, balance)
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>Customer statistics</returns>
    [HttpGet("{id}/stats")]
    public async Task<ActionResult<CustomerStatsDto>> GetCustomerStats(int id)
    {
        var query = new GetCustomerStatsQuery { CustomerId = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update customer balance (recalculate from invoices)
    /// </summary>
    /// <param name="id">Customer ID</param>
    /// <returns>No content on success</returns>
    [HttpPost("{id}/update-balance")]
    public async Task<ActionResult> UpdateBalance(int id)
    {
        var command = new UpdateCustomerBalanceCommand { CustomerId = id };
        var result = await _mediator.Send(command);

        if (!result)
            return NotFound(new { message = $"Customer with ID {id} not found" });

        return NoContent();
    }
}

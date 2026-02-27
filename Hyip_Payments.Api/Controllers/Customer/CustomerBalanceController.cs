using Hyip_Payments.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyip_Payments.Api.Controllers.Customer;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CustomerBalanceController : ControllerBase
{
    private readonly ICustomerBalanceService _balanceService;

    public CustomerBalanceController(ICustomerBalanceService balanceService)
    {
        _balanceService = balanceService;
    }

    /// <summary>
    /// Manually update a specific customer's balance
    /// </summary>
    /// <param name="customerId">Customer ID</param>
    [HttpPost("update/{customerId}")]
    public async Task<IActionResult> UpdateCustomerBalance(int customerId)
    {
        await _balanceService.UpdateCustomerBalanceAsync(customerId);
        return Ok(new { Message = $"Customer {customerId} balance updated successfully" });
    }

    /// <summary>
    /// Recalculate all customer balances
    /// Use for data reconciliation or maintenance
    /// </summary>
    [HttpPost("recalculate-all")]
    [Authorize(Roles = "Admin")] // Only admins can recalculate all
    public async Task<IActionResult> RecalculateAllBalances()
    {
        await _balanceService.RecalculateAllCustomerBalancesAsync();
        return Ok(new { Message = "All customer balances recalculated successfully" });
    }

    /// <summary>
    /// Update balances for multiple customers
    /// </summary>
    /// <param name="customerIds">List of customer IDs</param>
    [HttpPost("update-multiple")]
    public async Task<IActionResult> UpdateMultipleBalances([FromBody] List<int> customerIds)
    {
        await _balanceService.UpdateMultipleCustomerBalancesAsync(customerIds);
        return Ok(new { Message = $"{customerIds.Count} customer balances updated successfully" });
    }
}

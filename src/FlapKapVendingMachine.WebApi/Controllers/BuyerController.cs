// WebApi/Controllers/BuyerController.cs
using FlapKapVendingMachine.Application.Common.Interfaces;
using FlapKapVendingMachine.Domain.Entities;
using FlapKapVendingMachine.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlapKapVendingMachine.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Buyer")]
public class BuyerController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public BuyerController(IApplicationDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromQuery] int coin)
    {
        if (!Enum.IsDefined(typeof(CoinValue), coin))
            return BadRequest("Allowed coins: 5, 10, 20, 50, 100 cents");

        var user = await _context.Users.FindAsync(GetCurrentUserId());
        if (user == null) return Unauthorized();

        user.Deposit += coin;
        await _context.SaveChangesAsync();

        return Ok(new { deposit = user.Deposit });
    }

    [HttpPost("buy")]
    public async Task<IActionResult> Buy([FromQuery] int productId, [FromQuery] int quantity)
    {
        if (quantity <= 0) return BadRequest("Quantity must be greater than 0");

        var user = await _context.Users.FindAsync(GetCurrentUserId());
        if (user == null) return Unauthorized();

        var product = await _context.Products.FindAsync(productId);
        if (product == null) return NotFound("Product not found");

        if (product.AmountAvailable < quantity)
            return BadRequest("Not enough stock available");

        var totalCost = product.Cost * quantity;
        if (user.Deposit < totalCost)
            return BadRequest("Insufficient funds");

        // Deduct deposit
        user.Deposit -= totalCost;

        // Reduce stock
        product.AmountAvailable -= quantity;

        // Save changes
        await _context.SaveChangesAsync();

        // Calculate change in coins
        var change = CalculateChange(user.Deposit);

        // Reset deposit after purchase
        user.Deposit = 0;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            totalSpent = totalCost,
            productPurchased = product.ProductName,
            quantity,
            change
        });
    }

    [HttpPost("reset")]
    public async Task<IActionResult> Reset()
    {
        var user = await _context.Users.FindAsync(GetCurrentUserId());
        if (user == null) return Unauthorized();

        user.Deposit = 0;
        await _context.SaveChangesAsync();

        return Ok(new { deposit = 0 });
    }

    private List<int> CalculateChange(int amount)
    {
        var coins = new List<int>();
        int[] values = { 100, 50, 20, 10, 5 };

        foreach (var coin in values)
        {
            while (amount >= coin)
            {
                amount -= coin;
                coins.Add(coin);
            }
        }

        return coins;
    }
}

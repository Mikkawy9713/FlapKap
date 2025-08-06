// WebApi/Controllers/ProductController.cs
using FlapKapVendingMachine.Application.Common.Interfaces;
using FlapKapVendingMachine.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlapKapVendingMachine.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public ProductController(IApplicationDbContext context)
    {
        _context = context;
    }

    private int GetCurrentUserId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    }

    // ✅ GET - Public (anyone can see products)
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetProducts()
    {
        var products = _context.Products.ToList();
        return Ok(products);
    }

    // ✅ POST - Seller only
    [HttpPost]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        product.SellerId = GetCurrentUserId();

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
    }

    // ✅ PUT - Seller can only update their own products
    [HttpPut("{id}")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound("Product not found");

        if (product.SellerId != GetCurrentUserId())
            return Forbid("You can only update your own products");

        product.ProductName = updatedProduct.ProductName;
        product.AmountAvailable = updatedProduct.AmountAvailable;
        product.Cost = updatedProduct.Cost;

        await _context.SaveChangesAsync();
        return Ok(product);
    }

    // ✅ DELETE - Seller can only delete their own products
    [HttpDelete("{id}")]
    [Authorize(Roles = "Seller")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound("Product not found");

        if (product.SellerId != GetCurrentUserId())
            return Forbid("You can only delete your own products");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

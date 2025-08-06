// WebApi/Controllers/AuthController.cs
using FlapKapVendingMachine.Application.Common.Interfaces;
using FlapKapVendingMachine.Domain.Entities;
using FlapKapVendingMachine.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace FlapKapVendingMachine.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenService _jwtService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthController(IPasswordHasher<User> passwordHasher, IApplicationDbContext context, IJwtTokenService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(string username, string password, string role = "Buyer")
    {
        var user = new User
        {
            Username = username,
            Role = role
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("User registered successfully");
    }

    [HttpPost("login")]
    public IActionResult Login(string username, string password)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == username);
        if (user == null) return Unauthorized("Invalid username or password");

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed) return Unauthorized("Invalid username or password");

        var token = _jwtService.GenerateToken(user.Id, user.Username, user.Role);
        return Ok(new { token });
    }
}

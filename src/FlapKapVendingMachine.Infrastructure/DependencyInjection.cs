// Infrastructure/DependencyInjection.cs
using FlapKapVendingMachine.Infrastructure.Persistence;
using FlapKapVendingMachine.Infrastructure.Services;
using FlapKapVendingMachine.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FlapKapVendingMachine.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FlapKapVendingMachine.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        // JWT Service
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        return services;
    }
}

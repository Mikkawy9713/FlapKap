using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Linq;
using FlapKapVendingMachine.Infrastructure.Persistence;
using FlapKapVendingMachine.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;

namespace FlapKapVendingMachine.WebApi.Tests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        private SqliteConnection _connection;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Test");

            // ✅ Create and keep open the SQLite in-memory connection
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            builder.ConfigureServices(services =>
            {
                // Register hasher for tests
                services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

                // Remove existing DbContext registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Register in-memory SQLite DbContext
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                db.Database.EnsureCreated();

                // ✅ Pass the service provider so SeedTestData can resolve IPasswordHasher<User>
                SeedTestData(db, scope.ServiceProvider);
            });

        }

        private static void SeedTestData(ApplicationDbContext db, IServiceProvider sp)
        {
            var passwordHasher = sp.GetRequiredService<IPasswordHasher<User>>();

            var seller = new User
            {
                Id = 1,
                Username = "seller11",
                Role = "Seller",
                Deposit = 0
            };

            seller.PasswordHash = passwordHasher.HashPassword(seller, "123");
            Console.WriteLine($"[DEBUG] Seeded seller1 hash: {seller.PasswordHash}");

            var buyer = new User
            {
                Id = 2,
                Username = "buyer11",
                Role = "Buyer",
                Deposit = 0
            };
            buyer.PasswordHash = passwordHasher.HashPassword(buyer, "123");

            
            
                db.Users.AddRange(seller, buyer);
            

            if (!db.Products.Any())
            {
                db.Products.AddRange(
                    new Product { Id = 1, ProductName = "Coke", AmountAvailable = 10, Cost = 50, SellerId = 1 },
                    new Product { Id = 2, ProductName = "Pepsi", AmountAvailable = 15, Cost = 60, SellerId = 1 }
                );
            }

            db.SaveChanges();
        }


    }
}

using FlapKapVendingMachine.Application.Common.Interfaces;
using FlapKapVendingMachine.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlapKapVendingMachine.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Precomputed password hashes for "123"
        // Generated once using PasswordHasher<User>
        const string sellerPasswordHash = "AQAAAAIAAYagAAAAEIzfN++U3dsfz0j1sH2CSiZf5W6cfjET3y5E86gKjoKJhY6NQXSh42J37zzm2I/fuQ==";
        const string buyerPasswordHash = "AQAAAAIAAYagAAAAEIzfN++U3dsfz0j1sH2CSiZf5W6cfjET3y5E86gKjoKJhY6NQXSh42J37zzm2I/fuQ==";

        // Seller
        var seller = new User
        {
            Id = 6,
            Username = "seller1",
            Role = "Seller",
            Deposit = 0,
            PasswordHash = sellerPasswordHash
        };

        // Buyer
        var buyer = new User
        {
            Id = 5,
            Username = "buyer1",
            Role = "Buyer",
            Deposit = 0,
            PasswordHash = buyerPasswordHash
        };

        // Products for seller1
        var coke = new Product
        {
            Id = 1,
            ProductName = "Coke",
            AmountAvailable = 10,
            Cost = 50,
            SellerId = 1
        };

        var pepsi = new Product
        {
            Id = 2,
            ProductName = "Pepsi",
            AmountAvailable = 15,
            Cost = 60,
            SellerId = 1
        };

        // Seed data
        modelBuilder.Entity<User>().HasData(seller, buyer);
        modelBuilder.Entity<Product>().HasData(coke, pepsi);
    }
}

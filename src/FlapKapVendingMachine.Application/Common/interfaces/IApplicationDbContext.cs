// Application/Common/Interfaces/IApplicationDbContext.cs
using FlapKapVendingMachine.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace FlapKapVendingMachine.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Product> Products { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

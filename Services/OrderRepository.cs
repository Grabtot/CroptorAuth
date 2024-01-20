using CroptorAuth.Data;
using CroptorAuth.Models;
using Microsoft.EntityFrameworkCore;

namespace CroptorAuth.Services;

public class OrderRepository(ApplicationDbContext context)
{
    private readonly DbSet<Order> _dbSet = context.Set<Order>();

    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(order, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Order> GetOrderAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync([id], cancellationToken)
               ?? throw new InvalidOperationException($"Could not find {id}");
    }

    public async Task DeleteOrderAsync(Order order, CancellationToken cancellationToken = default)
    {
        _dbSet.Remove(order);
        await context.SaveChangesAsync(cancellationToken);
    }
}
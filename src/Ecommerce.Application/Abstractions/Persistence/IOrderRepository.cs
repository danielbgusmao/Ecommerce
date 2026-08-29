using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Abstractions.Persistence;

public interface IOrderRepository
{
    Task AddAsync(
        Order order,
        CancellationToken cancellationToken = default);
    
    Task<Order?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
}
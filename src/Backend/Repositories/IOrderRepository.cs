using Backend.Models;

namespace Backend.Repositories;

public interface IOrderRepository : IRepository<Order>
{
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<bool> UpdateStatusAsync(string id, string status);
}

using Backend.Models;
using MongoDB.Driver;

namespace Backend.Repositories;

public class OrderRepository : MongoRepository<Order>, IOrderRepository
{
    public OrderRepository(IMongoDatabase database) : base(database, "orders")
    {
        // Criar índice único para orderNumber
        var indexKeysDefinition = Builders<Order>.IndexKeys.Ascending(o => o.OrderNumber);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<Order>(indexKeysDefinition, indexOptions);
        _collection.Indexes.CreateOneAsync(indexModel);
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.OrderNumber, orderNumber);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateStatusAsync(string id, string status)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
        var update = Builders<Order>.Update
            .Set(o => o.Status, status)
            .Set(o => o.UpdatedAt, DateTime.UtcNow);
        var result = await _collection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }
}

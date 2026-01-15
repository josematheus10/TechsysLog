using Backend.Models;
using MongoDB.Driver;

namespace Backend.Data;

public interface IOrderStore
{
    Task<Order> CreateAsync(Order order);
    Task<Order?> GetByIdAsync(string id);
    Task<Order?> GetByOrderNumberAsync(string orderNumber);
    Task<List<Order>> GetByUserIdAsync(string userId);
    Task<List<Order>> GetAllAsync();
    Task<bool> UpdateAsync(Order order);
    Task<bool> UpdateStatusAsync(string id, string status);
    Task<bool> DeleteAsync(string id);
}

public class MongoOrderStore : IOrderStore
{
    private readonly IMongoCollection<Order> _ordersCollection;

    public MongoOrderStore(IMongoDatabase database)
    {
        _ordersCollection = database.GetCollection<Order>("orders");
        
        // Criar índice único para orderNumber
        var indexKeysDefinition = Builders<Order>.IndexKeys.Ascending(o => o.OrderNumber);
        var indexOptions = new CreateIndexOptions { Unique = true };
        var indexModel = new CreateIndexModel<Order>(indexKeysDefinition, indexOptions);
        _ordersCollection.Indexes.CreateOneAsync(indexModel);
    }

    public async Task<Order> CreateAsync(Order order)
    {
        await _ordersCollection.InsertOneAsync(order);
        return order;
    }

    public async Task<Order?> GetByIdAsync(string id)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
        return await _ordersCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.OrderNumber, orderNumber);
        return await _ordersCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<Order>> GetByUserIdAsync(string userId)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.UserId, userId);
        return await _ordersCollection.Find(filter).ToListAsync();
    }

    public async Task<List<Order>> GetAllAsync()
    {
        return await _ordersCollection.Find(_ => true).ToListAsync();
    }

    public async Task<bool> UpdateAsync(Order order)
    {
        order.UpdatedAt = DateTime.UtcNow;
        var filter = Builders<Order>.Filter.Eq(o => o.Id, order.Id);
        var result = await _ordersCollection.ReplaceOneAsync(filter, order);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> UpdateStatusAsync(string id, string status)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
        var update = Builders<Order>.Update
            .Set(o => o.Status, status)
            .Set(o => o.UpdatedAt, DateTime.UtcNow);
        var result = await _ordersCollection.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.Id, id);
        var result = await _ordersCollection.DeleteOneAsync(filter);
        return result.DeletedCount > 0;
    }
}

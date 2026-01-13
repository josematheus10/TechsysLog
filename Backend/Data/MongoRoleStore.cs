using Backend.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Backend.Data;

public class MongoRoleStore : IRoleStore<ApplicationRole>
{
    private readonly IMongoCollection<ApplicationRole> _rolesCollection;

    public MongoRoleStore(IMongoDatabase database)
    {
        _rolesCollection = database.GetCollection<ApplicationRole>("roles");
    }

    public void Dispose() { }

    public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        role.Id = Guid.NewGuid().ToString();
        await _rolesCollection.InsertOneAsync(role, cancellationToken: cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationRole>.Filter.Eq(r => r.Id, role.Id);
        await _rolesCollection.ReplaceOneAsync(filter, role, cancellationToken: cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationRole>.Filter.Eq(r => r.Id, role.Id);
        await _rolesCollection.DeleteOneAsync(filter, cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        return role.Id;
    }

    public async Task<string?> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        return role.Name;
    }

    public async Task SetRoleNameAsync(ApplicationRole role, string? roleName, CancellationToken cancellationToken)
    {
        role.Name = roleName;
    }

    public async Task<string?> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        return role.NormalizedName;
    }

    public async Task SetNormalizedRoleNameAsync(ApplicationRole role, string? normalizedName, CancellationToken cancellationToken)
    {
        role.NormalizedName = normalizedName;
    }

    public async Task<ApplicationRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationRole>.Filter.Eq(r => r.Id, roleId);
        return await _rolesCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ApplicationRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationRole>.Filter.Eq(r => r.NormalizedName, normalizedRoleName);
        return await _rolesCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }
}

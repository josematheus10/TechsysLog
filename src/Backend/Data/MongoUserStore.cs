using Backend.Models;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace Backend.Data;

public class MongoUserStore : IUserStore<ApplicationUser>, 
    IUserEmailStore<ApplicationUser>,
    IUserPhoneNumberStore<ApplicationUser>,
    IUserTwoFactorStore<ApplicationUser>,
    IUserPasswordStore<ApplicationUser>,
    IUserRoleStore<ApplicationUser>,
    IUserLoginStore<ApplicationUser>,
    IUserSecurityStampStore<ApplicationUser>,
    IUserClaimStore<ApplicationUser>,
    IUserLockoutStore<ApplicationUser>
{
    private readonly IMongoCollection<ApplicationUser> _usersCollection;

    public MongoUserStore(IMongoDatabase database)
    {
        _usersCollection = database.GetCollection<ApplicationUser>("users");
    }

    public void Dispose() { }

    public async Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.Id;
    }

    public async Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.UserName;
    }

    public async Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
    }

    public async Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.NormalizedUserName;
    }

    public async Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
    }

    public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        user.Id = Guid.NewGuid().ToString();
        await _usersCollection.InsertOneAsync(user, cancellationToken: cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.Eq(u => u.Id, user.Id);
        await _usersCollection.ReplaceOneAsync(filter, user, cancellationToken: cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.Eq(u => u.Id, user.Id);
        await _usersCollection.DeleteOneAsync(filter, cancellationToken);
        return IdentityResult.Success;
    }

    public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.Eq(u => u.Id, userId);
        return await _usersCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.Eq(u => u.NormalizedUserName, normalizedUserName);
        return await _usersCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    // IUserEmailStore
    public async Task SetEmailAsync(ApplicationUser user, string? email, CancellationToken cancellationToken)
    {
        user.Email = email;
    }

    public async Task<string?> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.Email;
    }

    public async Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.EmailConfirmed;
    }

    public async Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.Eq(u => u.NormalizedEmail, normalizedEmail);
        return await _usersCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.NormalizedEmail;
    }

    public async Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        user.NormalizedEmail = normalizedEmail;
    }

    // IUserPhoneNumberStore
    public async Task SetPhoneNumberAsync(ApplicationUser user, string? phoneNumber, CancellationToken cancellationToken)
    {
        user.PhoneNumber = phoneNumber;
    }

    public async Task<string?> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.PhoneNumber;
    }

    public async Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.PhoneNumberConfirmed;
    }

    public async Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
    {
        user.PhoneNumberConfirmed = confirmed;
    }

    // IUserTwoFactorStore
    public async Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
    {
        user.TwoFactorEnabled = enabled;
    }

    public async Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.TwoFactorEnabled;
    }

    // IUserPasswordStore
    public async Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
    }

    public async Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.PasswordHash;
    }

    public async Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return !string.IsNullOrEmpty(user.PasswordHash);
    }

    // IUserRoleStore
    public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        if (user.Roles == null)
            user.Roles = new List<string>();
        
        if (!user.Roles.Contains(roleName))
            user.Roles.Add(roleName);
    }

    public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        user.Roles?.Remove(roleName);
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.Roles ?? new List<string>();
    }

    public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        return user.Roles?.Contains(roleName) ?? false;
    }

    public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.AnyEq(u => u.Roles, roleName);
        return await _usersCollection.Find(filter).ToListAsync(cancellationToken);
    }

    // IUserSecurityStampStore
    public async Task SetSecurityStampAsync(ApplicationUser user, string stamp, CancellationToken cancellationToken)
    {
        user.SecurityStamp = stamp;
    }

    public async Task<string?> GetSecurityStampAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.SecurityStamp;
    }

    // IUserLoginStore
    public async Task AddLoginAsync(ApplicationUser user, UserLoginInfo login, CancellationToken cancellationToken)
    {
        if (user.Logins == null)
            user.Logins = new List<IdentityUserLogin<string>>();

        user.Logins.Add(new IdentityUserLogin<string>
        {
            LoginProvider = login.LoginProvider,
            ProviderKey = login.ProviderKey,
            ProviderDisplayName = login.ProviderDisplayName
        });
    }

    public async Task RemoveLoginAsync(ApplicationUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        user.Logins?.RemoveAll(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
    }

    public async Task<IList<UserLoginInfo>> GetLoginsAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.Logins?.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.ProviderDisplayName)).ToList() 
            ?? new List<UserLoginInfo>();
    }

    public async Task<ApplicationUser?> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.ElemMatch(u => u.Logins,
            Builders<IdentityUserLogin<string>>.Filter.And(
                Builders<IdentityUserLogin<string>>.Filter.Eq(l => l.LoginProvider, loginProvider),
                Builders<IdentityUserLogin<string>>.Filter.Eq(l => l.ProviderKey, providerKey)
            ));
        return await _usersCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    // IUserClaimStore
    public async Task<IList<System.Security.Claims.Claim>> GetClaimsAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.Claims?.Select(c => new System.Security.Claims.Claim(c.ClaimType!, c.ClaimValue!)).ToList() 
            ?? new List<System.Security.Claims.Claim>();
    }

    public async Task AddClaimsAsync(ApplicationUser user, IEnumerable<System.Security.Claims.Claim> claims, CancellationToken cancellationToken)
    {
        if (user.Claims == null)
            user.Claims = new List<IdentityUserClaim<string>>();

        foreach (var claim in claims)
        {
            user.Claims.Add(new IdentityUserClaim<string>
            {
                ClaimType = claim.Type,
                ClaimValue = claim.Value
            });
        }
    }

    public async Task ReplaceClaimAsync(ApplicationUser user, System.Security.Claims.Claim claim, System.Security.Claims.Claim newClaim, CancellationToken cancellationToken)
    {
        var matchedClaims = user.Claims?.Where(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value).ToList();
        if (matchedClaims != null)
        {
            foreach (var matchedClaim in matchedClaims)
            {
                matchedClaim.ClaimType = newClaim.Type;
                matchedClaim.ClaimValue = newClaim.Value;
            }
        }
    }

    public async Task RemoveClaimsAsync(ApplicationUser user, IEnumerable<System.Security.Claims.Claim> claims, CancellationToken cancellationToken)
    {
        foreach (var claim in claims)
        {
            user.Claims?.RemoveAll(c => c.ClaimType == claim.Type && c.ClaimValue == claim.Value);
        }
    }

    public async Task<IList<ApplicationUser>> GetUsersForClaimAsync(System.Security.Claims.Claim claim, CancellationToken cancellationToken)
    {
        var filter = Builders<ApplicationUser>.Filter.ElemMatch(u => u.Claims,
            Builders<IdentityUserClaim<string>>.Filter.And(
                Builders<IdentityUserClaim<string>>.Filter.Eq(c => c.ClaimType, claim.Type),
                Builders<IdentityUserClaim<string>>.Filter.Eq(c => c.ClaimValue, claim.Value)
            ));
        return await _usersCollection.Find(filter).ToListAsync(cancellationToken);
    }

    // IUserLockoutStore
    public async Task<DateTimeOffset?> GetLockoutEndDateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.LockoutEnd;
    }

    public async Task SetLockoutEndDateAsync(ApplicationUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
    {
        user.LockoutEnd = lockoutEnd;
    }

    public async Task<int> IncrementAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount++;
        return user.AccessFailedCount;
    }

    public async Task ResetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        user.AccessFailedCount = 0;
    }

    public async Task<int> GetAccessFailedCountAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.AccessFailedCount;
    }

    public async Task<bool> GetLockoutEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return user.LockoutEnabled;
    }

    public async Task SetLockoutEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
    {
        user.LockoutEnabled = enabled;
    }
}

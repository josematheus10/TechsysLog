using Microsoft.AspNetCore.Identity;

namespace Backend.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public List<string>? Roles { get; set; }
    public List<IdentityUserLogin<string>>? Logins { get; set; }
    public List<IdentityUserClaim<string>>? Claims { get; set; }
}

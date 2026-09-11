using Microsoft.AspNetCore.Identity;

namespace VoltajeModa.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string Nombre { get; set; } = string.Empty;
}


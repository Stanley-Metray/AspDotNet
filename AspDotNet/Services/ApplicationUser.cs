using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    // You can add custom properties for your user here
    public bool Has2FAEnabled { get; set; }
}

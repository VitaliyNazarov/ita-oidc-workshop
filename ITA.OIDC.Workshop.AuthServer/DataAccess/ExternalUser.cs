using Microsoft.AspNetCore.Identity;

namespace ITA.OIDC.Workshop.AuthServer.DataAccess;

public sealed class ExternalUser : IdentityUser
{
    public string? FullName { get; set; }
        
    public string? Position { get; set; }

    public string[] Roles { get; set; } = Array.Empty<string>();
}
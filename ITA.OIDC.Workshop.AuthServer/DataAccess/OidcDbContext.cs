using Microsoft.EntityFrameworkCore;
using OpenIddict.EntityFrameworkCore.Models;

namespace ITA.OIDC.Workshop.AuthServer.DataAccess;

public class OidcDbContext : DbContext
{
    public const string DefaultContextKey = "ITAOIDCDB";

    public OidcDbContext(DbContextOptions<OidcDbContext> options)
        : base(options)
    {
    }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OpenIddictEntityFrameworkCoreApplication>().ToTable("oidc_applications");
        modelBuilder.Entity<OpenIddictEntityFrameworkCoreAuthorization>().ToTable("oidc_authorizations");
        modelBuilder.Entity<OpenIddictEntityFrameworkCoreScope>().ToTable("oidc_scopes");
        modelBuilder.Entity<OpenIddictEntityFrameworkCoreToken>().ToTable("oidc_tokens");
    }
}
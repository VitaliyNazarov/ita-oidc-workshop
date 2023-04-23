using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ITA.OIDC.Workshop.AuthServer.DataAccess
{
    internal sealed class OidcDbDesignTimeContextFactory : IDesignTimeDbContextFactory<OidcDbContext>
    {
        #region Implementation of IDesignTimeDbContextFactory<out OidcDbContext>

        public OidcDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            var connectionString = configuration[OidcDbContext.DefaultContextKey];
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($"Command line connection string parameter '{OidcDbContext.DefaultContextKey}' is undefined.");
            }
            
            Console.WriteLine($"ConnectionString {OidcDbContext.DefaultContextKey}: {connectionString}");

            var optionsBuilder = new DbContextOptionsBuilder<OidcDbContext>();
            
            optionsBuilder.SetUpPgContext(connectionString);

            return new OidcDbContext(optionsBuilder.Options);
        }

        #endregion
    }
}
using Microsoft.EntityFrameworkCore;

namespace ITA.OIDC.Workshop.AuthServer.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection AddItaDataAccess(this IServiceCollection services)
        {
            //services.AddDbContext<OidcDbContext>();
            services.AddDbContextFactory<OidcDbContext>((provider, options) =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("ITAOIDCDB");
                options.SetUpPgContext(connectionString);
            });

            return services;
        }
        
        public static DbContextOptionsBuilder SetUpPgContext(
            this DbContextOptionsBuilder builder,
            string connectionString)
        {
            builder
                .EnableDetailedErrors()
                .UseNpgsql(
                    connectionString,
                    options => options.SetPostgresVersion(new Version(9, 6, 23)))
                .UseSnakeCaseNamingConvention()
                .UseOpenIddict();

            return builder;
        }
    }
}
using ACSAppBox.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ACSAppBox.SqlServerMigrations
{
    internal class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = 
                new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(Directory.GetCurrentDirectory() + "/../ACSAppBox/appsettings.Development.json")
                .Build();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("SqlServerConnectionString");
            builder.UseSqlServer(
                connectionString,
                x => x.MigrationsAssembly("ACSAppBox.SqlServerMigrations"));
            return new ApplicationDbContext(builder.Options, new OperationalStoreOptionsMigrations());
        }
    }
}

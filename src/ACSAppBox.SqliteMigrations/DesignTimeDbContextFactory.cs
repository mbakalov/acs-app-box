using ACSAppBox.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ACSAppBox.SqliteMigrations
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
            var connectionString = configuration.GetConnectionString("SqliteConnectionString");
            builder.UseSqlite(
                connectionString,
                x => x.MigrationsAssembly("ACSAppBox.SqliteMigrations"));
            return new ApplicationDbContext(builder.Options, new OperationalStoreOptionsMigrations());
        }
    }
}

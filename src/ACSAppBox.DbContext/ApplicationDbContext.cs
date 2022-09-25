using ACSAppBox.Models;
using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ACSAppBox.Data
{
    public class ApplicationDbContext : ApiAuthorizationDbContext<ApplicationUser>, IDataProtectionKeyContext
    {
        public DbSet<Room> Rooms { get; set; }

        public DbSet<ApplicationSetting> ApplicationSettings { get; set; }

        public ApplicationSetting? MySettings
        {
            get
            {
                return ApplicationSettings.FirstOrDefault();
            }
        }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IOptions<OperationalStoreOptions> operationalStoreOptions)
            : base(options, operationalStoreOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
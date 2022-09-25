using ACSAppBox.Data;
using ACSAppBox.Models;
using Azure.Communication.Identity;
using Microsoft.EntityFrameworkCore;

namespace ACSAppBox
{
    public class AdminUserProvider
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly CommunicationIdentityClient _communicationIdentityClient;

        private string? _adminUserId;

        public AdminUserProvider(
            IDbContextFactory<ApplicationDbContext> dbContextFactory,
            CommunicationIdentityClient communicationIdentityClient)
        {
            _dbContextFactory = dbContextFactory;
            _communicationIdentityClient = communicationIdentityClient;

            _adminUserId = null;
        }

        public string GetAdminUserId()
        {
            if (_adminUserId == null)
            {
                using var dbContext = _dbContextFactory.CreateDbContext();

                var existingSettings = dbContext.MySettings;
                if (existingSettings != null)
                {
                    _adminUserId = existingSettings.CommunicationAdminUserId;
                }
                else
                {
                    string newAcsUser = _communicationIdentityClient.CreateUser().Value.Id;
                    var newSettings = new ApplicationSetting { CommunicationAdminUserId = newAcsUser };
                    dbContext.ApplicationSettings.Add(newSettings);
                    dbContext.SaveChanges();

                    _adminUserId = newAcsUser;
                }
            }

            return _adminUserId;
        }
    }
}

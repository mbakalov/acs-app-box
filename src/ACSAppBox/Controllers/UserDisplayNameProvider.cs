using ACSAppBox.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace ACSAppBox.Controllers
{
    public class UserDisplayNameProvider
    {
        public static async Task<string> GetDisplayName(UserManager<ApplicationUser> userManager, ApplicationUser user)
        {
            var claims = await userManager.GetClaimsAsync(user);

            var displayNameClaim = claims.FirstOrDefault(c => c.Type == JwtClaimTypes.Name);
            if (displayNameClaim != null)
            {
                return displayNameClaim.Value;
            }
            else
            {
                return user.UserName;
            }
        }
    }
}

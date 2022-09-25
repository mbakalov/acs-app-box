using ACSAppBox.Models;
using Azure.Communication.Identity;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ACSAppBox.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CommunicationIdentityClient _communicationIdentityClient;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            CommunicationIdentityClient communicationIdentityClient,
            ILogger<UsersController> logger)
        {
            _userManager = userManager;
            _communicationIdentityClient = communicationIdentityClient;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<UserDetailsDto>> Get()
        {
            var allUsers = await _userManager.Users.ToListAsync();

            var result = new List<UserDetailsDto>();
            foreach (var appUser in allUsers)
            {
                string displayName = await UserDisplayNameProvider.GetDisplayName(_userManager, appUser);
                
                result.Add(new UserDetailsDto
                {
                    Id = appUser.Id,
                    Name = displayName,
                    Email = appUser.Email,
                    CommunicationUserId = appUser.CommunicationUserId
                });
            }

            return result;
        }

        [HttpGet("{userId}/token")]
        public async Task<ActionResult<AccessToken>> GetToken(string userId)
        {
            var appUser = _userManager.Users.Where(x => x.Id == userId).FirstOrDefault();

            if (appUser == null)
            {
                return NotFound();
            }

            var tokenResult = await _communicationIdentityClient.GetTokenAsync(
                new Azure.Communication.CommunicationUserIdentifier(appUser.CommunicationUserId),
                new[] { CommunicationTokenScope.Chat, CommunicationTokenScope.VoIP });

            var token = tokenResult.Value;
            return Ok(token);
        }
    }
}

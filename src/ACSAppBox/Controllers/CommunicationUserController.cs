using ACSAppBox.Models;
using Azure.Communication;
using Azure.Communication.Identity;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ACSAppBox.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CommunicationUserController : ControllerBase
    {
        private readonly ILogger<CommunicationUserController> _logger;
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly CommunicationIdentityClient _communicationIdentityClient;

        public CommunicationUserController(
            ILogger<CommunicationUserController> logger,
            UserManager<ApplicationUser> userManager,
            CommunicationIdentityClient communicationIdentityClient)
        {
            _logger = logger;
            _userManger = userManager;
            _communicationIdentityClient = communicationIdentityClient;
        }

        [HttpGet("token")]
        public async Task<CommunicationUserTokenDto> GetToken()
        {
            var currentUser = await _userManger.GetUserAsync(User);
            var userId = new CommunicationUserIdentifier(currentUser.CommunicationUserId);

            var tokenResult = (await _communicationIdentityClient.GetTokenAsync(
                userId,
                new[] { CommunicationTokenScope.Chat, CommunicationTokenScope.VoIP })).Value;

            return new CommunicationUserTokenDto
            {
                Token = tokenResult.Token,
                ExpiresOn = tokenResult.ExpiresOn,
                UserId = currentUser.CommunicationUserId
            };
        }
    }
}

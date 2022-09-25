using Azure.Communication;
using ACSAppBox.Models;

namespace ACSAppBox.Controllers
{
    public class JoinRoomDto
    {
        public Room Room { get; set; }
        public CommunicationUserIdentifier UserId { get; set; }
        public string AccessToken { get; set; }
        public string Endpoint { get; set; }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Azure.Communication;
using Azure.Communication.Chat;
using Azure.Communication.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ACSAppBox.Data;
using ACSAppBox.Models;
using IdentityModel;

namespace ACSAppBox.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public partial class RoomsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CommunicationUserController> _logger;
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly CommunicationIdentityClient _communicationIdentityClient;
        private readonly AdminUserProvider _adminUserProvider;
        private readonly string _endpoint;

        public RoomsController(ApplicationDbContext context,
            ILogger<CommunicationUserController> logger,
            UserManager<ApplicationUser> userManager,
            CommunicationIdentityClient communicationIdentityClient,
            AdminUserProvider adminUserProvider,
            string endpoint)
        {
            _context = context;
            _logger = logger;
            _userManger = userManager;
            _communicationIdentityClient = communicationIdentityClient;
            _adminUserProvider = adminUserProvider;
            _endpoint =  endpoint;
        }

        [Authorize]
        // POST: api/Rooms
        [HttpPost]
        public async Task<ActionResult<Room>> CreateRoom()
        {
            Uri endpoint = new Uri(_endpoint);
            var currentUser = await _userManger.GetUserAsync(User);
            var userId = new CommunicationUserIdentifier(currentUser.CommunicationUserId);
            var adminId = new CommunicationUserIdentifier(_adminUserProvider.GetAdminUserId());

            var adminTokenResult = (await _communicationIdentityClient.GetTokenAsync(
                adminId,
                new[] { CommunicationTokenScope.Chat, CommunicationTokenScope.VoIP })).Value;

            CommunicationTokenCredential communicationTokenCredential = new CommunicationTokenCredential(adminTokenResult.Token);
            ChatClient chatClient = new ChatClient(endpoint, communicationTokenCredential);

            string displayName = await UserDisplayNameProvider.GetDisplayName(_userManger, currentUser);

            var chatParticipant = new ChatParticipant(identifier: userId) 
            {
                DisplayName = displayName
            };

            CreateChatThreadResult createChatThreadResult = await chatClient.CreateChatThreadAsync(topic: "Test Thread", participants: new[] {chatParticipant});
            string threadId = createChatThreadResult.ChatThread.Id;

            Guid groupId = Guid.NewGuid();
            Guid roomId = Guid.NewGuid();

            Room createdRoom = new Room(groupId, threadId, roomId, "");
            _context.Rooms.Add(createdRoom);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRoom", new { id = createdRoom.Id }, createdRoom);
        }

        [Authorize]
        // Get: api/Rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Room>>> GetRooms()
        {
            if (_context.Rooms == null)
            {
                return NotFound();
            }
            return await _context.Rooms.ToListAsync();
        }

        [Authorize]
        // DELETE: api/Rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(string id)
        {
            if (_context.Rooms == null)
            {
                return NotFound();
            }
            var room = await _context.Rooms.FindAsync(new Guid(id));
            if(room == null)
            {
                return NotFound();
            }
            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("{id}/join")]
        public async Task<ActionResult<JoinRoomDto>> GetJoinInfo(string id)
        {
            var room = await _context.Rooms.FindAsync(new Guid(id));

            if (room == null)
            {
                return NotFound();
            }

            Uri endpoint = new Uri(_endpoint);

            var currentUser = await _userManger.GetUserAsync(User);
            var currentUserAcsId = new CommunicationUserIdentifier(currentUser.CommunicationUserId);
            var currentUserToken = await _communicationIdentityClient.GetTokenAsync(
                currentUserAcsId,
                new[] { CommunicationTokenScope.Chat, CommunicationTokenScope.VoIP });
            var accessToken = currentUserToken.Value.Token;

            string displayName = await UserDisplayNameProvider.GetDisplayName(_userManger, currentUser);

            var adminId = new CommunicationUserIdentifier(_adminUserProvider.GetAdminUserId());
            var adminTokenResult = (await _communicationIdentityClient.GetTokenAsync(
                adminId,
                new[] { CommunicationTokenScope.Chat })).Value;

            CommunicationTokenCredential communicationTokenCredential = new CommunicationTokenCredential(adminTokenResult.Token);
            ChatClient chatClient = new ChatClient(endpoint, communicationTokenCredential);
            ChatThreadClient chatThreadClient = chatClient.GetChatThreadClient(room.ThreadId);

            await chatThreadClient.AddParticipantAsync(new ChatParticipant(currentUserAcsId) { DisplayName = displayName });

            return Ok(new JoinRoomDto
            {
                Room = room,
                UserId = currentUserAcsId,
                AccessToken = accessToken,
                Endpoint = _endpoint
            });
        }

        // GET: api/Rooms/5/anonymousJoin?displayName=...
        [HttpGet("{id}/anonymousJoin")]
        public async Task<ActionResult<JoinRoomDto>> GetAnonymousJoinInfo(string id, string displayName)
        {
            var room = await _context.Rooms.FindAsync(new Guid(id));

            if (room == null)
            {
                return NotFound();
            }

            Uri endpoint = new Uri(_endpoint);
            var idResult = await _communicationIdentityClient.CreateUserAndTokenAsync(new[] { CommunicationTokenScope.Chat, CommunicationTokenScope.VoIP });
            var userId = idResult.Value.User;
            var accessToken =  idResult.Value.AccessToken.Token;
            var adminId = new CommunicationUserIdentifier(_adminUserProvider.GetAdminUserId());
            var adminTokenResult = (await _communicationIdentityClient.GetTokenAsync(
                adminId,
                new[] { CommunicationTokenScope.Chat, CommunicationTokenScope.VoIP })).Value;

            CommunicationTokenCredential communicationTokenCredential = new CommunicationTokenCredential(adminTokenResult.Token);
            ChatClient chatClient = new ChatClient(endpoint, communicationTokenCredential);
            ChatThreadClient chatThreadClient = chatClient.GetChatThreadClient(room.ThreadId);
            await chatThreadClient.AddParticipantAsync(participant: new ChatParticipant(userId){DisplayName = displayName});

            return Ok(new JoinRoomDto
            {
                Room = room,
                UserId = userId,
                AccessToken = accessToken,
                Endpoint = _endpoint
            });
        }
    }
}
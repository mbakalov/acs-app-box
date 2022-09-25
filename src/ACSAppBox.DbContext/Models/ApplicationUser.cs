using Microsoft.AspNetCore.Identity;

namespace ACSAppBox.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? CommunicationUserId { get; set; }
    }
}
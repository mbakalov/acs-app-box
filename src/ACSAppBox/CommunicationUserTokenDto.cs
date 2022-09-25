namespace ACSAppBox
{
    public class CommunicationUserTokenDto
    {
        public string? Token { get; set; }

        public DateTimeOffset? ExpiresOn { get; set; }

        public string? UserId { get; set; }
    }
}

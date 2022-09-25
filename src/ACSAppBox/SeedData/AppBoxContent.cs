#nullable disable

namespace ACSAppBox.SeedData
{
    public class AppBoxContent
    {
        public string EveryUserPassword { get; set; }

        public List<User> Users { get; set; }

        public List<ChatThread> ChatThreads { get; set; }
    }

    public class User
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public string GivenName { get; set; }

        public string FamilyName { get; set; }
    }

    public class ChatThread
    {
        public string Topic { get; set; }

        public List<string> Participants { get; set; }

        public List<ChatThreadMessage> Messages { get; set; }
    }

    public class ChatThreadMessage
    {
        public string Sender { get; set; }

        public string Content { get; set; }
    }
}

using ACSAppBox.Data;
using ACSAppBox.Models;
using ACSAppBox.SeedData;
using Azure.Communication;
using Azure.Communication.Chat;
using Azure.Communication.Identity;
using Azure.Core;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace ACSAppBox
{
    public class SeedDataCreator
    {
        class UserInfo
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public CommunicationUserIdentifier CommunicationUserId { get; internal set; }
            public AccessToken Token { get; internal set; }
            public ChatClient ChatClient { get; internal set; }
            public ChatThreadClient ChatThreadClient { get; internal set; }
        }

        public static async Task EnsureSeedData(
            string dbProvider,
            string sqliteConnectionString,
            string sqlServerConnectionString,
            string acsEndpoint,
            string acsConnectionString)
        {
            Console.WriteLine("Applying database migrations and creating seed data...");

            var identityClient = new CommunicationIdentityClient(acsConnectionString);

            var services = new ServiceCollection();
            services.AddLogging();

            services.AddDbContext<ApplicationDbContext>(
                options => _ = dbProvider switch
                {
                    "Sqlite" => options.UseSqlite(
                        sqliteConnectionString,
                        x => x.MigrationsAssembly("ACSAppBox.SqliteMigrations")),

                    "SqlServer" => options.UseSqlServer(
                        sqlServerConnectionString,
                        x => x.MigrationsAssembly("ACSAppBox.SqlServerMigrations")),

                    _ => throw new NotImplementedException()
                });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            context.Database.Migrate();

            bool usersCreated = false;

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string jsonString = File.ReadAllText("SeedData/data.json");

            var content = JsonSerializer.Deserialize<AppBoxContent>(
                jsonString,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                });

            if (content == null)
            {
                return;
            }

            string firstUserEmail = content.Users[0].Email;
            var dbUser = await userMgr.FindByNameAsync(firstUserEmail);
            if (dbUser != null)
            {
                Console.WriteLine("Database already seeded - nothing to do");
                return;
            }

            Console.WriteLine("Creating seed data...");

            Console.WriteLine("Creating users...");
            var users = new Dictionary<string, UserInfo>();
            foreach (var user in content.Users)
            {
                var acsRawId = (await identityClient.CreateUserAsync()).Value.Id;
                var appUser = new ApplicationUser
                {
                    UserName = user.Email,
                    Email = user.Email,
                    EmailConfirmed = true,
                    CommunicationUserId = acsRawId
                };

                var result = await userMgr.CreateAsync(appUser, content.EveryUserPassword);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                var claims = new List<Claim>()
                {
                    new Claim(JwtClaimTypes.Name, user.Name)
                };

                if (user.GivenName != null)
                {
                    claims.Add(new Claim(JwtClaimTypes.GivenName, user.GivenName));
                }

                if (user.FamilyName != null)
                {
                    claims.Add(new Claim(JwtClaimTypes.FamilyName, user.FamilyName));
                }

                result = await userMgr.AddClaimsAsync(appUser, claims);
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                var acsId = new CommunicationUserIdentifier(acsRawId);
                var token = (await identityClient.GetTokenAsync(acsId, new[] { CommunicationTokenScope.Chat })).Value;
                var chatClient = new ChatClient(new Uri(acsEndpoint), new CommunicationTokenCredential(token.Token));

                users.Add(
                    user.Id,
                    new UserInfo
                    {
                        Id = user.Id,
                        Name = user.Name,
                        CommunicationUserId = acsId,
                        Token = token,
                        ChatClient = chatClient
                    });
            }

            Console.WriteLine("Creating users... Done");

            Console.WriteLine("Creating chat threads...");

            foreach (var chatThread in content.ChatThreads)
            {
                bool threadCreated = false;
                string? chatThreadId = null;
                foreach (var id in chatThread.Participants)
                {
                    var u = users[id];

                    if (!threadCreated)
                    {
                        var createThreadResponse = await u.ChatClient.CreateChatThreadAsync(
                            chatThread.Topic,
                            chatThread.Participants.Where(x => x != id).Select(pid => new ChatParticipant(users[pid].CommunicationUserId))
                        );

                        chatThreadId = createThreadResponse.Value.ChatThread.Id;

                        threadCreated = true;
                    }

                    u.ChatThreadClient = u.ChatClient.GetChatThreadClient(chatThreadId);
                }

                foreach (var msg in chatThread.Messages)
                {
                    var u = users[msg.Sender];

                    u.ChatThreadClient.SendMessage(msg.Content, senderDisplayName: u.Name);
                }
            }

            Console.WriteLine("Creating chat threads... Done");

            Console.WriteLine("Creating seed data... Done");
        }
    }
}

using ACSAppBox;
using ACSAppBox.Data;
using ACSAppBox.Models;
using Azure.Communication.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var sqliteConnectionString = builder.Configuration.GetConnectionString("SqliteConnectionString");
var sqlServerConnectionString = builder.Configuration.GetConnectionString("SqlServerConnectionString");
string acsConnectionString = builder.Configuration.GetConnectionString("ACSConnectionString");
string endpoint = acsConnectionString.Split('=', ';')[1];

var dbProvider = builder.Configuration.GetValue("Provider", "Sqlite");

Console.WriteLine($"Using dbProvider={dbProvider}");

await SeedDataCreator.EnsureSeedData(
    dbProvider,
    sqliteConnectionString,
    sqlServerConnectionString,
    endpoint,
    acsConnectionString);

builder.Services.AddDbContextFactory<ApplicationDbContext>(
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

builder.Services.AddDataProtection().PersistKeysToDbContext<ApplicationDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityServer(options =>
    {
        var maybeIssuerUri = builder.Configuration.GetValue<string>("IdentityServer:IssuerUri");
        if (!string.IsNullOrEmpty(maybeIssuerUri))
        {
            options.IssuerUri = maybeIssuerUri;
        }
    })
    .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

// https://github.com/dotnet/aspnetcore/issues/20780
builder.Services.Configure<IdentityOptions>(options => options.ClaimsIdentity.UserIdClaimType = ClaimTypes.NameIdentifier);

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddSingleton(new CommunicationIdentityClient(acsConnectionString));
builder.Services.AddSingleton<AdminUserProvider>();
builder.Services.AddSingleton(endpoint);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
app.UseRouting();

app.UseAuthentication();
app.UseIdentityServer();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");
app.MapRazorPages();

app.MapFallbackToFile("index.html"); ;

app.Run();

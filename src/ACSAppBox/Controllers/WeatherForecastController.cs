using ACSAppBox.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ACSAppBox.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly AdminUserProvider _adminUserProvider;
        private readonly string _endpoint;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            UserManager<ApplicationUser> userManager,
            AdminUserProvider adminUserProvider,
            string endpoint)
        {
            _logger = logger;
            _userManger = userManager;
            _adminUserProvider = adminUserProvider;
            _endpoint =  endpoint;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            var currentUser = await _userManger.GetUserAsync(User);
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = _endpoint,
                Summary = currentUser.CommunicationUserId,
                AdminUserId = _adminUserProvider.GetAdminUserId()
            })
            .ToArray();
        }
    }
}
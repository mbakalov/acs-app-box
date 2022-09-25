using Microsoft.AspNetCore.Mvc;

namespace ACSAppBox.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppConfigurationController : ControllerBase
    {
        private ILogger<AppConfigurationController> _logger;
        private string _endpoint;

        public AppConfigurationController(
            ILogger<AppConfigurationController> logger,
            string endpoint)
        {
            _logger = logger;
            _endpoint = endpoint;
        }

        public AppConfigurationDto Get()
        {
            return new AppConfigurationDto
            {
                CommunicationServicesEndpoint = _endpoint
            };
        }
    }
}

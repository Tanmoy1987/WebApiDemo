using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace AspNetCoreWebApi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class HomeController: ControllerBase {
        private ILogger _logger;
        private ApplicationSettings _appSettings;
        public HomeController(ILogger logger, IOptions<ApplicationSettings> appSettings) {
            _logger= logger;
            _appSettings= appSettings.Value;
        } 

        [HttpGet]
        [Authorize(Policy= "ValidateUser")]
        public IActionResult Get(string name){
            _logger.LogInformation($"GET Method Call...{HttpContext.User.Identity.Name}"); // LAPTOP-N9FAJOE5\omen
            return StatusCode((int)HttpStatusCode.OK, $"Welcome {name}");
        }
    }
}
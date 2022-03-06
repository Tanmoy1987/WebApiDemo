using System;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace AspNetCoreWebApi.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class MyAppController: ControllerBase {
        private ILogger _logger;
        private ApplicationSettings _appSettings;
        public MyAppController(ILogger logger, IOptions<ApplicationSettings> appSettings) {
            _logger= logger;
            _appSettings= appSettings.Value;
        } 

        [HttpPost]
        [Authorize(Policy="RolePolicy")]
        public IActionResult Post([FromBody] AppRequest appReq){
            _logger.LogInformation("POST Method Call...");
            return StatusCode((int)HttpStatusCode.OK, appReq);
        }
    }
}
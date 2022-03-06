using Microsoft.AspNetCore.Authentication;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace AspNetCoreWebApi {

public class BasicAuthenticationHandler: AuthenticationHandler<AuthenticationSchemeOptions> {
    private IUserService _service;
    public BasicAuthenticationHandler(IUserService service
                                    , IOptionsMonitor<AuthenticationSchemeOptions> options
                                    , ILoggerFactory logger
                                    , UrlEncoder encoder
                                    , ISystemClock clock) : base(options, logger, encoder, clock) {
        _service= service;
    }
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
      bool isAuthenticated= false;
      string authorizationValue= Request.Headers["Authorization"]; 
      if(authorizationValue== null)
        return AuthenticateResult.NoResult();
        
      AuthenticationHeaderValue authHeader= AuthenticationHeaderValue.Parse(authorizationValue);

      if(!authHeader.Scheme.Equals("BASIC", System.StringComparison.OrdinalIgnoreCase)) {
          return AuthenticateResult.Fail($"Authorization Scheme Not Specified");
      }
      if(authHeader.Parameter == null) {
          return AuthenticateResult.Fail($"Authorization Key Missing.");
      }
      
      isAuthenticated= await _service.Authenticate(authHeader.Parameter);

      if(!isAuthenticated) {
          return AuthenticateResult.Fail($"Invalid Credential");
      }

      var claims= await _service.GetAllClaims(authHeader.Parameter);
      var identity= new ClaimsIdentity(claims, Scheme.Name);
      var principal= new ClaimsPrincipal(identity);
      var ticket= new AuthenticationTicket(principal, Scheme.Name);

      return AuthenticateResult.Success(ticket);

    }
  }
}

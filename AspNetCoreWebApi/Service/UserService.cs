using System;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCoreWebApi {
    public class UserService : IUserService {
        private AspNetCoreWebApi.ApplicationSettings _appSettings;
        private AspNetCoreWebApi.Authentication _authSettings;
        public UserService(IOptions<ApplicationSettings> appSettings){
            _appSettings= appSettings.Value;
            _authSettings= _appSettings.Authentication;
        }
        public async Task<bool> Authenticate(string key) {
            return await Task.Run(() =>  Validate(key));
        }
        public async Task<Claim[]> GetAllClaims(string key){
            string decodedValue= System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(key));
            var credential= decodedValue?.Split(":");
            string username= credential.Length== 2 ? credential[0]: "";
            return await Task.Run(() => new Claim[] { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, AppVariables.AppRoles.Administrator.ToString()) });
        }
        private bool Validate(string key){
            return string.Compare(key, _authSettings.Key, false)== 0;
        }
    }
}
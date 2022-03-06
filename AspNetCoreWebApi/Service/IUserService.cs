using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCoreWebApi {
    public interface IUserService {
        Task<bool> Authenticate(string key);
        Task<Claim[]> GetAllClaims(string key);
    }
}
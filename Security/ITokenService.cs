using TravelPlanner.Api.Models;

namespace TravelPlanner.Api.Security;

public interface ITokenService
{
    string GenerateToken(Usuario usuario);
}

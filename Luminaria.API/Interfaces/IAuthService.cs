using Luminaria.API.Dtos;
namespace Luminaria.API.Interfaces
{
    public interface IAuthService
    {
    Task<(AuthResponseDto dto, string token)> LoginAsync(LoginDto request);
    Task<(AuthResponseDto dto, string token)> RegisterAsync(RegisterDto request);
    }
}
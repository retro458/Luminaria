using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using Luminaria.API.Data;
using DotNetEnv;
using Luminaria.API.Interfaces;
using Luminaria.API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Luminaria.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Luminaria.API.Dtos;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Luminaria.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")] // api/auth/login
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
           try
            {
                 var (dto, token) = await _authService.LoginAsync(request);

                Response.Cookies.Append("X-Access-Token", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(8)
                });

                return Ok(RespuestaDto.Ok("Login exitoso", dto));
            }
            catch (Exception ex)
            {
                return Unauthorized(RespuestaDto.Error(ex.Message));
            }
        }

        [HttpPost("logout")] // api/auth/logout
        public IActionResult Logout()
        {            Response.Cookies.Delete("X-Access-Token");
            return Ok(RespuestaDto.Ok("Session cerrada exitosamente"));
        }
    }
}

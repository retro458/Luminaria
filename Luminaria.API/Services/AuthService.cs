using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DotNetEnv;
using Luminaria.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Luminaria.API.Dtos;
using Luminaria.API.Interfaces;
using Luminaria.API.Models;

namespace Luminaria.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly LuminariaContext _context;

        public AuthService(LuminariaContext context)
        {
            _context = context;
        }
    

    public async Task<(AuthResponseDto dto, string token)> LoginAsync(LoginDto request)
    {
        var user = await _context.Usuarios
            .Include(u => u.Rol)
            .FirstOrDefaultAsync(u => u.NombreUsuario == request.NombreUsuario);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Contraseña, user.ContraseñaHash))
        {
            throw new Exception("Credenciales inválidas");
        }

        var token = GenerateToken(user);
        var responseDto = new AuthResponseDto
        {
            NombreUsuario = user.NombreUsuario,
            Rol = user.Rol.NombreRol
        };

        return (responseDto, token);
    }

    public async Task<(AuthResponseDto dto, string token)> RegisterAsync(RegisterDto request)
    {
        if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == request.NombreUsuario))
        {
            throw new Exception("El nombre de usuario ya está en uso");
        }

        var user = new Usuarios
        {
            NombreUsuario = request.NombreUsuario,
            ContraseñaHash = BCrypt.Net.BCrypt.HashPassword(request.Contraseña),
            RolID = request.RolID
        };

        _context.Usuarios.Add(user);
        await _context.SaveChangesAsync();
        await _context.Entry(user).Reference(u => u.Rol).LoadAsync();
        var token = GenerateToken(user);
        var responseDto = new AuthResponseDto
        {
            NombreUsuario = user.NombreUsuario,
            Rol = user.Rol.NombreRol
        };

        return (responseDto, token);
    }

     private string GenerateToken(Usuarios user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Env.GetString("JWT_KEY")));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UsuarioID.ToString()),
                new Claim(ClaimTypes.Role, user.Rol.NombreRol),
            };

            var token = new JwtSecurityToken(
                issuer: Env.GetString("JWT_ISSUER"),
                audience: Env.GetString("JWT_AUDIENCE"),
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

   }
}
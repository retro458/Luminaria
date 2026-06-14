using Luminaria.API.Data;
using Luminaria.API.Dtos;
using Luminaria.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Luminaria.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Editor y Admin pueden entrar
    public class AdminController : ControllerBase
    {
        private readonly LuminariaContext _context;

        public AdminController(LuminariaContext context)
        {
            _context = context;
        }

        // =====================================
        // PERSONAJES
        // =====================================

        [HttpPost("personaje")]
        public async Task<IActionResult> GuardarPersonaje([FromBody] GuardarPersonajeDto dto)
        {
            var idParam = new SqlParameter
            {
                ParameterName = "@PersonajeID",
                SqlDbType = System.Data.SqlDbType.Int,
                Direction = System.Data.ParameterDirection.InputOutput,
                Value = dto.PersonajeID
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_GuardarPersonajeAdmin @PersonajeID OUTPUT, @Nombre, @Epoca, @FechaNacimiento, @FechaFallecimiento, @ResumenBreve, @BiografiaContenido, @ImgURL, @Destacado",
                idParam,
                new SqlParameter("@Nombre", dto.Nombre),
                new SqlParameter("@Epoca", dto.Epoca),
                new SqlParameter("@FechaNacimiento", dto.FechaNacimiento.ToDateTime(TimeOnly.MinValue)),
                new SqlParameter("@FechaFallecimiento", dto.FechaFallecimiento.ToDateTime(TimeOnly.MinValue)),
                new SqlParameter("@ResumenBreve", dto.ResumenBreve),
                new SqlParameter("@BiografiaContenido", dto.BiografiaContenido),
                new SqlParameter("@ImgURL", (object?)dto.ImgURL ?? DBNull.Value),
                new SqlParameter("@Destacado", dto.Destacado)
            );

            var personajeID = (int)idParam.Value;

            // Sincronizar categorías si vienen en el request
            if (dto.CategoriaIDs.Any())
            {
                // Limpiar las existentes y reinsertar
                await _context.Database.ExecuteSqlRawAsync(
                    "DELETE FROM PersonajeCategoria WHERE PersonajeID = @id",
                    new SqlParameter("@id", personajeID));

                foreach (var catID in dto.CategoriaIDs)
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "INSERT INTO PersonajeCategoria VALUES (@pid, @cid)",
                        new SqlParameter("@pid", personajeID),
                        new SqlParameter("@cid", catID));
                }
            }

            return Ok(RespuestaDto.Ok(
                dto.PersonajeID == 0 ? "Personaje creado" : "Personaje actualizado",
                new { PersonajeID = personajeID }));
        }

        // =====================================
        // HITOS
        // =====================================

        [HttpPost("hito")]
        public async Task<IActionResult> GuardarHito([FromBody] GuardarHitoDto dto)
        {
            if (dto.HitoID == 0)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO HitosHistoricos (PersonjaeID, Anno, TituloHito, DescripcionHito) VALUES (@pid, @anno, @titulo, @desc)",
                    new SqlParameter("@pid", dto.PersonajeID),
                    new SqlParameter("@anno", dto.Anno),
                    new SqlParameter("@titulo", dto.TituloHito),
                    new SqlParameter("@desc", dto.DescripcionHito));
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE HitosHistoricos SET Anno = @anno, TituloHito = @titulo, DescripcionHito = @desc WHERE HitoID = @id",
                    new SqlParameter("@anno", dto.Anno),
                    new SqlParameter("@titulo", dto.TituloHito),
                    new SqlParameter("@desc", dto.DescripcionHito),
                    new SqlParameter("@id", dto.HitoID));
            }

            return Ok(RespuestaDto.Ok(dto.HitoID == 0 ? "Hito creado" : "Hito actualizado"));
        }

        [HttpDelete("hito/{id}")]
        [Authorize(Roles = "Admin")] // solo Admin puede borrar
        public async Task<IActionResult> EliminarHito(int id)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "DELETE FROM HitosHistoricos WHERE HitoID = @id",
                new SqlParameter("@id", id));

            return Ok(RespuestaDto.Ok("Hito eliminado"));
        }

        // =====================================
        // FRASES
        // =====================================

        [HttpPost("frase")]
        public async Task<IActionResult> GuardarFrase([FromBody] GuardarFraseDto dto)
        {
            if (dto.FraseID == 0)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO FrasesCelebres (PersonajeID, Frase) VALUES (@pid, @frase)",
                    new SqlParameter("@pid", dto.PersonajeID),
                    new SqlParameter("@frase", dto.Frase));
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE FrasesCelebres SET Frase = @frase WHERE FraseID = @id",
                    new SqlParameter("@frase", dto.Frase),
                    new SqlParameter("@id", dto.FraseID));
            }

            return Ok(RespuestaDto.Ok(dto.FraseID == 0 ? "Frase creada" : "Frase actualizada"));
        }

        [HttpDelete("frase/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarFrase(int id)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "DELETE FROM FrasesCelebres WHERE FraseID = @id",
                new SqlParameter("@id", id));

            return Ok(RespuestaDto.Ok("Frase eliminada"));
        }

        // =====================================
        // LIBROS
        // =====================================

        [HttpPost("libro")]
        public async Task<IActionResult> GuardarLibro([FromBody] GuardarLibroDto dto)
        {
            if (dto.LibroID == 0)
            {
                var idParam = new SqlParameter
                {
                    ParameterName = "@LibroID",
                    SqlDbType = System.Data.SqlDbType.Int,
                    Direction = System.Data.ParameterDirection.Output
                };

                await _context.Database.ExecuteSqlRawAsync(
                    @"INSERT INTO Libros (Titulo, AñoPublicacion, Sinopsis, ImagenPortadaUrl, EsDominioPublico, ArchivoOrLinkUrl)
                      VALUES (@titulo, @año, @sinopsis, @img, @dominio, @link);
                      SET @LibroID = SCOPE_IDENTITY();",
                    new SqlParameter("@titulo", dto.Titulo),
                    new SqlParameter("@año", (object?)dto.AñoPublicacion ?? DBNull.Value),
                    new SqlParameter("@sinopsis", (object?)dto.Sinopsis ?? DBNull.Value),
                    new SqlParameter("@img", (object?)dto.ImagenPortadaUrl ?? DBNull.Value),
                    new SqlParameter("@dominio", dto.EsDominioPublico),
                    new SqlParameter("@link", (object?)dto.ArchivoOrLinkUrl ?? DBNull.Value),
                    idParam);

                var libroID = (int)idParam.Value;

                foreach (var pid in dto.PersonajeIDs)
                {
                    await _context.Database.ExecuteSqlRawAsync(
                        "INSERT INTO PersonajeLibro VALUES (@pid, @lid)",
                        new SqlParameter("@pid", pid),
                        new SqlParameter("@lid", libroID));
                }
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync(
                    @"UPDATE Libros SET Titulo = @titulo, AñoPublicacion = @año, Sinopsis = @sinopsis,
                      ImagenPortadaUrl = @img, EsDominioPublico = @dominio, ArchivoOrLinkUrl = @link
                      WHERE LibroID = @id",
                    new SqlParameter("@titulo", dto.Titulo),
                    new SqlParameter("@año", (object?)dto.AñoPublicacion ?? DBNull.Value),
                    new SqlParameter("@sinopsis", (object?)dto.Sinopsis ?? DBNull.Value),
                    new SqlParameter("@img", (object?)dto.ImagenPortadaUrl ?? DBNull.Value),
                    new SqlParameter("@dominio", dto.EsDominioPublico),
                    new SqlParameter("@link", (object?)dto.ArchivoOrLinkUrl ?? DBNull.Value),
                    new SqlParameter("@id", dto.LibroID));
            }

            return Ok(RespuestaDto.Ok(dto.LibroID == 0 ? "Libro creado" : "Libro actualizado"));
        }

        // =====================================
        // CATEGORIAS 
        // =====================================

        [HttpPost("categoria")]
        [Authorize(Roles = "Admin, Editor")] // ambos roles pueden crear/editar categorías
        public async Task<IActionResult> GuardarCategoria([FromBody] GuardarCategoriaDto dto)
        {
            if (dto.CategoriaID == 0)
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO Categorias (NombreCategoria, ColorEstilo) VALUES (@nombre, @color)",
                    new SqlParameter("@nombre", dto.NombreCategoria),
                    new SqlParameter("@color", dto.ColorEstilo));
            }
            else
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE Categorias SET NombreCategoria = @nombre, ColorEstilo = @color WHERE CategoriaID = @id",
                    new SqlParameter("@nombre", dto.NombreCategoria),
                    new SqlParameter("@color", dto.ColorEstilo),
                    new SqlParameter("@id", dto.CategoriaID));
            }

            return Ok(RespuestaDto.Ok(dto.CategoriaID == 0 ? "Categoría creada" : "Categoría actualizada"));
        }

        [HttpDelete("categoria/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "DELETE FROM Categorias WHERE CategoriaID = @id",
                new SqlParameter("@id", id));

            return Ok(RespuestaDto.Ok("Categoría eliminada"));
        }

        // =====================================
        // USUARIOS — solo Admin
        // =====================================

        [HttpPost("usuario")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CrearUsuario([FromBody] RegisterDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == dto.NombreUsuario))
                return BadRequest(RespuestaDto.Error("El nombre de usuario ya existe"));

            var usuario = new Usuarios
            {
                NombreUsuario = dto.NombreUsuario,
                ContraseñaHash = BCrypt.Net.BCrypt.HashPassword(dto.Contraseña),
                RolID = dto.RolID,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return Ok(RespuestaDto.Ok("Usuario creado"));
        }
    }
}
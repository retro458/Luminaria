using Microsoft.AspNetCore.Mvc;
using Luminaria.API.Data;
using Luminaria.API.Dtos;
using Microsoft.EntityFrameworkCore;
using Luminaria.API.Services;
using Luminaria.API.Models;
using Luminaria.API.Interfaces;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;

namespace Luminaria.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicoController : ControllerBase
    {
        private readonly LuminariaContext _context;
        public PublicoController(LuminariaContext context)
        {
            _context = context;
        }
         // GET api/publico/catalogo?categoria=STEM&busqueda=marie
        [HttpGet("catalogo")]
        public async Task<IActionResult> GetCatalogo(
            [FromQuery] string? categoria,
            [FromQuery] string? busqueda)
        {
            try
            {
            // 1. Preparar los parámetros para el procedimiento
            var parametros = new
            {
                Categoria = string.IsNullOrWhiteSpace(categoria) ? null : categoria.Trim(),
                Busqueda = string.IsNullOrWhiteSpace(busqueda) ? null : busqueda.Trim()
            };

            IEnumerable<PersonajeCatalogoRaw> personajes;
            IEnumerable<CategoriaRaw> categorias;

            // 2. Obtener la conexión subyacente de Entity Framework
            var conexion = _context.Database.GetDbConnection();

            // 3. Ejecutar y leer ambos resultsets en un solo viaje a la BD
            using (var multi = await conexion.QueryMultipleAsync(
                "[dbo].[sp_ObtenerCatálogoPublico]", 
                parametros, 
                commandType: CommandType.StoredProcedure))
            {
                // Lee el primer SELECT (Personajes)
                personajes = await multi.ReadAsync<PersonajeCatalogoRaw>();
                
                // Lee el segundo SELECT (Categorías)
                categorias = await multi.ReadAsync<CategoriaRaw>();
            }

            // 4. Mapear y armar tu JSON estructurado para el Frontend
            var resultado = personajes.Select(p => new PersonajeCatalogoDto
            {
                PersonajeID = p.PersonajeID,
                Nombre = p.Nombre,
                Epoca = p.Epoca,
                ResumenBreve = p.ResumenBreve,
                ImgURL = p.ImgURL,
                Destacado = p.Destacado,
                Categorias = categorias
                    .Where(c => c.PersonajeID == p.PersonajeID)
                    .Select(c => new CategoriaDto
                    {
                        NombreCategoria = c.NombreCategoria,
                        ColorEstilo = c.ColorEstilo
                    }).ToList()
            }).ToList();

            return Ok(RespuestaDto.Ok("Catálogo obtenido", resultado));
        }
        catch (Exception ex)
        {
            return BadRequest(RespuestaDto.Error(ex.Message));
        }
    }

        // GET api/publico/personaje/3
        [HttpGet("personaje/{id}")]
        public async Task<IActionResult> GetPersonaje(int id)
        {
            // sp_ObtenerPersonajeDetalle retorna 4 resultsets
            // EF Core no maneja múltiples resultsets bien con SqlQueryRaw
            // usamos ADO.NET directo
            var conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "EXEC sp_ObtenerPersonajeDetalle @PersonajeID";
            var param = cmd.CreateParameter();
            param.ParameterName = "@PersonajeID";
            param.Value = id;
            cmd.Parameters.Add(param);

            using var reader = await cmd.ExecuteReaderAsync();

            // Resultset 1: datos del personaje
            PersonajeDetalleDto? detalle = null;
            if (await reader.ReadAsync())
            {
                detalle = new PersonajeDetalleDto
                {
                    PersonajeID = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Epoca = reader.GetString(2),
                    FechaNacimiento = DateOnly.FromDateTime(reader.GetDateTime(3)),
                    FechaFallecimiento = DateOnly.FromDateTime(reader.GetDateTime(4)),
                    ResumenBreve = reader.GetString(5),
                    BiografiaContenido = reader.GetString(6),
                    ImgURL = reader.IsDBNull(7) ? null : reader.GetString(7)
                };
            }

            if (detalle == null)
                return NotFound(RespuestaDto.Error("Personaje no encontrado"));

            // Resultset 2: categorías
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                detalle.Categorias.Add(new CategoriaDto
                {
                    NombreCategoria = reader.GetString(1),
                    ColorEstilo = reader.GetString(2)
                });
            }

            // Resultset 3: hitos
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                detalle.Hitos.Add(new HitoDto
                {
                    HitoID = reader.GetInt32(0),
                    Anno = reader.GetInt32(1),
                    TituloHito = reader.GetString(2),
                    DescripcionHito = reader.GetString(3)
                });
            }

            // Resultset 4: libros
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                detalle.Libros.Add(new LibroDto
                {
                    LibroID = reader.GetInt32(0),
                    Titulo = reader.GetString(1),
                    AñoPublicacion = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                    Sinopsis = reader.IsDBNull(3) ? null : reader.GetString(3),
                    ImagenPortadaUrl = reader.IsDBNull(4) ? null : reader.GetString(4),
                    EsDominioPublico = reader.GetBoolean(5),
                    ArchivoOrLinkUrl = reader.IsDBNull(6) ? null : reader.GetString(6)
                });
            }
            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {           
                detalle.Frases.Add(new FraseDto
                {
                    FraseID = reader.GetInt32(0),
                    Frase   = reader.GetString(1)
                });
            }

            await conn.CloseAsync();
            return Ok(RespuestaDto.Ok("Personaje obtenido", detalle));
        }

        // GET api/publico/aleatorio
        [HttpGet("aleatorio")]
        public async Task<IActionResult> GetAleatorio()
        {
            var resultado = await _context.Database
                .SqlQueryRaw<PersonajeAleatorioDto>("EXEC sp_ObtenerPersonajeAleatorio")
                .ToListAsync();

            return Ok(RespuestaDto.Ok("Personaje aleatorio", resultado.FirstOrDefault()));
        }

        // GET api/publico/frase
        [HttpGet("frase")]
        public async Task<IActionResult> GetFraseDelDia()
        {
            var resultado = await _context.Database
                .SqlQueryRaw<FraseDelDiaDto>("EXEC sp_ObtenerFraseDelDia")
                .ToListAsync();

            return Ok(RespuestaDto.Ok("Frase del día", resultado.FirstOrDefault()));
        }
    }
    
}
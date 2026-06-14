namespace Luminaria.API.Dtos
{
    // =====================================
    // REQUESTS DTOs
    // =====================================
    public class LoginDto
    {
        public string NombreUsuario { get; set; } = null!;
        public string Contraseña { get; set; } = null!;
    }

    public class RegisterDto
    {
        public string NombreUsuario { get; set; } = null!;
        public string Contraseña { get; set; } = null!;
        public int RolID { get; set; }

    }

    // =====================================
    // RESPONSES DTOs
    // =====================================

    public class AuthResponseDto
    {
        
        public string NombreUsuario { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }

    public class RespuestaDto
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;

        public object? Data { get; set; }

        public static RespuestaDto Ok(string mensaje, object? data = null) =>
            new() { Exito = true, Mensaje = mensaje, Data = data };

        public static RespuestaDto Error(string mensaje) =>
            new() { Exito = false, Mensaje = mensaje };
    }

}
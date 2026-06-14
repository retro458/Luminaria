namespace Luminaria.API.Models
{
    public class Usuarios
    {
       public int UsuarioID { get; set; }
         public string NombreUsuario { get; set; } = string.Empty;
         public string ContraseñaHash { get; set; } = string.Empty;
         public int RolID { get; set; }
         public DateTime FechaCreacion { get; set; }

            public virtual Roles Rol { get; set; } = null!;
    }
}
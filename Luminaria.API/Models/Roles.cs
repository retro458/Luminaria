namespace Luminaria.API.Models
{
    public class Roles
    {
        public int RolID { get; set; }
        public string NombreRol { get; set; } = string.Empty;

        public virtual ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();
    }
    
}
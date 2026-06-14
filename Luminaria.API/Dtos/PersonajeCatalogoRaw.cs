namespace Luminaria.API.Dtos
{
    internal class PersonajeCatalogoRaw
    {
        public int PersonajeID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Epoca { get; set; } = string.Empty;
        public string ResumenBreve { get; set; } = string.Empty;
        public string? ImgURL { get; set; }
        public bool Destacado { get; set; }
    }

    internal class CategoriaRaw
    {
        public int PersonajeID { get; set; }
        public string NombreCategoria { get; set; } = string.Empty;
        public string ColorEstilo { get; set; } = string.Empty;
    }
}
using System;
using System.Collections.Generic;

namespace Luminaria.API.Models;

public partial class Libro
{
    public int LibroId { get; set; }

    public string Titulo { get; set; } = null!;

    public int? AñoPublicacion { get; set; }

    public string? Sinopsis { get; set; }

    public string? ImagenPortadaUrl { get; set; }

    public bool? EsDominioPublico { get; set; }

    public string? ArchivoOrLinkUrl { get; set; }

    public virtual ICollection<Personaje> Personajes { get; set; } = new List<Personaje>();
}

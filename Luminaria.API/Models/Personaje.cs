using System;
using System.Collections.Generic;

namespace Luminaria.API.Models;

public partial class Personaje
{
    public int PersonajeId { get; set; }

    public string Nombre { get; set; } = null!;

    public string Epoca { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public DateOnly FechaFallecimiento { get; set; }

    public string ResumenBreve { get; set; } = null!;

    public string BiografiaContenido { get; set; } = null!;

    public string? ImgUrl { get; set; }

    public bool? Destacado { get; set; }

    public virtual ICollection<FrasesCelebre> FrasesCelebres { get; set; } = new List<FrasesCelebre>();

    public virtual ICollection<HitosHistorico> HitosHistoricos { get; set; } = new List<HitosHistorico>();

    public virtual ICollection<Categoria> Categoria { get; set; } = new List<Categoria>();

    public virtual ICollection<Libro> Libros { get; set; } = new List<Libro>();
}

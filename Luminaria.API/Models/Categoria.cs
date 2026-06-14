using System;
using System.Collections.Generic;

namespace Luminaria.API.Models;

public partial class Categoria
{
    public int CategoriaId { get; set; }

    public string NombreCategoria { get; set; } = null!;

    public string ColorEstilo { get; set; } = null!;

    public virtual ICollection<Personaje> Personajes { get; set; } = new List<Personaje>();
}

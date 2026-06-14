using System;
using System.Collections.Generic;

namespace Luminaria.API.Models;

public partial class FrasesCelebre
{
    public int FraseId { get; set; }

    public int? PersonajeId { get; set; }

    public string Frase { get; set; } = null!;

    public virtual Personaje? Personaje { get; set; }
}

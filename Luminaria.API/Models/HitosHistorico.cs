using System;
using System.Collections.Generic;

namespace Luminaria.API.Models;

public partial class HitosHistorico
{
    public int HitoId { get; set; }

    public int? PersonjaeId { get; set; }

    public int Anno { get; set; }

    public string TituloHito { get; set; } = null!;

    public string DescripcionHito { get; set; } = null!;

    public virtual Personaje? Personjae { get; set; }
}

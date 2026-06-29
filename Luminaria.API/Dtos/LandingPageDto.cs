// =====================================
// LANDING PAGE - RESPONSE DTOs
// =====================================
namespace Luminaria.API.Dtos
{
// sp_ObtenerCatálogoPublico
public class PersonajeCatalogoDto
{
    public int PersonajeID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Epoca { get; set; } = string.Empty;
    public string ResumenBreve { get; set; } = string.Empty;
    public string? ImgURL { get; set; }
    public bool Destacado { get; set; }
    public List<CategoriaDto> Categorias { get; set; } = [];
}

public class CategoriaDto
{
    public string NombreCategoria { get; set; } = string.Empty;
    public string ColorEstilo { get; set; } = string.Empty;
}

// sp_ObtenerPersonajeDetalle
public class PersonajeDetalleDto
{
    public int PersonajeID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Epoca { get; set; } = string.Empty;
    public DateOnly FechaNacimiento { get; set; }
    public DateOnly FechaFallecimiento { get; set; }
    public string ResumenBreve { get; set; } = string.Empty;
    public string BiografiaContenido { get; set; } = string.Empty;
    public string? ImgURL { get; set; }
    public string? ComponenteEspecial { get; set; }
    public List<CategoriaDto> Categorias { get; set; } = [];
    public List<HitoDto> Hitos { get; set; } = [];
    public List<LibroDto> Libros { get; set; } = [];
    public List<FraseDto> Frases { get; set; } = [];
}

public class HitoDto
{
    public int HitoID { get; set; }
    public int Anno { get; set; }
    public string TituloHito { get; set; } = string.Empty;
    public string DescripcionHito { get; set; } = string.Empty;
}

public class LibroDto
{
    public int LibroID { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public int? AñoPublicacion { get; set; }
    public string? Sinopsis { get; set; }
    public string? ImagenPortadaUrl { get; set; }
    public bool EsDominioPublico { get; set; }
    public string? ArchivoOrLinkUrl { get; set; }
}

// sp_ObtenerPersonajeAleatorio
public class PersonajeAleatorioDto
{
    public int PersonajeID { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string ResumenBreve { get; set; } = string.Empty;
    public string? ImgURL { get; set; }
}

// sp_ObtenerFraseDelDia
public class FraseDelDiaDto
{
    public int FraseID { get; set; }
    public string Frase { get; set; } = string.Empty;
    public int PersonajeID { get; set; }
    public string Autora { get; set; } = string.Empty;
}

//=====================================
// RESQUESTS DTOs
//=====================================
public class FiltroCatalogoDto
{
    public string? Categoria { get; set; }  // "STEM", "Arte", "Literatura" etc
    public string? Busqueda { get; set; }   // por nombre
}

// =====================================
// ADMIN - REQUEST DTOs
// =====================================

public class GuardarPersonajeDto
{
    public int PersonajeID { get; set; } // 0 = nuevo, >0 = actualizar
    public string Nombre { get; set; } = null!;
    public string Epoca { get; set; } = null!;
    public DateOnly FechaNacimiento { get; set; }
    public DateOnly FechaFallecimiento { get; set; }
    public string ResumenBreve { get; set; } = null!;
    public string BiografiaContenido { get; set; } = null!;
    public string? ImgURL { get; set; }
    public bool Destacado { get; set; }
    public List<int> CategoriaIDs { get; set; } = []; // categorías a asociar
}

public class GuardarHitoDto
{
    public int HitoID { get; set; } // 0 = nuevo
    public int PersonajeID { get; set; }
    public int Anno { get; set; }
    public string TituloHito { get; set; } = null!;
    public string DescripcionHito { get; set; } = null!;
}

public class GuardarLibroDto
{
    public int LibroID { get; set; } // 0 = nuevo
    public string Titulo { get; set; } = null!;
    public int? AñoPublicacion { get; set; }
    public string? Sinopsis { get; set; }
    public string? ImagenPortadaUrl { get; set; }
    public bool EsDominioPublico { get; set; }
    public string? ArchivoOrLinkUrl { get; set; }
    public List<int> PersonajeIDs { get; set; } = []; // personajes asociados
}

public class GuardarFraseDto
{
    public int FraseID { get; set; } // 0 = nueva
    public int PersonajeID { get; set; }
    public string Frase { get; set; } = null!;
}

public class GuardarCategoriaDto
{
    public int CategoriaID { get; set; } // 0 = nueva
    public string NombreCategoria { get; set; } = null!;
    public string ColorEstilo { get; set; } = null!;
}

}
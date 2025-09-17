using System;
namespace backend.Models.Dtos;

public class UpdateBookDto
{

    public string Title { get; set; } = null!;
    public decimal? Score { get; set; }
    public string? Description { get; set; }
    public int? ChapterCount{ get; set; }
    public int ViewCount { get; set; }

    //Relacion con Genero
    public int IdGenero { get; set; }

    // Relación con Author
    public int IdAutor { get; set; }
    // public bool? EsActivo { get; set; }

    public int? IdEstado { get; set; }

//     public string? UrlPortada { get; set; }

//     public string? NombrePortada { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace backend.Models.Dtos;

public class VolumeCreateDto
{
    [Required]
    public string Title { get; set; }
    
    [Required]
    public int Order { get; set; }
    
    [Required]
    public int BookId { get; set; }
}
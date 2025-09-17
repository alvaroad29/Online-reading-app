namespace backend.Models.Dtos;

public class VolumeUpdateDto
{
    public string? Title { get; set; }
    public int? Order { get; set; }
    public bool? IsActive { get; set; }
}
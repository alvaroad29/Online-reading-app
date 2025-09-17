using System;

namespace backend.Models.Dtos;

public class UserDataDto
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? DisplayName { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}

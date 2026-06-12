using System.ComponentModel.DataAnnotations;

namespace TravelPlanner.Api.DTOs.Auth;

public class RegisterDto
{
    [Required]
    [MaxLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Senha { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? PreferenciasViagem { get; set; }

    [MaxLength(100)]
    public string? Especialidade { get; set; }

    [Range(1, 10)]
    public int? NivelAcesso { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace TravelPlanner.Api.Models;

public class CategoriaDespesa
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Nome { get; set; } = string.Empty;

    public ICollection<Despesa> Despesas { get; set; } = new List<Despesa>();
}

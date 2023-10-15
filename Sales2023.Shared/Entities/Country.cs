using System.ComponentModel.DataAnnotations;

namespace Sales2023.Shared.Entities
{
    public class Country
    {
        public int Id { get; set; }
        [Display(Name = "País")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El campo {0} no puede contener más de {1} caracteres.")]
        public string Name { get; set; } = null!;
    }
}

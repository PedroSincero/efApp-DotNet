using System.ComponentModel.DataAnnotations;

namespace EFEnergiaAPI.ViewModels.Setor;

public class SetorCreateViewModel
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(120, ErrorMessage = "Nome deve ter no máximo 120 caracteres")]
    public string Nome { get; set; } = string.Empty;
}


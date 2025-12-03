using System.ComponentModel.DataAnnotations;

namespace EFEnergiaAPI.ViewModels.Equipamento;

public class EquipamentoCreateViewModel
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [StringLength(120, ErrorMessage = "Nome deve ter no máximo 120 caracteres")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "SetorId é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "SetorId deve ser maior que zero")]
    public int SetorId { get; set; }
}


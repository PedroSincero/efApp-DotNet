using System.ComponentModel.DataAnnotations;

namespace EFEnergiaAPI.ViewModels.Alerta;

public class AlertaUpdateViewModel
{
    [Required(ErrorMessage = "Id é obrigatório")]
    public int Id { get; set; }

    [Required(ErrorMessage = "EquipamentoId é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "EquipamentoId deve ser maior que zero")]
    public int EquipamentoId { get; set; }

    [Required(ErrorMessage = "Mensagem é obrigatória")]
    [StringLength(250, ErrorMessage = "Mensagem deve ter no máximo 250 caracteres")]
    public string Mensagem { get; set; } = string.Empty;

    public bool Resolvido { get; set; }
}


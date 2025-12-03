using System.ComponentModel.DataAnnotations;

namespace EFEnergiaAPI.ViewModels.Leitura;

public class LeituraCreateViewModel
{
    [Required(ErrorMessage = "EquipamentoId é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "EquipamentoId deve ser maior que zero")]
    public int EquipamentoId { get; set; }

    [Required(ErrorMessage = "Temperatura é obrigatória")]
    [Range(-50, 150, ErrorMessage = "Temperatura deve estar entre -50 e 150 graus")]
    public decimal Temperatura { get; set; }
}


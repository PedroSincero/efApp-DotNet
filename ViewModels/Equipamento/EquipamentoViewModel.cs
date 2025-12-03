namespace EFEnergiaAPI.ViewModels.Equipamento;

public class EquipamentoViewModel
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int SetorId { get; set; }
    public string? SetorNome { get; set; }
}


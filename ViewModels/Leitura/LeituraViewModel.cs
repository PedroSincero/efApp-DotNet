namespace EFEnergiaAPI.ViewModels.Leitura;

public class LeituraViewModel
{
    public int Id { get; set; }
    public int EquipamentoId { get; set; }
    public string? EquipamentoNome { get; set; }
    public decimal Temperatura { get; set; }
    public DateTime DataRegistro { get; set; }
}


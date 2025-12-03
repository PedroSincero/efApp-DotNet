namespace EFEnergiaAPI.ViewModels.Alerta;

public class AlertaViewModel
{
    public int Id { get; set; }
    public int EquipamentoId { get; set; }
    public string? EquipamentoNome { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public bool Resolvido { get; set; }
}


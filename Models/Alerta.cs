using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class Alerta
    {
        public int Id { get; set; }

        [Required]
        public int EquipamentoId { get; set; }
        public Equipamento Equipamento { get; set; }

        [Required]
        [MaxLength(250)]
        public string Mensagem { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public bool Resolvido { get; set; } = false;
    }
}

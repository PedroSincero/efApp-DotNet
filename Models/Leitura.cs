using System;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class Leitura
    {
        public int Id { get; set; }

        [Required]
        public int EquipamentoId { get; set; }
        public Equipamento Equipamento { get; set; }

        [Required]
        public decimal Temperatura { get; set; }

        public DateTime DataRegistro { get; set; } = DateTime.UtcNow;
    }
}

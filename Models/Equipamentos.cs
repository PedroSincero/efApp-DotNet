using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class Equipamento
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(120)]
        public string Nome { get; set; }

        [Required]
        public int SetorId { get; set; }
        public Setor Setor { get; set; }

        public ICollection<Leitura> Leituras { get; set; } = new List<Leitura>();

        public ICollection<Alerta> Alertas { get; set; } = new List<Alerta>();
    }
}

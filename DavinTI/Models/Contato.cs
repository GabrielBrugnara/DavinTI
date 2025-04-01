using System.ComponentModel.DataAnnotations;

namespace DavinTI.Models
{
    public class Contato
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100)]
        public string Nome { get; set; }

        [Range(0, 120, ErrorMessage = "Idade inválida.")]
        public int Idade { get; set; }

        public List<Telefone> Telefones { get; set; } = new();

        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataUltimaAtualizacao { get; set; }
    }
}

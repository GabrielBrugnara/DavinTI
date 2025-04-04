using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DavinTI.Models
{
    public class Telefone
    {
        [Key]
        public long Id { get; set; }

        [Required(ErrorMessage = "O número é obrigatório.")]
        [StringLength(20)]
        public string Numero { get; set; }

        [Required(ErrorMessage = "Selecione um contato.")]
        public long IdContato { get; set; }


        [ForeignKey("IdContato")]
        public virtual Contato? Contato { get; set; }
    }
}

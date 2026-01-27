using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    /// <summary>
    /// Modelo que representa uma pessoa no sistema
    /// </summary>
    public class Person
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, ErrorMessage = "O nome não pode exceder 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Range(0, 150, ErrorMessage = "A idade deve estar entre 0 e 150")]
        public int Age { get; set; }

        // Relacionamento com transações (cascade delete configurado no contexto)
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    /// <summary>
    /// Enum que define os tipos de transação
    /// </summary>
    public enum TransactionType
    {
        Expense,    // Despesa
        Income      // Receita
    }

    /// <summary>
    /// Modelo que representa uma transação financeira
    /// </summary>
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(400, ErrorMessage = "A descrição não pode exceder 400 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser positivo")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "O tipo é obrigatório")]
        public TransactionType Type { get; set; }

        // Relacionamento com Categoria
        [Required(ErrorMessage = "A categoria é obrigatória")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; } = null!;

        // Relacionamento com Pessoa
        [Required(ErrorMessage = "A pessoa é obrigatória")]
        public int PersonId { get; set; }
        [ForeignKey("PersonId")]
        public virtual Person Person { get; set; } = null!;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
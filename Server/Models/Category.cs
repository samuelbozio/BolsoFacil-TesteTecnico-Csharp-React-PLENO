using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    /// <summary>
    /// Enum que define as possíveis finalidades de uma categoria
    /// </summary>
    public enum PurposeType
    {
        Expense,    // Apenas despesas
        Income,     // Apenas receitas
        Both        // Ambos
    }

    /// <summary>
    /// Modelo que representa uma categoria de transação
    /// </summary>
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(400, ErrorMessage = "A descrição não pode exceder 400 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "A finalidade é obrigatória")]
        public PurposeType Purpose { get; set; }

        // Relacionamento com transações
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
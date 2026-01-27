using Server.Models;
using System.ComponentModel.DataAnnotations;

namespace Server.DTOs
{
    /// <summary>
    /// DTO para criação de transação
    /// </summary>
    public class TransactionDTO
    {
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(400, ErrorMessage = "A descrição não pode exceder 400 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "O valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "O valor deve ser positivo")]
        public decimal Value { get; set; }

        [Required(ErrorMessage = "O tipo é obrigatório")]
        public TransactionType Type { get; set; }

        [Required(ErrorMessage = "A categoria é obrigatória")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "A pessoa é obrigatória")]
        public int PersonId { get; set; }
    }

    /// <summary>
    /// DTO para resposta de transação
    /// </summary>
    public class TransactionResponseDTO
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public TransactionType Type { get; set; }
        public int CategoryId { get; set; }
        public string CategoryDescription { get; set; } = string.Empty;
        public int PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
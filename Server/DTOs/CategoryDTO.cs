using Server.Models;
using System.ComponentModel.DataAnnotations;
using Server.Contracts.Dtos;

namespace Server.DTOs
{
    /// <summary>
    /// DTO para criação de categoria
    /// </summary>
    public class CategoryDTO : ICategoryRequest
    {
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(400, ErrorMessage = "A descrição não pode exceder 400 caracteres")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "A finalidade é obrigatória")]
        public PurposeType Purpose { get; set; }

        public string SupportedTransactionTypes => Purpose.ToString();

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Description);
        }

        public IEnumerable<string> GetValidationErrors()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Name)) errors.Add("Nome é obrigatório");
            if (string.IsNullOrWhiteSpace(Description)) errors.Add("Descrição é obrigatória");
            return errors;
        }
    }

    /// <summary>
    /// DTO para resposta de categoria
    /// </summary>
    public class CategoryResponseDTO : ICategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SupportedTransactionTypes { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO para resposta de categoria com totais (opcional)
    /// </summary>
    public class CategoryWithTotalsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public PurposeType Purpose { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
    }
}
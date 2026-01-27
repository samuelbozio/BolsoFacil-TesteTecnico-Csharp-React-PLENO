using Server.Contracts.Dtos;
using System.Text.Json.Serialization;

namespace Server.Application.DTOs
{
    /// <summary>
    /// DTO para request de criação de transação
    /// </summary>
    public class CreateTransactionRequestDTO : ITransactionRequest
    {
        /// <summary>
        /// Valor recebido do cliente como "value" e mapeado para Amount
        /// </summary>
        [JsonPropertyName("value")]
        public decimal Amount { get; set; }

        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Tipo recebido do cliente como número (0=Expense, 1=Income)
        /// Mantemos uma projeção string para o domínio.
        /// </summary>
        [JsonPropertyName("type")]
        public int TypeCode { get; set; }

        /// <summary>
        /// Representação em string usada no domínio ("Expense"/"Income").
        /// Não é esperada no payload, é derivada de TypeCode.
        /// </summary>
        [JsonIgnore]
        public string Type => TypeCode switch
        {
            0 => "Expense",
            1 => "Income",
            _ => string.Empty
        };

        public int CategoryId { get; set; }
        public int PersonId { get; set; }

        public bool IsValid()
        {
            return Amount > 0
                && !string.IsNullOrWhiteSpace(Description)
                && (TypeCode == 0 || TypeCode == 1)
                && CategoryId > 0
                && PersonId > 0;
        }

        public IEnumerable<string> GetValidationErrors()
        {
            var errors = new List<string>();
            if (Amount <= 0) errors.Add("Valor deve ser maior que zero");
            if (string.IsNullOrWhiteSpace(Description)) errors.Add("Descrição é obrigatória");
            if (TypeCode != 0 && TypeCode != 1) errors.Add("Tipo é obrigatório e deve ser Expense(0) ou Income(1)");
            if (CategoryId <= 0) errors.Add("Categoria é obrigatória");
            if (PersonId <= 0) errors.Add("Pessoa é obrigatória");
            return errors;
        }
    }

    /// <summary>
    /// DTO para response de transação
    /// </summary>
    public class TransactionResponseDTO : ITransactionResponse
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BRL";
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int PersonId { get; set; }
        public string PersonName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}

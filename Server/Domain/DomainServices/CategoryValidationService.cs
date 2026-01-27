using Server.Domain.Exceptions;

namespace Server.Domain.DomainServices
{
    /// <summary>
    /// Serviço de Domínio responsável por validações complexas de categorias
    /// </summary>
    public interface ICategoryValidationService
    {
        /// <summary>
        /// Valida se uma categoria pode ser criada
        /// </summary>
        void ValidateCategoryCreation(string description, string purpose);

        /// <summary>
        /// Valida se uma categoria suporta um tipo de transação
        /// </summary>
        bool SupportsTransactionType(string purpose, string transactionType);
    }

    /// <summary>
    /// Implementação do serviço de validação de categorias
    /// </summary>
    public class CategoryValidationService : ICategoryValidationService
    {
        /// <summary>
        /// Valida a criação de uma categoria
        /// </summary>
        public void ValidateCategoryCreation(string description, string purpose)
        {
            if (string.IsNullOrWhiteSpace(description) || description.Length > 400)
                throw new InvalidCategoryException("Descrição de categoria inválida");

            if (!IsValidPurpose(purpose))
                throw new InvalidCategoryException("Propósito de categoria inválido");
        }

        /// <summary>
        /// Verifica se uma categoria suporta um tipo de transação
        /// </summary>
        public bool SupportsTransactionType(string purpose, string transactionType)
        {
            return purpose switch
            {
                "Both" => true,
                "Expense" when transactionType == "Expense" => true,
                "Income" when transactionType == "Income" => true,
                _ => false
            };
        }

        /// <summary>
        /// Valida se um propósito é válido
        /// </summary>
        private bool IsValidPurpose(string purpose)
        {
            return purpose is "Expense" or "Income" or "Both";
        }
    }
}

namespace Server.Domain.Specifications
{
    /// <summary>
    /// Especificações de regras de negócio para transações
    /// Implementa o padrão Specification para validações complexas
    /// </summary>
    public static class TransactionSpecification
    {
        /// <summary>
        /// Valida se uma transação de receita pode ser criada por um menor de idade
        /// Regra de negócio: Menores não podem registrar receitas diretamente
        /// </summary>
        public static bool CanMinorCreateIncome(bool isMinor) => !isMinor;

        /// <summary>
        /// Valida se um valor de transação é válido
        /// </summary>
        public static bool IsValidAmount(decimal amount) => amount > 0;

        /// <summary>
        /// Valida se uma descrição de transação é válida
        /// </summary>
        public static bool HasValidDescription(string description)
        {
            return !string.IsNullOrWhiteSpace(description) && description.Length <= 400;
        }

        /// <summary>
        /// Valida se é um tipo de transação válido
        /// </summary>
        public static bool IsValidTransactionType(string type)
        {
            return type is "Expense" or "Income";
        }
    }
}

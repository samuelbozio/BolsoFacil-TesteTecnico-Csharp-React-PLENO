namespace Server.Domain.Specifications
{
    /// <summary>
    /// Especificações de regras de negócio para categorias
    /// Implementa o padrão Specification para validações complexas
    /// </summary>
    public static class CategorySpecification
    {
        /// <summary>
        /// Valida se uma categoria pode ser usada com um tipo de transação específico
        /// </summary>
        public static bool CanBeUsedWithTransactionType(string categoryPurpose, string transactionType)
        {
            // Mapeia os valores de Enum para strings para facilitar a comparação
            return categoryPurpose switch
            {
                "Both" => true, // Pode ser usada com qualquer tipo
                "Expense" when transactionType == "Expense" => true,
                "Income" when transactionType == "Income" => true,
                _ => false
            };
        }

        /// <summary>
        /// Valida se uma descrição de categoria é válida
        /// </summary>
        public static bool HasValidDescription(string description)
        {
            return !string.IsNullOrWhiteSpace(description) && description.Length <= 400;
        }
    }
}

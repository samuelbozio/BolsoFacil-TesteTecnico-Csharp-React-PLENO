using Server.Domain.Exceptions;

namespace Server.Domain.DomainServices
{
    /// <summary>
    /// Serviço de Domínio responsável por validações complexas de transações
    /// que envolvem múltiplas entidades de domínio
    /// 
    /// Diferença do Application Service:
    /// - Contém LÓGICA DE NEGÓCIO pura (sem conhecimento de persistência)
    /// - Trabalha com Agregados do Domínio
    /// - Lança exceções de domínio
    /// </summary>
    public interface ITransactionValidationService
    {
        /// <summary>
        /// Valida se uma transação pode ser criada considerando a pessoa e categoria
        /// </summary>
        void ValidateTransactionCreation(
            decimal amount,
            string type,
            int categoryId,
            int personId,
            bool isPersonMinor,
            bool categorySupportsType);

        /// <summary>
        /// Valida as regras de negócio que envolvem múltiplas entidades
        /// </summary>
        void ValidateBusinessRules(bool isPersonMinor, string transactionType, bool categoryCompatible);
    }

    /// <summary>
    /// Implementação do serviço de validação de transações
    /// </summary>
    public class TransactionValidationService : ITransactionValidationService
    {
        /// <summary>
        /// Valida a criação de uma transação com todas as regras de negócio
        /// </summary>
        public void ValidateTransactionCreation(
            decimal amount,
            string type,
            int categoryId,
            int personId,
            bool isPersonMinor,
            bool categorySupportsType)
        {
            // Validações básicas de valor
            if (amount <= 0)
                throw new InvalidTransactionException("O valor deve ser maior que zero");

            // Validações de tipo
            if (type != "Expense" && type != "Income")
                throw new InvalidTransactionException("Tipo de transação inválido");

            // Validações de compatibilidade
            if (!categorySupportsType)
                throw new InvalidTransactionException("A categoria não suporta este tipo de transação");

            // REGRA DE NEGÓCIO: Menores não podem criar receitas
            ValidateMinorRestrictions(isPersonMinor, type);
        }

        /// <summary>
        /// Valida as regras de negócio complexas
        /// </summary>
        public void ValidateBusinessRules(bool isPersonMinor, string transactionType, bool categoryCompatible)
        {
            ValidateMinorRestrictions(isPersonMinor, transactionType);

            if (!categoryCompatible)
                throw new InvalidTransactionException("Transação incompatível com a categoria selecionada");
        }

        /// <summary>
        /// Valida restrições específicas para menores de idade
        /// Esta é uma REGRA DE NEGÓCIO crítica
        /// </summary>
        private void ValidateMinorRestrictions(bool isPersonMinor, string transactionType)
        {
            if (isPersonMinor && transactionType == "Income")
            {
                throw new InvalidTransactionException(
                    "Menores de idade não podem registrar receitas. Um responsável deve fazer o registro.");
            }
        }
    }
}

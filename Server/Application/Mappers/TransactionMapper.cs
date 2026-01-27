using Server.DTOs;
using Server.Application.DTOs;
using Server.Models;

namespace Server.Application.Mappers
{
    /// <summary>
    /// Mapper para converter entre DTOs antigos e novos
    /// Mantém compatibilidade enquanto migramos para DDD
    /// 
    /// NOTA: Os DTOs têm nomes iguais mas namespaces diferentes!
    /// Server.DTOs.TransactionResponseDTO (antigo)
    /// Server.Application.DTOs.TransactionResponseDTO (novo)
    /// </summary>
    public static class TransactionMapper
    {
        /// <summary>
        /// Converte TransactionDTO antigo para CreateTransactionRequestDTO novo
        /// </summary>
        public static CreateTransactionRequestDTO ToCreateRequest(TransactionDTO oldDto)
        {
            return new CreateTransactionRequestDTO
            {
                Amount = oldDto.Value,
                Description = oldDto.Description,
                TypeCode = (int)oldDto.Type,
                CategoryId = oldDto.CategoryId,
                PersonId = oldDto.PersonId
            };
        }

        /// <summary>
        /// Converte TransactionResponseDTO novo para TransactionResponseDTO antigo
        /// (nota: nomes iguais, mas estruturas diferentes)
        /// </summary>
        public static Server.DTOs.TransactionResponseDTO ToOldResponse(
            Server.Application.DTOs.TransactionResponseDTO newDto)
        {
            return new Server.DTOs.TransactionResponseDTO
            {
                Id = newDto.Id,
                Description = newDto.Description,
                Value = newDto.Amount,
                Type = (Models.TransactionType)Enum.Parse(
                    typeof(Models.TransactionType), newDto.Type),
                CategoryId = newDto.CategoryId,
                PersonId = newDto.PersonId,
                CreatedAt = newDto.CreatedAt,
                CategoryDescription = "", // Será preenchido pelo Use Case se necessário
                PersonName = ""            // Será preenchido pelo Use Case se necessário
            };
        }

        /// <summary>
        /// Converte Transaction model para TransactionResponseDTO novo
        /// </summary>
        public static Server.Application.DTOs.TransactionResponseDTO ToNewResponse(
            Models.Transaction transaction,
            string categoryDescription = "",
            string personName = "")
        {
            return new Server.Application.DTOs.TransactionResponseDTO
            {
                Id = transaction.Id,
                Amount = transaction.Value,
                Description = transaction.Description,
                Type = transaction.Type.ToString(),
                CategoryId = transaction.CategoryId,
                PersonId = transaction.PersonId,
                CreatedAt = transaction.CreatedAt,
                IsActive = true
            };
        }
    }
}

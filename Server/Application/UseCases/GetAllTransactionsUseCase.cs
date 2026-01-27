using Server.Data;
using Server.Domain.Aggregates;
using Server.Domain.Exceptions;
using Server.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Server.Application.UseCases
{
    /// <summary>
    /// Use Case para obter todas as transações
    /// Carrega dados relacionados (Pessoa e Categoria) para resposta completa
    /// </summary>
    public interface IGetAllTransactionsUseCase
    {
        Task<IEnumerable<TransactionResponseDTO>> ExecuteAsync();
    }

    public class GetAllTransactionsUseCase : IGetAllTransactionsUseCase
    {
        private readonly ApplicationDbContext _context;

        public GetAllTransactionsUseCase(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TransactionResponseDTO>> ExecuteAsync()
        {
            var transactions = await _context.Transactions
                .AsNoTracking()
                .Include(t => t.Category)
                .Include(t => t.Person)
                .Select(t => new TransactionResponseDTO
                {
                    Id = t.Id,
                    Amount = t.Value,
                    Description = t.Description,
                    Type = t.Type.ToString(),
                    CategoryId = t.CategoryId,
                    PersonId = t.PersonId,
                    CreatedAt = t.CreatedAt,
                    IsActive = true
                })
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return transactions;
        }
    }
}


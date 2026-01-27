using Server.Data;
using Server.DTOs;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Application.UseCases
{
    /// <summary>
    /// Use Case para obter todas as categorias com totais de transações
    /// </summary>
    public interface IGetCategoriesWithTotalsUseCase
    {
        Task<IEnumerable<CategoryWithTotalsDTO>> ExecuteAsync();
    }

    public class GetCategoriesWithTotalsUseCase : IGetCategoriesWithTotalsUseCase
    {
        private readonly ApplicationDbContext _context;

        public GetCategoriesWithTotalsUseCase(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryWithTotalsDTO>> ExecuteAsync()
        {
            var categories = await _context.Categories
                .AsNoTracking()
                .Include(c => c.Transactions)
                .ToListAsync();

            return categories.Select(c => new CategoryWithTotalsDTO
            {
                Id = c.Id,
                Description = c.Description,
                Purpose = c.Purpose,
                TotalIncome = c.Transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Value),
                TotalExpense = c.Transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Value),
                Balance = c.Transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Value) -
                    c.Transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Value)
            }).ToList();
        }
    }
}

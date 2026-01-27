using Server.Data;
using Server.Domain.Exceptions;
using Server.DTOs;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Application.UseCases
{
    /// <summary>
    /// Use Case para obter uma pessoa por ID com seus totais
    /// </summary>
    public interface IGetPersonByIdUseCase
    {
        Task<PersonWithTotalsDTO?> ExecuteAsync(int id);
    }

    public class GetPersonByIdUseCase : IGetPersonByIdUseCase
    {
        private readonly ApplicationDbContext _context;

        public GetPersonByIdUseCase(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PersonWithTotalsDTO?> ExecuteAsync(int id)
        {
            var person = await _context.People
                .AsNoTracking()
                .Include(p => p.Transactions)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null)
                return null;

            var totalIncome = person.Transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.Value);

            var totalExpense = person.Transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.Value);

            return new PersonWithTotalsDTO
            {
                Id = person.Id,
                Name = person.Name,
                Age = person.Age,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense
            };
        }
    }
}

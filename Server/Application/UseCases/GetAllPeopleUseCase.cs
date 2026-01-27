using Server.Data;
using Server.Domain.Aggregates;
using Server.DTOs;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Application.UseCases
{
    /// <summary>
    /// Use Case para obter todas as pessoas com seus totais
    /// </summary>
    public interface IGetAllPeopleUseCase
    {
        Task<IEnumerable<PersonWithTotalsDTO>> ExecuteAsync();
    }

    public class GetAllPeopleUseCase : IGetAllPeopleUseCase
    {
        private readonly ApplicationDbContext _context;

        public GetAllPeopleUseCase(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PersonWithTotalsDTO>> ExecuteAsync()
        {
            var people = await _context.People
                .AsNoTracking()
                .Include(p => p.Transactions)
                .ToListAsync();

            var result = new List<PersonWithTotalsDTO>();

            foreach (var person in people)
            {
                var totalIncome = person.Transactions
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t => t.Value);

                var totalExpense = person.Transactions
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t => t.Value);

                result.Add(new PersonWithTotalsDTO
                {
                    Id = person.Id,
                    Name = person.Name,
                    Age = person.Age,
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    Balance = totalIncome - totalExpense
                });
            }

            return result;
        }
    }
}

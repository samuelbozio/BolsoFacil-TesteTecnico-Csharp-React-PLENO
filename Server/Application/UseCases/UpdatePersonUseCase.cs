using Server.Data;
using Server.Domain.Aggregates;
using Server.Domain.Exceptions;
using Server.DTOs;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Application.UseCases
{
    /// <summary>
    /// Use Case para atualizar uma pessoa usando o agregado de domínio
    /// </summary>
    public interface IUpdatePersonUseCase
    {
        Task<PersonWithTotalsDTO?> ExecuteAsync(int id, PersonDTO request);
    }

    public class UpdatePersonUseCase : IUpdatePersonUseCase
    {
        private readonly ApplicationDbContext _context;

        public UpdatePersonUseCase(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PersonWithTotalsDTO?> ExecuteAsync(int id, PersonDTO request)
        {
            var person = await _context.People
                .Include(p => p.Transactions)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (person == null)
                return null;

            // Criar agregado com dados antigos para validações
            var personAggregate = PersonAggregate.Create(person.Name, person.Age, person.Id);

            // Usar métodos do agregado para atualizar
            try
            {
                personAggregate.UpdateName(request.Name);
                personAggregate.UpdateAge(request.Age);
            }
            catch (InvalidPersonException ex)
            {
                throw new InvalidOperationException(ex.Message);
            }

            // Persistir mudanças
            person.Name = request.Name;
            person.Age = request.Age;

            _context.People.Update(person);
            await _context.SaveChangesAsync();

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

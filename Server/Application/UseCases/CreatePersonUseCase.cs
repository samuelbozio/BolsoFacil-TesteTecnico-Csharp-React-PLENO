using Server.Data;
using Server.Domain.Aggregates;
using Server.Domain.Exceptions;
using Server.DTOs;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Application.UseCases
{
    /// <summary>
    /// Use Case para criar uma pessoa usando o agregado de domínio
    /// </summary>
    public interface ICreatePersonUseCase
    {
        Task<PersonWithTotalsDTO> ExecuteAsync(PersonDTO request);
    }

    public class CreatePersonUseCase : ICreatePersonUseCase
    {
        private readonly ApplicationDbContext _context;

        public CreatePersonUseCase(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PersonWithTotalsDTO> ExecuteAsync(PersonDTO request)
        {
            // Validar nome duplicado ANTES de criar o agregado
            var existingPerson = await _context.People
                .FirstOrDefaultAsync(p => p.Name == request.Name);
            
            if (existingPerson != null)
                throw new InvalidPersonException($"Já existe uma pessoa com o nome '{request.Name}'.");

            // Criar agregado (com validações de domínio)
            var personAggregate = PersonAggregate.Create(request.Name, request.Age);

            // Persistir modelo
            var person = new Person
            {
                Name = personAggregate.Name.Value,
                Age = personAggregate.Age.Value
            };

            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return new PersonWithTotalsDTO
            {
                Id = person.Id,
                Name = person.Name,
                Age = person.Age,
                TotalIncome = 0,
                TotalExpense = 0,
                Balance = 0
            };
        }
    }
}

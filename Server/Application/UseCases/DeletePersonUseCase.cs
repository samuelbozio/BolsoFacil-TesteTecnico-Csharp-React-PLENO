using Server.Data;
using Server.Domain.Aggregates;
using Server.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Server.Application.UseCases
{
    /// <summary>
    /// Use Case para deletar uma pessoa usando o agregado de domínio
    /// </summary>
    public interface IDeletePersonUseCase
    {
        Task<bool> ExecuteAsync(int id);
    }

    public class DeletePersonUseCase : IDeletePersonUseCase
    {
        private readonly ApplicationDbContext _context;

        public DeletePersonUseCase(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExecuteAsync(int id)
        {
            var person = await _context.People.FindAsync(id);

            if (person == null)
                return false;

            // Criar agregado e desativar (soft delete via agregado)
            var personAggregate = PersonAggregate.Create(person.Name, person.Age, person.Id);

            try
            {
                personAggregate.Deactivate();
            }
            catch (InvalidPersonException)
            {
                // Já está inativa
            }

            // Hard delete (você pode mudar para soft delete conforme necessário)
            _context.People.Remove(person);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

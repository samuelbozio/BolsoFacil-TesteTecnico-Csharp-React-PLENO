using Server.Data;
using Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Server.Application.UseCases
{
    /// <summary>
    /// Use Case para obter todas as categorias
    /// </summary>
    public interface IGetAllCategoriesUseCase
    {
        Task<IEnumerable<Category>> ExecuteAsync();
    }

    public class GetAllCategoriesUseCase : IGetAllCategoriesUseCase
    {
        private readonly ApplicationDbContext _context;

        public GetAllCategoriesUseCase(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> ExecuteAsync()
        {
            return await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Description)
                .ToListAsync();
        }
    }
}

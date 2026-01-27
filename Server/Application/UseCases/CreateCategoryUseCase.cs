using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.DTOs;
using Server.Models;

namespace Server.Application.UseCases
{
    public class CreateCategoryUseCase : ICreateCategoryUseCase
    {
        private readonly ApplicationDbContext _dbContext;

        public CreateCategoryUseCase(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Category> ExecuteAsync(CategoryDTO categoryDto)
        {
            if (categoryDto == null)
                throw new ArgumentNullException(nameof(categoryDto), "Dados da categoria são obrigatórios");

            if (string.IsNullOrWhiteSpace(categoryDto.Description))
                throw new InvalidOperationException("Descrição da categoria é obrigatória");

            var exists = await _dbContext.Categories
                .AnyAsync(c => c.Description.ToLower() == categoryDto.Description.ToLower());

            if (exists)
                throw new InvalidOperationException("Já existe uma categoria com essa descrição");

            var category = new Category
            {
                Description = categoryDto.Description.Trim(),
                Purpose = categoryDto.Purpose
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            return category;
        }
    }
}

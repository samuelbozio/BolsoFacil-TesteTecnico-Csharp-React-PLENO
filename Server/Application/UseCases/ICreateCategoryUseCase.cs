using Server.DTOs;
using Server.Models;

namespace Server.Application.UseCases
{
    public interface ICreateCategoryUseCase
    {
        Task<Category> ExecuteAsync(CategoryDTO categoryDto);
    }
}

using Microsoft.AspNetCore.Mvc;
using Server.Application.DTOs;
using Server.Application.UseCases;
using Server.Contracts.Responses;
using Server.Contracts.Responses.Implementations;
using Server.DTOs;
using Server.Models;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IGetAllCategoriesUseCase _getAllCategoriesUseCase;
        private readonly IGetCategoriesWithTotalsUseCase _getCategoriesWithTotalsUseCase;
        private readonly ICreateCategoryUseCase _createCategoryUseCase;

        public CategoriesController(
            IGetAllCategoriesUseCase getAllCategoriesUseCase,
            IGetCategoriesWithTotalsUseCase getCategoriesWithTotalsUseCase,
            ICreateCategoryUseCase createCategoryUseCase
        )
        {
            _getAllCategoriesUseCase = getAllCategoriesUseCase;
            _getCategoriesWithTotalsUseCase = getCategoriesWithTotalsUseCase;
            _createCategoryUseCase = createCategoryUseCase;
        }

        [HttpGet]
        public async Task<ActionResult<IApiResponse<IEnumerable<Category>>>> GetCategories()
        {
            try
            {
                var categories = await _getAllCategoriesUseCase.ExecuteAsync();
                return Ok(ApiResponse<IEnumerable<Category>>.Ok(categories, "Categorias obtidas com sucesso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorResponse.CreateFromException(ex, "GET_CATEGORIES_ERROR"));
            }
        }

        [HttpGet("with-totals")]
        public async Task<ActionResult<IApiResponse<IEnumerable<CategoryWithTotalsDTO>>>> GetCategoriesWithTotals()
        {
            try
            {
                var categories = await _getCategoriesWithTotalsUseCase.ExecuteAsync();
                return Ok(ApiResponse<IEnumerable<CategoryWithTotalsDTO>>.Ok(
                    categories,
                    "Categorias com totais obtidas com sucesso"
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorResponse.CreateFromException(ex, "GET_CATEGORIES_WITH_TOTALS_ERROR"));
            }
        }

       [HttpPost]
        public async Task<ActionResult<IApiResponse<Category>>> CreateCategory([FromBody] CategoryDTO categoryDto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);

                return BadRequest(
                    ErrorResponse.CreateFromValidationErrors(errors)
                );
            }

            try
            {
                var category = await _createCategoryUseCase.ExecuteAsync(categoryDto);

                return Ok(
                    ApiResponse<Category>.Ok(category, "Categoria criada com sucesso")
                );
            }
            catch (Exception ex)
            {
                return StatusCode(
                    500,
                    ErrorResponse.CreateFromException(ex, "CREATE_CATEGORY_ERROR")
                );
            }
        }

    }
}

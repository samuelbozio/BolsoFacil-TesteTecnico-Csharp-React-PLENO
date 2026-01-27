using Server.DTOs;
using Server.Application.UseCases;
using Server.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Server.Contracts.Responses;
using Server.Contracts.Responses.Implementations;

namespace Server.Controllers
{
    /// <summary>
    /// Controlador responsável pelas operações CRUD de pessoas
    /// REFATORADO para usar DDD com Use Cases e Agregados
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly ICreatePersonUseCase _createPersonUseCase;
        private readonly IGetAllPeopleUseCase _getAllPeopleUseCase;
        private readonly IGetPersonByIdUseCase _getPersonByIdUseCase;
        private readonly IUpdatePersonUseCase _updatePersonUseCase;
        private readonly IDeletePersonUseCase _deletePersonUseCase;

        public PeopleController(
            ICreatePersonUseCase createPersonUseCase,
            IGetAllPeopleUseCase getAllPeopleUseCase,
            IGetPersonByIdUseCase getPersonByIdUseCase,
            IUpdatePersonUseCase updatePersonUseCase,
            IDeletePersonUseCase deletePersonUseCase)
        {
            _createPersonUseCase = createPersonUseCase;
            _getAllPeopleUseCase = getAllPeopleUseCase;
            _getPersonByIdUseCase = getPersonByIdUseCase;
            _updatePersonUseCase = updatePersonUseCase;
            _deletePersonUseCase = deletePersonUseCase;
        }

        /// <summary>
        /// Obtém todas as pessoas com seus totais de receitas e despesas
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IApiResponse<IEnumerable<PersonWithTotalsDTO>>>> GetPeople()
        {
            try
            {
                var people = await _getAllPeopleUseCase.ExecuteAsync();
                return Ok(ApiResponse<IEnumerable<PersonWithTotalsDTO>>.Ok(people, "Pessoas obtidas com sucesso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorResponse.CreateFromException(ex, "GET_PEOPLE_ERROR"));
            }
        }

        /// <summary>
        /// Obtém uma pessoa específica por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<IApiResponse<PersonWithTotalsDTO>>> GetPerson(int id)
        {
            try
            {
                var person = await _getPersonByIdUseCase.ExecuteAsync(id);
                if (person == null)
                {
                    return NotFound(ErrorResponse.CreateFromException(new KeyNotFoundException($"Pessoa com ID {id} não encontrada"), "NOT_FOUND"));
                }
                return Ok(ApiResponse<PersonWithTotalsDTO>.Ok(person, "Pessoa obtida com sucesso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorResponse.CreateFromException(ex, "GET_PERSON_ERROR"));
            }
        }

        /// <summary>
        /// Cria uma nova pessoa com validações de domínio
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<IApiResponse<PersonWithTotalsDTO>>> CreatePerson(PersonDTO personDto)
        {
            // Validar request
            if (!personDto.IsValid())
            {
                var errors = personDto.GetValidationErrors();
                return BadRequest(ErrorResponse.CreateFromValidationErrors(errors, "Dados de pessoa inválidos"));
            }

            try
            {
                var person = await _createPersonUseCase.ExecuteAsync(personDto);
                return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, ApiResponse<PersonWithTotalsDTO>.Ok(person, "Pessoa criada com sucesso"));
            }
            catch (InvalidPersonException ex)
            {
                return BadRequest(ErrorResponse.CreateFromException(ex, "INVALID_PERSON"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ErrorResponse.CreateFromException(ex, "INVALID_OPERATION"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ErrorResponse.CreateFromException(ex, "ARGUMENT_ERROR"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorResponse.CreateFromException(ex, "CREATE_PERSON_ERROR"));
            }
        }

        /// <summary>
        /// Atualiza os dados de uma pessoa existente com validações de domínio
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<IApiResponse<PersonWithTotalsDTO>>> UpdatePerson(int id, PersonDTO personDto)
        {
            // Validar request
            if (!personDto.IsValid())
            {
                var errors = personDto.GetValidationErrors();
                return BadRequest(ErrorResponse.CreateFromValidationErrors(errors, "Dados de pessoa inválidos"));
            }

            try
            {
                var person = await _updatePersonUseCase.ExecuteAsync(id, personDto);
                if (person == null)
                {
                    return NotFound(ErrorResponse.CreateFromException(new KeyNotFoundException($"Pessoa com ID {id} não encontrada"), "NOT_FOUND"));
                }
                return Ok(ApiResponse<PersonWithTotalsDTO>.Ok(person, "Pessoa atualizada com sucesso"));
            }
            catch (InvalidPersonException ex)
            {
                return BadRequest(ErrorResponse.CreateFromException(ex, "INVALID_PERSON"));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ErrorResponse.CreateFromException(ex, "INVALID_OPERATION"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ErrorResponse.CreateFromException(ex, "ARGUMENT_ERROR"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorResponse.CreateFromException(ex, "UPDATE_PERSON_ERROR"));
            }
        }

        /// <summary>
        /// Exclui uma pessoa e todas as suas transações
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult<IApiResponse>> DeletePerson(int id)
        {
            try
            {
                var success = await _deletePersonUseCase.ExecuteAsync(id);
                if (!success)
                {
                    return NotFound(ErrorResponse.CreateFromException(new KeyNotFoundException($"Pessoa com ID {id} não encontrada"), "NOT_FOUND"));
                }
                return Ok(ApiResponse.Ok("Pessoa deletada com sucesso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorResponse.CreateFromException(ex, "DELETE_PERSON_ERROR"));
            }
        }

        /// <summary>
        /// Obtém o resumo total geral de todas as pessoas
        /// </summary>
        [HttpGet("summary/totals")]
        public async Task<ActionResult<IApiResponse<TotalSummaryDTO>>> GetTotalSummary()
        {
            try
            {
                var people = await _getAllPeopleUseCase.ExecuteAsync();
                
                var totalIncome = people.Sum(p => p.TotalIncome);
                var totalExpense = people.Sum(p => p.TotalExpense);
                
                var summary = new TotalSummaryDTO
                {
                    TotalIncome = totalIncome,
                    TotalExpense = totalExpense,
                    Balance = totalIncome - totalExpense
                };

                return Ok(ApiResponse<TotalSummaryDTO>.Ok(summary, "Resumo obtido com sucesso"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ErrorResponse.CreateFromException(ex, "GET_SUMMARY_ERROR"));
            }
        }
    }
}
using Server.DTOs;
using Server.Application.DTOs;
using Server.Application.UseCases;
using Server.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Server.Contracts.Responses;
using Server.Contracts.Responses.Implementations;
using Server.Services;

namespace Server.Controllers
{
    /// <summary>
    /// Controlador responsável pelas operações de transações
    /// REFATORADO para usar DDD com Use Cases e Agregados
    /// 
    /// Este controller agora é FINO e apenas:
    /// - Traduz requests HTTP para DTOs de Application
    /// - Delega para Use Cases (orquestra domínio + persistência)
    /// - Trata exceções de domínio
    /// - Retorna respostas HTTP
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ICreateTransactionUseCase _createTransactionUseCase;
        private readonly IGetAllTransactionsUseCase _getAllTransactionsUseCase;
        private readonly ILogService _logService;

        public TransactionsController(
            ICreateTransactionUseCase createTransactionUseCase,
            IGetAllTransactionsUseCase getAllTransactionsUseCase,
            ILogService logService)
        {
            _createTransactionUseCase = createTransactionUseCase;
            _getAllTransactionsUseCase = getAllTransactionsUseCase;
            _logService = logService;
        }

        /// <summary>
        /// Obtém todas as transações
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IApiResponse<IEnumerable<Application.DTOs.TransactionResponseDTO>>>> GetTransactions()
        {
            try
            {
                _logService.LogInfo("Obtendo todas as transações", "TRANSACTION_CONTROLLER");
                var transactions = await _getAllTransactionsUseCase.ExecuteAsync();
                _logService.LogInfo($"{transactions.Count()} transações obtidas com sucesso", "TRANSACTION_CONTROLLER");
                return Ok(ApiResponse<IEnumerable<Application.DTOs.TransactionResponseDTO>>.Ok(transactions, "Transações obtidas com sucesso"));
            }
            catch (Exception ex)
            {
                _logService.LogError("Erro ao obter transações", ex, "TRANSACTION_CONTROLLER");
                return StatusCode(500, ErrorResponse.CreateFromException(ex, "GET_TRANSACTIONS_ERROR"));
            }
        }

        /// <summary>
        /// Cria uma nova transação com TODAS as validações de domínio
        /// 
        /// O Use Case garante:
        /// - Valor positivo
        /// - Descrição válida
        /// - Categoria existe e suporta o tipo
        /// - Pessoa existe
        /// - REGRA CRÍTICA: Menores NÃO podem criar receitas
        /// 
        /// Example request:
        /// {
        ///   "amount": 100.50,
        ///   "description": "Compra no mercado",
        ///   "type": "Expense",
        ///   "categoryId": 1,
        ///   "personId": 1
        /// }
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<IApiResponse<Application.DTOs.TransactionResponseDTO>>> CreateTransaction(CreateTransactionRequestDTO request)
        {
            // Validar request
            if (!request.IsValid())
            {
                var errors = request.GetValidationErrors();
                _logService.LogWarning("Request inválida para criar transação", "TRANSACTION_CONTROLLER", new { errors });
                return BadRequest(ErrorResponse.CreateFromValidationErrors(errors, "Dados de transação inválidos"));
            }

            try
            {
                _logService.LogInfo($"Criando transação: {request.Type} de {request.Amount}", "TRANSACTION_CONTROLLER", request);
                var transaction = await _createTransactionUseCase.ExecuteAsync(request);
                _logService.LogInfo($"Transação {request.Type} criada com sucesso (ID: {transaction.Id})", "TRANSACTION_CONTROLLER");
                return CreatedAtAction(nameof(GetTransactions), ApiResponse<Application.DTOs.TransactionResponseDTO>.Ok(transaction, "Transação criada com sucesso"));
            }
            catch (InvalidTransactionException ex)
            {
                _logService.LogWarning($"Transação inválida: {ex.Message}", "TRANSACTION_CONTROLLER", request);
                return BadRequest(ErrorResponse.CreateFromException(ex, "INVALID_TRANSACTION"));
            }
            catch (InvalidOperationException ex)
            {
                _logService.LogError("Operação inválida ao criar transação", ex, "TRANSACTION_CONTROLLER", request);
                return BadRequest(ErrorResponse.CreateFromException(ex, "INVALID_OPERATION"));
            }
            catch (ArgumentException ex)
            {
                _logService.LogError("Argumento inválido ao criar transação", ex, "TRANSACTION_CONTROLLER", request);
                return BadRequest(ErrorResponse.CreateFromException(ex, "ARGUMENT_ERROR"));
            }
            catch (Exception ex)
            {
                _logService.LogError("Erro geral ao criar transação", ex, "TRANSACTION_CONTROLLER", request);
                return StatusCode(500, ErrorResponse.CreateFromException(ex, "CREATE_TRANSACTION_ERROR"));
            }
        }

        // TODO: Implementar DELETE com domínio
        // TODO: Implementar PUT com domínio
    }
}
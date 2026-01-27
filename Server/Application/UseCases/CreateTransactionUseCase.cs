using Server.Data;
using Server.Domain.Aggregates;
using Server.Domain.DomainServices;
using Server.Domain.Exceptions;
using Server.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Server.Application.UseCases
{
    /// <summary>
    /// Use Case para criar uma transação usando a camada de Domain
    /// 
    /// Responsabilidades:
    /// - Orquestrar a criação de uma transação
    /// - Chamar os serviços de domínio para validações
    /// - Persistir o agregado no banco de dados
    /// - Publicar eventos de domínio
    /// </summary>
    public interface ICreateTransactionUseCase
    {
        Task<TransactionResponseDTO> ExecuteAsync(CreateTransactionRequestDTO request);
    }

    public class CreateTransactionUseCase : ICreateTransactionUseCase
    {
        private readonly ApplicationDbContext _context;
        private readonly ITransactionValidationService _transactionValidationService;
        private readonly ICategoryValidationService _categoryValidationService;

        public CreateTransactionUseCase(
            ApplicationDbContext context,
            ITransactionValidationService transactionValidationService,
            ICategoryValidationService categoryValidationService)
        {
            _context = context;
            _transactionValidationService = transactionValidationService;
            _categoryValidationService = categoryValidationService;
        }

        public async Task<TransactionResponseDTO> ExecuteAsync(CreateTransactionRequestDTO request)
        {
            // Passo 1: Buscar as entidades necessárias do banco
            var person = await _context.People.FindAsync(request.PersonId);
            if (person == null)
                throw new InvalidTransactionException($"Pessoa com ID {request.PersonId} não encontrada");

            var category = await _context.Categories.FindAsync(request.CategoryId);
            if (category == null)
                throw new InvalidTransactionException($"Categoria com ID {request.CategoryId} não encontrada");

            // Passo 2: Recriar os agregados a partir dos dados persistidos
            var personAggregate = PersonAggregate.Create(person.Name, person.Age, person.Id);
            var categoryAggregate = CategoryAggregate.Create(category.Description, 
                (CategoryPurposeType)category.Purpose, category.Id);

            // Passo 3: Validar se a categoria suporta o tipo de transação (Serviço de Domínio)
            bool categorySupportsType = _categoryValidationService.SupportsTransactionType(
                category.Purpose.ToString(),
                request.Type
            );

            // Passo 4: Criar o agregado de transação (com TODAS as validações de domínio)
            TransactionAggregate transaction = TransactionAggregate.Create(
                request.Amount,
                request.Description,
                (TransactionTypeEnum)Enum.Parse(typeof(TransactionTypeEnum), request.Type),
                request.CategoryId,
                request.PersonId,
                personAggregate.IsMinor,
                categorySupportsType
            );

            // Passo 5: Persistir a transação
            var transactionModel = new Models.Transaction
            {
                Description = transaction.Description,
                Value = transaction.Amount.Amount,
                Type = (Models.TransactionType)transaction.Type,
                CategoryId = transaction.CategoryId,
                PersonId = transaction.PersonId,
                CreatedAt = transaction.CreatedAt
            };

            _context.Transactions.Add(transactionModel);
            await _context.SaveChangesAsync();

            // Passo 6: Publicar eventos de domínio (aqui você poderia integrar com um mediador)
            var events = transaction.GetUncommittedEvents();
            transaction.ClearUncommittedEvents();
            // TODO: Publicar eventos

            // Passo 7: Retornar DTO
            return new TransactionResponseDTO
            {
                Id = transactionModel.Id,
                Amount = transactionModel.Value,
                Description = transactionModel.Description,
                Type = transactionModel.Type.ToString(),
                CategoryId = transactionModel.CategoryId,
                PersonId = transactionModel.PersonId,
                CreatedAt = transactionModel.CreatedAt,
                IsActive = true
            };
        }
    }
}

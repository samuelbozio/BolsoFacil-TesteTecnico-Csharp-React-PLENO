using System.ComponentModel.DataAnnotations;
using Server.Contracts.Dtos;

namespace Server.DTOs
{
    /// <summary>
    /// DTO para criação e atualização de pessoa
    /// </summary>
    public class PersonDTO : IPersonRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(200, ErrorMessage = "O nome não pode exceder 200 caracteres")]
        public string Name { get; set; } = string.Empty;

        [Range(0, 150, ErrorMessage = "A idade deve estar entre 0 e 150")]
        public int Age { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && Age >= 0 && Age <= 150;
        }

        public IEnumerable<string> GetValidationErrors()
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(Name)) errors.Add("Nome é obrigatório");
            if (Age < 0 || Age > 150) errors.Add("Idade deve estar entre 0 e 150");
            return errors;
        }
    }

    /// <summary>
    /// DTO para resposta de pessoa
    /// </summary>
    public class PersonResponseDTO : IPersonResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsMinor => Age < 18;
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO para resposta de pessoa com totais
    /// </summary>
    public class PersonWithTotalsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsMinor => Age < 18;
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
    }
}
namespace Server.DTOs
{
    /// <summary>
    /// DTO para resumo total de todas as pessoas
    /// </summary>
    public class TotalSummaryDTO
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
    }
}

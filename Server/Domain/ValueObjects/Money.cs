namespace Server.Domain.ValueObjects
{
    /// <summary>
    /// Value Object que representa dinheiro com validações
    /// </summary>
    public record Money
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }

        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        /// <summary>
        /// Factory method para criar um Money válido
        /// </summary>
        public static Money Create(decimal amount, string currency = "BRL")
        {
            if (amount <= 0)
                throw new ArgumentException("O valor deve ser maior que zero", nameof(amount));

            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Moeda é obrigatória", nameof(currency));

            return new Money(amount, currency);
        }

        /// <summary>
        /// Verifica se o valor é positivo
        /// </summary>
        public bool IsPositive => Amount > 0;

        /// <summary>
        /// Verifica se é zero
        /// </summary>
        public bool IsZero => Amount == 0;

        /// <summary>
        /// Soma dois valores monetários
        /// </summary>
        public Money Add(Money other)
        {
            if (other.Currency != Currency)
                throw new InvalidOperationException("Não é possível somar valores em moedas diferentes");

            return Create(Amount + other.Amount, Currency);
        }

        /// <summary>
        /// Subtrai um valor monetário de outro
        /// </summary>
        public Money Subtract(Money other)
        {
            if (other.Currency != Currency)
                throw new InvalidOperationException("Não é possível subtrair valores em moedas diferentes");

            return Create(Amount - other.Amount, Currency);
        }

        public override string ToString() => $"{Amount:C} {Currency}";
    }
}

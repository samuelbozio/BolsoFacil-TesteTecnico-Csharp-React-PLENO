namespace Server.Domain.ValueObjects
{
    /// <summary>
    /// Value Object que representa a idade de uma pessoa com validações
    /// </summary>
    public record Age
    {
        public int Value { get; private set; }

        // Constantes de domínio
        private const int MinimumAge = 0;
        private const int MaximumAge = 150;
        private const int MinorAge = 18;

        private Age(int value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method para criar uma idade válida
        /// </summary>
        public static Age Create(int value)
        {
            if (value < MinimumAge || value > MaximumAge)
                throw new ArgumentException($"A idade deve estar entre {MinimumAge} e {MaximumAge}", nameof(value));

            return new Age(value);
        }

        /// <summary>
        /// Verifica se a pessoa é menor de idade
        /// </summary>
        public bool IsMinor => Value < MinorAge;

        /// <summary>
        /// Verifica se é adulto
        /// </summary>
        public bool IsAdult => Value >= MinorAge;

        public override string ToString() => $"{Value} anos";
    }
}

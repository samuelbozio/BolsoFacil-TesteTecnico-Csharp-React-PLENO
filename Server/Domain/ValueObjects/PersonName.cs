namespace Server.Domain.ValueObjects
{
    /// <summary>
    /// Value Object que representa o nome de uma pessoa
    /// </summary>
    public record PersonName
    {
        public string Value { get; private set; }

        private PersonName(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method para criar um nome válido
        /// </summary>
        public static PersonName Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("O nome é obrigatório", nameof(value));

            if (value.Length > 200)
                throw new ArgumentException("O nome não pode exceder 200 caracteres", nameof(value));

            return new PersonName(value.Trim());
        }

        public override string ToString() => Value;
    }
}

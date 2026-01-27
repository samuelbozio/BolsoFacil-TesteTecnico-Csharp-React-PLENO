namespace Server.Domain.ValueObjects
{
    /// <summary>
    /// Value Object que representa a descrição de uma categoria
    /// </summary>
    public record CategoryDescription
    {
        public string Value { get; private set; }

        private CategoryDescription(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method para criar uma descrição válida
        /// </summary>
        public static CategoryDescription Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("A descrição é obrigatória", nameof(value));

            if (value.Length > 400)
                throw new ArgumentException("A descrição não pode exceder 400 caracteres", nameof(value));

            return new CategoryDescription(value.Trim());
        }

        public override string ToString() => Value;
    }
}

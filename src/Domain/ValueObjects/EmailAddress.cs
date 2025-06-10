using Events.Domain.Exceptions;
using System.Net.Mail;

namespace Events.Domain.ValueObjects
{
    public class EmailAddress : IEquatable<EmailAddress>
    {
        private readonly string _value;

        public string Value => _value;

        public EmailAddress(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvariantViolationException("Email address cannot be empty.");
            }
            var trimmed = value.Trim();

            try
            {
                var mail = new MailAddress(trimmed);
                _value = mail.Address;
            }
            catch (FormatException)
            {
                throw new InvariantViolationException($"Invalid email format: '{value}'.");
            }
        }

        public override bool Equals(object? obj) => Equals(obj as EmailAddress);

        public bool Equals(EmailAddress? other) =>
            other != null && StringComparer.OrdinalIgnoreCase.Equals(_value, other._value);

        public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(_value);

        public override string ToString() => _value;
    }
}

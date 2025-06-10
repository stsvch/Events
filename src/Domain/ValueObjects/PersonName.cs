using Events.Domain.Exceptions;

namespace Events.Domain.ValueObjects
{
    public class PersonName : IEquatable<PersonName>
    {
        public string FirstName { get; }
        public string LastName { get; }

        public PersonName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new InvariantViolationException("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(lastName))
                throw new InvariantViolationException("Last name cannot be empty.");

            var trimmedFirst = firstName.Trim();
            var trimmedLast = lastName.Trim();

            FirstName = char.ToUpper(trimmedFirst[0]) + trimmedFirst[1..].ToLower();
            LastName = char.ToUpper(trimmedLast[0]) + trimmedLast[1..].ToLower();
        }

        public override bool Equals(object? obj) => Equals(obj as PersonName);

        public bool Equals(PersonName? other) =>
            other != null &&
            string.Equals(FirstName, other.FirstName, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(LastName, other.LastName, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode() => HashCode.Combine(
            StringComparer.OrdinalIgnoreCase.GetHashCode(FirstName),
            StringComparer.OrdinalIgnoreCase.GetHashCode(LastName));

        public override string ToString() => $"{FirstName} {LastName}";
    }
}


namespace Events.Domain.Exceptions
{
    public class InvariantViolationException : DomainException
    {
        public InvariantViolationException(string message)
            : base(message) { }
    }
}

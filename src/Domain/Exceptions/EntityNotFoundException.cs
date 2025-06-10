

namespace Events.Domain.Exceptions
{
    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(Guid id)
            : base($"Entity with Id = '{id}' was not found.") { }

        public EntityNotFoundException(): base($"Entity was not found.") { }
    }
}

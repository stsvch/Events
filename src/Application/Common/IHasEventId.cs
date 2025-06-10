

namespace Events.Application.Common
{
    public interface IHasEventId
    {
        Guid EventId { get; }
    }
}

using MediatR;

namespace Events.Application.Queries
{
    public class GetFirstEventImageQuery : IRequest<string?>
    {
        public Guid EventId { get; set; }
    }
}

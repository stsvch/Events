using Events.Application.Common;
using MediatR;


namespace Events.Application.Commands
{
    public class AddEventImageCommand : IRequest<Unit>, IHasEventId
    {
        public Guid EventId { get; set; }
        public string Url { get; set; }
    }
}

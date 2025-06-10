using Events.Application.Common;
using MediatR;

namespace Events.Application.Commands
{
    public class DeleteEventCommand : IRequest<Unit>, IHasId
    {
        public Guid Id { get; set; }
    }
}

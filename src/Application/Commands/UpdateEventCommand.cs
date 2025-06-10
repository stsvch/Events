using Events.Application.Common;
using MediatR;

namespace Events.Application.Commands
{
    public class UpdateEventCommand : IRequest<Unit>, IHasId
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Venue { get; set; }
        public Guid CategoryId { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
    }

}

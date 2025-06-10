using Events.Application.Common;
using MediatR;

namespace Events.Application.Commands
{
    public class DeleteCategoryCommand : IRequest<Unit>, IHasId
    {
        public Guid Id { get; set; }
    }
}

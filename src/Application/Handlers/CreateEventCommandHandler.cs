using AutoMapper;
using Events.Application.Commands;
using Events.Domain.Entities;
using Events.Domain.Repositories;
using MediatR;

namespace Events.Application.Handlers
{
    public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Guid>
    {
        private readonly IEventRepository _repo;
        private readonly IMapper _mapper;

        public CreateEventCommandHandler(IEventRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<Guid> Handle(CreateEventCommand command, CancellationToken cancellationToken)
        {
            var evt = _mapper.Map<Event>(command);

            await _repo.AddAsync(evt, cancellationToken);
            return evt.Id;
        }
    }

}

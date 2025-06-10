using AutoMapper;
using Events.Application.DTOs;
using Events.Application.Queries;
using Events.Domain.Exceptions;
using Events.Domain.Repositories;
using MediatR;

namespace Events.Application.Handlers
{
    public class GetEventDetailQueryHandler : IRequestHandler<GetEventDetailQuery, EventDetailDto>
    {
        private readonly IEventRepository _repo;
        private readonly IMapper _mapper;

        public GetEventDetailQueryHandler(IEventRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<EventDetailDto> Handle(  GetEventDetailQuery request,  CancellationToken cancellationToken)
        {
            var entity = await _repo.GetByIdWithDetailsAsync( request.Id, cancellationToken)?? throw new EntityNotFoundException(request.Id);

            return _mapper.Map<EventDetailDto>(entity);
        }
    }
}

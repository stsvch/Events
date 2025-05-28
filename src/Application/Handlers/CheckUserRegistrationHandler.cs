using AutoMapper;
using Events.Application.DTOs;
using Events.Application.Queries;
using Events.Domain.Repositories;
using MediatR;


namespace Events.Application.Handlers
{
    public class CheckUserRegistrationHandler : IRequestHandler<CheckUserRegistrationQuery, RegistrationStatusDto>
    {
        private readonly IParticipantRepository _repository;
        private readonly IMapper _mapper;

        public CheckUserRegistrationHandler(IParticipantRepository repository, IMapper mapper)
        {
            _repository = repository;
        }

        public async Task<RegistrationStatusDto> Handle(
            CheckUserRegistrationQuery request,
            CancellationToken cancellationToken)
        {
            var exists = await _repository.IsUserRegisteredAsync(
                request.EventId,
                request.UserId,
                cancellationToken);

            return _mapper.Map<RegistrationStatusDto>(exists);
        }
    }
}

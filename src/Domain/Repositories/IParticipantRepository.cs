using Events.Domain.Entities;
using Events.Domain.Specifications;

namespace Events.Domain.Repositories
{
    public interface IParticipantRepository : IRepository<Participant>
    {
        public Task<Participant?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<bool> IsUserRegisteredAsync(Guid eventId, string userId, CancellationToken ct = default);

        Task<Participant?> GetBySpecAsync(ISpecification<Participant> spec, CancellationToken ct);
    }
}

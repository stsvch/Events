using Events.Domain.Entities;

namespace Events.Domain.Repositories
{
    public interface IParticipantRepository : IRepository<Participant>
    {
        public Task<Participant?> GetByEmailAsync(string email, CancellationToken ct = default);

    }
}

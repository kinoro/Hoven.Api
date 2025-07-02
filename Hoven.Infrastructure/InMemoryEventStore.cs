using Hoven.Domain.Events;

namespace Hoven.Infrastructure;

public class InMemoryEventStore
{
    private readonly Dictionary<Guid, List<IDomainEvent>> _store = new();

    public void SaveEvents(Guid aggregateId, IEnumerable<IDomainEvent> events)
    {
        if (!_store.ContainsKey(aggregateId))
        {
            _store[aggregateId] = new List<IDomainEvent>();
        }

        _store[aggregateId].AddRange(events);
    }

    public IEnumerable<IDomainEvent> GetEvents(Guid aggregateId)
    {
        return _store.ContainsKey(aggregateId)
            ? _store[aggregateId]
            : Enumerable.Empty<IDomainEvent>();
    }
}

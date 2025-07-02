namespace Hoven.Domain.Events;

public interface IDomainEvent
{
    public DateTime OccurredOn { get; }
}

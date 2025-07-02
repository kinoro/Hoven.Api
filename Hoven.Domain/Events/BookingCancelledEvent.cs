namespace Hoven.Domain.Events;

public record BookingCancelledEvent(
    Guid BookingId
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}


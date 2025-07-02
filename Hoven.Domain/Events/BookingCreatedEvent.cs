namespace Hoven.Domain.Events;

public record BookingCreatedEvent(
    Guid BookingId,
    Guid CustomerId,
    Guid HolidayParkId,
    DateTime ArrivalDate,
    DateTime DepartureDate
) : IDomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}


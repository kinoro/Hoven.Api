using Hoven.Domain.Common;
using Hoven.Domain.Events;

namespace Hoven.Domain.Aggregates;

public class Booking
{
    private readonly List<IDomainEvent> _changes = new();

    public Guid Id { get; private set; }
    public Guid CustomerId { get; private set; }
    public Guid HolidayParkId { get; private set; }
    public DateTime ArrivalDate { get; private set; }
    public DateTime DepartureDate { get; private set; }
    public BookingStatus Status { get; private set; } = BookingStatus.NotSet;

    public Booking(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
        {
            Apply(@event);
        }
    }

    private Booking() { }

    public static Result<Booking> Create(Guid bookingId, Guid customerId, Guid parkId, DateTime arrival, DateTime departure)
    {
        var booking = new Booking();
        var @event = new BookingCreatedEvent(bookingId, customerId, parkId, arrival, departure);
        booking.Apply(@event);
        booking._changes.Add(@event);
        return Result<Booking>.Success(booking);
    }

    public Result AmendDates(DateTime newArrivalDate, DateTime newDepartureDate)
    {
        if (Status == BookingStatus.Cancelled)
        {
            return Result.Failure(BookingErrors.CannotAmendCancelledBooking);
        }

        var @event = new BookingAmendedEvent(Id, CustomerId, HolidayParkId, newArrivalDate, newDepartureDate);
        Apply(@event);
        _changes.Add(@event);

        return Result.Success();
    }

    public Result Cancel()
    {
        var @event = new BookingCancelledEvent(Id);
        Apply(@event);
        _changes.Add(@event);

        return Result.Success();
    }

    private void Apply(IDomainEvent e)
    {
        switch (e)
        {
            case BookingCreatedEvent ev:
                Id = ev.BookingId;
                CustomerId = ev.CustomerId;
                HolidayParkId = ev.HolidayParkId;
                ArrivalDate = ev.ArrivalDate;
                DepartureDate = ev.DepartureDate;
                Status = BookingStatus.Booked;
                break;
            case BookingAmendedEvent ev:
                ArrivalDate = ev.ArrivalDate;
                DepartureDate = ev.DepartureDate;
                break;
            case BookingCancelledEvent _:
                Status = BookingStatus.Cancelled;
                break;
        }
    }

    public IReadOnlyCollection<IDomainEvent> GetUncommittedEvents() => _changes.AsReadOnly();

    public void ClearUncommittedEvents() => _changes.Clear();
}

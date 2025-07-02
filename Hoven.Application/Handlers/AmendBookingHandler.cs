using Hoven.Application.Commands;
using Hoven.Domain.Aggregates;
using Hoven.Domain.Common;
using Hoven.Infrastructure;

namespace Hoven.Application.Handlers;

public class AmendBookingHandler
{
    private readonly InMemoryEventStore _eventStore;

    public AmendBookingHandler(InMemoryEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public Result Handle(AmendBookingCommand cmd)
    {
        var events = _eventStore.GetEvents(cmd.BookingId);
        if (!events.Any())
        {
            throw new InvalidOperationException($"No booking found with ID {cmd.BookingId}.");
        }

        var booking = new Booking(events);
        var amendResult = booking.AmendDates(cmd.ArrivalDate, cmd.DepartureDate);
        if (!amendResult.IsSuccess)
        {
            return amendResult;
        }

        _eventStore.SaveEvents(booking.Id, booking.GetUncommittedEvents());
        booking.ClearUncommittedEvents();

        return Result.Success();
    }
}

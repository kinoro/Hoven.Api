using Hoven.Application.Commands;
using Hoven.Domain.Aggregates;
using Hoven.Domain.Common;
using Hoven.Infrastructure;

namespace Hoven.Application.Handlers;

public class CancelBookingHandler
{
    private readonly InMemoryEventStore _eventStore;

    public CancelBookingHandler(InMemoryEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public Result Handle(CancelBookingCommand cmd)
    {
        var events = _eventStore.GetEvents(cmd.BookingId);
        if (!events.Any())
        {
            throw new InvalidOperationException($"No booking found with ID {cmd.BookingId}.");
        }

        var booking = new Booking(events);

        var cancelResult = booking.Cancel();
        if (!cancelResult.IsSuccess)
        {
            return cancelResult;
        }

        _eventStore.SaveEvents(booking.Id, booking.GetUncommittedEvents());
        booking.ClearUncommittedEvents();

        return Result.Success();
    }
}

using Hoven.Application.Commands;
using Hoven.Domain.Aggregates;
using Hoven.Domain.Common;
using Hoven.Infrastructure;

namespace Hoven.Application.Handlers;

public class GetBookingHandler
{
    private readonly InMemoryEventStore _eventStore;

    public GetBookingHandler(InMemoryEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public Result<Booking> Handle(GetBookingCommand cmd)
    {
        var events = _eventStore.GetEvents(cmd.BookingId);
        if (!events.Any())
        {
            return Result<Booking>.Failure($"No booking found with ID {cmd.BookingId}.");
        }

        return Result<Booking>.Success(new Booking(events));
    }
}

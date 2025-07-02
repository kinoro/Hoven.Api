using Hoven.Application.Commands;
using Hoven.Domain.Aggregates;
using Hoven.Domain.Common;
using Hoven.Infrastructure;

namespace Hoven.Application.Handlers;

public class CreateBookingHandler
{
    private readonly InMemoryEventStore _eventStore;

    public CreateBookingHandler(InMemoryEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public Result Handle(CreateBookingCommand cmd)
    {
        var createResult = Booking.Create(
            cmd.BookingId,
            cmd.CustomerId,
            cmd.HolidayParkId,
            cmd.ArrivalDate,
            cmd.DepartureDate
        );

        if (!createResult.IsSuccess)
        {
            return createResult;
        }

        var booking = createResult.Value!;

        _eventStore.SaveEvents(booking.Id, booking.GetUncommittedEvents());
        booking.ClearUncommittedEvents();

        return Result.Success();
    }
}

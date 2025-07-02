namespace Hoven.Application.Commands;

public record CancelBookingCommand(
    Guid BookingId
);

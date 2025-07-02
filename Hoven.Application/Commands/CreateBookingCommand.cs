namespace Hoven.Application.Commands;

public record CreateBookingCommand(
    Guid BookingId,
    Guid CustomerId,
    Guid HolidayParkId,
    DateTime ArrivalDate,
    DateTime DepartureDate
);

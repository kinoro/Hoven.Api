namespace Hoven.Application.Commands;

public record AmendBookingCommand(
    Guid BookingId,
    Guid CustomerId,
    Guid HolidayParkId,
    DateTime ArrivalDate,
    DateTime DepartureDate
);

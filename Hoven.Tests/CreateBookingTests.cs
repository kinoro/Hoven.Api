using FluentAssertions;
using Hoven.Domain.Aggregates;
using Hoven.Domain.Events;

namespace Hoven.Tests;

public class CreateBookingTests
{
    [Fact]
    public void CreateBooking_WhenValid_ShouldProduceBookingCreatedEvent()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var holidayParkId = Guid.NewGuid();
        var arrival = new DateTime(2025, 8, 1);
        var departure = new DateTime(2025, 8, 7);

        // Act
        var createResult = Booking.Create(
            bookingId,
            customerId,
            holidayParkId,
            arrival,
            departure
        );

        var events = createResult.Value!.GetUncommittedEvents();

        // Assert
        events.Should().ContainSingle(e => e is BookingCreatedEvent);

        var createdEvent = (BookingCreatedEvent)events.First();
        createdEvent.BookingId.Should().Be(bookingId);
        createdEvent.ArrivalDate.Should().Be(arrival);
        createdEvent.DepartureDate.Should().Be(departure);
    }
}

using FluentAssertions;
using Hoven.Domain.Aggregates;
using Hoven.Domain.Events;

namespace Hoven.Tests;

public class CancelBookingTests
{
    [Fact]
    public void CancelBooking_WhenValid_ShouldProduceBookingCancelledEvent()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var initialEvent = new BookingCreatedEvent(
            bookingId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            new DateTime(2025, 8, 1),
            new DateTime(2025, 8, 7)
        );

        var booking = new Booking(new[] { initialEvent });

        // Act
        booking.Cancel();

        var events = booking.GetUncommittedEvents();

        // Assert
        events.Should().ContainSingle(e => e is BookingCancelledEvent);
    }


}

using FluentAssertions;
using Hoven.Domain.Aggregates;
using Hoven.Domain.Common;
using Hoven.Domain.Events;

namespace Hoven.Tests;

public class AmendBookingTests
{
    [Fact]
    public void AmendBooking__WhenValid_ShouldProduceBookingAmendedEvent()
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

        var newArrival = new DateTime(2025, 8, 10);
        var newDeparture = new DateTime(2025, 8, 15);

        // Act
        booking.AmendDates(newArrival, newDeparture);

        var events = booking.GetUncommittedEvents();

        // Assert
        events.Should().ContainSingle(e => e is BookingAmendedEvent);

        var amendedEvent = (BookingAmendedEvent)events.First();
        amendedEvent.ArrivalDate.Should().Be(newArrival);
        amendedEvent.DepartureDate.Should().Be(newDeparture);
    }

    [Fact]
    public void AmendBooking__WhenBookingIsCancelled_ShouldProduceErrorResult()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var initialEvents = new List<IDomainEvent> {
            new BookingCreatedEvent(
                bookingId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                new DateTime(2025, 8, 1),
                new DateTime(2025, 8, 7)),
            new BookingCancelledEvent(bookingId)
        };

        var booking = new Booking(initialEvents);

        var newArrival = new DateTime(2025, 8, 10);
        var newDeparture = new DateTime(2025, 8, 15);

        // Act
        var amendResult = booking.AmendDates(newArrival, newDeparture);

        // Assert
        amendResult.IsSuccess.Should().BeFalse();
        amendResult.Error.Should().Be(BookingErrors.CannotAmendCancelledBooking);
    }
}

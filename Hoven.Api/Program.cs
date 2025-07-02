using Hoven.Application.Commands;
using Hoven.Application.Handlers;
using Hoven.Infrastructure;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hoven Api", Version = "v1" });
});
builder.Services.AddSingleton<InMemoryEventStore>();
builder.Services.AddTransient<CreateBookingHandler>();
builder.Services.AddTransient<AmendBookingHandler>();
builder.Services.AddTransient<CancelBookingHandler>();
builder.Services.AddTransient<GetBookingHandler>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hoven Api V1");
});

app.MapPost("/booking", (CreateBookingCommand cmd, CreateBookingHandler handler) =>
{
    var result = handler.Handle(cmd);
    if (!result.IsSuccess)
    {
        return Results.Problem(
            title: "Booking creation failed",
            detail: result.Error,
            statusCode: StatusCodes.Status400BadRequest
        );
    }

    return Results.Ok("Booking created.");
});

app.MapPost("/booking/amend", (AmendBookingCommand cmd, AmendBookingHandler handler) =>
{
    var result = handler.Handle(cmd);
    if (!result.IsSuccess)
    {
        return Results.Problem(
            title: "Booking amendment failed",
            detail: result.Error,
            statusCode: StatusCodes.Status400BadRequest
        );
    }

    return Results.Ok("Booking amended.");
});

app.MapPost("/booking/cancel", (CancelBookingCommand cmd, CancelBookingHandler handler) =>
{
    var result = handler.Handle(cmd);
    if (!result.IsSuccess)
    {
        return Results.Problem(
            title: "Booking cancellation failed",
            detail: result.Error,
            statusCode: StatusCodes.Status400BadRequest
        );
    }

    return Results.Ok("Booking cancelled.");
});

app.MapGet("/booking/{id}", (Guid id, GetBookingHandler handler) =>
{
    var result = handler.Handle(new GetBookingCommand(id));
    if (!result.IsSuccess)
    {
        return Results.Problem(
            title: "Booking retrieval failed",
            detail: result.Error,
            statusCode: StatusCodes.Status404NotFound
        );
    }

    var booking = result.Value!;

    return Results.Ok(new
    {
        booking.Id,
        booking.CustomerId,
        booking.HolidayParkId,
        booking.ArrivalDate,
        booking.DepartureDate,
        Status = booking.Status.ToString()
    });
});

app.Run();

using MediatR;
using OrderEngine.Application;
using OrderEngine.Domain;

namespace OrderEngine.Api.Endpoints;

public static class OrderEndpoints
{
    public static WebApplication MapOrderEndpoints(this WebApplication app)
    {
        app.MapGet("/api/orders", async (ISender mediator, int page = 1, int pageSize = 10) =>
        {
            if (page < 1 || pageSize < 1 || pageSize > 100)
                return Results.BadRequest(new { message = "Page must be greater than zero and pageSize must be between 1 and 100." });

            var orders = await mediator.Send(new GetOrdersQuery(page, pageSize));
            return Results.Ok(orders);
        }).RequireAuthorization();

        app.MapGet("/api/orders/{id:guid}", async (Guid id, ISender mediator) =>
        {
            var order = await mediator.Send(new GetOrderByIdQuery(id));
            return order is null ? Results.NotFound() : Results.Ok(order);
        }).RequireAuthorization();

        app.MapPost("/api/orders", async (CreateOrderRequest request, ISender mediator) =>
        {
            var created = await mediator.Send(new CreateOrderCommand(request.CustomerId, request.Items));
            return Results.Created($"/api/orders/{created.Id}", created);
        }).RequireAuthorization();

        app.MapPatch("/api/orders/{id:guid}/cancel", async (Guid id, ISender mediator) =>
        {
            try
            {
            var updated = await mediator.Send(new UpdateOrderStatusCommand(id, OrderStatus.Cancelled));
                return Results.Ok(updated);
            }
            catch (KeyNotFoundException)
            {
                return Results.NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new { ex.Message });
            }
        }).RequireAuthorization();

        return app;
    }
}
using MediatR;
using OrderEngine.Application;

namespace OrderEngine.Api.Endpoints;

public static class OrderEndpoints
{
    public static WebApplication MapOrderEndpoints(this WebApplication app)
    {
        app.MapGet("/api/orders", async (ISender mediator) =>
        {
            var orders = await mediator.Send(new GetOrdersQuery());
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

        app.MapPatch("/api/orders/{id:guid}/status", async (Guid id, UpdateOrderStatusRequest request, ISender mediator) =>
        {
            try
            {
                var updated = await mediator.Send(new UpdateOrderStatusCommand(id, request.Status));
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
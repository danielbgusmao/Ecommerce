using Ecommerce.Application.Orders.Commands.CreateOrder;
using Ecommerce.Application.Orders.Queries.GetOrderById;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ecommerce.Application.Orders.Queries.GetOrders;
using Ecommerce.Application.Orders.Commands.CancelOrder;

namespace Ecommerce.Api.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly ISender _sender;

    public OrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        var orderId = await _sender.Send(
            command,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = orderId },
            new { id = orderId });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var order = await _sender.Send(
            new GetOrderByIdQuery(id),
            cancellationToken);

        if (order is null)
            return NotFound();

        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var result = await _sender.Send(
            new GetOrdersQuery(page, pageSize),
            cancellationToken);

        return Ok(result);
    }
    
    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cancelled = await _sender.Send(
            new CancelOrderCommand(id),
            cancellationToken);

        if (!cancelled)
            return NotFound();

        return NoContent();
    }

    
}
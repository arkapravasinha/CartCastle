using CartCastle.Domain.Commands;
using CartCastle.Service.API.DTOs;
using CartCastle.Services.Core.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Threading;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CartCastle.Service.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // GET: api/<OrderController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<OrderController>/5
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetOrder(Guid id,CancellationToken cancellationToken=default)
        {
            var result = await _mediator.Send(new OrderById(id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        // POST api/<OrderController>
        [HttpPost]
        public async Task<IActionResult> Post(CreateOrderDto createOrderDto, CancellationToken cancellation=default)
        {
            if (!ModelState.IsValid || createOrderDto == null)
                return BadRequest();
            var command = new CreateOrder(Guid.NewGuid(), createOrderDto.CustomerId, createOrderDto.LineItems.Select(x=>new Domain.LineItem(x.ProductId,x.Qty, x.PricePerQuantity)).ToList(), createOrderDto.Address);
            await _mediator.Send(command, cancellation);
            return CreatedAtAction("GetOrder", new { id = command.OrderId }, command);
        }

        // PUT api/<OrderController>/5
        [HttpPut("{id:guid}/AddLineItem")]
        public async Task<IActionResult> AddLineItem([FromRoute]Guid id,LineItemDto lineItemDto, CancellationToken cancellation = default)
        {
            var command = new AddLineItem(id, new Domain.LineItem(lineItemDto.ProductId, lineItemDto.Qty, lineItemDto.PricePerQuantity));
            await _mediator.Send(command, cancellation);
            return CreatedAtAction("GetOrder", new { id = id },command);
        }

        [HttpPut("{id:guid}/RemoveLineItem")]
        public async Task<IActionResult> RemoveLineItem([FromRoute] Guid id, LineItemDto lineItemDto, CancellationToken cancellation = default)
        {
            var command = new RemoveLineItem(id, new Domain.LineItem(lineItemDto.ProductId, lineItemDto.Qty, lineItemDto.PricePerQuantity));
            await _mediator.Send(command, cancellation);
            return CreatedAtAction("GetOrder", new { id = id },command);
        }
    }
}

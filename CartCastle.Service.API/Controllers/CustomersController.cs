using CartCastle.Domain.Commands;
using CartCastle.Service.API.DTOs;
using CartCastle.Services.Core.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CartCastle.Service.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get(CancellationToken cancellationToken=default)
        {
            var result = await _mediator.Send(new AllCustomers(), cancellationToken);
            return result==null ? NotFound():Ok(result);
        }

        
        [HttpGet("{id:guid}",Name ="GetCustomer")]
        public async Task<IActionResult> GetCustomer(Guid id, CancellationToken cancellationToken = default)
        {
            var result=await _mediator.Send(new CustomerById(id), cancellationToken);
            return result == null ? NotFound() : Ok(result);
        }

        
        [HttpPost]
        public async Task<IActionResult> Post(CreateCustomerDto createCustomerDto, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid || createCustomerDto == null)
                return BadRequest();
            var command = new CreateCustomer(Guid.NewGuid(), createCustomerDto.FirstName, createCustomerDto.LastName, createCustomerDto.Email, createCustomerDto.MobileNo);
            await _mediator.Send(command, cancellationToken);
            return CreatedAtAction("GetCustomer", new { id = command.CustomerId }, command);
        }
    }
}

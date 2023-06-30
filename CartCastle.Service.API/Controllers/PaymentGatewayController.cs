using CartCastle.Domain.Commands;
using CartCastle.Service.API.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CartCastle.Service.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentGatewayController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPaymentService _paymentService;

        public PaymentGatewayController(IMediator mediator,IPaymentService paymentService)
        {
            _mediator = mediator;
            _paymentService = paymentService;
        }

        // POST api/<PaymentGatewayController>
        [HttpPost]
        public async Task<IActionResult> Post(CreatePaymentDto createPaymentDto,CancellationToken cancellationToken)
        {
            var result = _paymentService.ProcessPayment(createPaymentDto);
            await _mediator.Send(new PaymentDone(result.OrderId, result.TransactionId, result.IsSuccessfull), cancellationToken);
            return Ok(result);
        }
    }
}

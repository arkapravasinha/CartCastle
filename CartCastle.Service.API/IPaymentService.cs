using CartCastle.Service.API.DTOs;

namespace CartCastle.Service.API
{
    public interface IPaymentService
    {
        PaymentResultDto ProcessPayment(CreatePaymentDto createPaymentDto);
    }
}

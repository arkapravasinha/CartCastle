using CartCastle.Service.API.DTOs;

namespace CartCastle.Service.API
{
    public class FakePaymentService : IPaymentService
    {
        public PaymentResultDto ProcessPayment(CreatePaymentDto createPaymentDto)
        {
            Random rand = new Random();
            return 
                new PaymentResultDto()
                {
                    OrderId = createPaymentDto.OrderId,
                    OrderValue = createPaymentDto.OrderValue,
                    TransactionId = Guid.NewGuid(),
                    IsSuccessfull = rand.NextDouble() >= 0.5,
                    PaymentMethod="CARD"
                };
        }
    }
}

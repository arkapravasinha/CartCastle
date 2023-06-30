using System.ComponentModel.DataAnnotations;

namespace CartCastle.Service.API.DTOs
{
    public class PaymentResultDto
    {
        public Guid OrderId { get; set; }
        public double OrderValue { get; set; }

        public Guid TransactionId { get; set; }
        public bool IsSuccessfull { get; set; }

        public string PaymentMethod { get; set; }
    }
}

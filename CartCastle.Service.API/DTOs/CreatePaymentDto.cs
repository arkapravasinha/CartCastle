using System.ComponentModel.DataAnnotations;

namespace CartCastle.Service.API.DTOs
{
    public class CreatePaymentDto
    {
        [Required]
        public Guid OrderId { get; set; }
        [Required]
        public double OrderValue { get; set; }
    }
}

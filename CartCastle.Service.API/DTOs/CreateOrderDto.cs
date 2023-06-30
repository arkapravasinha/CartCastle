using System.ComponentModel.DataAnnotations;

namespace CartCastle.Service.API.DTOs
{
    public class CreateOrderDto
    {

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public List<LineItemDto> LineItems {get;set;}

        [Required]
        public string Address { get; set; }

    }

    public class LineItemDto
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public int Qty { get; set; }
        [Required]
        public double PricePerQuantity { get; set; }
    }
}

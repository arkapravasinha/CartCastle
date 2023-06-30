using System.ComponentModel.DataAnnotations;

namespace CartCastle.Service.API.DTOs
{
    public class CreateCustomerDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public long MobileNo { get; set; }
    }
}

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models
{
    public class Company
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("Company")]
        public string CompanyName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        [DisplayName("Postal Code")]
        public string? PostalCode { get; set; }
        [DisplayName("Phone Number")]
        public string? PhoneNumber { get; set; }


    }
}

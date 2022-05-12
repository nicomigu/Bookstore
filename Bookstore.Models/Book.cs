using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        [Range(1, 1000)]
        public decimal Price { get; set; }
        [Range(1, 10000)]
        [Display(Name = "Price for more than 100 Copies")]
        public decimal Price100 { get; set; }
        [Range(1, 10000)]
        [Display(Name = "Price for more than 200 Copies")]
        public decimal Price200 { get; set; }
        [ValidateNever]
        public string ImageUrl { get; set; }
        [Required]
        [Display(Name = "Genre")]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }

        [Required]
        [Display(Name = "Type of Cover")]
        public int CoverId { get; set; }
        [ValidateNever]
        public Cover Cover { get; set; }

        [Required]
        public string Author { get; set; }
    }
}

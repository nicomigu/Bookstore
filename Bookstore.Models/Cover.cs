using System.ComponentModel.DataAnnotations;

namespace Bookstore.Models
{
    public class Cover
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Cover Type")]
        [MaxLength(30)]
        public string Name { get; set; }
    }
}

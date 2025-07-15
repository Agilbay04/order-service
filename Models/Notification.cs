using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderService.Models
{
    public class Notification : BaseModel
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Content { get; set; }

        [Required]
        public bool IsRead { get; set; } = false;

        [Required]
        public string Href { get; set; } // Redirect for FE

        [Required]
        public Guid UserId { get; set; }

    }
}

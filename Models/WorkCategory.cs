using System.ComponentModel.DataAnnotations;

namespace WebFrontToBack.Models
{
    public class WorkCategory
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        public bool IsDeleted { get; set; }
    }
}

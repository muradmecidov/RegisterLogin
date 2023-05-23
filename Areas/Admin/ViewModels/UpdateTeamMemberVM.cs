using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebFrontToBack.Areas.Admin.ViewModels
{
    public class UpdateTeamMemberVM
    {
        [Required, MaxLength(50)]
        public string FullName { get; set; }
        [Required]
        public string Profession { get; set; }
        [Required, NotMapped]
        public IFormFile Photo { get; set; }
    }
}

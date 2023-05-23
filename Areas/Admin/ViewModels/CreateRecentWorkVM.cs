using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebFrontToBack.Areas.Admin.ViewModels
{
    public class CreateRecentWorkVM
    {

        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
     
        public IFormFile Photo { get; set; }

    }
}

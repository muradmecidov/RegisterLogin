using Microsoft.AspNetCore.Identity;

namespace WebFrontToBack.Models.Auth
{
    public class AppUser:IdentityUser
    {
        public string Fullname { get; set; }
        public bool IsActive { get; set; }  
    }
}

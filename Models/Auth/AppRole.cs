using Microsoft.AspNetCore.Identity;

namespace WebFrontToBack.Models.Auth
{
    public class AppRole: IdentityRole
    {
        public bool IsActivated { get; set; }
    }
}

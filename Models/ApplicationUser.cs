using Microsoft.AspNetCore.Identity;

namespace API_Laundry.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string NamaLengkap { get; set; }
        public string NomorTelepon { get; set; }
    }
}

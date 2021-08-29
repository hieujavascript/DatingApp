using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    // Join cac entity voi nhau
    public class AppUserRole : IdentityUserRole<int> 
    {
    
       public AppUser User { get; set; }
       public AppRole Role { get; set; }
    }
}
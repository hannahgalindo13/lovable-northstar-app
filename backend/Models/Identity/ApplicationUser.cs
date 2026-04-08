using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace Backend.Models.Identity;

public class ApplicationUser : IdentityUser
{
    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();
}

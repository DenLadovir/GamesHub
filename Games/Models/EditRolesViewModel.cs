using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

public class EditRolesViewModel
{
    public string UserId { get; set; }
    public string UserEmail { get; set; }
    public IList<string> UserRoles { get; set; } = new List<string>();
    public IEnumerable<IdentityRole> AllRoles { get; set; } = new List<IdentityRole>();
}
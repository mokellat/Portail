using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LinkedinClone.Areas.Identity.Data;

// Add profile data for application users by adding properties to the AdminUser class
public class AdminUser : IdentityUser
{
    public string Firstname { get; set; }
    public string lastname { get; set; }
}


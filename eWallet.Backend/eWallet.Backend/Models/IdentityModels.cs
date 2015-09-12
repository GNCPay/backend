﻿using MongoDB.AspNet.Identity;

namespace eWallet.Backend.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
       public string Status { get; set; }
       public string Organization_code { get; set; }
    }

    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    public ApplicationDbContext()
    //        : base("DefaultConnection")
    //    {
    //    }
    //}
}
using MongoDB.AspNet.Identity;
using System.Web.Mvc;
using System.Web;
using System.Web.Security;
using MongoDB.Driver.Builders;

namespace eWallet.Backend.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
       public string Status { get; set; }
    }
    
    //public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    //{
    //    public ApplicationDbContext()
    //        : base("DefaultConnection")
    //    {
    //    }
    //}
}
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace LoginRegWithIdentity.Models
{
    public class MyContext : IdentityDbContext<User>
    {
        public MyContext(DbContextOptions options) : base(options) {}
        public DbSet<User> users {get;set;} // do not use uppercase "Users", it gives warning: 
        // 'MyContext.Users' hides inherited member 'IdentityUserContext<User, string, IdentityUserClaim<string>, IdentityUserLogin<string>, IdentityUserToken<string>>.Users'. To make the current member override that implementation, add the override keyword. Otherwise add the new keyword. [LoginRegWithIdentity]csharp(CS0114)
    }
}
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MamlatdarEcourt.Models;


namespace MamlatdarEcourt.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }


        public DbSet<User> User { get; set; }
        public DbSet<Case> Case {get;set;}
        public DbSet<Advocate> Advocate {get; set;}

    }

}


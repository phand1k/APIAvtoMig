using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using APIAvtoMig.Models;

namespace APIAvtoMig.Auth
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<ModelCar> ModelCars { get; set; }
        public DbSet<WashOrder> WashOrders { get; set; }
        public DbSet<SmsActivate> SmsActivates { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<TypeOfOrganization> TypeOfOrganizations { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}

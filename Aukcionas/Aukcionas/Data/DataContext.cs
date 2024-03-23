using Aukcionas.Auth.Model;
using Aukcionas.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Aukcionas.Data
{

    public class DataContext : IdentityDbContext<ForumRestUser>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Auction> Auctions => Set<Auction>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<Payment> Payments => Set<Payment>();
 
       
    }
}

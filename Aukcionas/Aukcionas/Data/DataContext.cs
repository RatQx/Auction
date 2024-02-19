using Aukcionas.Models;
using Microsoft.EntityFrameworkCore;

namespace Aukcionas.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Auction> Auctions => Set<Auction>();
    }
}

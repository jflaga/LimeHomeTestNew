using LimeHomeTest.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace LimeHomeTest.Repository
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options) {}

        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hotel>().HasKey(x => x.Id);
            modelBuilder.Entity<Booking>().HasKey(x => x.Id);
        }
    }
}
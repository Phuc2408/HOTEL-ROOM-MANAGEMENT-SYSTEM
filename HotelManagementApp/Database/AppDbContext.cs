using Microsoft.EntityFrameworkCore;
using HotelManagementApp.Models;
using HotelManagementApp.Views;

namespace HotelManagementApp.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Room> Room { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Rent> Rent { get; set; }
        public DbSet<Invoice> Invoice { get; set; }
        public DbSet<Service> Service { get; set; }
        public DbSet<ServiceUsage> ServiceUsage { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(LocalDB)\MSSQLLocalDB;Database=HotelDB;Trusted_Connection=True;");
        }
    }
}

using Microsoft.EntityFrameworkCore;
using smartclinic_web.Models;
using smartclinic_web.Data;

namespace smartclinic_web.Data
{
    public class SmartClinicDbContext : DbContext
    {
        public SmartClinicDbContext(DbContextOptions<SmartClinicDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}

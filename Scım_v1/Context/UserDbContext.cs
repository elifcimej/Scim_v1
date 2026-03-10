using Scım_v1.Models;
using Microsoft.EntityFrameworkCore;

namespace Scım_v1.Context
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source = DESKTOP-N9JSA6E\\SQLEXPRESS; database = Scım_v1; Integrated Security= True; TrustServerCertificate=True;");
        //}

        public DbSet<User> Users { get; set; }
    }
}

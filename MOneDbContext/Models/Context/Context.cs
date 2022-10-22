using Microsoft.EntityFrameworkCore;

using System.Configuration;

namespace MOneDbContext.Models.Context
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        protected Context()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["ConnStr"].ConnectionString);
            }
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserInfo> Infos { get; set; }
    }
}


using Microsoft.EntityFrameworkCore;
using Models;

namespace DataAccess
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options) {
        }     
        public DbSet<Queue> Queue { get; set; }
        public DbSet<Place> Place { get; set; }
        public DbSet<Option> Option { get; set; }
        public DbSet<Protocol> Protocol { get; set; }
        public DbSet<Setting> Setting { get; set; }        
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<PlaceOption> PlaceOption { get; set; }
        public DbSet<OptionQueue> OptionQueue { get; set; }
    }
}

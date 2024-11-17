using Apator.Model;
using Microsoft.EntityFrameworkCore;

namespace Apator.Services
{
    public class DataContext : DbContext
    {
        public DbSet<Card> Cards { get; set; }
        public DbSet<User> Users { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=ApatorDB;Integrated Security=True;TrustServerCertificate=True;");
        }

        public void EnsureSeedData()
        {
            if (!Users.Any())
            {
                Users.Add(new User
                {
                    Login = "admin",
                    Password = "admin"
                });

                SaveChanges();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                        .HasIndex(u => u.Login)
                        .IsUnique();

            modelBuilder.Entity<Card>()
                        .HasOne(c => c.User)  
                        .WithMany(u => u.Cards)  
                        .HasForeignKey(c => c.UserId) 
                        .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

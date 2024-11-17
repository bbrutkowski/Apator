using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Apator.Services
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DataContext>();

            optionsBuilder.UseSqlServer("Server=.;Database=ApatorDb;Trusted_Connection=True;TrustServerCertificate=True;");

            return new DataContext(optionsBuilder.Options);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Persistence;

public class GibUserDbContextFactory : IDesignTimeDbContextFactory<GibUserDbContext>
{
    public GibUserDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<GibUserDbContext>();
        optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=GibUserDb;Integrated Security=True;Encrypt=True");
        return new GibUserDbContext(optionsBuilder.Options);
    }
}
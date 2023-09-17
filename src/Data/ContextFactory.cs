using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data;

public class ContextFactory : IDesignTimeDbContextFactory<Context>
{
    public Context CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<Context>()
            .UseNpgsql(args[0])
            .Options;

        return new Context(options);
    }
}
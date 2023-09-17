using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class Context : DbContext
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Post> Posts { get; set; }

    public Context(DbContextOptions<Context> options) : base(options)
    {
    }
}
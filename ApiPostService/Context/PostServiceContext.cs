using ApiPostService.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiPostService.Context;

public class PostServiceContext : DbContext
{
    public PostServiceContext(DbContextOptions<PostServiceContext> options)
       : base(options)
    { }
    public DbSet<Post> Posts { get; set; }
    public DbSet<User> Users { get; set; }
}

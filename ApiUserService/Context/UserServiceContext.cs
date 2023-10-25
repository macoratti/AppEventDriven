using ApiUserService.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiUserService.Context;

public class UserServiceContext : DbContext
{
    public UserServiceContext(DbContextOptions<UserServiceContext> options)
            : base(options)
    {}
    public DbSet<User> Users { get; set; }
}

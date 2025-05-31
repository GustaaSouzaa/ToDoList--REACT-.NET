using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TodoApi.Models;

namespace TodoApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<TodoItem> TodoItems => Set<TodoItem>();
    }
}

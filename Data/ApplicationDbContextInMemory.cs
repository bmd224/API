using Microsoft.EntityFrameworkCore;
using MVC.Models;

namespace MVC.Data
{
    public class ApplicationDbContextInMemory : DbContext
    {
        public ApplicationDbContextInMemory(DbContextOptions options) : base(options) { }
        public required DbSet<Post> Posts { get; set; }
        public required DbSet<Comment> Comments { get; set; }
    }
}

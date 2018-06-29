using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TheWall.Models
{
	public class DataContext : IdentityDbContext<User>
	{
		public DataContext (DbContextOptions<DataContext> options) : base (options) { }
		public DbSet<User> users { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<Comment> Comments { get; set; }
	}
}
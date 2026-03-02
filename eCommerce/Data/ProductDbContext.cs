using eCommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace eCommerce.Data;

public class ProductDbContext : DbContext
{
	public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Ensure the member Username is unique
		modelBuilder.Entity<Member>()
			.HasIndex(m => m.Username)
			.IsUnique();

		// Ensure the member Email is Unique
		modelBuilder.Entity<Member>()
			.HasIndex(m => m.Email)
			.IsUnique();
	}

	// Entities to be tracked by DbContext
	public DbSet<Product> Products { get; set; }

	public DbSet<Member> Members { get; set; }
}

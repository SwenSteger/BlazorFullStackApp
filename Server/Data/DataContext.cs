using Microsoft.EntityFrameworkCore;

namespace BlazorFullStackApp.Server.Data;

public class DataContext : DbContext
{

	public DbSet<SuperHero> SuperHeroes { get; set; }
	public DbSet<Comic> Comics { get; set; }

	public DataContext(DbContextOptions<DataContext> options) : base(options) { }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<SuperHero>()
			.HasOne(s => s.Comic)
			.WithMany()
			.HasForeignKey(s => s.ComicId);
		
		modelBuilder.Entity<Comic>().HasData(
			new Comic{ Id = 1, Name = "Marvel" },
			new Comic{ Id = 2, Name = "DC"}
		);
		modelBuilder.Entity<SuperHero>().HasData(
			new SuperHero
			{
				Id = 1,
				FirstName = "Peter",
				LastName = "Parker",
				HeroName = "Spiderman",
				ComicId = 1,
			},
			new SuperHero
			{
				Id = 2,
				FirstName = "Bruce",
				LastName = "Wayne",
				HeroName = "Batman",
				ComicId = 2,
			}
		);
	}
}

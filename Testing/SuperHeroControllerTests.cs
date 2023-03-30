using BlazorFullStackApp.Server.Controllers;
using BlazorFullStackApp.Server.Data;
using BlazorFullStackApp.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorFullStackApp.Testing;

[TestFixture]
public class SuperHeroControllerTests
{
	private DataContext _context;
	private SuperHeroController _controller;

	[SetUp]
	public void Setup()
	{
		var options = new DbContextOptionsBuilder<DataContext>()
			.UseInMemoryDatabase(databaseName: "SuperHeroesTestDatabase")
			.Options;

		_context = new DataContext(options);
		
		// Remove any existing data from the database
		_context.SuperHeroes.RemoveRange(_context.SuperHeroes);
		_context.Comics.RemoveRange(_context.Comics);
		_context.SaveChanges();
		
		// Seed the database with test data
		var comic1 = new Comic { Id = 1, Name = "Marvel" };
		_context.Comics.Add(comic1);
		var comic2 = new Comic { Id = 2, Name = "DC" };
		_context.Comics.Add(comic2);
		
		var hero1 = new SuperHero { Id = 1, FirstName = "Peter", LastName = "Parker", HeroName = "Spiderman", ComicId = 1 };
		_context.SuperHeroes.Add(hero1);
		var hero2 = new SuperHero { Id = 2, FirstName = "Bruce", LastName = "Wayne", HeroName = "Batman", ComicId = 2 };
		_context.SuperHeroes.Add(hero2);

		_context.SaveChanges();
		
		_controller = new SuperHeroController(_context);
	}

	[Test]
	public async Task GetSuperHeroes_ReturnsOkResultWithListOfSuperHeroes()
	{
		// Arrange
		var dbHeroes = await _context.SuperHeroes.ToListAsync();

		// Act
		var result = await _controller.GetSuperHeroes();

		// Assert
		Assert.IsInstanceOf<OkObjectResult>(result.Result);

		var heroes = ((OkObjectResult)result.Result).Value as List<SuperHero>;

		Assert.IsNotNull(heroes);
		Assert.AreEqual(dbHeroes.Count, heroes.Count);
		for (int i = 0; i < heroes.Count; i++)
			Assert.AreEqual(dbHeroes[i], heroes[i]);
	}

	[Test]
	public async Task GetComics_ReturnsOkResultWithListOfComics()
	{
		// Arrange
		var dbComics = await _context.Comics.ToListAsync();
		var comic1 = new Comic { Id = 1, Name = "Marvel" };
		var comic2 = new Comic { Id = 2, Name = "DC" };

		// Act
		var result = await _controller.GetComics();

		// Assert
		Assert.IsInstanceOf<OkObjectResult>(result.Result);

		var comics = ((OkObjectResult)result.Result).Value as List<Comic>;

		Assert.IsNotNull(comics);
		Assert.AreEqual(dbComics.Count, comics.Count);
		for (int i = 0; i < comics.Count; i++)
			Assert.AreEqual(dbComics[i], comics[i]);
	}

	[Test]
	public async Task GetSuperHero_WithValidId_ReturnsOkResultWithSuperHero()
	{
		// Arrange
		var hero = await _context.SuperHeroes.FirstOrDefaultAsync(x => x.Id == 1);

		// Act
		var result = await _controller.GetSuperHero(1);

		// Assert
		Assert.IsInstanceOf<OkObjectResult>(result.Result);

		var dbHero = ((OkObjectResult)result.Result).Value as SuperHero;

		Assert.IsNotNull(dbHero);
		Assert.AreEqual(hero.FirstName, dbHero.FirstName);
	}

	[Test]
	public async Task GetSuperHero_WithInvalidId_ReturnsNotFoundResult()
	{
		// Arrange

		// Act
		var result = await _controller.GetSuperHero(10);

		// Assert
		Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
	}

	[Test]
	public async Task CreateSuperHero_ReturnsOkResultWithListOfSuperHeroes()
	{
		// Arrange
		var hero = new SuperHero { FirstName = "Clark", LastName = "Kent", HeroName = "Superman", ComicId = 2 };
		
		// Act
		var result = await _controller.CreateSuperHero(hero);

		// Assert
		Assert.IsInstanceOf<OkObjectResult>(result.Result);

		var heroes = ((OkObjectResult)result.Result).Value as List<SuperHero>;

		Assert.IsNotNull(heroes);
		Assert.AreEqual(3, heroes.Count);
		Assert.AreEqual(hero.FirstName, heroes[2].FirstName);
	}

	[Test]
	public async Task UpdateSuperHero_WithValidId_ReturnsOkResultWithListOfSuperHeroes()
	{
		// Arrange
		var hero = await _context.SuperHeroes.FirstOrDefaultAsync(x => x.Id == 1);

		// Act
		var result = await _controller.UpdateSuperHero(hero, 1);

		// Assert
		Assert.IsInstanceOf<OkObjectResult>(result.Result);

		var heroes = ((OkObjectResult)result.Result).Value as List<SuperHero>;

		Assert.IsNotNull(heroes);
		Assert.AreEqual(2, heroes.Count);
		// Interestingly... the heroes returned is not ordered by Id so had to change away from heroes[0].ComicId
		// I wonder if the update reorders things?
		Assert.AreEqual(hero.ComicId, heroes.First(x => x.Id == 1).ComicId);
	}

	[Test]
	public async Task UpdateSuperHero_WithInvalidId_ReturnsNotFoundResult()
	{
		// Arrange
		var hero = new SuperHero { Id = 1, FirstName = "Peter", LastName = "Parker", HeroName = "Spiderman", ComicId = 1 };

		// Act
		var result = await _controller.UpdateSuperHero(hero, 10);

		// Assert
		Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
	}

	[Test]
	public async Task DeleteSuperHero_WithValidId_ReturnsOkResultWithListOfSuperHeroes()
	{
		// Arrange

		// Act
		var result = await _controller.DeleteSuperHero(1);

		// Assert
		Assert.IsInstanceOf<OkObjectResult>(result.Result);

		var heroes = ((OkObjectResult)result.Result).Value as List<SuperHero>;

		Assert.IsNotNull(heroes);
		Assert.AreEqual(1, heroes.Count);
	}

	[Test]
	public async Task DeleteSuperHero_WithInvalidId_ReturnsNotFoundResult()
	{
		// Arrange

		// Act
		var result = await _controller.DeleteSuperHero(100);

		// Assert
		Assert.IsInstanceOf<NotFoundObjectResult>(result.Result);
	}
}

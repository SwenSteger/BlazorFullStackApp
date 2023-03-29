using BlazorFullStackApp.Server.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorFullStackApp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuperHeroController : ControllerBase
{
	private readonly DataContext _context;

	public SuperHeroController(DataContext context)
	{
		_context = context;
	}

	[HttpGet]
	public async Task<ActionResult<List<SuperHero>>> GetSuperHeroes()
	{
		var heroes = await _context.SuperHeroes.Include(h => h.Comic).ToListAsync();
		return Ok(heroes);
	}

	[HttpGet("comics")]
	public async Task<ActionResult<List<Comic>>> GetComics()
	{
		var comics = await _context.Comics.ToListAsync();
		return Ok(comics);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<SuperHero>> GetSuperHero(int id)
	{
		var hero = await _context.SuperHeroes
			.Include(h => h.Comic)
			.FirstOrDefaultAsync(x => x.Id == id);
		
		if (hero is null)
			return NotFound("Sorry, no hero found :(");
		return Ok(hero);
	}
	
	[HttpPost]
	public async Task<ActionResult<List<SuperHero>>> CreateSuperHero(SuperHero hero)
	{
		hero.Comic = null;
		_context.SuperHeroes.Add(hero);
		await _context.SaveChangesAsync();
		
		return Ok(await GetDbHeroes());
	}

	[HttpPut("{id:int}")]
	public async Task<ActionResult<List<SuperHero>>> UpdateSuperHero(SuperHero hero, int id)
	{
		var dbHero = await _context.SuperHeroes
			.Include(h => h.Comic)
			.FirstOrDefaultAsync(x => x.Id == id);
		
		if (dbHero is null)
			return NotFound("Sorry, hero not found.");

		dbHero.FirstName = hero.FirstName;
		dbHero.LastName = hero.LastName;
		dbHero.HeroName = hero.HeroName;
		dbHero.ComicId = hero.ComicId;
		
		await _context.SaveChangesAsync();
		
		return Ok(await GetDbHeroes());
	}
	
	[HttpDelete("{id:int}")]
	public async Task<ActionResult<List<SuperHero>>> DeleteSuperHero(int id)
	{
		var dbHero = await _context.SuperHeroes
			.Include(h => h.Comic)
			.FirstOrDefaultAsync(x => x.Id == id);
		
		if (dbHero == null)
			return NotFound("Sorry, hero not found.");

		_context.SuperHeroes.Remove(dbHero);
		await _context.SaveChangesAsync();
		
		return Ok(await GetDbHeroes());
	}
	
	private async Task<List<SuperHero>> GetDbHeroes()
	{
		return await _context.SuperHeroes.Include(x => x.Comic).ToListAsync();
	}
}

using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;

namespace BlazorFullStackApp.Client.Services.SuperHeroService;

public class SuperHeroService : ISuperHeroService
{
	public List<SuperHero> Heroes { get; set; } = new();
	public List<Comic> Comics { get; set; } = new();
	
	private readonly HttpClient _http;
	private readonly NavigationManager _navigationManager;

	public SuperHeroService(HttpClient http, NavigationManager navigationManager)
	{
		_http = http;
		_navigationManager = navigationManager;
	}
	
	public async Task GetComics()
	{
		var result = await _http.GetFromJsonAsync<List<Comic>>("api/superhero/comics");
		if (result is not null)
			Comics = result;
	}

	public async Task GetSuperHeroes()
	{
		var result = await _http.GetFromJsonAsync<List<SuperHero>>("api/superhero");
		if (result is not null)
			Heroes = result;
	}

	public async Task<SuperHero> GetSuperHero(int id)
	{
		var result = await _http.GetFromJsonAsync<SuperHero>($"api/superhero/{id}");
		if (result is not null)
			return result;
		
		throw new Exception("Hero not found!");
	}

	public async Task CreateHero(SuperHero hero)
	{
		var result = await _http.PostAsJsonAsync("api/superhero", hero);
		await SetHeroes(result);
	}

	public async Task UpdateHero(SuperHero hero)
	{
		var result = await _http.PutAsJsonAsync($"api/superhero/{hero.Id}", hero);
		await SetHeroes(result);
	}

	public async Task DeleteHero(int id)
	{
		var result = await _http.DeleteAsync($"api/superhero/{id}");
		await SetHeroes(result);
	}

	private async Task SetHeroes(HttpResponseMessage result)
	{
		var response = await result.Content.ReadFromJsonAsync<List<SuperHero>>();
		if (response is not null)
			Heroes = response;
		
		_navigationManager.NavigateTo("superheroes");
	}
}

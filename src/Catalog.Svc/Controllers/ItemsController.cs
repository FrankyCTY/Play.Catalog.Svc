using Catalog.svc.Dtos;
using Catalog.Svc.Entities;
using Catalog.Svc.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Svc.Controllers;

// https://localhost:5001/items
[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
	private readonly ItemsRepository itemsRepository = new();

	[HttpGet]
	public async Task<IEnumerable<ItemDto>> GetAsync()
	{
		var items = await itemsRepository.GetAllAsync();

		var itemDtos = items.Select(item => item.AsDto());
		return itemDtos;
	}

	[HttpGet("{id}")]
	public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
	{
		var item = await itemsRepository.GetAsync(id);

		if (item is null)
		{
			return NotFound();
		}

		return item.AsDto();
	}

	[HttpPost]
	public async Task<ActionResult<ItemDto>> CreateAsync(CreateItemDto createItemDto)
	{
		var newItem = new Item
		{
			Name = createItemDto.Name,
			Description = createItemDto.Description,
			Price = createItemDto.Price,
			CreatedDate = DateTimeOffset.UtcNow
		};

		await itemsRepository.CreateAsync(newItem);

		return CreatedAtAction(nameof(GetByIdAsync), new { id = newItem.Id }, newItem);
	}

	[HttpPut("{id}")]
	public async Task<IActionResult> UpdateAsync(Guid id, UpdateItemDto updateItemDto)
	{
		var existingItem = await itemsRepository.GetAsync(id);

		if (existingItem is null)
		{
			// Create the item if not exist
			var newItem = new Item
			{
				Name = updateItemDto.Name,
				Description = updateItemDto.Description,
				Price = updateItemDto.Price,
				CreatedDate = DateTimeOffset.UtcNow
			};

			await itemsRepository.CreateAsync(newItem);

			return NoContent();
		}

		existingItem.Name = updateItemDto.Name;
		existingItem.Description = updateItemDto.Description;
		existingItem.Price = updateItemDto.Price;

		await itemsRepository.UpdateAsync(existingItem);

		return NoContent();
	}

	[HttpDelete("{id}")]
	public async Task<IActionResult> DeleteAsync(Guid id)
	{
		var existingItem = await itemsRepository.GetAsync(id);

		if (existingItem is null)
		{
			return NotFound();
		}

		await itemsRepository.RemoveAsync(id);

		return NoContent();
	}
}
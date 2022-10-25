using Catalog.svc.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Svc.Controllers;

// https://localhost:5001/items
[ApiController]
[Route("items")]
public class ItemsController : ControllerBase
{
	private static readonly List<ItemDto> items = new()
	{
		new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
		new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
		new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow),
	};

	[HttpGet]
	public IEnumerable<ItemDto> Get()
	{
		return items;
	}

	[HttpGet("{id}")]
	public ActionResult<ItemDto> GetById(Guid id)
	{
		var item = items.Where((item) => item.Id == id).SingleOrDefault();

		if (item is null)
		{
			return NotFound();
		}

		return item;
	}

	[HttpPost]
	public ActionResult<ItemDto> Create(CreateItemDto createItemDto)
	{
		var newItem = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);
		items.Add(newItem);

		return CreatedAtAction(nameof(GetById), new { id = newItem.Id }, newItem);
	}

	[HttpPut("{id}")]
	public IActionResult Update(Guid id, UpdateItemDto updateItemDto)
	{
		var existingItem = items.Where((item) => item.Id == id).SingleOrDefault();

		if (existingItem is null)
		{
			// Create the item if not exist
			var newItem = new ItemDto(Guid.NewGuid(), updateItemDto.Name, updateItemDto.Description, updateItemDto.Price, DateTimeOffset.UtcNow);
			items.Add(newItem);

			return NoContent();
		}

		var updatedItem = existingItem with
		{
			Name = updateItemDto.Name,
			Description = updateItemDto.Description,
			Price = updateItemDto.Price
		};

		var existingItemIdx = items.FindIndex(item => item.Id == id);
		items[existingItemIdx] = updatedItem;

		return NoContent();
	}

	[HttpDelete("{id}")]
	public IActionResult Delete(Guid id)
	{
		var targetItemIdx = items.FindIndex((item) => item.Id == id);

		if (targetItemIdx == -1)
		{
			return NotFound();
		}

		items.RemoveAt(targetItemIdx);

		return Ok();
	}
}
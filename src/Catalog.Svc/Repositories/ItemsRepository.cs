using Catalog.Svc.Entities;
using MongoDB.Driver;

namespace Catalog.Svc.Repositories;

public class ItemRepository
{
	private const string collectionName = "items";
	private readonly IMongoCollection<Item> dbCollection;
	private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

	public ItemRepository()
	{
		var mongoClient = new MongoClient("mongodb://localhost:27017");
		var database = mongoClient.GetDatabase("Catalog");

		dbCollection = database.GetCollection<Item>(collectionName);
	}

	public async Task<IReadOnlyCollection<Item>> GetAllAsync()
	{
		return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
	}

	public async Task<Item> GetAsync(Guid id)
	{
		FilterDefinition<Item> filter = filterBuilder.Eq(entity => entity.Id, id);
		return await dbCollection.Find(filter).FirstOrDefaultAsync();
	}

	public async Task CreateAsync(Item entity)
	{
		if (entity is null)
		{
			throw new ArgumentNullException(nameof(entity));
		}

		await dbCollection.InsertOneAsync(entity);
	}

	public async Task UpdateAsync(Item entity)
	{
		if (entity is null)
		{
			throw new ArgumentNullException(nameof(entity));
		}

		FilterDefinition<Item> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
		await dbCollection.ReplaceOneAsync(filter, entity);
	}

	public async Task RemoveAsync(Guid id)
	{
		FilterDefinition<Item> filter = filterBuilder.Eq(entity => entity.Id, id);

		await dbCollection.DeleteOneAsync(filter);
	}
}
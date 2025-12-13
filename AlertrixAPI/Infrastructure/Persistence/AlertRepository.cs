using AlertrixAPI.Application.Interfaces;
using AlertrixAPI.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AlertrixAPI.Infrastructure.Persistence
{
    public class MongoSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }

    public class AlertRepository : IAlertRepository
    {
        private readonly IMongoCollection<Alert> _collection;

        public AlertRepository(IOptions<MongoSettings> opts)
        {
            var settings = opts.Value;
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            _collection = db.GetCollection<Alert>("Alerts");
        }

        public async Task<List<Alert>> GetAsync(string userId, int page, int pageSize, CancellationToken cancellationToken)
        {
            var filter = Builders<Alert>.Filter.Eq(a => a.UserId, userId);
            var skip = (page - 1) * pageSize;
            var find = _collection.Find(filter)
                            .SortByDescending(a => a.CreatedAt)
                            .Skip(skip)
                            .Limit(pageSize);
            return await find.ToListAsync(cancellationToken);
        }

        public async Task<Alert?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var filter = Builders<Alert>.Filter.Eq(a => a.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task CreateAsync(Alert alert, CancellationToken cancellationToken)
        {
            await _collection.InsertOneAsync(alert, null, cancellationToken);
        }

        public async Task<bool> DeleteAsync(string id, string userId, CancellationToken cancellationToken)
        {
            var filter = Builders<Alert>.Filter.And(
                Builders<Alert>.Filter.Eq(a => a.Id, id),
                Builders<Alert>.Filter.Eq(a => a.UserId, userId)
            );
            var result = await _collection.DeleteOneAsync(filter, cancellationToken);
            return result.DeletedCount > 0;
}

        public async Task<long> CountAsync(string userId, CancellationToken cancellationToken)
        {
            var filter = string.IsNullOrWhiteSpace(userId) ? Builders<Alert>.Filter.Empty : Builders<Alert>.Filter.Eq(a => a.UserId, userId);
            return await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        }
    }
}

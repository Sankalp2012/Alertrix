using AlertrixAPI.Application.Interfaces;
using AlertrixAPI.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace AlertrixAPI.Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;

        public UserRepository(IOptions<MongoSettings> opts)
        {
            var settings = opts.Value;
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            _collection = db.GetCollection<User>("Users");
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Email, email.ToLowerInvariant());
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<User?> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task CreateAsync(User user, CancellationToken cancellationToken)
        {
            await _collection.InsertOneAsync(user, null, cancellationToken);
        }

        public async Task UpdateAsync(User user, CancellationToken cancellationToken)
        {
            var filter = Builders<User>.Filter.Eq(u => u.Id, user.Id);
            await _collection.ReplaceOneAsync(filter, user, new ReplaceOptions { IsUpsert = false }, cancellationToken);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            var filter = Builders<User>.Filter.ElemMatch(u => u.RefreshTokens, rt => rt.Token == refreshToken);
            return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }
    }
}

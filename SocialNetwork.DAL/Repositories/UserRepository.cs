using MongoDB.Driver;
using SocialNetwork.DAL.Repositories.Base;
using SocialNetwork.DAL.Resources;
using SocialNetwork.DTO.Entities;

namespace SocialNetwork.DAL.Repositories
{
    public class UserRepository : BaseRepository
    {
        public UserRepository(ResourceDbContext context) : base(context)
        {

        }

        public Task CreateAsync(string userId, Profile profile)
        {
            var user = new User() { Id = userId, Profile = profile };

            return CreateAsync(user);
        }

        public Task CreateAsync(string userId, string name, string gender, string image)
        {
            var user = new User
            {
                Id = userId,
                Profile = new Profile(name, image, gender)
            };

            return CreateAsync(user);
        }

        public async Task CreateAsync(User user)
        {
            await Task.WhenAll(_context.Users.InsertOneAsync(user),
                _context.Wall.InsertOneAsync(new Feed { UserId = user.Id }),
                _context.News.InsertOneAsync(new Feed { UserId = user.Id }));
        }

        public async Task<bool> FollowAsync(string userId, string destId)
        {
            var profile = await ProfileAsync(userId);

            if (profile != null)
            {
                var path = "Followers." + userId;

                var update = Builders<User>.Update.Set(path, profile)
                    .Set(x => x.Meta.Updated, DateTime.UtcNow);

                var result = await _context.Users.UpdateOneAsync(x => x.Id == destId, update, new UpdateOptions()
                {
                    IsUpsert = true
                });

                return result.ModifiedCount > 0;
            }

            return false;
        }

        public Task<Profile> ProfileAsync(string userId)
        {
            return _context.Users.Find(x => x.Id == userId).Project(x => x.Profile).SingleOrDefaultAsync();
        }

    }
}

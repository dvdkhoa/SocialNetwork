using MongoDB.Driver;
using SocialNetwork.DAL.Repositories.Base;
using SocialNetwork.DAL.Resources;
using SocialNetwork.DTO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.DAL.Repositories
{
    public class NotifyRepository : BaseRepository
    {
        public NotifyRepository(ResourceDbContext context) : base(context)
        {

        }

        public async Task CreateAsync(string userId, string message, string thumnail, string intent)
        {
            Notification notification = new Notification
            {
                Created = DateTime.Now,
                Message = message,
                Intent = intent,
                Seen = false,
                Thumbnail = thumnail
            };

            var filter = Builders<User>.Filter.Eq(u => u.Id, userId);

            var update = Builders<User>.Update.Push("Notifications", notification);

            var results = await _context.Users.UpdateOneAsync(filter, update);
        }
    }
}

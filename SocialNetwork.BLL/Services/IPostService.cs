using SocialNetwork.DTO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BLL.Services
{
    public interface IPostService
    {
        public Task<List<Post>> GetNewsFeed(string userId, int page = 0);
        public Task<List<Post>> GetWallFeed(string userId, int page = 0);
        public Task<Post> CreatePostAsync(string userId, string text, params Photo[] photos);
        public Task<Comment> CommentAsync(string userId, string postId, string text);
        public Task<int> LikeAsync(string userId, string postId);
    }
}

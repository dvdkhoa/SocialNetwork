using SocialNetwork.DAL.Repositories;
using SocialNetwork.DTO.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialNetwork.BLL.Services.Implements
{
    public class PostService : IPostService
    {
        private readonly PostRepository _postRepository;

        public PostService(PostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Comment> CommentAsync(string userId, string postId, string text)
        {
            await _postRepository.CommentAsync(userId, postId, text);

            var post = await _postRepository.GetPostById(postId);

            var newComment = post.Comments.OrderByDescending(cmt => cmt.Ts).FirstOrDefault();

            return newComment;
        }

        public async Task<Post> CreatePostAsync(string userId, string text, params Photo[]? photos)
        {
            await _postRepository.CreateAsync(userId, text, photos);

            return _postRepository.GetNewPost();
        }

        public async Task<List<Post>> GetNewsFeed(string userId, int page = 0)
        {
            return await _postRepository.GetNewsFeed(userId, page);
        }

        public async Task<List<Post>> GetWallFeed(string userId, int page = 0)
        {
            return await _postRepository.GetWallFeed(userId, page);
        }
        public async Task<int> LikeAsync(string userId, string postId)
        {
            var exists = await _postRepository.LikeExists(userId, postId);

            if(exists)
                await _postRepository.UnLikeAsync(userId, postId);
            else
                await _postRepository.LikeAsync(userId, postId);

            var post = await _postRepository.GetPostById(postId);

            return post.Likes.Count();
        }
    }
}

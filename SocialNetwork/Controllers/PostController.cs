using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using SocialNetwork.BLL.Services;
using SocialNetwork.DTO.Entities;
using SocialNetwork.DTO.Posts;
using SocialNetwork.DTO.Shared;

namespace SocialNetwork.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IWebHostEnvironment _env;

        public PostController(IPostService postService, IWebHostEnvironment env)
        {
            _postService = postService;
            _env = env;
        }

        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePostAsync([FromForm]CreatePostModel createPostModel)
        {
            string PhotoName = "";
            if(createPostModel.PhotoFile.Length > 0)
            {
                var path = Path.Combine(_env.WebRootPath, "imgs", createPostModel.PhotoFile.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await createPostModel.PhotoFile.CopyToAsync(stream);
                }
                PhotoName = Path.Combine("imgs", createPostModel.PhotoFile.FileName);
            }

            var list = new List<Photo>
            {
                new Photo
                {
                    Id = Guid.NewGuid().ToString(),
                    Url = PhotoName
                }
            };

            var newPost = await _postService.CreatePostAsync(createPostModel.UserId, createPostModel.Text, list.ToArray());

            var newPostViewModel = new
            {
                id = newPost.Id.ToString(),
                newPost.By,
                newPost.Type,
                newPost.Meta,
                newPost.Detail,
                newPost.Comments,
                newPost.Likes
            };

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Data = newPostViewModel,
                Message = "Create post successfully"
            });
        }

        [HttpGet("GetNews")]
        public async Task<IActionResult> GetNews(string userId)
        {
            var posts = await _postService.GetNewsFeed(userId, 0);

            var postsViewModel = posts.Select(post =>
            {
                return new
                {
                    id = post.Id.ToString(),
                    post.By,
                    post.Type,
                    post.Meta,
                    post.Detail,
                    post.Comments,
                    post.Likes
                };
            });
   

            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Data = postsViewModel
            });
        }

        [HttpGet("GetWall")]
        public async Task<IActionResult> GetWall(string userId)
        {
            var posts = await _postService.GetWallFeed(userId, 0);

            var postsViewModel = posts.Select(post =>
            {
                return new
                {
                    id = post.Id.ToString(),
                    post.By,
                    
                    post.Type,
                    post.Detail,
                    post.Comments,
                    post.Likes
                };
            });
            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Data = postsViewModel
            });
        }

        [HttpPost("Comment")]
        public async Task<IActionResult> Comment([FromBody] CommentModel commentModel)
        {
            var newComment = await _postService.CommentAsync(commentModel.UserId, commentModel.PostId, commentModel.Text);
            return Ok(new ApiResponse
            {
                IsSuccess = true,
                Data = newComment,
                Message = "Comment successfully"
            });
        }

        [HttpPost("Like")]
        public async Task<IActionResult> Like([FromBody] LikeModel likeModel)
        {
            var likeCount = await _postService.LikeAsync(likeModel.UserId, likeModel.PostId);
            return Ok(new ApiResponse
            {
                Data = new { likeCount = likeCount },
                IsSuccess = true
            });
        }
    }
}

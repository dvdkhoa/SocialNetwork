﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using SocialNetwork.Api.Hubs;
using SocialNetwork.BLL.Services;
using SocialNetwork.BLL.Services.Implements;
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
        private readonly IHubContext<NotifyHub> _notifyHubContext;

        public PostController(IPostService postService, IWebHostEnvironment env, IHubContext<NotifyHub> notifyHubContext)
        {
            _postService = postService;
            _env = env;
            _notifyHubContext = notifyHubContext;
        }

        [HttpPost("CreatePost")]
        public async Task<IActionResult> CreatePostAsync([FromForm]CreatePostModel createPostModel)
        {
            string PhotoName = "";
            List<Photo>? list = null;
            if (createPostModel.PhotoFile?.Length > 0)
            {
                var path = Path.Combine(_env.WebRootPath, "imgs", createPostModel.PhotoFile.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await createPostModel.PhotoFile.CopyToAsync(stream);
                }
                PhotoName = Path.Combine("imgs", createPostModel.PhotoFile.FileName);

                list = new List<Photo>
                {
                    new Photo
                    {
                        Id = Guid.NewGuid().ToString(),
                        Url = PhotoName
                    }
                };
            }

            var newPost = await _postService.CreatePostAsync(createPostModel.UserId, createPostModel.Text, list?.ToArray());

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
            

            await _notifyHubContext.Clients.All.SendAsync("receiveMessage", $"Tài khoản có userId: {createPostModel.UserId} đăng tải {createPostModel.Text}");

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

            var postsViewModel = posts?.Select(post =>
            {
                return new
                {
                    id = post.Id.ToString(),
                    post.By,
                    post.Meta,
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


        [HttpGet("Comments")]
        public async Task<IActionResult> GetComments(string postId)
        {
            var comments = await _postService.GetCommentsByPostId(postId);

            return Ok(new ApiResponse
            {
                Data= comments,
                IsSuccess = true

            });
        }


        [HttpPut("UpdatePost")]
        public async Task<IActionResult> UpdatePostAsync(string postId, string text)
        {
            await _postService.UpdatePostAsync(postId, text);
            return Ok();
        }

        [HttpDelete("DeletePost")]
        public async Task<IActionResult> DeletePostAsync(string postId)
        {
            await _postService.DeletePostAsync(postId);
            return Ok();
        }
    }
}

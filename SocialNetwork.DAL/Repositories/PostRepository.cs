using MongoDB.Bson;
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
    public class PostRepository : FeedRepository
    {
        public PostRepository(ResourceDbContext context) : base(context)
        {

        }

        public Task CreateAsync(string userId, string text, params Photo[] photos)
        {
            var profile = _context.Users.Find(x => x.Id == userId)
                .Project(x => x.Profile)
                .SingleOrDefault();

            if (profile != null)
            {
                if (photos?.Any(x => x.Url != null) == true)
                    return CreateAsync(new Owner(userId, profile), text, photos.ToList());

                return CreateAsync(new Owner(userId, profile), text);
            }

            return Task.CompletedTask;

        }

        public Task CreateAsync(Owner owner, string text, List<Photo> photos)
        {
            if (photos == null)
                throw new ArgumentNullException(nameof(photos));

            var post = new Post
            {
                By = owner,
                Detail = new Detail { Text = text, Photos = photos },
                Type = PostType.Photo,
            };

            return CreateAsync(post);
        }

        public async Task CreateAsync(Owner owner, string text)
        {
            var post = new Post
            {
                By = owner,
                Detail = new Detail { Text = text },
                Type = PostType.Status
            };

            await CreateAsync(post);
        }

        async Task CreateAsync(Post post)
        {
            await _context.Posts.InsertOneAsync(post);

            await AppendPostAsync(post);
        }

        public Task CommentAsync(string userId, string postId, string text)
        {
            var profile = _context.Users.Find(x => x.Id == userId)
                .Project(x => x.Profile).SingleOrDefault();

            if (profile != null)
                return CommentAsync(new Owner(userId, profile), postId, text);

            return Task.CompletedTask;
        }

        public async Task CommentAsync(Owner user, string postId, string text)
        {
            var comment = new Comment { By = user, Text = text, Ts = DateTime.UtcNow };

            await _context.Posts.UpdateOneAsync(
                Builders<Post>.Filter.Eq("_id", ObjectId.Parse(postId)),
                Builders<Post>.Update.Push(nameof(Post.Comments), comment));

            await AppendCommentAsync(postId, comment);
        }

        public async Task LikeAsync(string userId, string postId)
        {
            var profile = getProfileByUserId(userId);

            if (profile != null)
            {
                var owner = new Owner(userId, profile);
                var like = new Like { By=owner, Ts = DateTime.UtcNow };

                await _context.Posts.UpdateOneAsync(
                        Builders<Post>.Filter.Eq("_id", ObjectId.Parse(postId)),
                        Builders<Post>.Update.AddToSet("Likes", like));
                await AppendLikeAsync(postId, like);
            }
        }

        public async Task<bool> LikeExists(string userId, string postId)
        {
            var profile = getProfileByUserId(userId);

            //var postFilter = new BsonDocument { { "_id", ObjectId.Parse(postId) } };
            //var likeFilter = new BsonDocument { { "Likes.By._id", userId } };

            var postFilter = Builders<Post>.Filter.Eq("_id", ObjectId.Parse(postId));
            var likeFilter = Builders<Post>.Filter.Eq("Likes.By._id", userId);  
            

            var posts = await _context.Posts.FindAsync(Builders<Post>.Filter.And(postFilter,likeFilter));

            return posts.Any();
        }
        public async Task UnLikeAsync(string userId, string postId)
        {
            var profile = getProfileByUserId(userId);

            if (profile != null)
            {

                var update = new BsonDocument
                        {
                            { "$pull", new BsonDocument{ { "Likes", new BsonDocument { { "By._id", userId } } } } }
                        };

                await _context.Posts.UpdateOneAsync(
                        Builders<Post>.Filter.Eq("_id", ObjectId.Parse(postId)), update);
                await AppendUnLikeAsync(postId, userId);
            }
        }
        private Profile getProfileByUserId(string userId)
        {
            return _context.Users.Find(user => user.Id == userId)
                                        .Project(user => user.Profile)
                                        .SingleOrDefault();
        }

        public Task<Post> GetPostById(string postId)
        {
            return _context.Posts.Find(post => post.Id == ObjectId.Parse(postId)).FirstOrDefaultAsync();
        }

        public Post GetNewPost()
        {
            var newPost = _context.Posts.AsQueryable().OrderByDescending(p => p.Meta.Created).FirstOrDefault();

            return newPost;
        }


        public async Task<List<Comment>> GetCommentsByPostId(string postId)
        {
            var post = await (await _context.Posts.FindAsync(p => p.Id == ObjectId.Parse(postId))).FirstOrDefaultAsync();


            return post.Comments;
        }
    }
}

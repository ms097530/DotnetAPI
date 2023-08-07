using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    // * Authorize attribute makes authenticated used required
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PostController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        public PostController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet]
        public IEnumerable<Post> GetPosts()
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts";

            IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);

            return posts;
        }

        [HttpGet("{postId}")]
        public Post GetPost(int postId)
        {
            string sql = $@"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                    WHERE PostId = {postId}";

            var post = _dapper.LoadDataSingle<Post>(sql);

            return post;
        }

        [HttpGet("PostsByUser/{userId}")]
        public IEnumerable<Post> GetUserPosts(int userId)
        {
            string sql = $@"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated]
                FROM TutorialAppSchema.Posts
                    WHERE UserId = {userId}";

            IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);

            return posts;
        }

        [HttpGet("MyPosts")]
        public IEnumerable<Post> GetMyPosts()
        {
            // * User is from ControllerBase and is associated with user executing action -> FindFirst finds first claim with match -> Value gives value of claim
            // * user used below has access to token
            string sql = $@"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated]
                FROM TutorialAppSchema.Posts
                    WHERE UserId = {this.User.FindFirst("userId")?.Value}";

            IEnumerable<Post> posts = _dapper.LoadData<Post>(sql);

            return posts;
        }

        [HttpGet("search")]
        // * ASP.NET Core will automatically bind form values, route values and query strings by name, so below works for pulling from query string
        public IEnumerable<Post> SearchPosts([FromQuery] string title = "", [FromQuery] string content = "")
        {
            Console.WriteLine($"Title: {title} Content: {content}");

            string sql = $@"SELECT * FROM TutorialAppSchema.Posts
                WHERE PostTitle LIKE '%{title}%'
                    OR PostContent LIKE '%{content}%'";


            return _dapper.LoadData<Post>(sql);
        }

        [HttpPost]
        public IActionResult AddPost(PostToAddDTO postToAdd)
        {
            // * NOTE: using SQL GETDATE() function to populate PostCreated and PostUpdated
            string sql = $@"INSERT INTO TutorialAppSchema.Posts(
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated]) VALUES (
                    {this.User.FindFirst("userId")?.Value},
                    '{postToAdd.PostTitle}',
                    '{postToAdd.PostContent}',
                    GETDATE(),
                    GETDATE()
                )";

            if (_dapper.ExecuteSql(sql))
                return Ok();

            throw new Exception("Unable to add post");
        }

        [HttpPut]
        public IActionResult EditPost(PostToEditDTO postToEdit)
        {
            // string sql = $@"UPDATE TutorialAppSchema.Posts SET 
            //     PostTitle = '{postToEdit.PostTitle}',
            //     PostContent = '{postToEdit.PostContent}',
            //     PostUpdated = GETDATE()            
            // WHERE PostId = {postToEdit.PostId}
            // AND UserId = {this.User.FindFirst("userId")?.Value}";

            System.Console.WriteLine("{0} {1} {2}", postToEdit.PostTitle, postToEdit.PostContent, postToEdit.PostId);

            string sql = $@"UPDATE TutorialAppSchema.Posts SET 
                        PostTitle = '{postToEdit.PostTitle}',
                        PostContent = '{postToEdit.PostContent}',
                        PostUpdated = GETDATE()
                    WHERE PostId = {postToEdit.PostId}
                        AND UserId = {this.User.FindFirst("userId")?.Value}";

            if (_dapper.ExecuteSql(sql))
                return Ok();

            throw new Exception("Unable to edit post");
        }

        // * allows user to delete own post
        // ? add another delete method to allow another user to delete someone's post with elevated authorization?
        [HttpDelete("{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string sql = $@"DELETE FROM TutorialAppSchema.Posts
                WHERE PostId = {postId}
                    AND UserId = {User.FindFirst("userId")?.Value}";

            if (_dapper.ExecuteSql(sql))
            {
                return Ok();
            }

            throw new Exception("Unable to delete post");
        }
    }
}
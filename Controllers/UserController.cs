using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetAPI;
using DotnetAPI.Data;
using DotnetAPI.DTO;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

// ? good practice to use sub-namespaces so things can be loaded separately/modularly
namespace DotnetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        DataContextDapper _dapper;
        public UserController(IConfiguration config)
        {
            // * configuration object provides access to this from appsettings.json - unique to .NET 6+
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("TestConnection")]
        public DateTime TestConnection()
        {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
        }


        [HttpGet]
        // public ActionResult<IEnumerable<User>> GetUsers()
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            // Console.WriteLine("GETTING USERS");
            // return _dapper.LoadData<User>("SELECT * FROM TutorialAppSchema.Users").ToList();
            return _dapper.LoadData<User>("SELECT * FROM TutorialAppSchema.Users").ToArray();
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id, [FromQuery] int code)
        {
            string sql = $@"
                SELECT * FROM TutorialAppSchema.Users
                WHERE  UserId = {id}
            ";
            try
            {
                ActionResult<User> result = _dapper.LoadDataSingle<User>(sql);
                return result;
            }
            catch
            {
                // * using anonymous object to determine contents of bad request response
                return BadRequest(new { Message = "AHHH", StatusCode = 400 });
                // return StatusCode(400, "Couldn't find a match");
            }
        }

        [HttpPost]
        // * use UserDTO because we just need temporary mapping, not full User object (don't need ID)
        public IActionResult AddUser(UserDTO user)
        {
            Console.WriteLine("ADD USER");

            string sql = @$"
                INSERT INTO TutorialAppSchema.Users(
                    [FirstName],
                    [LastName],
                    [Email],
                    [Gender],
                    [Active]
                ) VALUES (
                    '{user.FirstName}',
                    '{user.LastName}',
                    '{user.Email}',
                    '{user.Gender}',
                    {Convert.ToInt32(user.Active)}
                )
            ";

            bool wasSuccessful = _dapper.ExecuteSql(sql);

            if (wasSuccessful)
            {
                return Ok();
            }

            throw new Exception("Unable to add user");
        }

        [HttpPut("{id}")]
        // * can get values from body using appropriate attribute
        // public IActionResult EditUser(int id, [FromBody] string value)
        // * when accepting a Model, a model is constructed based on provided body
        public IActionResult EditUser(User user)
        {
            // Console.WriteLine(id);
            // Console.WriteLine(value);

            string sql = @$"
            UPDATE TutorialAppSchema.Users
                SET
                [FirstName] = '{user.FirstName}',
                [LastName] = '{user.LastName}',
                [Email] = '{user.Email}',
                [Gender] = '{user.Gender}',
                [Active] = {Convert.ToInt32(user.Active)}
                    WHERE UserId = {user.UserId}
            ";

            Console.WriteLine(sql);
            // Console.WriteLine(Convert.ToBoolean(1));
            // Console.WriteLine(Convert.ToInt32(user.Active));

            bool wasSuccessful = _dapper.ExecuteSql(sql);
            Console.WriteLine(wasSuccessful);

            if (wasSuccessful)
            {
                // comes with ControllerBase class
                return Ok();
            }

            throw new Exception("Unable to update user");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {

            string sql = @$"
                DELETE FROM TutorialAppSchema.Users
                    WHERE UserId = {id}
            ";

            bool wasSuccessful = _dapper.ExecuteSql(sql);

            if (wasSuccessful)
            {
                return Ok();
            }

            throw new Exception("Unable to delete user");
        }
    }
}
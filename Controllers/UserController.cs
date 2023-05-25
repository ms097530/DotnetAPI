using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotnetAPI;
using Microsoft.AspNetCore.Mvc;

namespace Namespace
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
        public void Post([FromBody] string value)
        {
            Console.WriteLine("HELLO");
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            Console.WriteLine(id);
            Console.WriteLine(value);
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
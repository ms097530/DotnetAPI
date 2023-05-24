using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Namespace
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetUsers()
        {
            return new string[] { "user1", "user2" };
        }

        [HttpGet("{id}")]
        public ActionResult<string> Get(int id, [FromQuery] int code)
        {
            Console.WriteLine(code);
            return "User";
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
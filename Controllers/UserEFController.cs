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
    public class UserEFController : ControllerBase
    {
        DataContextEF _entityFramework;
        public UserEFController(IConfiguration config)
        {
            // * configuration object provides access to this from appsettings.json - unique to .NET 6+
            _entityFramework = new DataContextEF(config);
        }


        [HttpGet]
        // public ActionResult<IEnumerable<User>> GetUsers()
        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> users = _entityFramework.Users.ToList<User>();
            return users;
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            User? user = _entityFramework.Users
                .Where(u => u.UserId == id)
                .FirstOrDefault<User>();

            if (user != null)
            {
                return user;
            }

            throw new Exception("Could not find user");
        }

        [HttpPost]
        // * use UserDTO because we just need temporary mapping, not full User object (don't need ID)
        public IActionResult AddUser(UserDTO user)
        {
            User userDB = new User();

            // * can use automapper to streamline, but this works for now
            userDB.Active = user.Active;
            userDB.FirstName = user.FirstName;
            userDB.LastName = user.LastName;
            userDB.Email = user.Email;
            userDB.Email = user.Email;
            userDB.Gender = user.Gender;

            _entityFramework.Add(userDB);

            if (_entityFramework.SaveChanges() > 0)
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
            User? userDB = _entityFramework.Users
                             .Where(u => u.UserId == user.UserId)
                             .FirstOrDefault<User>();

            if (userDB != null)
            {
                userDB.Active = user.Active;
                userDB.FirstName = user.FirstName;
                userDB.LastName = user.LastName;
                userDB.Email = user.Email;
                userDB.Email = user.Email;
                userDB.Gender = user.Gender;

                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }

                throw new Exception("Unable to update user");
            }

            throw new Exception("Unable to find user");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            User? userDB = _entityFramework.Users
                            .Where(u => u.UserId == id)
                            .FirstOrDefault();

            if (userDB != null)
            {
                _entityFramework.Remove(userDB);

                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }

                throw new Exception("Unable to delete user");
            }

            throw new Exception("Unable to find user");
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
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
        IMapper _mapper;
        public UserEFController(IConfiguration config)
        {
            // * configuration object provides access to this from appsettings.json - unique to .NET 6+
            _entityFramework = new DataContextEF(config);
            _mapper = new Mapper(
                new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>())
            );
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
            User userDB = _mapper.Map<User>(user);

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
        public IActionResult EditUser(UserDTO user, int id)
        {
            User? userDB = _entityFramework.Users
                             .Where(u => u.UserId == id)
                             .FirstOrDefault<User>();

            if (userDB != null)
            {
                userDB.Active = user.Active;
                userDB.FirstName = user.FirstName;
                userDB.LastName = user.LastName;
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

        [HttpGet("jobinfo")]
        public IEnumerable<UserJobInfo> GetJobInfo()
        {
            IEnumerable<UserJobInfo> jobInfo = _entityFramework.UserJobInfo.ToList();

            return jobInfo;
        }

        [HttpGet("{id}/jobinfo")]
        public ActionResult<UserJobInfo> GetUserJobInfo(int id)
        {
            UserJobInfo? userJobInfo = _entityFramework.UserJobInfo
                                        .Where(uji => uji.UserId == id)
                                        .FirstOrDefault();

            if (userJobInfo != null)
            {
                return userJobInfo;
            }

            throw new Exception("Unable to find user job info");
        }

        [HttpPost("{id}/jobinfo")]
        public IActionResult AddUserJobInfo(int id, UserJobInfo jobInfo)
        {
            User? user = _entityFramework.Users
                            .Where(u => u.UserId == id)
                            .FirstOrDefault();
            UserJobInfo? userJobInfo = _entityFramework.UserJobInfo
                            .Where(uji => uji.UserId == id)
                            .FirstOrDefault();

            // * user exists but does not have job info
            if (userJobInfo == null && user != null)
            {
                jobInfo.UserId = id;

                _entityFramework.UserJobInfo.Add(jobInfo);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
            }

            string errMsg = user == null ? "User does not exist" : "This user already has job info";
            throw new Exception(errMsg);
        }

        [HttpPut("{id}/jobinfo")]
        public IActionResult EditUserJobInfo(int id, UserJobInfo jobInfo)
        {
            User? user = _entityFramework.Users
                            .Where(u => u.UserId == id)
                            .FirstOrDefault();
            UserJobInfo? userJobInfo = _entityFramework.UserJobInfo
                            .Where(uji => uji.UserId == id)
                            .FirstOrDefault();

            // * user exists and does have job info
            if (userJobInfo != null && user != null)
            {
                userJobInfo.JobTitle = jobInfo.JobTitle;
                userJobInfo.Department = jobInfo.Department;

                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                return StatusCode(500);
            }

            string errMsg = user == null ? "User does not exist" : "User does not have job info to edit";
            throw new Exception(errMsg);
        }
        [HttpDelete("{id}/jobinfo")]
        public IActionResult DeleteUserJobInfo(int id)
        {
            UserJobInfo? userJobInfo = _entityFramework.UserJobInfo
                                .Where(uji => uji.UserId == id)
                                .FirstOrDefault();

            if (userJobInfo != null)
            {
                _entityFramework.UserJobInfo.Remove(userJobInfo);

                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                return StatusCode(500);
            }

            throw new Exception("Unable to find matching job info");
        }

        [HttpGet("salary")]
        public IEnumerable<UserSalary> GetSalary()
        {
            IEnumerable<UserSalary> salary = _entityFramework.UserSalary.ToList();

            return salary;
        }

        [HttpGet("{id}/salary")]
        public ActionResult<UserSalary> GetUserSalary(int id)
        {
            UserSalary? userSalary = _entityFramework.UserSalary
                                        .Where(uji => uji.UserId == id)
                                        .FirstOrDefault();

            if (userSalary != null)
            {
                return userSalary;
            }

            throw new Exception("Unable to find user salary");
        }

        [HttpPost("{id}/salary")]
        public IActionResult AddUserSalary(int id, UserSalary salary)
        {
            User? user = _entityFramework.Users
                            .Where(u => u.UserId == id)
                            .FirstOrDefault();
            UserSalary? userSalary = _entityFramework.UserSalary
                            .Where(uji => uji.UserId == id)
                            .FirstOrDefault();

            // * user exists but does not have job info
            if (userSalary == null && user != null)
            {
                salary.UserId = id;

                _entityFramework.UserSalary.Add(salary);
                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                return StatusCode(500);
            }

            string errMsg = user == null ? "User does not exist" : "This user already has salary";
            throw new Exception(errMsg);
        }

        [HttpPut("{id}/salary")]
        public IActionResult EditUserSalary(int id, UserSalary salary)
        {
            User? user = _entityFramework.Users
                            .Where(u => u.UserId == id)
                            .FirstOrDefault();
            UserSalary? userSalary = _entityFramework.UserSalary
                            .Where(uji => uji.UserId == id)
                            .FirstOrDefault();

            // * user exists and does have job info
            if (userSalary != null && user != null)
            {
                userSalary.Salary = salary.Salary;

                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                return StatusCode(500);
            }

            string errMsg = user == null ? "User does not exist" : "User does not have salary to edit";
            throw new Exception(errMsg);
        }
        [HttpDelete("{id}/salary")]
        public IActionResult DeleteUserSalary(int id)
        {
            UserSalary? userSalary = _entityFramework.UserSalary
                                .Where(uji => uji.UserId == id)
                                .FirstOrDefault();

            if (userSalary != null)
            {
                _entityFramework.UserSalary.Remove(userSalary);

                if (_entityFramework.SaveChanges() > 0)
                {
                    return Ok();
                }
                return StatusCode(500);
            }

            throw new Exception("Unable to find matching salary");
        }
    }
}
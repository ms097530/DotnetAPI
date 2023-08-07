using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DotnetAPI;
using DotnetAPI.Data;
using DotnetAPI.DTOs;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

// ? good practice to use sub-namespaces so things can be loaded separately/modularly
namespace DotnetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserEFController : ControllerBase
    {
        // * access setup in Program as a scoped service
        IUserRepository _userRepository;
        IMapper _mapper;
        public UserEFController(IConfiguration config, IUserRepository userRepository)
        {
            _userRepository = userRepository;

            _mapper = new Mapper(
                new MapperConfiguration(cfg => cfg.CreateMap<UserDTO, User>())
            );
        }


        [HttpGet]
        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> users = _userRepository.GetUsers();
            return users;
        }

        [HttpGet("{id}")]
        public ActionResult<User> GetUser(int id)
        {
            return _userRepository.GetUser(id);
        }

        [HttpPost]
        // * use UserDTO because we just need temporary mapping, not full User object (don't need ID)
        public IActionResult AddUser(UserDTO user)
        {
            User userDB = _mapper.Map<User>(user);

            // * implicitly getting <User> for template
            _userRepository.AddEntity(userDB);

            if (_userRepository.SaveChanges())
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
            User userDB = _userRepository.GetUser(id);

            if (userDB != null)
            {
                userDB.Active = user.Active;
                userDB.FirstName = user.FirstName;
                userDB.LastName = user.LastName;
                userDB.Email = user.Email;
                userDB.Gender = user.Gender;

                if (_userRepository.SaveChanges())
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
            User userDB = _userRepository.GetUser(id);

            if (userDB != null)
            {
                _userRepository.RemoveEntity(userDB);

                if (_userRepository.SaveChanges())
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
            return _userRepository.GetUserJobInfos();
        }

        [HttpGet("{id}/jobinfo")]
        public ActionResult<UserJobInfo> GetUserJobInfo(int id)
        {
            return _userRepository.GetUserJobInfo(id);
        }

        [HttpPost("{id}/jobinfo")]
        public IActionResult AddUserJobInfo(int id, UserJobInfo jobInfo)
        {
            jobInfo.UserId = id;
            _userRepository.AddEntity(jobInfo);

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }

            throw new Exception("Unable to add user job info");
        }

        [HttpPut("{id}/jobinfo")]
        public IActionResult EditUserJobInfo(int id, UserJobInfo jobInfo)
        {
            UserJobInfo userJobInfo = _userRepository.GetUserJobInfo(id);

            userJobInfo.JobTitle = jobInfo.JobTitle;
            userJobInfo.Department = jobInfo.Department;

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }

            throw new Exception("Unable to edit user job info");
        }
        [HttpDelete("{id}/jobinfo")]
        public IActionResult DeleteUserJobInfo(int id)
        {
            UserJobInfo userJobInfo = _userRepository.GetUserJobInfo(id);


            _userRepository.RemoveEntity(userJobInfo);

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }


            throw new Exception("Unable to find matching job info");
        }

        [HttpGet("salary")]
        public IEnumerable<UserSalary> GetSalary()
        {
            IEnumerable<UserSalary> salary = _userRepository.GetUserSalaries();

            return salary;
        }

        [HttpGet("{id}/salary")]
        public ActionResult<UserSalary> GetUserSalary(int id)
        {
            return _userRepository.GetUserSalary(id);
        }

        [HttpPost("{id}/salary")]
        public IActionResult AddUserSalary(int id, UserSalary salary)
        {
            salary.UserId = id;
            _userRepository.AddEntity(salary);

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }
            return StatusCode(500);

            throw new Exception("Unable to add user salary");
        }

        [HttpPut("{id}/salary")]
        public IActionResult EditUserSalary(int id, UserSalary salary)
        {
            UserSalary userSalary = _userRepository.GetUserSalary(id);

            userSalary.Salary = salary.Salary;

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }

            throw new Exception("Unable to update user salary");
        }
        [HttpDelete("{id}/salary")]
        public IActionResult DeleteUserSalary(int id)
        {
            UserSalary userSalary = _userRepository.GetUserSalary(id);
            _userRepository.RemoveEntity(userSalary);

            if (_userRepository.SaveChanges())
            {
                return Ok();
            }


            throw new Exception("Unable to find matching salary");
        }
    }
}
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Data
{
    public class UserRepository : IUserRepository
    {
        DataContextEF _entityFramework;
        public UserRepository(IConfiguration config)
        {
            _entityFramework = new DataContextEF(config);

        }

        public bool SaveChanges()
        {
            return _entityFramework.SaveChanges() > 0;
        }

        public void AddEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                _entityFramework.Add(entityToAdd);
            }
        }

        // * alternative that returns whether or not add was successful
        // public bool AddEntity<T>(T entityToAdd)
        // {
        //     if (entityToAdd != null)
        //     {
        //         _entityFramework.Add(entityToAdd);
        //         return true;
        //     }
        //     return false;
        // }

        public void RemoveEntity<T>(T entityToAdd)
        {
            if (entityToAdd != null)
            {
                // * don't need _entity.Framework.Users.Remove(entityToRemove) -> can use generically like below
                _entityFramework.Remove(entityToAdd);
            }
        }

        public IEnumerable<User> GetUsers()
        {
            IEnumerable<User> users = _entityFramework.Users.ToList<User>();
            return users;
        }

        public User GetUser(int id)
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

        public UserSalary GetUserSalary(int id)
        {
            UserSalary? userSalary = _entityFramework.UserSalary
                                        .Where(usal => usal.UserId == id)
                                        .FirstOrDefault();

            if (userSalary != null)
            {
                return userSalary;
            }

            throw new Exception("Unable to find user salary");
        }

        public UserJobInfo GetUserJobInfo(int id)
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
    }
}
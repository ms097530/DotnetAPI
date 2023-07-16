using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Data
{
    public interface IUserRepository
    {
        public bool SaveChanges();
        public void AddEntity<T>(T entityToAdd);
        public void RemoveEntity<T>(T entityToRemove);
        public IEnumerable<User> GetUsers();
        public User GetUser(int id);
        public IEnumerable<UserJobInfo> GetUserJobInfos();
        public UserJobInfo GetUserJobInfo(int id);
        public IEnumerable<UserSalary> GetUserSalaries();
        public UserSalary GetUserSalary(int id);
    }
}
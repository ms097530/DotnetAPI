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
    }
}
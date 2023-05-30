using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

// ? good practice to use sub-namespaces so things can be loaded separately/modularly
namespace DotnetAPI.Data
{
    class DataContextDapper
    {
        private readonly IConfiguration _config;
        public DataContextDapper(IConfiguration config)
        {
            // can access connection string from config in appsettings provided by .NET
            _config = config;
        }

        public IEnumerable<T> LoadData<T>(string sql)
        // sql 
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql);
        }

        public T LoadDataSingle<T>(string sql)
        // sql 
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql);
        }

        public bool ExecuteSql(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            // * execute will return number of rows affected, if > 0 it was at least partially successful
            return dbConnection.Execute(sql) > 0;
        }

        public int ExecuteSqlWithRowCount(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            // * execute will return number of rows affected, if > 0 it was at least partially successful
            return dbConnection.Execute(sql);
        }
    }
}
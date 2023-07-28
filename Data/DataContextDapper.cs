using System.Data.Common;
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
            // * returns type T or null
            return dbConnection.QuerySingleOrDefault<T>(sql);
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

        public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> parameters)
        {
            // * since using parameters, have to create SqlCommand and use that
            SqlCommand commandWithParams = new SqlCommand(sql);
            foreach (SqlParameter param in parameters)
            {
                commandWithParams.Parameters.Add(param);
            }

            // * implicitly converting SqlConnection to IDbConnection before -> change to SqlConnection so can set connection of SqlCommand
            SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            dbConnection.Open();
            // * SqlCommand.Connection must be of type SqlConnection -> IDbConnection could be different type of connection, so, even if dbConnection was SqlConnection under the hood, must change to SqlConnection to use SqlConnection specific functionality and so SqlCommand knows type is okay
            commandWithParams.Connection = dbConnection;

            int rowsAffected = commandWithParams.ExecuteNonQuery();

            dbConnection.Close();

            return rowsAffected > 0;
        }
    }
}
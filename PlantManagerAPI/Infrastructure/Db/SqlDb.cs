using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace PlantManagerAPI.Infrastructure.Db
{
    public class SqlDb : IDataAccess
    {
        private readonly IConfiguration _configuration;

        private readonly string _cnnString;

        public SqlDb(IConfiguration configuration)
        {
            _configuration = configuration;
            _cnnString = _configuration.GetConnectionString(ConnectionStringData.SqlConnectionName);
        }


        public async Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters)
        {

            using (IDbConnection connection = new SqlConnection(_cnnString))
            {
                var rows = await connection.QueryAsync<T>(storedProcedure,
                                                          parameters,
                                                          commandType: CommandType.StoredProcedure);
                return rows.ToList();
            }
        }


        public async Task<int> SaveData<T>(string storedProcedure, T parameters)
        {
            using (IDbConnection connection = new SqlConnection(_cnnString))
            {
                return await connection.ExecuteAsync(storedProcedure,
                                                     parameters,
                                                     commandType: CommandType.StoredProcedure);
            }
        }
    }
}

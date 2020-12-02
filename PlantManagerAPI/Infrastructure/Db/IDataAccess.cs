using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantManagerAPI.Infrastructure.Db
{
    public interface IDataAccess
    {
        //Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName);
        //Task<int> SaveData<T>(string storedProcedure, T parameters, string connectionStringName);

        Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters);
        Task<int> SaveData<T>(string storedProcedure, T parameters);

    }
}

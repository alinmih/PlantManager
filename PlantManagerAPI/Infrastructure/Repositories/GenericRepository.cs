using PlantManagerAPI.Infrastructure.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantManagerAPI.Infrastructure.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T: class
    {
        private readonly IDataAccess _dataAccess;

        protected string InsertProcedure { get; set; }
        protected string ReadProcedure { get; set; }
        protected string UpdateProcedure { get; set; }
        protected string DeleteProcedure { get; set; }

        public GenericRepository(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public GenericRepository()
        {

        }

        public virtual async Task<int> InsertAsync(T entity)
        {
            return await _dataAccess.SaveData(InsertProcedure, entity);
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            await _dataAccess.SaveData(UpdateProcedure, entity);
            return entity;
        }

        public virtual async Task DeleteAsync(int id)
        {
            await _dataAccess.SaveData(DeleteProcedure, id);
        }
        public virtual async Task<T> GetOneAsync(int id)
        {
            var result = await _dataAccess.LoadData<T, int>(ReadProcedure, id);

            return result.FirstOrDefault();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dataAccess.LoadData<T, dynamic>(ReadProcedure, new { });
        }

        public virtual async Task<IEnumerable<T>> GetMultipleFilteredAsync(T entity)
        {
            return await _dataAccess.LoadData<T, dynamic>(ReadProcedure, entity);
        }
        public abstract Task<T> GetOneWithRelatedDataAsync(T entity);

        public abstract Task<IEnumerable<T>> GetMultipleWithRelatedDataAsync();


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlantManagerAPI.Infrastructure.Repositories
{
    public interface IGenericRepository<T>
    {
        /// <summary>
        /// Insert T data
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> InsertAsync(T entity);

        /// <summary>
        /// Update T data
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> UpdateAsync(T entity);

        /// <summary>
        /// Delete T data
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task DeleteAsync(int id);
        
        /// <summary>
        /// Get one item data 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> GetOneAsync(int id);

        /// <summary>
        /// Get all the T data
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Get all the T data
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<T>> GetMultipleFilteredAsync(T entity);

        /// <summary>
        /// Get one of the T data with related entities
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<T> GetOneWithRelatedDataAsync(T entity);

        /// <summary>
        /// Get all of the T data with related entities
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> GetMultipleWithRelatedDataAsync();
    }
}

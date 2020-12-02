using PlantManagerAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlantManagerAPI.Services.UserServices
{
    public interface IUserService
    {
        Task<User> Authenticate(string username, string password);
        Task<User> Create(User user, string password);
        Task Delete(int id);
        Task<IEnumerable<User>> GetAll();
        Task<User> GetById(int id);
        Task Update(User userParam, string password = null);
    }
}
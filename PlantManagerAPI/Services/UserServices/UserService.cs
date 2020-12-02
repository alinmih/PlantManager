using Dapper;
using PlantManagerAPI.Entities;
using PlantManagerAPI.Helpers;
using PlantManagerAPI.Infrastructure.Db;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;



namespace PlantManagerAPI.Services.UserServices
{

    public class UserService : IUserService
    {
        private readonly IDataAccess _dataAccess;

        public UserService(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public async Task<User> Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                return null;
            }

            //get user from db
            User user = await GetUserHelper(username);

            if (user == null)
            {
                return null;
            }


            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            return user;
        }


        public async Task<IEnumerable<User>> GetAll()
        {
            return await _dataAccess.LoadData<User, dynamic>("UserManagementSelect", new User());
        }

        public async Task<User> GetById(int id)
        {
            User user = await GetUserHelper(id);
            return user;
        }


        public async Task<User> Create(User user, string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new AppException("Password is required");
            }
            User findUser = await GetUserHelper(user.Username);

            if (findUser != null)
            {
                throw new AppException("Username \"" + user.Username + "\"is allready taken");
            }

            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            DynamicParameters p = new DynamicParameters();

            p.Add("FirstName", user.FirstName);
            p.Add("LastName", user.LastName);
            p.Add("Username", user.Username);
            p.Add("PasswordHash", user.PasswordHash);
            p.Add("PasswordSalt", user.PasswordSalt);
            p.Add("Id", DbType.Int32, direction: ParameterDirection.Output);

            await _dataAccess.SaveData("UserManagementInsert", user);
            user.Id = p.Get<int>("Id");

            return user;
        }

        public async Task Update(User userParam, string password = null)
        {
            var user = await GetUserHelper(userParam.Id);

            if (user == null)
            {
                throw new AppException("User not found");
            }

            //update username if has changed
            if (!string.IsNullOrWhiteSpace(userParam.Username) && userParam.Username != user.Username)
            {
                //throw error if the new user is taken
                if (user.Username == userParam.Username)
                {
                    throw new AppException("Username " + userParam.Username + " is already taken");
                }
                user.Username = userParam.Username;
            }

            //update user props if provided
            if (!string.IsNullOrWhiteSpace(userParam.FirstName))
            {
                user.FirstName = userParam.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(userParam.LastName))
            {
                user.LastName = userParam.LastName;
            }

            //update password if provided
            if (!string.IsNullOrWhiteSpace(password))
            {
                byte[] passwordHash, passwordSalt;
                CreatePasswordHash(password, out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }

            await _dataAccess.SaveData("UserManagementUpdate", user);

        }

        public async Task Delete(int id)
        {
            var user = await GetUserHelper(id);
            if (user != null)
            {
                await _dataAccess.SaveData("UserManagementDelete",
                                        new { Id = id });
            }
        }

        private async Task<User> GetUserHelper(string username)
        {
            var users = await _dataAccess.LoadData<User, dynamic>("UserManagementSelect", new User(){ Username = username });
            var user = users.FirstOrDefault();
            return user;
        }

        private async Task<User> GetUserHelper(int id)
        {
            var users = await _dataAccess.LoadData<User, dynamic>("UserManagementSelect", new User() { Id=id});
            var user = users.FirstOrDefault();
            return user;
        }

        //private helper methods
        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null)
            {
                throw new ArgumentNullException("password");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null)
            {
                throw new ArgumentException("password");
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            }
            if (storedHash.Length != 64)
            {
                throw new ArgumentException("Invalid lenght of password hash(64 bytes expected)", "passwordHash");
            }
            if (storedSalt.Length != 128)
            {
                throw new ArgumentException("Invalid lenght of password salt(128 bytes expected)", "passwordSalt");
            }

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}

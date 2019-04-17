using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HabFitAPI.Contract;
using HabFitAPI.Data;
using HabFitAPI.Entities;

namespace HabFitAPI.Business
{
    public class UserRepository : IUserRepository
    {
        private readonly UserData _userData;

        public UserRepository(UserData habfitData)
        {
            _userData = habfitData;
        }

        public void AddPhoto(Photo photo)
        {
            _userData.AddPhoto(photo);
        }

        public void AddUser(Users user)
        {
            _userData.AddUser(user);
        }

        public void DeletePhoto(string photoID)
        {
            _userData.DeletePhoto(photoID);
        }

        public void DeleteUser(string userID)
        {
            _userData.DeleteUser(userID);
        }

        public Task<Users> GetUser(string userID)
        {
            return _userData.GetUser(userID);
        }

        public Task<IEnumerable<Users>> GetUsers()
        {
            return _userData.GetUsers();
        }

        public Task<bool> SaveAll(string id, Users user)
        {
            return _userData.SaveAll(id, user);
        }
    }
}

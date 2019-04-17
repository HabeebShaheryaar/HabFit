using HabFitAPI.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HabFitAPI.Contract
{
    public interface IUserRepository
    {
        void AddUser(Users user);
        void DeleteUser(string userID);
        void AddPhoto(Photo photo);
        void DeletePhoto(string photoID);
        Task<bool> SaveAll(string id, Users user);
        Task<IEnumerable<Users>> GetUsers();
        Task<Users> GetUser(string userID);
    }
}

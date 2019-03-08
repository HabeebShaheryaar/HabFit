using HabFitAPI.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HabFitAPI.Contract
{
    public interface IHabFitContract
    {
        Task<Users> Register(Users user, string password);
        Task<Users> Login(string username, string password);
        Task<bool> UserExists(string username);
        Task<List<Users>> GetUsers();
    }
}

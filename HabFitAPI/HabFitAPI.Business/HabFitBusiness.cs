using HabFitAPI.Contract;
using HabFitAPI.Data;
using HabFitAPI.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HabFitAPI.Business
{
    public class HabFitBusiness : IHabFitContract
    {
        private readonly HabFitData _habfitData;

        public HabFitBusiness(HabFitData habfitData)
        {
            _habfitData = habfitData;
        }

        public async Task<List<Users>> GetUsers()
        {
            List<Users> lstUsers = new List<Users>();
            lstUsers = await _habfitData.GetUsers();
            return lstUsers;
        }

        public async Task<Users> Get(string id)
        {
            return await _habfitData.Get(id);
        }

        public async Task<Users> Register(Users user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            user = await _habfitData.Register(user);
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<Users> Login(string username, string password)
        {
            //Users users = new Users();
            var user = await _habfitData.Login(username);

            if (user == null)
                return null;

            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;
            
            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<bool> UserExists(string username)
        {
            if (await _habfitData.UserExists(username))
                return true;
            return false;
        }

        //public string AddDomain(Domains domains)
        //{
        //    Domains objDomain = new Domains();
        //    objDomain = _domainData.Create(domains);
        //    return objDomain.ID;
        //}
    }
}

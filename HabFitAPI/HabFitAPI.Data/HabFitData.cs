using HabFitAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HabFitAPI.Data
{
    public class HabFitData
    {
        //public DataContext(DbContextOptions<DbContext> options) : base(options) { }

        //public DbSet<Users> Users { get; set; }

        private readonly IMongoCollection<Users> _users;
        private readonly IMongoCollection<Photo> _photos;

        IConfiguration config { get; }

        public HabFitData(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("HabFitDB"));
            var database = client.GetDatabase("HabFitDB");
            _users = database.GetCollection<Users>("Users");
            _photos = database.GetCollection<Photo>("Photos");
        }

        public async Task<List<Users>> GetUsers()
        {
            return await _users.Find(user => true).ToListAsync();
        }

        public async Task<Users> Get(string id)
        {
            return await _users.Find<Users>(user => user.ID == id).FirstOrDefaultAsync();
        }

        public async Task<Users> Register(Users user)
        {
            await _users.InsertOneAsync(user);
            return user;
        }

        public Users Create(Users users)
        {
            _users.InsertOne(users);
            return users;
        }

        public void Update(string id, Users users)
        {
            _users.ReplaceOne(user => user.ID == id, users);
        }

        public void Remove(Users users)
        {
            _users.DeleteOne(user => user.ID == users.ID);
        }

        public void Remove(string id)
        {
            _users.DeleteOne(user => user.ID == id);
        }

        public async Task<Users> Login(string username)
        {
            var objUser = await _users.Find<Users>(user => user.UserName == username).FirstOrDefaultAsync();
            var objMainPhoto = _photos.AsQueryable<Photo>().Where(u => u.UserID == objUser.ID).Where(p => p.IsMain).FirstOrDefault();
            objUser.Photos = new List<Photo>();
            objUser.Photos.Add(objMainPhoto);
            return objUser;
        }

        public async Task<bool> UserExists(string username)
        {
            try
            {
                var cursor = await _users.FindAsync(x => x.UserName == username);
                IEnumerable<Users> lst = new List<Users>();

                while (await cursor.MoveNextAsync())
                {
                    lst = cursor.Current;

                    foreach (var item in lst)
                    {
                        if (!string.IsNullOrEmpty(item.ID))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

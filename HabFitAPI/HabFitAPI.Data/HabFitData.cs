using HabFitAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HabFitAPI.Data
{
    public class HabFitData
    {
        //public DataContext(DbContextOptions<DbContext> options) : base(options) { }

        //public DbSet<Users> Users { get; set; }

        private readonly IMongoCollection<Users> _users;

        IConfiguration config { get; }

        public HabFitData(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("HabFitDB"));
            var database = client.GetDatabase("HabFitDB");
            _users = database.GetCollection<Users>("Users");
        }

        public async Task<List<Users>> GetUsers()
        {
            return await _users.Find(user => true).ToListAsync();
        }

        public async Task<Users> Get(string id)
        {
            return await _users.Find<Users>(user => user.ID == id).FirstOrDefaultAsync();
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
    }
}

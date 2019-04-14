using HabFitAPI.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HabFitAPI.Data
{
    public class UserData
    {
        private readonly IMongoCollection<Users> _users;
        private readonly IMongoCollection<Photo> _photos;

        IConfiguration config { get; }

        public UserData(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("HabFitDB"));
            var database = client.GetDatabase("HabFitDB");
            _users = database.GetCollection<Users>("Users");
            _photos = database.GetCollection<Photo>("Photos");
        }

        public void AddUser(Users user)
        {
            _users.InsertOne(user);
        }

        public void AddPhoto(Photo photo)
        {
            _photos.InsertOne(photo);
        }

        public void DeletePhoto(string photoID)
        {
            _photos.DeleteOne(photo => photo.ID == photoID);
        }

        public void DeleteUser(string userID)
        {
            _users.DeleteOne(user => user.ID == userID);
        }

        public async Task<Users> GetUser(string id)
        {
            Users objUser = new Users();
            Photo objPhoto = new Photo();
            objUser = await _users.Find<Users>(user => user.ID == id).FirstOrDefaultAsync();

            if (objUser != null && !string.IsNullOrEmpty(objUser.ID))
            {
                objPhoto = await _photos.Find<Photo>(photo => photo.UserID == objUser.ID).FirstOrDefaultAsync();
                objUser.Photos = new List<Photo>();
                objUser.Photos.Add(objPhoto);
            }

            return objUser;
        }

        public async Task<IEnumerable<Users>> GetUsers()
        {
            Photo objPhoto = new Photo();
            IEnumerable<Users> lstUsers = await _users.Find(user => true).ToListAsync();

            foreach (var item in lstUsers)
            {
                if (item != null && !string.IsNullOrEmpty(item.ID))
                {
                    objPhoto = await _photos.Find<Photo>(photo => photo.UserID == item.ID).FirstOrDefaultAsync();
                    item.Photos = new List<Photo>();
                    item.Photos.Add(objPhoto);
                }
            }

            return lstUsers;
        }

        public Task<bool> SaveAll()
        {
            throw new NotImplementedException();
        }
    }
}

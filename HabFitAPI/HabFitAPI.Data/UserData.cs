using HabFitAPI.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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

        public Photo GetMainPhotoForUser(string userID)
        {
            var result = _photos.AsQueryable<Photo>().Where(u => u.UserID == userID).Where(p => p.IsMain).FirstOrDefault();
            return result;
                //Find<Photo>(u => u.UserID == userID).Where( .FirstOrDefaultAsync();
        }

        public void SetMainPhoto(Photo photoFromRepo, Photo currentMainPhoto)
        {
            try
            {
                var photoFromRepoResponse = _photos.ReplaceOne(doc => doc.ID == photoFromRepo.ID, photoFromRepo, new UpdateOptions { IsUpsert = true });
                var currentMainPhotoResponse = _photos.ReplaceOne(doc => doc.ID == currentMainPhoto.ID, currentMainPhoto, new UpdateOptions { IsUpsert = true });
                if (!(currentMainPhotoResponse.MatchedCount == 1 && currentMainPhotoResponse.ModifiedCount == 1) && !(currentMainPhotoResponse.MatchedCount == 1 && currentMainPhotoResponse.ModifiedCount == 1))
                    throw new Exception("Problem while setting up the Main Photo");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Photo> GetPhoto(string photoID)
        {
            Photo objPhoto = new Photo();
            objPhoto = await _photos.Find<Photo>(photo => photo.ID == photoID).FirstOrDefaultAsync();
            return objPhoto;
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
            List<Photo> lstPhotos = new List<Photo>();
            objUser = await _users.Find<Users>(user => user.ID == id).FirstOrDefaultAsync();

            if (objUser != null && !string.IsNullOrEmpty(objUser.ID))
            {
                lstPhotos = await _photos.Find<Photo>(photo => photo.UserID == objUser.ID).ToListAsync();
                objUser.Photos = lstPhotos;
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

        //public async Task<ReplaceOneResult> SaveUserAsync(Users entity) where T : class
        //{
        //    var result = await _users.ReplaceOneAsync(i => i.ID == entity.ID, entity, new UpdateOptions { IsUpsert = true });
        //    return result;
        //    // look at result.MatchedCount to see whether it was an insert or update.
        //}

        public async Task<bool> SaveAll(string id, Users user)
        {
            bool result = false;

            try
            {
                var response = await _users.ReplaceOneAsync(doc => doc.ID == id, user, new UpdateOptions { IsUpsert = true });
                if (response.MatchedCount == 1 && response.ModifiedCount == 1)
                    result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return result;
        }
    }
}

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
using HabFitAPI.Helpers;
using HabFit.Common.Helpers;

namespace HabFitAPI.Data
{
    public class UserData
    {
        private readonly IMongoCollection<Users> _users;
        private readonly IMongoCollection<Photo> _photos;
        private readonly IMongoCollection<Like> _likes;
        private readonly IMongoCollection<Message> _messages;

        IConfiguration config { get; }

        public UserData(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("HabFitDB"));
            var database = client.GetDatabase("HabFitDB");
            _users = database.GetCollection<Users>("Users");
            _photos = database.GetCollection<Photo>("Photos");
            _likes = database.GetCollection<Like>("Likes");
            _messages = database.GetCollection<Message>("Messages");
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

        public async Task<PagedList<Users>> GetUsers(UserParams userParams)
        {
            Photo objPhoto = new Photo();
            IEnumerable<Users> lstUsers = await _users.Find(user => true).ToListAsync();

            lstUsers = lstUsers.Where(u => u.ID != userParams.UserId);
            lstUsers = lstUsers.Where(u => u.Gender == userParams.Gender);
            lstUsers = lstUsers.OrderByDescending(u => u.LastActive);

            if (userParams.Likers)
            {
                List<Like> userLikers = this.GetUserLikes(userParams.UserId, userParams.Likers);

                List<Users> tempUsersList = new List<Users>();
                List<Users> tempUsersList2 = null;
                tempUsersList = lstUsers.ToList();

                List<Users> NewTempUsersList = new List<Users>();

                for (int i = 0; i < userLikers.Count; i++)
                {
                    if (tempUsersList.Exists(u => u.ID == userLikers[i].LikerID))
                    {
                        tempUsersList2 = new List<Users>();
                        tempUsersList2 = tempUsersList.Where(u => u.ID == userLikers[i].LikerID).ToList();

                        foreach (var tempUser in tempUsersList2)
                        {
                            NewTempUsersList.Add(tempUser);
                        }
                    }
                }

                lstUsers = NewTempUsersList.AsEnumerable();
            }

            if (userParams.Likees)
            {
                List<Like> userLikers = this.GetUserLikes(userParams.UserId, userParams.Likers);

                List<Users> tempUsersList = new List<Users>();
                List<Users> tempUsersList2 = null;
                tempUsersList = lstUsers.ToList();

                List<Users> NewTempUsersList = new List<Users>();

                for (int i = 0; i < userLikers.Count; i++)
                {
                    if (tempUsersList.Exists(u => u.ID == userLikers[i].LikeeID))
                    {
                        tempUsersList2 = new List<Users>();
                        tempUsersList2 = tempUsersList.Where(u => u.ID == userLikers[i].LikeeID).ToList();

                        foreach (var tempUser in tempUsersList2)
                        {
                            NewTempUsersList.Add(tempUser);
                        }
                    }
                }

                lstUsers = NewTempUsersList.AsEnumerable();
            }

            if (userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDOB = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                var maxDOB = DateTime.Today.AddYears(-userParams.MinAge);

                lstUsers = lstUsers.Where(u => u.DateOfBirth >= minDOB && u.DateOfBirth <= maxDOB);
            }

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch (userParams.OrderBy)
                {
                    case "created":
                        lstUsers = lstUsers.OrderByDescending(u => u.Created);
                        break;
                    default:
                        lstUsers = lstUsers.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            foreach (var item in lstUsers)
            {
                if (item != null && !string.IsNullOrEmpty(item.ID))
                {
                    objPhoto = await _photos.Find<Photo>(photo => photo.UserID == item.ID).FirstOrDefaultAsync();
                    item.Photos = new List<Photo>();
                    item.Photos.Add(objPhoto);
                }
            }

            return PagedList<Users>.Create(lstUsers.AsQueryable(), userParams.PageNumber, userParams.PageSize);
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

        //Likes
        public async Task<Like> GetLike(string userID, string recipientID)
        {
            return await _likes.Find<Like>(l => l.LikerID == userID && l.LikeeID == recipientID).FirstOrDefaultAsync();
        }

        public void LikeUser(Like like)
        {
            _likes.InsertOneAsync(like);
        }

        private List<Like> GetUserLikes(string userID, bool likers)
        {
            if (likers) //get the list if users who liked the currently logged in User
            {
                //this list object contains the list of users which currently logged In User has liked
                List<Like> lstLikers = _likes.Find(u => u.LikeeID == userID).ToList();
                return lstLikers;
            }
            else
            {
                //this list object contains the list of users which have liked the currently logged In User
                List<Like> lstLikees = _likes.Find(u => u.LikerID == userID).ToList();
                return lstLikees;
            }
        }

        //Messages
        public async Task<Message> GetMessage(string ID)
        {
            return await _messages.Find<Message>(m => m.ID == ID).FirstOrDefaultAsync();
        }

        public void CreateMessage(Message message)
        {
            _messages.InsertOneAsync(message);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            //if we would have used Entity Framework the couuld have been like below
            //var messages = _messages
            //    .Include(u => u.Sender).ThenInclude(p => p.Photos)
            //    .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            //    .AsQueryable();

            //but as we aren't using it, the code became so big. How we can optimize it???

            #region Code to optimize

            var messages = await _messages.Find(m => true).ToListAsync();

            foreach (var item in messages)
            {
                var senderID = item.SenderID;
                var recipientID = item.RecipientID;

                item.Sender = new Users();
                item.Sender = await this.GetUser(senderID);

                item.Recipient = new Users();
                item.Recipient = await this.GetUser(recipientID);
            }

            #endregion

            switch (messageParams.MessageContainer)
            {
                case "Inbox":
                    messages = messages.Where(u => u.RecipientID == messageParams.UserId && u.RecipientDeleted == false).ToList();
                    break;

                case "Outbox":
                    messages = messages.Where(u => u.SenderID == messageParams.UserId && u.SenderDeleted == false).ToList();
                    break;

                default:
                    messages = messages.Where(u => u.RecipientID == messageParams.UserId && u.RecipientDeleted == false && u.IsRead == false).ToList();
                    break;
            }

            messages = messages.OrderByDescending(d => d.MessageSent).ToList();
            return PagedList<Message>.Create(messages.AsQueryable(), messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessagesThread(string userID, string recipientID)
        {
            #region Code to optimize

            var messages = await _messages.Find(m => true).ToListAsync();

            messages = messages.Where(m => m.RecipientID == userID && m.RecipientDeleted == false && m.SenderID == recipientID
                                    || m.RecipientID == recipientID && m.SenderID == userID && m.SenderDeleted == false)
                               .OrderByDescending(m => m.MessageSent)
                               .ToList();

            foreach (var item in messages)
            {
                var senderID = item.SenderID;
                var _recipientID = item.RecipientID;

                item.Sender = new Users();
                item.Sender = await this.GetUser(senderID);

                item.Recipient = new Users();
                item.Recipient = await this.GetUser(_recipientID);
            }
            #endregion

            return messages.AsEnumerable<Message>();
        }

        public string DeleteMessage(string ID)
        {
            string response = string.Empty;
            DeleteResult result = _messages.DeleteOne(mes => mes.ID == ID);

            if (result.DeletedCount == 1)
                response = "Deleted successfully";
            else
                response = "Error in deleting message";

            return response;
        }

        public async Task<bool> SaveAll(string id, Message message)
        {
            bool result = false;

            try
            {
                var response = await _messages.ReplaceOneAsync(doc => doc.ID == id, message, new UpdateOptions { IsUpsert = true });
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

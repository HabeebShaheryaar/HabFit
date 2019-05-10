using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HabFitAPI.Contract;
using HabFitAPI.Data;
using HabFitAPI.Entities;
using HabFitAPI.Helpers;

namespace HabFitAPI.Business
{
    public class UserRepository : IUserRepository
    {
        private readonly UserData _userData;

        public UserRepository(UserData habfitData)
        {
            _userData = habfitData;
        }

        //Users
        public void AddUser(Users user)
        {
            _userData.AddUser(user);
        }

        public void DeleteUser(string userID)
        {
            _userData.DeleteUser(userID);
        }

        public Task<Users> GetUser(string userID)
        {
            return _userData.GetUser(userID);
        }

        public Task<PagedList<Users>> GetUsers(UserParams userParams)
        {
            return _userData.GetUsers(userParams);
        }

        public Task<bool> SaveAll(string id, Users user)
        {
            return _userData.SaveAll(id, user);
        }

        //Photos

        public void SetMainPhoto(Photo photoFromRepo, Photo currentMainPhoto)
        {
            _userData.SetMainPhoto(photoFromRepo, currentMainPhoto);
        }

        public Photo GetMainPhotoForUser(string userID)
        {
            return _userData.GetMainPhotoForUser(userID);
        }

        public Task<Photo> GetPhoto(string userID)
        {
            return _userData.GetPhoto(userID);
        }

        public void AddPhoto(Photo photo)
        {
            _userData.AddPhoto(photo);
        }

        public void DeletePhoto(string photoID)
        {
            _userData.DeletePhoto(photoID);
        }

        //Likes

        public Task<Like> GetLike(string userID, string recipientID)
        {
            return _userData.GetLike(userID, recipientID);
        }

        public void LikeUser(Like like)
        {
            _userData.LikeUser(like);
        }
    }
}

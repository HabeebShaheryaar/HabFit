using HabFit.Common.Helpers;
using HabFitAPI.Entities;
using HabFitAPI.Helpers;
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
        Task<PagedList<Users>> GetUsers(UserParams userParams);
        Task<Users> GetUser(string userID);
        Task<Photo> GetPhoto(string userID);
        Photo GetMainPhotoForUser(string userID);
        void SetMainPhoto(Photo photoFromRepo, Photo currentMainPhoto);

        Task<Like> GetLike(string userID, string recipientID);

        void LikeUser(Like like);

        Task<Message> GetMessage(string ID);
        Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<Message>> GetMessagesThread(string userID, string recipientID);
        void CreateMessage(Message message);

        string DeleteMessage(string ID);

        Task<bool> SaveAll(string id, Message message);
    }
}

using AutoMapper;
using HabFit.Common.Helpers;
using HabFitAPI.Contract;
using HabFitAPI.DTOs;
using HabFitAPI.Entities;
using HabFitAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HabFitAPI.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/user/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IMapper _mapper;

        public MessagesController(IUserRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(string userId, string id)
        {
            if (userId != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo == null)
                return NotFound();

            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(string userId, [FromQuery]MessageParams messageParams)
        {
            if (userId != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            messageParams.UserId = userId;

            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);

            var messages = _mapper.Map<IEnumerable<MessageToReturnDTO>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(string userId, string recipientId)
        {
            if (userId != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            var messageFromRepo = await _repo.GetMessagesThread(userId, recipientId);

            var messageThread = _mapper.Map<IEnumerable<MessageToReturnDTO>>(messageFromRepo);

            return Ok(messageThread);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(string userId, MessageForCreationDTO messageForCreationDTO)
        {
            var sender = await _repo.GetUser(userId);

            if (sender.ID != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            messageForCreationDTO.SenderId = userId;

            var recipient = await _repo.GetUser(messageForCreationDTO.RecipientId);

            if (recipient == null)
                return BadRequest("Could not find the User");

            var message = _mapper.Map<Message>(messageForCreationDTO);

            try
            {
                _repo.CreateMessage(message);
                var messageToReturn = _mapper.Map<MessageToReturnDTO>(message);

                //
                var userToReturn = _mapper.Map<UserForDetailedDTO>(sender);
                messageToReturn.SenderKnownAs = userToReturn.KnownAs;
                messageToReturn.SenderPhotoUrl = userToReturn.PhotoUrl;

                userToReturn = _mapper.Map<UserForDetailedDTO>(recipient);

                messageToReturn.RecipientKnownAs = userToReturn.KnownAs;
                messageToReturn.RecipientPhotoUrl = userToReturn.PhotoUrl;

                //
                return CreatedAtRoute("GetMessage", new { id = message.ID }, messageToReturn);
            }
            catch (Exception ex)
            {
                throw new Exception("Creating the message failed on Save");
            }
        }

        /// <summary>
        /// This method deletes the messages of the conversation, IF AND ONLY IF both the users have opted to delete the message
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(string id, string userId)
        {
            string response = string.Empty;
            if (userId != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo.SenderID == userId)
                messageFromRepo.SenderDeleted = true;
            if (messageFromRepo.RecipientID == userId)
                messageFromRepo.RecipientDeleted = true;

            //write the logic below to update the Sender Deleted & RecipientDeleted, so that user1 deletes the msg 
            //it should not appear in his/her chat history
            //Then the delete functionality will work
            bool result = await _repo.SaveAll(id, messageFromRepo);

            if (result)
                return NoContent();


            if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
                response = _repo.DeleteMessage(messageFromRepo.ID);

            if (response == "Deleted successfully")
                return NoContent();

            throw new Exception("Error Deleting the message");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(string userId, string id)
        {
            if (userId != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            var message = await _repo.GetMessage(id);

            if (message.RecipientID != userId)
                return Unauthorized();

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            bool result = await _repo.SaveAll(id, message);

            return NoContent();
        }
    }
}

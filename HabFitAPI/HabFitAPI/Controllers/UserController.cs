using AutoMapper;
using HabFitAPI.Contract;
using HabFitAPI.DTOs;
using HabFitAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HabFit.Common.Helpers;
using HabFitAPI.Entities;

namespace HabFitAPI.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UserController(IUserRepository repo, IConfiguration config, IMapper mapper)
        {
            _repo = repo;
            _config = config;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userparams)
        {
            var currentUserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userFromRepo = await _repo.GetUser(currentUserID);
            userparams.UserId = currentUserID;

            if (string.IsNullOrEmpty(userparams.Gender))
            {
                userparams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }

            var users = await _repo.GetUsers(userparams);
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDTO>>(users);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _repo.GetUser(id);
            var userToReturn = _mapper.Map<UserForDetailedDTO>(user);
            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(string id, UserForUpdateDTO userForUpdateDTO)
        {
            if (id != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(id);

            _mapper.Map(userForUpdateDTO, userFromRepo);

            if (await _repo.SaveAll(id, userFromRepo))
                return NoContent();

            throw new Exception($"Updating user {id} failed on save");
        }

        [HttpPost("{id}/like/{recipientID}")]
        public async Task<IActionResult> LikeUser(string id, string recipientID)
        {
            if (id != User.FindFirst(ClaimTypes.NameIdentifier).Value)
                return Unauthorized();

            var like = await _repo.GetLike(id, recipientID);

            if (like != null)
                return BadRequest("You already like this User");

            if (await _repo.GetUser(recipientID) == null)
                return NotFound();

            like = new Like
            {
                LikerID = id,
                LikeeID = recipientID
            };

            try
            {
                _repo.LikeUser(like);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to like the User");
            }
        }
    }
}

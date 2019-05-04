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
        public async Task<IActionResult> GetUsers()
        {
            var users = await _repo.GetUsers();
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDTO>>(users);
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HabFitAPI.Business;
using HabFitAPI.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HabFitAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly HabFitBusiness _habFitBusiness;

        public AccountController(HabFitBusiness habFitBusiness)
        {
            _habFitBusiness = habFitBusiness;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            List<Users> lstUsers = new List<Users>();
            lstUsers = await _habFitBusiness.GetUsers();
            string res = JsonConvert.SerializeObject(lstUsers);
            return res;
        }

        // GET api/values
        //[HttpGet]
        //public async Task<string[]> GetUsers()
        //{
        //    List<Users> lstUsers = new List<Users>();
        //    lstUsers = await _habFitBusiness.GetUsers();
        //    return new string[] { "value1", "value2" };

        //    //return new string[] { "value1", "value34" };
        //}

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> Get(string id)
        {
            Users users = new Users();
            users = await _habFitBusiness.Get(id);
            return users.UserName;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
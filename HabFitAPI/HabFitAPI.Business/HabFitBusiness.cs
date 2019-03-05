using HabFitAPI.Data;
using HabFitAPI.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HabFitAPI.Business
{
    public class HabFitBusiness
    {
        private readonly HabFitData _habfitData;

        public HabFitBusiness(HabFitData habfitData)
        {
            _habfitData = habfitData;
        }

        public async Task<List<Users>> GetUsers()
        {
            List<Users> lstUsers = new List<Users>();
            lstUsers = await _habfitData.GetUsers();
            return lstUsers;
        }

        public async Task<Users> Get(string id)
        {
            return await _habfitData.Get(id);
        }

        //public string AddDomain(Domains domains)
        //{
        //    Domains objDomain = new Domains();
        //    objDomain = _domainData.Create(domains);
        //    return objDomain.ID;
        //}
    }
}

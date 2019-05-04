using HabFitAPI.Contract;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace HabFitAPI.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            var userID = resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await repo.GetUser(userID);
            user.LastActive = DateTime.Now;
            await repo.SaveAll(userID, user);

        }
    }
}
